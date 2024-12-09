using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{

    /// <summary>Хранилище видеоданных, предназначенное для работы с видеозаписями ВидеоИнспектор-2008/2010</summary>
    public class AskoVideoStorage : BaseVideoStorage
    {

        /// <summary>Конструктор объектов класса</summary>
        public AskoVideoStorage() { }

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
            return new AskoVideoRecord(id, partition, mode);
        }

        /// <summary>Создать объект для работы с видеозаписью хранилища</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="stat">Статус объекта, работающего с видеозаписью</param>
        /// <returns>Объект для работы с видеозаписью</returns>
        protected override BaseVideoRecord NewVideoRecord(string id, VideoStorageIntStat stat)
        {
            return new AskoVideoRecord(id, stat);
        }

        /// <summary>Создать объект для работы с видеозаписью хранилища</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздел хранилища, на котором расположена видеозапись</param>
        /// <param name="stat">Статус объекта, работающего с видеозаписью</param>
        /// <returns>Объект для работы с видеозаписью</returns>
        protected override BaseVideoRecord NewVideoRecord(string id, int partition_id, VideoStorageIntStat stat)
        {
            return new AskoVideoRecord(id, partition_id, stat);
        }

    }
}
