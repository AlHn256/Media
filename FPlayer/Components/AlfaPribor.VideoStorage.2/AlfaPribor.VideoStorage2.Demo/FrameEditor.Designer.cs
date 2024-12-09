namespace AlfaPribor.VideoStorage.Demo
{
    partial class FrameEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numCameraId = new System.Windows.Forms.NumericUpDown();
            this.numTimeStamp = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtbContetType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnLoadVideoData = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.pbxVideoData = new System.Windows.Forms.PictureBox();
            this.btnClearVideoData = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCameraId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeStamp)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxVideoData)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numCameraId);
            this.groupBox1.Controls.Add(this.numTimeStamp);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtbContetType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры";
            // 
            // numCameraId
            // 
            this.numCameraId.Location = new System.Drawing.Point(157, 19);
            this.numCameraId.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numCameraId.Name = "numCameraId";
            this.numCameraId.Size = new System.Drawing.Size(70, 20);
            this.numCameraId.TabIndex = 9;
            // 
            // numTimeStamp
            // 
            this.numTimeStamp.Location = new System.Drawing.Point(157, 46);
            this.numTimeStamp.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numTimeStamp.Name = "numTimeStamp";
            this.numTimeStamp.Size = new System.Drawing.Size(70, 20);
            this.numTimeStamp.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Метка времени";
            // 
            // txtbContetType
            // 
            this.txtbContetType.Location = new System.Drawing.Point(157, 73);
            this.txtbContetType.Name = "txtbContetType";
            this.txtbContetType.Size = new System.Drawing.Size(155, 20);
            this.txtbContetType.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Тип содержимого";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Идентификатор камеры";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pbxVideoData);
            this.groupBox2.Location = new System.Drawing.Point(12, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(318, 231);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Видеоданные";
            // 
            // btnLoadVideoData
            // 
            this.btnLoadVideoData.Location = new System.Drawing.Point(12, 358);
            this.btnLoadVideoData.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnLoadVideoData.Name = "btnLoadVideoData";
            this.btnLoadVideoData.Size = new System.Drawing.Size(75, 23);
            this.btnLoadVideoData.TabIndex = 2;
            this.btnLoadVideoData.Text = "Загрузить";
            this.btnLoadVideoData.UseVisualStyleBackColor = true;
            this.btnLoadVideoData.Click += new System.EventHandler(this.btnLoadVideoData_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(255, 358);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "vdata";
            this.openFileDialog.Filter = "Файлы видеоданных|*.ico;*.bmp;*.gif; *.jpeg; *.wmf|Все файлы|*.*";
            this.openFileDialog.Title = "Открыть файл с видеоданными";
            // 
            // pbxVideoData
            // 
            this.pbxVideoData.Location = new System.Drawing.Point(6, 19);
            this.pbxVideoData.Name = "pbxVideoData";
            this.pbxVideoData.Size = new System.Drawing.Size(306, 206);
            this.pbxVideoData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxVideoData.TabIndex = 0;
            this.pbxVideoData.TabStop = false;
            // 
            // btnClearVideoData
            // 
            this.btnClearVideoData.Location = new System.Drawing.Point(93, 358);
            this.btnClearVideoData.Name = "btnClearVideoData";
            this.btnClearVideoData.Size = new System.Drawing.Size(75, 23);
            this.btnClearVideoData.TabIndex = 6;
            this.btnClearVideoData.Text = "Очистить";
            this.btnClearVideoData.UseVisualStyleBackColor = true;
            this.btnClearVideoData.Click += new System.EventHandler(this.btnClearVideoData_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(174, 358);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отменить";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FrameEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 393);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClearVideoData);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnLoadVideoData);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Редактор кадра";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCameraId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeStamp)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxVideoData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtbContetType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numTimeStamp;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnLoadVideoData;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.NumericUpDown numCameraId;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.PictureBox pbxVideoData;
        private System.Windows.Forms.Button btnClearVideoData;
        private System.Windows.Forms.Button btnCancel;
    }
}