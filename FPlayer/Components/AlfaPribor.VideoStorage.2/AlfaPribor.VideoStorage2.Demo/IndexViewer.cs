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
    public partial class IndexViewer : Form
    {
        private IVideoIndex _VideoIndex;

        public IndexViewer(IVideoIndex index)
        {
            InitializeComponent();
            _VideoIndex = index;
            ViewIndex(index);
        }

        private void ViewIndex (IVideoIndex video_index)
        {
            txtbId.Text = video_index.Id;
            txtbBeginRecord.Text = video_index.RecordStarted.ToString();
            txtbEndRecord.Text = video_index.RecordFinished.ToString();
            lvwStreamInfo.BeginUpdate();
            lvwStreamInfo.Items.Clear();
            foreach (VideoStreamInfo info in video_index.StreamInfoList)
            {
                ListViewItem item = new ListViewItem();
                SetStreamInfo(item, info);
                lvwStreamInfo.Items.Add(item);
            }
            lvwStreamInfo.EndUpdate();
        }

        /// <summary>Записать данные видеопотока в визуальный контрол</summary>
        /// <param name="item">Элемент списка lvwStreamInfo, содержащий данные о видеопотоке</param>
        /// <param name="partition">Информация о видеопотоке</param>
        private void SetStreamInfo(ListViewItem item, VideoStreamInfo info)
        {
            SetValueByColumn(item, clhStreamId, info.Id.ToString());
            SetValueByColumn(item, clhContentType, info.ContentType);
            SetValueByColumn(item, clhResolution, info.Resolution);
            SetValueByColumn(item, clhRotation, info.Rotation.ToString());
            SetValueByColumn(item, clhWidth, info.Width.ToString());
            SetValueByColumn(item, clhHeight, info.Height.ToString());
        }

        /// <summary>Задать значение для элемента item сиписка lvwStreamInfo по имени столбца</summary>
        /// <param name="item">Элемента списка lvwStreamInfo, значение которого изменяется</param>
        /// <param name="col_header">Столбец, в котором нужно изменить значение</param>
        /// <param name="value"></param>
        /// <exception cref="System.ApplicationException">
        /// Столбец с заданным именем не найден в контроле lvwStreamInfo
        /// </exception>
        private void SetValueByColumn(ListViewItem item, ColumnHeader col_header, string value)
        {
            if (!lvwStreamInfo.Columns.Contains(col_header))
            {
                throw new ApplicationException("Column '" + col_header.Name + "' not found in control '" + lvwStreamInfo.Name + "'!");
            }
            int index = lvwStreamInfo.Columns.IndexOf(col_header);
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

        private void btnFirstFrame_Click(object sender, EventArgs e)
        {
            int result = _VideoIndex.GetStartTime((int)numDeltaTime.Value);
            txtbFirstFrame.Text = result.ToString();
        }

        private void btnLastFrame_Click(object sender, EventArgs e)
        {
            int result = _VideoIndex.GetFinishTime((int)numDeltaTime.Value);
            txtbLastFrame.Text = result.ToString();
        }

        private void btnNextFrame_Click(object sender, EventArgs e)
        {
            int result = _VideoIndex.GetNextFrameTime((int)numStreamId.Value, (int)numTimeStamp.Value);
            txtbNextFrame.Text = result.ToString();
        }

        private void btnPrevFrame_Click(object sender, EventArgs e)
        {
            int result = _VideoIndex.GetPrevFrameTime((int)numStreamId.Value, (int)numTimeStamp.Value);
            txtbPrevFrame.Text = result.ToString();
        }
    }
}
