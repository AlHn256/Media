namespace FramesPlayer
{
    partial class FormConnectionDB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConnectionDB));
            this.cbInstallSystemType = new System.Windows.Forms.ComboBox();
            this.InstalSystemTypeLabel = new System.Windows.Forms.Label();
            this.tbSetDtatbaseConnection = new System.Windows.Forms.Button();
            this.gbConncetion = new System.Windows.Forms.GroupBox();
            this.gbResult = new System.Windows.Forms.GroupBox();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.bTest = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.gbConncetion.SuspendLayout();
            this.gbResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbInstallSystemType
            // 
            this.cbInstallSystemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInstallSystemType.FormattingEnabled = true;
            this.cbInstallSystemType.Items.AddRange(new object[] {
            "ASKIN",
            "ASKO"});
            this.cbInstallSystemType.Location = new System.Drawing.Point(63, 22);
            this.cbInstallSystemType.Name = "cbInstallSystemType";
            this.cbInstallSystemType.Size = new System.Drawing.Size(77, 21);
            this.cbInstallSystemType.TabIndex = 24;
            // 
            // InstalSystemTypeLabel
            // 
            this.InstalSystemTypeLabel.AutoSize = true;
            this.InstalSystemTypeLabel.Location = new System.Drawing.Point(6, 25);
            this.InstalSystemTypeLabel.Name = "InstalSystemTypeLabel";
            this.InstalSystemTypeLabel.Size = new System.Drawing.Size(54, 13);
            this.InstalSystemTypeLabel.TabIndex = 22;
            this.InstalSystemTypeLabel.Text = "Система:";
            // 
            // tbSetDtatbaseConnection
            // 
            this.tbSetDtatbaseConnection.Location = new System.Drawing.Point(146, 21);
            this.tbSetDtatbaseConnection.Name = "tbSetDtatbaseConnection";
            this.tbSetDtatbaseConnection.Size = new System.Drawing.Size(115, 23);
            this.tbSetDtatbaseConnection.TabIndex = 21;
            this.tbSetDtatbaseConnection.Text = "Соединение с БД";
            this.tbSetDtatbaseConnection.UseVisualStyleBackColor = true;
            this.tbSetDtatbaseConnection.Click += new System.EventHandler(this.tbSetDtatbaseConnection_Click);
            // 
            // gbConncetion
            // 
            this.gbConncetion.Controls.Add(this.tbSetDtatbaseConnection);
            this.gbConncetion.Controls.Add(this.cbInstallSystemType);
            this.gbConncetion.Controls.Add(this.InstalSystemTypeLabel);
            this.gbConncetion.Location = new System.Drawing.Point(12, 12);
            this.gbConncetion.Name = "gbConncetion";
            this.gbConncetion.Size = new System.Drawing.Size(274, 65);
            this.gbConncetion.TabIndex = 26;
            this.gbConncetion.TabStop = false;
            this.gbConncetion.Text = "Настройка соединения с БД";
            // 
            // gbResult
            // 
            this.gbResult.Controls.Add(this.rtbResult);
            this.gbResult.Location = new System.Drawing.Point(12, 111);
            this.gbResult.Name = "gbResult";
            this.gbResult.Size = new System.Drawing.Size(274, 117);
            this.gbResult.TabIndex = 27;
            this.gbResult.TabStop = false;
            this.gbResult.Text = "Статус связи с БД";
            // 
            // rtbResult
            // 
            this.rtbResult.BackColor = System.Drawing.SystemColors.Control;
            this.rtbResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbResult.Enabled = false;
            this.rtbResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rtbResult.Location = new System.Drawing.Point(9, 20);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(252, 91);
            this.rtbResult.TabIndex = 0;
            this.rtbResult.Text = "";
            // 
            // bTest
            // 
            this.bTest.Location = new System.Drawing.Point(21, 82);
            this.bTest.Name = "bTest";
            this.bTest.Size = new System.Drawing.Size(131, 23);
            this.bTest.TabIndex = 25;
            this.bTest.Text = "Тест";
            this.bTest.UseVisualStyleBackColor = true;
            this.bTest.Click += new System.EventHandler(this.bTest_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(158, 82);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(128, 23);
            this.bSave.TabIndex = 28;
            this.bSave.Text = "Сохранить";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // FormConnectionDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 247);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.bTest);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.gbConncetion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConnectionDB";
            this.Text = "Настройки соединения с Базой Данных";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConnectionDB_FormClosing);
            this.Load += new System.EventHandler(this.FromConnectionDB_Load);
            this.gbConncetion.ResumeLayout(false);
            this.gbConncetion.PerformLayout();
            this.gbResult.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbInstallSystemType;
        private System.Windows.Forms.Label InstalSystemTypeLabel;
        private System.Windows.Forms.Button tbSetDtatbaseConnection;
        private System.Windows.Forms.GroupBox gbConncetion;
        private System.Windows.Forms.GroupBox gbResult;
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.Button bTest;
        private System.Windows.Forms.Button bSave;
    }
}