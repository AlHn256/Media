using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Remoting.Messaging;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Базовый класс коммуникационных объектов, обеспечивающих обмен данными посредством WCF</summary>
    public abstract class VideoStorageCommunicationObject : IVideoStorageCommunicationObject, IDisposable
    {
        #region Fields

        /// <summary>Сетевой адрес</summary>
        private string _Address;

        /// <summary>Номер сетевого порта</summary>
        private int _Port;

        /// <summary>Признак освобождения ресурсов объекта</summary>
        protected bool _disposed;

        /// <summary>Коммуникационный объект</summary>
        protected CommunicationObject _CommunicationObject;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        public VideoStorageCommunicationObject()
        {
            _Address = "localhost";
            _Port = 8080;
            _disposed = false;
            _CommunicationObject = null;
        }

        /// <summary>Конструктор класса</summary>
        /// <param name="address">Сетевой адрес коммуникационного объекта</param>
        /// <param name="port">Номер сетевого порта</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом</exception>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта</exception>
        public VideoStorageCommunicationObject(string address, int port)
            : this()
        {
            Address = address;
            Port = port;
        }

        /// <summary>Декструктор класса</summary>
        ~VideoStorageCommunicationObject()
        {
            // Освобождаем неуправляемые ресурсы...
            Dispose(false);
        }

        /// <summary>Строит URL (унифицированный локатор ресурса) для удаленного доступа к ресурсу</summary>
        /// <param name="address">Сетевой адресс хоста</param>
        /// <param name="port">Номер сетевого порта хоста</param>
        /// <param name="relative_uri">Относительный сетевой идентификатор ресурса</param>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес хоста</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого</exception>
        /// <returns>Строка URL</returns>
        public static string BuildUrl(string address, int port, string relative_uri)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("Invalid net address!", "address");
            }
            if (port < 0)
            {
                throw new ArgumentOutOfRangeException("port", "Invalid port number!");
            }
            string url = "net.tcp://" + address + ":" + port.ToString();
            if (!string.IsNullOrEmpty(relative_uri))
            {
                if (relative_uri[0] != '/')
                {
                    url += "/";
                }
                url += relative_uri;
            }
            return url;
        }

        /// <summary>Генерирует событие "Коммуникационный объект открыт"</summary>
        private void RaiseOpenedEvent()
        {
            if (Opened != null)
            {
                try
                {
                    Opened(this, EventArgs.Empty);
                }
                catch { }
            }
        }

        /// <summary>Генерирует событие "Коммуникационный объект закрыт"</summary>
        private void RaiseClosedEvent()
        {
            if (Closed != null)
            {
                try
                {
                    Closed(this, EventArgs.Empty);
                }
                catch { }
            }
        }

        /// <summary>Генерирует событие "Коммуникационный объект неисправен"</summary>
        private void RaiseFaultedEvent()
        {
            if (Faulted != null)
            {
                try
                {
                    Faulted(this, EventArgs.Empty);
                }
                catch { }
            }
        }

        /// <summary>Создает коммуникационный объект</summary>
        /// <returns>Ссылка на коммуникационный объект</returns>
        protected abstract CommunicationObject CreateComObject();

        /// <summary>Высвобождает ресурсы объекта</summary>
        /// <param name="disposing">
        /// Если равен FALSE - освобождаются только неуправляемые ресурсы,
        /// иначе - освобождаются все ресурсы объекта
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            try
            {
                Close();
            }
            catch { }
            if (disposing)
            {
                _Address = null;
                _CommunicationObject = null;
            }
            _disposed = true;
        }

        /// <summary>Обработчик события "Коммуникационный объект неисправен"</summary>
        /// <param name="sender">Ссылка на коммуникационный объект, породивший событие</param>
        /// <param name="e">Дополнительные сведения о событии</param>
        private void _CommunicationObject_Faulted(object sender, EventArgs e)
        {
            try
            {
                OnFaulted();
            }
            catch { }
            RaiseFaultedEvent();
        }

        /// <summary>Обработчик события "Коммуникационный объект закрыт"</summary>
        /// <param name="sender">Ссылка на коммуникационный объект, породивший событие</param>
        /// <param name="e">Дополнительные сведения о событии</param>
        private void _CommunicationObject_Closed(object sender, EventArgs e)
        {
            try
            {
                OnClosed();
            }
            catch { }
            RaiseClosedEvent();
        }

        /// <summary>Обработчик события "Коммуникационный объект открыт"</summary>
        /// <param name="sender">Ссылка на коммуникационный объект, породивший событие</param>
        /// <param name="e">Дополнительные сведения о событии</param>
        private void _CommunicationObject_Opened(object sender, EventArgs e)
        {
            try
            {
                OnOpened();
            }
            catch { }
            RaiseOpenedEvent();
        }

        /// <summary>Реакция на событие "Коммуникационный объект открыт"</summary>
        protected virtual void OnOpened()
        {
        }

        /// <summary>Реакция на событие "Коммуникационный объект закрыт"</summary>
        protected virtual void OnClosed()
        {
        }

        /// <summary>Реакция на событие "Коммуникационный объект неисправен"</summary>
        protected virtual void OnFaulted()
        {
        }

        /// <summary>Действия до открытия коммуникационного объекта</summary>
        protected virtual void BeforeOpen()
        {
        }

        /// <summary>Действия после открытия коммуникационного оъекта</summary>
        protected virtual void AfterOpen()
        {
        }

        /// <summary>Действия до закрытия коммуникационного объекта</summary>
        protected virtual void BeforeClose()
        {
        }

        /// <summary>Действия после закрытия коммуникационного объекта</summary>
        protected virtual void AfterClose()
        {
        }

        #endregion

        #region Properties

        /// <summary>Признак того, что коммуникационный объект открыт</summary>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        public bool Active
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageCommunicationObject");
                }
                bool result = false;
                if (_CommunicationObject != null)
                {
                    result = (_CommunicationObject.State != CommunicationState.Created) &&
                        (_CommunicationObject.State != CommunicationState.Closed);
                }
                return result;
            }
        }

        #endregion

        #region IVideoStorageCommunicationObject Members

        #region Methods

        /// <summary>Открывает коммуникационный объект для обмена данными по сети
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        /// <exception cref="System.InvalidOperationException">Коммуникационный канал не может быть создан</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного канала, установленное по умолчанию</exception>
        public void Open()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("VideoStorageCommunicationObject");
            }
            _CommunicationObject = CreateComObject();
            _CommunicationObject.Opened += new EventHandler(_CommunicationObject_Opened);
            _CommunicationObject.Closed += new EventHandler(_CommunicationObject_Closed);
            _CommunicationObject.Faulted += new EventHandler(_CommunicationObject_Faulted);
            BeforeOpen();
            _CommunicationObject.Open();
            AfterOpen();
        }

        /// <summary>Открывает коммуникационный объект для обмена данными по сети в течение заданного интервала времени
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="timeout">Временной интервал открытия коммуникационного канала</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Задан недопустимый временной интервал</exception>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        /// <exception cref="System.InvalidOperationException">Коммуникационный канал не может быть создан</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного канала</exception>
        public void Open(TimeSpan timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("VideoStorageCommunicationObject");
            }
            _CommunicationObject = CreateComObject();
            _CommunicationObject.Opened += new EventHandler(_CommunicationObject_Opened);
            _CommunicationObject.Closed += new EventHandler(_CommunicationObject_Closed);
            _CommunicationObject.Faulted += new EventHandler(_CommunicationObject_Faulted);
            BeforeOpen();
            _CommunicationObject.Open(timeout);
            AfterOpen();
        }

        /// <summary>Закрывает коммуникационный объект
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного канала</exception>
        public void Close()
        {
            BeforeClose();
            if (_CommunicationObject != null)
            {
                try
                {
                    _CommunicationObject.Close();
                }
                catch (CommunicationObjectFaultedException)
                {
                }
                _CommunicationObject.Opened -= new EventHandler(_CommunicationObject_Opened);
                _CommunicationObject.Closed -= new EventHandler(_CommunicationObject_Closed);
                _CommunicationObject.Faulted -= new EventHandler(_CommunicationObject_Faulted);
            }
            AfterClose();
        }

        /// <summary>Закрывает коммуникационный объект в течение заданного интервала времени
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="timeout">Временной интервал ожидания закрытия коммуникационного канала</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного канала</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Задан недопустимый временной интервал</exception>
        /// <exception cref="System.ServiceModel.CommunicationObjectFaultedException">Коммуникационный объект находится в состоянии Faulted
        /// и не может быть использован
        /// </exception>
        public void Close(TimeSpan timeout)
        {
            BeforeClose();
            if (_CommunicationObject != null)
            {
                try
                {
                    _CommunicationObject.Close();
                }
                catch (CommunicationObjectFaultedException)
                {
                }
                _CommunicationObject.Opened -= new EventHandler(_CommunicationObject_Opened);
                _CommunicationObject.Closed -= new EventHandler(_CommunicationObject_Closed);
                _CommunicationObject.Faulted -= new EventHandler(_CommunicationObject_Faulted);
            }
            AfterClose();
        }

        /// <summary>Начинает асинхронную операцию открытия коммуникационного объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="callback">Метод обратного вызова, который будет вызван по окончании операции открытия</param>
        /// <param name="state">Объект с дополнительными сведениями об операции открытия коммуникационного объекта</param>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        /// <exception cref="System.InvalidOperationException">Коммуникационный канал не может быть создан</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного канала, установленное по умолчанию</exception>
        /// <exception cref="System.ServiceModel.CommunicationObjectFaultedException">Коммуникационный объект находится в состоянии Faulted
        /// и не может быть использован для связи
        /// </exception>
        /// <returns>Результат выполнения операции</returns>
        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("VideoStorageCommunicationObject");
            }
            _CommunicationObject = CreateComObject();
            _CommunicationObject.Opened += new EventHandler(_CommunicationObject_Opened);
            _CommunicationObject.Closed += new EventHandler(_CommunicationObject_Closed);
            _CommunicationObject.Faulted += new EventHandler(_CommunicationObject_Faulted);
            BeforeOpen();
            return _CommunicationObject.BeginOpen(callback, state);
        }

        /// <summary>Начинает асинхронную операцию открытия коммуникационного объекта в течении заданного интервала времени
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="timeout">Время ожидания открытия коммуникационного объекта</param>
        /// <param name="callback">Метод обратного вызова, который будет вызван по окончании операции открытия</param>
        /// <param name="state">Объект с дополнительными сведениями об операции открытия коммуникационного объекта</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Задан недопустимый временной интервал</exception>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        /// <exception cref="System.InvalidOperationException">Коммуникационный канал не может быть создан</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного канала, установленное по умолчанию</exception>
        /// <exception cref="System.ServiceModel.CommunicationObjectFaultedException">Коммуникационный объект находится в состоянии Faulted
        /// и не может быть использован для связи
        /// </exception>
        /// <returns>Результат выполнения операции</returns>
        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("VideoStorageCommunicationObject");
            }
            _CommunicationObject = CreateComObject();
            _CommunicationObject.Opened += new EventHandler(_CommunicationObject_Opened);
            _CommunicationObject.Closed += new EventHandler(_CommunicationObject_Closed);
            _CommunicationObject.Faulted += new EventHandler(_CommunicationObject_Faulted);
            BeforeOpen();
            return _CommunicationObject.BeginOpen(timeout, callback, state);
        }

        /// <summary>Начинает асинхронную операцию закрытия коммуникационного объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="callback">Метод обратного вызова, который будет вызван по окончании операции закрытия</param>
        /// <param name="state">Объект с дополнительными сведениями об операции закрытия коммуникационного объекта</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания закрытия коммуникационного канала, установленное по умолчанию</exception>
        /// <exception cref="System.ServiceModel.CommunicationObjectFaultedException">Коммуникационный объект находится в состоянии Faulted
        /// и не может быть использован
        /// </exception>
        /// <returns>Результат выполнения операции</returns>
        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            BeforeClose();
            if (_CommunicationObject != null)
            {
                return _CommunicationObject.BeginClose(callback, state);
            }
            else
            {
                throw new CommunicationObjectFaultedException();
            }
        }

        /// <summary>Начинает асинхронную операцию закрытия коммуникационного объекта в течение заданного интервала времени
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="timeout">Время ожидания закрытия коммуникационного объекта</param>
        /// <param name="callback">Метод обратного вызова, который будет вызван по окончании операции закрытия</param>
        /// <param name="state">Объект с дополнительными сведениями об операции закрытия коммуникационного объекта</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Задан недопустимый временной интервал</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания закрытия коммуникационного оъекта</exception>
        /// <exception cref="System.ServiceModel.CommunicationObjectFaultedException">Коммуникационный объект находится в состоянии Faulted
        /// и не может быть использован
        /// </exception>
        /// <returns>Результат выполнения операции</returns>
        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            BeforeClose();
            if (_CommunicationObject != null)
            {
                return _CommunicationObject.BeginClose(timeout, callback, state);
            }
            else
            {
                throw new CommunicationObjectFaultedException();
            }
        }

        /// <summary>Завершает асинхронную операцию открытия коммуникационного объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="result">Результат операции открытия</param>
        public void EndOpen(IAsyncResult result)
        {
            _CommunicationObject.EndOpen(result);
            AfterOpen();
        }

        /// <summary>Завершает асинхронную операцию закрытия коммуникационного объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="result">Результат операции закрытия</param>
        public void EndClose(IAsyncResult result)
        {
            _CommunicationObject.EndOpen(result);
            AfterClose();
        }

        #endregion

        #region Properties

        /// <summary>Сетевой адрес коммуникационного объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <exception cref="System.Exception">Невозможно изменить сетевой адрес у открытого коммуникационного оъекта</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом</exception>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес</exception>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        public string Address
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageCommunicationObject");
                }
                return _Address;
            }
            set
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageCommunicationObject");
                }
                if (Active)
                {
                    throw new Exception("Can not change property value of opened object!");
                }
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (value == string.Empty)
                {
                    throw new ArgumentException();
                }
                _Address = value;
            }
        }

        /// <summary>Номер сетевого порта коммуникационного объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <exception cref="System.Exception">Невозможно изменить номер сетевого порта у открытого коммуникационного объекта</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта</exception>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        public int Port
        {
            get 
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageCommunicationObject");
                }
                return _Port; 
            }
            set
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageCommunicationObject");
                }
                if (Active)
                {
                    throw new Exception("Can not change property value of opened object!");
                }
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _Port = value;
            }
        }

        /// <summary>Коммуникационное состояние объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        public CommunicationState State
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageCommunicationObject");
                }
                return _CommunicationObject != null ? _CommunicationObject.State : CommunicationState.Closed;
            }
        }

        #endregion

        #region Events

        /// <summary>Событие "Коммуникационный объект открыт"
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        public event EventHandler Opened;

        /// <summary>Событие "Коммуникационный объект закрыт"
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        public event EventHandler Closed;

        /// <summary>Событие "Коммуникационный объект неисправен"
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageCommunicationObject"/>
        /// </summary>
        public event EventHandler Faulted;

        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>Освобождает ресурсы объекта</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
