using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Интерфейс хранилища видеоданных, предоставляющий  инструментарий для разделенного использования хранилища разными потоками</summary>
    public interface ISyncVideoStorage : IVideoStorage
    {
        /// <summary>Объект синхронизации доступа к открытым членам класса из разных потоков</summary>
        object SyncRoot { get; }
    }
}
