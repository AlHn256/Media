namespace AlfaPribor.VideoStorage.Demo
{
    partial class FramesViewer
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
            this.lvwFrames = new System.Windows.Forms.ListView();
            this.clhStreamId = new System.Windows.Forms.ColumnHeader();
            this.clhTimeStamp = new System.Windows.Forms.ColumnHeader();
            this.clhContentType = new System.Windows.Forms.ColumnHeader();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pbxVideoData = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxVideoData)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Видеокадры";
            // 
            // lvwFrames
            // 
            this.lvwFrames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwFrames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhStreamId,
            this.clhTimeStamp,
            this.clhContentType});
            this.lvwFrames.FullRowSelect = true;
            this.lvwFrames.GridLines = true;
            this.lvwFrames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwFrames.Location = new System.Drawing.Point(12, 25);
            this.lvwFrames.MultiSelect = false;
            this.lvwFrames.Name = "lvwFrames";
            this.lvwFrames.Size = new System.Drawing.Size(269, 170);
            this.lvwFrames.TabIndex = 1;
            this.lvwFrames.UseCompatibleStateImageBehavior = false;
            this.lvwFrames.View = System.Windows.Forms.View.Details;
            this.lvwFrames.SelectedIndexChanged += new System.EventHandler(this.lvwFrames_SelectedIndexChanged);
            // 
            // clhStreamId
            // 
            this.clhStreamId.Text = "ID потока";
            this.clhStreamId.Width = 40;
            // 
            // clhTimeStamp
            // 
            this.clhTimeStamp.Text = "Метка времени";
            // 
            // clhContentType
            // 
            this.clhContentType.Text = "Тип содержимого";
            this.clhContentType.Width = 143;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pbxVideoData);
            this.groupBox1.Location = new System.Drawing.Point(12, 201);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 216);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Содержимое";
            // 
            // pbxVideoData
            // 
            this.pbxVideoData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxVideoData.Location = new System.Drawing.Point(6, 19);
            this.pbxVideoData.Name = "pbxVideoData";
            this.pbxVideoData.Size = new System.Drawing.Size(257, 191);
            this.pbxVideoData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxVideoData.TabIndex = 0;
            this.pbxVideoData.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(207, 423);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FramesViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 452);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lvwFrames);
            this.Controls.Add(this.label1);
            this.MinimizeBox = false;
            this.Name = "FramesViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FramesViewer";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxVideoData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvwFrames;
        private System.Windows.Forms.ColumnHeader clhStreamId;
        private System.Windows.Forms.ColumnHeader clhTimeStamp;
        private System.Windows.Forms.ColumnHeader clhContentType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox pbxVideoData;
    }
}