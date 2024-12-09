using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FramesPlayer.ExportConfiguration;
using FramesPlayer.DataTypes;

namespace FramesPlayer
{

    public partial class FormConnectionString : Form
    {

        bool _isChangeConnectionPeremetrs;

        public FormConnectionString()
        {
            InitializeComponent();
        }

        void ConnectionStringForm_Load(object sender, EventArgs e)
        {
            tbServerName.Text = SettingContainer.ConnectionSettings.ServerName;
            cbAutorization.Checked = SettingContainer.ConnectionSettings.AutorizationType;
            tbLogin.Text = SettingContainer.ConnectionSettings.Login;
            tbPassword.Text = SettingContainer.ConnectionSettings.Password;
            ddlDataBaseName.Text = SettingContainer.ConnectionSettings.DataBaseName;
            gbAuthorize.Enabled = cbAutorization.Checked;
            DBSelect_Click(btnDBSelect, EventArgs.Empty);
            _isChangeConnectionPeremetrs = false;
        }

        void Save_Click(object sender, EventArgs e)
        {
            SaveConnectionChanges();
        }

        /// <summary>Сохранить параметры подключения к базе</summary>
        void SaveConnectionChanges()
        {
            SettingContainer.ConnectionSettings.ServerName = tbServerName.Text;
            SettingContainer.ConnectionSettings.AutorizationType = cbAutorization.Checked;
            SettingContainer.ConnectionSettings.Login = tbLogin.Text;
            SettingContainer.ConnectionSettings.Password = tbPassword.Text;
            SettingContainer.ConnectionSettings.DataBaseName = ddlDataBaseName.Text;
            SettingContainer.ConnectionSettings.Save();

            //переопределить провайдер данных
            SettingContainer.ResetProvider();

            //Установить флаг сохранения параметров
            _isChangeConnectionPeremetrs = false;
            this.Close();
        }

        void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>Заполнение списка баз данных</summary>
        /// <param name="itemList">Список</param>
        void FillDBNamesComboBox(List<string> itemList)
        {
            ddlDataBaseName.Items.Clear();
            if (itemList.Count == 0) return;
            int selectedIndex = 0;
            for(int i=0;i<itemList.Count;i++)
            {
                ddlDataBaseName.Items.Add(itemList[i]);
                if (SettingContainer.ConnectionSettings.DataBaseName == itemList[i])
                    selectedIndex = i;
            }
            if (selectedIndex < ddlDataBaseName.Items.Count) ddlDataBaseName.SelectedIndex = selectedIndex;
        }

        void DBSelect_Click(object sender, EventArgs e)
        {
            //Постороение строки подключения
            string result = string.Empty;
            if (!cbAutorization.Checked)
                result = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;",
                                       tbServerName.Text, ddlDataBaseName.Text);
            else
                result = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3};",
                                       tbServerName.Text, ddlDataBaseName.Text, tbLogin.Text, tbPassword.Text);
            DBOperation dbOperation = new DBOperation(result);
            List<string> databaseNames = dbOperation.GetDataBaseList();
            FillDBNamesComboBox(databaseNames);
        }

        void ServerName_TextChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        void cbAutorization_CheckedChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
            gbAuthorize.Enabled = cbAutorization.Checked;
        }

        void Login_TextChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        void Password_TextChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        void DataBaseName_SelectedIndexChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        void ConnectionStringForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isChangeConnectionPeremetrs)
            {
                if (MessageBox.Show("Вы хотите сохранить изменения?", "Сохранение параметров", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    SaveConnectionChanges();
            }
        }
        
    }
}
