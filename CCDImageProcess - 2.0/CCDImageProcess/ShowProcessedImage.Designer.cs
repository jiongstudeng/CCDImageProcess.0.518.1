namespace CCDImageProcess
{
    partial class ShowProcessedImage
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
            this.originalSize = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.suitableSize = new System.Windows.Forms.Button();
            this.getBigger = new System.Windows.Forms.Button();
            this.getSmaller = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.sizeOk = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.locationOk = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // originalSize
            // 
            this.originalSize.Location = new System.Drawing.Point(93, 7);
            this.originalSize.Name = "originalSize";
            this.originalSize.Size = new System.Drawing.Size(75, 23);
            this.originalSize.TabIndex = 0;
            this.originalSize.Text = "原始大小";
            this.originalSize.UseVisualStyleBackColor = true;
            this.originalSize.Click += new System.EventHandler(this.originalSize_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(12, 70);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 500);
            this.panel1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(500, 500);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // suitableSize
            // 
            this.suitableSize.Location = new System.Drawing.Point(12, 7);
            this.suitableSize.Name = "suitableSize";
            this.suitableSize.Size = new System.Drawing.Size(75, 23);
            this.suitableSize.TabIndex = 2;
            this.suitableSize.Text = "窗口适应窗";
            this.suitableSize.UseVisualStyleBackColor = true;
            this.suitableSize.Click += new System.EventHandler(this.suitableSize_Click);
            // 
            // getBigger
            // 
            this.getBigger.Location = new System.Drawing.Point(174, 7);
            this.getBigger.Name = "getBigger";
            this.getBigger.Size = new System.Drawing.Size(75, 23);
            this.getBigger.TabIndex = 3;
            this.getBigger.Text = "放大10%";
            this.getBigger.UseVisualStyleBackColor = true;
            this.getBigger.Click += new System.EventHandler(this.getBigger_Click);
            // 
            // getSmaller
            // 
            this.getSmaller.Location = new System.Drawing.Point(255, 7);
            this.getSmaller.Name = "getSmaller";
            this.getSmaller.Size = new System.Drawing.Size(75, 23);
            this.getSmaller.TabIndex = 4;
            this.getSmaller.Text = "缩小10%";
            this.getSmaller.UseVisualStyleBackColor = true;
            this.getSmaller.Click += new System.EventHandler(this.getSmaller_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(355, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "输入倍率：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(424, 8);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(44, 21);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "1";
            // 
            // sizeOk
            // 
            this.sizeOk.Location = new System.Drawing.Point(472, 7);
            this.sizeOk.Name = "sizeOk";
            this.sizeOk.Size = new System.Drawing.Size(43, 23);
            this.sizeOk.TabIndex = 8;
            this.sizeOk.Text = "确定";
            this.sizeOk.UseVisualStyleBackColor = true;
            this.sizeOk.Click += new System.EventHandler(this.sizeOk_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(774, 42);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 12);
            this.label11.TabIndex = 30;
            this.label11.Text = "(0:0:0)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(697, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 12);
            this.label10.TabIndex = 29;
            this.label10.Text = "分量(R:G:B)：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(977, -1);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 28;
            this.label8.Text = "y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(899, -1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 27;
            this.label7.Text = "x";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(951, 14);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(65, 21);
            this.textBox3.TabIndex = 26;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(874, 14);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(65, 21);
            this.textBox2.TabIndex = 25;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(833, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 24;
            this.label5.Text = "定位：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(774, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 23;
            this.label4.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(697, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 22;
            this.label3.Text = "像素灰度值：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(605, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "0:0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(529, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 20;
            this.label6.Text = "坐标(x:y):";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(940, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(11, 12);
            this.label9.TabIndex = 31;
            this.label9.Text = ":";
            // 
            // locationOk
            // 
            this.locationOk.Location = new System.Drawing.Point(1022, 14);
            this.locationOk.Name = "locationOk";
            this.locationOk.Size = new System.Drawing.Size(38, 23);
            this.locationOk.TabIndex = 32;
            this.locationOk.Text = "确定";
            this.locationOk.UseVisualStyleBackColor = true;
            this.locationOk.Click += new System.EventHandler(this.locationOk_Click);
            // 
            // ShowProcessedImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1086, 595);
            this.Controls.Add(this.locationOk);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.sizeOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.getSmaller);
            this.Controls.Add(this.getBigger);
            this.Controls.Add(this.suitableSize);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.originalSize);
            this.Name = "ShowProcessedImage";
            this.Text = "bigpicture";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Drawing.Bitmap BigImage;
        private System.Windows.Forms.Button originalSize;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button suitableSize;
        private System.Windows.Forms.Button getBigger;
        private System.Windows.Forms.Button getSmaller;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button sizeOk;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button locationOk;
    }
}