using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoExport
{
    /// <summary>Базовое исключение классов, осуществляющих экспорт изображений в потоковое видео</summary>
    public class VideoExportException : Exception
    {
        /// <summary>Конструктор класса</summary>
        public VideoExportException() { }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса сообщением об исключительной ситуации.
        /// </summary>
        /// <param name="message">Тексторое сообщение о возникшей исключительной ситуации</param>
        public VideoExportException(string message)
            : base(message) { }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса данными о возникшей исключительной ситуации
        /// </summary>
        /// <param name="message">Описание исключительной ситуации</param>
        /// <param name="innerException">Ссылка на объект класса возникшего исключения</param>
        public VideoExportException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
