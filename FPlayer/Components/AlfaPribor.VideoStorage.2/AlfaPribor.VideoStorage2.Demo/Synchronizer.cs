using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlfaPribor.VideoStorage.Demo
{
    public partial class Synchronizer : Form
    {
        /// <summary>Хранилище видеоданных</summary>
        IVideoStorage _Storage;

        public Synchronizer(IVideoStorage storage)
        {
            InitializeComponent();
            _Storage = storage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }

        private void btnSynchronize_Click(object sender, EventArgs e)
        {
            VideoStorageResult result = SynchronizeStorage();
            if (result != VideoStorageResult.Ok)
            {
                MessageBox.Show("Ошибка запуска синхронизации хранилища!\nРезультат операции: " + result);
            }
            else
            {
                (this.Owner as MainForm).ShowStatusMessage("Синхронизация запущена...");
            }
        }

        /// <summary>Синхронизирует хранилище видеоданных со списком записей, указанных пользователем</summary>
        /// <returns>Результат выполнения операции</returns>
        private VideoStorageResult SynchronizeStorage()
        {
            List<string> list = new List<string>();
            string ids = txtbIds.Text;
            int index = 0;
            while (index < ids.Length)
            {
                int pos = ids.IndexOf(',', index);
                string id = (pos == -1) ? ids.Substring(index) : ids.Substring(index, pos-index);
                list.Add(id);
                if (pos > -1)
                {
                    index = pos + 1;
                }
                else
                {
                    index += id.Length;
                }
            }
            return _Storage.Synchronize(list);
        }
    }
}
