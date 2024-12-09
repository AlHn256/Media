using System;
using System.Threading;

namespace AlfaPribor.Threads
{
    /// <summary>Класс, представляющий собой "обертку" вокруг потока (класса Thread),
    /// способный корректно завершать (на усмотрение приграммиста) работу потока -
    /// не используя метод Thread.Abort.
    /// </summary>
    public abstract class TerminatedThread
    {
        #region Fields

        /// <summary>Объект, предоставляющий управление потоком</summary>
        private Thread _Thread;

        /// <summary>Признак необходимого завершения роботы потока</summary>
        private volatile bool _Terminated;

        /// <summary>Объект синхронизации. 
        /// Предназначен для доступа к открытым свойствам и методам класса из разных потоков
        /// </summary>
        private object _SyncRoot;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        public TerminatedThread()
            : this(ThreadPriority.Normal, true) { }

        /// <summary>Конструктор класса.
        /// Создает поток с указанным приоритетом, но не запускает его на выполнение
        /// </summary>
        /// <param name="priority">Приоритет создаваемого потока</param>
        public TerminatedThread(System.Threading.ThreadPriority priority)
            : this(priority, true) { }

        /// <summary>Конструктор класса.
        /// Создает поток с указанным приоритетом и запускает его на выполнение, если параметр suspended равен false
        /// </summary>
        /// <param name="priority">Приоритет создаваемого потока</param>
        /// <param name="suspended">Отвечает за запрет автозапуска созданного потока</param>
        public TerminatedThread(System.Threading.ThreadPriority priority, bool suspended)
        {
            _SyncRoot = new object();
            _Terminated = false;
            _Thread = new Thread(new ThreadStart(Execute));
            _Thread.Priority = priority;
            if (!suspended)
            {
                _Thread.Start();
            }
        }

        /// <summary>Код, который исполняется в потоке</summary>
        private void Execute()
        {
            try
            {
                RaiseOnStart();
            }
            catch { }
            try
            {
                DoExecute();
            }
            catch(ThreadInterruptedException)
            {
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            catch (Exception e)
            {
                try
                {
                    RaiseOnException(e);
                }
                catch { }
            }
            try
            {
                RaiseOnTerminate();
            }
            catch { }
        }

        /// <summary>Генерирует событие "Завершение выполнения потока"</summary>
        /// <remarks>Метод не перехватывает исключения, произошедшие в обработчике события</remarks>
        private void RaiseOnTerminate()
        {
            if (OnTerminate != null)
            {
                OnTerminate(this, EventArgs.Empty);
            }
        }

        /// <summary>Генерирует событие "Начало выполнения потока"</summary>
        /// <remarks>Метод не перехватывает исключения, произошедшие в обработчике события</remarks>
        private void RaiseOnStart()
        {
            if (OnStart != null)
            {
                OnStart(this, EventArgs.Empty);
            }
        }

        /// <summary>Генерирует событие "Возникновение исключительной ситуации при выполнении потока"</summary>
        /// <remarks>Метод не перехватывает исключения, произошедшие в обработчике события</remarks>
        protected void RaiseOnException(Exception e)
        {
            if (OnException != null)
            {
                ThreadExceptionEventArgs args = new ThreadExceptionEventArgs(e);
                OnException(this, args);
            }
        }

        /// <summary>Код, реализуемый дочерним классом. Выполняется в потоке</summary>
        protected abstract void DoExecute();

        /// <summary>Сигнализирует потоку о необходимости завершения работы</summary>
        public void Terminate()
        {
            _Terminated = true;
        }

        /// <summary>Запускает поток на выполнение</summary>
        /// <exception cref="System.OutOfMemoryException">
        /// Недостаточно памяти для запуска этого потока
        /// </exception>
        /// <exception cref="System.Threading.ThreadStateException">
        /// Поток уже запущен
        /// </exception>
        public void Start()
        {
            // Если поток завершил свою работу - создаем его снова
            if (_Thread.ThreadState == ThreadState.Stopped)
            {
                _Thread = new Thread(new ThreadStart(Execute));
            }
            _Terminated = false;
            _Thread.Start();
        }

        /// <summary>Запускает поток на выполнение с заданным приоритетом</summary>
        /// <exception cref="System.OutOfMemoryException">
        /// Недостаточно памяти для запуска этого потока
        /// </exception>
        /// <exception cref="System.Threading.ThreadStateException">
        /// Поток уже запущен
        /// </exception>
        public void Start(ThreadPriority priority)
        {
            // Если поток завершил свою работу - создаем его снова
            if (_Thread.ThreadState == ThreadState.Stopped)
            {
                _Thread = new Thread(new ThreadStart(Execute));
            }
            _Terminated = false;
            _Thread.Priority = priority;
            _Thread.Start();
        }

        #endregion

        #region Properties

        /// <summary>Объект, предоставляющий управление потоком</summary>
        public Thread Thread
        {
            get { return _Thread; }
        }

        /// <summary>Свойство, которое сигнализирует методу DoExecute о том, что ему необходимо завершить работу<\summury>
        /// <remarks>
        /// Ввутри метода DoExecute нужно периодически проверять значение свойства Terminated и выйти из метода
        /// при равенстве Terminated значению TRUE.
        /// </remarks>
        public bool Terminated
        {
            get { return _Terminated; }
            set { _Terminated = value; }
        }

        /// <summary>Объект синхронизации. 
        /// Предназначен для доступа к открытым свойствам и методам класса из разных потоков
        /// </summary>
        public object SyncRoot
        {
            get { return _SyncRoot; }
        }

        #endregion

        #region Events

        /// <summary>Событие "Начало выполнения потока"</summary>
        public event EventHandler OnStart;

        /// <summary>Событие "Завершение выполнения потока"</summary>
        public event EventHandler OnTerminate;

        /// <summary>Событие "Возникновение исключительной ситуации при выполнении потока"</summary>
        public event ThreadExceptionEventHandler OnException;

        #endregion
    }
}
