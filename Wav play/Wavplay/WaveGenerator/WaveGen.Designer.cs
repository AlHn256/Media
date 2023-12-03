
namespace Wavplay.WaveGenerator
{
    partial class WaveGen
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
            this.VolumLabel = new System.Windows.Forms.Label();
            this.VolumhScBar = new System.Windows.Forms.HScrollBar();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.HzLabel = new System.Windows.Forms.Label();
            this.AutoPlayChkBox = new System.Windows.Forms.CheckBox();
            this.HzScBar = new System.Windows.Forms.HScrollBar();
            this.picBoxLeft = new System.Windows.Forms.PictureBox();
            this.picBoxRight = new System.Windows.Forms.PictureBox();
            this.RTB = new System.Windows.Forms.RichTextBox();
            this.btnGenerateWave = new System.Windows.Forms.Button();
            this.HzTextBox = new System.Windows.Forms.TextBox();
            this.DurationTextBox = new System.Windows.Forms.TextBox();
            this.DurationLab = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxRight)).BeginInit();
            this.SuspendLayout();
            // 
            // VolumLabel
            // 
            this.VolumLabel.AutoSize = true;
            this.VolumLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.VolumLabel.Location = new System.Drawing.Point(3, 43);
            this.VolumLabel.Name = "VolumLabel";
            this.VolumLabel.Size = new System.Drawing.Size(59, 20);
            this.VolumLabel.TabIndex = 24;
            this.VolumLabel.Text = "100 %";
            // 
            // VolumhScBar
            // 
            this.VolumhScBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VolumhScBar.Location = new System.Drawing.Point(142, 36);
            this.VolumhScBar.Maximum = 109;
            this.VolumhScBar.Minimum = 1;
            this.VolumhScBar.Name = "VolumhScBar";
            this.VolumhScBar.Size = new System.Drawing.Size(225, 29);
            this.VolumhScBar.TabIndex = 23;
            this.VolumhScBar.Value = 100;
            this.VolumhScBar.ValueChanged += new System.EventHandler(this.VolumhScBar_ValueChanged);
            // 
            // PlayBtn
            // 
            this.PlayBtn.Location = new System.Drawing.Point(183, 4);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(82, 29);
            this.PlayBtn.TabIndex = 22;
            this.PlayBtn.Text = "Play";
            this.PlayBtn.UseVisualStyleBackColor = true;
            this.PlayBtn.Click += new System.EventHandler(this.PlayBtn_Click_1);
            // 
            // HzLabel
            // 
            this.HzLabel.AutoSize = true;
            this.HzLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HzLabel.Location = new System.Drawing.Point(108, 76);
            this.HzLabel.Name = "HzLabel";
            this.HzLabel.Size = new System.Drawing.Size(31, 20);
            this.HzLabel.TabIndex = 21;
            this.HzLabel.Text = "Hz";
            // 
            // AutoPlayChkBox
            // 
            this.AutoPlayChkBox.AutoSize = true;
            this.AutoPlayChkBox.Location = new System.Drawing.Point(271, 11);
            this.AutoPlayChkBox.Name = "AutoPlayChkBox";
            this.AutoPlayChkBox.Size = new System.Drawing.Size(68, 17);
            this.AutoPlayChkBox.TabIndex = 20;
            this.AutoPlayChkBox.Text = "AutoPlay";
            this.AutoPlayChkBox.UseVisualStyleBackColor = true;
            // 
            // HzScBar
            // 
            this.HzScBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HzScBar.Location = new System.Drawing.Point(142, 70);
            this.HzScBar.Maximum = 17009;
            this.HzScBar.Minimum = 10;
            this.HzScBar.Name = "HzScBar";
            this.HzScBar.Size = new System.Drawing.Size(795, 29);
            this.HzScBar.TabIndex = 19;
            this.HzScBar.Value = 440;
            this.HzScBar.ValueChanged += new System.EventHandler(this.HzScBar_ValueChanged);
            this.HzScBar.MouseLeave += new System.EventHandler(this.HzScBar_MouseLeave);
            // 
            // picBoxLeft
            // 
            this.picBoxLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxLeft.BackColor = System.Drawing.Color.Black;
            this.picBoxLeft.Location = new System.Drawing.Point(4, 330);
            this.picBoxLeft.Name = "picBoxLeft";
            this.picBoxLeft.Size = new System.Drawing.Size(933, 209);
            this.picBoxLeft.TabIndex = 18;
            this.picBoxLeft.TabStop = false;
            // 
            // picBoxRight
            // 
            this.picBoxRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxRight.BackColor = System.Drawing.Color.Black;
            this.picBoxRight.Location = new System.Drawing.Point(4, 111);
            this.picBoxRight.Name = "picBoxRight";
            this.picBoxRight.Size = new System.Drawing.Size(933, 209);
            this.picBoxRight.TabIndex = 17;
            this.picBoxRight.TabStop = false;
            // 
            // RTB
            // 
            this.RTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTB.Location = new System.Drawing.Point(4, 545);
            this.RTB.Name = "RTB";
            this.RTB.Size = new System.Drawing.Size(933, 76);
            this.RTB.TabIndex = 16;
            this.RTB.Text = "";
            // 
            // btnGenerateWave
            // 
            this.btnGenerateWave.Location = new System.Drawing.Point(4, 4);
            this.btnGenerateWave.Name = "btnGenerateWave";
            this.btnGenerateWave.Size = new System.Drawing.Size(173, 29);
            this.btnGenerateWave.TabIndex = 13;
            this.btnGenerateWave.Text = "Generate";
            this.btnGenerateWave.UseVisualStyleBackColor = true;
            this.btnGenerateWave.Click += new System.EventHandler(this.btnGenerateWave_Click);
            // 
            // HzTextBox
            // 
            this.HzTextBox.Location = new System.Drawing.Point(4, 75);
            this.HzTextBox.Name = "HzTextBox";
            this.HzTextBox.Size = new System.Drawing.Size(100, 20);
            this.HzTextBox.TabIndex = 25;
            this.HzTextBox.Text = "440";
            this.HzTextBox.TextChanged += new System.EventHandler(this.HzTextBox_TextChanged);
            // 
            // DurationTextBox
            // 
            this.DurationTextBox.Location = new System.Drawing.Point(435, 10);
            this.DurationTextBox.Name = "DurationTextBox";
            this.DurationTextBox.Size = new System.Drawing.Size(48, 20);
            this.DurationTextBox.TabIndex = 26;
            this.DurationTextBox.Text = "1";
            this.DurationTextBox.TextChanged += new System.EventHandler(this.DurationTextBox_TextChanged);
            // 
            // DurationLab
            // 
            this.DurationLab.AutoSize = true;
            this.DurationLab.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DurationLab.Location = new System.Drawing.Point(345, 9);
            this.DurationLab.Name = "DurationLab";
            this.DurationLab.Size = new System.Drawing.Size(89, 20);
            this.DurationLab.TabIndex = 27;
            this.DurationLab.Text = "Duration -";
            // 
            // WaveGen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 625);
            this.Controls.Add(this.DurationLab);
            this.Controls.Add(this.DurationTextBox);
            this.Controls.Add(this.HzTextBox);
            this.Controls.Add(this.VolumLabel);
            this.Controls.Add(this.VolumhScBar);
            this.Controls.Add(this.PlayBtn);
            this.Controls.Add(this.HzLabel);
            this.Controls.Add(this.AutoPlayChkBox);
            this.Controls.Add(this.HzScBar);
            this.Controls.Add(this.picBoxLeft);
            this.Controls.Add(this.picBoxRight);
            this.Controls.Add(this.RTB);
            this.Controls.Add(this.btnGenerateWave);
            this.Name = "WaveGen";
            this.Text = "WaveGen";
            ((System.ComponentModel.ISupportInitialize)(this.picBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxRight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label VolumLabel;
        private System.Windows.Forms.HScrollBar VolumhScBar;
        private System.Windows.Forms.Button PlayBtn;
        private System.Windows.Forms.Label HzLabel;
        private System.Windows.Forms.CheckBox AutoPlayChkBox;
        private System.Windows.Forms.HScrollBar HzScBar;
        private System.Windows.Forms.PictureBox picBoxLeft;
        private System.Windows.Forms.PictureBox picBoxRight;
        private System.Windows.Forms.RichTextBox RTB;
        private System.Windows.Forms.Button btnGenerateWave;
        private System.Windows.Forms.TextBox HzTextBox;
        private System.Windows.Forms.TextBox DurationTextBox;
        private System.Windows.Forms.Label DurationLab;
    }
}