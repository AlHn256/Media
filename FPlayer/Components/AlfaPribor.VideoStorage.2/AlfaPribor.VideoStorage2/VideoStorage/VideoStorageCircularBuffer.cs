using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using AlfaPribor.Threads;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Класс "кольцевого буфера" хранилища видеоданных</summary>
    /// <remarks>
    /// Следит за указанными разделами хранилища и в случае, если занимаемое место на диске под раздел
    /// превышеет установленный предел, удаляет записи из раздела
    /// </remarks>
    class VideoStorageCircularBuffer : TerminatedThread
    {
        #region Fields

        /// <summary>Список разделов хранилища, за которыми требуется вести наблюдение</summary>
        protected List<VideoPartition> _Partitions;

        /// <summary>Интервал между проверками занимаемого разделами места на диске (мсек)</summary>
        private int _CheckInterval;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <exception cref="System.ArgumentNullException">
        /// Недопустимое значение параметра partiotions (попытка присвоить значение null)
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Значение параметра interval должно быть больше нуля
        /// </exception>
        public VideoStorageCircularBuffer(IList<VideoPartition> partitions, int interval)
            : base(ThreadPriority.BelowNormal)
        {
            Partitions = partitions;
            CheckInterval = interval;
        }

        /// <summary>Конструктор класса</summary>
        /// <exception cref="System.ArgumentNullException">
        /// Недопустимое значение параметра partiotions (попытка присвоить значение null)
        /// </exception>
        public VideoStorageCircularBuffer(IList<VideoPartition> partitions)
            : this(partitions, 60000) { }

        /// <summary>Конструктор класса</summary>
        public VideoStorageCircularBuffer()
            : this(new List<VideoPartition>()) { }

        /// <summary>Виртуальный метод, в котором осуществляется слежение за разделами</summary>
        protected override void DoExecute()
        {
            while (!Terminated)
            {
                int PartitionsCount = 0;        // Число доступных разделов хранилища, исключая сетевые разделы
                int FullPartitionsCount = 0;    // Число разделов, которые больше недоступны для записи (заполнены до предела)

                // Повторяем, пока не освободится достаточно места для хранения данных хотябы на одном
                // из разделов хранилища
                while (PartitionsCount == FullPartitionsCount)
                {
                    if (!Monitor.TryEnter(SyncRoot, 100))
                    {
                        continue;
                    }
                    PartitionsCount = 0;
                    FullPartitionsCount = 0;
                    foreach (VideoPartition partition in _Partitions)
                    {
                        if (partition.Status != VideoPartitionState.Ok)
                        {
                            continue;
                        }
                        // Существование директории не проверяем, т.к. это отражает статус раздела
                        DirectoryInfo dir = new DirectoryInfo(partition.Path);
                        try
                        {
                            // Если раздел имеет сетевой доступ, по при определении его размера возникнет исключение,
                            // т.о. он не попадет в счетчик PartitionsCount
                            if (partition.Full)
                            {
                                ++FullPartitionsCount;
                            }
                            ++PartitionsCount;
                        }
                        catch (Exception e)
                        {
                            Monitor.Exit(SyncRoot);
                            try
                            {
                                RaiseOnException(e);
                            }
                            catch { }
                            Monitor.Enter(SyncRoot);

                        }
                        // Проверяем условие завершения работы потока
                        if (Terminated)
                        {
                            Monitor.Exit(SyncRoot);
                            return;
                        }
                    }
                    // Если нет ни одного раздела, имеющего статус VideoPartitionState.Ok,
                    // прекращаем проверку и ожидаем...
                    if (PartitionsCount == 0)
                    {
                        Monitor.Exit(SyncRoot);
                        break;
                    }
                    // Если все разделы заполнены данными больше, чем оговорено свойством FreeSpaceLimit
                    // для каждого раздела - генерируем событие с запросом на удаление записи с целью
                    // освобождения места для записи в хранилище
                    if (PartitionsCount == FullPartitionsCount)
                    {
                        Monitor.Exit(SyncRoot);
                        try
                        {
                            bool cancel = false;
                            RaiseOnDeleting(ref cancel);
                            if (cancel)
                            {
                                break;
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
                        Monitor.Enter(SyncRoot);
                    }
                    // Проверяем условие завершения работы потока
                    if (Terminated)
                    {
                        Monitor.Exit(SyncRoot);
                        return;
                    }
                    Monitor.Exit(SyncRoot);
                }
                Thread.Sleep(_CheckInterval);
            }
        }

        /// <summary>Генерирует событие с запросом на удаление видеозаписей из хранилища</summary>
        /// <remarks>Метод не перехватывает исключения, произошедшие в обработчике события</remarks>
        protected void RaiseOnDeleting(ref bool cancel)
        {
            if (OnDeleting != null)
            {
                CancelEventArgs arg = new CancelEventArgs(cancel);
                OnDeleting(this, arg);
                cancel = arg.Cancel;
            }
        }

        #endregion

        #region Properties

        /// <summary>Список разделов хранилища, за которыми требуется вести наблюдение</summary>
        /// <exception cref="System.ArgumentNullException">
        /// Недопустимое значение свойства (попытка присвоить значение null)
        /// </exception>
        public IList<VideoPartition> Partitions
        {
            get { return _Partitions; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _Partitions = new List<VideoPartition>(value);
            }
        }

        /// <summary>Интервал между проверками занимаемого разделами места на диске (мсек)</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Значение свойства должно быть больше нуля
        /// </exception>
        public int CheckInterval
        {
            get { return _CheckInterval; }
            set 
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _CheckInterval = value; 
            }
        }

        #endregion

        #region Events

        /// <summary>Событие с запросом на удаление видеозаписи из хранилища</summary>
        public event CancelEventHandler OnDeleting;

        #endregion
    }
}
