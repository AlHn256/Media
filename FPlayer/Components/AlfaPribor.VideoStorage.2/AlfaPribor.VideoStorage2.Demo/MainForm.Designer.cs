namespace AlfaPribor.VideoStorage.Demo
{
    partial class MainForm
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
            this.chbWriteDuplicates = new System.Windows.Forms.CheckBox();
            this.numCheckInterval = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.gbxPartitionProperties = new System.Windows.Forms.GroupBox();
            this.btnGetFreeSpace = new System.Windows.Forms.Button();
            this.txtbId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chbxActive = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numFreeSpace = new System.Windows.Forms.NumericUpDown();
            this.btnChoicePath = new System.Windows.Forms.Button();
            this.txtbPartitionPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lvwPartitions = new System.Windows.Forms.ListView();
            this.clhId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhFreeSpaceLimit = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhActive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.slblMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRewrite = new System.Windows.Forms.Button();
            this.btnGetInfo = new System.Windows.Forms.Button();
            this.btnSynchronize = new System.Windows.Forms.Button();
            this.btnSetSettings = new System.Windows.Forms.Button();
            this.btnGetSettings = new System.Windows.Forms.Button();
            this.btnDeleteRecord = new System.Windows.Forms.Button();
            this.btnReadRecord = new System.Windows.Forms.Button();
            this.btnWriteRecord = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckInterval)).BeginInit();
            this.gbxPartitionProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFreeSpace)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chbWriteDuplicates);
            this.groupBox1.Controls.Add(this.numCheckInterval);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnApply);
            this.groupBox1.Controls.Add(this.btnChange);
            this.groupBox1.Controls.Add(this.gbxPartitionProperties);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.lvwPartitions);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(11, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(407, 391);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки";
            // 
            // chbWriteDuplicates
            // 
            this.chbWriteDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbWriteDuplicates.AutoSize = true;
            this.chbWriteDuplicates.Location = new System.Drawing.Point(19, 359);
            this.chbWriteDuplicates.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.chbWriteDuplicates.Name = "chbWriteDuplicates";
            this.chbWriteDuplicates.Size = new System.Drawing.Size(281, 17);
            this.chbWriteDuplicates.TabIndex = 14;
            this.chbWriteDuplicates.Text = "Разрешить помещать записи с существующим ID";
            this.chbWriteDuplicates.UseVisualStyleBackColor = true;
            // 
            // numCheckInterval
            // 
            this.numCheckInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numCheckInterval.Location = new System.Drawing.Point(333, 334);
            this.numCheckInterval.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.numCheckInterval.Name = "numCheckInterval";
            this.numCheckInterval.Size = new System.Drawing.Size(57, 20);
            this.numCheckInterval.TabIndex = 11;
            this.numCheckInterval.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 336);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(298, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Интервал проверки свободного места в хранилище (сек)";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(294, 151);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Отменить";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(294, 122);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(96, 23);
            this.btnApply.TabIndex = 8;
            this.btnApply.Text = "Применить";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnChange
            // 
            this.btnChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChange.Enabled = false;
            this.btnChange.Location = new System.Drawing.Point(294, 93);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(96, 23);
            this.btnChange.TabIndex = 7;
            this.btnChange.Text = "Изменить";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // gbxPartitionProperties
            // 
            this.gbxPartitionProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxPartitionProperties.Controls.Add(this.btnGetFreeSpace);
            this.gbxPartitionProperties.Controls.Add(this.txtbId);
            this.gbxPartitionProperties.Controls.Add(this.label5);
            this.gbxPartitionProperties.Controls.Add(this.chbxActive);
            this.gbxPartitionProperties.Controls.Add(this.label3);
            this.gbxPartitionProperties.Controls.Add(this.numFreeSpace);
            this.gbxPartitionProperties.Controls.Add(this.btnChoicePath);
            this.gbxPartitionProperties.Controls.Add(this.txtbPartitionPath);
            this.gbxPartitionProperties.Controls.Add(this.label2);
            this.gbxPartitionProperties.Enabled = false;
            this.gbxPartitionProperties.Location = new System.Drawing.Point(19, 180);
            this.gbxPartitionProperties.Name = "gbxPartitionProperties";
            this.gbxPartitionProperties.Size = new System.Drawing.Size(371, 148);
            this.gbxPartitionProperties.TabIndex = 6;
            this.gbxPartitionProperties.TabStop = false;
            this.gbxPartitionProperties.Text = "Раздел";
            // 
            // btnGetFreeSpace
            // 
            this.btnGetFreeSpace.Location = new System.Drawing.Point(334, 65);
            this.btnGetFreeSpace.Name = "btnGetFreeSpace";
            this.btnGetFreeSpace.Size = new System.Drawing.Size(26, 20);
            this.btnGetFreeSpace.TabIndex = 8;
            this.btnGetFreeSpace.Text = "?";
            this.btnGetFreeSpace.UseVisualStyleBackColor = true;
            this.btnGetFreeSpace.Click += new System.EventHandler(this.btnGetFreeSpace_Click);
            // 
            // txtbId
            // 
            this.txtbId.Location = new System.Drawing.Point(245, 96);
            this.txtbId.Name = "txtbId";
            this.txtbId.ReadOnly = true;
            this.txtbId.Size = new System.Drawing.Size(84, 20);
            this.txtbId.TabIndex = 7;
            this.txtbId.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Идентификатор раздела";
            // 
            // chbxActive
            // 
            this.chbxActive.AutoSize = true;
            this.chbxActive.Location = new System.Drawing.Point(16, 123);
            this.chbxActive.Name = "chbxActive";
            this.chbxActive.Size = new System.Drawing.Size(115, 17);
            this.chbxActive.TabIndex = 5;
            this.chbxActive.Text = "Активный раздел";
            this.chbxActive.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(226, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Лимит свободного места на диске (МБайт)";
            // 
            // numFreeSpace
            // 
            this.numFreeSpace.Location = new System.Drawing.Point(245, 65);
            this.numFreeSpace.Maximum = new decimal(new int[] {
            320000000,
            0,
            0,
            0});
            this.numFreeSpace.Name = "numFreeSpace";
            this.numFreeSpace.Size = new System.Drawing.Size(84, 20);
            this.numFreeSpace.TabIndex = 3;
            this.numFreeSpace.ThousandsSeparator = true;
            // 
            // btnChoicePath
            // 
            this.btnChoicePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChoicePath.Location = new System.Drawing.Point(335, 32);
            this.btnChoicePath.Name = "btnChoicePath";
            this.btnChoicePath.Size = new System.Drawing.Size(25, 20);
            this.btnChoicePath.TabIndex = 2;
            this.btnChoicePath.Text = "...";
            this.btnChoicePath.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnChoicePath.UseVisualStyleBackColor = true;
            this.btnChoicePath.Click += new System.EventHandler(this.btnChoicePath_Click);
            // 
            // txtbPartitionPath
            // 
            this.txtbPartitionPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbPartitionPath.Location = new System.Drawing.Point(16, 32);
            this.txtbPartitionPath.Name = "txtbPartitionPath";
            this.txtbPartitionPath.Size = new System.Drawing.Size(313, 20);
            this.txtbPartitionPath.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Путь";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(294, 64);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(96, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(294, 35);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(96, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lvwPartitions
            // 
            this.lvwPartitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwPartitions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhId,
            this.clhPath,
            this.clhFreeSpaceLimit,
            this.clhActive});
            this.lvwPartitions.FullRowSelect = true;
            this.lvwPartitions.GridLines = true;
            this.lvwPartitions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwPartitions.LabelWrap = false;
            this.lvwPartitions.Location = new System.Drawing.Point(19, 35);
            this.lvwPartitions.MultiSelect = false;
            this.lvwPartitions.Name = "lvwPartitions";
            this.lvwPartitions.Size = new System.Drawing.Size(269, 139);
            this.lvwPartitions.TabIndex = 3;
            this.lvwPartitions.UseCompatibleStateImageBehavior = false;
            this.lvwPartitions.View = System.Windows.Forms.View.Details;
            // 
            // clhId
            // 
            this.clhId.Text = "ID";
            this.clhId.Width = 36;
            // 
            // clhPath
            // 
            this.clhPath.Text = "Путь";
            this.clhPath.Width = 228;
            // 
            // clhFreeSpaceLimit
            // 
            this.clhFreeSpaceLimit.Text = "Лимит места (МБ)";
            this.clhFreeSpaceLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.clhFreeSpaceLimit.Width = 100;
            // 
            // clhActive
            // 
            this.clhActive.Text = "Активный";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Разделы";
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Выберите папку для раздела хранилища";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slblMessage});
            this.statusStrip.Location = new System.Drawing.Point(0, 435);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(672, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // slblMessage
            // 
            this.slblMessage.Name = "slblMessage";
            this.slblMessage.Size = new System.Drawing.Size(0, 17);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnRewrite);
            this.groupBox2.Controls.Add(this.btnGetInfo);
            this.groupBox2.Controls.Add(this.btnSynchronize);
            this.groupBox2.Controls.Add(this.btnSetSettings);
            this.groupBox2.Controls.Add(this.btnGetSettings);
            this.groupBox2.Controls.Add(this.btnDeleteRecord);
            this.groupBox2.Controls.Add(this.btnReadRecord);
            this.groupBox2.Controls.Add(this.btnWriteRecord);
            this.groupBox2.Location = new System.Drawing.Point(424, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(236, 391);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Действия";
            // 
            // btnRewrite
            // 
            this.btnRewrite.Location = new System.Drawing.Point(16, 151);
            this.btnRewrite.Name = "btnRewrite";
            this.btnRewrite.Size = new System.Drawing.Size(205, 23);
            this.btnRewrite.TabIndex = 15;
            this.btnRewrite.Text = "Переписать запись...";
            this.btnRewrite.UseVisualStyleBackColor = true;
            this.btnRewrite.Click += new System.EventHandler(this.btnRewrite_Click);
            // 
            // btnGetInfo
            // 
            this.btnGetInfo.Location = new System.Drawing.Point(16, 266);
            this.btnGetInfo.Name = "btnGetInfo";
            this.btnGetInfo.Size = new System.Drawing.Size(205, 23);
            this.btnGetInfo.TabIndex = 14;
            this.btnGetInfo.Text = "Информация о хранилище";
            this.btnGetInfo.UseVisualStyleBackColor = true;
            this.btnGetInfo.Click += new System.EventHandler(this.btnGetInfo_Click);
            // 
            // btnSynchronize
            // 
            this.btnSynchronize.Location = new System.Drawing.Point(16, 122);
            this.btnSynchronize.Name = "btnSynchronize";
            this.btnSynchronize.Size = new System.Drawing.Size(205, 23);
            this.btnSynchronize.TabIndex = 13;
            this.btnSynchronize.Text = "Синхронизировать";
            this.btnSynchronize.UseVisualStyleBackColor = true;
            this.btnSynchronize.Click += new System.EventHandler(this.btnSynchronize_Click);
            // 
            // btnSetSettings
            // 
            this.btnSetSettings.Location = new System.Drawing.Point(16, 220);
            this.btnSetSettings.Name = "btnSetSettings";
            this.btnSetSettings.Size = new System.Drawing.Size(205, 23);
            this.btnSetSettings.TabIndex = 12;
            this.btnSetSettings.Text = "Применить настройки";
            this.btnSetSettings.UseVisualStyleBackColor = true;
            this.btnSetSettings.Click += new System.EventHandler(this.btnSetSettings_Click);
            // 
            // btnGetSettings
            // 
            this.btnGetSettings.Location = new System.Drawing.Point(16, 191);
            this.btnGetSettings.Name = "btnGetSettings";
            this.btnGetSettings.Size = new System.Drawing.Size(205, 23);
            this.btnGetSettings.TabIndex = 11;
            this.btnGetSettings.Text = "Получить настройки";
            this.btnGetSettings.UseVisualStyleBackColor = true;
            this.btnGetSettings.Click += new System.EventHandler(this.btnGetSettings_Click);
            // 
            // btnDeleteRecord
            // 
            this.btnDeleteRecord.Location = new System.Drawing.Point(16, 93);
            this.btnDeleteRecord.Name = "btnDeleteRecord";
            this.btnDeleteRecord.Size = new System.Drawing.Size(205, 23);
            this.btnDeleteRecord.TabIndex = 10;
            this.btnDeleteRecord.Text = "Удалить из хранилища...";
            this.btnDeleteRecord.UseVisualStyleBackColor = true;
            this.btnDeleteRecord.Click += new System.EventHandler(this.btnDeleteRecord_Click);
            // 
            // btnReadRecord
            // 
            this.btnReadRecord.Location = new System.Drawing.Point(16, 64);
            this.btnReadRecord.Name = "btnReadRecord";
            this.btnReadRecord.Size = new System.Drawing.Size(205, 23);
            this.btnReadRecord.TabIndex = 9;
            this.btnReadRecord.Text = "Читать из хранилища...";
            this.btnReadRecord.UseVisualStyleBackColor = true;
            this.btnReadRecord.Click += new System.EventHandler(this.btnReadRecord_Click);
            // 
            // btnWriteRecord
            // 
            this.btnWriteRecord.Location = new System.Drawing.Point(16, 35);
            this.btnWriteRecord.Name = "btnWriteRecord";
            this.btnWriteRecord.Size = new System.Drawing.Size(205, 23);
            this.btnWriteRecord.TabIndex = 8;
            this.btnWriteRecord.Text = "Поместить в хранилище...";
            this.btnWriteRecord.UseVisualStyleBackColor = true;
            this.btnWriteRecord.Click += new System.EventHandler(this.btnWriteRecord_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(585, 407);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 457);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Демонстрация работы с хранилищем видеофайлов";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckInterval)).EndInit();
            this.gbxPartitionProperties.ResumeLayout(false);
            this.gbxPartitionProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFreeSpace)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvwPartitions;
        private System.Windows.Forms.ColumnHeader clhPath;
        private System.Windows.Forms.GroupBox gbxPartitionProperties;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnChoicePath;
        private System.Windows.Forms.TextBox txtbPartitionPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numFreeSpace;
        private System.Windows.Forms.CheckBox chbxActive;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.ColumnHeader clhId;
        private System.Windows.Forms.ColumnHeader clhFreeSpaceLimit;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.NumericUpDown numCheckInterval;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel slblMessage;
        private System.Windows.Forms.ColumnHeader clhActive;
        private System.Windows.Forms.TextBox txtbId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGetFreeSpace;
        private System.Windows.Forms.CheckBox chbWriteDuplicates;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSynchronize;
        private System.Windows.Forms.Button btnSetSettings;
        private System.Windows.Forms.Button btnGetSettings;
        private System.Windows.Forms.Button btnDeleteRecord;
        private System.Windows.Forms.Button btnReadRecord;
        private System.Windows.Forms.Button btnWriteRecord;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnGetInfo;
        private System.Windows.Forms.Button btnRewrite;
    }
}

