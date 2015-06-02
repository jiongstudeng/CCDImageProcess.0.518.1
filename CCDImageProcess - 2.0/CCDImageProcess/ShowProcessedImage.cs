using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CCDImageProcess
{
    public partial class ShowProcessedImage : Form
    {
         public ShowProcessedImage(Bitmap bmp)
        {
            InitializeComponent();
            BigImage = bmp;
            this.MouseWheel += FormSample_MouseWheel;
            this.pictureBox1.Image = BigImage;

            panel1.Width = this.Width - 100;
            panel1.Height = this.Height - 50;
            this.pictureBox1.Width = BigImage.Width;
            this.pictureBox1.Height = BigImage.Height;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;

            this.panel1.AutoScroll = true;
            picautosiz = false;
            getzoom();

            panel1.Width = this.Width - 100;
            panel1.Height = this.Height - 50;
            getzoom();
        }

        public void getzoom()
        {
            double temp1 = (double)pictureBox1.Width / (double)BigImage.Width;
            double temp2 = (double)pictureBox1.Height / (double)BigImage.Height;
            textBox1.Text = temp1 < temp2 ? Math.Round(temp1, 2).ToString() : Math.Round(temp2, 2).ToString();

        }
        public void FormSample_MouseWheel(object sender, MouseEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            //换算成相对本窗体的位置
            mousePoint.Offset(this.Location.X, this.Location.Y);
            //判断是否在panel内
            if (panel1.RectangleToScreen(panel1.DisplayRectangle).Contains(mousePoint))
            {
                //滚动
                panel1.AutoScrollPosition = new Point(panel1.HorizontalScroll.Value, panel1.VerticalScroll.Value - e.Delta);
            }
        }
        bool picautosiz = true;


        private void originalSize_Click(object sender, EventArgs e)
        {
            panel1.Width = this.Width - 100;
            panel1.Height = this.Height - 50;
            this.pictureBox1.Width = BigImage.Width;
            this.pictureBox1.Height = BigImage.Height;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;

            this.panel1.AutoScroll = true;
            picautosiz = false;
            getzoom();
        }

        private void suitableSize_Click(object sender, EventArgs e)
        {
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            picautosiz = true;
            panel1.Width = this.Width - 300;
            panel1.Height = this.Height - 300;
            pictureBox1.Width = panel1.Width - 10;
            pictureBox1.Height = panel1.Height - 10;
            getzoom();
        }

        private void getBigger_Click(object sender, EventArgs e)
        {
            double zoom = double.Parse(textBox1.Text);
            zoom = Math.Round(zoom * 1.1, 2);
            if (zoom > 20)
            {
                zoom = 20;
            }
            textBox1.Text = zoom.ToString();
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.panel1.AutoScroll = true;
            picautosiz = true;
            pictureBox1.Width = (int)(zoom * BigImage.Width);
            pictureBox1.Height = (int)(zoom * BigImage.Height);
            getzoom();
        }

        private void getSmaller_Click(object sender, EventArgs e)
        {
            double zoom = double.Parse(textBox1.Text);
            zoom = Math.Round(zoom / 1.1, 2);

            textBox1.Text = zoom.ToString();
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.panel1.AutoScroll = true;
            picautosiz = true;
            pictureBox1.Width = (int)(zoom * BigImage.Width);
            pictureBox1.Height = (int)(zoom * BigImage.Height);
            getzoom();
        }

        private void sizeOk_Click(object sender, EventArgs e)
        {
            double zoom = double.Parse(textBox1.Text);

            textBox1.Text = zoom.ToString();
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.panel1.AutoScroll = true;
            picautosiz = true;
            pictureBox1.Width = (int)(zoom * BigImage.Width);
            pictureBox1.Height = (int)(zoom * BigImage.Height);
            getzoom();
        }


        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public int colorkind;//colorbar颜色种类，0为没有颜色（原始图片）
        public double max;
        public double avg = 0.5;
        public int startline = 0;

        public Point pt1; //记录图片真实坐标
        public Point pt2;//记录图片真实坐标
        public Point windowp1;//记录窗口点击坐标
        public Point windowp2;//记录窗口点击坐标

        public bool isline;
        public int isdawR = 0;    //判断是否需要画框

        public Rectangle rect;

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Color curColor;
            if (BigImage == null)//没有图片直接返回
            {
                return;
            }
            double locationx = (double)e.X;
            double locationy = (double)e.Y;
            double bigImage_xby = (double)BigImage.Width / (double)BigImage.Height;
            double picbox_xby = (double)pictureBox1.Width / (double)pictureBox1.Height;
            int huiduzhi = 0;
            int R = 0;
            int G = 0;
            int B = 0;

            if (picbox_xby <= bigImage_xby)
            {
                locationx = (int)(((double)e.X / (double)pictureBox1.Width) * (double)BigImage.Width);
                locationy = ((int)((e.Y - ((1.000 / picbox_xby) - (1.000 / bigImage_xby)) * (double)pictureBox1.Width / 2.000) / ((1.000 / bigImage_xby) * (double)pictureBox1.Width) * (double)BigImage.Height));
            }
            else
            {
                locationx = ((int)((e.X - (picbox_xby - bigImage_xby) * (double)pictureBox1.Height / 2.000) / (bigImage_xby * (double)pictureBox1.Height) * (double)BigImage.Width));

                locationy = (int)(((double)e.Y / (double)pictureBox1.Height) * (double)BigImage.Height);
            }

            if (locationx >= 0 && locationy >= 0 && (locationx < BigImage.Width) && (locationy < BigImage.Height))
            {
                curColor = BigImage.GetPixel((int)locationx, (int)locationy);
                huiduzhi = (int)((curColor.R + curColor.G + curColor.B) / 3);
                int diata = huiduzhi;
                R = (int)curColor.R;
                G = (int)curColor.G;
                B = (int)curColor.B;
                label4.Text = diata.ToString();
                label2.Text = locationx.ToString() + ":" + locationy.ToString();
                label11.Text = "(" + R + ":" + G + ":" + B + ")";
            }
            else
            {
                label4.Text = "";
                label2.Text = " " + ":" + " ";
                label11.Text = "(" + " " + ":" + " " + ":" + " " + ")";
            }

        }

        int locationx = 0;
        int locationy = 0;
        Point result = new Point(0, 0);
        int p_x, p_y;
        Point p;
        public Point getwindowpoint(Point p)
        {

            if (BigImage == null)//没有图片直接返回
            {
                return result;
            }

            p_x = p.X;
            p_y = p.Y;
            double xby = (double)BigImage.Width / (double)BigImage.Height;
            double picboxxby = (double)pictureBox1.Width / (double)pictureBox1.Height;

            if (picboxxby <= xby)
            {
                locationx = (int)(((double)p.X / (double)BigImage.Width) * pictureBox1.Width);
                locationy = (int)((p.Y - ((1.000 / xby) - (1.000 / picboxxby)) * (double)BigImage.Height / 2.000) / ((1.000 / picboxxby) * (double)BigImage.Height) * pictureBox1.Height);
            }
            else
            {
                locationx = (int)((p.X - (xby - picboxxby) * (double)BigImage.Width / 2.000) / (picboxxby * (double)BigImage.Width) * pictureBox1.Width);
                locationy = (int)(((double)p.Y / (double)BigImage.Height) * pictureBox1.Height);
            }


            if (locationx > 0 && locationy > 0 && locationx < pictureBox1.Width && locationy < pictureBox1.Height)
            {
                result.X = locationx;
                result.Y = locationy;

            }
            return result;

        }



        private void locationOk_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int x;
            int y;
            if (int.TryParse(textBox2.Text, out x) && int.TryParse(textBox3.Text, out y))
            {
                panel1.AutoScrollPosition = getwindowpoint(new Point(x, y));
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int x;
            int y;
            if (int.TryParse(textBox2.Text, out x) && int.TryParse(textBox3.Text, out y))
            {
                panel1.AutoScrollPosition = getwindowpoint(new Point(x, y));
            }
        }





    }
}
