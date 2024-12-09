using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Client
{
    /// <summary>Интерфейс классов, позволяющих читать данные из удаленного хранилища</summary>
    public interface IVideoReaderClient : IVideoStorageCommunicationObject
    {
        /// <summary>Получить интерфейс чтения видеоданных по относительному сетевому идентификатору</summary>
        /// <param name="relative_uri">Относительный сетевой идентификатор интерфейса чтения данных</param>
        /// <returns>Интерфейс чтения данных</returns>
        IVideoReaderService this[string relative_uri] { get; }
    }
}
