namespace FramesPlayer
{
    partial class FormExport
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExport));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.gbPath = new System.Windows.Forms.GroupBox();
            this.btnSelectPath = new System.Windows.Forms.Button();
            this.tbExportPath = new System.Windows.Forms.TextBox();
            this.gbTK = new System.Windows.Forms.GroupBox();
            this.dgvTelecameras = new System.Windows.Forms.DataGridView();
            this.ColumnEnable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFrameSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbVideo = new System.Windows.Forms.GroupBox();
            this.cbExportFrames = new System.Windows.Forms.CheckBox();
            this.btnCodecOptions = new System.Windows.Forms.Button();
            this.cbCodecs = new System.Windows.Forms.ComboBox();
            this.progressMain = new System.Windows.Forms.ProgressBar();
            this.gbProgress = new System.Windows.Forms.GroupBox();
            this.fbdpath = new System.Windows.Forms.FolderBrowserDialog();
            this.cbCompress = new System.Windows.Forms.CheckBox();
            this.gbSelectDb = new System.Windows.Forms.GroupBox();
            this.btnSelectWagons = new System.Windows.Forms.Button();
            this.cbInstallSystemType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.InstalSystemTypeLabel = new System.Windows.Forms.Label();
            this.tbSetDtatbaseConnection = new System.Windows.Forms.Button();
            this.cbSelectWagonOption = new System.Windows.Forms.CheckBox();
            this.tsbStart = new System.Windows.Forms.ToolStripButton();
            this.tsbPause = new System.Windows.Forms.ToolStripButton();
            this.tsbStop = new System.Windows.Forms.ToolStripButton();
            this.tsbExport = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.gbPath.SuspendLayout();
            this.gbTK.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTelecameras)).BeginInit();
            this.gbVideo.SuspendLayout();
            this.gbProgress.SuspendLayout();
            this.gbSelectDb.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbStart,
            this.tsbPause,
            this.tsbStop,
            this.tsbExport});
            this.toolStrip1.Location = new System.Drawing.Point(0, 418);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(298, 39);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // gbPath
            // 
            this.gbPath.Controls.Add(this.btnSelectPath);
            this.gbPath.Controls.Add(this.tbExportPath);
            this.gbPath.Location = new System.Drawing.Point(12, 309);
            this.gbPath.Name = "gbPath";
            this.gbPath.Size = new System.Drawing.Size(272, 47);
            this.gbPath.TabIndex = 17;
            this.gbPath.TabStop = false;
            this.gbPath.Text = "Путь экспорта";
            // 
            // btnSelectPath
            // 
            this.btnSelectPath.Location = new System.Drawing.Point(243, 18);
            this.btnSelectPath.Name = "btnSelectPath";
            this.btnSelectPath.Size = new System.Drawing.Size(22, 21);
            this.btnSelectPath.TabIndex = 8;
            this.btnSelectPath.Text = "..";
            this.btnSelectPath.UseVisualStyleBackColor = true;
            this.btnSelectPath.Click += new System.EventHandler(this.btnSelectPath_Click);
            // 
            // tbExportPath
            // 
            this.tbExportPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.tbExportPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.tbExportPath.Location = new System.Drawing.Point(6, 19);
            this.tbExportPath.Name = "tbExportPath";
            this.tbExportPath.Size = new System.Drawing.Size(237, 20);
            this.tbExportPath.TabIndex = 10;
            // 
            // gbTK
            // 
            this.gbTK.Controls.Add(this.dgvTelecameras);
            this.gbTK.Location = new System.Drawing.Point(12, 8);
            this.gbTK.Name = "gbTK";
            this.gbTK.Size = new System.Drawing.Size(272, 156);
            this.gbTK.TabIndex = 16;
            this.gbTK.TabStop = false;
            this.gbTK.Text = "Экспортируемые видеоканалы";
            // 
            // dgvTelecameras
            // 
            this.dgvTelecameras.AllowUserToAddRows = false;
            this.dgvTelecameras.AllowUserToDeleteRows = false;
            this.dgvTelecameras.AllowUserToResizeColumns = false;
            this.dgvTelecameras.AllowUserToResizeRows = false;
            this.dgvTelecameras.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvTelecameras.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTelecameras.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTelecameras.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTelecameras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTelecameras.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnEnable,
            this.ColumnName,
            this.ColumnFrameSize,
            this.ColumnFile});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTelecameras.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTelecameras.Location = new System.Drawing.Point(6, 19);
            this.dgvTelecameras.MultiSelect = false;
            this.dgvTelecameras.Name = "dgvTelecameras";
            this.dgvTelecameras.RowHeadersVisible = false;
            this.dgvTelecameras.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvTelecameras.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTelecameras.Size = new System.Drawing.Size(259, 133);
            this.dgvTelecameras.TabIndex = 4;
            this.dgvTelecameras.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTelecameras_CellContentClick);
            // 
            // ColumnEnable
            // 
            this.ColumnEnable.FillWeight = 20F;
            this.ColumnEnable.HeaderText = "";
            this.ColumnEnable.Name = "ColumnEnable";
            this.ColumnEnable.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnEnable.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnEnable.Width = 20;
            // 
            // ColumnName
            // 
            this.ColumnName.FillWeight = 73F;
            this.ColumnName.HeaderText = "Видеоканал";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnName.Width = 73;
            // 
            // ColumnFrameSize
            // 
            this.ColumnFrameSize.FillWeight = 70F;
            this.ColumnFrameSize.HeaderText = "Размеры";
            this.ColumnFrameSize.Name = "ColumnFrameSize";
            this.ColumnFrameSize.ReadOnly = true;
            this.ColumnFrameSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnFrameSize.Width = 70;
            // 
            // ColumnFile
            // 
            this.ColumnFile.FillWeight = 97F;
            this.ColumnFile.HeaderText = "Файл";
            this.ColumnFile.Name = "ColumnFile";
            this.ColumnFile.Width = 97;
            // 
            // gbVideo
            // 
            this.gbVideo.Controls.Add(this.cbExportFrames);
            this.gbVideo.Controls.Add(this.btnCodecOptions);
            this.gbVideo.Controls.Add(this.cbCodecs);
            this.gbVideo.Location = new System.Drawing.Point(12, 166);
            this.gbVideo.Name = "gbVideo";
            this.gbVideo.Size = new System.Drawing.Size(272, 48);
            this.gbVideo.TabIndex = 15;
            this.gbVideo.TabStop = false;
            // 
            // cbExportFrames
            // 
            this.cbExportFrames.AutoSize = true;
            this.cbExportFrames.Location = new System.Drawing.Point(111, 0);
            this.cbExportFrames.Name = "cbExportFrames";
            this.cbExportFrames.Size = new System.Drawing.Size(131, 17);
            this.cbExportFrames.TabIndex = 5;
            this.cbExportFrames.Text = "Сохранять во Frames";
            this.cbExportFrames.UseVisualStyleBackColor = true;
            // 
            // btnCodecOptions
            // 
            this.btnCodecOptions.Location = new System.Drawing.Point(243, 18);
            this.btnCodecOptions.Name = "btnCodecOptions";
            this.btnCodecOptions.Size = new System.Drawing.Size(22, 23);
            this.btnCodecOptions.TabIndex = 4;
            this.btnCodecOptions.Text = "..";
            this.btnCodecOptions.UseVisualStyleBackColor = true;
            this.btnCodecOptions.Click += new System.EventHandler(this.btnCodecOptions_Click);
            // 
            // cbCodecs
            // 
            this.cbCodecs.DisplayMember = "name";
            this.cbCodecs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCodecs.FormattingEnabled = true;
            this.cbCodecs.Location = new System.Drawing.Point(7, 19);
            this.cbCodecs.Name = "cbCodecs";
            this.cbCodecs.Size = new System.Drawing.Size(236, 21);
            this.cbCodecs.TabIndex = 3;
            this.cbCodecs.ValueMember = "handler";
            this.cbCodecs.SelectedIndexChanged += new System.EventHandler(this.cbCodecs_SelectedIndexChanged);
            // 
            // progressMain
            // 
            this.progressMain.Location = new System.Drawing.Point(6, 19);
            this.progressMain.Name = "progressMain";
            this.progressMain.Size = new System.Drawing.Size(260, 18);
            this.progressMain.TabIndex = 18;
            // 
            // gbProgress
            // 
            this.gbProgress.Controls.Add(this.progressMain);
            this.gbProgress.Location = new System.Drawing.Point(11, 362);
            this.gbProgress.Name = "gbProgress";
            this.gbProgress.Size = new System.Drawing.Size(273, 44);
            this.gbProgress.TabIndex = 19;
            this.gbProgress.TabStop = false;
            this.gbProgress.Text = "Прогресс текущей операции";
            // 
            // fbdpath
            // 
            this.fbdpath.Description = "Выберите каталог для экспорта видео";
            // 
            // cbCompress
            // 
            this.cbCompress.AutoSize = true;
            this.cbCompress.Location = new System.Drawing.Point(20, 166);
            this.cbCompress.Name = "cbCompress";
            this.cbCompress.Size = new System.Drawing.Size(97, 17);
            this.cbCompress.TabIndex = 5;
            this.cbCompress.Text = "Сжатие видео";
            this.cbCompress.UseVisualStyleBackColor = true;
            this.cbCompress.CheckedChanged += new System.EventHandler(this.cbCompress_CheckedChanged);
            // 
            // gbSelectDb
            // 
            this.gbSelectDb.Controls.Add(this.btnSelectWagons);
            this.gbSelectDb.Controls.Add(this.cbInstallSystemType);
            this.gbSelectDb.Controls.Add(this.label1);
            this.gbSelectDb.Controls.Add(this.InstalSystemTypeLabel);
            this.gbSelectDb.Controls.Add(this.tbSetDtatbaseConnection);
            this.gbSelectDb.Enabled = false;
            this.gbSelectDb.Location = new System.Drawing.Point(12, 217);
            this.gbSelectDb.Name = "gbSelectDb";
            this.gbSelectDb.Size = new System.Drawing.Size(272, 88);
            this.gbSelectDb.TabIndex = 16;
            this.gbSelectDb.TabStop = false;
            // 
            // btnSelectWagons
            // 
            this.btnSelectWagons.Location = new System.Drawing.Point(64, 56);
            this.btnSelectWagons.Name = "btnSelectWagons";
            this.btnSelectWagons.Size = new System.Drawing.Size(79, 23);
            this.btnSelectWagons.TabIndex = 20;
            this.btnSelectWagons.Text = "Выбрать..";
            this.btnSelectWagons.UseVisualStyleBackColor = true;
            this.btnSelectWagons.Click += new System.EventHandler(this.btnSelectWagons_Click);
            // 
            // cbInstallSystemType
            // 
            this.cbInstallSystemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInstallSystemType.FormattingEnabled = true;
            this.cbInstallSystemType.Items.AddRange(new object[] {
            "ASKIN",
            "ASKO"});
            this.cbInstallSystemType.Location = new System.Drawing.Point(65, 27);
            this.cbInstallSystemType.Name = "cbInstallSystemType";
            this.cbInstallSystemType.Size = new System.Drawing.Size(77, 21);
            this.cbInstallSystemType.TabIndex = 7;
            this.cbInstallSystemType.SelectedIndexChanged += new System.EventHandler(this.InstallSystemTypeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Вагоны:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // InstalSystemTypeLabel
            // 
            this.InstalSystemTypeLabel.AutoSize = true;
            this.InstalSystemTypeLabel.Location = new System.Drawing.Point(8, 30);
            this.InstalSystemTypeLabel.Name = "InstalSystemTypeLabel";
            this.InstalSystemTypeLabel.Size = new System.Drawing.Size(54, 13);
            this.InstalSystemTypeLabel.TabIndex = 6;
            this.InstalSystemTypeLabel.Text = "Система:";
            this.InstalSystemTypeLabel.Click += new System.EventHandler(this.InstalSystemTypeLabel_Click);
            // 
            // tbSetDtatbaseConnection
            // 
            this.tbSetDtatbaseConnection.Location = new System.Drawing.Point(148, 26);
            this.tbSetDtatbaseConnection.Name = "tbSetDtatbaseConnection";
            this.tbSetDtatbaseConnection.Size = new System.Drawing.Size(115, 23);
            this.tbSetDtatbaseConnection.TabIndex = 4;
            this.tbSetDtatbaseConnection.Text = "Соединение с БД";
            this.tbSetDtatbaseConnection.UseVisualStyleBackColor = true;
            this.tbSetDtatbaseConnection.Click += new System.EventHandler(this.SetDtatbaseConnectionTextBox_Click);
            // 
            // cbSelectWagonOption
            // 
            this.cbSelectWagonOption.AutoSize = true;
            this.cbSelectWagonOption.Location = new System.Drawing.Point(20, 217);
            this.cbSelectWagonOption.Name = "cbSelectWagonOption";
            this.cbSelectWagonOption.Size = new System.Drawing.Size(112, 17);
            this.cbSelectWagonOption.TabIndex = 5;
            this.cbSelectWagonOption.Text = "Экспорт вагонов";
            this.cbSelectWagonOption.UseVisualStyleBackColor = true;
            this.cbSelectWagonOption.CheckedChanged += new System.EventHandler(this.cbSelectWagonOption_CheckedChanged);
            // 
            // tsbStart
            // 
            this.tsbStart.Image = ((System.Drawing.Image)(resources.GetObject("tsbStart.Image")));
            this.tsbStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbStart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStart.Name = "tsbStart";
            this.tsbStart.Size = new System.Drawing.Size(74, 36);
            this.tsbStart.Text = "Старт";
            this.tsbStart.Click += new System.EventHandler(this.tsbStart_Click);
            // 
            // tsbPause
            // 
            this.tsbPause.Enabled = false;
            this.tsbPause.Image = ((System.Drawing.Image)(resources.GetObject("tsbPause.Image")));
            this.tsbPause.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbPause.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPause.Name = "tsbPause";
            this.tsbPause.Size = new System.Drawing.Size(75, 36);
            this.tsbPause.Text = "Пауза";
            this.tsbPause.Click += new System.EventHandler(this.tsbPause_Click);
            // 
            // tsbStop
            // 
            this.tsbStop.Enabled = false;
            this.tsbStop.Image = ((System.Drawing.Image)(resources.GetObject("tsbStop.Image")));
            this.tsbStop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStop.Name = "tsbStop";
            this.tsbStop.Size = new System.Drawing.Size(70, 36);
            this.tsbStop.Text = "Стоп";
            this.tsbStop.Click += new System.EventHandler(this.tsbStop_Click);
            // 
            // tsbExport
            // 
            this.tsbExport.Image = ((System.Drawing.Image)(resources.GetObject("tsbExport.Image")));
            this.tsbExport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExport.Name = "tsbExport";
            this.tsbExport.Size = new System.Drawing.Size(58, 36);
            this.tsbExport.Text = "DB";
            this.tsbExport.Visible = false;
            this.tsbExport.Click += new System.EventHandler(this.tsbExport_Click);
            // 
            // FormExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 457);
            this.Controls.Add(this.cbSelectWagonOption);
            this.Controls.Add(this.gbSelectDb);
            this.Controls.Add(this.cbCompress);
            this.Controls.Add(this.gbProgress);
            this.Controls.Add(this.gbPath);
            this.Controls.Add(this.gbTK);
            this.Controls.Add(this.gbVideo);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Экспорт видео";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormExport_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormExport_FormClosed);
            this.Load += new System.EventHandler(this.FormExport_Load);
            this.Shown += new System.EventHandler(this.FormExport_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.gbPath.ResumeLayout(false);
            this.gbPath.PerformLayout();
            this.gbTK.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTelecameras)).EndInit();
            this.gbVideo.ResumeLayout(false);
            this.gbVideo.PerformLayout();
            this.gbProgress.ResumeLayout(false);
            this.gbSelectDb.ResumeLayout(false);
            this.gbSelectDb.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbStart;
        private System.Windows.Forms.ToolStripButton tsbPause;
        private System.Windows.Forms.ToolStripButton tsbStop;
        private System.Windows.Forms.GroupBox gbPath;
        private System.Windows.Forms.Button btnSelectPath;
        private System.Windows.Forms.TextBox tbExportPath;
        private System.Windows.Forms.GroupBox gbTK;
        private System.Windows.Forms.DataGridView dgvTelecameras;
        private System.Windows.Forms.GroupBox gbVideo;
        private System.Windows.Forms.Button btnCodecOptions;
        private System.Windows.Forms.ComboBox cbCodecs;
        private System.Windows.Forms.ProgressBar progressMain;
        private System.Windows.Forms.GroupBox gbProgress;
        private System.Windows.Forms.FolderBrowserDialog fbdpath;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnEnable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFrameSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFile;
        private System.Windows.Forms.CheckBox cbCompress;
        private System.Windows.Forms.ToolStripButton tsbExport;
        private System.Windows.Forms.GroupBox gbSelectDb;
        private System.Windows.Forms.CheckBox cbSelectWagonOption;
        private System.Windows.Forms.Button tbSetDtatbaseConnection;
        private System.Windows.Forms.ComboBox cbInstallSystemType;
        private System.Windows.Forms.Label InstalSystemTypeLabel;
        private System.Windows.Forms.Button btnSelectWagons;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbExportFrames;
    }
}