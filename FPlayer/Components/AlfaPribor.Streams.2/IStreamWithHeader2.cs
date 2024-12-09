using System;
using System.IO;

namespace AlfaPribor.Streams2
{

    /// <summary>Описывает интерфейс потока с заголовком</summary>
    public interface IStreamWithHeader2
    {

        #region Methods

        /// <summary>Проверить заголовок потока</summary>
        /// <returns>Состояние заголовка потока</returns>
        StreamHeaderState CheckHeader();

        /// <summary>Принудительный сброс кэша данных в поток</summary>
        void Flush();

        /// <summary>Закрывает поток и освобождает занимаемые им ресурсы</summary>
        void Close();

        /// <summary>Прочитать данные из потока</summary>
        /// <param name="buffer">Буфер для размещения прочитанных данных</param>
        /// <param name="offset">Смещение от начала буфера, по которому будут записаны прочитанные из потока данные</param>
        /// <param name="count">Количество читаемых байт данных, байт</param>
        /// <returns>Прочитанное количество данных из потока, байт</returns>
        int Read(byte[] buffer, int offset, int count);

        /// <summary>Переместить указатель чтения/записи в потоке на заданное количество байт</summary>
        /// <param name="offset">Смещение от позиции, заданной параметром origin</param>
        /// <param name="origin">Граница, от которой проводить смещение курсора</param>
        /// <returns>Измененная позиция курсора чтения/записи в потоке</returns>
        long Seek(long offset, SeekOrigin origin);

        /// <summary>Задать длину потока в байтах</summary>
        /// <param name="value">Новая длина потока, байт</param>
        void SetLength(long value);

        /// <summary>Записать данные в поток</summary>
        /// <param name="buffer">Буфер, содержащий данные для записи в поток</param>
        /// <param name="offset">Смещение от начала буфера, по которому расположены записываемые данные</param>
        /// <param name="count">Количество записываемых в поток байт данных</param>
        void Write(byte[] buffer, int offset, int count);

        #endregion

        #region Properties

        /// <summary>Сигнатура заголовка потока</summary>
        byte[] Signature { get; }

        /// <summary>Номер версии данных, содержащихся в потоке</summary>
        int Version { get; }

        /// <summary>Отражает возможность чтения данных из потока</summary>
        bool CanRead { get; }

        /// <summary>Отражает возможность изменения позиции курсора записи/чтения внутри потока</summary>
        bool CanSeek { get; }

        /// <summary>Отражает возможность записи данных в поток</summary>
        bool CanWrite { get; }

        /// <summary>Размер заголовка потока, байт</summary>
        int HeaderSize { get; }

        /// <summary>Длина потока данных без учета длины заголовка, байт</summary>
        long Length { get; }

        /// <summary>Признак внесения изменений в поток. Отражает факт записи данных в поток</summary>
        bool Modified { get; }

        /// <summary>Позиция курсора чтения/записи в потоке</summary>
        long Position { get; set; }

        #endregion
    }
}
