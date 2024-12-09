namespace FramesPlayer
{
    partial class FormConnectionString
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
            this.components = new System.ComponentModel.Container();
            this.tbServerName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlDataBaseName = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDBSelect = new System.Windows.Forms.Button();
            this.gbAuthorize = new System.Windows.Forms.GroupBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.cbAutorization = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbAuthorize.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbServerName
            // 
            this.tbServerName.Location = new System.Drawing.Point(113, 21);
            this.tbServerName.Name = "tbServerName";
            this.tbServerName.Size = new System.Drawing.Size(177, 20);
            this.tbServerName.TabIndex = 0;
            this.tbServerName.TextChanged += new System.EventHandler(this.ServerName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Сервер СУБД";
            // 
            // ddlDataBaseName
            // 
            this.ddlDataBaseName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlDataBaseName.FormattingEnabled = true;
            this.ddlDataBaseName.Location = new System.Drawing.Point(113, 50);
            this.ddlDataBaseName.Name = "ddlDataBaseName";
            this.ddlDataBaseName.Size = new System.Drawing.Size(148, 21);
            this.ddlDataBaseName.TabIndex = 8;
            this.ddlDataBaseName.SelectedIndexChanged += new System.EventHandler(this.DataBaseName_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "База данных";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(215, 183);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.Close_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(134, 183);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.Save_Click);
            // 
            // btnDBSelect
            // 
            this.btnDBSelect.Font = new System.Drawing.Font("Webdings", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDBSelect.Location = new System.Drawing.Point(264, 49);
            this.btnDBSelect.Name = "btnDBSelect";
            this.btnDBSelect.Size = new System.Drawing.Size(26, 23);
            this.btnDBSelect.TabIndex = 12;
            this.btnDBSelect.Text = "q";
            this.toolTip1.SetToolTip(this.btnDBSelect, "Обновить список баз данных");
            this.btnDBSelect.UseVisualStyleBackColor = true;
            this.btnDBSelect.Click += new System.EventHandler(this.DBSelect_Click);
            // 
            // gbAuthorize
            // 
            this.gbAuthorize.Controls.Add(this.tbPassword);
            this.gbAuthorize.Controls.Add(this.label4);
            this.gbAuthorize.Controls.Add(this.label3);
            this.gbAuthorize.Controls.Add(this.tbLogin);
            this.gbAuthorize.Location = new System.Drawing.Point(9, 83);
            this.gbAuthorize.Name = "gbAuthorize";
            this.gbAuthorize.Size = new System.Drawing.Size(290, 84);
            this.gbAuthorize.TabIndex = 13;
            this.gbAuthorize.TabStop = false;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(104, 52);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(177, 20);
            this.tbPassword.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Пароль";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Логин";
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(104, 23);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(177, 20);
            this.tbLogin.TabIndex = 8;
            // 
            // cbAutorization
            // 
            this.cbAutorization.AutoSize = true;
            this.cbAutorization.Location = new System.Drawing.Point(15, 81);
            this.cbAutorization.Name = "cbAutorization";
            this.cbAutorization.Size = new System.Drawing.Size(92, 17);
            this.cbAutorization.TabIndex = 12;
            this.cbAutorization.Text = "Авторизация";
            this.cbAutorization.UseVisualStyleBackColor = true;
            this.cbAutorization.CheckedChanged += new System.EventHandler(this.cbAutorization_CheckedChanged);
            // 
            // FormConnectionString
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(311, 222);
            this.Controls.Add(this.cbAutorization);
            this.Controls.Add(this.gbAuthorize);
            this.Controls.Add(this.btnDBSelect);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ddlDataBaseName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbServerName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConnectionString";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка соединения с базой данных";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectionStringForm_FormClosing);
            this.Load += new System.EventHandler(this.ConnectionStringForm_Load);
            this.gbAuthorize.ResumeLayout(false);
            this.gbAuthorize.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbServerName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlDataBaseName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDBSelect;
        private System.Windows.Forms.GroupBox gbAuthorize;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.CheckBox cbAutorization;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}