namespace AlfaPribor.VideoStorage.Client.Demo
{
    partial class VideoPlayer
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
            this.components = new System.ComponentModel.Container();
            this.pbxImage = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPlayForward = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnPlayBackward = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.mstMainMenu = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miVideocamera = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbxImage)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.mstMainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbxImage
            // 
            this.pbxImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxImage.Location = new System.Drawing.Point(12, 27);
            this.pbxImage.Name = "pbxImage";
            this.pbxImage.Size = new System.Drawing.Size(640, 480);
            this.pbxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxImage.TabIndex = 0;
            this.pbxImage.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnLast);
            this.groupBox1.Controls.Add(this.btnNext);
            this.groupBox1.Controls.Add(this.btnPlayForward);
            this.groupBox1.Controls.Add(this.btnPause);
            this.groupBox1.Controls.Add(this.btnPlayBackward);
            this.groupBox1.Controls.Add(this.btnPrev);
            this.groupBox1.Controls.Add(this.btnFirst);
            this.groupBox1.Location = new System.Drawing.Point(12, 513);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(640, 87);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btnLast
            // 
            this.btnLast.Location = new System.Drawing.Point(551, 19);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(85, 54);
            this.btnLast.TabIndex = 6;
            this.btnLast.Text = "Последний кадр";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(460, 19);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(85, 54);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "Следующий кадр";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPlayForward
            // 
            this.btnPlayForward.Location = new System.Drawing.Point(369, 19);
            this.btnPlayForward.Name = "btnPlayForward";
            this.btnPlayForward.Size = new System.Drawing.Size(85, 54);
            this.btnPlayForward.TabIndex = 4;
            this.btnPlayForward.Text = "Проиграть преред";
            this.btnPlayForward.UseVisualStyleBackColor = true;
            this.btnPlayForward.Click += new System.EventHandler(this.btnPlayForward_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(278, 19);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(85, 54);
            this.btnPause.TabIndex = 3;
            this.btnPause.Text = "Пауза";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnPlayBackward
            // 
            this.btnPlayBackward.Location = new System.Drawing.Point(187, 19);
            this.btnPlayBackward.Name = "btnPlayBackward";
            this.btnPlayBackward.Size = new System.Drawing.Size(85, 54);
            this.btnPlayBackward.TabIndex = 2;
            this.btnPlayBackward.Text = "Проиграть назад";
            this.btnPlayBackward.UseVisualStyleBackColor = true;
            this.btnPlayBackward.Click += new System.EventHandler(this.btnPlayBackward_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(96, 19);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(85, 54);
            this.btnPrev.TabIndex = 1;
            this.btnPrev.Text = "Предыдущий кадр";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(5, 19);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(85, 54);
            this.btnFirst.TabIndex = 0;
            this.btnFirst.Text = "Первый кадр";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // mstMainMenu
            // 
            this.mstMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miVideocamera});
            this.mstMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mstMainMenu.Name = "mstMainMenu";
            this.mstMainMenu.Size = new System.Drawing.Size(664, 24);
            this.mstMainMenu.TabIndex = 2;
            this.mstMainMenu.Text = "menuStrip1";
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExit});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(51, 20);
            this.miFile.Text = "Файл";
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(152, 22);
            this.miExit.Text = "Выход";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // miVideocamera
            // 
            this.miVideocamera.Name = "miVideocamera";
            this.miVideocamera.Size = new System.Drawing.Size(97, 20);
            this.miVideocamera.Text = "Видеокамера";
            // 
            // timer
            // 
            this.timer.Interval = 40;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // VideoPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 612);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pbxImage);
            this.Controls.Add(this.mstMainMenu);
            this.MainMenuStrip = this.mstMainMenu;
            this.Name = "VideoPlayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Видеоплеер";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VideoPlayer_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pbxImage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.mstMainMenu.ResumeLayout(false);
            this.mstMainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxImage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPlayForward;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnPlayBackward;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.MenuStrip mstMainMenu;
        private System.Windows.Forms.ToolStripMenuItem miFile;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miVideocamera;
        private System.Windows.Forms.Timer timer;
    }
}