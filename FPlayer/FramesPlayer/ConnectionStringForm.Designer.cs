namespace FramesPlayer
{
    partial class ConnectionStringForm
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
            this.ServerName = new System.Windows.Forms.TextBox();
            this.Login = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AutorizationType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.DataBaseName = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Close = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.DBSelect = new System.Windows.Forms.Button();
            this.SelectWagon = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ServerName
            // 
            this.ServerName.Location = new System.Drawing.Point(113, 31);
            this.ServerName.Name = "ServerName";
            this.ServerName.Size = new System.Drawing.Size(177, 20);
            this.ServerName.TabIndex = 0;
            this.ServerName.TextChanged += new System.EventHandler(this.ServerName_TextChanged);
            // 
            // Login
            // 
            this.Login.Location = new System.Drawing.Point(113, 107);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(177, 20);
            this.Login.TabIndex = 1;
            this.Login.TextChanged += new System.EventHandler(this.Login_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Сервер СУБД";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Тип Авторизации";
            // 
            // AutorizationType
            // 
            this.AutorizationType.FormattingEnabled = true;
            this.AutorizationType.Items.AddRange(new object[] {
            "Windows",
            "SQL Server"});
            this.AutorizationType.Location = new System.Drawing.Point(113, 67);
            this.AutorizationType.Name = "AutorizationType";
            this.AutorizationType.Size = new System.Drawing.Size(177, 21);
            this.AutorizationType.TabIndex = 4;
            this.AutorizationType.SelectedIndexChanged += new System.EventHandler(this.AutorizationType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Логин";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Пароль";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(113, 145);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(177, 20);
            this.Password.TabIndex = 7;
            this.Password.TextChanged += new System.EventHandler(this.Password_TextChanged);
            // 
            // DataBaseName
            // 
            this.DataBaseName.FormattingEnabled = true;
            this.DataBaseName.Location = new System.Drawing.Point(113, 186);
            this.DataBaseName.Name = "DataBaseName";
            this.DataBaseName.Size = new System.Drawing.Size(177, 21);
            this.DataBaseName.TabIndex = 8;
            this.DataBaseName.SelectedIndexChanged += new System.EventHandler(this.DataBaseName_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 189);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "База данных";
            // 
            // Close
            // 
            this.Close.Location = new System.Drawing.Point(134, 216);
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(75, 23);
            this.Close.TabIndex = 10;
            this.Close.Text = "Закрыть";
            this.Close.UseVisualStyleBackColor = true;
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(215, 216);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 11;
            this.Save.Text = "Сохранить";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // DBSelect
            // 
            this.DBSelect.Location = new System.Drawing.Point(15, 216);
            this.DBSelect.Name = "DBSelect";
            this.DBSelect.Size = new System.Drawing.Size(113, 23);
            this.DBSelect.TabIndex = 12;
            this.DBSelect.Text = "Выбрать базу данных";
            this.DBSelect.UseVisualStyleBackColor = true;
            this.DBSelect.Click += new System.EventHandler(this.DBSelect_Click);
            // 
            // SelectWagon
            // 
            this.SelectWagon.Location = new System.Drawing.Point(256, 2);
            this.SelectWagon.Name = "SelectWagon";
            this.SelectWagon.Size = new System.Drawing.Size(34, 23);
            this.SelectWagon.TabIndex = 13;
            this.SelectWagon.Text = "SW";
            this.SelectWagon.UseVisualStyleBackColor = true;
            this.SelectWagon.Visible = false;
            this.SelectWagon.Click += new System.EventHandler(this.SelectWagon_Click);
            // 
            // ConnectionStringForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 247);
            this.Controls.Add(this.SelectWagon);
            this.Controls.Add(this.DBSelect);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DataBaseName);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AutorizationType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.ServerName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionStringForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка соединения с Базой данных";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectionStringForm_FormClosing);
            this.Load += new System.EventHandler(this.ConnectionStringForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ServerName;
        private System.Windows.Forms.TextBox Login;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox AutorizationType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.ComboBox DataBaseName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button Close;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button DBSelect;
        private System.Windows.Forms.Button SelectWagon;
    }
}