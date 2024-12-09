using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlfaPribor.VideoStorage;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Server.Demo
{
    public partial class FormServer : Form
    {
        private VideoStorageServer _Server;

        private VideoStorage _Storage;

        private List<string> _Events;

        public FormServer()
        {
            _Events = new List<string>();
            InitializeComponent();
            InitializeServer();
        }

        private void InitializeServer()
        {
            _Storage = new VideoStorage();

            VideoPartitionSettings partition = new VideoPartitionSettings(
                0,
                true,
                "C:\\Storage",
                1024*1024*1024
            );
            VideoStorageSettings settings = new VideoStorageSettings();
            settings.CircleBufferCheckInterval = 15;
            settings.Partitions.Add(partition);
            _Storage.SetSettings(settings);

            _Storage.Active = true;

            _Server = new VideoStorageServer(_Storage);
            _Server.RelativeUri = "VideoStorageServer";
            _Server.Opened += new EventHandler(_Server_Opened);
            _Server.Closed += new EventHandler(_Server_Closed);
            _Server.Faulted += new EventHandler(_Server_Faulted);

            _Server.Open();
        }

        void _Server_Faulted(object sender, EventArgs e)
        {
            _Events.Add(DateTime.Now.ToString() + " Сервер остановлен по ошибке");
        }

        void _Server_Closed(object sender, EventArgs e)
        {
            _Events.Add(DateTime.Now.ToString() + " Сервер остановлен");
        }

        void _Server_Opened(object sender, EventArgs e)
        {
            _Events.Add(DateTime.Now.ToString() + " Сервер запущен");
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            _Storage.Active = false;
            _Server.Close();
            this.Close();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            ShowConfig();
            this.WindowState = FormWindowState.Normal;
        }

        private void ShowConfig()
        {
            textBoxAddress.Text = _Server.Address;
            numericUpDownPort.Value = _Server.Port;
            textBoxRelativeUri.Text = _Server.RelativeUri;
            textBoxCurrentUrl.Text = _Server.Url;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            Configurate();
            this.WindowState = FormWindowState.Minimized;
        }

        private void Configurate()
        {
            _Server.Close();

            _Server.Address = textBoxAddress.Text;
            _Server.Port = (int)numericUpDownPort.Value;
            _Server.RelativeUri = textBoxRelativeUri.Text;

            _Server.Open();
        }

        private void toolStripMenuItemState_Click(object sender, EventArgs e)
        {
            using (FormState form = new FormState(_Server))
            {
                form.LoadEvents(_Events.ToArray());
                form.ShowDialog();
            }
        }
    }
}
