using System;
using System.IO;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Интерфейс потока индексов видеоданных</summary>
    public interface IVideoIndexesStream
    {
        #region Methods

        /// <summary>Прочитать индексы из потока</summary>
        /// <param name="indexes">Буфер для размещения прочитанных индексов</param>
        /// <param name="offset">Смещение от начала буфера, по которому будут записаны прочитанные из потока индексы</param>
        /// <param name="count">Количество читаемых индексов, шт</param>
        /// <param name="bytes_readed">Количество прочитанных байт данных из потока</param>
        /// <returns>Прочитанное количество индексов из потока, шт</returns>
        int Read(SingleStreamFrameIndex[] indexes, int offset, int count, ref int bytes_readed);

        /// <summary>Читать очередное значение индекса из потока</summary>
        /// <param name="index">Ссылка на объект, принимающий данные</param>
        /// <returns>
        /// Возвращает количество прочитанных из потока байтов, если индекс был прочитан успешно.
        /// В случае достижения конца потока возвращает 0
        /// </returns>
        int ReadIndex(out SingleStreamFrameIndex index);

        /// <summary>Переместить указатель чтения/записи в потоке на заданное количество индексов</summary>
        /// <param name="offset">Смещение от позиции, заданной параметром origin</param>
        /// <param name="origin">Граница, от которой проводить смещение курсора</param>
        /// <returns>Измененная позиция курсора чтения/записи в потоке</returns>
        long Seek(long offset, SeekOrigin origin);

        /// <summary>Задать длину потока в индексах</summary>
        /// <param name="value">Новая длина потока в индексах</param>
        void SetLength(long value);

        /// <summary>Записать массив индексов видеоданных в топок</summary>
        /// <param name="indexes">Ссылка на массив, содержащий данные видеоиндексов</param>
        /// <param name="offset">
        /// Индекс первого элемента в массиве, начиная с которого видеоиндексы будут читаться из массива
        /// и записываться в поток
        /// </param>
        /// <param name="count">Количество видеоиндексов, которое нужно записать в потока</param>
        /// <param name="bytes_wrote">По окончании процесса записи этот параметр будет содержать количество записанных байтов в поток</param>
        /// <returns>Возвращает количество записанных в поток индексов</returns>
        int Write(SingleStreamFrameIndex[] indexes, int offset, int count, ref int bytes_wrote);

        /// <summary>Записать индекс видеоданных в топок</summary>
        /// <param name="index">Ссылка на объект, содержащий данные индекса</param>
        /// <returns>Возвращает количество записанных в поток байтов</returns>
        int WriteIndex(SingleStreamFrameIndex index);

        /// <summary>Принудительный сброс кэша данных в поток</summary>
        void Flush();

        /// <summary>Закрывает поток и освобождает занимаемые им ресурсы</summary>
        void Close();

        #endregion

        #region Properties

        /// <summary>Отражает возможность чтения данных из потока</summary>
        bool CanRead { get; }

        /// <summary>Отражает возможность изменения позиции курсора записи/чтения внутри потока</summary>
        bool CanSeek { get; }

        /// <summary>Отражает возможность записи данных в поток</summary>
        bool CanWrite { get; }

        /// <summary>Длина потока данных без учета длины заголовка, байт</summary>
        long Length { get; }

        /// <summary>Признак внесения изменений в поток. Отражает факт записи данных в поток</summary>
        bool Modified { get; }

        /// <summary>Позиция курсора чтения/записи в потоке</summary>
        long Position { get; set; }

        /// <summary>Размер заголовка потока в байтах</summary>
        int HeaderSize { get; }

        #endregion
    }
}
