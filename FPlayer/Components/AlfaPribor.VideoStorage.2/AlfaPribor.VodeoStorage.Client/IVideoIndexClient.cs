using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Client
{
    /// <summary>Интерфейс классов, позволяющих читать индексы видеоданных из удаленного хранилища</summary>
    public interface IVideoIndexClient : IVideoStorageCommunicationObject
    {
        /// <summary>Получить интерфейс чтения индексов видеоданных по относительному сетевому идентификатору</summary>
        /// <param name="relative_uri">Относительный сетевой идентификатор интерфейса чтения индексов видеоданных</param>
        /// <returns>Интерфейс чтения данных</returns>
        IVideoIndexService this[string relative_uri] { get; }
    }
}
