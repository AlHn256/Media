using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Интерфейс для чтения видеоданных из хранилища</summary>
    [ServiceContract]
    public interface IVideoReaderService : IVideoInterfaceService
    {
        /// <summary>Прочитать кадр указанной камеры, соответствующий указанному моменту времени</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="frame_info">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract]
        VideoStorageResult ReadFrame(int cam_id, int time_stamp, out VideoFrameInfo frame_info);

        /// <summary>Прочитать кадр указанной камеры, соответствующий указанному моменту времени</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="delta_time">Интервал времени относительно time_stamp, в пределах которого будет искаться видеокадр</param>
        /// <param name="frame_info">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract(Name = "ReadFrameByDeltaTime")]
        VideoStorageResult ReadFrame(int cam_id, int time_stamp, int delta_time, out VideoFrameInfo frame_info);

        /// <summary>Прочитать первый кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract]
        VideoStorageResult ReadFirstFrame(int cam_id, out VideoFrameInfo frame);

        /// <summary>Прочитать последний кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract]
        VideoStorageResult ReadLastFrame(int cam_id, out VideoFrameInfo frame);

        /// <summary>Прочитать следующий кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="step">Инкремент номера кадра</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract]
        VideoStorageResult ReadNextFrame(int cam_id, int step, out VideoFrameInfo frame);

        /// <summary>Прочитать предыдущий кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="step">Инкремент номера кадра</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        [OperationContract]
        VideoStorageResult ReadPrevFrame(int cam_id, int step, out VideoFrameInfo frame);

        /// <summary>Прочитать видеоданные последнего запрошенного кадра</summary>
        /// <returns>Видеоданные кадра</returns>
        [OperationContract]
        Stream ReadFrameData();

        /// <summary>Интерфейс чтения видеоданных из хранилища</summary>
        IVideoReader VideoReader { get; }
    }
}
