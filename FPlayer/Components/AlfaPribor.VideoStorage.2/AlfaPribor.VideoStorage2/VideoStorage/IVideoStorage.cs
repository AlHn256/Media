using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Интерфейс "Хранилище видеоданных"</summary>
    public interface IVideoStorage
    {
        #region Methods

        /// <summary>Получение интерфейса для чтения видеоданных из хранилища</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Интерфейс для чтения из хранилища</returns>
        IVideoReader GetReader(string id);

        /// <summary>Получение интерфейса для чтения видеоданных из хранилища</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <param name="part_id">Идентификатор раздела хранилища</param>
        /// <returns>Интерфейс для чтения из хранилища</returns>
        IVideoReader GetReader(string id, int part_id);

        /// <summary>Получение интерфейса для записи видеоданных в хранилище</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Интерфейс для записи в хранилище</returns>
        IVideoWriter GetWriter(string id);

        /// <summary>Получение интерфейса для перезаписи видеоданных в хранилище</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Интерфейс для перезаписи</returns>
        /// <remarks>
        /// При открытии ищется видеозапись с указанным идентификатором
        /// Запись ведется во временный файл
        /// При закрытии существующая запись заменяется на содержимое временного файла
        /// (через удаление и переименование)
        /// </remarks>
        IVideoWriter GetRewriter(string id);

        /// <summary>Удаление видеоданных из хранилища</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult Delete(string id);

        /// <summary>Удаление всех видеоданных из хранилища</summary>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult DeleteAll();

        /// <summary>Синхронизация содержимого хранилища (удаление несвязанных с БД видеозаписей)
        /// Выполняется асинхронно
        /// </summary>
        /// <param name="id_list">Список идентификаторов поездов / видеозаписей (из БД)</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult Synchronize(IList<string> id_list);

        /// <summary>Чтение настроек хранилища видеоданных</summary>
        VideoStorageSettings GetSettings();

        /// <summary>Изменение настроек хранилища видеоданных</summary>
        void SetSettings(VideoStorageSettings settings);

        /// <summary>Получить временный идентификатор записи</summary>
        /// <param name="original_id">Исходный идентификатор записи</param>
        /// <returns>Временный идентификатор видеозаписи</returns>
        string GetTempId(string original_id);

        /// <summary>Возвращает исходный идентификатор видеозаписи, основываясь на ее временном идентификаторе</summary>
        /// <param name="temp_id">Временный идентификатор видеозаписи</param>
        /// <returns>Исходный идентификатор видеозаписи</returns>
        string GetOriginalId(string temp_id);

        #endregion

        #region Properties

        /// <summary>Признак активности хранилища</summary>
        bool Active { get; set; }

        /// <summary>Интервал проверки кольцевого буфера в секундах</summary>
        int CircularBufferCheckInterval { get; set; }

        /// <summary>Информация о хранилище видеоданных</summary>
        VideoStorageInfo Info { get; }

        /// <summary>Признак выполнения синхронизации хранилища</summary>
        bool Synchronizing { get; }

        #endregion

        #region Events

        /// <summary>Событие "Запрос на удаление записи из кольцевого буфера"</summary>
        event CancelEventHandler OnCircularBufferDeleting;

        /// <summary>Событие "Удалена запись из хранилища видеоданных"</summary>
        event EvCircularBufferDeleted OnCircularBufferDeleted;

        /// <summary>Событие "Закончена синхронизация хранилища"</summary>
        event EvSyncComplete OnSyncComplete;

        #endregion
    }
}
