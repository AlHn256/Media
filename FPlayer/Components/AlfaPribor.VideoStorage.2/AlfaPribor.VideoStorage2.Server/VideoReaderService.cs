using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Класс, обеспечивающий доступ к хранилищу для чтения видеоданных</summary>
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    class VideoReaderService : VideoServiceBase, IVideoReaderService
    {
        #region Fields

        /// <summary>Интерфейс чтения видеоданных</summary>
        private IVideoReader _IReader;

        /// <summary>Видеоданные последнего запрошенного кадра</summary>
        private byte[] _FrameData;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <param name="reader">Интерфейс чтения видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задан интерфейс чтения видеоданных</exception>
        public VideoReaderService(IVideoReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException();
            }
            _IReader = reader;
            _FrameData = null;
        }

        #endregion

        #region IVideoReaderService Members

        #region Methods

        /// <summary>Прочитать кадр указанной камеры, соответствующий указанному моменту времени
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoReaderService"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="frame_info">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFrame(int cam_id, int time_stamp, out VideoFrameInfo frame_info)
        {
            try
            {
                VideoFrame frame;
                VideoStorageResult result = _IReader.ReadFrame(cam_id, time_stamp, out frame);
                if (result != VideoStorageResult.Ok)
                {
                    frame_info = null;
                    _FrameData = null;
                }
                else
                {
                    frame_info = new VideoFrameInfo(frame);
                    _FrameData = frame.FrameData;
                }
                return result;
            }
            catch
            {
                frame_info = null;
                return VideoStorageResult.Fault;
            }
        }

        /// <summary>Прочитать кадр указанной камеры, соответствующий указанному моменту времени
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoReaderService"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="delta_time">Интервал времени относительно time_stamp, в пределах которого будет искаться видеокадр</param>
        /// <param name="frame_info">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFrame(int cam_id, int time_stamp, int delta_time, out VideoFrameInfo frame_info)
        {
            try
            {
                VideoFrame frame;
                VideoStorageResult result = _IReader.ReadFrame(cam_id, time_stamp, delta_time, out frame);
                if (result != VideoStorageResult.Ok)
                {
                    frame_info = null;
                    _FrameData = null;
                }
                else
                {
                    frame_info = new VideoFrameInfo(frame);
                    _FrameData = frame.FrameData;
                }
                return result;
            }
            catch
            {
                frame_info = null;
                return VideoStorageResult.Fault;
            }
        }

        /// <summary>Прочитать первый кадр указанного видеопотока
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoReaderService"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="frame_info">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFirstFrame(int cam_id, out VideoFrameInfo frame_info)
        {
            try
            {
                VideoFrame frame;
                VideoStorageResult result = _IReader.ReadFirstFrame(cam_id, out frame);
                if (result != VideoStorageResult.Ok)
                {
                    frame_info = null;
                    _FrameData = null;
                }
                else
                {
                    frame_info = new VideoFrameInfo(frame);
                    _FrameData = frame.FrameData;
                }
                return result;
            }
            catch
            {
                frame_info = null;
                return VideoStorageResult.Fault;
            }
        }

        /// <summary>Прочитать последний кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="frame_info">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadLastFrame(int cam_id, out VideoFrameInfo frame_info)
        {
            try
            {
                VideoFrame frame;
                VideoStorageResult result = _IReader.ReadLastFrame(cam_id, out frame);
                if (result != VideoStorageResult.Ok)
                {
                    frame_info = null;
                    _FrameData = null;
                }
                else
                {
                    frame_info = new VideoFrameInfo(frame);
                    _FrameData = frame.FrameData;
                }
                return result;
            }
            catch
            {
                frame_info = null;
                return VideoStorageResult.Fault;
            }
        }

        /// <summary>Прочитать следующий кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="step">Инкремент номера кадра</param>
        /// <param name="frame_info">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadNextFrame(int cam_id, int step, out VideoFrameInfo frame_info)
        {
            try
            {
                VideoFrame frame;
                VideoStorageResult result = _IReader.ReadNextFrame(cam_id, step, out frame);
                if (result != VideoStorageResult.Ok)
                {
                    frame_info = null;
                    _FrameData = null;
                }
                else
                {
                    frame_info = new VideoFrameInfo(frame);
                    _FrameData = frame.FrameData;
                }
                return result;
            }
            catch
            {
                frame_info = null;
                return VideoStorageResult.Fault;
            }
        }

        /// <summary>Прочитать предыдущий кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="step">Инкремент номера кадра</param>
        /// <param name="frame_info">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadPrevFrame(int cam_id, int step, out VideoFrameInfo frame_info)
        {
            try
            {
                VideoFrame frame;
                VideoStorageResult result = _IReader.ReadPrevFrame(cam_id, step, out frame);
                if (result != VideoStorageResult.Ok)
                {
                    frame_info = null;
                    _FrameData = null;
                }
                else
                {
                    frame_info = new VideoFrameInfo(frame);
                    _FrameData = frame.FrameData;
                }
                return result;
            }
            catch
            {
                frame_info = null;
                return VideoStorageResult.Fault;
            }
        }

        /// <summary>Прочитать первый кадр указанного видеопотока
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoReaderService"/>
        /// </summary>
        /// <returns>Видеоданные кадра</returns>
        public Stream ReadFrameData()
        {
            if (_FrameData == null)
            {
                return null;
            }
            try
            {
                MemoryStream stream = new MemoryStream(_FrameData);
                return stream;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Properties

        /// <summary>Интерфейс чтения видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoReaderService"/>
        /// </summary>
        public IVideoReader VideoReader
        {
            get { return _IReader; }
        }

        #endregion

        #endregion

        #region IVideoInterfaceService Members

        /// <summary>Получить идентификатор видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Идентификатор видеозаписи</returns>
        public string GetId()
        {
            return _IReader.Id;
        }

        /// <summary>Получить идентификатор раздела хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Идентификатор раздела хранилища</returns>
        public int GetPartitionId()
        {
            return _IReader.PartitionId;
        }

        /// <summary>Получить статус интерфейса чтения видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Cтатус интерфейса чтения видеоданных</returns>
        public VideoStorageIntStat GetStatus()
        {
            return _IReader.Status;
        }

        /// <summary>Завершить чтение видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Cтатус интерфейса чтения видеоданных</returns>
        public VideoStorageResult Close()
        {
            VideoStorageResult result = _IReader.Close();
            if (result == VideoStorageResult.Ok)
            {
                RaiseCloseEvent();
            }
            return result;
        }

        #endregion
    }
}
