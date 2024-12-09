using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Базовый интерфейс сервисов, предоставляющих операции чтения/записи видеоданных из/в хранилище</summary>
    [ServiceContract]
    public interface IVideoInterfaceService
    {
        /// <summary>Идентификатор поезда или видеоданных</summary>
        [OperationContract]
        string GetId();

        /// <summary>Идентификатор раздела хранилища</summary>
        [OperationContract]
        int GetPartitionId();

        /// <summary>Статус интерфейса</summary>
        [OperationContract]
        VideoStorageIntStat GetStatus();

        /// <summary>Завершить чтение/запись (закрыть поток)</summary>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract]
        VideoStorageResult Close();
    }
}
