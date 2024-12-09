using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.AviFile
{
    /// <summary>Содержит дополнительные сведения о событии "Экспорт видеокадра"</summary>
    public class ExportFrameEventArgs : EventArgs
    {
        /// <summary>Порядковый номер экспортируемого кадра</summary>
        private int _FrameNumber;


        /// <summary>Конструктор объектов класса</summary>
        /// <param name="number">Порядковый номер экспортируемого кадра</param>
        public ExportFrameEventArgs(int number)
        {
            _FrameNumber = number;
        }

        /// <summary>Порядковый номер экспортируемого кадра</summary>
        public int FrameNumber
        {
            get { return _FrameNumber; }
        }
    }

    /// <summary>Содержит дополнительные сведения о событии "Трансформация видеокадра"</summary>
    public class TransformFrameEventArgs : EventArgs
    {
        /// <summary>Номер видеокадра</summary>
        private int _Number;

        /// <summary>Заголовок данных изображения</summary>
        private Avi.BITMAPINFOHEADER _Header;

        /// <summary>Данные изображения кадра</summary>
        private byte[] _Data;

        /// <param name="number">Номер видеокадра</param>
        /// <param name="header">Заголовок данных изображения</param>
        /// <param name="data">Данные изображения кадра</param>
        public TransformFrameEventArgs(int number, Avi.BITMAPINFOHEADER header, byte[] data)
        {
            _Number = number;
            _Header = header;
            _Data = data;
        }

        /// <summary>Данные изображения кадра</summary>
        public byte[] Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        /// <summary>Заголовок данных изображения</summary>
        public Avi.BITMAPINFOHEADER Header
        {
            get { return _Header; }
            set { _Header = value; }
        }

        /// <summary>Номер видеокадра</summary>
        public int Number
        {
            get { return _Number; }
        }
    }

    /// <summary>Содержит детальную информацию о видеопотоке</summary>
    public class StreamEventArgs : EventArgs
    {
        /// <summary>Конструктор объектов класса. Инициализирует свойство класса детальной информацией о видеопотоке</summary>
        /// <param name="info">Информация о видеопотоке</param>
        public StreamEventArgs(Avi.AVISTREAMINFO info)
        {
            Info = info;
        }

        /// <summary>Информация о видеопотоке</summary>
        public Avi.AVISTREAMINFO Info { get; set; }
    }
}
