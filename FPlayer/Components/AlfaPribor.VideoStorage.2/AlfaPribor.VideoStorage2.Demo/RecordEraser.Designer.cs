namespace AlfaPribor.VideoStorage.Demo
{
    partial class RecordEraser
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
            this.rbtnDeleteOne = new System.Windows.Forms.RadioButton();
            this.txtbId = new System.Windows.Forms.TextBox();
            this.rbtnDeleteAll = new System.Windows.Forms.RadioButton();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnDeleteAll);
            this.groupBox1.Controls.Add(this.txtbId);
            this.groupBox1.Controls.Add(this.rbtnDeleteOne);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 92);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки";
            // 
            // rbtnDeleteOne
            // 
            this.rbtnDeleteOne.AutoSize = true;
            this.rbtnDeleteOne.Location = new System.Drawing.Point(22, 26);
            this.rbtnDeleteOne.Name = "rbtnDeleteOne";
            this.rbtnDeleteOne.Size = new System.Drawing.Size(241, 17);
            this.rbtnDeleteOne.TabIndex = 0;
            this.rbtnDeleteOne.TabStop = true;
            this.rbtnDeleteOne.Text = "Удалить видеозапись с идентификатором";
            this.rbtnDeleteOne.UseVisualStyleBackColor = true;
            // 
            // txtbId
            // 
            this.txtbId.Location = new System.Drawing.Point(269, 25);
            this.txtbId.Name = "txtbId";
            this.txtbId.Size = new System.Drawing.Size(81, 20);
            this.txtbId.TabIndex = 2;
            // 
            // rbtnDeleteAll
            // 
            this.rbtnDeleteAll.AutoSize = true;
            this.rbtnDeleteAll.Location = new System.Drawing.Point(22, 49);
            this.rbtnDeleteAll.Name = "rbtnDeleteAll";
            this.rbtnDeleteAll.Size = new System.Drawing.Size(158, 17);
            this.rbtnDeleteAll.TabIndex = 3;
            this.rbtnDeleteAll.TabStop = true;
            this.rbtnDeleteAll.Text = "Удалить все видеозаписи";
            this.rbtnDeleteAll.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(12, 110);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(180, 23);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(305, 110);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // RecordEraser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 138);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordEraser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Удалить видеозапись";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnDeleteAll;
        private System.Windows.Forms.TextBox txtbId;
        private System.Windows.Forms.RadioButton rbtnDeleteOne;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
    }
}