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
    public partial class ConnectionStringForm : Form
    {
        bool _isChangeConnectionPeremetrs;

        public ConnectionStringForm()
        {
            InitializeComponent();
        }

        private void ConnectionStringForm_Load(object sender, EventArgs e)
        {
            ServerName.Text = FramesPlayerSettingContainer.ConnectionSettings.ServerName;
            AutorizationType.SelectedIndex = (int)FramesPlayerSettingContainer.ConnectionSettings.AutorizationType;
            Login.Text = FramesPlayerSettingContainer.ConnectionSettings.Login;
            Password.Text = FramesPlayerSettingContainer.ConnectionSettings.Password;
            DataBaseName.Text = FramesPlayerSettingContainer.ConnectionSettings.DataBaseName;
            _isChangeConnectionPeremetrs = false;
        }

        private void SelectWagon_Click(object sender, EventArgs e)
        {
            SelectWagonsForm form = new SelectWagonsForm();
            form.Show();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveConnectionChanges();
        }
        
        private void SaveConnectionChanges()
        {
            FramesPlayerSettingContainer.ConnectionSettings.ServerName = ServerName.Text;
            FramesPlayerSettingContainer.ConnectionSettings.AutorizationType = (SQLAutorizationType)AutorizationType.SelectedIndex;
            FramesPlayerSettingContainer.ConnectionSettings.Login = Login.Text;
            FramesPlayerSettingContainer.ConnectionSettings.Password = Password.Text;
            FramesPlayerSettingContainer.ConnectionSettings.DataBaseName = DataBaseName.Text;
            FramesPlayerSettingContainer.ConnectionSettings.Save();
            //переопределить провайдер данных
            FramesPlayerSettingContainer.ResetProvider();
            //Установить флаг сохранения параметров
            _isChangeConnectionPeremetrs = false;
            this.Close();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FillDBNamesComboBox(List<string> itemList)
        {
            if (itemList.Count == 0)
                return;
            DataBaseName.Items.Clear();
            int selectedIndex = 0;
            for(int i=0;i<itemList.Count;i++)
            {
                DataBaseName.Items.Add(itemList[i]);
                if (FramesPlayerSettingContainer.ConnectionSettings.DataBaseName == itemList[i])
                {
                    selectedIndex = i;
                }
            }
            if (selectedIndex < DataBaseName.Items.Count)
            {
                DataBaseName.SelectedIndex = selectedIndex;
            }
        }

        private void DBSelect_Click(object sender, EventArgs e)
        {
            DBOperation dbOperation = new DBOperation(FramesPlayerSettingContainer.ConnectionSettings.BuildConnectionString());
            List<string> databaseNames = dbOperation.GetDataBaseList();
            FillDBNamesComboBox(databaseNames);
        }

        private void ServerName_TextChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        private void AutorizationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        private void Login_TextChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        private void DataBaseName_SelectedIndexChanged(object sender, EventArgs e)
        {
            _isChangeConnectionPeremetrs = true;
        }

        private void ConnectionStringForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isChangeConnectionPeremetrs)
            {
                if (MessageBox.Show("Вы хотите сохранить изменения?", "Сохранение параметров", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    SaveConnectionChanges();
                }
            }
        }
    }
}
