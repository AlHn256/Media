using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Информация о маркированном кадре</summary>
    public class MarkedFrame : IEquatable<MarkedFrame>
    {

        /// <summary>Конструктор</summary>
        public MarkedFrame()
        {
            TrainId = 0;
            CameraId = 0;
            CameraName = string.Empty;
            TimeSpan = 0;
        }

        /// <summary>Конструктор</summary>
        /// <param name="camera_id">Идентификатор камеры</param>
        /// <param name="time_span">Метка времени в миллисекундах</param>
        public MarkedFrame(int camera_id, int time_span)
        {
            TrainId = 0;
            CameraId = camera_id;
            CameraName = string.Empty;
            TimeSpan = time_span;
        }

        /// <summary>Конструктор</summary>
        /// <param name="train_id">Идентификатор поезда в системе АСКО ПВ</param>
        /// <param name="camera_id">Идентификатор камеры</param>
        /// <param name="camera_name">Наименование камеры</param>
        /// <param name="time_span">Метка времени в миллисекундах</param>
        public MarkedFrame(int train_id, int camera_id, string camera_name, int time_span)
        {
            TrainId = train_id;
            CameraId = camera_id;
            CameraName = camera_name;
            TimeSpan = time_span;
        }

        /// <summary>Идентификатор поезда в системе АСКО ПВ</summary>
        public int TrainId { get; set; }

        /// <summary>Идентификатор камеры</summary>
        public int CameraId { get; set; }

        /// <summary>Наименование камеры</summary>
        public string CameraName { get; set; }

        /// <summary>Метка времени в миллисекундах</summary>
        public int TimeSpan { get; set; }

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

        #region Члены IEquatable<FrameMarkData>

        /// <summary>Проверяет равенство текущего объекта заданному</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты равны, FALSE - в противном случае</returns>
        public bool Equals(MarkedFrame other)
        {
            if (other == null)
            {
                return false;
            }
            return
                TrainId == other.TrainId &&
                CameraId == other.CameraId &&
                CameraName == other.CameraName &&
                TimeSpan == other.TimeSpan;
        }

        #endregion
    }

}
