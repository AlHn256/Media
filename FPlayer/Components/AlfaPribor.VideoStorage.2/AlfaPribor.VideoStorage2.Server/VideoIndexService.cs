using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Класс, обеспечивающий доступ к хранилищу для чтения индексов видеоданных</summary>
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    class VideoIndexService : VideoServiceBase, IVideoIndexService
    {
        #region Fields

        /// <summary>Интерфейс чтения индексов видеозаписи</summary>
        private IVideoIndex _Index;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <param name="index">Интерфейс чтения индексов видеозаписи</param>
        /// <exception cref="System.ArgumentNullException">Не задан интерфейс чтения индексов видеозаписи</exception>
        public VideoIndexService(IVideoIndex index)
        {
            if (index == null)
            {
                throw new ArgumentNullException();
            }
            _Index = index;
        }

        #endregion

        #region Члены IVideoIndexService

        /// <summary>Получить метку времени первого кадра по всем видеопотокам
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <param name="delta">Время от начала записи, в пределах которого должен находится первый кадр</param>
        /// <returns>Метка времени или минус 1, если не найдено</returns>
        public int GetStartTime(int delta)
        {
            return _Index.GetFinishTime(delta);
        }

        /// <summary>Получить метку времени последнего кадра по всем видеопотокам
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <param name="delta">Допуск на разность меток времени кадров соседних каналов</param>
        /// <returns>Метка времени или минус 1, если не найдено</returns>
        public int GetFinishTime(int delta)
        {
            return _Index.GetFinishTime(delta);
        }

        /// <summary>Получить метку времени следующего кадра (мсек)
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если следующий кадр не найден</returns>
        public int GetNextFrameTime(int cam_id, int current_time)
        {
            return _Index.GetNextFrameTime(cam_id, current_time);
        }

        /// <summary>Получить метку времени предыдущего кадра
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если кадр не найден</returns>
        public int GetPrevFrameTime(int cam_id, int current_time)
        {
            return _Index.GetPrevFrameTime(cam_id, current_time);
        }

        /// <summary>Получить метку времени начала кадра (мсек)
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если кадр не найден</returns>
        public int GetFrameTime(int cam_id, int current_time)
        {
            return _Index.GetFrameTime(cam_id, current_time);
        }

        /// <summary>Получить номер кадра в видеопотоке
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Индекс кадра или минус 1, если кадр не найден</returns>
        public int GetFrameIndex(int cam_id, int current_time)
        {
            return _Index.GetFrameIndex(cam_id, current_time);
        }

        /// <summary>Получить общее количество кадров в видеопотоке
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <returns>Количество кадров в видеопотоке или минус 1,
        /// если видеопоток с заданным идентификатором не существует
        /// </returns>
        public int GetFramesCount(int cam_id)
        {
            return _Index.GetFramesCount(cam_id);
        }

        /// <summary>Получить информацию о видеопотоках
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <returns>Список с данными по каждому присутствующему в записи видеопотоку</returns>
        public IList<VideoStreamInfo> GetStreamInfoList()
        {
            return _Index.StreamInfoList;
        }

        /// <summary>Получить дата/время команды начала записи видеопотоков
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <returns>Дата/время команды начала записи</returns>
        public DateTime GetRecordStarted()
        {
            return _Index.RecordStarted;
        }

        /// <summary>Получить дату/время команды окончания записи видеопотоков
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        /// <returns>Дату/время команды окончания записи</returns>
        public DateTime GetRecordFinished()
        {
            return _Index.RecordFinished;
        }

        /// <summary>Интерфейс чтения индексов видеозаписи из хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoIndexService"/>
        /// </summary>
        public IVideoIndex VideoIndex
        {
            get { return _Index; }
        }

        #endregion

        #region IVideoInterfaceService Members

        /// <summary>Получить идентификатор видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Идентификатор видеозаписи</returns>
        public string GetId()
        {
            return _Index.Id;
        }

        /// <summary>Получить идентификатор раздела хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Идентификатор раздела хранилища</returns>
        public int GetPartitionId()
        {
            return _Index.PartitionId;
        }

        /// <summary>Получить статус интерфейса чтения индексов видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Cтатус интерфейса чтения видеоданных</returns>
        public VideoStorageIntStat GetStatus()
        {
            return _Index.Status;
        }

        /// <summary>Завершить чтение индексов видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoInterfaceService"/>
        /// </summary>
        /// <returns>Cтатус интерфейса чтения видеоданных</returns>
        public VideoStorageResult Close()
        {
            return _Index.Close();
        }

        #endregion
    }
}
