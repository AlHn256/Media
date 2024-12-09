using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage
{
    /// <summary>
    /// Элемент коллекции видеоиндексов
    /// </summary>
    /// <remarks>
    /// Содержит данные об отдельно взятом видеокадре. Реализует интерфейсы IEquatable и IComparable
    /// для использования в упорядоченных коллекциях
    /// </remarks>
    [Serializable]
    public struct FrameIndex : IEquatable<FrameIndex>, IComparable<FrameIndex>
    {
        #region Fields

        /// <summary>
        /// Значимость поля "CameraId" при сравнении объектов данного типа
        /// </summary>
        const int CameraIdWeight = (TimeWeight + OffsetWeight) * 2;

        /// <summary>
        /// Значимость поля "Time" при сравнении объектов данного типа
        /// </summary>
        const int TimeWeight = OffsetWeight * 2;

        /// <summary>
        /// Значимость поля "Offset" при сравнении объектов данного типа
        /// </summary>
        const int OffsetWeight = 1;

        /// <summary>
        /// Идентификатор камеры/видеопотока
        /// </summary>
        public int CameraId;

        /// <summary>
        /// Метка времени видеокадра (мсек)
        /// </summary>
        public int Time;

        /// <summary>
        /// Смещение блока данных видеокадра в файле с видеоданными (байт)
        /// </summary>
        public long Offset;

        #endregion

        #region Methods

        /// <summary>
        /// Инициализирует объект данными о видеокадре
        /// </summary>
        /// <param name="cam_id">Идентификатор камеры/видеопотока</param>
        /// <param name="time">Метка времени видеокадра (мсек)</param>
        /// <param name="offset">Смещение блока данных видеокадра в файле с видеоданными (байт)</param>
        public FrameIndex(int cam_id, int time, long offset)
        {
            CameraId = cam_id;
            Time = time;
            Offset = offset;
        }

        #endregion

        #region IEquatable<FrameIndex> Members

        /// <summary>
        /// Метод сравнения объектов на равенство значений
        /// </summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>
        /// Возвращает TRUE, если объекты равны, в противном случае возвращает FALSE
        /// </returns>
        public bool Equals(FrameIndex other)
        {
            return
                (CameraId == other.CameraId) &&
                (Time == other.Time) &&
                (Offset == other.Offset);
        }

        #endregion

        #region IComparable<FrameIndex> Members

        /// <summary>
        /// Метод ставнения объектов данного типа
        /// </summary>
        /// <param name="other">Объект, с тоторым происходит сравнение</param>
        /// <returns>
        /// Возвращает значения:
        /// меньше 0 = данный объект меньше объекта, заданного параметром other
        /// 0        = объекты равны
        /// больше 0 = данный объект больше объекта, заданного параметром other
        /// </returns>
        public int CompareTo(FrameIndex other)
        {
            int result = 0;
            // Вычисляем разницу между объектами исходя из значимости разницы их полей
            if (CameraId < other.CameraId)
                result -= CameraIdWeight;
            else if (CameraId > other.CameraId)
                result += CameraIdWeight;

            if (Time < other.Time)
                result -= TimeWeight;
            else if (Time > other.Time)
                result += TimeWeight;

            if (Offset < other.Offset)
                result -= OffsetWeight;
            else if (Offset > other.Offset)
                result += OffsetWeight;

            return result;
        }

        #endregion
    }
}
