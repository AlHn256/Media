using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Интерфейс для записи видеоданных в хранилище</summary>
    [ServiceContract]
    public interface IVideoWriterService : IVideoInterfaceService
    {
        /// <summary>Подготавливает кадр к записи</summary>
        /// <param name="frame_info">Данные кадра</param>
        [OperationContract(IsOneWay= true)]
        void WriteFrame(VideoFrameInfo frame_info);

        /// <summary>Подготавливает кадр к записи</summary>
        /// <param name="camera_id">Номер камеры (видеопотока)</param>
        /// <param name="time_stamp">Время от начала записи в миллисекундах</param>
        /// <param name="content_type">Тип содержимого кадра, например: image/jpeg или image/raw</param>
        [OperationContract(IsOneWay = true)]
        void WriteFrame(int camera_id, int time_stamp, string content_type);

        /// <summary>Записать последний подготовленный к записи кадр</summary>
        /// <param name="data">Видеоданные кадра</param>
        /// <returns>Результат выполения операции</returns>
        [OperationContract]
        VideoStorageResult WriteFrameData(Stream data);

        /// <summary>Получить дату/время команды начала записи видеопотоков</summary>
        [OperationContract]
        DateTime GetRecordStarted();

        /// <summary>Задать дату/время команды начала записи видеопотоков</summary>
        /// <param name="value">Новое значение даты/времени начала записи</param>
        [OperationContract(IsOneWay = true)]
        void SetRecordStarted(DateTime value);

        /// <summary>Получить дату/время команды окончания записи видеопотоков</summary>
        [OperationContract]
        DateTime GetRecordFinished();

        /// <summary>Задать дату/время команды окончания записи видеопотоков</summary>
        /// <param name="value">Новое значение даты/времени окончания записи</param>
        [OperationContract(IsOneWay = true)]
        void SetRecordFinished(DateTime value);
    }
}
