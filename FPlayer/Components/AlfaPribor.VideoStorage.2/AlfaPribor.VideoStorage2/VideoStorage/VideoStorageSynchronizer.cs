using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using AlfaPribor.Threads;


namespace AlfaPribor.VideoStorage2
{
    /// <summary>Класс, синхронизирующий содержимое выбранных разделов хранилища с заданным списком видеозаписей</summary>
    class VideoStorageSynchronizer : TerminatedThread
    {
        #region Fields

        /// <summary>Список разделов хранилища, которые нужно синхронизировать</summary>
        protected List<VideoPartition> _Partitions;

        /// <summary>Список идентификаторов целевых видеозаписей</summary>
        protected List<string> _Ids;

        /// <summary>Признак ожидания видеозаписей, удаление которых было запрещено</summary>
        protected bool _WaitCanceledRecords;

        /// <summary>Количество удаленных видеозаписей в процессе синхронизации</summary>
        protected int _ErasedCount;

        /// <summary>Тип хранилища видеоданных</summary>
        private VideoStorageType _StorageType;

        #endregion

        #region Methods
        
        /// <summary>Конструктор класса</summary>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <param name="partitions">Список разделов для синхронизации</param>
        /// <param name="ids">
        /// Список идентификаторов видеозаписей, которые должны остаться в хранилище после синхронизации
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Недопустимое значение параметра (параметр равен значению null)
        /// </exception>
        public VideoStorageSynchronizer(VideoStorageType storage_type, IList<VideoPartition> partitions, IList<string> ids)
            : base(ThreadPriority.BelowNormal)
        {
            _StorageType = storage_type;
            Partitions = partitions;
            Ids = ids;
            _WaitCanceledRecords = true;
            _ErasedCount = 0;
        }

        /// <summary>Конструктор класса</summary>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        public VideoStorageSynchronizer(VideoStorageType storage_type)
            : this(storage_type, new List<VideoPartition>(), new List<string>())
        {
        }

        /// <summary>Генерирует событие с запросом об удалении видеозаписи</summary>
        /// <param name="id">Идентификатор удаляемой видеозаписи</param>
        /// <param name="cancel">Реакция на удаление видеозаписи. Если равно TRUE - требуется отменить удаление.</param>
        /// <remarks>Метод не перехватывает исключения, произошедшие в обработчике события</remarks>
        protected void RaiseOnDeleting(string id, ref bool cancel)
        {
            if (OnDeleting != null)
            {
                CircularBufferCancelEventArgs args = new CircularBufferCancelEventArgs(id);
                args.Cancel = false;
                OnDeleting(this, args);
                cancel = args.Cancel;
            }
        }

        /// <summary>Синхронизирует заданные разделы хранилище видеоданных</summary>
        protected override void DoExecute()
        {
            // Список разделов, которые требуется синхронизировать
            List<VideoPartition> check_partitions;
            lock (SyncRoot)
            {
                check_partitions = new List<VideoPartition>(_Partitions);
            }
            // Проверяем пока есть, что проверять...
            while (check_partitions.Count > 0)
            {
                for (int i = 0; i < check_partitions.Count; ++i)
                {
                    VideoPartition partition = check_partitions[i];
                    DirectoryInfo dir = new DirectoryInfo(partition.Path);
                    try
                    {
                        IList<string> ids = VideoRecord.GetRecords(dir, _StorageType);
                        bool HasUsedRecords = false;
                        foreach (string id in ids)
                        {
                            bool need_delete = false;
                            bool is_used = false;
                            lock (SyncRoot)
                            {
                                need_delete = _Ids.BinarySearch(id) < 0;
                            }
                            try
                            {
                                RaiseOnDeleting(id, ref is_used);
                                if (is_used)
                                {
                                    HasUsedRecords = true;
                                }
                            }
                            catch { }
                            try
                            {
                                if (need_delete && !is_used)
                                {
                                    VideoRecord.Delete(dir, id, _StorageType);
                                    lock (SyncRoot)
                                    {
                                        ++_ErasedCount;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                try
                                {
                                    RaiseOnException(e);
                                }
                                catch { }
                            }
                            if (Terminated)
                            {
                                return;
                            }
                        }
                        lock (SyncRoot)
                        {
                            if (!_WaitCanceledRecords)
                            {
                                check_partitions.Remove(partition);
                                --i;
                            }
                            else if (!HasUsedRecords)
                            {
                                check_partitions.Remove(partition);
                                --i;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            RaiseOnException(e);
                        }
                        catch { }
                    }
                    if (Terminated)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>Список разделов хранилища, содержимое которых синхронизируется</summary>
        /// <exception cref="System.ArgumentNullException">
        /// Недопустимое значение свойства (попытка присвоить значение null)
        /// </exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Невозможно изменить список разделов хранилища, если синхронизация уже выполняется
        /// </exception>
        public IList<VideoPartition> Partitions
        {
            get { return new List<VideoPartition>(_Partitions); }
            set
            {
                if (Thread.IsAlive)
                {
                    throw new VideoStorageException("Synchronizer in process! Stop processing before change.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _Partitions = new List<VideoPartition>(value.Count);
                // Из представленного списка разделов выбираем только действующие, при этом проверяем существование
                // пути
                foreach (VideoPartition partition in value)
                {
                    if (partition == null)
                    {
                        continue;
                    }
                    if (partition.Status != VideoPartitionState.Ok)
                    {
                        continue;
                    }
                    if (!Directory.Exists(partition.Path))
                    {
                        continue;
                    }
                    _Partitions.Add(partition);
                }
            }
        }

        /// <summary>Список идентификаторов целевых видеозаписей</summary>
        /// <exception cref="System.ArgumentNullException">
        /// Недопустимое значение свойства (попытка присвоить значение null)
        /// </exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Невозможно изменить список идентификаторов записей, если синхронизация уже выполняется
        /// </exception>
        public IList<string> Ids
        {
            get { return new List<string>(_Ids); }
            set
            {
                if (Thread.IsAlive)
                {
                    throw new VideoStorageException("Synchronizer in process! Stop processing before change.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _Ids = new List<string>(value);
                _Ids.Sort();
            }
        }

        /// <summary>Признак ожидания записей, удаление которых было отменено в обработчике события OnDeleting</summary>
        public bool WaitCanceledRecords
        {
            get { return _WaitCanceledRecords; }
            set { _WaitCanceledRecords = value; }
        }

        /// <summary>Количество удаленных видеозаписей в процессе синхронизации</summary>
        public int ErasedCount
        {
            get { return _ErasedCount; }
        }

        /// <summary>Тип хранилища видеоданных</summary>
        public VideoStorageType StorageType
        {
            get { return _StorageType; }
        }

        #endregion

        #region Events

        /// <summary>Событие запроса на удаление видеозаписи</summary>
        public event EvCircularBufferDeleting OnDeleting;

        #endregion
    }
}
