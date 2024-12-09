using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Информация о видеокадре</summary>
   
    public class VideoFrameData : IEquatable<VideoFrameData>
    {

        /// <summary>Конструктор</summary>
        public VideoFrameData()
        {
            CameraId = 0;
            TimeSpan = 0;
        }

        /// <summary>Конструктор</summary>
        /// <param name="camera_id">Идентификатор камеры</param>
        /// <param name="time_span">Метка времени в миллисекундах</param>
        public VideoFrameData(int camera_id, int time_span)
        {
            CameraId = camera_id;
            TimeSpan = time_span;
        }

        /// <summary>Конструктор</summary>
        /// <param name="camera_id">Идентификатор камеры</param>
        /// <param name="time_span">Метка времени в миллисекундах</param>
        /// <param name="data">Видеоданные кадра</param>
        public VideoFrameData(int camera_id, int time_span, byte[] data)
        {
            CameraId = camera_id;
            TimeSpan = time_span;
            Data = data;
        }

        /// <summary>Идентификатор видеокамеры</summary>
       
        public int CameraId { get; set; }

        /// <summary>Метка времени кадра от начала записи (в миллисекундах)</summary>
       
        public int TimeSpan { get; set; }

        /// <summary>Видеоданные кадра</summary>
       
        public byte[] Data { get; set; }

        /// <summary>Получить строку метки времени</summary>
        /// <returns>Строковое представление метки времени</returns>
        public string GetTimeSpanString()
        {
            int value = TimeSpan;
            if (value >= 0)
            {
                int ms = value % 1000;
                int ts = value / 1000;
                int m = ts / 60;
                int s = ts % 60;
                return m.ToString() + ":" + s.ToString("00") + "." + ms.ToString("000");
            }
            return string.Empty;
        }

        #region Члены IEquatable<VideoFrameData>

        /// <summary>Проверяет равенство текущего объекта заданному</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты равны, FALSE - в противном случае</returns>
        public bool Equals(VideoFrameData other)
        {
            if (other == null)
            {
                return false;
            }
            bool result = CameraId == other.CameraId && TimeSpan == other.TimeSpan;
            if (result)
            {
                if (Data == other.Data)
                {
                    return result;
                }
                result = Data != null && other.Data != null;
            }
            if (result)
            {
                result = Data.Length == other.Data.Length;
            }
            if (result)
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    if (Data[i] != other.Data[i])
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
