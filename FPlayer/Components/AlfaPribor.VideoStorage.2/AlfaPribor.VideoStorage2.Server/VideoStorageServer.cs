using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.InteropServices;
using AlfaPribor.VideoStorage;
using System.ServiceModel.Channels;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Сервер видеохранилища</summary>
    /// <remarks>предоставляет доступ к видеохранилищу посредством служб WCF</remarks>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class VideoStorageServer : VideoStorageCommunicationObject, IVideoStorageServer, IVideoStorageService
    {
        #region Fields

        /// <summary>Интерфейс хранилища видеоданных</summary>
        private IVideoStorage _VideoStorage;

        /// <summary>Отображение URL интерфейсов видеоданных на сервисы доступа к ним</summary>
        private Dictionary<string, ServiceHost> _Services;

        /// <summary>Отображение URL интерфейсов чтения видеоданных на объекты, реализующие эти интерфейсы</summary>
        private Dictionary<string, IVideoReaderService> _Readers;

        /// <summary>Отображение URL интерфейсов чтения индексов видеоданных на объекты, реализующие эти интерфейсы</summary>
        private Dictionary<string, IVideoIndexService> _Indexes;

        /// <summary>Отображение URL интерфейсов записи видеоданных на объекты, реализующие эти интерфейсы</summary>
        private Dictionary<string, IVideoWriterService> _Writers;

        /// <summary>Относительный идентификатор сетевого ресурса (хранилища видеоданных)</summary>
        private string _RelativeUri;

        #endregion

        #region Methods

        /// <summary>Конструктор класса.
        /// Инициирует свойства объекта класса значениями по умолчанию
        /// </summary>
        /// <param name="storage">Интерфейс хранилища видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на интерфейс хранилища видеоданных</exception>
        public VideoStorageServer(IVideoStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException();
            }
            _VideoStorage = storage;
            _Services = new Dictionary<string, ServiceHost>();
            _Readers = new Dictionary<string, IVideoReaderService>();
            _Indexes = new Dictionary<string, IVideoIndexService>();
            _Writers = new Dictionary<string, IVideoWriterService>();
            _RelativeUri = string.Empty;
        }

        /// <summary>Конструктор класса.
        /// Инициирует свойства объекта класса заданными значениями сетевого адреса, номера сетевого порта
        /// относительного сетевого идентификатора ресурса
        /// </summary>
        /// <param name="storage">Хранилище видеоданных</param>
        /// <param name="address">Сетевой адрес сервера удаленного доступа к хранилищу видеоданных</param>
        /// <param name="port">Номер сетевого порта сервера удаленного доступа</param>
        /// <param name="relative_uri">Относительный сетевой идентификатор хранилища</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на интерфейс хранилища видеоданных
        ///  или сетевой адрес хранилища
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес хранилища видеоданных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта</exception>
        public VideoStorageServer(VideoStorage storage, string address, int port, string relative_uri)
            : this(storage)
        {
            Address = address;
            Port = port;
            if (!string.IsNullOrEmpty(relative_uri))
            {
                _RelativeUri = relative_uri;
            }
        }

        /// <summary>Реакция на закрытие сервиса, обеспечивающего работу с видеозаписями хранилища</summary>
        /// <param name="sender">Объект, породивший событие</param>
        /// <param name="e">Дополнительные данные события</param>
        private void VideoServiceClosing(object sender, EventArgs e)
        {
            try
            {
                if (sender is IVideoReaderService)
                {
                    RemoveReader((IVideoReaderService)sender);
                }
                else if (sender is IVideoIndexService)
                {
                    RemoveIndex((IVideoIndexService)sender);
                }
                else if (sender is IVideoWriterService)
                {
                    RemoveWriter((IVideoWriterService)sender);
                }
            }
            catch { }
        }

        /// <summary>Строит строку уникального сетевого идентификатора для удаленного доступа к интерфейсу чтения видеоданных</summary>
        /// <param name="address">Сетевой адресс хранилища видеоданных</param>
        /// <param name="port">Номер сетевого порта хранилища видеоданных</param>
        /// <param name="relative_uri">Относительный сетевой идентификатор хранилища видеоданных</param>
        /// <param name="id">Идентификатор видеозаписи в хранилище</param>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес хранилища видеоданных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта хранилища видеоданных</exception>
        /// <returns>Строка уникального сетевого идентификатора</returns>
        private string BuildReaderUrl(string address, int port, string relative_uri, string id)
        {
            return 
                VideoStorageCommunicationObject.BuildUrl(address, port, relative_uri) + 
                "/reader/" + id + "-" + Guid.NewGuid().ToString();
        }

        /// <summary>Строит строку уникального сетевого идентификатора для удаленного доступа 
        /// к интерфейсу чтения индексов видеоданных</summary>
        /// <param name="address">Сетевой адресс хранилища видеоданных</param>
        /// <param name="port">Номер сетевого порта хранилища видеоданных</param>
        /// <param name="id">Идентификатор видеозаписи в хранилище</param>
        /// <param name="relative_uri">Относительный сетевой идентификатор хранилища видеоданных</param>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес хранилища видеоданных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта хранилища видеоданных</exception>
        /// <returns>Строка уникального сетевого идентификатора</returns>
        private string BuildIndexUrl(string address, int port, string relative_uri, string id)
        {
            return
                VideoStorageCommunicationObject.BuildUrl(address, port, relative_uri) +
                "/index/" + id + "-" + Guid.NewGuid().ToString();
        }

        /// <summary>Строит строку уникального сетевого идентификатора для удаленного доступа к интерфейсу чтения видеоданных</summary>
        /// <param name="address">Сетевой адресс хранилища видеоданных</param>
        /// <param name="port">Номер сетевого порта хранилища видеоданных</param>
        /// <param name="relative_uri">Относительный сетевой идентификатор хранилища видеоданных</param>
        /// <param name="id">Идентификатор видеозаписи в хранилище</param>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес хранилища видеоданных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта хранилища видеоданных</exception>
        /// <returns>Строка уникального сетевого идентификатора</returns>
        private string BuildWriterUrl(string address, int port, string relative_uri, string id)
        {
            return 
                VideoStorageCommunicationObject.BuildUrl(address, port, relative_uri) + 
                "/writer/" + id;
        }

        /// <summary>Создает сервис для доступа к интерфейсу чтения видеоданных из хранилища</summary>
        /// <param name="reader">Интерфейс чтения видеоданных</param>
        /// <param name="uri">URI интерфейса чтения видеоданных</param>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения видеоданных</exception>
        /// <returns>Сервис доступа к интерфейсу чтения видеоданных</returns>
        private ServiceHost CreateReaderService(IVideoReaderService reader, string uri)
        {
            ServiceHost service = new ServiceHost(reader, new Uri(uri));
            NetTcpBinding binding = new NetTcpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.Security.Mode = SecurityMode.None;
            service.AddServiceEndpoint(typeof(IVideoReaderService), binding, "");
            service.Open();
            return service;
        }

        /// <summary>Создает сервис для доступа к интерфейсу записи видеоданных в хранилище</summary>
        /// <param name="writer">Интерфейс записи видеоданных</param>
        /// <param name="uri">URI интерфейса записи видеоданных</param>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения видеоданных</exception>
        /// <returns>Сервис доступа к интерфейсу чтения видеоданных</returns>
        private ServiceHost CreateWriterService(IVideoWriterService writer, string uri)
        {
            ServiceHost service = new ServiceHost(writer, new Uri(uri));
            NetTcpBinding binding = new NetTcpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.Security.Mode = SecurityMode.None;
            service.AddServiceEndpoint(typeof(IVideoWriterService), binding, "");
            service.Open();
            return service;
        }

        /// <summary>Создает сервис доступа к интерфейсу чтения индексов видеоданных из хранилища</summary>
        /// <param name="index">Интерфейс чтения индексов видеоданных</param>
        /// <param name="uri">URI интерфейса чтения индексов видеоданных</param>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения видеоданных</exception>
        /// <returns>Сервис доступа к интерфейсу чтения индексов видеоданных</returns>
        private ServiceHost CreateIndexService(IVideoIndexService index, string uri)
        {
            ServiceHost service = new ServiceHost(index, new Uri(uri));
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            service.AddServiceEndpoint(typeof(IVideoIndexService), binding, "");
            service.Open();
            return service;
        }

        /// <summary>Создает сервис удаленного доступа к интерфейсу чтения видеоданных и сохраняет информацию о нем</summary>
        /// <param name="reader">Открываемый интерфейс чтения видеоданных</param>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса (URI) содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <returns>URI открытого интерфейса чтения видеоданных</returns>
        private string AddReader(IVideoReaderService reader)
        {
            string uri = BuildReaderUrl(Address, Port, _RelativeUri, reader.GetId());
            ServiceHost service = CreateReaderService(reader, uri);
            _Services.Add(uri, service);
            _Readers.Add(uri, reader);
            return uri;
        }

        /// <summary>Создает сервис удаленного доступа к интерфейсу записи видеоданных и сохраняет информацию о нем</summary>
        /// <param name="writer">Открываемый интерфейс записи видеоданных</param>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса (URI) содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <returns>URI открытого интерфейса записи видеоданных</returns>
        private string AddWriter(IVideoWriterService writer)
        {
            string uri = BuildReaderUrl(Address, Port, _RelativeUri, writer.GetId());
            ServiceHost service = CreateWriterService(writer, uri);
            _Services.Add(uri, service);
            _Writers.Add(uri, writer);
            return uri;
        }

        /// <summary>Создает сервис удаленного доступа к интерфейсу чтения видеоданных и сохраняет информацию о нем</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="index">Открываемый интерфейс чтения индексов видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом сервера
        /// или на идентификатор открываемой видеозаписи
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан URI интерфейса чтения видеозаписи</exception>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса (URI) содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <returns>URI открытого интерфейса чтения видеоданных</returns>
        private string AddIndex(string id, IVideoIndexService index)
        {
            string uri = BuildIndexUrl(Address, Port, _RelativeUri, id);
            ServiceHost service = CreateIndexService(index, uri);
            _Services.Add(uri, service);
            _Indexes.Add(uri, index);
            return uri;
        }

        /// <summary>Закрывает сервис доступа к интерфейсу видеозаписи</summary>
        /// <param name="uri">URI интерфейса видеозаписи</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания закрытия сервиса</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса видеозаписи</exception>
        private void DeleteService(string uri)
        {
            ServiceHost service;
            if (_Services.TryGetValue(uri, out service))
            {
                try
                {
                    service.Close();
                }
                catch (CommunicationObjectFaultedException)
                {
                    // Это можно смело проигнорировать, т.к. сервис все-равно будет закрыт
                }
            }
        }

        /// <summary>Закрывает сервис доступа к видеоданным и удаляет информацию о нем</summary>
        /// <param name="uri">URI закрываемого сервиса доступа к данных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку URI интерфейса доступа к видеоданным</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        private void RemoveService(string uri)
        {
            try
            {
                DeleteService(uri);
            }
            finally
            {
                _Services.Remove(uri);
            }
        }

        /// <summary>Закрывает сервис чтения видеоданных и удаляет информацию о нем</summary>
        /// <param name="reader">Интерфейс чтения данных</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <exception cref="System.InvalidOperationException">Не существует сервиса с заданным URI</exception>
        private void RemoveReader(IVideoReaderService reader)
        {
            KeyValuePair<string, IVideoReaderService> item = _Readers.First(target => target.Value == reader);
            try
            {
                RemoveService(item.Key);
            }
            finally
            {
                _Readers.Remove(item.Key);
            }
        }

        /// <summary>Закрывает сервис чтения индексов видеоданных и удаляет информацию о нем</summary>
        /// <param name="index">Интерфейс чтения индекса видеоданных</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания закрытия сервиса доступа к данным</exception>
        /// <exception cref="System.InvalidOperationException">Не существует сервиса с заданным URI</exception>
        private void RemoveIndex(IVideoIndexService index)
        {
            KeyValuePair<string, IVideoIndexService> item =_Indexes.First(target => target.Value == index);
            try
            {
                RemoveService(item.Key);
            }
            finally
            {
                _Indexes.Remove(item.Key);
            }
        }

        /// <summary>Закрывает сервис записи видеоданных и удаляет информацию о нем</summary>
        /// <param name="index">Интерфейс записи видеоданных</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания закрытия сервиса доступа к данным</exception>
        /// <exception cref="System.InvalidOperationException">Не существует сервиса с заданным URI</exception>
        private void RemoveWriter(IVideoWriterService index)
        {
            KeyValuePair<string, IVideoIndexService> item = _Indexes.First(target => target.Value == index);
            try
            {
                RemoveService(item.Key);
            }
            finally
            {
                _Writers.Remove(item.Key);
            }
        }

        /// <summary>Инициализирует сервис удаленного доступа</summary>
        private void InitService()
        {
            ServiceHost _VideoStorageService = (ServiceHost)_CommunicationObject;
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            _VideoStorageService.AddServiceEndpoint(
                typeof(IVideoStorageService),
                binding,
                ""
            );
        }

        /// <summary>Закрывает все открытые сервисы</summary>
        private void CloseAllServices()
        {
            foreach (KeyValuePair<string,ServiceHost> item in _Services)
            {
                try
                {
                    item.Value.Close();
                }
                catch { }
            }
            _Services.Clear();
        }

        /// <summary>Закрывает все открытые интерфейсы работы с видеоданными</summary>
        private void CloseAllInterfaces()
        {
            CloseReaders();
            CloseWriters();
            CloseIndexes();
        }

        /// <summary>Закрывает все открытые интерфейсы чтения видеоданных</summary>
        private void CloseReaders()
        {
            if (_Readers == null)
            {
                return;
            }
            List< KeyValuePair<string,IVideoReaderService> > readers_list = _Readers.ToList();
            foreach (KeyValuePair<string,IVideoReaderService> item in readers_list)
            {
                try
                {
                    item.Value.Close();
                }
                catch { }
            }
            readers_list.Clear();
        }

        /// <summary>Закрывает все открытые интерфейсы записи видеоданных</summary>
        private void CloseWriters()
        {
            if (_Writers == null)
            {
                return;
            }
            List<KeyValuePair<string, IVideoWriterService>> writers_list = _Writers.ToList();
            foreach (KeyValuePair<string, IVideoWriterService> item in writers_list)
            {
                try
                {
                    item.Value.Close();
                }
                catch { }
            }
            writers_list.Clear();
        }

        /// <summary>Закрывает все открытые интерфейсы чтения индексов видеоданных</summary>
        private void CloseIndexes()
        {
            if (_Indexes == null)
            {
                return;
            }
            List<KeyValuePair<string, IVideoIndexService>> indexes_list = _Indexes.ToList();
            foreach (KeyValuePair<string, IVideoIndexService> item in indexes_list)
            {
                try
                {
                    item.Value.Close();
                }
                catch { }
            }
            indexes_list.Clear();
        }

        #endregion

        #region VideoStorageCommunicationObject Members

        /// <summary>Создает коммуникационный объект - сервис доступа к хранилищу видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        /// <returns>Ссылка на коммуникационный объект</returns>
        protected override CommunicationObject CreateComObject()
        {
            string uri = BuildUrl(Address, Port, _RelativeUri);
            return new ServiceHost(this, new Uri(uri));
        }

        /// <summary>Выполняет основные действия по открытию коммуникационного объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cервис удаленного доступа не может быть запущен</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия сервиса удаленного доступа</exception>
        protected override void  BeforeOpen()
        {
 	        base.BeforeOpen();
            InitService();
        }

        /// <summary>Высвобождает ресурсы объекта
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        /// <param name="disposing">
        /// Если равен FALSE - освобождаются только неуправляемые ресурсы,
        /// иначе - освобождаются все ресурсы объекта
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            CloseAllInterfaces();
            CloseAllServices();
            if (disposing)
            {
                _VideoStorage = null;
                _Services.Clear();
                _Services = null;
                _Readers.Clear();
                _Readers = null;
                _Indexes.Clear();
                _Indexes = null;
                _Writers.Clear();
                _Writers = null;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region IVideoStorageServer Members

        /// <summary>Интерфейс хранилища видеоданных, к которому предоставляется удаленный доступ
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageServer"/>
        /// </summary>
        public IVideoStorage VideoStorage
        {
            get { return _VideoStorage; }
        }

        /// <summary>Возвращает универсальный сетевой локатор хранилища видеоданных</summary>
        public string Url
        {
            get
            {
                return BuildUrl(Address, Port, _RelativeUri);
            }
        }

        /// <summary>Относительный идентификатор хранилища в сети
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageServer"/>
        /// </summary>
        /// <exception cref="System.Exception">Невозможно изменить URI у открытого коммуникационного объекта</exception>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с относительным URI</exception>
        public string RelativeUri
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageServer");
                }
                return _RelativeUri;
            }
            set
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageServer");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("RelativeUri");
                }
                if (Active)
                {
                    throw new Exception("Can not change property value of opened object!");
                }
                _RelativeUri = value;
            }
        }

        #endregion

        #region IVideoStorageService Members

        /// <summary>Получение URI сервиса чтения видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        public VideoStorageIntResult GetReader(string id)
        {
            IVideoReader i_reader = _VideoStorage.GetReader(id);
            if (i_reader.Status != VideoStorageIntStat.Ok)
            {
                return new VideoStorageIntResult(i_reader.Status, string.Empty);
            }
            VideoReaderService reader = new VideoReaderService(i_reader);
            reader.Closing += new EventHandler(VideoServiceClosing);
            try
            {
                string uri = AddReader(reader);
                return new VideoStorageIntResult(i_reader.Status, uri);
            }
            catch
            {
                return new VideoStorageIntResult(i_reader.Status, string.Empty);
            }
        }

        /// <summary>Получение URI сервиса чтения видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <param name="part_id">Идентификатор раздела хранилища</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        public VideoStorageIntResult GetReader(string id, int part_id)
        {
            IVideoReader i_reader = _VideoStorage.GetReader(id, part_id);
            if (i_reader.Status != VideoStorageIntStat.Ok)
            {
                return new VideoStorageIntResult(i_reader.Status, string.Empty);
            }
            VideoReaderService reader = new VideoReaderService(i_reader);
            reader.Closing +=new EventHandler(VideoServiceClosing);
            try
            {
                
                string uri = AddReader(reader);
                return new VideoStorageIntResult(i_reader.Status, uri);
            }
            catch
            {
                return new VideoStorageIntResult(i_reader.Status, string.Empty);
            }

        }

        /// <summary>Получение сервиса записи видеоданных в хранилище
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        public VideoStorageIntResult GetWriter(string id)
        {
            IVideoWriter i_writer = _VideoStorage.GetWriter(id);
            if (i_writer.Status != VideoStorageIntStat.Ok)
            {
                return new VideoStorageIntResult(i_writer.Status, string.Empty);
            }
            VideoWriterService writer = new VideoWriterService(i_writer);
            writer.Closing += new EventHandler(VideoServiceClosing);
            try
            {
                string uri = AddWriter(writer);
                return new VideoStorageIntResult(i_writer.Status, uri);
            }
            catch
            {
                return new VideoStorageIntResult(i_writer.Status, string.Empty);
            }
        }

        /// <summary>Получение сервиса перезаписи видеоданных в хранилище
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        /// <remarks>
        /// При открытии ищется видеозапись с указанным идентификатором
        /// Запись ведется во временный файл
        /// При закрытии существующая запись заменяется на содержимое временного файла
        /// (через удаление и переименование)
        /// </remarks>
        public VideoStorageIntResult GetRewriter(string id)
        {
            IVideoWriter i_writer = _VideoStorage.GetRewriter(id);
            if (i_writer.Status != VideoStorageIntStat.Ok)
            {
                return new VideoStorageIntResult(i_writer.Status, string.Empty);
            }
            VideoWriterService writer = new VideoWriterService(i_writer);
            writer.Closing += new EventHandler(VideoServiceClosing);
            try
            {
                string uri = AddWriter(writer);
                return new VideoStorageIntResult(i_writer.Status, uri);
            }
            catch
            {
                return new VideoStorageIntResult(i_writer.Status, string.Empty);
            }
        }

        /// <summary>Получение сервиса чтения индексов видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном сервисе</returns>
        public VideoStorageIntResult GetVideoIndex(string id)
        {
            IVideoReader i_reader = _VideoStorage.GetReader(id);
            if (i_reader.Status != VideoStorageIntStat.Ok)
            {
                return new VideoStorageIntResult(i_reader.Status, string.Empty);
            }
            VideoIndexService index = null;
            try
            {
                index = new VideoIndexService(i_reader.VideoIndex);
                index.Closing += new EventHandler(VideoServiceClosing);
                id = AddIndex(id, index);
                return new VideoStorageIntResult(index.GetStatus(), id);
            }
            catch
            {
                return 
                    new VideoStorageIntResult(index != null ? index.GetStatus() : VideoStorageIntStat.FailToOpen, string.Empty);
            }
        }

        /// <summary>Удаление видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult Delete(string id)
        {
            return _VideoStorage.Delete(id);
        }

        /// <summary>Удаление всех видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult DeleteAll()
        {
            return _VideoStorage.DeleteAll();
        }

        /// <summary>Чтение настроек хранилища видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        public VideoStorageSettings GetSettings()
        {
            return _VideoStorage.GetSettings();
        }

        /// <summary>Изменение настроек хранилища видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.IVideoStorageService"/>
        /// </summary>
        public void SetSettings(VideoStorageSettings settings)
        {
            _VideoStorage.SetSettings(settings);
        }

        #endregion
    }
}
