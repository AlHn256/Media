using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Интерфейс индекса видеоданных</summary>
    /// <remarks>Предназначен для ускорения работы с видеоданными</remarks>
    [ServiceContract]
    public interface IVideoIndexService : IVideoInterfaceService
    {
        /// <summary>Получить метку времени первого кадра по всем видеопотокам</summary>
        /// <param name="delta">Время от начала записи, в пределах которого должен находится первый кадр</param>
        /// <returns>Метка времени или минус 1, если не найдено</returns>
        /// <remarks>Смещение от команды начала записи видеопотоков (мсек)</remarks>
        [OperationContract]
        int GetStartTime(int delta);

        /// <summary>Получить метку времени последнего кадра по всем видеопотокам</summary>
        /// <param name="delta">Допуск на разность меток времени кадров соседних каналов</param>
        /// <returns>Метка времени или минус 1, если не найдено</returns>
        /// <remarks>Смещение от команды окончания записи видеопотоков (мсек)</remarks>
        [OperationContract]
        int GetFinishTime(int delta);

        /// <summary>Получить метку времени следующего кадра (мсек)</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если следующий кадр не найден</returns>
        /// <remarks>
        /// Число миллисекунд, прошедших от команды начала записи видеопотоков до начала видеоданных кадра
        /// ,следующего за кадром, поступившим ко времени current_time (мсек) после получения команды
        /// начала записи видеопотоков
        /// </remarks>
        [OperationContract]
        int GetNextFrameTime(int cam_id, int current_time);

        /// <summary>Получить метку времени предыдущего кадра</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если кадр не найден</returns>
        /// <remarks>
        /// Число миллисекунд, прошедших от команды начала записи видеопотоков до начала видеоданных кадра
        /// ,идущего перед кадром, поступившим ко времени current_time (мсек) после получения команды
        /// начала записи видеопотоков
        /// </remarks>
        [OperationContract]
        int GetPrevFrameTime(int cam_id, int current_time);

        /// <summary>Получить метку времени начала кадра (мсек)</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если кадр не найден</returns>
        [OperationContract]
        int GetFrameTime(int cam_id, int current_time);

        /// <summary>Получить номер кадра в видеопотоке</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Индекс кадра или минус 1, если кадр не найден</returns>
        [OperationContract]
        int GetFrameIndex(int cam_id, int current_time);

        /// <summary>Получить общее количество кадров в видеопотоке</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <returns>Количество кадров в видеопотоке или минус 1,
        /// если видеопоток с заданным идентификатором не существует
        /// </returns>
        [OperationContract]
        int GetFramesCount(int cam_id);

        /// <summary>Получить информацию о видеопотоках</summary>
        /// <returns>Список с данными по каждому присутствующему в записи видеопотоку</returns>
        [OperationContract]
        IList<VideoStreamInfo> GetStreamInfoList();

        /// <summary>Получить дата/время команды начала записи видеопотоков</summary>
        /// <returns>Дата/время команды начала записи</returns>
        [OperationContract]
        DateTime GetRecordStarted();

        /// <summary>Получить дату/время команды окончания записи видеопотоков</summary>
        /// <returns>Дату/время команды окончания записи</returns>
        [OperationContract]
        DateTime GetRecordFinished();

        /// <summary>Интерфейс чтения индексов видеозаписи из хранилища</summary>
        IVideoIndex VideoIndex { get; }
    }
}
