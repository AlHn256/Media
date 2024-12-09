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
    public partial class FramesRewriter : Form
    {
        /// <summary>Хранилище видеозаписей</summary>
        private IVideoStorage _Storage;

        public FramesRewriter(IVideoStorage storage)
        {
            InitializeComponent();
            _Storage = storage;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRewrite_Click(object sender, EventArgs e)
        {
            string id = txtId.Text;
            IVideoReader reader = _Storage.GetReader(id);
            if (reader.Status != VideoStorageIntStat.Ok)
            {
                MessageBox.Show("Не могу получить читающий интерфейс!\nСтатус возвращенного интерфейса: " + reader.Status);
                return;
            }

            IVideoIndex index = reader.VideoIndex;
            IVideoWriter writer = _Storage.GetRewriter(id);
            
            if (writer.Status != VideoStorageIntStat.Ok)
            {
                MessageBox.Show("Не могу получить пишущий интерфейс!\nСтатус возвращенного интерфейса: " + writer.Status);
                reader.Close();
                return;
            }
            VideoFrame frame = null;
            VideoStorageResult reader_result = VideoStorageResult.Ok;
            VideoStorageResult writer_result = VideoStorageResult.Ok;
            try
            {
                foreach (VideoStreamInfo stream_info in index.StreamInfoList)
                {
                    int cam_id = stream_info.Id;
                    reader_result = reader.ReadFirstFrame(cam_id, out frame);
                    if (reader_result != VideoStorageResult.Ok)
                    {
                        goto End_Rewrite;
                    }
                    writer_result = writer.WriteFrame(frame);
                    if (writer_result != VideoStorageResult.Ok)
                    {
                        goto End_Rewrite;
                    }
                    while ((reader_result = reader.ReadNextFrame(cam_id, 1, out frame)) == VideoStorageResult.Ok)
                    {
                        writer_result = writer.WriteFrame(frame);
                        if (writer_result != VideoStorageResult.Ok)
                        {
                            goto End_Rewrite;
                        }
                    }
                }

            End_Rewrite:
                if ((reader_result != VideoStorageResult.Ok) && (reader_result != VideoStorageResult.NotFound))
                {
                    MessageBox.Show("Ошибка чтения кадра!\nКод ошибки: " + reader_result);
                    return;
                }
                if (writer_result != VideoStorageResult.Ok)
                {
                    MessageBox.Show("Ошибка записи кадра!\nКод ошибки: " + reader_result);
                    return;
                }
            }
            finally
            {
                reader.Close();
                writer.Close();
            }
        }
    }
}
