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
    public partial class RecordEraser : Form
    {
        private IVideoStorage _Storage;

        public RecordEraser(IVideoStorage storage)
        {
            InitializeComponent();
            _Storage = storage;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            VideoStorageResult result;
            if (rbtnDeleteOne.Checked)
            {
                result = Delete(txtbId.Text);
            }
            else
            {
                result = DeleteAll();
            }
            if (result != VideoStorageResult.Ok)
            {
                MessageBox.Show("Ошибка удаления записи(ей) из хранилища!\nРезультат операции: " + result);
            }
        }

        /// <summary>Удаляет выбранную запись из хранилища</summary>
        /// <param name="id">Идентификатор удаляемой видеозаписи</param>
        /// <returns>Результат операции</returns>
        private VideoStorageResult Delete(string id)
        {
            return _Storage.Delete(id);
        }

        /// <summary>Удаляет все записи из хранилища</summary>
        /// <returns>Результат операции</returns>
        private VideoStorageResult DeleteAll()
        {
            return _Storage.DeleteAll();
        }
    }
}
