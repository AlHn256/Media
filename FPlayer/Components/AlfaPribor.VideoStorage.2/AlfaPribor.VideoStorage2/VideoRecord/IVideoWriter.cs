using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Интерфейс для записи видеоданных в хранилище</summary>
    public interface IVideoWriter : IVideoInterface
    {
        /// <summary>Дата и время команды начала записи видеопотоков</summary>
        DateTime RecordStarted { get; set; }

        /// <summary>Дата и время команды окончания записи видеопотоков</summary>
        DateTime RecordFinished { get; set; }

        /// <summary>Записать кадр</summary>
        /// <param name="camera_id">Номер камеры (видеопотока)</param>
        /// <param name="time_stamp">Время от начала записи в миллисекундах</param>
        /// <param name="content_type">Тип содержимого кадра, например: image/jpeg или image/raw</param>
        /// <param name="frame_data">Бинарные данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult WriteFrame(int camera_id, int time_stamp, string content_type, byte[] frame_data);

        /// <summary>Записать кадр</summary>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult WriteFrame(VideoFrame frame);

        /// <summary>Аварийное закрытие записи</summary>
        /// <returns>Результат выполнения операции</returns>
        /// <remarks>При аварийном закрытии видеозаписи целостность данных в ней не гарантирутся,
        /// т.к. изменения в индексах и в информации о видеопотоках не сохраняются
        /// </remarks>
        VideoStorageResult Abort();

    }
}
