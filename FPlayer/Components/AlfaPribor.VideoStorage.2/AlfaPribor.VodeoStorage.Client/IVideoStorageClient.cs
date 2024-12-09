using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Client
{
    /// <summary>Интерфейс клиента хранилища видеоданных</summary>
    public interface IVideoStorageClient : IVideoStorageCommunicationObject
    {
        /// <summary>Интерфейс хранилища видеоданных, к которому предоставляется удаленный доступ</summary>
        IVideoStorageService VideoStorageService { get; }

        /// <summary>Относительный сетевой идентификатор хранилища видеоданных</summary>
        string RelativeUri { get; set; }
    }
}
