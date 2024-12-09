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
    public partial class FormConnectionDB : Form
    {
        public FormConnectionDB()
        {
            InitializeComponent();
        }

        private void FromConnectionDB_Load(object sender, EventArgs e)
        {
            if(!SettingContainer.WagonDataProvider.CheckConnection()){
                rtbResult.Font = new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold|FontStyle.Underline);
                rtbResult.Text = "Соединение с БД не установлено";
            }else{
                rtbResult.Font = new Font(FontFamily.GenericSansSerif,9,FontStyle.Regular);
                rtbResult.Text = "Установлено соединение с Базой данных " + SettingContainer.ConnectionSettings.DataBaseName;
            }
            cbInstallSystemType.SelectedIndex = (int)SettingContainer.DatabaseProviderType;
        }

        private void tbSetDtatbaseConnection_Click(object sender, EventArgs e)
        {
            FormConnectionString form = new FormConnectionString();
            form.Show();
        }

        private bool TrySetProviderType(ProviderType providerType, out ProviderType oldProviderType)
        {
            oldProviderType = ProviderType.ASKO;
            bool result = false;
            try
            {
                oldProviderType = SettingContainer.DatabaseProviderType;
                SettingContainer.DatabaseProviderType = providerType;
                SettingContainer.ResetProvider();
                result = SettingContainer.WagonDataProvider.CheckConnection();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private void bTest_Click(object sender, EventArgs e)
        {
            CheckSettings();
        }

        private bool  CheckSettings()
        {
            bool result = false;
            ProviderType oldProvider = ProviderType.ASKO;
            ProviderType settedProvider = cbInstallSystemType.SelectedIndex != -1 ? (ProviderType)cbInstallSystemType.SelectedIndex : ProviderType.ASKO;
            if (!TrySetProviderType(settedProvider, out oldProvider))
            {
                ProviderType restoredProvider = oldProvider;
                TrySetProviderType(restoredProvider, out oldProvider);
                rtbResult.Text = "Некорректный выбор параметров подключения. Измените тип системы или параметры подключения к БД.";
            }
            else
            {
                result = true;
                rtbResult.Text = "Настройки успешно установлены";
            }
            return result;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormConnectionDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckSettings())

                e.Cancel = MessageBox.Show("Установлены некорректные настройки. Исправить настройки?", "Предупреждение закрытия формы", MessageBoxButtons.OKCancel) == DialogResult.OK;
            else
                e.Cancel = false;
        }
        
    }
}
