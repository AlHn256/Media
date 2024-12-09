using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using AlfaPribor.VideoStorage;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Интерфейс для удаленного доступа к хранилищу видеоданных</summary>
    [ServiceContract]
    public interface IVideoStorageService
    {
        /// <summary>Чтение настроек хранилища видеоданных</summary>
        [OperationContract]
        VideoStorageSettings GetSettings();

        /// <summary>Изменение настроек хранилища видеоданных</summary>
        [OperationContract(IsOneWay = true)]
        void SetSettings(VideoStorageSettings settings);

        /// <summary>Получение URI сервиса чтения видеоданных из хранилища</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        [OperationContract]
        VideoStorageIntResult GetReader(string id);

        /// <summary>Получение URI сервиса чтения видеоданных из хранилища</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <param name="part_id">Идентификатор раздела хранилища</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        [OperationContract(Name="GetReaderByPartition")]
        VideoStorageIntResult GetReader(string id, int part_id);

        /// <summary>Получение URI сервиса записи видеоданных в хранилище</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        [OperationContract]
        VideoStorageIntResult GetWriter(string id);

        /// <summary>Получение URI сервиса перезаписи видеоданных в хранилище</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        /// <remarks>
        /// При открытии ищется видеозапись с указанным идентификатором
        /// Запись ведется во временный файл
        /// При закрытии существующая запись заменяется на содержимое временного файла
        /// (через удаление и переименование)
        /// </remarks>
        [OperationContract]
        VideoStorageIntResult GetRewriter(string id);

        /// <summary>Получение URI сервиса чтения индексов видеоданных</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        [OperationContract]
        VideoStorageIntResult GetVideoIndex(string id);

        /// <summary>Удаление видеоданных из хранилища</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract]
        VideoStorageResult Delete(string id);

        /// <summary>Удаление всех видеоданных из хранилища</summary>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract]
        VideoStorageResult DeleteAll();
    }
}
