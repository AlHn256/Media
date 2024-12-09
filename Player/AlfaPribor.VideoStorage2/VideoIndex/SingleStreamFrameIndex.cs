using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Элемент коллекции видеоиндексов для отдельно взятого видеопотока</summary>
    /// <remarks>
    /// Содержит данные о видеокадре и его расположении внутри видеопотока.
    /// Класс предназначен для использования в коллекциях для хранения данных одного видеопотока.
    /// Реализует интерфейсы IEquatable и IComparable для использования в упорядоченных коллекциях
    /// </remarks>
    [Serializable]
    public class SingleStreamFrameIndex : 
        IEquatable<SingleStreamFrameIndex>, IComparable<SingleStreamFrameIndex>
    {
        #region Fields

        /// <summary>Идентификатор камеры/видеопотока</summary>
        private int _StreamId;

        /// <summary>Метка времени видеокадра (мсек)</summary>
        private int _TimeStamp;

        /// <summary>Смещение блока данных видеокадра в файле с видеоданными (байт)</summary>
        private long _FileOffset;

        #endregion

        #region Methods

        /// <summary>Создает объект с нулевыми значениями свойств</summary>
        /// <param name="stream_id">Идентификатор камеры/видеопотока</param>
        public SingleStreamFrameIndex(int stream_id)
        {
            _StreamId = stream_id;
            _TimeStamp = 0;
            _FileOffset = 0;
        }

        /// <summary>Конструктор копирования</summary>
        /// <param name="other">Объект, с которого создается копия</param>
        public SingleStreamFrameIndex(SingleStreamFrameIndex other)
        {
            _StreamId = other._StreamId;
            _TimeStamp = other._TimeStamp;
            _FileOffset = other._FileOffset;
        }

        /// <summary>Инициализирует объект данными о видеокадре</summary>
        /// <param name="stream_id">Идентификатор камеры/видеопотока</param>
        /// <param name="time">Метка времени видеокадра (мсек)</param>
        /// <param name="offset">Смещение блока данных видеокадра в файле с видеоданными (байт)</param>
        public SingleStreamFrameIndex(int stream_id, int time, long offset)
        {
            _StreamId = stream_id;
            _TimeStamp = time;
            _FileOffset = offset;
        }

        /// <summary>Копирует значения свойств (кроме StreamId) другого объекта</summary>
        /// <param name="other">Объект, свойства которого копируются</param>
        public virtual void Copy(SingleStreamFrameIndex other)
        {
            _TimeStamp = other._TimeStamp;
            _FileOffset = other._FileOffset;
        }

        /// <summary>Изменяет значение свойства StreamId</summary>
        /// <param name="stream_id">Новое значение идентификатора камеры/видеопотока</param>
        protected void SetStreamId(int stream_id)
        {
            _StreamId = stream_id;
        }

        /// <summary>Суммарный размер полей данных класса (байт)</summary>
        public virtual int SIZE()
        {
            return
                sizeof(int) +   // StreamId
                sizeof(int) +   // TimeStamp
                sizeof(long);   // FileOffset
        }

        /// <summary>Проверяет выполнение условия - значение свойства TimeStamp текущего объекта 
        /// должно быть больше или равно, чем у заданного параметром obj
        /// </summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>Возвращает TRUE, если условие выполняется, иначе - FALSE</returns>
        public bool TimeStampGreaterOrEqualThen(SingleStreamFrameIndex obj)
        {
            return _TimeStamp >= obj._TimeStamp;
        }

        /// <summary>Проверяет выполнение условия - значение свойства TimeStamp текущего объекта 
        /// должно быть больше, чем у заданного параметром obj
        /// </summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>Возвращает TRUE, если условие выполняется, иначе - FALSE</returns>
        public bool TimeStampGreaterThen(SingleStreamFrameIndex obj)
        {
            return _TimeStamp > obj._TimeStamp;
        }

        /// <summary>Проверяет выполнение условия - значение свойства TimeStamp текущего объекта
        /// должно быть меньше или равно, чем у заданного параметром obj
        /// </summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>Возвращает TRUE, если условие выполняется, иначе - FALSE</returns>
        public bool TimeStampLessOrEqualThen(SingleStreamFrameIndex obj)
        {
            return _TimeStamp <= obj._TimeStamp;
        }

        /// <summary>Проверяет выполнение условия - значение свойства TimeStamp текущего объекта
        /// должно быть меньше, чем у заданного параметром obj
        /// </summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>Возвращает TRUE, если условие выполняется, иначе - FALSE</returns>
        public bool TimeStampLessThen(SingleStreamFrameIndex obj)
        {
            return _TimeStamp < obj._TimeStamp;
        }

        #endregion

        #region Properties

        /// <summary>Идентификатор камеры/видеопотока</summary>
        public int StreamId
        {
            get { return _StreamId; }
        }

        /// <summary>Метка времени видеокадра (мсек)</summary>
        public int TimeStamp
        {
            get { return _TimeStamp; }
            set { _TimeStamp = value; }
        }

        /// <summary>Смещение блока данных видеокадра в файле с видеоданными (байт)</summary>
        public long FileOffset
        {
            get { return _FileOffset; }
            set { _FileOffset = value; }
        }

        #endregion

        #region IEquatable<SingleStreamFrameIndex> Members

        /// <summary>Метод сравнения объектов на равенство значений</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>
        /// Возвращает TRUE, если объекты равны, в противном случае возвращает FALSE
        /// </returns>
        public bool Equals(SingleStreamFrameIndex other)
        {
            return
                (_StreamId == other._StreamId) &&
                (_TimeStamp == other._TimeStamp) &&
                (_FileOffset == other._FileOffset);
        }

        #endregion

        #region IComparable<SingleStreamFrameIndex> Members

        /// <summary>Метод ставнения объектов данного типа</summary>
        /// <param name="other">Объект, с тоторым происходит сравнение</param>
        /// <returns>
        /// Возвращает значение разници меток времени двух объектов:
        /// 
        /// меньше 0 = данный объект меньше объекта, заданного параметром other
        /// 0        = объекты равны
        /// больше 0 = данный объект больше объекта, заданного параметром other
        /// </returns>
        public int CompareTo(SingleStreamFrameIndex other)
        {
            return _TimeStamp.CompareTo(other._TimeStamp);
        }

        #endregion
    }
}
