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
    public partial class StorageInfo : Form
    {
        /// <summary>Хранилище видеоданных</summary>
        private BaseVideoStorage _Storage;

        public StorageInfo(BaseVideoStorage storage)
        {
            InitializeComponent();
            _Storage = storage;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        /// <summary>Обновляет информацию о разделах хранилища</summary>
        public void RefreshInfo()
        {
            lvwPartitions.BeginUpdate();
            lvwPartitions.Items.Clear();
            VideoStorageInfo info = _Storage.Info;
            foreach (VideoPartitionInfo partition in info.Partitions)
            {
                ListViewItem item = new ListViewItem();
                SetPartitionInfo(item, partition);
                lvwPartitions.Items.Add(item);
            }
            lvwPartitions.EndUpdate();
        }

        /// <summary>Записать данные раздела хранилища в визуальный контрол</summary>
        /// <param name="item">Элемент списка lvwPartitions, содержащий данные о разделе хранилища</param>
        /// <param name="partition">Информация о разделе хранилища</param>
        private void SetPartitionInfo(ListViewItem item, VideoPartitionInfo partition)
        {
            SetValueByColumn(item, clhId, partition.Id.ToString());
            SetValueByColumn(item, clhPath, partition.Path);
            SetValueByColumn(item, clhActive, partition.Active.ToString());
            SetValueByColumn(item, clhSpace, (partition.TotalSpace / (1024L * 1024L)).ToString());
            SetValueByColumn(item, clhFreeSpace, (partition.FreeSpace / (1024L * 1024L)).ToString());
            SetValueByColumn(item, clhUsed, (partition.UsedSpace / (1024L * 1024L)).ToString());
            SetValueByColumn(item, clhRecordsCount, partition.RecordCount.ToString());
        }

        /// <summary>Задать значение для элемента item сиписка lvwPartitions по имени столбца</summary>
        /// <param name="item">Элемента списка lvwPartitions, значение которого изменяется</param>
        /// <param name="col_header">Столбец, в котором нужно изменить значение</param>
        /// <param name="value"></param>
        /// <exception cref="System.ApplicationException">
        /// Столбец с заданным именем не найден в контроле lvwPartitions
        /// </exception>
        private void SetValueByColumn(ListViewItem item, ColumnHeader col_header, string value)
        {
            if (!lvwPartitions.Columns.Contains(col_header))
            {
                throw new ApplicationException("Column '" + col_header.Name + "' not found in control '" + lvwPartitions.Name + "'!");
            }
            int index = lvwPartitions.Columns.IndexOf(col_header);
            ListViewItem.ListViewSubItemCollection sub_items = item.SubItems;
            if (index < sub_items.Count)
            {
                sub_items[index].Text = value;
            }
            else
            {
                ListViewItem.ListViewSubItem sub_item = new ListViewItem.ListViewSubItem(item, value);
                sub_items.Insert(index, sub_item);
            }
        }

        private void StorageInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
