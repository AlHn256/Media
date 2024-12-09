namespace FramesPlayer
{
    partial class FormAutoExportSettings
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
            this.cbUseAutoViewVideoExport = new System.Windows.Forms.CheckBox();
            this.tbVideoCadrsStorragePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudExportVideoStep = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudMaxVideoFrame = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bSelectVideoFramesPath = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbUseImageResize = new System.Windows.Forms.CheckBox();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.bSaveData = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.nudChannelID = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudExportVideoStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxVideoFrame)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudChannelID)).BeginInit();
            this.SuspendLayout();
            // 
            // cbUseAutoViewVideoExport
            // 
            this.cbUseAutoViewVideoExport.AutoSize = true;
            this.cbUseAutoViewVideoExport.Location = new System.Drawing.Point(15, 19);
            this.cbUseAutoViewVideoExport.Name = "cbUseAutoViewVideoExport";
            this.cbUseAutoViewVideoExport.Size = new System.Drawing.Size(219, 17);
            this.cbUseAutoViewVideoExport.TabIndex = 0;
            this.cbUseAutoViewVideoExport.Text = "Задействовать экспорт кадров видео";
            this.cbUseAutoViewVideoExport.UseVisualStyleBackColor = true;
            // 
            // tbVideoCadrsStorragePath
            // 
            this.tbVideoCadrsStorragePath.Location = new System.Drawing.Point(122, 48);
            this.tbVideoCadrsStorragePath.Name = "tbVideoCadrsStorragePath";
            this.tbVideoCadrsStorragePath.Size = new System.Drawing.Size(213, 20);
            this.tbVideoCadrsStorragePath.TabIndex = 1;
            this.tbVideoCadrsStorragePath.Text = "D:\\FramesPlayerImages";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "Каталог экспорита\r\nкадров";
            // 
            // nudExportVideoStep
            // 
            this.nudExportVideoStep.Location = new System.Drawing.Point(276, 85);
            this.nudExportVideoStep.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudExportVideoStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudExportVideoStep.Name = "nudExportVideoStep";
            this.nudExportVideoStep.Size = new System.Drawing.Size(59, 20);
            this.nudExportVideoStep.TabIndex = 3;
            this.nudExportVideoStep.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Шаг экспорита кадров видео";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(223, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Максимальное колличество кадров видео";
            // 
            // nudMaxVideoFrame
            // 
            this.nudMaxVideoFrame.Location = new System.Drawing.Point(276, 124);
            this.nudMaxVideoFrame.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaxVideoFrame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.nudMaxVideoFrame.Name = "nudMaxVideoFrame";
            this.nudMaxVideoFrame.Size = new System.Drawing.Size(59, 20);
            this.nudMaxVideoFrame.TabIndex = 5;
            this.nudMaxVideoFrame.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.nudChannelID);
            this.groupBox1.Controls.Add(this.bSelectVideoFramesPath);
            this.groupBox1.Controls.Add(this.cbUseAutoViewVideoExport);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.nudMaxVideoFrame);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbVideoCadrsStorragePath);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudExportVideoStep);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(418, 193);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Основные настройки экспорта кадров";
            // 
            // bSelectVideoFramesPath
            // 
            this.bSelectVideoFramesPath.Location = new System.Drawing.Point(342, 47);
            this.bSelectVideoFramesPath.Name = "bSelectVideoFramesPath";
            this.bSelectVideoFramesPath.Size = new System.Drawing.Size(48, 23);
            this.bSelectVideoFramesPath.TabIndex = 7;
            this.bSelectVideoFramesPath.Text = "...";
            this.bSelectVideoFramesPath.UseVisualStyleBackColor = true;
            this.bSelectVideoFramesPath.Click += new System.EventHandler(this.bSelectVideoFramesPath_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cbUseImageResize);
            this.groupBox2.Controls.Add(this.nudHeight);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.nudWidth);
            this.groupBox2.Location = new System.Drawing.Point(13, 221);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(417, 136);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Параметры разрешения";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Высота кадра";
            // 
            // cbUseImageResize
            // 
            this.cbUseImageResize.AutoSize = true;
            this.cbUseImageResize.Location = new System.Drawing.Point(6, 19);
            this.cbUseImageResize.Name = "cbUseImageResize";
            this.cbUseImageResize.Size = new System.Drawing.Size(227, 17);
            this.cbUseImageResize.TabIndex = 8;
            this.cbUseImageResize.Text = "Задействовать изменение разрешения";
            this.cbUseImageResize.UseVisualStyleBackColor = true;
            // 
            // nudHeight
            // 
            this.nudHeight.Location = new System.Drawing.Point(275, 83);
            this.nudHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(59, 20);
            this.nudHeight.TabIndex = 10;
            this.nudHeight.Value = new decimal(new int[] {
            960,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Ширина кадра";
            // 
            // nudWidth
            // 
            this.nudWidth.Location = new System.Drawing.Point(275, 44);
            this.nudWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(59, 20);
            this.nudWidth.TabIndex = 8;
            this.nudWidth.Value = new decimal(new int[] {
            1280,
            0,
            0,
            0});
            // 
            // bSaveData
            // 
            this.bSaveData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bSaveData.Location = new System.Drawing.Point(19, 375);
            this.bSaveData.Name = "bSaveData";
            this.bSaveData.Size = new System.Drawing.Size(411, 49);
            this.bSaveData.TabIndex = 9;
            this.bSaveData.Text = "Применить";
            this.bSaveData.UseVisualStyleBackColor = true;
            this.bSaveData.Click += new System.EventHandler(this.bSaveData_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Номер канала";
            // 
            // nudChannelID
            // 
            this.nudChannelID.Location = new System.Drawing.Point(276, 158);
            this.nudChannelID.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nudChannelID.Name = "nudChannelID";
            this.nudChannelID.Size = new System.Drawing.Size(59, 20);
            this.nudChannelID.TabIndex = 8;
            // 
            // FormAutoExportSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 445);
            this.Controls.Add(this.bSaveData);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormAutoExportSettings";
            this.Text = "Настройки автоматического экспорта видео";
            ((System.ComponentModel.ISupportInitialize)(this.nudExportVideoStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxVideoFrame)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudChannelID)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbUseAutoViewVideoExport;
        private System.Windows.Forms.TextBox tbVideoCadrsStorragePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudExportVideoStep;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudMaxVideoFrame;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bSelectVideoFramesPath;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbUseImageResize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.Button bSaveData;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudChannelID;
    }
}