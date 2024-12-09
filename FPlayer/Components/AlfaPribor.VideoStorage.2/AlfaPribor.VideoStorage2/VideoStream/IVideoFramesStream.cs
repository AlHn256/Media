using System;
using System.IO;
using System.Collections.Generic;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Интерфейс потока видеокадров</summary>
    public interface IVideoFramesStream
    {
        #region Methods

        /// <summary>Прочитать видеокадры из потока</summary>
        /// <param name="frames">Буфер для размещения прочитанных видеокадров</param>
        /// <param name="offset">Смещение от начала буфера, по которому будут записаны прочитанные из потока видеокадры</param>
        /// <param name="count">Количество читаемых видеокадров, шт</param>
        /// <param name="bytes_readed">Количество прочитанных байт данных из потока</param>
        /// <returns>Прочитанное количество видеокадров из потока, шт</returns>
        int Read(VideoFrame[] frames, int offset, int count, ref int bytes_readed);

        /// <summary>Читать очередной видеокадр из потока</summary>
        /// <param name="frame">Ссылка на объект, принимающий данные</param>
        /// <returns>Возвращает количество прочитанных из потока байтов, если индекс был прочитан успешно.
        /// В случае достижения конца потока возвращает 0</returns>
        int ReadFrame(out VideoFrame frame);

        /// <summary>Читать очередной видеокадр из потока</summary>
        /// <param name="frame">Ссылка на объект, принимающий данные</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Возвращает количество прочитанных из потока байтов, если индекс был прочитан успешно.
        /// В случае достижения конца потока возвращает 0</returns>
        int ReadFrame(out VideoFrame frame, out string message);

        /// <summary>Переместить указатель чтения/записи в потоке на заданное количество видеокадров</summary>
        /// <param name="offset">Смещение от позиции, заданной параметром origin</param>
        /// <param name="origin">Граница, от которой проводить смещение курсора</param>
        /// <returns>Измененная позиция курсора чтения/записи в потоке</returns>
        long Seek(long offset, SeekOrigin origin);

        /// <summary>Задать длину потока в видеокадрах</summary>
        /// <param name="value">Новая длина потока в видеокадрах</param>
        void SetLength(long value);

        /// <summary>Записать массив видеокадров в топок</summary>
        /// <param name="frames">Ссылка на массив, содержащий данные видеокадров</param>
        /// <param name="offset">Индекс первого элемента в массиве, начиная с которого видеокадры будут читаться из массива и записываться в поток</param>
        /// <param name="count">Количество видеокадров, которое нужно записать в потока</param>
        /// <param name="bytes_wrote">По окончании процесса записи этот параметр будет содержать количество записанных байтов в поток</param>
        /// <returns>Возвращает количество записанных в поток видеокадров</returns>
        int Write(VideoFrame[] frames, int offset, int count, ref int bytes_wrote);

        /// <summary>Записать видеокадр в топок</summary>
        /// <param name="frame">Ссылка на объект, содержащий данные видеокадра</param>
        /// <returns>Возвращает количество записанных в поток байтов</returns>
        int WriteFrame(VideoFrame frame);

        /// <summary>Принудительный сброс кэша данных в поток</summary>
        void Flush();

        /// <summary>Прочитать подробную информацию о видео, содержащемся в потоке</summary>
        /// <param name="info">Подробная информация о видеокадрах</param>
        /// <returns>TRUE если данные были прочитаны успешно, FALSE - возникла ошибка в процессе чтения данных</returns>
        bool ReadStreamInfo(IList<VideoStreamInfo> info);

        /// <summary>Записать подробную информацию о видео в поток</summary>
        /// <param name="info">Подробная информация о видеокадрах</param>
        /// <returns>TRUE если данные были записаны успешно, FALSE - возникла ошибка в процессе записи данных</returns>
        bool WriteStreamInfo(System.Collections.Generic.IList<VideoStreamInfo> info);

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

        /// <summary>Дата/время начала видеозаписи</summary>
        DateTime RecordStarted { get; set; }

        /// <summary>Дата/время окончания видеозаписи</summary>
        DateTime RecordFinished { get; set; }

        /// <summary>Средний размер видеокадра в байтах</summary>
        int AverageFrameSize { get; set; }
        
        /// <summary>Разделитель видеокадров в потоке</summary>
        byte[] Boundary { get; }

        /// <summary>Размер заголовка потока в байтах</summary>
        int HeaderSize { get; }

        #endregion
    }
}
