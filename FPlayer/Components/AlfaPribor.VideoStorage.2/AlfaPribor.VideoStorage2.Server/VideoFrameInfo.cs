using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using AlfaPribor.VideoStorage;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Класс содержит информацию о видеокадре</summary>
    [DataContract]
    public class VideoFrameInfo: VideoFrame, IEquatable<VideoFrameInfo>, IComparable<VideoFrameInfo>
    {
        #region Fields

        /// <summary>Значимость свойства "CameraId" при сравнении объектов данного типа</summary>
        private const int CameraIdWeight = TimeStampWeight * 2;

        /// <summary>Значимость свойства "TimeStamp" при сравнении объектов данного типа</summary>
        private const int TimeStampWeight = 1;

        /// <summary>Размер видеоданных кадра (байт)</summary>
        private int _FrameDataSize;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        public VideoFrameInfo()
        {
            _FrameDataSize = 0;
        }

        /// <summary>Конструктор</summary>
        /// <param name="cam_id">Номер камеры / видеоканала</param>
        /// <param name="time_stamp">Метка времени в мс</param>
        public VideoFrameInfo(int cam_id, int time_stamp)
            : this()
        {
            CameraId = cam_id;
            TimeStamp = time_stamp;
        }

        /// <summary>Конструктор</summary>
        /// <param name="cam_id">Номер камеры / видеоканала</param>
        /// <param name="time_stamp">Метка времени в мс</param>
        /// <param name="content_type">Тип содержимого кадра</param>
        public VideoFrameInfo(int cam_id, int time_stamp, string content_type)
            : base(cam_id, time_stamp, content_type, null)
        {
            _FrameDataSize = 0;
        }

        /// <summary>Конструктор копирования</summary>
        /// <param name="other">Видеокадр</param>
        public VideoFrameInfo(VideoFrame other) :
            this()
        {
            this.CameraId = other.CameraId;
            this.TimeStamp = other.TimeStamp;
            this.FrameData = other.FrameData;
            this.ContentType = other.ContentType;
        }

        #endregion

        #region Properties

        /// <summary>Размер видеоданных кадра (байт)</summary>
        [DataMember]
        public int FrameDataSize
        {
            get 
            {
                if (FrameData == null)
                {
                    return _FrameDataSize;
                }
                else
                {
                    return FrameData.Length;
                }
            }
            set 
            {
                _FrameDataSize = value;
            }
        }

        #endregion

        #region IEquatable<VideoFrameInfo> Members

        /// <summary>Определяет идентичность текущего объекта и заданного</summary>
        /// <param name="other">Сравниваемый объект</param>
        /// <returns>TRUE - объекты идентичны, FALSE - в противном случае</returns>
        public bool Equals(VideoFrameInfo other)
        {
            return
                (this.CameraId == other.CameraId) &&
                (this.TimeStamp == other.TimeStamp);
        }

        #endregion

        #region Члены IComparable<VideoFrameInfo>

        /// <summary>Метод ставнения объектов данного типа</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>
        /// Возвращает значения:
        /// меньше 0 = данный объект меньше объекта, заданного параметром other
        /// 0        = объекты равны
        /// больше 0 = данный объект больше объекта, заданного параметром other
        /// </returns>
        public int CompareTo(VideoFrameInfo other)
        {
            int result = 0;
            if (this.CameraId < other.CameraId)
            {
                result -= CameraIdWeight;
            }
            else if (this.CameraId > other.CameraId)
            {
                result += CameraIdWeight;
            }

            if (this.TimeStamp < other.TimeStamp)
            {
                result -= TimeStampWeight;
            }
            else if (this.TimeStamp < other.TimeStamp)
            {
                result += TimeStampWeight;
            }

            return result;
        }

        #endregion
    }
}
