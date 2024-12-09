namespace AlfaPribor.AviFile.Demo
{
    partial class formMain
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
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonChoiceFile = new System.Windows.Forms.Button();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBoxReadOnly = new System.Windows.Forms.GroupBox();
            this.radioButtonReadAndWrite = new System.Windows.Forms.RadioButton();
            this.radioButtonWriteOnly = new System.Windows.Forms.RadioButton();
            this.radioButtonReadOnly = new System.Windows.Forms.RadioButton();
            this.buttonView = new System.Windows.Forms.Button();
            this.buttonViewInfo = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonCompressorChoice = new System.Windows.Forms.Button();
            this.checkBoxCompressStreams = new System.Windows.Forms.CheckBox();
            this.checkBoxDecompressStreams = new System.Windows.Forms.CheckBox();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonSelectSources = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonAppend = new System.Windows.Forms.RadioButton();
            this.radioButtonCreate = new System.Windows.Forms.RadioButton();
            this.buttonOpenDestFile = new System.Windows.Forms.Button();
            this.buttonChoiceDestFile = new System.Windows.Forms.Button();
            this.textBoxDestFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxReadOnly.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Имя файла:";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(15, 25);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(308, 20);
            this.textBoxFileName.TabIndex = 1;
            // 
            // buttonChoiceFile
            // 
            this.buttonChoiceFile.Location = new System.Drawing.Point(329, 23);
            this.buttonChoiceFile.Name = "buttonChoiceFile";
            this.buttonChoiceFile.Size = new System.Drawing.Size(24, 23);
            this.buttonChoiceFile.TabIndex = 2;
            this.buttonChoiceFile.Text = "...";
            this.buttonChoiceFile.UseVisualStyleBackColor = true;
            this.buttonChoiceFile.Click += new System.EventHandler(this.buttonChoiceFile_Click);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(359, 23);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenFile.TabIndex = 3;
            this.buttonOpenFile.Text = "Открыть";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.CheckFileExists = false;
            this.openFileDialog.Filter = "Видео файлы (*.avi)|*.avi|Все файлы (*.*)|*.*";
            this.openFileDialog.ShowReadOnly = true;
            this.openFileDialog.SupportMultiDottedExtensions = true;
            this.openFileDialog.Title = "Открыть файл";
            // 
            // groupBoxReadOnly
            // 
            this.groupBoxReadOnly.Controls.Add(this.radioButtonReadAndWrite);
            this.groupBoxReadOnly.Controls.Add(this.radioButtonWriteOnly);
            this.groupBoxReadOnly.Controls.Add(this.radioButtonReadOnly);
            this.groupBoxReadOnly.Location = new System.Drawing.Point(15, 51);
            this.groupBoxReadOnly.Name = "groupBoxReadOnly";
            this.groupBoxReadOnly.Size = new System.Drawing.Size(308, 99);
            this.groupBoxReadOnly.TabIndex = 4;
            this.groupBoxReadOnly.TabStop = false;
            this.groupBoxReadOnly.Text = "Режим открытия";
            // 
            // radioButtonReadAndWrite
            // 
            this.radioButtonReadAndWrite.AutoSize = true;
            this.radioButtonReadAndWrite.Location = new System.Drawing.Point(22, 65);
            this.radioButtonReadAndWrite.Name = "radioButtonReadAndWrite";
            this.radioButtonReadAndWrite.Size = new System.Drawing.Size(131, 17);
            this.radioButtonReadAndWrite.TabIndex = 2;
            this.radioButtonReadAndWrite.Text = "Для чтения и записи";
            this.radioButtonReadAndWrite.UseVisualStyleBackColor = true;
            // 
            // radioButtonWriteOnly
            // 
            this.radioButtonWriteOnly.AutoSize = true;
            this.radioButtonWriteOnly.Location = new System.Drawing.Point(22, 42);
            this.radioButtonWriteOnly.Name = "radioButtonWriteOnly";
            this.radioButtonWriteOnly.Size = new System.Drawing.Size(122, 17);
            this.radioButtonWriteOnly.TabIndex = 1;
            this.radioButtonWriteOnly.Text = "Только для записи";
            this.radioButtonWriteOnly.UseVisualStyleBackColor = true;
            // 
            // radioButtonReadOnly
            // 
            this.radioButtonReadOnly.AllowDrop = true;
            this.radioButtonReadOnly.AutoSize = true;
            this.radioButtonReadOnly.Checked = true;
            this.radioButtonReadOnly.Location = new System.Drawing.Point(22, 19);
            this.radioButtonReadOnly.Name = "radioButtonReadOnly";
            this.radioButtonReadOnly.Size = new System.Drawing.Size(120, 17);
            this.radioButtonReadOnly.TabIndex = 0;
            this.radioButtonReadOnly.TabStop = true;
            this.radioButtonReadOnly.Text = "Только для чтения";
            this.radioButtonReadOnly.UseVisualStyleBackColor = true;
            // 
            // buttonView
            // 
            this.buttonView.Enabled = false;
            this.buttonView.Location = new System.Drawing.Point(329, 52);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(105, 23);
            this.buttonView.TabIndex = 5;
            this.buttonView.Text = "Просмотр";
            this.buttonView.UseVisualStyleBackColor = true;
            this.buttonView.Click += new System.EventHandler(this.buttonView_Click);
            // 
            // buttonViewInfo
            // 
            this.buttonViewInfo.Enabled = false;
            this.buttonViewInfo.Location = new System.Drawing.Point(329, 81);
            this.buttonViewInfo.Name = "buttonViewInfo";
            this.buttonViewInfo.Size = new System.Drawing.Size(105, 23);
            this.buttonViewInfo.TabIndex = 6;
            this.buttonViewInfo.Text = "Информация";
            this.buttonViewInfo.UseVisualStyleBackColor = true;
            this.buttonViewInfo.Click += new System.EventHandler(this.buttonViewInfo_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonCompressorChoice);
            this.groupBox1.Controls.Add(this.checkBoxCompressStreams);
            this.groupBox1.Controls.Add(this.checkBoxDecompressStreams);
            this.groupBox1.Controls.Add(this.buttonCopy);
            this.groupBox1.Controls.Add(this.buttonSelectSources);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.buttonOpenDestFile);
            this.groupBox1.Controls.Add(this.buttonChoiceDestFile);
            this.groupBox1.Controls.Add(this.textBoxDestFileName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(3, 156);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(450, 202);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки копирования";
            // 
            // buttonCompressorChoice
            // 
            this.buttonCompressorChoice.Enabled = false;
            this.buttonCompressorChoice.Location = new System.Drawing.Point(234, 161);
            this.buttonCompressorChoice.Name = "buttonCompressorChoice";
            this.buttonCompressorChoice.Size = new System.Drawing.Size(86, 23);
            this.buttonCompressorChoice.TabIndex = 13;
            this.buttonCompressorChoice.Text = "Компрессор";
            this.buttonCompressorChoice.UseVisualStyleBackColor = true;
            this.buttonCompressorChoice.Click += new System.EventHandler(this.buttonCompressorChoice_Click);
            // 
            // checkBoxCompressStreams
            // 
            this.checkBoxCompressStreams.AutoSize = true;
            this.checkBoxCompressStreams.Location = new System.Drawing.Point(34, 165);
            this.checkBoxCompressStreams.Name = "checkBoxCompressStreams";
            this.checkBoxCompressStreams.Size = new System.Drawing.Size(194, 17);
            this.checkBoxCompressStreams.TabIndex = 12;
            this.checkBoxCompressStreams.Text = "Сжимать несжатые видеопотоки";
            this.checkBoxCompressStreams.UseVisualStyleBackColor = true;
            this.checkBoxCompressStreams.CheckedChanged += new System.EventHandler(this.checkBoxCompressStreams_CheckedChanged);
            // 
            // checkBoxDecompressStreams
            // 
            this.checkBoxDecompressStreams.AutoSize = true;
            this.checkBoxDecompressStreams.Location = new System.Drawing.Point(34, 142);
            this.checkBoxDecompressStreams.Name = "checkBoxDecompressStreams";
            this.checkBoxDecompressStreams.Size = new System.Drawing.Size(210, 17);
            this.checkBoxDecompressStreams.TabIndex = 11;
            this.checkBoxDecompressStreams.Text = "Декодировать сжатые видеопотоки";
            this.checkBoxDecompressStreams.UseVisualStyleBackColor = true;
            // 
            // buttonCopy
            // 
            this.buttonCopy.Enabled = false;
            this.buttonCopy.Location = new System.Drawing.Point(326, 99);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(104, 23);
            this.buttonCopy.TabIndex = 10;
            this.buttonCopy.Text = "Копировать";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonSelectSources
            // 
            this.buttonSelectSources.Enabled = false;
            this.buttonSelectSources.Location = new System.Drawing.Point(326, 70);
            this.buttonSelectSources.Name = "buttonSelectSources";
            this.buttonSelectSources.Size = new System.Drawing.Size(104, 23);
            this.buttonSelectSources.TabIndex = 9;
            this.buttonSelectSources.Text = "Выбрать потоки";
            this.buttonSelectSources.UseVisualStyleBackColor = true;
            this.buttonSelectSources.Click += new System.EventHandler(this.buttonSelectSources_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonAppend);
            this.groupBox2.Controls.Add(this.radioButtonCreate);
            this.groupBox2.Location = new System.Drawing.Point(13, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(308, 69);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Режим открытия";
            // 
            // radioButtonAppend
            // 
            this.radioButtonAppend.AutoSize = true;
            this.radioButtonAppend.Location = new System.Drawing.Point(22, 42);
            this.radioButtonAppend.Name = "radioButtonAppend";
            this.radioButtonAppend.Size = new System.Drawing.Size(164, 17);
            this.radioButtonAppend.TabIndex = 1;
            this.radioButtonAppend.Text = "Дописать в существующий";
            this.radioButtonAppend.UseVisualStyleBackColor = true;
            // 
            // radioButtonCreate
            // 
            this.radioButtonCreate.AllowDrop = true;
            this.radioButtonCreate.AutoSize = true;
            this.radioButtonCreate.Checked = true;
            this.radioButtonCreate.Location = new System.Drawing.Point(22, 19);
            this.radioButtonCreate.Name = "radioButtonCreate";
            this.radioButtonCreate.Size = new System.Drawing.Size(102, 17);
            this.radioButtonCreate.TabIndex = 0;
            this.radioButtonCreate.TabStop = true;
            this.radioButtonCreate.Text = "Создать новый";
            this.radioButtonCreate.UseVisualStyleBackColor = true;
            // 
            // buttonOpenDestFile
            // 
            this.buttonOpenDestFile.Location = new System.Drawing.Point(356, 41);
            this.buttonOpenDestFile.Name = "buttonOpenDestFile";
            this.buttonOpenDestFile.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenDestFile.TabIndex = 7;
            this.buttonOpenDestFile.Text = "Открыть";
            this.buttonOpenDestFile.UseVisualStyleBackColor = true;
            this.buttonOpenDestFile.Click += new System.EventHandler(this.buttonOpenDestFile_Click);
            // 
            // buttonChoiceDestFile
            // 
            this.buttonChoiceDestFile.Location = new System.Drawing.Point(326, 41);
            this.buttonChoiceDestFile.Name = "buttonChoiceDestFile";
            this.buttonChoiceDestFile.Size = new System.Drawing.Size(24, 23);
            this.buttonChoiceDestFile.TabIndex = 6;
            this.buttonChoiceDestFile.Text = "...";
            this.buttonChoiceDestFile.UseVisualStyleBackColor = true;
            this.buttonChoiceDestFile.Click += new System.EventHandler(this.buttonChoiceDestFile_Click);
            // 
            // textBoxDestFileName
            // 
            this.textBoxDestFileName.Location = new System.Drawing.Point(13, 41);
            this.textBoxDestFileName.Name = "textBoxDestFileName";
            this.textBoxDestFileName.Size = new System.Drawing.Size(308, 20);
            this.textBoxDestFileName.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Имя файла:";
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 363);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonViewInfo);
            this.Controls.Add(this.buttonView);
            this.Controls.Add(this.groupBoxReadOnly);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.buttonChoiceFile);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Демонстрация работы компонента AviFile";
            this.groupBoxReadOnly.ResumeLayout(false);
            this.groupBoxReadOnly.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonChoiceFile;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBoxReadOnly;
        private System.Windows.Forms.RadioButton radioButtonReadOnly;
        private System.Windows.Forms.RadioButton radioButtonReadAndWrite;
        private System.Windows.Forms.RadioButton radioButtonWriteOnly;
        private System.Windows.Forms.Button buttonView;
        private System.Windows.Forms.Button buttonViewInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOpenDestFile;
        private System.Windows.Forms.Button buttonChoiceDestFile;
        private System.Windows.Forms.TextBox textBoxDestFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonAppend;
        private System.Windows.Forms.RadioButton radioButtonCreate;
        private System.Windows.Forms.Button buttonSelectSources;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.CheckBox checkBoxCompressStreams;
        private System.Windows.Forms.CheckBox checkBoxDecompressStreams;
        private System.Windows.Forms.Button buttonCompressorChoice;
    }
}

