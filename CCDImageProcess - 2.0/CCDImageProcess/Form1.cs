using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace CCDImageProcess
{
    public partial class Form1 : Form
    {
        private delegate void FlushClient();//代理

        public string imageName;
        public FileInfo dataFile_Size;
        public string dataFile_Name;
        public string dataFile_Path;
        public int start;
        public int end;
        public long data_length;
        public const long width = 512;
        public long height;
        public double theta;
        public double numXuanzhuanjiao = 1.000;
        public int numWaijin = 2048, numNeijin = 1000;
        public long size;
        public long size_standard;
        public long size_bmpstandard;
        public byte[] temp_read;
        public byte[] temp_write;
        public int circleNumH = 100;
        //更新进度列表
        private delegate void SetPos(int ipos);  //     定义一个代理，用于更新ProgressBar的值（Value）

        private void SetTextMessage(int ipos)
        {
            if (this.InvokeRequired)
            {
                SetPos setpos = new SetPos(SetTextMessage);
                this.Invoke(setpos, new object[] { ipos });
            }
            else
            {
                this.label1.Text = ipos.ToString() + "/100";
                this.progressBar1.Value = Convert.ToInt32(ipos);
            }
        }

        //    public color* buffer1;
        //    public color* buffer2;
        //    public color* buffer3;

        public struct color_RGB
        {
            public byte b, g, r;
        }
        //    public static List<color_RGB> buffer_NearstInsert;

        // public static color_RGB* buffer_NearstInsert;
        public color_RGB[] buffer_NearstInsert;//初始赋值
     

        public color_RGB[] buffer_standard;
        //    color_RGB[] buffer1;



        //public color* buffer5;


        //Form1类构造函数  定时器
        public Form1()
        {
            InitializeComponent();
            myTimer = new HiPerfTimer();
        }

        public double asin(double bizhi)
        {
            bizhi = Math.Atan(bizhi / Math.Sqrt(-bizhi * bizhi + 1));
            return bizhi;
        }

        public double acos(double bizhi)
        {
            bizhi = Math.Atan(-bizhi / Math.Sqrt(-bizhi * bizhi + 1)) + 2 * Math.Atan(1);
            return bizhi;
        }

        public int changWaiJin(int waijin)
        {
            ///////////////////////////////////////////
            waijin = (int)(((waijin - 1) / 2048 + 1) * 2048);
            return waijin;
        }

        public double base_F(double S)
        {
            if (System.Math.Abs(S) > 2)
            {
                return 0;
            }

            if (System.Math.Abs(S) <= 1)
            {
                return (1 - 2.5 * System.Math.Abs(S) * System.Math.Abs(S) + 1.5 * System.Math.Abs(S) * System.Math.Abs(S) * System.Math.Abs(S));
            }

            if ((System.Math.Abs(S) <= 2) && (System.Math.Abs(S) >= 1))
            {
                return (2 - 4 * System.Math.Abs(S) + 2.5 * System.Math.Abs(S) * System.Math.Abs(S) - 0.5 * System.Math.Abs(S) * System.Math.Abs(S) * System.Math.Abs(S));
            }
            else
                return 0;
        }

        public int F_X(double X, int f_X_1, int f_X0, int f_X1, int f_X2)
        {
            int X_1 = (int)(X) - 1;
            int X0 = (int)(X);
            int X1 = (int)(X) + 1;
            int X2 = (int)(X) + 2;

            double S_1 = X - X_1;
            double S0 = X - X0;
            double S1 = X - X1;
            double S2 = X - X2;

            int f_X = (int)(base_F(S_1) * f_X_1 + base_F(S0) * f_X0 + base_F(S1) * f_X1 + base_F(S2) * f_X2);
            return f_X;
        }

        public int changStartNum(int start, int end, long fileData_length)
        {
            int esnum = (end - start + 1) % 2048;
            if (esnum == 0)
            {
                //  start = start;
                //  end = end;
                return start;
            }
            else
            {
                if ((end - esnum) > start)
                {
                    //   start = start;
                    //   end = end - esnum;
                    return start;
                }
                else
                {
                    if ((end - esnum) + width <= fileData_length)
                    {
                        //   start = start;
                        //   end =(end - esnum) + 2048;
                        return start;
                    }
                    else
                    {
                        start = (int)((fileData_length) - width);
                        //   end = (int)(fileData_length);
                        return start;
                    }
                }
            }
        }


        static int changEndNum(int start, int end, long fileData_length)
        {
            int esnum = (int)((end - start + 1) % width);
            if (esnum == 0)
            {
                //  start = start;
                //  end = end;
                return end;
            }
            else
            {
                if ((end - esnum) > start)
                {
                    //   start = start;
                    end = end - esnum;
                    return end;
                }
                else
                {
                    if ((end - esnum) + width <= fileData_length)
                    {
                        //   start = start;
                        end = (int)((end - esnum) + width);
                        return end;
                    }
                    else
                    {
                        // start = (int)(fileData_length) - 2048;
                        end = (int)(fileData_length);
                        return end;
                    }
                }
            }
        }

        //public Image bmp8;
        private String curFileName;//图像文件名
        private System.Drawing.Bitmap originalBitmap;//定义了一个图像对象surBitmap

        private String dataFileName;
        private System.Drawing.Bitmap data_Bitmap;

        private System.Drawing.Bitmap bmp8;
        private System.Drawing.Bitmap bmp_standard;

        public String bmp_standardName;
        //private System.Drawing.Imaging.BitmapData curBitmap8;//定义了一个8位位图数据
        private HiPerfTimer myTimer;
        //  public OpenFileDialog opnDlg = new OpenFileDialog();


        /// <summary>
        /// “打开图片”按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void open_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnDlg = new OpenFileDialog();//创建一个打开文件对话框

            opnDlg.Filter = "所有图像文件 | *.bmp; *.pcx; *.png; *.jpg; *.gif;" +
                            "*.tif; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf|" +
                            "位图( *.bmp; *.jpg; *.png;...) | *.bmp; *.pcx; *.png; *.jpg; *.gif; *.tif; *.ico|" +
                            "矢量图( *.wmf; *.eps; *.emf;...) | *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf"; //打开文件的格式，打开文件对话框格式过滤器

            opnDlg.Title = "打开图像文件";//设置对话框标题
            opnDlg.ShowHelp = true;//启用对话框“帮助”按钮
            //如果对话框结果为“打开”，选定文件
            if (opnDlg.ShowDialog() == DialogResult.OK)
            {
                curFileName = opnDlg.FileName;//读取当前对话框文件名，并赋值给图像文件名
                try
                {
                    originalBitmap = (Bitmap)Image.FromFile(curFileName);
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);//跑出异常
                }
            }

            ///使打开的图像文件名直接显示在窗口栏///
            int index = opnDlg.FileName.LastIndexOf("\\") + 1;//取出打开文件的文件名索引值

            if (index > 0)//判断是否为空
            {
                this.Text = "线阵CCD采样图像处理——" + opnDlg.FileName.Substring(index);//将打开文件的文件名赋给当前的窗口名
                imageName = this.Text;
            }

            ///*************************///
            pictureBox1.Image = originalBitmap;//将图片显示在pictureBox1中
            ///*************************///

            Invalidate();//刷新显示，对窗口进行重新绘制，强行进行paint事件的处理程序
        }

        /// <summary>
        /// “保存图片”按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void save_Click(object sender, EventArgs e)
        {
            //如果没有创建图像，则退出
            if (originalBitmap == null)
            {
                return;
            }


            SaveFileDialog saveDlg = new SaveFileDialog();//创建一个保存对话框对象saveDlg
            saveDlg.Title = "保存为";//设置保存对话框标题为“保存为”
            saveDlg.OverwritePrompt = true;//改写已存在文件时提醒用户
            saveDlg.Filter =
            "BMP文件 (*.bmp) | *.bmp|" +
            "Gif文件 (*.gif) | *.gif|" +
            "JPEG文件 (*.jpg) | *.jpg|" +
            "PNG文件 (*.png) | *.png";//设置保存对话框的过滤器
            saveDlg.ShowHelp = true;//启用保存对话框的帮助按钮
            //如果选择好了格式点击确定，则保存图像
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveDlg.FileName;//将保存对话框中的文件名设置为保存后的文件名
                string strFilExtn = fileName.Remove(0, fileName.Length - 3);//获取用户选择的拓展用户名
                //保存文件的格式
                switch (strFilExtn)
                {
                    case "bmp":
                        originalBitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "jpg":
                        originalBitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "gif":
                        originalBitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case "tif":
                        originalBitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case "png":
                        originalBitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// “关闭”按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 绘制panel1伪彩图色条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = 20;//色条的宽度
            int height = 400;//色条的长度
            int y = 5;//色条的位置高度
            ColorMap cm = new ColorMap(64, 255);

            DrawColorBar(g, 0, y, width, height, cm, "Gray");//画色条ColorBar
            DrawScale(g, width, y - 1, height - 20, 0, 255);// 画刻度Scale
        }


        /// <summary>
        /// 画色条ColorBar方法
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="map"></param>
        /// <param name="str"></param>
        private void DrawColorBar(Graphics g, int x, int y, int width, int height, ColorMap map, string str)
        {
            int[,] cmap = new int[64, 4];
            switch (str)
            {
                case "Jet":
                    cmap = map.Jet();
                    break;
                case "Hot":
                    cmap = map.Hot();
                    break;
                case "Gray":
                    cmap = map.Gray();
                    break;
                case "Cool":
                    cmap = map.Cool();
                    break;
                case "Summer":
                    cmap = map.Summer();
                    break;
                case "Autumn":
                    cmap = map.Autumn();
                    break;
                case "Spring":
                    cmap = map.Spring();
                    break;
                case "Winter":
                    cmap = map.Winter();
                    break;
            }

            int ymin = 0;
            int ymax = 32;
            int dy = height / (ymax - ymin);
            int m = 64;

            for (int i = 0; i < 32; i++)
            {
                int colorIndex = (int)((i - ymin) * m / (ymax - ymin));
                SolidBrush aBrush = new SolidBrush(Color.FromArgb(
                    cmap[colorIndex, 0], cmap[colorIndex, 1],
                    cmap[colorIndex, 2], cmap[colorIndex, 3]));
                g.FillRectangle(aBrush, x, y + i * dy, width, dy);
            }
        }


        /// <summary>
        /// 画刻度Scale方法
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="heigt"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void DrawScale(Graphics g, int x, int y, double heigt, double min, double max)
        {
            Font aFont = new Font("Arial", 7, FontStyle.Bold);
            for (int i = 0; i <= 10; i++)
            {

                g.DrawString((min + ((double)i / 10) * (max - min)).ToString(), aFont, Brushes.Black, x, (int)(y + (heigt / 10) * i));
            }
        }


        /// <summary>
        /// 显示原始大图按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openOriginalImage_Click(object sender, EventArgs e)
        {

            if (originalBitmap != null)
            {

                ShowOriginalImage showOriImage = new ShowOriginalImage(originalBitmap);
                showOriImage.Text = imageName;

                showOriImage.ShowDialog();
                showOriImage.Close();
            }
        }

        private void matchProcessing_Click(object sender, EventArgs e)
        {
            int neijing = 0;
            if (originalBitmap != null)
            {
                // threedforcomput();
                myTimer.ClearTimer();
                myTimer.Start();

                neijing = int.Parse(textBox_NeiJin.Text);



                Color curColor;
                int ret = 0;
                int n = originalBitmap.Width;//n是列数，也即图片的宽度
                int m = originalBitmap.Height;//m是行数，也即图片的高度

                int x, y;
                int X, Y;
                double rad = 2 * Math.PI / m;

                //在原图中划定一块区域
                Rectangle rect = new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height);

                //picturebox2中的8位位图
                bmp8 = new Bitmap((originalBitmap.Width + neijing) * 2 + 1, (originalBitmap.Width + neijing) * 2 + 1, PixelFormat.Format24bppRgb);


                //设置初始图片背景
                for (X = 0; X < (n + neijing) * 2 + 1; X++)
                {
                    for (Y = 0; Y < (n + neijing) * 2 + 1; Y++)
                    {
                        //
                        //ret = (int)(curColor.R * 0.299 + curColor.G * 0.587 + curColor.B * 0.114);
                        bmp8.SetPixel(X, Y, Color.FromArgb(ret, ret, ret));

                    }
                }

                ////进行了优化,只适用于灰度图!!!
                for (y = 0; y < m; y++)
                {
                    for (x = 0; x < n; x++)
                    {
                        X = (int)(Math.Cos(rad * (y - 1)) * (x + neijing)) + originalBitmap.Width + neijing;
                        Y = (int)(Math.Sin(rad * (y - 1)) * (x + neijing)) + originalBitmap.Width + neijing;

                        curColor = originalBitmap.GetPixel(x, y);
                        ret = (int)(curColor.R);//只适用于灰度图!!!
                        bmp8.SetPixel(X, Y, Color.FromArgb(ret, ret, ret));
                    }
                }

                loaded = 1;
                myTimer.Stop();
               // pictureBox2.Image = bmp8;//将图片显示在pictureBox1中
                timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
                Invalidate();
            }
        }

        private void xuanzhuanpinjieprocess()
        {
            /*添加过程*/
            //while (true)
            //{
            //    this.timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
            //    Thread.Sleep(1000);
            //}
            //timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒"     
        }

        private void ThreadFunction()
        {
            if (this.timeBox.InvokeRequired)//等待异步
            {
                FlushClient fc = new FlushClient(ThreadFunction);
                this.Invoke(fc);//通过代理调用刷新方法
            }
            else
            {
                this.timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
            }
        }




        int loaded;

        //定义了一个线程thcaculate，线程调用函数为xuanzhuanpinjieprocess（）
        public void threedforcomput()
        {
            loaded = 0;
            // label2.Text = "加载中......      ";
            // thcaculate = new Thread(xuanzhuanpinjieprocess);
            Thread thcaculate = new Thread(xuanzhuanpinjieprocess);
            thcaculate.IsBackground = true;
            //thcaculate.BeginInvoke(null, null);
            thcaculate.Start();
            // timer1.Interval = 100;
            timer1.Start();

        }





        private void saveProcessImage_Click(object sender, EventArgs e)
        {
            //如果没有创建图像，则退出
            if (bmp8 == null)
            {
                return;
            }

            
            if (bmp_standard == null)
            {
                return;
            }


            SaveFileDialog saveDlg2 = new SaveFileDialog();//创建一个保存对话框对象saveDlg
            saveDlg2.Title = "保存为";//设置保存对话框标题为“保存为”
            saveDlg2.OverwritePrompt = true;//改写已存在文件时提醒用户
            saveDlg2.Filter =
            "BMP文件 (*.bmp) | *.bmp|" +
            "Gif文件 (*.gif) | *.gif|" +
            "JPEG文件 (*.jpg) | *.jpg|" +
            "PNG文件 (*.png) | *.png";//设置保存对话框的过滤器
            saveDlg2.ShowHelp = true;//启用保存对话框的帮助按钮
            //如果选择好了格式点击确定，则保存图像
            if (saveDlg2.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveDlg2.FileName;//将保存对话框中的文件名设置为保存后的文件名
                string strFilExtn = fileName.Remove(0, fileName.Length - 3);//获取用户选择的拓展用户名
                //保存文件的格式
                switch (strFilExtn)
                {
                    case "bmp":
                        bmp8.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "jpg":
                        bmp8.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "gif":
                        bmp8.Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case "tif":
                        bmp8.Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case "png":
                        bmp8.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
            }
            if (saveDlg2.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveDlg2.FileName;//将保存对话框中的文件名设置为保存后的文件名
                string strFilExtn = fileName.Remove(0, fileName.Length - 3);//获取用户选择的拓展用户名
                //保存文件的格式
                switch (strFilExtn)
                {
                    case "bmp":
                        bmp_standard.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "jpg":
                        bmp_standard.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "gif":
                        bmp_standard.Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case "tif":
                        bmp_standard.Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case "png":
                        bmp_standard.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
            }
        }




        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = 20;//色条的宽度
            int height = 400;//色条的长度
            int y = 5;//色条的位置高度
            ColorMap cm = new ColorMap(64, 255);

            DrawColorBar(g, 0, y, width, height, cm, "Gray");//画色条ColorBar
            DrawScale(g, width, y - 1, height - 20, 0, 255);// 画刻度Scale
        }




        private void timer1_Tick(object sender, EventArgs e)
        {
            if (loaded == 1)
            {
                // label2.Text = "";
                open.Enabled = true;
                save.Enabled = true;
                openOriginalImage.Enabled = true;
                close.Enabled = true;
                matchProcessing.Enabled = true;
                //button7.Enabled = true;
                saveProcessImage.Enabled = true;

                pictureBox1.Image = originalBitmap;
                pictureBox2.Image = bmp8;

                //draw_light_intensity();
                loaded = 0;

                timer1.Stop();

            }
        }



        private void showProcessedImage_Click(object sender, EventArgs e)
        {
            if (bmp8 != null)
            {
                ShowProcessedImage showProImage = new ShowProcessedImage(bmp8);
                showProImage.Text = imageName;
                showProImage.ShowDialog();
                showProImage.Close();
            }
        }




        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// “关闭”按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        public Bitmap AddHeader(byte[] imageDataDetails)
        {
            Bitmap bitmap = null;
            int length = imageDataDetails.GetLength(0);
            using (MemoryStream stream = new MemoryStream(length + 14))//为头腾出14个长度的空间
            {
                byte[] buffer = new byte[13];
                buffer[0] = 0x42;//Bitmap固定常数
                buffer[1] = 0x4d;//Bitmap固定常数
                stream.Write(buffer, 0, 2);//先写入头的前两个字节

                //把我们之前获得的数据流的长度转换成字节,
                //这个是用来告诉“头”我们的实际图像数据有多大
                byte[] bytes = BitConverter.GetBytes(length);
                stream.Write(bytes, 0, 4);//把这个长度写入头中去
                buffer[0] = 0;
                buffer[1] = 0;
                buffer[2] = 0;
                buffer[3] = 0;
                stream.Write(buffer, 0, 4);//在写入4个字节长度的数据到头中去
                int num2 = 0x36;//Bitmap固定常数
                bytes = BitConverter.GetBytes(num2);
                stream.Write(bytes, 0, 4);//在写入最后4个字节的长度
                stream.GetBuffer();
                stream.Write(imageDataDetails, 0, length);//把实际的图像数据全部追加到头的后面
                bitmap = new Bitmap(stream);//用内存流构造出一幅bitmap的图片
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                stream.Close();
                return bitmap;//最后就得到了我们想要的图片了
            }
        }



        byte[] byDataValue = new byte[200];
        char[] charDataValue = new char[200];
        int CYL = 10 * 1024;
        /// <summary>
        /// FilesStream读取用法
        /// </summary>
        private void FilesStreamReadFile()
        {
            try
            {
                FileStream fsFile = new FileStream(@"d:/log.cs", FileMode.Open);
                //文件指针移到文件的135个字节
                fsFile.Seek(135, SeekOrigin.Begin);
                //将接下来的字节读到Array中
                fsFile.Read(byDataValue, 0, 200);
            }
            catch (Exception e)
            {
                throw e;
            }
            //将字节转换成字符
            Decoder dc = Encoding.UTF8.GetDecoder();
            //字节数组转换成字符数组，便于显示
            dc.GetChars(byDataValue, 0, byDataValue.Length, charDataValue, 0);

        }


        string fileurl;
        FileInfo data;
        private void openFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnDlg = new OpenFileDialog();//创建一个打开文件对话框

            //  opnDlg.Filter = "数据文件"; //打开文件的格式，打开文件对话框格式过滤器

            opnDlg.Title = "打开数据文件";//设置对话框标题
            opnDlg.ShowHelp = true;//启用对话框“帮助”按钮

            //如果对话框结果为“打开”，选定文件
            if (opnDlg.ShowDialog() == DialogResult.OK)
            {
                dataFileName = opnDlg.FileName;//读取当前对话框文件名，并赋值给图像文件名
                //获取文件大小 opnDlg.FileName.Length;
                //获取文件路径   opnDlg.FileName
                dataFile_Name = Path.GetFileName(opnDlg.FileName);
                dataFile_Path = Path.GetDirectoryName(opnDlg.FileName);
                // string dataFile_Size = Size.
                dataFile_Size = new FileInfo(opnDlg.FileName);
                sizeBox_Byte.Text = ((double)(dataFile_Size.Length)).ToString() + "字节  = " + ((dataFile_Size.Length) / 1024).ToString() + "KB  = " + ((double)(dataFile_Size.Length) / 1024 / 1024).ToString("0.00") + "MB";
                dataFile_PathBox.Text = dataFile_Path + "   " + dataFile_Name;
                bmp_highBox.Text = ((int)(dataFile_Size.Length / 1024 / 0.5)).ToString();
                bmp_widthBox.Text = width.ToString();
                workTime_Box.Text = ((double)(dataFile_Size.Length / 1024 / 0.5) / CYL).ToString("0.000") ;
                SizeBox_bmpSize.Text = ((int)(dataFile_Size.Length * 3 + 54) / 1024 / 1024).ToString("0.00") + "  MB";

                // lbFileurl.Text = fileurl;
                FileInfo data = new FileInfo(dataFileName);
                tbStart.Text = "0";
                tbDest.Text = (data.Length).ToString();

                textBox_zhuansu.Text = "5";
                textBox_s_width.Text = "10240";
                //    textBox_BeginAngle.Text = "0";
                textBox_NeiJin.Text = "1000";
                textBox_WaiJin.Text = "2048";
                numNeijin = int.Parse(textBox_NeiJin.Text);
                numWaijin = int.Parse(textBox_WaiJin.Text);
                textBox_BeginAngle.Text = "0";
                textBox_EndAngle.Text = "360";
                label_beginendbyte.Text = "0 - " + (data.Length).ToString();
                textBox_setHeigth.Text = bmp_highBox.Text;
                textBox_setWidth.Text = bmp_widthBox.Text;

                start = int.Parse(tbStart.Text);
                end = int.Parse(tbDest.Text);
                // width = (long)(((end - start)) / width) + 1;
                height = (long)(((end - start)) / width) + 1;

                textBox_hangNum.Text = width.ToString();

                try
                {
                    //   data_Bitmap = (Bitmap)Image.FromFile(dataFileName);
                    // 选取头和尾将数据文件转换为图像, 将数据文件转换为图像

                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);//跑出异常
                }

            }

            ///使打开的图像文件名直接显示在窗口栏///
            int index = opnDlg.FileName.LastIndexOf("\\") + 1;//取出打开文件的文件名索引值

            if (index > 0)//判断是否为空
            {
                this.Text = "线阵CCD采样图像处理——" + opnDlg.FileName.Substring(index);//将打开文件的文件名赋给当前的窗口名
                imageName = this.Text;
            }
            /*
                Bitmap bmp = new Bitmap(700, 550);                      //改图只显示最近输入的700个点的数据曲线。
                Graphics graphics = Graphics.FromImage(bmp);
                SolidBrush brush1 = new SolidBrush(Color.FromArgb(236, 233, 216));
                graphics.FillRectangle(brush1, 0, 0, this.panel3.Width, this.panel3.Height);//Brushes.Sienna
                //DrawXYSys(new Point(orgx,orgy), 710, 500, graphics);//在图形上画出坐标系

                Graphics g = Graphics.FromHwnd(this.panel3.Handle);
                g.DrawImage(bmp, new Point(0, 0));//在内存里画完后显示在控件上，避免闪。
                // 保存到硬盘上
                bmp.Save("c:\\11.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);//指定图片格式   
                bmp.Dispose(); g.Dispose(); graphics.Dispose(); brush1.Dispose();//一定释放内存。
            */

            ///*************************///
            //   pictureBox1.Image = originalBitmap;//将图片显示在pictureBox1中
            ///*************************///

            //  Invalidate();//刷新显示，对窗口进行重新绘制，强行进行paint事件的处理程序
        }


        unsafe void SaveBmpInDlg(String dataFileName, int height, int width, char* data)
        {
            int size = width * height * 3 + 54;
            int[] head ={
                               0x4D42,(int)(size%0x10000),(int)(size/0x10000),0,0,0x36,0,0x28,
                               0,(int)(width%0x10000),(int)(width/0x10000),(int)(height%0x10000),
                               (int)(height/0x10000),0x10,0x18,0,0,0,0,0,0,0,0,0,0,0,0};
            FileStream fs_readData = new FileStream(dataFileName, FileMode.Open);
            FileStream fs_writeBMP = new FileStream(dataFileName + ".bmp", FileMode.Create);

            //if (!fs_readData) return;
            // fwrite(head, 1, sizeof(head), bmp);
            //byte[] b = new byte[data_length];
            // fs_readData.Read(b, 0, (int)(height * width));    ///////
            // fwrite(data, 1, size, bmp);
            // fclose(bmp);

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        unsafe private void button_createBMP_Click(object sender, EventArgs e)
        {
            myTimer.ClearTimer();
            myTimer.Start();

            Invalidate();//刷新显示，对窗口进行重新绘制，强行进行paint事件的处理程序
            start = int.Parse(tbStart.Text);
            end = int.Parse(tbDest.Text);


            double numEDITZhongzhijiao = double.Parse(textBox_EndAngle.Text);
            double numEDITChushijiao = double.Parse(textBox_BeginAngle.Text);
            numXuanzhuanjiao = (numEDITZhongzhijiao - numEDITChushijiao) / 360;
            int numEDITWidth = int.Parse(textBox_setWidth.Text);
            int numEDITHeigth = int.Parse(textBox_setHeigth.Text);


            start = changStartNum(start, end, dataFile_Size.Length);
            end = changEndNum(start, end, dataFile_Size.Length);
            tbStart.Text = (start).ToString();
            tbDest.Text = (end).ToString();
            data_length = end - start + 1;


            height = (long)(((end - start)) / width) + 1;
            //    long height = 1;
            textBox_setHeigth.Text = height.ToString();
            textBox_setWidth.Text = width.ToString();


            size = width * height * 3 + 54;

            color_RGB[] buffer0 = new color_RGB[height * width];

            FileStream fs1 = new FileStream(dataFileName, FileMode.Open);
            FileStream fs2 = new FileStream(dataFileName + ".bmp", FileMode.Create);




            int[] head ={0x4D42,(int)(size%0x10000),(int)(size/0x10000),0,0,0x36,0,0x28,
        0,(int)(width%0x10000),(int)(width/0x10000),(int)(height%0x10000),(int)(height/0x10000),0x10,0x18,0,
        0,0,0,0,0,0,0,0,0,0,0};

            byte[] head2 = new byte[head.Length * 2];
            for (int i = 0; i < head.Length; i++)
            {
                head2[i * 2] = (byte)(head[i] % 0x100);
                head2[i * 2 + 1] = (byte)(head[i] / 0x100);
            }
            fs2.Write(head2, 0, head2.Length);

            byte[] temp_read = new byte[data_length];
            byte[] temp_write = new byte[width * height * 3];
            fs1.Seek(start, SeekOrigin.Begin);
            fs1.Read(temp_read, 0, (int)(data_length));    ///////


       //     progressBar1.Maximum = (int)(width * height);
       //     progressBar1.Value = 0;
      //      progressBar1.Step = 1;  
      //         progressBar1.Value += progressBar1.Step;//让进度条增加一次
            for (long i = 0; i < width * height; i++)
            {
                long num = start + i;

                temp_write[i * 3] = temp_read[i];
                temp_write[i * 3 + 1] = temp_read[i];
                temp_write[i * 3 + 2] = temp_read[i];
                buffer0[i].r = temp_read[i];
                buffer0[i].g = temp_read[i];
                buffer0[i].b = temp_read[i];

            }
            fs2.Write(temp_write, 0, (int)(width * height * 3));

            fs1.Close();
            fs2.Close();
            fs1.Dispose();
            fs2.Dispose();

            originalBitmap = (Bitmap)Image.FromFile(dataFileName + ".bmp");
            pictureBox1.Image = originalBitmap;//将图片显示在pictureBox1中
            ///*************************///

            Invalidate();//刷新显示，对窗口进行重新绘制，强行进行paint事件的处理程序





            ///////////////////////////第二张图///////////////////////////
            buffer_standard = new color_RGB[numWaijin * numWaijin * 4];
            theta = Math.PI * 2 / height;
            FileStream fs_standard = new FileStream(dataFileName + "standard", FileMode.Create);
            FileStream fs_bmp_standard = new FileStream(dataFileName + "_standard.bmp", FileMode.Create);
            int[] head_bmpstandard ={
                                        0x4D42,(int)(size_bmpstandard%0x10000),(int)(size_bmpstandard/0x10000),0,0,0x36,0,0x28,
                                        0,(int)((numWaijin*2)%0x10000),(int)((numWaijin*2)/0x10000),(int)((numWaijin*2)%0x10000),(int)((numWaijin*2)/0x10000),
                                        0x10,0x18,0,0,0,0,0,0,0,0,0,0,0,0
                                    };

      //      progressBar1.Value =  (int)(width * height);
            for (int standard_num = 0; standard_num < numWaijin * numWaijin * 4; standard_num++)
            {
                buffer_standard[standard_num].b = buffer_standard[standard_num].g = buffer_standard[standard_num].r = 0;//算法在此处
            }
          

        

            progressBar1.Maximum = (int)(height);
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            for (int im = 0; im < height; im++)
            {
                for (int j = 0; j < width; j++)
                {
                    int x = (int)(numWaijin + ((j + numNeijin) * Math.Cos(im * theta * numXuanzhuanjiao)));
                    int y = (int)(numWaijin + ((j + numNeijin) * Math.Sin(im * theta * numXuanzhuanjiao)));
                    if (buffer_standard[x + y * numWaijin * 2].b == 0)
                    {
                        buffer_standard[x + y * numWaijin * 2].b = buffer_standard[x + y * numWaijin * 2].g
                        = buffer_standard[x + y * numWaijin * 2].r
                        = buffer0[im * width + j].r;
                    }
                    else
                    {
                        buffer_standard[x + y * numWaijin * 2].b = buffer_standard[x + y * numWaijin * 2].g
                        = buffer_standard[x + y * numWaijin * 2].r
                        = (byte)((buffer_standard[x + y * numWaijin * 2].r + buffer0[im * width + j].r) / 2);
                    }
                }
                progressBar1.Value += progressBar1.Step;//让进度条增加一次
            }


            numWaijin = int.Parse(textBox_WaiJin.Text);
            numWaijin = changWaiJin(numWaijin);
            textBox_WaiJin.Text = numWaijin.ToString();

            size_standard = numWaijin * numWaijin * 4;
            size_bmpstandard = numWaijin * numWaijin * 4 * 3 + 54;



       //     progressBar1.Maximum = (int)(height) + (int)(numWaijin * numWaijin * 4) + (int)(head_bmpstandard.Length) + (int)(numWaijin * numWaijin * 4); ;
       //     progressBar1.Step = 1;
       //     progressBar1.Value = (int)(height) ;
            byte[] standard_data = new byte[numWaijin * numWaijin * 4];
            for (long i = 0; i < numWaijin * numWaijin * 4; i++)
            {
                standard_data[i] = buffer_standard[i].r;
            }
            fs_standard.Write(standard_data, 0, (int)(numWaijin * numWaijin * 4));


       //     progressBar1.Maximum = (int)(height) + (int)(numWaijin * numWaijin * 4) + (int)(head_bmpstandard.Length) + (int)(numWaijin * numWaijin * 4); ;
       //     progressBar1.Step = 1;
       //     progressBar1.Value = (int)(height) + (int)(numWaijin * numWaijin * 4);
            byte[] head2_bmpstandard = new byte[head_bmpstandard.Length * 2];
            for (int i = 0; i < head_bmpstandard.Length; i++)
            {
                head2_bmpstandard[i * 2] = (byte)(head_bmpstandard[i] % 0x100);
                head2_bmpstandard[i * 2 + 1] = (byte)(head_bmpstandard[i] / 0x100);
            }
            fs_bmp_standard.Write(head2_bmpstandard, 0, head2.Length);


            progressBar1.Maximum = (int)(numWaijin * numWaijin * 4) ;
            progressBar1.Step = 1;
            progressBar1.Value =0 ;
            //     progressBar1.Value += progressBar1.Step;//让进度条增加一次
            byte[] standard_bmp_data = new byte[numWaijin * numWaijin * 4 * 3];
            for (long i = 0; i < numWaijin * numWaijin * 4; i++)
            {
                standard_bmp_data[i * 3] = buffer_standard[i].r;
                standard_bmp_data[i * 3 + 1] = buffer_standard[i].g;
                standard_bmp_data[i * 3 + 2] = buffer_standard[i].b;
           //     progressBar1.Value += progressBar1.Step;//让进度条增加一次
            }
            fs_bmp_standard.Write(standard_bmp_data, 0, (int)(numWaijin * numWaijin * 4 * 3));



            fs_standard.Close();
            fs_bmp_standard.Close();
            fs_standard.Dispose();
            fs_bmp_standard.Dispose();


            // FILE* bmp = fopen(filename, "wb");
            //FileStream fs1 = new FileStream(dataFileName, FileMode.Open);

            //if (!fs1) return;
            // byte[] head2 = new byte[head.Length * 2];
            // fs1.Write(head2,0,head.Length);
            // fwrite(data, 1, size, bmp);
            // fclose(bmp);

            /*int[] head ={0x4D42,(int)(size%0x10000),(int)(size/0x10000),0,0,0x36,0,0x28,
        0,(int)(width%0x10000),(int)(width/0x10000),(int)(height%0x10000),(int)(height/0x10000),0x10,0x18,0,
        0,0,0,0,0,0,0,0,0,0,0};
                        
             * int[] head = { 0x4D42,(int)(size%0x10000),(int)(size/0x10000),0,0,0x36,0,0x28,
        0,(int)(width%0x10000),(int)(width/0x10000),(int)(height%0x10000),(int)(height/0x10000),0x10,0x18,0,
		0,0,0,0,0,0,0,0,0,0,0};

             */
            /*
                     byte[] head2 = new byte[head.Length * 2];
                     for (int i = 0; i < head.Length; i++)
                     {
                         head2[i * 2] = (byte)(head[i] % 0x100);
                         head2[i * 2 + 1] = (byte)(head[i] / 0x100);
                     }
                     fs2.Write(head2, 0, head2.Length);
                     long data_length = data.Length;
                     byte[] file_data = new byte[data.Length];

                    // FileStream fsFile = new FileStream(@"d:/log.cs", FileMode.Open);
                     //文件指针移到文件的n个字节
                     fs1.Seek(start, SeekOrigin.Begin);
                     //将接下来的字节读到Array中
                     fs1.Read(file_data, 0, end - start+1);

            
                     charDataValue = "This is test string".ToCharArray();
                     byDataValue = new byte[charDataValue.Length];

                     //将字符数组转换成字节数组
                    // Encoder ec = Encoding.UTF8.GetEncoder();
                   //  ec.GetBytes(charDataValue, 0, charDataValue.Length, byDataValue, 0, true);

                     //将指针设定起始位置
                     fs2.Seek(0, SeekOrigin.Begin);
                     //写入文件
                     fs2.Write(file_data, 0, end - start + 1);

                     //fs2.Write(data, 0, head2.Length);

                      /*
                     byte[] temp = new byte[1024];
                     int count = 0;
                     int n = start;

                  //   bmp_widthBox.Text = (width).ToString();

                     do
                     {
                         count = fs1.Read(temp, 0, 1024);
                         fs2.Write(temp, 0, count);
                         n += count;
                     } while (count != 0 && n <= end);
                   fs1.Close();
                     fs2.Close();
                     fs1.Dispose();
                     fs2.Dispose();                    */


            /*      byte[]imageDataDetails  =new byte[];
                Bitmap bitmap = null;
                int length = imageDataDetails.GetLength(0);     */
            ////////////////////////////////////////////////////////////////////////////////////////

            // 

            try
            {


            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);//跑出异常
            }

           // dataFileName 
            //获取文件大小 opnDlg.FileName.Length;
            //获取文件路径   opnDlg.FileName

          //  dataFile_Path = Path.GetDirectoryName(opnDlg.FileName);

            bmp_standard = (Bitmap)Image.FromFile(dataFileName + "_standard.bmp");

            ///*************************///
            pictureBox2.Image = bmp_standard;//将图片显示在pictureBox1中
            ///*************************///

            Invalidate();//刷新显示，对窗口进行重新绘制，强行进行paint事件的处理程序



            ///////////////////////////////////////////////////////////////
            myTimer.Stop();
     
            timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
            Invalidate();

        }

        private void label_NeiJin_Click(object sender, EventArgs e)
        {

        }

        private void button_Average_Click(object sender, EventArgs e)
        {
            FileStream fs1_Average = new FileStream(dataFileName, FileMode.Open);
            FileStream fs2_Average = new FileStream(dataFileName + "_Average.bmp", FileMode.Create);
            fs1_Average.Seek(start, SeekOrigin.Begin);


            int[] head1_Average ={
                                          0x4D42,(int)(size%0x10000),(int)(size/0x10000),0,0,0x36,0,0x28,
                                          0,(int)((numWaijin*2)%0x10000),(int)((numWaijin*2)/0x10000),(int)((numWaijin*2)%0x10000),
                                          (int)((numWaijin*2)/0x10000),0x10,0x18,0,0,0,0,0,0,0,0,0,0,0,0
                                      };

            byte[] head2_Average = new byte[head1_Average.Length * 2];

            for (int i = 0; i < head1_Average.Length; i++)
            {
                head2_Average[i * 2] = (byte)(head1_Average[i] % 0x100);
                head2_Average[i * 2 + 1] = (byte)(head1_Average[i] / 0x100);
            }

            fs2_Average.Write(head2_Average, 0, head2_Average.Length);

            byte[] temp_read = new byte[data_length];
            fs1_Average.Seek(start, SeekOrigin.Begin);
            fs1_Average.Read(temp_read, 0, (int)(data_length));    ///////


            byte[] temp_write = new byte[numWaijin * numWaijin * 4 * 3];

            byte[] buffer0 = new byte[width * height];

            for (long i = 0; i < width * height; i++)
            {
                buffer0[i] = temp_read[i];
            }

            unsafe
            {

                myTimer.ClearTimer();
                myTimer.Start();
                theta = Math.PI * 2 / height;


                // color_RGB* buffer_Average;

                color_RGB[] buffer_Average = new color_RGB[numWaijin * numWaijin * 4];//初始赋值

                //  static List<color_RGB> buffer_Average;
                //  buffer_Average = new List<color_RGB>();
                //  buffer_Average=new List<color_RGB>(numWaijin*numWaijin*4);

                for (int buffer_Average_Num = 0; buffer_Average_Num < numWaijin * numWaijin * 4; buffer_Average_Num++)
                {
                    buffer_Average[buffer_Average_Num].b = buffer_Average[buffer_Average_Num].g = buffer_Average[buffer_Average_Num].r = buffer_standard[buffer_Average_Num].r;//算法在此处
                }

                int buffer1x, buffer1y, j = 0, suffer0H = 0;//buffer1x,buffer1y为生成图中的x,y
                double buffer0h, j0;
                double hudu;
                double bizhi;
                int x1, x2, y1, y2;
                int r1, r2;
                byte o11, o21, o12, o22;

                progressBar1.Maximum = (int)(numWaijin * 2);
                progressBar1.Step = 1;
                progressBar1.Value = 0;
                //     progressBar1.Value += progressBar1.Step;//让进度条增加一次
                for (buffer1x = 0; buffer1x < numWaijin * 2; buffer1x++)
                {
                    //m_Progress.SetPos(buffer1x*50/numWaijin);
                    for (buffer1y = 0; buffer1y < numWaijin * 2; buffer1y++)
                    {
                        if (
                            ((System.Math.Abs(buffer1x - numWaijin) * System.Math.Abs(buffer1x - numWaijin) + System.Math.Abs(buffer1y - numWaijin) * System.Math.Abs(buffer1y - numWaijin)) >= numNeijin * numNeijin) &&
                            ((System.Math.Abs(buffer1x - numWaijin) * System.Math.Abs(buffer1x - numWaijin) + System.Math.Abs(buffer1y - numWaijin) * System.Math.Abs(buffer1y - numWaijin)) <= (numNeijin + width) * (numNeijin + width))
                          )
                        {
                            if (buffer_standard[buffer1x + buffer1y * numWaijin * 2].r == 0)
                            {
                                if ((buffer1x >= numWaijin) && (buffer1x < numWaijin * 2) && (buffer1y >= numWaijin) && (buffer1y < numWaijin * 2))//第一象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(buffer1x - numWaijin) / (j + numNeijin);//注意数据类型的改变！
                                    hudu = acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(buffer1y - numWaijin) / (j + numNeijin);
                                        hudu = asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);

                                    //unsigned char t=buffer0[suffer0H*width+j].r;
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*width+j];						
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;

                                    /////最近四个点//////
                                    o11 = buffer0[y1 * width + x1];
                                    o12 = buffer0[y2 * width + x1];
                                    o21 = buffer0[y1 * width + x2];
                                    o22 = buffer0[y2 * width + x2];
    
                                    buffer_Average[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_Average[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_Average[buffer1x + buffer1y * numWaijin * 2].b
                                        = (byte)((o11+o12+o21+o22)/4);
                                }

                                if ((buffer1x >= 0) && (buffer1x < numWaijin) && (buffer1y >= numWaijin) && (buffer1y < numWaijin * 2))//第二象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(numWaijin - buffer1x) / (j + numNeijin);//注意数据类型的改变！
                                    hudu = Math.PI - acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(buffer1y - numWaijin) / (j + numNeijin);
                                        hudu = Math.PI - asin(bizhi);
                                    }

                                    //	hudu=Math.PI-acos(bizhi);
                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    //unsigned char t=buffer0[suffer0H*width+j].r;
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*width+j];						
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;


                                                                  /////最近四个点//////
                                    o11 = buffer0[y1 * width + x1];
                                    o12 = buffer0[y2 * width + x1];
                                    o21 = buffer0[y1 * width + x2];
                                    o22 = buffer0[y2 * width + x2];


                                        
                                    buffer_Average[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_Average[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_Average[buffer1x + buffer1y * numWaijin * 2].b
                                        = (byte)((o11+o12+o21+o22)/4);
                                }

                                if ((buffer1x >= 0) && (buffer1x < numWaijin) && (buffer1y >= 0) && (buffer1y < numWaijin))//第三象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(numWaijin - buffer1x) / (j + numNeijin);//注意数据类型的改变！
                                    hudu = Math.PI + acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(numWaijin - buffer1y) / (j + numNeijin);
                                        hudu = Math.PI + asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    //unsigned char t=buffer0[suffer0H*width+j].r;
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*width+j];						
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;
                                   
                                    /////最近四个点//////
                                    o11 = buffer0[y1 * width + x1];
                                    o12 = buffer0[y2 * width + x1];
                                    o21 = buffer0[y1 * width + x2];
                                    o22 = buffer0[y2 * width + x2];
        
                                    buffer_Average[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_Average[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_Average[buffer1x + buffer1y * numWaijin * 2].b
                                        = (byte)((o11+o12+o21+o22)/4);
                                }

                                if ((buffer1x >= numWaijin) && (buffer1x < numWaijin * 2) && (buffer1y >= 0) && (buffer1y < numWaijin))//第四象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(buffer1x - numWaijin) / (j + numNeijin);//注意数据类型的改变！！！！！！！！
                                    hudu = 2 * Math.PI - acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(numWaijin - buffer1y) / (j + numNeijin);
                                        hudu = 2 * Math.PI - asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    //unsigned char t=buffer0[suffer0H*width+j].r;
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*width+j];						
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;
                                    if ((x2 == 512) && (y2 == (height - 1)))
                                        x2 = x1;

                                
                                    /////最近四个点//////
                                    o11 = buffer0[y1 * width + x1];
                                    o12 = buffer0[y2 * width + x1];
                                    o21 = buffer0[y1 * width + x2];
                                    o22 = buffer0[y2 * width + x2];
    
                                    buffer_Average[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_Average[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_Average[buffer1x + buffer1y * numWaijin * 2].b
                                        = (byte)((o11+o12+o21+o22)/4);
                                }
                            }
                        }
                    }
                    progressBar1.Value += progressBar1.Step;//让进度条增加一次
                }

                for (long i = 0; i < numWaijin * numWaijin * 4; i++)
                {
                    temp_write[i * 3] = buffer_Average[i].r;
                    temp_write[i * 3 + 1] = buffer_Average[i].g;
                    temp_write[i * 3 + 2] = buffer_Average[i].b;
                    //  buffer0[i] = temp_read[i];
                }

                fs2_Average.Write(temp_write, 0, (int)(numWaijin * numWaijin * 4 * 3));

                fs1_Average.Close();
                fs2_Average.Close();
                fs1_Average.Dispose();
                fs2_Average.Dispose();


                //	SaveBmpInDlg("E:\\OutPut-最近邻法插值.bmp",numWaijin*2,numWaijin*2,(unsigned char*)buffer_Average);

                //delete [] buffer_Average;

                myTimer.Stop();
                // pictureBox2.Image = bmp8;//将图片显示在pictureBox1中
                timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
                Invalidate();
            }
        }

        private void button_CoordinateChange_Click(object sender, EventArgs e)
        {

        }


        //最邻近插值
        public void button_NearstInsert_Click(object sender, EventArgs e)
        {
            FileStream fs1_NearstInsert = new FileStream(dataFileName, FileMode.Open);
            FileStream fs2_NearstInsert = new FileStream(dataFileName + "_NearstInsert.bmp", FileMode.Create);
            fs1_NearstInsert.Seek(start, SeekOrigin.Begin);


            int[] head1_NearstInsert ={
                                          0x4D42,(int)(size%0x10000),(int)(size/0x10000),0,0,0x36,0,0x28,
                                          0,(int)((numWaijin*2)%0x10000),(int)((numWaijin*2)/0x10000),(int)((numWaijin*2)%0x10000),
                                          (int)((numWaijin*2)/0x10000),0x10,0x18,0,0,0,0,0,0,0,0,0,0,0,0
                                      };

            byte[] head2_NearstInsert = new byte[head1_NearstInsert.Length * 2];

            for (int i = 0; i < head1_NearstInsert.Length; i++)
            {
                head2_NearstInsert[i * 2] = (byte)(head1_NearstInsert[i] % 0x100);
                head2_NearstInsert[i * 2 + 1] = (byte)(head1_NearstInsert[i] / 0x100);
            }

            fs2_NearstInsert.Write(head2_NearstInsert, 0, head2_NearstInsert.Length);

            byte[] temp_read = new byte[data_length];
            fs1_NearstInsert.Seek(start, SeekOrigin.Begin);
            fs1_NearstInsert.Read(temp_read, 0, (int)(data_length));    ///////


            byte[] temp_write = new byte[numWaijin * numWaijin * 4 * 3];

            byte[] buffer0 = new byte[width * height];

            for (long i = 0; i < width * height; i++)
            {
                buffer0[i] = temp_read[i];
            }

            unsafe
            {

                myTimer.ClearTimer();
                myTimer.Start();
                theta = Math.PI * 2 / height;


                // color_RGB* buffer_NearstInsert;

                color_RGB[] buffer_NearstInsert = new color_RGB[numWaijin * numWaijin * 4];//初始赋值

                //  static List<color_RGB> buffer_NearstInsert;
                //  buffer_NearstInsert = new List<color_RGB>();
                //  buffer_NearstInsert=new List<color_RGB>(numWaijin*numWaijin*4);

                for (int buffer_NearstInsert_Num = 0; buffer_NearstInsert_Num < numWaijin * numWaijin * 4; buffer_NearstInsert_Num++)
                {
                    buffer_NearstInsert[buffer_NearstInsert_Num].b = buffer_NearstInsert[buffer_NearstInsert_Num].g = buffer_NearstInsert[buffer_NearstInsert_Num].r = buffer_standard[buffer_NearstInsert_Num].r;//算法在此处
                }

                int buffer1x, buffer1y, j = 0, suffer0H = 0;//buffer1x,buffer1y为生成图中的x,y
                double buffer0h, j0;
                double hudu;
                double bizhi;
                int x1, x2, y1, y2;
                int r1, r2;
                byte o11, o21, o12, o22;

                progressBar1.Maximum = (int)(numWaijin * 2);
                progressBar1.Step = 1;
                progressBar1.Value = 0;
                //     progressBar1.Value += progressBar1.Step;//让进度条增加一次
                for (buffer1x = 0; buffer1x < numWaijin * 2; buffer1x++)
                {
                    //m_Progress.SetPos(buffer1x*50/numWaijin);
                    for (buffer1y = 0; buffer1y < numWaijin * 2; buffer1y++)
                    {
                        if (
                            ((System.Math.Abs(buffer1x - numWaijin) * System.Math.Abs(buffer1x - numWaijin) + System.Math.Abs(buffer1y - numWaijin) * System.Math.Abs(buffer1y - numWaijin)) >= numNeijin * numNeijin) &&
                            ((System.Math.Abs(buffer1x - numWaijin) * System.Math.Abs(buffer1x - numWaijin) + System.Math.Abs(buffer1y - numWaijin) * System.Math.Abs(buffer1y - numWaijin)) <= (numNeijin + width) * (numNeijin + width))
                          )
                        {
                            if (buffer_standard[buffer1x + buffer1y * numWaijin * 2].r == 0)
                            {
                                if ((buffer1x >= numWaijin) && (buffer1x < numWaijin * 2) && (buffer1y >= numWaijin) && (buffer1y < numWaijin * 2))//第一象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(buffer1x - numWaijin) / (j + numNeijin);//注意数据类型的改变！
                                    hudu = acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(buffer1y - numWaijin) / (j + numNeijin);
                                        hudu = asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);

                                    //unsigned char t=buffer0[suffer0H*width+j].r;
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*width+j];						
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;

                                    /////最近邻插值//////
                                    o11 = buffer0[y1 * width + x1];
                                    o12 = buffer0[y2 * width + x1];
                                    o21 = buffer0[y1 * width + x2];
                                    o22 = buffer0[y2 * width + x2];

                                    if (((j0 - x1) >= (x2 - j0)) && ((buffer0h - y1) >= (y2 - buffer0h)))
                                    {
                                        buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                        = o22;
                                    }
                                    else
                                    {
                                        if (((j0 - x1) < (x2 - j0)) && ((buffer0h - y1) >= (y2 - buffer0h)))
                                        {
                                            buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                            = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                            = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                            = o12;
                                        }
                                        else
                                        {
                                            if (((j0 - x1) < (x2 - j0)) && ((buffer0h - y1) < (y2 - buffer0h)))
                                            {
                                                buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                                = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                                = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                                = o11;
                                            }
                                            else
                                            {
                                                if (((j0 - x1) >= (x2 - j0)) && ((buffer0h - y1) < (y2 - buffer0h)))
                                                {
                                                    buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                                    = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                                    = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                                    = o21;
                                                }
                                            }
                                        }
                                    }

                                }

                                if ((buffer1x >= 0) && (buffer1x < numWaijin) && (buffer1y >= numWaijin) && (buffer1y < numWaijin * 2))//第二象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(numWaijin - buffer1x) / (j + numNeijin);//注意数据类型的改变！
                                    hudu = Math.PI - acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(buffer1y - numWaijin) / (j + numNeijin);
                                        hudu = Math.PI - asin(bizhi);
                                    }

                                    //	hudu=Math.PI-acos(bizhi);
                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    //unsigned char t=buffer0[suffer0H*width+j].r;
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*width+j];						
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;


                                    /////最近邻插值//////
                                    o11 = buffer0[y1 * width + x1];
                                    o12 = buffer0[y2 * width + x1];
                                    o21 = buffer0[y1 * width + x2];
                                    o22 = buffer0[y2 * width + x2];

                                    if (((j0 - x1) >= (x2 - j0)) && ((buffer0h - y1) >= (y2 - buffer0h)))
                                    {
                                        buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                        = o22;
                                    }
                                    else
                                    {
                                        if (((j0 - x1) < (x2 - j0)) && ((buffer0h - y1) >= (y2 - buffer0h)))
                                        {
                                            buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                            = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                            = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                            = o12;
                                        }
                                        else
                                        {
                                            if (((j0 - x1) < (x2 - j0)) && ((buffer0h - y1) < (y2 - buffer0h)))
                                            {
                                                buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                                = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                                = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                                = o11;
                                            }
                                            else
                                            {
                                                if (((j0 - x1) >= (x2 - j0)) && ((buffer0h - y1) < (y2 - buffer0h)))
                                                {
                                                    buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                                    = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                                    = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                                    = o21;
                                                }
                                            }
                                        }
                                    }
                                }

                                if ((buffer1x >= 0) && (buffer1x < numWaijin) && (buffer1y >= 0) && (buffer1y < numWaijin))//第三象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(numWaijin - buffer1x) / (j + numNeijin);//注意数据类型的改变！
                                    hudu = Math.PI + acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(numWaijin - buffer1y) / (j + numNeijin);
                                        hudu = Math.PI + asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    //unsigned char t=buffer0[suffer0H*width+j].r;
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*width+j];						
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;

                                    /////最近邻插值//////
                                    o11 = buffer0[y1 * width + x1];
                                    o12 = buffer0[y2 * width + x1];
                                    o21 = buffer0[y1 * width + x2];
                                    o22 = buffer0[y2 * width + x2];

                                    if (((j0 - x1) >= (x2 - j0)) && ((buffer0h - y1) >= (y2 - buffer0h)))
                                    {
                                        buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                        = o22;
                                    }
                                    else
                                    {
                                        if (((j0 - x1) < (x2 - j0)) && ((buffer0h - y1) >= (y2 - buffer0h)))
                                        {
                                            buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                            = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                            = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                            = o12;
                                        }
                                        else
                                        {
                                            if (((j0 - x1) < (x2 - j0)) && ((buffer0h - y1) < (y2 - buffer0h)))
                                            {
                                                buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                                = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                                = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                                = o11;
                                            }
                                            else
                                            {
                                                if (((j0 - x1) >= (x2 - j0)) && ((buffer0h - y1) < (y2 - buffer0h)))
                                                {
                                                    buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                                    = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                                    = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                                    = o21;
                                                }
                                            }
                                        }
                                    }
                                }

                                if ((buffer1x >= numWaijin) && (buffer1x < numWaijin * 2) && (buffer1y >= 0) && (buffer1y < numWaijin))//第四象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(buffer1x - numWaijin) / (j + numNeijin);//注意数据类型的改变！！！！！！！！
                                    hudu = 2 * Math.PI - acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(numWaijin - buffer1y) / (j + numNeijin);
                                        hudu = 2 * Math.PI - asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    //unsigned char t=buffer0[suffer0H*width+j].r;
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*width+j];						
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;
                                    if ((x2 == 512)&&(y2==(height-1)))
                                        x2 = x1;


                                    /////最近邻插值//////
                                    o11 = buffer0[y1 * width + x1];
                                    o12 = buffer0[y2 * width + x1];
                                    o21 = buffer0[y1 * width + x2];
                                    o22 = buffer0[y2 * width + x2];

                                    if (((j0 - x1) >= (x2 - j0)) && ((buffer0h - y1) >= (y2 - buffer0h)))
                                    {
                                        buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                        = o22;
                                    }
                                    else
                                    {
                                        if (((j0 - x1) < (x2 - j0)) && ((buffer0h - y1) >= (y2 - buffer0h)))
                                        {
                                            buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                            = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                            = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                            = o12;
                                        }
                                        else
                                        {
                                            if (((j0 - x1) < (x2 - j0)) && ((buffer0h - y1) < (y2 - buffer0h)))
                                            {
                                                buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                                = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                                = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                                = o11;
                                            }
                                            else
                                            {
                                                if (((j0 - x1) >= (x2 - j0)) && ((buffer0h - y1) < (y2 - buffer0h)))
                                                {
                                                    buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].r
                                                    = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].g
                                                    = buffer_NearstInsert[buffer1x + buffer1y * numWaijin * 2].b
                                                    = o21;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    progressBar1.Value += progressBar1.Step;//让进度条增加一次
                }

                for (long i = 0; i < numWaijin * numWaijin * 4; i++)
                {
                    temp_write[i * 3] = buffer_NearstInsert[i].r;
                    temp_write[i * 3 + 1] = buffer_NearstInsert[i].g;
                    temp_write[i * 3 + 2] = buffer_NearstInsert[i].b;
                    //  buffer0[i] = temp_read[i];

                }


                fs2_NearstInsert.Write(temp_write, 0, (int)(numWaijin * numWaijin * 4 * 3));

                fs1_NearstInsert.Close();
                fs2_NearstInsert.Close();
                fs1_NearstInsert.Dispose();
                fs2_NearstInsert.Dispose();


                //	SaveBmpInDlg("E:\\OutPut-最近邻法插值.bmp",numWaijin*2,numWaijin*2,(unsigned char*)buffer_NearstInsert);

                //delete [] buffer_NearstInsert;

                myTimer.Stop();
               // pictureBox2.Image = bmp8;//将图片显示在pictureBox1中
                timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
                Invalidate();
            }
        }

        private void button_BilinearInsert_Click(object sender, EventArgs e)
        {
            FileStream fs1_BilinearInsert = new FileStream(dataFileName, FileMode.Open);
            FileStream fs2_BilinearInsert = new FileStream(dataFileName + "_BilinearInsert.bmp", FileMode.Create);
            fs1_BilinearInsert.Seek(start, SeekOrigin.Begin);

            int[] head1_BilinearInsert ={
                                          0x4D42,(int)(size%0x10000),(int)(size/0x10000),0,0,0x36,0,0x28,
                                          0,(int)((numWaijin*2)%0x10000),(int)((numWaijin*2)/0x10000),(int)((numWaijin*2)%0x10000),
                                          (int)((numWaijin*2)/0x10000),0x10,0x18,0,0,0,0,0,0,0,0,0,0,0,0
                                      };

            byte[] head2_BilinearInsert = new byte[head1_BilinearInsert.Length * 2];


            for (int i = 0; i < head1_BilinearInsert.Length; i++)
            {
                head2_BilinearInsert[i * 2] = (byte)(head1_BilinearInsert[i] % 0x100);
                head2_BilinearInsert[i * 2 + 1] = (byte)(head1_BilinearInsert[i] / 0x100);
            }

            fs2_BilinearInsert.Write(head2_BilinearInsert, 0, head2_BilinearInsert.Length);
            byte[] temp_read = new byte[data_length];
            fs1_BilinearInsert.Seek(start, SeekOrigin.Begin);
            fs1_BilinearInsert.Read(temp_read, 0, (int)(data_length));    ///////


            byte[] temp_write = new byte[numWaijin * numWaijin * 4 * 3];

            byte[] buffer0 = new byte[width * height];
            for (long i = 0; i < width * height; i++)
            //      for (long j = 0; j <height ; j++)
            {
                long num = start + i;
                //  long num = width * (i % width) + (i / width);
                long x, y;

                //     temp_write[i * 3] = temp_read[i];
                //     temp_write[i * 3 + 1] = temp_read[i];
                //     temp_write[i * 3 + 2] = temp_read[i];
                buffer0[i] = temp_read[i];

            }


            unsafe
            {
                myTimer.ClearTimer();
                myTimer.Start();
                theta = Math.PI * 2 / height;

                color_RGB[] buffer_BilinearInsert = new color_RGB[numWaijin * numWaijin * 4];//初始赋值

                for (int buffer_BilinearInsert_Num = 0; buffer_BilinearInsert_Num < numWaijin * numWaijin * 4; buffer_BilinearInsert_Num++)
                {
                    buffer_BilinearInsert[buffer_BilinearInsert_Num].b = buffer_BilinearInsert[buffer_BilinearInsert_Num].g = buffer_BilinearInsert[buffer_BilinearInsert_Num].r
                        = buffer_standard[buffer_BilinearInsert_Num].r;//算法在此处
                }

                int buffer1x, buffer1y, j = 0, suffer0H = 0;//buffer1x,buffer1y为生成图中的x,y	       
                double buffer0h, j0;
                double hudu;
                double bizhi;
                int x1, x2, y1, y2;
                int r1, r2;
                byte o11, o21, o12, o22;


                progressBar1.Maximum = (int)(numWaijin * 2);
                progressBar1.Step = 1;
                progressBar1.Value = 0;
                //    
                for (buffer1x = 0; buffer1x < numWaijin * 2; buffer1x++)
                {
                    for (buffer1y = 0; buffer1y < numWaijin * 2; buffer1y++)
                    {
                        //m_Progress.SetPos(buffer1x*50/numWaijin);		
                        if (
                            ((System.Math.Abs(buffer1x - numWaijin) * System.Math.Abs(buffer1x - numWaijin) + System.Math.Abs(buffer1y - numWaijin) * System.Math.Abs(buffer1y - numWaijin)) >= numNeijin * numNeijin) &&
                            ((System.Math.Abs(buffer1x - numWaijin) * System.Math.Abs(buffer1x - numWaijin) + System.Math.Abs(buffer1y - numWaijin) * System.Math.Abs(buffer1y - numWaijin)) <= (numNeijin + width) * (numNeijin + width))
                            )
                        {
                            if (buffer_standard[buffer1x + buffer1y * numWaijin * 2].b == 0)
                            {
                                if ((buffer1x >= numWaijin) && (buffer1x < numWaijin * 2) && (buffer1y >= numWaijin) && (buffer1y < numWaijin * 2))//第一象限
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(buffer1x - numWaijin) / (j + numNeijin);//注意数据类型的改变！					
                                    hudu = acos(bizhi);
                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(buffer1y - numWaijin) / (j + numNeijin);
                                        hudu = asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);


                                    //unsigned char t=buffer0[suffer0H*512+j].r;

                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*512+j];						

                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;


                                    /////双线性插值//////						
                                    o11 = buffer0[y1 * 512 + x1];
                                    o12 = buffer0[y2 * 512 + x1];
                                    o21 = buffer0[y1 * 512 + x2];
                                    o22 = buffer0[y2 * 512 + x2];

                                    r1 = (int)((x2 - j0) / (x2 - x1) * o11 + (j0 - x1) / (x2 - x1) * o21);
                                    r2 = (int)((x2 - j0) / (x2 - x1) * o12 + (j0 - x1) / (x2 - x1) * o22);

                                    buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].b
                                        = (byte)((y2 - buffer0h) / (y2 - y1) * r1 + (buffer0h - y1) / (y2 - y1) * r2);
                                }

                                if ((buffer1x >= 0) && (buffer1x < numWaijin) && (buffer1y >= numWaijin) && (buffer1y < numWaijin * 2))//第二象限					
                                {

                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(numWaijin - buffer1x) / (j + numNeijin);//注意数据类型的改变！						
                                    hudu = Math.PI - acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(buffer1y - numWaijin) / (j + numNeijin);
                                        hudu = Math.PI - asin(bizhi);
                                    }

                                    //	hudu=Math.PI-acos(bizhi);						
                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    //unsigned char t=buffer0[suffer0H*512+j].r;						
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*512+j];	

                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;

                                    /////双线性插值//////						
                                    o11 = buffer0[y1 * 512 + x1];
                                    o12 = buffer0[y2 * 512 + x1];
                                    o21 = buffer0[y1 * 512 + x2];
                                    o22 = buffer0[y2 * 512 + x2];

                                    r1 = (int)((x2 - j0) / (x2 - x1) * o11 + (j0 - x1) / (x2 - x1) * o21);
                                    r2 = (int)((x2 - j0) / (x2 - x1) * o12 + (j0 - x1) / (x2 - x1) * o22);

                                    buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].b
                                        = (byte)((y2 - buffer0h) / (y2 - y1) * r1 + (buffer0h - y1) / (y2 - y1) * r2);
                                }


                                if ((buffer1x >= 0) && (buffer1x < numWaijin) && (buffer1y >= 0) && (buffer1y < numWaijin))//第三象限				
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(numWaijin - buffer1x) / (j + numNeijin);//注意数据类型的改变！						
                                    hudu = Math.PI + acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(numWaijin - buffer1y) / (j + numNeijin);
                                        hudu = Math.PI + asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);

                                    //unsigned char t=buffer0[suffer0H*512+j].r;						
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*512+j];												
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;

                                    /////双线性插值//////						
                                    o11 = buffer0[y1 * 512 + x1];
                                    o12 = buffer0[y2 * 512 + x1];
                                    o21 = buffer0[y1 * 512 + x2];
                                    o22 = buffer0[y2 * 512 + x2];

                                    r1 = (int)((x2 - j0) / (x2 - x1) * o11 + (j0 - x1) / (x2 - x1) * o21);
                                    r2 = (int)((x2 - j0) / (x2 - x1) * o12 + (j0 - x1) / (x2 - x1) * o22);

                                    buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].b
                                        = (byte)((y2 - buffer0h) / (y2 - y1) * r1 + (buffer0h - y1) / (y2 - y1) * r2);
                                }

                                if ((buffer1x >= numWaijin) && (buffer1x < numWaijin * 2) && (buffer1y >= 0) && (buffer1y < numWaijin))//第四象限					
                                {
                                    j = (int)(Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);
                                    bizhi = (double)(buffer1x - numWaijin) / (j + numNeijin);//注意数据类型的改变！！！！！！！！					
                                    hudu = 2 * Math.PI - acos(bizhi);

                                    if (bizhi > 0.7071067811)
                                    {
                                        bizhi = (double)(numWaijin - buffer1y) / (j + numNeijin);
                                        hudu = 2 * Math.PI - asin(bizhi);
                                    }

                                    double beichu = theta * numXuanzhuanjiao;
                                    suffer0H = (int)(hudu / beichu);
                                    buffer0h = (int)(hudu / beichu);
                                    j0 = (Math.Sqrt((double)((buffer1x - numWaijin) * (buffer1x - numWaijin) + (buffer1y - numWaijin) * (buffer1y - numWaijin))) - numNeijin);

                                    //unsigned char t=buffer0[suffer0H*512+j].r;					
                                    //buffer2[buffer1x+buffer1y*numWaijin*2]=buffer0[suffer0H*512+j];											
                                    x1 = (int)(Math.Floor(j0));
                                    x2 = x1 + 1;
                                    y1 = (int)(Math.Floor(buffer0h));
                                    y2 = y1 + 1;
                                    if ((x2 == 512) && (y2 == (height - 1)))
                                        x2 = x1;

                                    /////双线性插值//////						
                                    o11 = buffer0[y1 * 512 + x1];
                                    o12 = buffer0[y2 * 512 + x1];
                                    o21 = buffer0[y1 * 512 + x2];
                                    o22 = buffer0[y2 * 512 + x2];

                                    r1 = (int)((x2 - j0) / (x2 - x1) * o11 + (j0 - x1) / (x2 - x1) * o21);
                                    r2 = (int)((x2 - j0) / (x2 - x1) * o12 + (j0 - x1) / (x2 - x1) * o22);

                                    buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].r
                                        = buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].g
                                        = buffer_BilinearInsert[buffer1x + buffer1y * numWaijin * 2].b
                                        = (byte)((y2 - buffer0h) / (y2 - y1) * r1 + (buffer0h - y1) / (y2 - y1) * r2);
                                }
                            }
                        }
                    } 
                    progressBar1.Value += progressBar1.Step;//让进度条增加一次
                }

                for (long i = 0; i < numWaijin * numWaijin * 4; i++)
                {
                    temp_write[i * 3] = buffer_BilinearInsert[i].r;
                    temp_write[i * 3 + 1] = buffer_BilinearInsert[i].g;
                    temp_write[i * 3 + 2] = buffer_BilinearInsert[i].b;
                }

                fs2_BilinearInsert.Write(temp_write, 0, (int)(numWaijin * numWaijin * 4 * 3));
                fs1_BilinearInsert.Close();
                fs2_BilinearInsert.Close();
                fs1_BilinearInsert.Dispose();
                fs2_BilinearInsert.Dispose();

                myTimer.Stop();
                //      pictureBox2.Image = bmp8;//将图片显示在pictureBox1中
                timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
                Invalidate();
            }
        }

        private void button_BiCubicInsert_Click(object sender, EventArgs e)
        {
            
            FileStream fs1_BiCubicInsert = new FileStream(dataFileName, FileMode.Open);
            FileStream fs2_BiCubicInsert = new FileStream(dataFileName + "_BiCubicInsert.bmp", FileMode.Create);
            fs1_BiCubicInsert.Seek(start, SeekOrigin.Begin);
        
	        
            int[] head1_BiCubicInsert ={
                                          0x4D42,(int)(size%0x10000),(int)(size/0x10000),0,0,0x36,0,0x28,
                                          0,(int)((numWaijin*2)%0x10000),(int)((numWaijin*2)/0x10000),(int)((numWaijin*2)%0x10000),
                                          (int)((numWaijin*2)/0x10000),0x10,0x18,0,0,0,0,0,0,0,0,0,0,0,0
                                      };

            byte[] head2_BiCubicInsert = new byte[head1_BiCubicInsert.Length * 2];
                
            for (int i = 0; i < head1_BiCubicInsert.Length; i++)           
            {            
                head2_BiCubicInsert[i * 2] = (byte)(head1_BiCubicInsert[i] % 0x100);           
                head2_BiCubicInsert[i * 2 + 1] = (byte)(head1_BiCubicInsert[i] / 0x100);           
            }
 
            fs2_BiCubicInsert.Write(head2_BiCubicInsert, 0, head2_BiCubicInsert.Length);

            byte[] temp_read = new byte[data_length];
            fs1_BiCubicInsert.Seek(start, SeekOrigin.Begin);
            fs1_BiCubicInsert.Read(temp_read, 0, (int)(data_length));    ///////

            byte[] temp_write = new byte[numWaijin * numWaijin * 4 * 3];
            byte[] buffer0 = new byte[width * height];

            for (long i = 0; i < width * height; i++)
            {
                long num = start + i;
                long x, y;
                buffer0[i] = temp_read[i];
            }

                 
            unsafe
            {
                myTimer.ClearTimer();
                myTimer.Start();
                theta=Math.PI*2/height;

                color_RGB[] buffer_BiCubicInsert=new color_RGB[numWaijin*numWaijin*4];//初始赋值
                	       
                for(int buffer_BiCubicInsert_Num=0;buffer_BiCubicInsert_Num<numWaijin*numWaijin*4;buffer_BiCubicInsert_Num++)
                {     
                    buffer_BiCubicInsert[buffer_BiCubicInsert_Num].b
                        =buffer_BiCubicInsert[buffer_BiCubicInsert_Num].g
                        =buffer_BiCubicInsert[buffer_BiCubicInsert_Num].r
                        = buffer_standard[buffer_BiCubicInsert_Num].r;//算法在此处    
                }
  
                int buffer1x,buffer1y,j=0,suffer0H=0;//buffer1x,buffer1y为生成图中的x,y
                double buffer0h,j0;	        
                double hudu;	       
                double bizhi;	        
                int x0,x1,x2,x3,y0,y1,y2,y3;	       
                int r1,r2;          
                byte o00,o01,o02,o03,o10,o11,o12,o13,
                     o20,o21,o22,o23,o30,o31,o32,o33;


                progressBar1.Maximum = (int)(numWaijin * 2);
                progressBar1.Step = 1;
                progressBar1.Value = 0;
                //    
                for(buffer1x=0;buffer1x<numWaijin*2;buffer1x++)	
                {		
                    for(buffer1y=0;buffer1y<numWaijin*2;buffer1y++)		
                    {		
                        if(
                            ((System.Math.Abs(buffer1x-numWaijin)*System.Math.Abs(buffer1x-numWaijin)+System.Math.Abs(buffer1y-numWaijin)*System.Math.Abs(buffer1y-numWaijin))>=numNeijin*numNeijin)&&
                            ((System.Math.Abs(buffer1x-numWaijin)*System.Math.Abs(buffer1x-numWaijin)+System.Math.Abs(buffer1y-numWaijin)*System.Math.Abs(buffer1y-numWaijin))<=(numNeijin+512)*(numNeijin+512))
                          )		
                        {
                            if(buffer_standard[buffer1x+buffer1y*numWaijin*2].b==0)
                            {
                                if((buffer1x>=numWaijin)&&(buffer1x<numWaijin*2)&&(buffer1y>=numWaijin)&&(buffer1y<numWaijin*2))//第一象限
                                {
                                    j=(int)(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
                                    j0=(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
                                    bizhi=(double)(buffer1x-numWaijin)/(j+numNeijin);//注意数据类型的改变！
                                    hudu=acos(bizhi);

                                    if(bizhi>0.7071067811)
                                    {
                                        bizhi=(double)(buffer1y-numWaijin)/(j+numNeijin);
                                        hudu=asin(bizhi);
                                    }

                                    double beichu=theta*numXuanzhuanjiao;  
                                    double buffer0H=(hudu/beichu);
                                    suffer0H=(int)(hudu/beichu);
                                    buffer0h=(int)(hudu/beichu);
						
                                    j0=(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
										
                                    x1=(int)(Math.Floor(j0));
                                    x0=x1-1;
                                    x2=x1+1;
                                    x3=x1+2;
						
                                    y1=(int)(Math.Floor(buffer0h));						
                                    y0=y1-1;						
                                    y2=y1+1;						
                                    y3=y1+2;
						
                                    if((x0<=0)||(y0<=0)) 
                                    {
                                        x0=x1;
                                        y0=y1;
                                    }
						
                                    /////三次内插法//////
                                    o00=buffer0[y0*512+x0];    o10=buffer0[y0*512+x1];   o20=buffer0[y0*512+x2];    o30=buffer0[y0*512+x3];
                                    o01=buffer0[y1*512+x0];    o11=buffer0[y1*512+x1];   o21=buffer0[y1*512+x2];    o31=buffer0[y1*512+x3];
                                    o02=buffer0[y2*512+x0];    o12=buffer0[y2*512+x1];   o22=buffer0[y2*512+x2];    o32=buffer0[y2*512+x3];
                                    o03=buffer0[y3*512+x0];    o13=buffer0[y3*512+x1];   o23=buffer0[y3*512+x2];    o33=buffer0[y3*512+x3];
												
                                    int f_y0=F_X(j0,o00,o10,o20,o30);
                                    int f_y1=F_X(j0,o01,o11,o21,o31);
                                    int f_y2=F_X(j0,o02,o12,o22,o32);
                                    int f_y3=F_X(j0,o03,o13,o23,o33);
                                    int f_xy=F_X(buffer0H,f_y0,f_y1,f_y2,f_y3);

						
                                    buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].r
                                        =buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].g
                                        =buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].b
                                        =(byte)(f_xy);
                                }		

					
                                if((buffer1x>=0)&&(buffer1x<numWaijin)&&(buffer1y>=numWaijin)&&(buffer1y<numWaijin*2))//第二象限
                                {					
                                    j=(int)(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
                                    bizhi=(double)(numWaijin-buffer1x)/(j+numNeijin);//注意数据类型的改变！
                                    hudu=Math.PI-acos(bizhi);

                                    if(bizhi>0.7071067811)
                                    {
                                        bizhi=(double)(buffer1y-numWaijin)/(j+numNeijin);
                                        hudu=Math.PI-asin(bizhi);
                                    }

                                    double beichu=theta*numXuanzhuanjiao;  
                                    double buffer0H=(hudu/beichu);
                                    suffer0H=(int)(hudu/beichu);
                                    buffer0h=(int)(hudu/beichu);
						
                                    j0=(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
				                                   
                                    x1=(int)(Math.Floor(j0));
                                    x0=x1-1;
                                    x2=x1+1;
                                    x3=x1+2;
						
                                    y1=(int)(Math.Floor(buffer0h));						
                                    y0=y1-1;						
                                    y2=y1+1;						
                                    y3=y1+2;

						
                                    if((x0<=0)||(y0<=0)) 
                                    {
                                        x0=x1;
                                        y0=y1;
                                    }

                                    /////三次内插法//////
                                    o00=buffer0[y0*512+x0];    o10=buffer0[y0*512+x1];   o20=buffer0[y0*512+x2];    o30=buffer0[y0*512+x3];
                                    o01=buffer0[y1*512+x0];    o11=buffer0[y1*512+x1];   o21=buffer0[y1*512+x2];    o31=buffer0[y1*512+x3];
                                    o02=buffer0[y2*512+x0];    o12=buffer0[y2*512+x1];   o22=buffer0[y2*512+x2];    o32=buffer0[y2*512+x3];
                                    o03=buffer0[y3*512+x0];    o13=buffer0[y3*512+x1];   o23=buffer0[y3*512+x2];    o33=buffer0[y3*512+x3];
						
                                    int f_y0=F_X(j0,o00,o10,o20,o30);
                                    int f_y1=F_X(j0,o01,o11,o21,o31);
                                    int f_y2=F_X(j0,o02,o12,o22,o32);
                                    int f_y3=F_X(j0,o03,o13,o23,o33);
                                    int f_xy=F_X(buffer0H,f_y0,f_y1,f_y2,f_y3);
			
                                    buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].r
                                        =buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].g
                                        =buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].b
                                        = (byte)(f_xy);
					}		

					if((buffer1x>=0)&&(buffer1x<numWaijin)&&(buffer1y>=0)&&(buffer1y<numWaijin))//第三象限
					{
						j=(int)(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
						bizhi=(double)(numWaijin-buffer1x)/(j+numNeijin);//注意数据类型的改变！
						hudu=Math.PI+acos(bizhi);

						if(bizhi>0.7071067811)
						{
							bizhi=(double)(numWaijin-buffer1y)/(j+numNeijin);
							hudu=Math.PI+asin(bizhi);
						}

						double beichu=theta*numXuanzhuanjiao;  
						double buffer0H=(hudu/beichu);
						suffer0H=(int)(hudu/beichu);
						buffer0h=(int)(hudu/beichu);
						
						j0=(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
				
                                  
                        x1=(int)(Math.Floor(j0));               
                        x0=x1-1;            
                        x2=x1+1;             
                        x3=x1+2;
						         
                        y1=(int)(Math.Floor(buffer0h));						         
                        y0=y1-1;						          
                        y2=y1+1;						    
                        y3=y1+2;

						if((x0<=0)||(y0<=0)) 
						{
							x0=x1;
							y0=y1;
						}

						/////三次内插法//////
						o00=buffer0[y0*512+x0];    o10=buffer0[y0*512+x1];   o20=buffer0[y0*512+x2];    o30=buffer0[y0*512+x3];
						o01=buffer0[y1*512+x0];    o11=buffer0[y1*512+x1];   o21=buffer0[y1*512+x2];    o31=buffer0[y1*512+x3];
						o02=buffer0[y2*512+x0];    o12=buffer0[y2*512+x1];   o22=buffer0[y2*512+x2];    o32=buffer0[y2*512+x3];
						o03=buffer0[y3*512+x0];    o13=buffer0[y3*512+x1];   o23=buffer0[y3*512+x2];    o33=buffer0[y3*512+x3];
						
						int f_y0=F_X(j0,o00,o10,o20,o30);
						int f_y1=F_X(j0,o01,o11,o21,o31);
						int f_y2=F_X(j0,o02,o12,o22,o32);
						int f_y3=F_X(j0,o03,o13,o23,o33);
						int f_xy=F_X(buffer0H,f_y0,f_y1,f_y2,f_y3);

						buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].r
							=buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].g
							=buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].b
						    =(byte)(f_xy);
					}		

					if((buffer1x>=numWaijin)&&(buffer1x<numWaijin*2)&&(buffer1y>=0)&&(buffer1y<numWaijin))//第四象限
					{
						j=(int)(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
						bizhi=(double)(buffer1x-numWaijin)/(j+numNeijin);//注意数据类型的改变！！！！！！！！
						hudu=2*Math.PI-acos(bizhi);

						if(bizhi>0.7071067811)
						{
							bizhi=(double)(numWaijin-buffer1y)/(j+numNeijin);
							hudu=2*Math.PI-asin(bizhi);
						}

						double beichu=theta*numXuanzhuanjiao;  
						double buffer0H=(hudu/beichu);
						suffer0H=(int)(hudu/beichu);
						buffer0h=(int)(hudu/beichu);
						
						j0=(Math.Sqrt((double)((buffer1x-numWaijin)*(buffer1x-numWaijin)+(buffer1y-numWaijin)*(buffer1y-numWaijin)))-numNeijin);
				
                        x1=(int)(Math.Floor(j0));               
                        x0=x1-1;            
                        x2=x1+1;             
                        x3=x1+2;
						         
                        y1=(int)(Math.Floor(buffer0h));						         
                        y0=y1-1;						          
                        y2=y1+1;						    
                        y3=y1+2;

						if((x0<=0)||(y0<=0)) 
						{
							x0=x1;
							y0=y1;
						}
                        if ((y3 == (height - 1)) && (x2 == 512))
                        {
                            x2 = x1;
                            x3 = x1;
                        
                        }
                        if ((y2 == (height - 1)) && (x2 == 512))
                        {
                            x2 = x1;
                            x3 = x1;
                            y3 = y2;

                        }

						/////三次内插法//////
						o00=buffer0[y0*512+x0];    o10=buffer0[y0*512+x1];   o20=buffer0[y0*512+x2];    o30=buffer0[y0*512+x3];
						o01=buffer0[y1*512+x0];    o11=buffer0[y1*512+x1];   o21=buffer0[y1*512+x2];    o31=buffer0[y1*512+x3];
						o02=buffer0[y2*512+x0];    o12=buffer0[y2*512+x1];   o22=buffer0[y2*512+x2];    o32=buffer0[y2*512+x3];
						o03=buffer0[y3*512+x0];    o13=buffer0[y3*512+x1];   o23=buffer0[y3*512+x2];    o33=buffer0[y3*512+x3];
						
						int f_y0=F_X(j0,o00,o10,o20,o30);
						int f_y1=F_X(j0,o01,o11,o21,o31);
						int f_y2=F_X(j0,o02,o12,o22,o32);
						int f_y3=F_X(j0,o03,o13,o23,o33);
						int f_xy=F_X(buffer0H,f_y0,f_y1,f_y2,f_y3);

						buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].r
							=buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].g
							=buffer_BiCubicInsert[buffer1x+buffer1y*numWaijin*2].b
						    =(byte)(f_xy);
                    }		
                            }
                        }
                    } 
                    progressBar1.Value += progressBar1.Step;//让进度条增加一次
                }

                for (long i = 0; i < numWaijin * numWaijin * 4; i++)
                {
                    temp_write[i * 3] = buffer_BiCubicInsert[i].r;
                    temp_write[i * 3 + 1] = buffer_BiCubicInsert[i].g;
                    temp_write[i * 3 + 2] = buffer_BiCubicInsert[i].b;
                }

                fs2_BiCubicInsert.Write(temp_write, 0, (int)(numWaijin * numWaijin * 4 * 3));
                fs1_BiCubicInsert.Close();
                fs2_BiCubicInsert.Close();
                fs1_BiCubicInsert.Dispose();
                fs2_BiCubicInsert.Dispose();

                myTimer.Stop();
                //pictureBox2.Image = bmp8;//将图片显示在pictureBox1中
                timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
                Invalidate();
        }





        ///
        /*刷新法会有延迟现象
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (curBitmap != null)
            {
                g.DrawImage(curBitmap, 0, 0, curBitmap.Width, curBitmap.Height);
            }
            this.pictureBox1.Refresh();
        }*/

    }
        private void getCicleNum()
        {
            for (int i = 0; i < 500; i++)
            {
              //  System.Threading.Thread.Sleep(10);//没什么意思，单纯的执行延时
                SetTextMessage(100 * i / 500);
            }









        }

        private void button_getCicleNum_Click(object sender, EventArgs e)
        {             
            myTimer.ClearTimer();
            myTimer.Start();  //记录开始时间


     //       Thread fThread = new Thread(new ThreadStart(getCicleNum));//开辟一个新的线程
     //       fThread.Start();

 
            
          
            
            String CircleNum;
            string CircleNum_angel;
            int circleNum_heigth = 100;   //模板的头开始的初始行数
            double circleMin = 0;  //相关处理的值的最小值
            double circleSum = 0;  //相关处理的值
            circleNumH = 100;  //每一圈的行数
            int circleMubanH = 100;//模板的高度
            int circle_unitNum = 0;
            double circleNumber;

            double numZhuanSu = double.Parse(textBox_zhuansu.Text); ;


            FileStream fs_GetCircleNum = new FileStream(dataFileName, FileMode.Open);
            fs_GetCircleNum.Seek(start, SeekOrigin.Begin);

            start = int.Parse(tbStart.Text);
            end = int.Parse(tbDest.Text);
            data_length = (long)(end - start);
            // width = (long)(((end - start)) / width) + 1;
            height = (long)(((end - start)) / width) + 1;

            byte[] temp_read = new byte[data_length];
            fs_GetCircleNum.Seek(start, SeekOrigin.Begin);
            fs_GetCircleNum.Read(temp_read, 0, (int)(data_length));    ///////      data_length和w与h的关系

            byte[] buffer0 = new byte[data_length];

            for (long i = 0; i < (height - 1) * width; i++)
            {
                buffer0[i] = temp_read[i];
            }



            progressBar1.Maximum = (int)(circleMubanH * 512);//设置最大长度值
            progressBar1.Value = 0;//设置当前值
            progressBar1.Step = 1;//设置没次增长多少
            for (circle_unitNum = 0; circle_unitNum < circleMubanH * 512; circle_unitNum++)
            {
                circleSum = System.Math.Abs(buffer0[circle_unitNum + circleNum_heigth * 512] - buffer0[circle_unitNum]) * System.Math.Abs(buffer0[circle_unitNum + circleNum_heigth * 512] - buffer0[circle_unitNum]) + circleSum;
                circleMin = circleSum;
                progressBar1.Value += progressBar1.Step;//让进度条增加一次
            }

            //	for(circleNum=100;circleNum<h-circleMubanH;circleNum++)
            //   for (circleNum = (int)(5000 / numZhuanSu - 500); circleNum < (int)(5000 / numZhuanSu + 500); circleNum++)


           // progressBar1.Maximum = (int)(10240 / numZhuanSu);//设置最大长度值
          //  progressBar1.Value = 0;//设置当前值           1024*numZhuanSu + 1000
         //   progressBar1.Step = 1;//设置没次增长多少
            circleNumH = (int)(10240 / numZhuanSu - 100);
            for (circleNum_heigth = (int)(10240 / numZhuanSu -100); circleNum_heigth < (int)(10240 / numZhuanSu + 100); circleNum_heigth++)
            {
                circleSum = 0;
                for (circle_unitNum = 0; circle_unitNum < circleMubanH * 512; circle_unitNum++)
                {
                    circleSum = System.Math.Abs(buffer0[circle_unitNum + circleNum_heigth * 512] - buffer0[circle_unitNum]) * System.Math.Abs(buffer0[circle_unitNum + circleNum_heigth * 512] - buffer0[circle_unitNum]) + circleSum;

                }
                if (circleMin >= circleSum)
                {
                    circleMin = circleSum;
                    circleNumH = circleNum_heigth;
                }

                  // System.Threading.Thread.Sleep(1);//暂停0.001秒
          //          progressBar1.Value += progressBar1.Step;//让进度条增加一次
                 //   SetTextMessage(100 * (circleNum_heigth - 1000) / (1024 * (int)(numZhuanSu)));
                 //   label_bilv.Text = (100 * (circleNum_heigth - 1000) / (1024 * (int)(numZhuanSu))).ToString() + "/100";
            }


            circleNumber = ((double)((double)height / (double)circleNumH));
            double Number_Angle = circleNumber * 360;
            textBox_CircleNum.Text = circleNumber.ToString("0.000000");// +"圈  " + Number_Angle.ToString() + "度";
            textBox_CircleNum_angel.Text = Number_Angle.ToString("0.000000");
            textBox_EndAngle.Text = Number_Angle.ToString("0.000");



            fs_GetCircleNum.Close();
            fs_GetCircleNum.Dispose();

 /*
            ///////////////////设置默认初始角、内外径值////////////////////////////
            Number = 0;
            CircleNum.Format("%f", Number);
            SetDlgItemText(IDC_EDITChushijiao, CircleNum);//设置默认初始角为0度

            Number = 1000;
            CircleNum.Format("%f", Number);
            SetDlgItemText(IDC_EDITNeijin, CircleNum);//设置默认内径值为1000

            Number = 2000;
            CircleNum.Format("%f", Number);
            SetDlgItemText(IDC_EDITWaijin, CircleNum);//设置默认外径值为2000

            //////////////////设置默认对比度参数/////////////////////////////
            Number = 100;
            CircleNum.Format("%f", Number);
            SetDlgItemText(IDC_low_in, CircleNum);//设置默认low_in值为100

            Number = 150;
            CircleNum.Format("%f", Number);
            SetDlgItemText(IDC_high_in, CircleNum);//设置默认high_in值为200

            Number = 80;
            CircleNum.Format("%f", Number);
            SetDlgItemText(IDC_low_out, CircleNum);//设置默认low_out值为100

            Number = 200;
            CircleNum.Format("%f", Number);
            SetDlgItemText(IDC_high_out, CircleNum);//设置默认high_out值为200
 */
            myTimer.Stop();
            //pictureBox2.Image = bmp8;//将图片显示在pictureBox1中
            timeBox.Text = myTimer.Duration.ToString("####.###") + "毫秒";
            Invalidate();
        }


        public void File_LeftToRight() 
        {
            FileStream fs1 = new FileStream(dataFileName, FileMode.Open);
            FileStream fs2 = new FileStream(dataFileName + "LeftToRight", FileMode.Create);
            FileInfo data = new FileInfo(dataFileName);


            long dataFileSize = ((data.Length) / 512) * 512;

            byte[] temp_read = new byte[dataFileSize];
            byte[] temp_write = new byte[dataFileSize];

            fs1.Read(temp_read, 0, (int)(dataFileSize));    ///////


     //       progressBar1.Maximum = (int)(dataFileSize);
     //       progressBar1.Value = 0;
     //       progressBar1.Step = 1;  
            //         
            for (long i = 0; i < dataFileSize; i++)
            {
                temp_write[i] = temp_read[i / 512 * 512 + (512 - i % 512) - 1];    
     //           progressBar1.Value += progressBar1.Step;//让进度条增加一次
            }
            fs2.Write(temp_write, 0, (int)(dataFileSize));

            fs1.Close();
            fs1.Close();
            fs2.Dispose();
            fs2.Dispose();
        }

        public void File_UpToDown()
        {
            FileStream fs1 = new FileStream(dataFileName, FileMode.Open);
            FileStream fs2 = new FileStream(dataFileName + "UpToDown", FileMode.Create);
            FileInfo data = new FileInfo(dataFileName);


            long dataFileSize = ((data.Length) / 512) * 512;

            byte[] temp_read = new byte[dataFileSize];
            byte[] temp_write = new byte[dataFileSize];

            fs1.Read(temp_read, 0, (int)(dataFileSize));    ///////

            long H= dataFileSize/512;

    //        progressBar1.Maximum = (int)(dataFileSize);
    //        progressBar1.Value = 0;
    //        progressBar1.Step = 1;  
            for (long i = 0; i < dataFileSize; i++)
            {
                long  h=i/512;
                temp_write[i] = temp_read[(H-h-1)*512+i%512];
    //            progressBar1.Value += progressBar1.Step;//让进度条增加一次
            }
            fs2.Write(temp_write, 0, (int)(dataFileSize));

            fs1.Close();
            fs1.Close();
            fs2.Dispose();
            fs2.Dispose();

        }

        public void File_LeftToRight_UpToDown() 
        {
            FileStream fs1 = new FileStream(dataFileName, FileMode.Open);
            FileStream fs2 = new FileStream(dataFileName + "LeftToRight_UpToDown", FileMode.Create);
            FileInfo data = new FileInfo(dataFileName);


            long dataFileSize = ((data.Length) / 512) * 512;

            byte[] temp_read = new byte[dataFileSize];
            byte[] temp_write = new byte[dataFileSize];
            byte[] temp_write2 = new byte[dataFileSize];
            fs1.Read(temp_read, 0, (int)(dataFileSize));    ///////

            for (long i = 0; i < dataFileSize; i++)
            {
                temp_write[i] = temp_read[i / 512 * 512 + (512 - i % 512) - 1];
            }

      //      progressBar1.Maximum = (int)(dataFileSize);
      //      progressBar1.Value = 0;
      //      progressBar1.Step = 1;  
            long H = dataFileSize / 512;
            for (long i = 0; i < dataFileSize; i++)
            {
                long h = i / 512;
                temp_write2[i] = temp_write[(H - h - 1) * 512 + i % 512];
     //           progressBar1.Value += progressBar1.Step;//让进度条增加一次
            }
            fs2.Write(temp_write2, 0, (int)(dataFileSize));

            fs1.Close();
            fs1.Close();
            fs2.Dispose();
            fs2.Dispose();
        }

        private void button_FileProcess_Click(object sender, EventArgs e)
        {
             
            if ((checkBox_LeftToRight.Checked == false) && (checkBox_UpToDown.Checked == false))
            {
 
            }

            if ((checkBox_LeftToRight.Checked == true) && (checkBox_UpToDown.Checked == false))     
            {
                File_LeftToRight();  
            }
            if ((checkBox_LeftToRight.Checked == false) && (checkBox_UpToDown.Checked == true))
            {
                File_UpToDown();
            }
            if ((checkBox_LeftToRight.Checked == true) && (checkBox_UpToDown.Checked == true))
            {
                File_LeftToRight_UpToDown();
            }

        }
    }
}

