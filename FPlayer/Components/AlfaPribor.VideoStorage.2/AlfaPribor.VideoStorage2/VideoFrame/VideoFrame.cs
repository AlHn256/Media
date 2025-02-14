using System;
using System.Runtime.Serialization;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Видеокадр</summary>
    [Serializable]
    [DataContract]
    public class VideoFrame : IEquatable<VideoFrame>
    {
        #region Fields

        /// <summary>Номер камеры / видеоканала</summary>
        int _CameraId;

        /// <summary>Метка времени кадра в миллисекундах от начала записи</summary>
        int _TimeStamp;

        /// <summary>Дополнительные сведения о кадре</summary>
        VideoContent _Content;

        /// <summary>Бинарные данные кадра</summary>
        byte[] _FrameData;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        public VideoFrame()
        {
            _CameraId = 0;
            _TimeStamp = 0;
            _Content = new VideoContent(VideoContentType.jpeg);
            _FrameData = null;
        }

        /// <summary>Конструктор</summary>
        /// <param name="camera_id">Номер камеры / видеоканала</param>
        /// <param name="time_stamp">Метка времени в мс</param>
        /// <param name="content_type">Тип содержимого, например: image/jpeg</param>
        /// <param name="frame_data">Бинарные данные кадра</param>
        public VideoFrame(int camera_id, int time_stamp, string content_type, byte[] frame_data)
        {
            _CameraId = camera_id;
            _TimeStamp = time_stamp;
            _Content = new VideoContent(VideoContentType.jpeg);
            _Content.Parse(content_type);
            _FrameData = frame_data;
        }

        /// <summary>Конструктор</summary>
        /// <param name="camera_id">Номер камеры / видеоканала</param>
        /// <param name="time_stamp">Метка времени в мс</param>
        /// <param name="video_content">Тип содержимого видеоданных</param>
        /// <param name="frame_data">Бинарные данные кадра</param>
        public VideoFrame(int camera_id, int time_stamp, VideoContentType video_content, byte[] frame_data)
        {
            _CameraId = camera_id;
            _TimeStamp = time_stamp;
            _Content = new VideoContent(VideoContentType.jpeg);
            _Content.Type = video_content;
            _FrameData = frame_data;
        }

        /// <summary>Конструктор</summary>
        /// <param name="camera_id">Номер камеры / видеоканала</param>
        /// <param name="time_stamp">Метка времени в мс</param>
        /// <param name="video_content">Тип содержимого видеоданных</param>
        /// <param name="frame_width">Ширина видеокадра в пикселах</param>
        /// <param name="frame_height">Высота видеокадра в пикселах</param>
        /// <param name="frame_data">Бинарные данные кадра</param>
        public VideoFrame(int camera_id, int time_stamp, VideoContentType video_content, int frame_width, int frame_height, byte[] frame_data)
        {
            _CameraId = camera_id;
            _TimeStamp = time_stamp;
            _Content = new VideoContent(VideoContentType.jpeg);
            _Content.Type = video_content;
            _Content.Width = frame_width;
            _Content.Height = frame_height;
            _FrameData = frame_data;
        }

        #endregion

        #region Properties

        /// <summary>Номер камеры / видеоканала</summary>
        [DataMember]
        public int CameraId
        {
            get { return _CameraId; }
            set { _CameraId = value; }
        }

        /// <summary>Метка времени кадра в миллисекундах от начала записи</summary>
        [DataMember]
        public int TimeStamp
        {
            get { return _TimeStamp; }
            set { _TimeStamp = value; }
        }

        /// <summary>Информация о содержимом выдеокадра: высота/ширина изображения, тип видеоданных, угол поворота изображения, ...</summary>
        [DataMember]
        public VideoContent ContentType
        {
            get { return _Content; }
            set { _Content = value; }
        }

        /// <summary>Бинарные данные кадра</summary>
        [DataMember]
        public byte[] FrameData
        {
            get { return _FrameData; }
            set { _FrameData = value; }
        }

        #endregion

        #region Члены IEquatable<VideoFrame>

        /// <summary>
        /// Определяет идентичность текущего объекта заданному
        /// </summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты идентичны, FALSE - в противном случае</returns>
        public bool Equals(VideoFrame other)
        {
            if (other == null)
            {
                return false;
            }
            bool result = _CameraId == other._CameraId && _TimeStamp == other._TimeStamp && _Content.Equals(other._Content);

            if (_FrameData == other.FrameData) return result;
	        
            if (result)
            {
                result = _FrameData != null && other._FrameData != null;
            }
            if (result)
            {
                result = _FrameData.Length == other._FrameData.Length;
            }
            if (result)
            {
                for (int i = 0; i < _FrameData.Length; i++)
                {
                    if (_FrameData[i] != other._FrameData[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
