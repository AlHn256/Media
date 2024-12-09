using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Класс, обеспечивающий доступ к хранилищу для записи видеоданных</summary>
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    class VideoWriterService : VideoServiceBase, IVideoWriterService
    {
        #region Fields

        /// <summary>Интерфейс записи видеоданных</summary>
        private IVideoWriter _IWriter;

        /// <summary>Последний подготовленный для записи кадр</summary>
        private VideoFrameInfo _FrameInfo;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <param name="writer">Интерфейс записи видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задан интерфейс записи видеоданных</exception>
        public VideoWriterService(IVideoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException();
            }
            _IWriter = writer;
            _FrameInfo = null;
        }

        #endregion

        #region IVideoWriterService Members

        /// <summary>Подготавливает кадр к записи
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoWriterService"/>
        /// </summary>
        /// <param name="frame_info">Данные кадра</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект</exception>
        public void WriteFrame(VideoFrameInfo frame_info)
        {
            _FrameInfo = frame_info;
            if (frame_info == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>Подготавливает кадр к записи
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoWriterService"/>
        /// </summary>
        /// <param name="camera_id">Номер камеры (видеопотока)</param>
        /// <param name="time_stamp">Время от начала записи в миллисекундах</param>
        /// <param name="content_type">Тип содержимого кадра, например: image/jpeg или image/raw</param>
        public void WriteFrame(int camera_id, int time_stamp, string content_type)
        {
            _FrameInfo = new VideoFrameInfo(camera_id, time_stamp, content_type);
        }

        /// <summary>Записать последний подготовленный к записи кадр
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoWriterService"/>
        /// </summary>
        /// <param name="stream">Видеоданные кадра</param>
        /// <returns>Результат выполения операции</returns>
        public VideoStorageResult WriteFrameData(Stream stream)
        {
            if ((stream == null) || (_FrameInfo == null))
            {
                return VideoStorageResult.Fault;
            }
            try
            {
                MemoryStream received_data = new MemoryStream(_FrameInfo.FrameDataSize <= 0 ? 0xFFFF : _FrameInfo.FrameDataSize);
                int buffer_len = 8384;
                byte[] buffer = new byte[buffer_len];
                int count = 0;
                while (stream.CanRead || (count = stream.Read(buffer, 0, buffer_len)) != 0)
                {
                    received_data.Write(buffer, 0, count);
                }
                _FrameInfo.FrameData = received_data.ToArray();
                return _IWriter.WriteFrame((VideoFrame)_FrameInfo);
            }
            catch 
            {
                return VideoStorageResult.Fault;
            }
        }

        /// <summary>Получить дату/время команды начала записи видеопотоков
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoWriterService"/>
        /// </summary>
        public DateTime GetRecordStarted()
        {
            return _IWriter.RecordStarted;
        }

        /// <summary>Задать дату/время команды начала записи видеопотоков
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoWriterService"/>
        /// </summary>
        /// <param name="value">Новое значение даты/времени начала записи</param>
        public void SetRecordStarted(DateTime value)
        {
            _IWriter.RecordStarted = value;
        }

        /// <summary>Дата и время команды окончания записи видеопотоков
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoWriterService"/>
        /// </summary>
        public DateTime GetRecordFinished()
        {
            return _IWriter.RecordFinished;
        }

        /// <summary>Задать дату/время команды окончания записи видеопотоков
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoWriterService"/>
        /// </summary>
        /// <param name="value">Новое значение даты/времени окончания записи</param>
        public void SetRecordFinished(DateTime value)
        {
            _IWriter.RecordFinished = value;
        }

        #endregion

        #region IVideoInterfaceService Members

        /// <summary>Получить идентификатор видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Идентификатор видеозаписи</returns>
        public string GetId()
        {
            return _IWriter.Id;
        }

        /// <summary>Получить идентификатор раздела хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Идентификатор раздела хранилища</returns>
        public int GetPartitionId()
        {
            return _IWriter.PartitionId;
        }

        /// <summary>Получить статус интерфейса чтения видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Cтатус интерфейса чтения видеоданных</returns>
        public VideoStorageIntStat GetStatus()
        {
            return _IWriter.Status;
        }

        /// <summary>Завершить запись
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Cтатус интерфейса чтения видеоданных</returns>
        public VideoStorageResult Close()
        {
            VideoStorageResult result = _IWriter.Close();
            if (result == VideoStorageResult.Ok)
            {
                RaiseCloseEvent();
            }
            return result;
        }

        #endregion
    }
}
