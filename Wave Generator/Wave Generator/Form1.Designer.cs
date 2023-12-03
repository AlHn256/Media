namespace Wave_Generator
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGenerateWave = new System.Windows.Forms.Button();
            this.ReadBut = new System.Windows.Forms.Button();
            this.WriteBut = new System.Windows.Forms.Button();
            this.RTB = new System.Windows.Forms.RichTextBox();
            this.picBoxRight = new System.Windows.Forms.PictureBox();
            this.picBoxLeft = new System.Windows.Forms.PictureBox();
            this.HzScBar = new System.Windows.Forms.HScrollBar();
            this.AutoPlayChkBox = new System.Windows.Forms.CheckBox();
            this.HzLabel = new System.Windows.Forms.Label();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.VolumhScBar = new System.Windows.Forms.HScrollBar();
            this.VolumLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxLeft)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGenerateWave
            // 
            this.btnGenerateWave.Location = new System.Drawing.Point(13, 13);
            this.btnGenerateWave.Name = "btnGenerateWave";
            this.btnGenerateWave.Size = new System.Drawing.Size(173, 29);
            this.btnGenerateWave.TabIndex = 0;
            this.btnGenerateWave.Text = "Generate";
            this.btnGenerateWave.UseVisualStyleBackColor = true;
            this.btnGenerateWave.Click += new System.EventHandler(this.button1_Click);
            // 
            // ReadBut
            // 
            this.ReadBut.Location = new System.Drawing.Point(192, 13);
            this.ReadBut.Name = "ReadBut";
            this.ReadBut.Size = new System.Drawing.Size(81, 29);
            this.ReadBut.TabIndex = 1;
            this.ReadBut.Text = "Read";
            this.ReadBut.UseVisualStyleBackColor = true;
            this.ReadBut.Click += new System.EventHandler(this.ReadBut_Click);
            // 
            // WriteBut
            // 
            this.WriteBut.Location = new System.Drawing.Point(279, 13);
            this.WriteBut.Name = "WriteBut";
            this.WriteBut.Size = new System.Drawing.Size(82, 29);
            this.WriteBut.TabIndex = 2;
            this.WriteBut.Text = "Write";
            this.WriteBut.UseVisualStyleBackColor = true;
            // 
            // RTB
            // 
            this.RTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTB.Location = new System.Drawing.Point(13, 572);
            this.RTB.Name = "RTB";
            this.RTB.Size = new System.Drawing.Size(951, 58);
            this.RTB.TabIndex = 3;
            this.RTB.Text = "";
            // 
            // picBoxRight
            // 
            this.picBoxRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxRight.BackColor = System.Drawing.Color.Black;
            this.picBoxRight.Location = new System.Drawing.Point(12, 126);
            this.picBoxRight.Name = "picBoxRight";
            this.picBoxRight.Size = new System.Drawing.Size(951, 209);
            this.picBoxRight.TabIndex = 4;
            this.picBoxRight.TabStop = false;
            // 
            // picBoxLeft
            // 
            this.picBoxLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxLeft.BackColor = System.Drawing.Color.Black;
            this.picBoxLeft.Location = new System.Drawing.Point(13, 341);
            this.picBoxLeft.Name = "picBoxLeft";
            this.picBoxLeft.Size = new System.Drawing.Size(951, 209);
            this.picBoxLeft.TabIndex = 5;
            this.picBoxLeft.TabStop = false;
            // 
            // HzScBar
            // 
            this.HzScBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HzScBar.Location = new System.Drawing.Point(118, 79);
            this.HzScBar.Maximum = 17009;
            this.HzScBar.Minimum = 10;
            this.HzScBar.Name = "HzScBar";
            this.HzScBar.Size = new System.Drawing.Size(846, 29);
            this.HzScBar.TabIndex = 7;
            this.HzScBar.Value = 440;
            this.HzScBar.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
            this.HzScBar.MouseLeave += new System.EventHandler(this.hScrollBar1_MouseLeave);
            // 
            // AutoPlayChkBox
            // 
            this.AutoPlayChkBox.AutoSize = true;
            this.AutoPlayChkBox.Location = new System.Drawing.Point(452, 20);
            this.AutoPlayChkBox.Name = "AutoPlayChkBox";
            this.AutoPlayChkBox.Size = new System.Drawing.Size(68, 17);
            this.AutoPlayChkBox.TabIndex = 8;
            this.AutoPlayChkBox.Text = "AutoPlay";
            this.AutoPlayChkBox.UseVisualStyleBackColor = true;
            // 
            // HzLabel
            // 
            this.HzLabel.AutoSize = true;
            this.HzLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HzLabel.Location = new System.Drawing.Point(12, 85);
            this.HzLabel.Name = "HzLabel";
            this.HzLabel.Size = new System.Drawing.Size(66, 20);
            this.HzLabel.TabIndex = 9;
            this.HzLabel.Text = "440 Hz";
            // 
            // PlayBtn
            // 
            this.PlayBtn.Location = new System.Drawing.Point(364, 13);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(82, 29);
            this.PlayBtn.TabIndex = 10;
            this.PlayBtn.Text = "Play";
            this.PlayBtn.UseVisualStyleBackColor = true;
            this.PlayBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // VolumhScBar
            // 
            this.VolumhScBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VolumhScBar.Location = new System.Drawing.Point(118, 45);
            this.VolumhScBar.Maximum = 109;
            this.VolumhScBar.Minimum = 1;
            this.VolumhScBar.Name = "VolumhScBar";
            this.VolumhScBar.Size = new System.Drawing.Size(243, 29);
            this.VolumhScBar.TabIndex = 11;
            this.VolumhScBar.Value = 100;
            this.VolumhScBar.ValueChanged += new System.EventHandler(this.VolumhScBar_ValueChanged);
            // 
            // VolumLabel
            // 
            this.VolumLabel.AutoSize = true;
            this.VolumLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.VolumLabel.Location = new System.Drawing.Point(12, 52);
            this.VolumLabel.Name = "VolumLabel";
            this.VolumLabel.Size = new System.Drawing.Size(59, 20);
            this.VolumLabel.TabIndex = 12;
            this.VolumLabel.Text = "100 %";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 642);
            this.Controls.Add(this.VolumLabel);
            this.Controls.Add(this.VolumhScBar);
            this.Controls.Add(this.PlayBtn);
            this.Controls.Add(this.HzLabel);
            this.Controls.Add(this.AutoPlayChkBox);
            this.Controls.Add(this.HzScBar);
            this.Controls.Add(this.picBoxLeft);
            this.Controls.Add(this.picBoxRight);
            this.Controls.Add(this.RTB);
            this.Controls.Add(this.WriteBut);
            this.Controls.Add(this.ReadBut);
            this.Controls.Add(this.btnGenerateWave);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picBoxRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxLeft)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateWave;
        private System.Windows.Forms.Button ReadBut;
        private System.Windows.Forms.Button WriteBut;
        private System.Windows.Forms.RichTextBox RTB;
        private System.Windows.Forms.PictureBox picBoxRight;
        private System.Windows.Forms.PictureBox picBoxLeft;
        private System.Windows.Forms.HScrollBar HzScBar;
        private System.Windows.Forms.CheckBox AutoPlayChkBox;
        private System.Windows.Forms.Label HzLabel;
        private System.Windows.Forms.Button PlayBtn;
        private System.Windows.Forms.HScrollBar VolumhScBar;
        private System.Windows.Forms.Label VolumLabel;
    }
}

