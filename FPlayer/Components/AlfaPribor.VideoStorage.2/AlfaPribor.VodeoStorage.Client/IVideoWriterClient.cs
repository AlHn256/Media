using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Client
{
    /// <summary>Интерфейс классов, позволяющих записывать видеоданные в удаленное хранилище</summary>
    public interface IVideoWriterClient : IVideoStorageCommunicationObject
    {
        /// <summary>Получить интерфейс записи видеоданных по относительному сетевому идентификатору</summary>
        /// <param name="relative_uri">Относительный сетевой идентификатор интерфейса записи данных</param>
        /// <returns>Интерфейс записи данных</returns>
        IVideoWriterService this[string relative_uri] { get; }
    }
}
