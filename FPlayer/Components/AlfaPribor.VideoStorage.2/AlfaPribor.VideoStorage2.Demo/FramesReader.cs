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
    public partial class FramesReader : Form
    {
        /// <summary>Хранилище видеоданных</summary>
        private IVideoStorage _Storage;

        /// <summary>Список прочитанных видеокадров</summary>
        private List<VideoFrame> _Frames;

        private FramesViewer _Viewer;

        /// <summary>Интерфейс для последовательного чтения кадров из хранилища</summary>
        private IVideoReader _ConsistentReader;

        public FramesReader(IVideoStorage storage)
        {
            InitializeComponent();
            _Storage = storage;
            _Frames = new List<VideoFrame>();
            _Viewer = new FramesViewer();
            _ConsistentReader = null;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReadOne_Click(object sender, EventArgs e)
        {
            VideoFrame frame;
            _Frames.Clear();
            VideoStorageIntStat result;
            int partition_id = chbUsePartitionId.Checked ? (int)numPartitionId.Value : -1;
            int delta_time = chbUseDeltaTime.Checked ? (int)numDeltaTime.Value : -1;
            result = Read(out frame, (int)numStreamId.Value, (int)numTimeStamp.Value, txtbId.Text, partition_id, delta_time);
            if (result == VideoStorageIntStat.Ok)
            {
                if (frame != null)
                {
                    _Frames.Add(frame);
                }
            }
            else
            {
                MessageBox.Show(
                    "Невозможно прочитать видеокадр из хранилища!\nСтатус полученного интерфейса: " +
                    result
                );
            }
        }

        /// <summary>Читает видеокадр с заданными параметрами из хранилища видеоданных</summary>
        /// <param name="frame">Ссылка на прочитанный видеокадр</param>
        /// <param name="stream_id">Идентификатор видеопотока</param>
        /// <param name="time_stamp">Метка времени кадра</param>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздела хранилища</param>
        /// <param name="delta_time">Допуск к метке времени time_stamp</param>
        /// <returns>Результат операции</returns>
        private VideoStorageIntStat Read(out VideoFrame frame, int stream_id, int time_stamp, string id, int partition_id, int delta_time)
        {
            ResetConsistentReader();
            IVideoReader reader;
            if (partition_id < 0)
            {
                reader = _Storage.GetReader(id);
            }
            else
            {
                reader = _Storage.GetReader(id, partition_id);
            }
            if (reader.Status != VideoStorageIntStat.Ok)
            {
                frame = null;
                return reader.Status;
            }
            VideoStorageResult result;
            try
            {
                if (delta_time < 0)
                {
                    result = reader.ReadFrame(stream_id, time_stamp, out frame);
                }
                else
                {
                    result = reader.ReadFrame(stream_id, time_stamp, delta_time, out frame);
                }
            }
            finally
            {
                reader.Close();
            }
            if (result != VideoStorageResult.Ok)
            {
                MessageBox.Show(
                    "Ошибка чтения видеокадра из хранилища!\nРезультат операции: " +
                    result
                );
            }
            return VideoStorageIntStat.Ok;
        }

        private void btnReadMany_Click(object sender, EventArgs e)
        {
            IList<VideoFrame> frames;
            _Frames.Clear();
            VideoStorageIntStat result;
            int partition_id = chbUsePartitionId.Checked ? (int)numPartitionId.Value : -1;
            int delta_time = chbUseDeltaTime.Checked ? (int)numDeltaTime.Value : -1;
            result = Read(out frames, (int)numTimeStamp.Value, txtbId.Text, partition_id, delta_time);
            if (result == VideoStorageIntStat.Ok)
            {
                _Frames.AddRange(frames);
            }
            else
            {
                MessageBox.Show(
                    "Невозможно прочитать видеозапись из хранилища!\nСтатус полученного интерфейса: " +
                    result
                );
            }
        }

        /// <summary>Читаеит все видеокадры записи с метками времени, больше заданной</summary>
        /// <param name="frames">Список прочитанных видеокадров</param>
        /// <param name="time_stamp">Начальная метка времени</param>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздела хранилища</param>
        /// <param name="delta_time">Допуск к метке времени time_stamp</param>
        /// <returns>Результат операции</returns>
        private VideoStorageIntStat Read(out IList<VideoFrame> frames, int time_stamp, string id, int partition_id, int delta_time)
        {
            ResetConsistentReader();
            IVideoReader reader;
            if (partition_id < 0)
            {
                reader = _Storage.GetReader(id);
            }
            else
            {
                reader = _Storage.GetReader(id, partition_id);
            }
            
            if (reader.Status != VideoStorageIntStat.Ok)
            {
                frames = null;
                return reader.Status;
            }
            VideoStorageResult result;
            try
            {
                if (delta_time < 0)
                {
                    result = reader.ReadFrames(time_stamp, out frames);
                }
                else
                {
                    result = reader.ReadFrames(time_stamp, delta_time, out frames);
                }
            }
            finally
            {
                reader.Close();
            }
            if (result != VideoStorageResult.Ok)
            {
                MessageBox.Show(
                    "Ошибка чтения видеокадра из хранилища!\nРезультат операции: " +
                    result
                );
            }
            return VideoStorageIntStat.Ok;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            _Viewer.Frames = _Frames;
            _Viewer.ShowDialog();
        }

        private void btnReadFirst_Click(object sender, EventArgs e)
        {
            VideoFrame frame;
            _Frames.Clear();
            VideoStorageIntStat result = ReadFirst(out frame, (int)numStreamId.Value);
            if (result == VideoStorageIntStat.Ok)
            {
                if (frame != null)
                {
                    _Frames.Add(frame);
                }
            }
            else
            {
                MessageBox.Show(
                    "Невозможно прочитать видеокадр из хранилища!\nСтатус полученного интерфейса: " +
                    result
                );
            }
        }

        /// <summary>Читает из хранилища первый видеокадр для заданного видеопотока</summary>
        /// <param name="frame">Прочитанный видеокадр</param>
        /// <param name="cam_id">Идентификатор видеопотока</param>
        /// <returns>Результат операции</returns>
        private VideoStorageIntStat ReadFirst(out VideoFrame frame, int cam_id)
        {

            ResetConsistentReader();
            _ConsistentReader = _Storage.GetReader(txtbId.Text);
            CheckButtons();
            if (_ConsistentReader.Status != VideoStorageIntStat.Ok)
            {
                frame = null;
                return _ConsistentReader.Status;
            }
            VideoStorageResult result = _ConsistentReader.ReadFirstFrame(cam_id, out frame);
            if (result != VideoStorageResult.Ok)
            {
                MessageBox.Show(
                    "Ошибка чтения видеокадра из хранилища!\nРезультат операции: " +
                    result
                );
            }
            return VideoStorageIntStat.Ok;
        }

        private void btnReadNext_Click(object sender, EventArgs e)
        {
            VideoFrame frame;
            _Frames.Clear();
            VideoStorageIntStat result = ReadNext(out frame, (int)numStreamId.Value, (int)numOffset.Value);
            if (result == VideoStorageIntStat.Ok)
            {
                if (frame != null)
                {
                    _Frames.Add(frame);
                }
            }
            else
            {
                MessageBox.Show(
                    "Невозможно прочитать видеокадр из хранилища!\nСтатус полученного интерфейса: " +
                    result
                );
            }
        }

        /// <summary>Читает из хранилища следующий видеокадр для заданного видеопотока</summary>
        /// <param name="frame">Прочитанный видеокадр</param>
        /// <param name="cam_id">Идентификатор видеопотока</param>
        /// <param name="step">Приращение для следующего кадра относительно последнего прочитанного</param>
        /// <returns>Результат операции</returns>
        private VideoStorageIntStat ReadNext(out VideoFrame frame, int cam_id, int step)
        {
            if (_ConsistentReader == null)
            {
                frame = null;
                return VideoStorageIntStat.Ok;
            }
            if (_ConsistentReader.Status != VideoStorageIntStat.Ok)
            {
                frame = null;
                return _ConsistentReader.Status;
            }
            VideoStorageResult result = _ConsistentReader.ReadNextFrame(cam_id, step, out frame);
            if (result != VideoStorageResult.Ok)
            {
                MessageBox.Show(
                    "Ошибка чтения видеокадра из хранилища!\nРезультат операции: " +
                    result
                );
            }
            return VideoStorageIntStat.Ok;
        }

        private void CheckButtons()
        {
            btnReadNext.Enabled = _ConsistentReader != null;
            btnReadPrev.Enabled = _ConsistentReader != null;
        }

        private void txtbId_TextChanged(object sender, EventArgs e)
        {
            ResetConsistentReader();
        }

        /// <summary>Удалить иньтерфейс последовательного чтения кадров</summary>
        private void ResetConsistentReader()
        {
            if (_ConsistentReader != null)
            {
                _ConsistentReader.Close();
            }
            _ConsistentReader = null;
            CheckButtons();
        }

        private void btnIndex_Click(object sender, EventArgs e)
        {
            IVideoIndex index = _Storage.GetReader(txtbId.Text).VideoIndex;
            if (index.Status != VideoStorageIntStat.Ok)
            {
                MessageBox.Show(
                    "Невозможно прочитать индекс видеокадра!\nСтатус полученного интерфейса: " +
                    index.Status
                );
                return;
            }
            using (IndexViewer viewer = new IndexViewer(index))
            {
                viewer.ShowDialog();
            }
            index.Close();
        }

        private void btnReadLast_Click(object sender, EventArgs e)
        {
            VideoFrame frame;
            _Frames.Clear();
            VideoStorageIntStat result = ReadLast(out frame, (int)numStreamId.Value);
            if (result == VideoStorageIntStat.Ok)
            {
                if (frame != null)
                {
                    _Frames.Add(frame);
                }
            }
            else
            {
                MessageBox.Show(
                    "Невозможно прочитать видеокадр из хранилища!\nСтатус полученного интерфейса: " +
                    result
                );
            }
        }

        /// <summary>Читает из хранилища последний видеокадр для заданного видеопотока</summary>
        /// <param name="frame">Прочитанный видеокадр</param>
        /// <param name="cam_id">Идентификатор видеопотока</param>
        /// <returns>Результат операции</returns>
        private VideoStorageIntStat ReadLast(out VideoFrame frame, int cam_id)
        {

            ResetConsistentReader();
            _ConsistentReader = _Storage.GetReader(txtbId.Text);
            CheckButtons();
            if (_ConsistentReader.Status != VideoStorageIntStat.Ok)
            {
                frame = null;
                return _ConsistentReader.Status;
            }
            VideoStorageResult result = _ConsistentReader.ReadLastFrame(cam_id, out frame);
            if (result != VideoStorageResult.Ok)
            {
                MessageBox.Show(
                    "Ошибка чтения видеокадра из хранилища!\nРезультат операции: " +
                    result
                );
            }
            return VideoStorageIntStat.Ok;
        }

        private void btnReadPrev_Click(object sender, EventArgs e)
        {
            VideoFrame frame;
            _Frames.Clear();
            VideoStorageIntStat result = ReadPrev(out frame, (int)numStreamId.Value, (int)numOffset.Value);
            if (result == VideoStorageIntStat.Ok)
            {
                if (frame != null)
                {
                    _Frames.Add(frame);
                }
            }
            else
            {
                MessageBox.Show(
                    "Невозможно прочитать видеокадр из хранилища!\nСтатус полученного интерфейса: " +
                    result
                );
            }
        }

        /// <summary>Читает из хранилища предыдущий видеокадр для заданного видеопотока</summary>
        /// <param name="frame">Прочитанный видеокадр</param>
        /// <param name="cam_id">Идентификатор видеопотока</param>
        /// <param name="step">Приращение для следующего кадра относительно последнего прочитанного</param>
        /// <returns>Результат операции</returns>
        private VideoStorageIntStat ReadPrev(out VideoFrame frame, int cam_id, int step)
        {
            if (_ConsistentReader == null)
            {
                frame = null;
                return VideoStorageIntStat.Ok;
            }
            if (_ConsistentReader.Status != VideoStorageIntStat.Ok)
            {
                frame = null;
                return _ConsistentReader.Status;
            }
            VideoStorageResult result = _ConsistentReader.ReadPrevFrame(cam_id, step, out frame);
            if (result != VideoStorageResult.Ok)
            {
                MessageBox.Show(
                    "Ошибка чтения видеокадра из хранилища!\nРезультат операции: " +
                    result
                );
            }
            return VideoStorageIntStat.Ok;
        }
    }
}
