using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.AviFile
{
    /// <summary>Базовое исключение классов, работающих с AVI-файлами</summary>
    public class AviFileException : Exception
    {
        /// <summary>Конструктор класса</summary>
        public AviFileException() { }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса сообщением об исключительной ситуации.
        /// </summary>
        /// <param name="message">Тексторое сообщение о возникшей исключительной ситуации</param>
        public AviFileException(string message)
            : base(message) { }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса данными о возникшей исключительной ситуации
        /// </summary>
        /// <param name="message">Описание исключительной ситуации</param>
        /// <param name="innerException">Ссылка на объект класса возникшего исключения</param>
        public AviFileException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>Класс исключения, возникающего в случае неизвестного формата у открываемого AVI-файла</summary>
    public class BadAviFormatException : AviFileException
    {
        /// <summary>Конструктор класса</summary>
        public BadAviFormatException()
            : base("Unknown format of AVI-file!") { }

        /// <summary>Конструктор класса.
        /// Инициализирует экземпляр класса сообщением о возникшем исключении
        /// </summary>
        /// <param name="message">Текстовое сообщение с описанием возникшей исключительной ситуации</param>
        public BadAviFormatException(string message)
            : base(message) { }
    }

    /// <summary>Класс исключения, возникающего в случае неизвестного типа требуемого кодека данных</summary>
    public class CodecException : AviFileException
    {
        /// <summary>Конструктор класса.
        /// Инициализирует экземпляр класса сообщением о возникшем исключении
        /// </summary>
        /// <param name="message">Текстовое сообщение с описанием возникшей исключительной ситуации</param>
        public CodecException(string message)
            : base(message) { }
    }

    /// <summary>Класс исключения, возникающего в случае неизвестного типа требуемого кодека при сжатии данных</summary>
    public class CompressorException : CodecException
    {
        /// <summary>Конструктор класса.
        /// Инициализирует экземпляр класса сообщением о возникшем исключении
        /// </summary>
        /// <param name="message">Текстовое сообщение с описанием возникшей исключительной ситуации</param>
        public CompressorException(string message)
            : base(message) { }
    }

    /// <summary>Класс исключения, возникающего в случае неизвестного типа требуемого кодека при расжатии данных</summary>
    public class DecompressorException : CodecException
    {
        /// <summary>Конструктор класса.
        /// Инициализирует экземпляр класса сообщением о возникшем исключении
        /// </summary>
        /// <param name="message">Текстовое сообщение с описанием возникшей исключительной ситуации</param>
        public DecompressorException(string message)
            : base(message) { }
    }

    /// <summary>Класс исключения, используемого для отмены дальнейших действий сторонним кодом</summary>
    public class AbortException : AviFileException
    {
    }

    /// <summary>Класс исключения, возникающего при попытке чтения данных в буфер, размер которого недостаточен 
    /// для размещения хотябы одного сэмпла данных
    /// </summary>
    public class BufferTooSmallException : AviFileException
    {
        /// <summary>Конструктор класса</summary>
        public BufferTooSmallException()
            : base("Buffer too small for read single sample!") { }

        /// <summary>Конструктор класса.
        /// Инициализирует экземпляр класса сообщением о возникшем исключении
        /// </summary>
        /// <param name="message">Текстовое сообщение с описанием возникшей исключительной ситуации</param>
        public BufferTooSmallException(string message)
            : base(message) { }
    }
}
