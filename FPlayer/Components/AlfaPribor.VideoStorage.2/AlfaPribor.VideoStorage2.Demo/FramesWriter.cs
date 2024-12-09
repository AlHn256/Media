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
    public partial class FramesWriter : Form
    {
        /// <summary>Отредактированный пользователем видеокадр</summary>
        private VideoFrame _Frame;

        private FrameEditor FrameEditor;

        /// <summary>Хранилище видеоданных</summary>
        private IVideoStorage _Storage;

        public FramesWriter(IVideoStorage storage)
        {
            InitializeComponent();
            _Storage = storage;
            _Frame = new VideoFrame();
            FrameEditor = new FrameEditor();
        }

        ~FramesWriter()
        {
            FrameEditor.Dispose();
        }

        private void btnEditFrame_Click(object sender, EventArgs e)
        {
            FrameEditor.Frame = _Frame;
            if (FrameEditor.ShowDialog() == DialogResult.OK)
            {
                _Frame = FrameEditor.Frame;
            }
        }

        private void bttClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnWriteFrame_Click(object sender, EventArgs e)
        {
            VideoStorageIntStat result;
            DateTime rec_begin = dtpBeginRecDate.Value.Date + dtpBeginRecTime.Value.TimeOfDay;
            DateTime rec_end = dtpEndRecDate.Value.Date + dtpEndRecTime.Value.TimeOfDay;
            if (rbtnWriteOne.Checked)
            {
                if (chbChangeRecTime.Checked)
                {
                    result = Write(txtbId.Text, _Frame, rec_begin, rec_end);
                }
                else
                {
                    result = Write(txtbId.Text, _Frame);
                }
            }
            else if (rbtnWriteMany.Checked)
            {
                if (chbChangeRecTime.Checked)
                {
                    result = Write(txtbId.Text, _Frame, (int)numFramesCount.Value, chbManyStreams.Checked, rec_begin, rec_end);
                }
                else
                {
                    result = Write(txtbId.Text, _Frame, (int)numFramesCount.Value, chbManyStreams.Checked);
                }
            }
            else
            {
                if (chbChangeRecTime.Checked)
                {
                    result = Write(txtbId.Text, _Frame, (int)numFramesOnStream.Value, (int)numStreamsCount.Value, rec_begin, rec_end);
                }
                else
                {
                    result = Write(txtbId.Text, _Frame, (int)numFramesOnStream.Value, (int)numStreamsCount.Value); ;
                }
            }
            if (result != VideoStorageIntStat.Ok)
            {
                MessageBox.Show(
                    "Невозможно поместить видеозапись в хранилище!\nСтатус полученного интерфейса: " +
                    result
                );
            }

        }

        /// <summary>Поместить несколько видеокадров в хранилище</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="frame">Видеокадр</param>
        /// <param name="count">Количество повторений кадра</param>
        /// <param name="unique_stream_id">Использовать уникальный идентификатор потока для каждого кадра</param>
        /// <returns>Статус возвращенного щранилищем интерфейса</returns>
        private VideoStorageIntStat Write(string id, VideoFrame frame, int count, bool unique_stream_id)
        {
            IVideoWriter writer = _Storage.GetWriter(id);
            if (writer.Status != VideoStorageIntStat.Ok)
            {
                return writer.Status;
            }
            // Записываем серию кадров с разными метками временем (через 40 мсек)
            int time_stamp = frame.TimeStamp;
            int cam_id = frame.CameraId;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    VideoFrame new_frame = new VideoFrame(cam_id, time_stamp, frame.ContentType.Type, frame.FrameData);
                    writer.WriteFrame(new_frame);
                    time_stamp += 40;
                    if (unique_stream_id)
                    {
                        cam_id += 1;
                    }
                }
            }
            finally
            {
                writer.Close();
            }
            return writer.Status;
        }

        /// <summary>Поместить запись с несколькими видеопотоками в хранилище</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="frame">Эталонный видеокадр</param>
        /// <param name="count">Количество повторений кадра в каждом потоке</param>
        /// <param name="streams_count">Количество видеопотоков</param>
        /// <returns>Статус возвращенного щранилищем интерфейса</returns>
        private VideoStorageIntStat Write(string id, VideoFrame frame, int count, int streams_count)
        {
            IVideoWriter writer = _Storage.GetWriter(id);
            if (writer.Status != VideoStorageIntStat.Ok)
            {
                return writer.Status;
            }
            Random generator = new Random();
            try
            {
                for (int cam_id = 0; cam_id < streams_count; ++cam_id)
                {
                    int time_stamp = generator.Next(1001);
                    for (int i = 0; i < count; i++)
                    {
                        VideoFrame new_frame = new VideoFrame(cam_id, time_stamp, frame.ContentType.Type, frame.FrameData);
                        writer.WriteFrame(new_frame);
                        time_stamp += 40;
                    }
                }
            }
            finally
            {
                writer.Close();
            }
            return writer.Status;
        }

        /// <summary>Поместить запись с несколькими видеопотоками в хранилище</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="frame">Эталонный видеокадр</param>
        /// <param name="count">Количество повторений кадра в каждом потоке</param>
        /// <param name="streams_count">Количество видеопотоков</param>
        /// <param name="rec_begin">Дата/время начала записи</param>
        /// <param name="rec_end">Дата/время окончания записи</param>
        /// <returns>Статус возвращенного щранилищем интерфейса</returns>
        private VideoStorageIntStat Write(string id, VideoFrame frame, int count, int streams_count, DateTime rec_begin, DateTime rec_end)
        {
            IVideoWriter writer = _Storage.GetWriter(id);
            if (writer.Status != VideoStorageIntStat.Ok)
            {
                return writer.Status;
            }
            Random generator = new Random();
            try
            {
                for (int cam_id = 0; cam_id < streams_count; ++cam_id)
                {
                    int time_stamp = generator.Next(1001);
                    for (int i = 0; i < count; i++)
                    {
                        VideoFrame new_frame = new VideoFrame(cam_id, time_stamp, frame.ContentType.Type, frame.FrameData);
                        writer.WriteFrame(new_frame);
                        time_stamp += 40;
                    }
                    ++cam_id;
                }
            }
            finally
            {
                writer.RecordStarted = rec_begin;
                writer.RecordFinished = rec_end;
                writer.Close();
            }
            return writer.Status;
        }

        /// <summary>Поместить несколько видеокадров в хранилище</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="frame">Видеокадр</param>
        /// <param name="count">Количество повторений кадра</param>
        /// <param name="unique_stream_id">Использовать уникальный идентификатор потока для каждого кадра</param>
        /// <param name="rec_begin">Дата/время начала записи</param>
        /// <param name="rec_end">Дата/время окончания записи</param>
        /// <returns>Статус возвращенного щранилищем интерфейса</returns>
        private VideoStorageIntStat Write(string id, VideoFrame frame, int count, bool unique_stream_id, DateTime rec_begin, DateTime rec_end)
        {
            IVideoWriter writer = _Storage.GetWriter(id);
            if (writer.Status != VideoStorageIntStat.Ok)
            {
                return writer.Status;
            }
            // Записываем серию кадров с разными метками временем (через 40 мсек)
            int time_stamp = frame.TimeStamp;
            int cam_id = frame.CameraId;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    VideoFrame new_frame = new VideoFrame(cam_id, time_stamp, frame.ContentType.Type, frame.FrameData);
                    writer.WriteFrame(new_frame);
                    time_stamp += 40;
                    if (unique_stream_id)
                    {
                        cam_id += 1;
                    }
                }
            }
            finally
            {
                writer.RecordStarted = rec_begin;
                writer.RecordFinished = rec_end;
                writer.Close();
            }
            return writer.Status;
        }
        
        /// <summary>Поместить видеокадр в хранилище</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="frame">Видеокадр</param>
        /// <param name="count">Число повторений записи кадра</param>
        /// <returns>Статус возвращенного щранилищем интерфейса</returns>
        private VideoStorageIntStat Write(string id, VideoFrame frame)
        {
            IVideoWriter writer = _Storage.GetWriter(id);
            if (writer.Status != VideoStorageIntStat.Ok)
            {
                return writer.Status;
            }
            try
            {
                writer.WriteFrame(frame);
            }
            finally
            {
                writer.Close();
            }
            
            return writer.Status;
        }

        /// <summary>Поместить видеокадр в хранилище</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="frame">Видеокадр</param>
        /// <param name="count">Число повторений записи кадра</param>
        /// <param name="rec_begin">Дата/время начала записи</param>
        /// <param name="rec_end">Дата/время окончания записи</param>
        /// <returns>Статус возвращенного щранилищем интерфейса</returns>
        private VideoStorageIntStat Write(string id, VideoFrame frame, DateTime rec_begin, DateTime rec_end)
        {
            IVideoWriter writer = _Storage.GetWriter(id);
            if (writer.Status != VideoStorageIntStat.Ok)
            {
                return writer.Status;
            }
            try
            {
                writer.WriteFrame(frame);
                writer.RecordStarted = rec_begin;
                writer.RecordFinished = rec_end;
            }
            finally
            {
                writer.Close();
            }

            return writer.Status;
        }

    }
}
