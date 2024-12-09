namespace AlfaPribor.AviFile.VideoExtractorDemo
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSourceFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDestFileName = new System.Windows.Forms.TextBox();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelHint = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBarExtract = new System.Windows.Forms.ToolStripProgressBar();
            this.buttonChooseDest = new System.Windows.Forms.Button();
            this.buttonChooseSource = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownBufferSize = new System.Windows.Forms.NumericUpDown();
            this.checkBoxDefaultBufferSize = new System.Windows.Forms.CheckBox();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBufferSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Источник";
            // 
            // textBoxSourceFileName
            // 
            this.textBoxSourceFileName.Location = new System.Drawing.Point(12, 25);
            this.textBoxSourceFileName.Name = "textBoxSourceFileName";
            this.textBoxSourceFileName.Size = new System.Drawing.Size(271, 20);
            this.textBoxSourceFileName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Приемник";
            // 
            // textBoxDestFileName
            // 
            this.textBoxDestFileName.Location = new System.Drawing.Point(12, 64);
            this.textBoxDestFileName.Name = "textBoxDestFileName";
            this.textBoxDestFileName.Size = new System.Drawing.Size(271, 20);
            this.textBoxDestFileName.TabIndex = 4;
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(12, 136);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(75, 23);
            this.buttonSettings.TabIndex = 6;
            this.buttonSettings.Text = "Настройки";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(208, 136);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(75, 23);
            this.buttonExport.TabIndex = 7;
            this.buttonExport.Text = "Экспорт";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelHint,
            this.toolStripProgressBarExtract});
            this.statusStrip.Location = new System.Drawing.Point(0, 162);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(326, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 9;
            // 
            // toolStripStatusLabelHint
            // 
            this.toolStripStatusLabelHint.Name = "toolStripStatusLabelHint";
            this.toolStripStatusLabelHint.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripProgressBarExtract
            // 
            this.toolStripProgressBarExtract.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBarExtract.AutoToolTip = true;
            this.toolStripProgressBarExtract.Name = "toolStripProgressBarExtract";
            this.toolStripProgressBarExtract.Size = new System.Drawing.Size(90, 16);
            this.toolStripProgressBarExtract.Visible = false;
            // 
            // buttonChooseDest
            // 
            this.buttonChooseDest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonChooseDest.Image = global::AlfaPribor.AviFile.VideoExtractorDemo.Properties.Resources.windows_explorer_16x16_XP;
            this.buttonChooseDest.Location = new System.Drawing.Point(289, 62);
            this.buttonChooseDest.Name = "buttonChooseDest";
            this.buttonChooseDest.Size = new System.Drawing.Size(25, 25);
            this.buttonChooseDest.TabIndex = 5;
            this.buttonChooseDest.UseVisualStyleBackColor = true;
            this.buttonChooseDest.Click += new System.EventHandler(this.buttonChooseDest_Click);
            // 
            // buttonChooseSource
            // 
            this.buttonChooseSource.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonChooseSource.Image = global::AlfaPribor.AviFile.VideoExtractorDemo.Properties.Resources.windows_explorer_16x16_XP;
            this.buttonChooseSource.Location = new System.Drawing.Point(289, 23);
            this.buttonChooseSource.Name = "buttonChooseSource";
            this.buttonChooseSource.Size = new System.Drawing.Size(25, 25);
            this.buttonChooseSource.TabIndex = 2;
            this.buttonChooseSource.UseVisualStyleBackColor = true;
            this.buttonChooseSource.Click += new System.EventHandler(this.buttonChooseSource_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "avi";
            this.openFileDialog.Filter = "Видеофайлы AVI (*.avi)|*.avi|Все файлы (*.*)|*.*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Размер буфера, Мб";
            // 
            // numericUpDownBufferSize
            // 
            this.numericUpDownBufferSize.Location = new System.Drawing.Point(208, 90);
            this.numericUpDownBufferSize.Name = "numericUpDownBufferSize";
            this.numericUpDownBufferSize.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownBufferSize.TabIndex = 11;
            this.numericUpDownBufferSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // checkBoxDefaultBufferSize
            // 
            this.checkBoxDefaultBufferSize.AutoSize = true;
            this.checkBoxDefaultBufferSize.Location = new System.Drawing.Point(15, 113);
            this.checkBoxDefaultBufferSize.Name = "checkBoxDefaultBufferSize";
            this.checkBoxDefaultBufferSize.Size = new System.Drawing.Size(254, 17);
            this.checkBoxDefaultBufferSize.TabIndex = 12;
            this.checkBoxDefaultBufferSize.Text = "Использовать размер буфера по умолчанию";
            this.checkBoxDefaultBufferSize.UseVisualStyleBackColor = true;
            this.checkBoxDefaultBufferSize.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 184);
            this.Controls.Add(this.checkBoxDefaultBufferSize);
            this.Controls.Add(this.numericUpDownBufferSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonChooseDest);
            this.Controls.Add(this.textBoxDestFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonChooseSource);
            this.Controls.Add(this.textBoxSourceFileName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AVI video extractor";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBufferSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSourceFileName;
        private System.Windows.Forms.Button buttonChooseSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDestFileName;
        private System.Windows.Forms.Button buttonChooseDest;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarExtract;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelHint;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownBufferSize;
        private System.Windows.Forms.CheckBox checkBoxDefaultBufferSize;

    }
}

