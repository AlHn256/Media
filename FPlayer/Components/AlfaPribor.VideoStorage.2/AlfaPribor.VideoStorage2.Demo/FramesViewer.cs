using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlfaPribor.VideoStorage.Demo
{
    public partial class FramesViewer : Form
    {
        /// <summary>Список видеокадров</summary>
        private IList<VideoFrame> _Frames;


        public FramesViewer()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public IList<VideoFrame> Frames
        {
            get { return _Frames; }
            set
            {
                pbxVideoData.Image = null;
                _Frames = value;
                lvwFrames.BeginUpdate();
                lvwFrames.Items.Clear();
                foreach (VideoFrame frame in value)
                {
                    ListViewItem item = new ListViewItem();
                    if (frame.FrameData != null)
                    {
                        item.Tag = frame.FrameData;
                    }
                    SetFrame(item, frame);
                    lvwFrames.Items.Add(item);
                }
                lvwFrames.EndUpdate();
            }
        }

        /// <summary>Записать данные видеокадра в визуальный контрол</summary>
        /// <param name="item">Элемент списка lvwFrames, содержащий данные о видеокадре</param>
        /// <param name="partition">Информация о видеокадре</param>
        private void SetFrame(ListViewItem item, VideoFrame frame)
        {
            SetValueByColumn(item, clhStreamId, frame.CameraId.ToString());
            SetValueByColumn(item, clhTimeStamp, frame.TimeStamp.ToString());
            SetValueByColumn(item, clhContentType, frame.ContentType.ToString());
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
            if (!lvwFrames.Columns.Contains(col_header))
            {
                throw new ApplicationException("Column '" + col_header.Name + "' not found in control '" + lvwFrames.Name + "'!");
            }
            int index = lvwFrames.Columns.IndexOf(col_header);
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

        /// <summary>Получить значение для элемента item сиписка lvwPartitions по имени столбца</summary>
        /// <param name="col_header">Столбец, из которого нужно прочитать значение</param>
        /// <param name="item">Элемент списка lvwPartitions, из которого нужно прочитать значение</param>
        private string GetValueByColumn(ColumnHeader col_header, ListViewItem item)
        {
            if (!lvwFrames.Columns.Contains(col_header))
            {
                throw new ApplicationException("Column '" + col_header.Name + "' not found in control '" + lvwFrames.Name + "'!");
            }
            int index = lvwFrames.Columns.IndexOf(col_header);
            ListViewItem.ListViewSubItemCollection sub_items = item.SubItems;
            return sub_items[index].Text;
        }

        private void lvwFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = lvwFrames.SelectedItems;
            if (items.Count == 0)
            {
                return;
            }
            byte[] FrameData = (byte[])items[0].Tag;
            if (FrameData == null)
            {
                pbxVideoData.Image = null;
            }
            else
            {
                try
                {
                    using (MemoryStream stream = new MemoryStream(FrameData))
                    {
                        pbxVideoData.Image = Image.FromStream(stream);
                    }
                }
                catch
                {
                    pbxVideoData.Image = null;
                }
            }
        }

    }
}
