namespace AlfaPribor.VideoStorage.Client.Demo
{
    partial class FormClient
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonApplySettings = new System.Windows.Forms.Button();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRecordId = new System.Windows.Forms.TextBox();
            this.checkBoxUsePartitionId = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.numericUpDownPartitionId = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxRelativeUri = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPartitionId)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxRelativeUri);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.buttonApplySettings);
            this.groupBox1.Controls.Add(this.numericUpDownPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxAddress);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 149);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки";
            // 
            // buttonApplySettings
            // 
            this.buttonApplySettings.Location = new System.Drawing.Point(214, 110);
            this.buttonApplySettings.Name = "buttonApplySettings";
            this.buttonApplySettings.Size = new System.Drawing.Size(132, 23);
            this.buttonApplySettings.TabIndex = 4;
            this.buttonApplySettings.Text = "Применить";
            this.buttonApplySettings.UseVisualStyleBackColor = true;
            this.buttonApplySettings.Click += new System.EventHandler(this.buttonApplySettings_Click);
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(214, 58);
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            this.numericUpDownPort.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownPort.TabIndex = 3;
            this.numericUpDownPort.Value = new decimal(new int[] {
            8080,
            0,
            0,
            0});
            this.numericUpDownPort.ValueChanged += new System.EventHandler(this.textBoxAddress_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Номер сетевого порта";
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(214, 32);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(132, 20);
            this.textBoxAddress.TabIndex = 1;
            this.textBoxAddress.Text = "localhost";
            this.textBoxAddress.TextChanged += new System.EventHandler(this.textBoxAddress_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Сетевой адрес сервера";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Идентификатор видеозаписи";
            // 
            // textBoxRecordId
            // 
            this.textBoxRecordId.Location = new System.Drawing.Point(226, 171);
            this.textBoxRecordId.Name = "textBoxRecordId";
            this.textBoxRecordId.Size = new System.Drawing.Size(132, 20);
            this.textBoxRecordId.TabIndex = 2;
            // 
            // checkBoxUsePartitionId
            // 
            this.checkBoxUsePartitionId.AutoSize = true;
            this.checkBoxUsePartitionId.Location = new System.Drawing.Point(12, 198);
            this.checkBoxUsePartitionId.Name = "checkBoxUsePartitionId";
            this.checkBoxUsePartitionId.Size = new System.Drawing.Size(213, 17);
            this.checkBoxUsePartitionId.TabIndex = 3;
            this.checkBoxUsePartitionId.Text = " Идентификатор раздела хранилища";
            this.checkBoxUsePartitionId.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(226, 223);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(132, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Открыть плеер";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // numericUpDownPartitionId
            // 
            this.numericUpDownPartitionId.Location = new System.Drawing.Point(226, 197);
            this.numericUpDownPartitionId.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownPartitionId.Name = "numericUpDownPartitionId";
            this.numericUpDownPartitionId.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownPartitionId.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Относительный URI";
            // 
            // textBoxRelativeUri
            // 
            this.textBoxRelativeUri.Location = new System.Drawing.Point(214, 84);
            this.textBoxRelativeUri.Name = "textBoxRelativeUri";
            this.textBoxRelativeUri.Size = new System.Drawing.Size(132, 20);
            this.textBoxRelativeUri.TabIndex = 6;
            this.textBoxRelativeUri.Text = "VideoStorageServer";
            this.textBoxRelativeUri.TextChanged += new System.EventHandler(this.textBoxAddress_TextChanged);
            // 
            // FormClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 256);
            this.Controls.Add(this.numericUpDownPartitionId);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBoxUsePartitionId);
            this.Controls.Add(this.textBoxRecordId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VideoStorage client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClient_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPartitionId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxRecordId;
        private System.Windows.Forms.CheckBox checkBoxUsePartitionId;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.NumericUpDown numericUpDownPartitionId;
        private System.Windows.Forms.Button buttonApplySettings;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxRelativeUri;
    }
}

