namespace AlfaPribor.AviFile.VideoExtractorDemo
{
    partial class AviExportProperties
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSamplesCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownStartSample = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownEndSample = new System.Windows.Forms.NumericUpDown();
            this.checkBoxCompress = new System.Windows.Forms.CheckBox();
            this.buttonChooseCodec = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndSample)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Файл";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(12, 25);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.ReadOnly = true;
            this.textBoxFileName.Size = new System.Drawing.Size(268, 20);
            this.textBoxFileName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Всего кадров";
            // 
            // textBoxSamplesCount
            // 
            this.textBoxSamplesCount.Location = new System.Drawing.Point(180, 57);
            this.textBoxSamplesCount.Name = "textBoxSamplesCount";
            this.textBoxSamplesCount.ReadOnly = true;
            this.textBoxSamplesCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxSamplesCount.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Начало фрагмента";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Конец фрагмента";
            // 
            // numericUpDownStartSample
            // 
            this.numericUpDownStartSample.Location = new System.Drawing.Point(180, 84);
            this.numericUpDownStartSample.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownStartSample.Name = "numericUpDownStartSample";
            this.numericUpDownStartSample.Size = new System.Drawing.Size(100, 20);
            this.numericUpDownStartSample.TabIndex = 7;
            this.numericUpDownStartSample.ThousandsSeparator = true;
            // 
            // numericUpDownEndSample
            // 
            this.numericUpDownEndSample.Location = new System.Drawing.Point(180, 110);
            this.numericUpDownEndSample.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownEndSample.Name = "numericUpDownEndSample";
            this.numericUpDownEndSample.Size = new System.Drawing.Size(100, 20);
            this.numericUpDownEndSample.TabIndex = 8;
            this.numericUpDownEndSample.ThousandsSeparator = true;
            // 
            // checkBoxCompress
            // 
            this.checkBoxCompress.AutoSize = true;
            this.checkBoxCompress.Location = new System.Drawing.Point(15, 141);
            this.checkBoxCompress.Name = "checkBoxCompress";
            this.checkBoxCompress.Size = new System.Drawing.Size(136, 17);
            this.checkBoxCompress.TabIndex = 9;
            this.checkBoxCompress.Text = "Пересжать фрагмент";
            this.checkBoxCompress.UseVisualStyleBackColor = true;
            // 
            // buttonChooseCodec
            // 
            this.buttonChooseCodec.Location = new System.Drawing.Point(180, 137);
            this.buttonChooseCodec.Name = "buttonChooseCodec";
            this.buttonChooseCodec.Size = new System.Drawing.Size(100, 23);
            this.buttonChooseCodec.TabIndex = 10;
            this.buttonChooseCodec.Text = "Выбрать кодек";
            this.buttonChooseCodec.UseVisualStyleBackColor = true;
            this.buttonChooseCodec.Click += new System.EventHandler(this.buttonChooseCodec_Click);
            // 
            // AviExportProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 170);
            this.Controls.Add(this.buttonChooseCodec);
            this.Controls.Add(this.checkBoxCompress);
            this.Controls.Add(this.numericUpDownEndSample);
            this.Controls.Add(this.numericUpDownStartSample);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxSamplesCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AviExportProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки экспорта";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndSample)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSamplesCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownStartSample;
        private System.Windows.Forms.NumericUpDown numericUpDownEndSample;
        private System.Windows.Forms.CheckBox checkBoxCompress;
        private System.Windows.Forms.Button buttonChooseCodec;
    }
}