using System;
using AlfaPribor.Streams2;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Базовый класс всех исключительных ситуаций, возникающий в процессе работы хранилища видеоданных</summary>
    public class VideoStorageException : Exception
    {
        /// <summary>Конструктор класса</summary>
        public VideoStorageException() { }

        /// <summary>Конструктор класса</summary>
        /// <param name="message">Сообщение, описывающее исключительную ситуацию</param>
        public VideoStorageException(string message) : 
            base(message) { }

        /// <summary>Конструктор класса</summary>
        /// <param name="message">Сообщение, описывающее исключительную ситуацию</param>
        /// <param name="except">Внутреннее исключение, вызвавшее возникновение этой исключительной ситуации</param>
        public VideoStorageException(string message, Exception except)
            : base(message, except) { }
    }

    /// <summary>Исключение, возникающее при попытке чтения данных из потока, имеющего поврежденный заголовок
    /// или содержащего данные несовместимой версии
    /// </summary>
    public class InvalidStreamHeaderException : VideoStorageException
    {
        #region Fields

        /// <summary>Состояние заголовка потока</summary>
        private StreamHeaderState _StreamHeaderState;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <param name="state">Состояние заголовка потока</param>
        public InvalidStreamHeaderException(StreamHeaderState state) :
            this("Invalid stream header. Status: " + state.ToString(), state) { }

        /// <summary>Конструктор класса</summary>
        /// <param name="message">Сообщение, описывающее исключительную ситуацию</param>
        /// <param name="state">Состояние заголовка потока</param>
        public InvalidStreamHeaderException(string message, StreamHeaderState state) :
            base(message)
        {
            _StreamHeaderState = state;
        }

        /// <summary>Конструктор класса</summary>
        /// <param name="message">Сообщение, описывающее исключительную ситуацию</param>
        /// <param name="state">Состояние заголовка потока</param>
        /// <param name="except">Внутреннее исключение, породившее эту исключительную ситуацию</param>
        public InvalidStreamHeaderException(string message, StreamHeaderState state, Exception except) :
            base(message, except)
        {
            _StreamHeaderState = state;
        }

        #endregion

        #region Properties

        /// <summary>Состояние заголовка потока</summary>
        public StreamHeaderState StreamHeaderState
        {
            get { return _StreamHeaderState; }
        }

        #endregion
    }

    /// <summary>Исключение, возникающее в результате чтения элемента данных потока, содержащего ошибку</summary>
    public class InvalidStreamDataException : VideoStorageException
    {
        /// <summary>Конструктор класса</summary>
        public InvalidStreamDataException() :
            base("Data contain error.") { }

        /// <summary>Конструктор класса</summary>
        /// <param name="message">Сообщение, описывающее исключительную ситуацию</param>
        public InvalidStreamDataException(string message) : 
            base(message) { }

        /// <summary>Конструктор класса</summary>
        /// <param name="message">Сообщение, описывающее исключительную ситуацию</param>
        /// <param name="except">Внутреннее исключение, вызвавшее возникновение этой исключительной ситуации</param>
        public InvalidStreamDataException(string message, Exception except)
            : base(message, except) { }
    }
}
