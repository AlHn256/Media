using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Базовый интерфейс классов, реализующих операции чтения/записи видеоданных из/в хранилище</summary>
    public interface IVideoInterface
    {
        /// <summary>Идентификатор поезда или видеоданных</summary>
        string Id { get; }

        /// <summary>Идентификатор раздела хранилища</summary>
        int PartitionId { get; }

        /// <summary>Статус интерфейса</summary>
        VideoStorageIntStat Status { get; }

        /// <summary>Завершить чтение/запись (закрыть поток)</summary>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult Close();
    }
}
