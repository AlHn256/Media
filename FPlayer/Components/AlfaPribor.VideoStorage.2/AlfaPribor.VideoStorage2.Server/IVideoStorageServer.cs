using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.VideoStorage;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Интерфейс сервера хранилища видеоданных</summary>
    public interface IVideoStorageServer : IVideoStorageCommunicationObject
    {
        /// <summary>Интерфейс хранилища видеоданных, к которому предоставляется удаленный доступ</summary>
        IVideoStorage VideoStorage { get; }

        /// <summary>Универсальный сетевой локатор хранилища видеоданных</summary>
        string Url { get; }

        /// <summary>Относительный идентификатор хранилища в сети</summary>
        string RelativeUri { get; set; }
    }
}
