namespace CCDImageProcess
{
    partial class ShowOriginalImage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.originalSize = new System.Windows.Forms.Button();
            this.suitableSize = new System.Windows.Forms.Button();
            this.getBigger = new System.Windows.Forms.Button();
            this.getSmaller = new System.Windows.Forms.Button();
            this.sizeOk = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.locationOk = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(500, 500);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(30, 104);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 500);
            this.panel1.TabIndex = 1;
            // 
            // originalSize
            // 
            this.originalSize.Location = new System.Drawing.Point(107, 29);
            this.originalSize.Name = "originalSize";
            this.originalSize.Size = new System.Drawing.Size(75, 23);
            this.originalSize.TabIndex = 2;
            this.originalSize.Text = "原始大小";
            this.originalSize.UseVisualStyleBackColor = true;
            this.originalSize.Click += new System.EventHandler(this.originalSize_Click);
            // 
            // suitableSize
            // 
            this.suitableSize.Location = new System.Drawing.Point(26, 29);
            this.suitableSize.Name = "suitableSize";
            this.suitableSize.Size = new System.Drawing.Size(75, 23);
            this.suitableSize.TabIndex = 3;
            this.suitableSize.Text = "窗口适应窗";
            this.suitableSize.UseVisualStyleBackColor = true;
            this.suitableSize.Click += new System.EventHandler(this.suitableSize_Click);
            // 
            // getBigger
            // 
            this.getBigger.Location = new System.Drawing.Point(188, 29);
            this.getBigger.Name = "getBigger";
            this.getBigger.Size = new System.Drawing.Size(75, 23);
            this.getBigger.TabIndex = 4;
            this.getBigger.Text = "放大10%";
            this.getBigger.UseVisualStyleBackColor = true;
            this.getBigger.Click += new System.EventHandler(this.getBigger_Click);
            // 
            // getSmaller
            // 
            this.getSmaller.Location = new System.Drawing.Point(269, 29);
            this.getSmaller.Name = "getSmaller";
            this.getSmaller.Size = new System.Drawing.Size(75, 23);
            this.getSmaller.TabIndex = 5;
            this.getSmaller.Text = "缩小10%";
            this.getSmaller.UseVisualStyleBackColor = true;
            this.getSmaller.Click += new System.EventHandler(this.getSmaller_Click);
            // 
            // sizeOk
            // 
            this.sizeOk.Location = new System.Drawing.Point(476, 30);
            this.sizeOk.Name = "sizeOk";
            this.sizeOk.Size = new System.Drawing.Size(43, 23);
            this.sizeOk.TabIndex = 6;
            this.sizeOk.Text = "确定";
            this.sizeOk.UseVisualStyleBackColor = true;
            this.sizeOk.Click += new System.EventHandler(this.sizeOk_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(428, 31);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(44, 21);
            this.textBox1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(546, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "坐标(x:y):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(622, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "0:0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(546, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "像素灰度值：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(623, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(809, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "定位：";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(850, 31);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(65, 21);
            this.textBox2.TabIndex = 12;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(916, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = ":";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(927, 31);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(65, 21);
            this.textBox3.TabIndex = 14;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(875, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "x";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(953, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "y";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(364, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 17;
            this.label9.Text = "输入倍率：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(545, 80);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 12);
            this.label10.TabIndex = 18;
            this.label10.Text = "分量(R:G:B)：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(622, 80);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 12);
            this.label11.TabIndex = 19;
            this.label11.Text = "(0:0:0)";
            // 
            // locationOk
            // 
            this.locationOk.Location = new System.Drawing.Point(998, 30);
            this.locationOk.Name = "locationOk";
            this.locationOk.Size = new System.Drawing.Size(41, 23);
            this.locationOk.TabIndex = 20;
            this.locationOk.Text = "确定";
            this.locationOk.UseVisualStyleBackColor = true;
            this.locationOk.Click += new System.EventHandler(this.locationOk_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(28, 104);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 12);
            this.label12.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 120);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(0, 12);
            this.label13.TabIndex = 21;
            // 
            // ShowOriginalImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1053, 666);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.locationOk);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.sizeOk);
            this.Controls.Add(this.getSmaller);
            this.Controls.Add(this.getBigger);
            this.Controls.Add(this.suitableSize);
            this.Controls.Add(this.originalSize);
            this.Controls.Add(this.panel1);
            this.Name = "ShowOriginalImage";
            this.Text = "原始图像";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        //该窗口中显示的图像的数据
        private System.Drawing.Bitmap BigImage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button originalSize;
        private System.Windows.Forms.Button suitableSize;
        private System.Windows.Forms.Button getBigger;
        private System.Windows.Forms.Button getSmaller;
        private System.Windows.Forms.Button sizeOk;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button locationOk;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;




    }
}