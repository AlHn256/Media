using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Обобщенное хранилище видеоданных</summary>
    public abstract class BaseVideoStorage : IVideoStorage
    {

        #region Fields

        /// <summary>Список разделов хранилища</summary>
        List<VideoPartition> _Partitions;

        /// <summary>Используемые в текущий момент времени видеозаписи</summary>
        Dictionary<BaseVideoRecord, string> _UsedRecords;

        /// <summary>Упорядоченный список используемых в текущий момент времени идентификаторов видеозаписей.
        /// Требует синхронизации доступа, т.к. используется разными потоками.
        /// </summary>
        List<string> _UsedRecordIds;

        /// <summary>Признак активности хранилища видеоданных</summary>
        bool _Active;

        /// <summary>Интервал проверки кольцевого буфера в секундах</summary>
        int _CircularBufferCheckInterval;

        /// <summary>Способ реагирования на попытку помещения в хранилище видеозаписи
        /// с уже имеющимся в хранилище идентификатором.
        /// </summary>
        bool _DeleteDuplicateRecords;

        /// <summary>Список идентификаторов видеозаписей для синхронизации.
        /// Требует синхронизации доступа, т.к. используется разными потоками.
        /// </summary>
        List<string> _SyncRecIds;

        /// <summary>Объект, выполняющий синхронизацию хранилища видеоданных</summary>
        VideoStorageSynchronizer Synchronizer;

        /// <summary>"Кольцевой буфер" хранилища видеоданных.
        /// Требует синхронизации доступа, т.к. используется разными потоками
        /// </summary>
        /// <remarks>
        /// Следит за объемом занимаемого разделами места на диске и прорежает разделы, если занимаемое ими место
        /// на диске превышает определенный (заданный) предел
        /// </remarks>
        VideoStorageCircularBuffer CircularBuffer;

        #endregion
        
        #region Methods

        /// <summary>Конструктор класса</summary>
        public BaseVideoStorage()
        {
            _Active = false;
            _Partitions = new List<VideoPartition>();
            _UsedRecords = new Dictionary<BaseVideoRecord, string>();
            _UsedRecordIds = new List<string>();
            _SyncRecIds = new List<string>();
            
            Synchronizer = new VideoStorageSynchronizer(GetVideoStorageType());
            Synchronizer.OnDeleting += new EvCircularBufferDeleting(Synchronizer_OnDeleting);
            Synchronizer.OnTerminate += new EventHandler(Synchronizer_OnTerminate);
            
            CircularBuffer = new VideoStorageCircularBuffer();
            CircularBuffer.OnDeleting += new CancelEventHandler(CircularBuffer_OnDeleting);

            _DeleteDuplicateRecords = false;
        }

        /// <summary>Деструктор класса.
        /// Отслеживает завершение работающих дочерних потоков
        /// </summary>
        ~BaseVideoStorage()
        {
            if (Active) Active = false;
            TerminateCircularBuffer();
            TerminateSynchronizer();
        }

        /// <summary>Получить тип хранилища видеоданных для вызывающего объекта</summary>
        /// <returns>Тип хранилища видеоданных</returns>
        protected VideoStorageType GetVideoStorageType()
        {
            if (this is AskoVideoStorage) { return VideoStorageType.ASKO; }
            else { return VideoStorageType.Original; }
        }

        /// <summary>Создать объект для работы с видеозаписью хранилища</summary>
         /// <param name="id">Идентификатор видеозаписи</param>
         /// <param name="partition">Раздел хранилища, на котором расположена видеозапись</param>
         /// <param name="mode">Режим открытия видеозаписи</param>
         /// <returns>Объект для работы с видеозаписью</returns>
        protected abstract BaseVideoRecord NewVideoRecord(string id, VideoPartition partition, VideoRecordOpenMode mode);

        /// <summary>Создать объект для работы с видеозаписью хранилища</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="stat">Статус объекта, работающего с видеозаписью</param>
        /// <returns>Объект для работы с видеозаписью</returns>
        protected abstract BaseVideoRecord NewVideoRecord(string id, VideoStorageIntStat stat);

        /// <summary>Создать объект для работы с видеозаписью хранилища</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздел хранилища, на котором расположена видеозапись</param>
        /// <param name="stat">Статус объекта, работающего с видеозаписью</param>
        /// <returns>Объект для работы с видеозаписью</returns>
        protected abstract BaseVideoRecord NewVideoRecord(string id, int partition_id, VideoStorageIntStat stat);

        /// <summary>Генерирует событие "Запрос на удаление видеозаписи из хранилища"</summary>
        /// <param name="sender">Ссылка на "кольцевой буфер" хранилища</param>
        /// <param name="e">Дополнительные данные события</param>
        void CircularBuffer_OnDeleting(object sender, CancelEventArgs e)
        {
            if (OnCircularBufferDeleting != null) OnCircularBufferDeleting(this, e);
        }

        /// <summary>Ищет раздел хранилища, содержащий видеозапись с заданным идентификатором</summary>
        /// <param name="record_id">Идентификатор видеозаписи</param>
        /// <exception cref="System.ArgumentNullException">
        /// не задан идентификатор видеозаписи
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Пустой идентификатор видеозаписи
        /// </exception>
        /// <returns>
        /// Возвращает раздел хранилища, содержащий заданную видеозапись, или null, если ничего не найдено
        /// </returns>
        VideoPartition FindPartition(string record_id)
        {
            if (string.IsNullOrEmpty(record_id)) throw new ArgumentNullException();

            for (int i = 0; i < _Partitions.Count; ++i)
            {
                VideoPartition partition = _Partitions[i];
                if (partition.Status != VideoPartitionState.Ok) continue;
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(partition.Path);
                    if (VideoRecord.Exist(dir, record_id, GetVideoStorageType())) return partition;
                }
                catch { }
            }
            return null;
        }

        /// <summary>Выбрать раздел для хранения новой видеозаписи</summary>
        /// <returns>Раздел хранилища, куда будет помещена вновь создаваемая видеозапись</returns>
        VideoPartition GetPartitionForWrite()
        {
            if (_Partitions.Count == 0) return null;

            // Ищем разделы, которые еще доступны для записи
            VideoPartition result = null;
            long max_space = Int64.MinValue;
            List<VideoPartition> _remote_partitions = new List<VideoPartition>(_Partitions.Count);
            foreach (VideoPartition partition in _Partitions)
            {
                if (partition.Status != VideoPartitionState.Ok) continue;
                try
                {
                    long have_space = partition.FreeSpace - partition.FreeSpaceLimit;
                    if (have_space > max_space)
                    {
                        max_space = have_space;
                        result = partition;
                    }
                }
                catch (VideoStorageException)
                {
                    // Если мы попали сюда, значит при определении свободного места на диске возникло исключение,
                    // т.е. этот диск недоступен или является сетевым...
                    if (!string.IsNullOrEmpty(partition.Path) && (partition.Path.Substring(0, 2) == "\\\\"))
                    {
                        // Если путь начинается с символов '\\', значит мы имееем дело с сетевым адресом...
                        _remote_partitions.Add(partition);
                    }
                }
            }
            // Если других доступных разделов нет, берем первый найденный сетевой раздел
            if ((result == null) && (_remote_partitions.Count > 0))
            {
                result = _remote_partitions[0];
            }
            return result;
        }

        /// <summary>Получить объект для работы с видеозаписью</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition">Раздел хранилища, в котором размещена видеозапись</param>
        /// <exception cref="System.ArgumentNullException">Один или нескольно аргументов не заданы</exception>
        /// <exception cref="System.ArgumentException">Недопустимый идентификатор видеозаписи</exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">Ошибка ввода/вывода</exception>
        /// <returns>Возвращает указатель на объект для работы с видеозаписью (чтения/записи)</returns>
        BaseVideoRecord GetRecord(string id, VideoPartition partition)
        {
            VideoRecordOpenMode mode =  VideoRecord.Exist(partition.Path, id, GetVideoStorageType()) ? 
                                        VideoRecordOpenMode.Read : VideoRecordOpenMode.ReadWrite;

            BaseVideoRecord record = NewVideoRecord(id, partition, mode);
            record.OnDisposed += OnRecordDisposed;
            _UsedRecords.Add(record, id);
            lock (_UsedRecordIds)
            {
                int index = _UsedRecordIds.BinarySearch(id);
                if (index < 0)
                {
                    index = ~index;
                    if (index >= _UsedRecordIds.Count) _UsedRecordIds.Add(id);
                    else _UsedRecordIds.Insert(index, id);
                }
            }
            return record;
        }

        /// <summary>Получить объект для работы с видеозаписью</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition">Раздел хранилища, в котором размещена видеозапись</param>
        /// <param name="for_write">Признак, означающий, что объект создается для записи видеокадров</param>
        /// <exception cref="System.ArgumentNullException">Один или нескольно аргументов не заданы</exception>
        /// <exception cref="System.ArgumentException">Недопустимый идентификатор видеозаписи</exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">Ошибка ввода/вывода</exception>
        /// <returns>Возвращает указатель на объект для работы с видеозаписью (чтения/записи)</returns>
        BaseVideoRecord GetRecord(string id, VideoPartition partition, bool for_write)
        {
            VideoRecordOpenMode mode = for_write || !BaseVideoRecord.Exist(partition.Path, id, GetVideoStorageType()) ? 
                                       VideoRecordOpenMode.ReadWrite : VideoRecordOpenMode.Read;
            BaseVideoRecord record = NewVideoRecord(id, partition, mode);
            if (for_write) record.OnDisposing += OnRecordDisposing;
            record.OnDisposed += OnRecordDisposed;
            
            _UsedRecords.Add(record, id);
            lock (_UsedRecordIds)
            {
                int index = _UsedRecordIds.BinarySearch(id);
                if (index < 0)
                {
                    index = ~index;
                    if (index >= _UsedRecordIds.Count) _UsedRecordIds.Add(id);
                    else _UsedRecordIds.Insert(index, id);
                }
            }
            return record;
        }

        /// <summary>Обработчик события перед завершением работы с видеозаписью</summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="args">Дополнительные данные для события</param>
        void OnRecordDisposing(object sender, EventArgs args)
        {
            if (sender == null) return;
            BaseVideoRecord record = sender as BaseVideoRecord;
            if (record == null) return;
            record.OnDisposing -= OnRecordDisposing;
            // Фиксируем дату/время завершения записи, если она еще не была зафиксирована через интерфейс IVideoWriter
            try { if (record.RecordFinished == DateTime.MinValue) record.RecordFinished = DateTime.Now; } catch { }
        }

        /// <summary>Обработчик события завершения работы с видеозаписью</summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="args">Дополнительные данные для события</param>
        void OnRecordDisposed(object sender, EventArgs args)
        {
            if (sender == null) return;
            BaseVideoRecord record = sender as BaseVideoRecord;
            if (record == null) return;
            record.OnDisposed -= OnRecordDisposed;
            string id;
            if (_UsedRecords.TryGetValue(record, out id))
            {
                // Фиксируем освобождение видеозаписи
                _UsedRecords.Remove(record);
                // Если освобождаемая запись с данным идентификатором больше никем не используется -
                // удаляем идентификатор из списка _UsedRecordIds
                if (!_UsedRecords.ContainsValue(id))
                {
                    lock (_UsedRecordIds) { _UsedRecordIds.Remove(id); }
                }
            }
            try { OnRecordCloseAction(id); } catch { }
        }

        /// <summary>Обработчик события завершения работы с видеозаписью для перезаписываемой видеозаписи</summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="args">Дополнительные данные для события</param>
        void OnRewritedRecordDisposed(object sender, EventArgs args)
        {
            if (sender == null) return;
            BaseVideoRecord record = sender as BaseVideoRecord;
            if (record == null) return;
            string id = record.Id;
            if (record.Aborted)
            {
                // При аварийном закрытии видеозаписи удаляем временный файл.
                // Оригинальная запись при этом не изменяется
                try { BaseVideoRecord.Delete(record.Partition.Path, id, GetVideoStorageType()); }
                catch
                {
                    // Если возникли трудности с удалением временной видеозаписи,
                    // изменяем у нее статус интерфейса
                    record.Status = VideoStorageIntStat.IOError;
                }
            }
            else
            {
                // При нормальном закрытии записи подменяем оригинал перезаписанным вариантом видеозаписи
                try
                {
                    string original_id = GetOriginalId(id);
                    BaseVideoRecord.Delete(record.Partition.Path, original_id, GetVideoStorageType());
                    BaseVideoRecord.Rename(record.Partition.Path, id, original_id, GetVideoStorageType());
                }
                catch
                {
                    // Если возникли трудности с переименованием переписанной видеозаписи,
                    // изменяем у нее статус интерфейса
                    record.Status = VideoStorageIntStat.IOError;
                }
            }
        }

        /// <summary>Обработчик события завершения синхронизации хранилища видеоданных</summary>
        /// <param name="sender">Родитель события - синхронизирующий объект</param>
        /// <param name="e">Дополнительные данные о событии (не используется)</param>
        void Synchronizer_OnTerminate(object sender, EventArgs e)
        {
            if (OnSyncComplete != null)
            {
                VideoStorageSynchronizer sync = sender as VideoStorageSynchronizer;
                if (sync != null)
                {
                    SyncCompleteEventArgs args = new SyncCompleteEventArgs(sync.ErasedCount);
                    OnSyncComplete(sender, args);
                }
            }
            lock (_SyncRecIds) { _SyncRecIds.Clear(); }
        }

        /// <summary>Обработчик события запроса на удаление видеозаписи</summary>
        /// <param name="sender">Родитель события - синхронизирующий объект</param>
        /// <param name="e">Дополнительные сведения о событии</param>
        void Synchronizer_OnDeleting(object sender, CircularBufferCancelEventArgs e)
        {
            lock (_UsedRecordIds)
            {
                // Запрещаем удаление открытых для чтения/записи видеозаписей
                e.Cancel = _UsedRecordIds.BinarySearch(e.Id) >= 0;
            }
        }

        /// <summary>Принудительное завершение синхронизации хранилища</summary>
        /// <returns>Результат выполнения операции завершения синхронизации</returns>
        public VideoStorageResult StopSynchronizing()
        {
            if (!_Active) return VideoStorageResult.Fault;
            if (!Synchronizing) return VideoStorageResult.Fault;
            lock (Synchronizer.SyncRoot)
            {
                if (Synchronizer.Thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin) Synchronizer.Thread.Interrupt();
                else Synchronizer.Terminate();
            }
            return VideoStorageResult.Ok;
        }

        /// <summary>Экстренное завершение работы синхронизатора хранилища</summary>
        void TerminateSynchronizer()
        {
            lock (Synchronizer.SyncRoot)
            {
                // Отписываемся от извещения о завершении работы
                Synchronizer.OnTerminate -= Synchronizer_OnTerminate;
                // Завершаем работу синхронизатора
                switch (Synchronizer.Thread.ThreadState)
                {
                    case System.Threading.ThreadState.Stopped:
                    case System.Threading.ThreadState.Unstarted:
                        return;
                    case System.Threading.ThreadState.WaitSleepJoin:
                        Synchronizer.Thread.Interrupt();
                        break;
                    default:
                        Synchronizer.Thread.Abort();
                        break;
                }
                Synchronizer.Thread.Join();
            }
        }

        /// <summary>Активирует проверку "кольцевого буфера"</summary>
        void StartCircularBuffer()
        {
            lock (CircularBuffer.SyncRoot)
            {
                CircularBuffer.Partitions = _Partitions;
                CircularBuffer.CheckInterval = _CircularBufferCheckInterval * 1000;
                CircularBuffer.Start();
            }
        }

        /// <summary>Экстренное завершение работы "кольцевого буфера"</summary>
        void TerminateCircularBuffer()
        {
            System.Threading.Monitor.Enter(CircularBuffer.SyncRoot);
            {
                // Завершаем работу "кольцевого буфера"
                switch (CircularBuffer.Thread.ThreadState)
                {
                    case System.Threading.ThreadState.Stopped:
                    case System.Threading.ThreadState.Unstarted:
                        return;
                    case System.Threading.ThreadState.WaitSleepJoin:
                        CircularBuffer.Thread.Interrupt();
                        break;
                    default:
                        CircularBuffer.Thread.Abort();
                        break;
                }
                CircularBuffer.Thread.Join();
            }
            System.Threading.Monitor.Exit(CircularBuffer.SyncRoot);
        }

        /// <summary>Генерирует событие "Удалена запись из хранилища видеоданных"</summary>
        /// <param name="id">Идентификатор удаленной видеозаписи</param>
        void RaiseOnCircularBufferDeletedEvent(string id)
        {
            try
            {
                if (OnCircularBufferDeleted != null)
                {
                    CircularBufferEventArgs args = new CircularBufferEventArgs(id);
                    OnCircularBufferDeleted(this, args);
                }
            }
            catch { }
        }

        /// <summary>Метод для описания реакции хранилища на удаление видеозаписи</summary>
        /// <remarks>Может использоваться дочерними классами для отслеживания события удаления видеозаписи</remarks>
        /// <param name="record">Удаляемая видеозапись</param>
        protected virtual void OnRecordDeleteAction(object record) { }

        /// <summary>Метод для описания реакции хранилища на открытие видеозаписи</summary>
        /// <remarks>Может использоваться дочерними классами для отслеживания события удаления видеозаписи</remarks>
        /// <param name="record">Открываемая видеозапись</param>
        protected virtual void OnRecordOpenAction(object record) { }

        /// <summary>Метод для описания реакции хранилища на закрытие видеозаписи</summary>
        /// <remarks>Может использоваться дочерними классами для отслеживания события удаления видеозаписи</remarks>
        /// <param name="record">Закрываемая видеозапись</param>
        protected virtual void OnRecordCloseAction(object record) { }

        /// <summary>Генерирует событие "Открыта видеозапись хранилища"</summary>
        /// <param name="record">Видеозапись хранилища</param>
        protected void RaiseOnRecordOpenEvent(object record)
        {
            try { OnRecordOpenAction(record); } catch { }
            try { if (OnRecordOpen != null) OnRecordOpen(this, new VideoRecordEventArgs(record)); } catch { }
        }

        /// <summary>Генерирует событие "Закрытие видеозаписи хранилища"</summary>
        /// <param name="record">Видеозапись хранилища</param>
        protected void RaiseOnRecordCloseEvent(object record)
        {
            try { OnRecordCloseAction(record); } catch { }
            try { if (OnRecordClose != null) OnRecordClose(this, new VideoRecordEventArgs(record)); } catch { }
        }

        /// <summary>Генерирует событие "Удаление видеозаписи из хранилища"</summary>
        /// <param name="record">Видеозапись хранилища</param>
        protected void RaiseOnRecordDeleteEvent(object record)
        {
            try { OnRecordDeleteAction(record); } catch { }
            try { if (OnRecordDelete != null) OnRecordDelete(this, new VideoRecordEventArgs(record)); } catch { }
        }

        /// <summary>Получить временный идентификатор записи</summary>
        /// <param name="original_id">Исходный идентификатор записи</param>
        /// <returns>Временный идентификатор видеозаписи</returns>
        public string GetTempId(string original_id)
        {
            return original_id + "_rewrited";
        }

        /// <summary>Возвращает исходный идентификатор видеозаписи, основываясь на ее временном идентификаторе</summary>
        /// <param name="temp_id">Временный идентификатор видеозаписи</param>
        /// <returns>Исходный идентификатор видеозаписи</returns>
        public string GetOriginalId(string temp_id)
        {
            int index = temp_id.IndexOf("_rewrited");
            if (index >= 0) return temp_id.Substring(0, index);
            return temp_id;
        }

        #endregion

        #region Properties

        /// <summary>Признак выполнения синхронизации хранилища</summary>
        public bool Synchronizing
        {
            get
            {
                bool result;
                lock (Synchronizer.SyncRoot) { result = Synchronizer.Thread.IsAlive; }
                return result;
            }
        }

        /// <summary>Буферизация видео при записи</summary>
        public bool EnableBufferingVideo { get; set; }

        #endregion

        #region IVideoStorage Members

        #region Methods

        /// <summary>Получение настроек хранилища видеоданных
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        /// <returns>Объект, содержащий настройки хранилища видеоданных</returns>
        public VideoStorageSettings GetSettings()
        {
            VideoStorageSettings settings = new VideoStorageSettings();
            settings.CircleBufferCheckInterval = _CircularBufferCheckInterval;
            foreach (VideoPartition partition in _Partitions)
            {
                settings.Partitions.Add(VideoPartition.GetSettings(partition));
            }
            return settings;
            
        }

        /// <summary>Изменени настроек хранилища видеоданных.
        /// Допускается "горячее" изменение настроек (при активнированной работе хранилище -> Active = TRUE)
        /// <see cref="AlfaPribor.VideoStorage2.IVideoStorage"/>
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Не задан объект с настройками хранилища видеоданных
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// В объекте с настройками хранилища не заданы настройки разделов
        /// </exception>
        /// <exception cref="AlfaPribor.VideoStorage2.VideoStorageException">
        /// Возникает в случае, если синхронизация еще не завершена
        /// </exception>
        public void SetSettings(VideoStorageSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException();
            }
            if (settings.Partitions == null)
            {
                throw new ArgumentException();
            }
            if (Synchronizing)
            {
                throw new VideoStorageException("Synchronization in process!");
            }
            bool HasActive = Active;
            // Отключаем хранилище для его конфигурации
            Active = false;

            _Partitions.Clear();
            foreach (VideoPartitionSettings item in settings.Partitions)
            {
                try
                {
                    if (item == null)
                    {
                        continue;
                    }
                    VideoPartition partition = new VideoPartition(GetVideoStorageType(), item);
                    _Partitions.Add(partition);
                }
                catch { }
            }
            _CircularBufferCheckInterval = settings.CircleBufferCheckInterval;
            // Включаем хранилище, если оно было включено до конфигурирования
            if (HasActive)
            {
                Active = true;
            }
        }

        /// <summary>Получение интерфейса для чтения видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Интерфейс для чтения из хранилища</returns>
        public IVideoReader GetReader(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NewVideoRecord(id, VideoStorageIntStat.NotFound);
            }
            VideoPartition partition = FindPartition(id);
            if (partition == null)
            {
                return NewVideoRecord(id, VideoStorageIntStat.NotFound);
            }
            // Если запущена синхронизация хранилища и запрашиваемая видеозапись отсутствует
            // в целевом списке видеозаписей, которые останутся после синхронизации, то можно смело
            // возвратить ответ с кодом "На найдено", т.к. даже если она еще существует, то
            // она все-равно будет удалена.
            if (Synchronizing)
            {
                lock (_SyncRecIds)
                {
                    if (_SyncRecIds.BinarySearch(id) < 0)
                    {
                        return NewVideoRecord(id, VideoStorageIntStat.NotFound);
                    }
                }
            }
            try
            {
                BaseVideoRecord record = GetRecord(id, partition);
                RaiseOnRecordOpenEvent(record);
                return record;
            }
            catch
            {
                return NewVideoRecord(id, VideoStorageIntStat.FailToOpen);
            }
        }

        /// <summary>Получение интерфейса для чтения видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <param name="part_id">Идентификатор раздела хранилища</param>
        /// <returns>Интерфейс для чтения из хранилища</returns>
        public IVideoReader GetReader(string id, int part_id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NewVideoRecord(id, part_id, VideoStorageIntStat.NotFound);
            }
            int index = _Partitions.FindIndex(
                new Predicate<VideoPartition>(new VideoPartition.IdComparer(part_id).Equal)
            );
            if (index < 0)
            {
                return NewVideoRecord(id, part_id, VideoStorageIntStat.NotFound);
            }
            if (_Partitions[index].Status != VideoPartitionState.Ok)
            {
                return NewVideoRecord(id, part_id, VideoStorageIntStat.NotFound);
            }
            // Проверяем наличие видеозаписи с заданным идентификатором в указанном разделе хранилища
            bool record_exist = false;
            try
            {
                record_exist = BaseVideoRecord.Exist(_Partitions[index].Path, id, GetVideoStorageType());
            }
            catch { }
            if (!record_exist)
            {
                return NewVideoRecord(id, part_id, VideoStorageIntStat.NotFound);
            }
            // Если запущена синхронизация хранилища и запрашиваемая видеозапись отсутствует
            // в целевом списке видеозаписей, которые останутся после синхронизации, то можно смело
            // возвратить ответ с кодом "На найдено", т.к. даже если она еще существует, то
            // она все-равно будет удалена.
            if (Synchronizing)
            {
                lock (_SyncRecIds)
                {
                    if (_SyncRecIds.BinarySearch(id) < 0)
                    {
                        return NewVideoRecord(id, part_id, VideoStorageIntStat.NotFound);
                    }
                }
            }
            try
            {
                BaseVideoRecord record = GetRecord(id, _Partitions[index]);
                RaiseOnRecordOpenEvent(record);
                return record;
            }
            catch
            {
                return NewVideoRecord(id, part_id, VideoStorageIntStat.FailToOpen);
            }
        }

        /// <summary>Получение интерфейса для записи видеоданных в хранилище
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Интерфейс для записи в хранилище</returns>
        public IVideoWriter GetWriter(string id)
        {
            if (!_Active)
                return NewVideoRecord(id, VideoStorageIntStat.StorageInactive);

            if (string.IsNullOrEmpty(id))
                return NewVideoRecord(id, VideoStorageIntStat.NotFound);

            // Если запущена синхронизация хранилища и запрашиваемая видеозапись отсутствует
            // в целевом списке видеозаписей, которые останутся после синхронизации,
            // возвращаем ответ с кодом ошибки
            if (Synchronizing)
            {
                lock (_SyncRecIds)
                {
                    if (_SyncRecIds.BinarySearch(id) < 0)
                        return NewVideoRecord(id, VideoStorageIntStat.FailToCreate);
                }
            }

            // Проверяем, существует видеозапись с таким идентификатором или нет?
            VideoPartition partition = FindPartition(id);
            if (partition != null)
            {
                //Видеозапись существует.
                if (_DeleteDuplicateRecords)
                {
                    lock (_UsedRecordIds)
                    {
                        // Если видеозапись уже используется - отказать в удалении
                        if (_UsedRecordIds.BinarySearch(id) >= 0)
                            return NewVideoRecord(id, VideoStorageIntStat.FailToDelete);
                    }
                    // Если нет - пробуем удалить...
                    try
                    {
                        using (BaseVideoRecord record = NewVideoRecord(id, partition, VideoRecordOpenMode.Read))
                        {
                            RaiseOnRecordDeleteEvent(record);
                        }
                        BaseVideoRecord.Delete(partition.Path, id, GetVideoStorageType());
                    }
                    catch
                    {
                        //Невозможно удалить
                        return NewVideoRecord(id, VideoStorageIntStat.FailToDelete);
                    }
                }
                else
                {
                    //Уже существует
                    return NewVideoRecord(id, VideoStorageIntStat.AlreadyExist);
                }
            }
            else
            {
                //Выбираем раздел для записи
                partition = GetPartitionForWrite();
            }
            if (partition == null)
            {
                //Нет хранилища - невозможно создать
                return NewVideoRecord(id, VideoStorageIntStat.FailToCreate);
            }
            try
            {
                //Нормальное создание
                BaseVideoRecord record = GetRecord(id, partition, true);
                record.RecordStarted = DateTime.Now;
                RaiseOnRecordOpenEvent(record);
                return record;
            }
            catch
            {
                return NewVideoRecord(id, VideoStorageIntStat.FailToCreate);
            }
        }

        /// <summary>Получение интерфейса для перезаписи видеоданных в хранилище
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Интерфейс для перезаписи</returns>
        public IVideoWriter GetRewriter(string id)
        {
            if (!_Active)
            {
                return NewVideoRecord(id, VideoStorageIntStat.StorageInactive);
            }
            if (string.IsNullOrEmpty(id))
            {
                return NewVideoRecord(id, VideoStorageIntStat.NotFound);
            }
            // Если запущена синхронизация хранилища и запрашиваемая видеозапись отсутствует
            // в целевом списке видеозаписей, которые останутся после синхронизации,
            // возвращаем ответ с кодом ошибки
            if (Synchronizing)
            {
                lock (_SyncRecIds)
                {
                    if (_SyncRecIds.BinarySearch(id) < 0)
                    {
                        return NewVideoRecord(id, VideoStorageIntStat.FailToCreate);
                    }
                }
            }
            // Проверяем, существует видеозапись с таким идентификатором или нет?
            VideoPartition partition = FindPartition(id);
            if (partition == null)
            {
                return NewVideoRecord(id, VideoStorageIntStat.NotFound);
            }
            //выбираем раздел для записи
            partition = GetPartitionForWrite();
            if (partition == null)
            {
                return NewVideoRecord(id, VideoStorageIntStat.FailToCreate);
            }
            try
            {
                DateTime record_started = DateTime.Now;
                DateTime record_finished = DateTime.Now;
                try
                {
                    // Пробуем открыть исходную видеозапись для получения времени ее начала и окончания
                    IVideoIndex index = (IVideoIndex)GetRecord(id, partition);
                    try
                    {
                        if (index.Status == VideoStorageIntStat.Ok)
                        {
                            record_started = index.RecordStarted;
                            record_finished = index.RecordFinished;
                        }
                    }
                    finally
                    {
                        index.Close();
                    }
                }
                catch { }
                BaseVideoRecord record = GetRecord(GetTempId(id), partition);
                record.OnDisposed += new EventHandler(OnRewritedRecordDisposed);
                // Новая запись сохраняет данные о времени начала и окончания прежней видеозаписи
                record.RecordStarted = record_started;
                record.RecordFinished = record_finished;
                RaiseOnRecordOpenEvent(record);
                return record;
            }
            catch
            {
                return NewVideoRecord(id, VideoStorageIntStat.FailToCreate);
            }
        }

        /// <summary>Удаление видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <remarks>Вызывает обработчик события OnCircularBufferDeleted для удаляемой видеозаписи</remarks>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return VideoStorageResult.NotFound;
            }
            // Проверяем, существует видеозапись с таким идентификатором или нет?
            VideoPartition partition = FindPartition(id);
            if (partition == null)
            {
                return VideoStorageResult.NotFound;
            }
            lock (_UsedRecordIds)
            {
                // Если видеозапись используется - запрещаем удаление
                if (_UsedRecordIds.BinarySearch(id) >= 0)
                {
                    return VideoStorageResult.Blocked;
                }
            }
            // иначе - пытаемся удалить
            try
            {
                using (BaseVideoRecord record = NewVideoRecord(id, partition, VideoRecordOpenMode.Read))
                {
                    RaiseOnRecordDeleteEvent(record);
                }
                BaseVideoRecord.Delete(partition.Path, id, GetVideoStorageType());
                RaiseOnCircularBufferDeletedEvent(id);
            }
            catch
            {
                return VideoStorageResult.Fault;
            }
            return VideoStorageResult.Ok;
        }

        /// <summary>Удаление всех видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        /// <remarks>Вызывает обработчик события OnCircularBufferDeleted для каждой удаляемой видеозаписи</remarks>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult DeleteAll()
        {
            if (_Partitions.Count == 0)
            {
                return VideoStorageResult.Ok;
            }
            bool HasBlocked = false;
            int AllRecords = 0;
            int Deleted = 0;
            foreach (VideoPartition partition in _Partitions)
            {
                DirectoryInfo dir = new DirectoryInfo(partition.Path);
                if ((partition.Status == VideoPartitionState.Inactive) || !dir.Exists)
                {
                    HasBlocked = true;
                    continue;
                }
                try
                {
                    IList<string> ids = BaseVideoRecord.GetRecords(dir, GetVideoStorageType());
                    AllRecords += ids.Count;
                    foreach (string id in ids)
                    {
                        lock (_UsedRecordIds)
                        {
                            if (_UsedRecordIds.BinarySearch(id) >= 0)
                            {
                                continue;
                            }
                        }
                        try
                        {
                            using (BaseVideoRecord record = NewVideoRecord(id, partition, VideoRecordOpenMode.Read))
                            {
                                RaiseOnRecordDeleteEvent(record);
                            }
                            BaseVideoRecord.Delete(dir, id, GetVideoStorageType());
                            Deleted += 1;
                            RaiseOnCircularBufferDeletedEvent(id);
                        }
                        catch { }
                    }
                }
                catch { }
            }
            if ((AllRecords == 0) && HasBlocked)
            {
                return VideoStorageResult.Blocked;
            }
            else if (AllRecords != 0)
            {
                if (Deleted == 0)
                {
                    return VideoStorageResult.Fault;
                }
                else if (AllRecords != Deleted)
                {
                    return VideoStorageResult.Partial;
                }
                else
                {
                    return VideoStorageResult.Ok;
                }
            }
            else
            {
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Синхронизация содержимого хранилища (удаление несвязанных с БД видеозаписей)
        /// Выполняется асинхронно
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        /// <param name="id_list">Список идентификаторов поездов / видеозаписей (из БД)</param>
        /// <exception cref="System.ArgumentNullException">
        /// Не задан список идентификаторов видеозаписей для синхронизации
        /// </exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult Synchronize(IList<string> id_list)
        {
            if (id_list == null)
            {
                throw new ArgumentNullException();
            }
            if (!_Active)
            {
                return VideoStorageResult.Fault;
            }
            if (Synchronizing)
            {
                return VideoStorageResult.Blocked;
            }
            try
            {
                // DONE: Создать и запустить поток для синхронизации видеозаписей в хранилище
                lock (_SyncRecIds)
                {
                    _SyncRecIds = new List<string>(id_list);
                    _SyncRecIds.Sort();

                    Synchronizer.Partitions = _Partitions;
                    Synchronizer.Ids = _SyncRecIds;
                    Synchronizer.Start();
                }
            }
            catch
            {
                lock (_SyncRecIds)
                {
                    _SyncRecIds.Clear();
                }
                return VideoStorageResult.Fault;
            }

            return VideoStorageResult.Ok;
        }

        #endregion

        #region Properties

        /// <summary>Признак активности хранилища</summary>
        public bool Active
        {
            get
            {
                return _Active;
            }
            set
            {
                if (_Active == value)
                {
                    return;
                }
                _Active = value;
                if (_Active)
                {
                    if (CircularBuffer.Thread.IsAlive)
                    {
                        TerminateCircularBuffer();
                    }
                    StartCircularBuffer();
                }
                else
                {
                    lock(CircularBuffer.SyncRoot)
                    {
                        if (CircularBuffer.Thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                        {
                            CircularBuffer.Thread.Interrupt();
                        }
                        else
                        {
                            CircularBuffer.Terminate();
                        }
                    }
                }
            }
        }

        /// <summary>Информация о хранилище</summary>
        public VideoStorageInfo Info
        {
            get
            {
                VideoStorageInfo info = new VideoStorageInfo();
                foreach (VideoPartition partition in _Partitions)
                {
                    info.Partitions.Add(VideoPartition.GetInfo(partition));
                }
                return info;
            }
        }

        /// <summary>Интервал проверки кольцевого буфера в секундах</summary>
        public int CircularBufferCheckInterval
        {
            get { return _CircularBufferCheckInterval; }
            set { _CircularBufferCheckInterval = value; }
        }

        /// <summary>Удалять или нет дубликат видеозаписи в хранилище 
        /// при попытке помещения видеозаписи с уже имеющимся в хранилище идентификатором
        /// (через интерфейс IVideoWriter)
        /// </summary>
        public bool DeleteDuplicateRecords
        {
            get { return _DeleteDuplicateRecords; }
            set { _DeleteDuplicateRecords = value; }
        }

        #endregion

        #region Events

        /// <summary>Событие "Запрос на удаление записи из кольцевого буфера"
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        public event CancelEventHandler OnCircularBufferDeleting;

        /// <summary>Событие "Удалена запись из хранилища видеоданных"
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        public event EvCircularBufferDeleted OnCircularBufferDeleted;

        /// <summary>Событие "Закончена синхронизация хранилища"
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorage"/>
        /// </summary>
        public event EvSyncComplete OnSyncComplete;

        /// <summary>Событие "Удаление видеозаписи из хранилища"</summary>
        public event EvRecordDelete OnRecordDelete;

        /// <summary>Событие "Завершение работы с видеозаписью"</summary>
        /// <remarks>Возникает в случае закрытия последнего из открытых ранее интерфейсов взаимодействия с видеозаписью</remarks>
        public event EvRecordClose OnRecordClose;

        /// <summary>Событие "Открытие видеозаписи"</summary>
        /// <remarks>Возникает в случае запроса интерфейса для работы с видеозаписью</remarks>
        public event EvRecordOpen OnRecordOpen;

        #endregion

        #endregion
    }
}
