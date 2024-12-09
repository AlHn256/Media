using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Элемент коллекции видеоиндексов для любого видеопотока</summary>
    /// <remarks>
    /// Содержит данные о видеокадре и его расположении внутри видеопотока.
    /// Класс предназначен для использования в коллекциях для хранения данных всех видеопотоков.
    /// </remarks>
    [Serializable]
    public class MultiStreamFrameIndex : SingleStreamFrameIndex, IComparable<MultiStreamFrameIndex>
    {
        #region Fields

        /// <summary>Значимость свойства "StreamId" при сравнении объектов данного типа</summary>
        const int StreamIdIdWeight = 100;

        /// <summary>Значимость свойства "TimeStamp" при сравнении объектов данного типа</summary>
        const int TimeWeight = 10;

        /// <summary>Значимость свойства "FileOffset" при сравнении объектов данного типа</summary>
        const int OffsetWeight = 1;

        #endregion

        #region Methods

        /// <summary>Создает объект с нулевыми значениями свойств</summary>
        public MultiStreamFrameIndex() : base(0)
        {
            TimeStamp = 0;
            FileOffset = 0;
        }

        /// <summary>Инициализирует объект данными о видеокадре</summary>
        /// <param name="stream_id">Идентификатор камеры/видеопотока</param>
        /// <param name="time">Метка времени видеокадра (мсек)</param>
        /// <param name="offset">Смещение блока данных видеокадра в файле с видеоданными (байт)</param>
        public MultiStreamFrameIndex(int stream_id, int time, long offset) : base (stream_id, time, offset) { }

        /// <summary>Копирует значения свойств другого объекта</summary>
        /// <param name="other">Объект, свойства которого копируются</param>
        public override void Copy(SingleStreamFrameIndex other)
        {
            StreamId = other.StreamId;
            TimeStamp = other.TimeStamp;
            FileOffset = other.FileOffset;
        }

        #endregion

        #region Properties

        /// <summary>Идентификатор камеры/видеопотока</summary>
        public new int StreamId
        {
            get { return base.StreamId; }
            set { SetStreamId(value); }
        }

        #endregion

        #region IComparable<MultiStreamFrameIndex> Members

        /// <summary>Метод ставнения объектов данного типа</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>
        /// Возвращает значения:
        /// меньше 0 = данный объект меньше объекта, заданного параметром other
        /// 0        = объекты равны
        /// больше 0 = данный объект больше объекта, заданного параметром other
        /// </returns>
        public int CompareTo(MultiStreamFrameIndex other)
        {
            int result = 0;
            // Вычисляем разницу между объектами исходя из значимости разницы их полей
            if (StreamId < other.StreamId) result -= StreamIdIdWeight;
            else if (StreamId > other.StreamId) result += StreamIdIdWeight;
            if (TimeStamp < other.TimeStamp) result -= TimeWeight;
            else if (TimeStamp > other.TimeStamp) result += TimeWeight;
            if (FileOffset < other.FileOffset) result -= OffsetWeight;
            else if (FileOffset > other.FileOffset) result += OffsetWeight;
            return result;
        }

        #endregion
    }
}
