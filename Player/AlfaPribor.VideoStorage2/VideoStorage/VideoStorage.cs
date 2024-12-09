using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Хранилище видеоданных</summary>
    public class VideoStorage : BaseVideoStorage
    {

        /// <summary>Конструктор объектов класса</summary>
        public VideoStorage() { }

        /// <summary>Создать объект для работы с видеозаписью хранилища</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition">Раздел хранилища, на котором расположена видеозапись</param>
        /// <param name="mode">Режим открытия видеозаписи</param>
        /// <exception cref="System.ArgumentException">
        /// Идентификатор видеозаписи имеет недопустимое значение
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Не указан раздел хранилища, к которому относится видеозапись
        /// </exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Возникла ошибка при создании или чтении потоков ввода/вывода
        /// </exception>
        /// <returns>объект для работы с видеозаписью</returns>
        protected override BaseVideoRecord NewVideoRecord(string id, VideoPartition partition, VideoRecordOpenMode mode)
        {
            if (!EnableBufferingVideo) return new VideoRecord(id, partition, mode);
            return new BufferedVideoRecord(id, partition, mode);
        }

        /// <summary>Создать объект для работы с видеозаписью хранилища</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="stat">Статус объекта, работающего с видеозаписью</param>
        /// <returns>Объект для работы с видеозаписью</returns>
        protected override BaseVideoRecord NewVideoRecord(string id, VideoStorageIntStat stat)
        {
            if (!EnableBufferingVideo) return new VideoRecord(id, stat);
            return new BufferedVideoRecord(id, stat);
        }

        /// <summary>Создать объект для работы с видеозаписью хранилища</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздел хранилища, на котором расположена видеозапись</param>
        /// <param name="stat">Статус объекта, работающего с видеозаписью</param>
        /// <returns>Объект для работы с видеозаписью</returns>
        protected override BaseVideoRecord NewVideoRecord(string id, int partition_id, VideoStorageIntStat stat)
        {
            if (!EnableBufferingVideo) return new VideoRecord(id, partition_id, stat);
            return new BufferedVideoRecord(id, partition_id, stat);
        }
    }
}
