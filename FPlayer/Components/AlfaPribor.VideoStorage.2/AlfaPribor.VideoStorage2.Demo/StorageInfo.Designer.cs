namespace AlfaPribor.VideoStorage.Demo
{
    partial class StorageInfo
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
            this.lvwPartitions = new System.Windows.Forms.ListView();
            this.clhId = new System.Windows.Forms.ColumnHeader();
            this.clhPath = new System.Windows.Forms.ColumnHeader();
            this.clhActive = new System.Windows.Forms.ColumnHeader();
            this.clhSpace = new System.Windows.Forms.ColumnHeader();
            this.clhFreeSpace = new System.Windows.Forms.ColumnHeader();
            this.clhUsed = new System.Windows.Forms.ColumnHeader();
            this.clhRecordsCount = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvwPartitions
            // 
            this.lvwPartitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwPartitions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhId,
            this.clhPath,
            this.clhActive,
            this.clhSpace,
            this.clhFreeSpace,
            this.clhUsed,
            this.clhRecordsCount});
            this.lvwPartitions.FullRowSelect = true;
            this.lvwPartitions.GridLines = true;
            this.lvwPartitions.Location = new System.Drawing.Point(12, 24);
            this.lvwPartitions.Name = "lvwPartitions";
            this.lvwPartitions.Size = new System.Drawing.Size(508, 203);
            this.lvwPartitions.TabIndex = 0;
            this.lvwPartitions.UseCompatibleStateImageBehavior = false;
            this.lvwPartitions.View = System.Windows.Forms.View.Details;
            // 
            // clhId
            // 
            this.clhId.Text = "ID";
            this.clhId.Width = 30;
            // 
            // clhPath
            // 
            this.clhPath.Text = "Путь";
            this.clhPath.Width = 174;
            // 
            // clhActive
            // 
            this.clhActive.Text = "Активный";
            // 
            // clhSpace
            // 
            this.clhSpace.Text = "Всего (МБ)";
            // 
            // clhFreeSpace
            // 
            this.clhFreeSpace.Text = "Свободно (МБ)";
            // 
            // clhUsed
            // 
            this.clhUsed.Text = "Занято (МБ)";
            // 
            // clhRecordsCount
            // 
            this.clhRecordsCount.Text = "Записей";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Разделы";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(364, 233);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(445, 233);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // StorageInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 266);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvwPartitions);
            this.MinimizeBox = false;
            this.Name = "StorageInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Информация о хранилище видеоданных";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StorageInfo_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwPartitions;
        private System.Windows.Forms.ColumnHeader clhId;
        private System.Windows.Forms.ColumnHeader clhPath;
        private System.Windows.Forms.ColumnHeader clhActive;
        private System.Windows.Forms.ColumnHeader clhSpace;
        private System.Windows.Forms.ColumnHeader clhFreeSpace;
        private System.Windows.Forms.ColumnHeader clhUsed;
        private System.Windows.Forms.ColumnHeader clhRecordsCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;
    }
}