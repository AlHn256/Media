using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using AlfaPribor.VideoStorage;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Сервер видеохранилища</summary>
    /// <remarks>предоставляет доступ к видеохранилищу посредством служб WCF</remarks>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class VideoStorageServer : VideoStorage, IVideoStorageServer, IVideoStorageService
    {
        #region Fields

        /// <summary>Сервис удаленного доступа к хранилищу видеоданных</summary>
        private ServiceHost _VideoStorageService;

        /// <summary>Сетевой адрес сервера</summary>
        private string _Address;

        /// <summary>Номер сетевого порта сервера</summary>
        private int _Port;

        /// <summary>Отображение URI интерфейсов чтения видеоданных на сервисы доступа к ним</summary>
        private Dictionary<string, ServiceHost> _ReaderServices;

        /// <summary>Отображение URI интерфейсов чтения индексов видеоданных на сервисы доступа к ним</summary>
        private Dictionary<string, ServiceHost> _IndexServices;

        /// <summary>Отображение URI интерфейсов чтения видеоданных на объекты, реализующие эти интерфейсы</summary>
        private Dictionary<string, IVideoReader> _Readers;

        /// <summary>Отображение URI интерфейсов чтения индексов видеоданных на объекты, реализующие эти интерфейсы</summary>
        private Dictionary<string, IVideoIndex> _Indexes;

        #endregion

        #region Methods

        /// <summary>Конструктор класса.
        /// Инициирует свойства объекта класса значениями по умолчанию
        /// </summary>
        public VideoStorageServer()
        {
            _Address = "localhost";
            _Port = 8080;
            _ReaderServices = new Dictionary<string, ServiceHost>();
            _IndexServices = new Dictionary<string, ServiceHost>();
            _Readers = new Dictionary<string, IVideoReader>();
            _Indexes = new Dictionary<string, IVideoIndex>();
        }

        /// <summary>Конструктор класса.
        /// Инициирует свойства объекта класса заданными значениями сетевого адреса и номера сетевого порта
        /// </summary>
        /// <param name="address">Сетевой адрес сервера удаленного доступа к хранилищу видеоданных</param>
        /// <param name="port">Номер сетевого порта сервера удаленного доступа</param>
        public VideoStorageServer(string address, int port)
            : this()
        {
            Address = address;
            Port = port;
        }

        /// <summary>Строит URI для удаленного доступа к хранилищу видеоданных</summary>
        /// <param name="address">Сетевой адресс хранилища видеоданных</param>
        /// <param name="port">Номер сетевого порта хранилища видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом хранилища видеоданных</exception>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес хранилища видеоданных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта</exception>
        /// <returns>Строка URI</returns>
        public static string BuildStorageUri(string address, int port)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address == string.Empty)
            {
                throw new ArgumentException("String is empty!", "address");
            }
            if (port < 0)
            {
                throw new ArgumentOutOfRangeException("port");
            }
            return "net.tcp://" + address + ":" + port.ToString() + "//storage";
        }

        /// <summary>Строит строку уникального сетевого идентификатора для удаленного доступа к интерфейсу чтения видеоданных</summary>
        /// <param name="address">Сетевой адресс хранилища видеоданных</param>
        /// <param name="port">Номер сетевого порта хранилища видеоданных</param>
        /// <param name="id">Идентификатор видеозаписи в хранилище</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом хранилища видеоданных или с идентификатором видеозаписи</exception>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес хранилища видеоданных или идентификатор видеозаписи</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта</exception>
        /// <returns>Строка уникального сетевого идентификатора</returns>
        public static string BuildReaderUri(string address, int port, string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id == string.Empty)
            {
                throw new ArgumentException("String is empty!", "id");
            }
            return BuildStorageUri(address, port) + "//reader//" + id + "." + Guid.NewGuid().ToString();
        }

        /// <summary>Строит строку уникального сетевого идентификатора для удаленного доступа 
        /// к интерфейсу чтения индексов видеоданных</summary>
        /// <param name="reader_uri">URI интерфейса чтения видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения видеоданных</exception>
        public static string BuildIndexUri(string reader_uri)
        {
            if (reader_uri == null)
            {
                throw new ArgumentNullException("id");
            }
            return reader_uri + "//index";
        }

        /// <summary>Создает сервис для доступа к интерфейсу чтения видеоданных из хранилища</summary>
        /// <param name="reader">Интерфейс чтения видеоданных</param>
        /// <param name="uri">URI интерфейса чтения видеоданных</param>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения видеоданных</exception>
        /// <returns>Сервис доступа к интерфейсу чтения видеоданных</returns>
        private ServiceHost CreateReaderService(IVideoReader reader, string uri)
        {
            ServiceHost service = new ServiceHost(reader, new Uri(uri));
            NetTcpBinding binding = new NetTcpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.Security.Mode = SecurityMode.None;
            service.AddServiceEndpoint(typeof(IVideoReaderService), binding, "");
            service.Open();
            return service;
        }

        /// <summary>Закрывает сервис доступа к интерфейсу чтения видеоданных из хранилища</summary>
        /// <param name="uri">URI интерфейса чтения видеозаписи</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания закрытия сервиса</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения видеозаписи</exception>
        /// <exception cref="System.ArgumentException">Не задан URI интерфейса чтения видеозаписи</exception>
        private void DeleteReaderService(string uri)
        {
            ServiceHost service;
            if (_ReaderServices.TryGetValue(uri, out service))
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

        /// <summary>Создает сервис доступа к интерфейсу чтения индексов видеоданных из хранилища</summary>
        /// <param name="index">Интерфейс чтения индексов видеоданных</param>
        /// <param name="uri">URI интерфейса чтения индексов видеоданных</param>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения видеоданных</exception>
        /// <returns>Сервис доступа к интерфейсу чтения индексов видеоданных</returns>
        private ServiceHost CreateIndexService(IVideoIndex index, string uri)
        {
            ServiceHost service = new ServiceHost(index, new Uri(uri));
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            service.AddServiceEndpoint(typeof(IVideoIndexService), binding, "");
            service.Open();
            return service;
        }

        /// <summary>Закрывает сервис доступа к интерфейсу чтения индексов видеоданных из хранилища</summary>
        /// <param name="uri">URI интерфейса чтения индесков видеозаписи</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания закрытия сервиса</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения индексов видеозаписи</exception>
        /// <exception cref="System.ArgumentException">Не задан URI интерфейса чтения индексов видеозаписи</exception>
        private void DeleteIndexService(string uri)
        {
            ServiceHost service;
            if (_IndexServices.TryGetValue(uri, out service))
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

        /// <summary>Реакция на событие удаления видеозаписи из хранилища</summary>
        /// <param name="record">Удаляемая видеозапись</param>
        protected override void OnRecordDeleteAction(object record)
        {
            base.OnRecordDeleteAction(record);
            try
            {
                RemoveReader(record as IVideoReader);
            }
            catch { }

            try
            {
                RemoveIndex(record as IVideoIndex);
            }
            catch { }
        }

        /// <summary>Реакция на событие закрытия видеозаписи</summary>
        /// <param name="record">Закрываемая видеозапись</param>
        protected override void OnRecordCloseAction(object record)
        {
            base.OnRecordCloseAction(record);
            try
            {
                RemoveReader(record as IVideoReader);
            }
            catch { }

            try
            {
                RemoveIndex(record as IVideoIndex);
            }
            catch { }
        }

        /// <summary>Создает сервис удаленного доступа к интерфейсу чтения видеоданных и сохраняет информацию о нем</summary>
        /// <param name="reader">Открываемый интерфейс чтения видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом сервера
        /// или на идентификатор открываемой видеозаписи
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес или идентификатор открываемой видеозаписи</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Неверно задан номер сетевого порта сервера</exception>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса (URI) содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <returns>URI открытого интерфейса чтения видеоданных</returns>
        private string AddReader(IVideoReader reader)
        {
            string uri = BuildReaderUri(this.Address, this.Port, reader.Id);
            ServiceHost service = CreateReaderService(reader, uri);
            _ReaderServices.Add(uri, service);
            _Readers.Add(uri, reader);
            return uri;
        }

        /// <summary>Закрывает сервис чтения видеоданных и удаляет информацию о нем</summary>
        /// <param name="reader">Закрываемый интерфейс чтения данных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом сервера
        /// или на идентификатор открываемой видеозаписи
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес или идентификатор открываемой видеозаписи</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Неверно задан номер сетевого порта сервера</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        private void RemoveReader(IVideoReader reader)
        {
            string uri = BuildReaderUri(_Address, _Port, reader.Id);
            try
            {
                DeleteReaderService(uri);
            }
            finally
            {
                _ReaderServices.Remove(uri);
                _Readers.Remove(uri);
            }
        }

        /// <summary>Создает сервис удаленного доступа к интерфейсу чтения видеоданных и сохраняет информацию о нем</summary>
        /// <param name="reader_uri">URI интерфейса чтения индекса видеоданных</param>
        /// <param name="index">Открываемый интерфейс чтения индексов видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом сервера
        /// или на идентификатор открываемой видеозаписи
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан URI интерфейса чтения видеозаписи</exception>
        /// <exception cref="System.UriFormatException">Сетевой адрес интерфейса (URI) содержит ошибки</exception>
        /// <exception cref="System.InvalidOperationException">Невозможно запустить сервис</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        /// <returns>URI открытого интерфейса чтения видеоданных</returns>
        private string AddIndex(string reader_uri, IVideoIndex index)
        {
            string uri = BuildIndexUri(reader_uri);
            ServiceHost service = CreateIndexService(index, uri);
            _IndexServices.Add(uri, service);
            _Indexes.Add(uri, index);
            return uri;
        }

        /// <summary>Закрывает сервис чтения индексов видеоданных и удаляет информацию о нем</summary>
        /// <param name="index_uri">URI интерфейса чтения индекса видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с URI интерфейса чтения индексов видеоданных</exception>
        /// <exception cref="System.ArgumentException">Не задан URI интерфейса чтения интексов видеоданных</exception>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        private void RemoveIndex(string index_uri)
        {
            try
            {
                DeleteIndexService(index_uri);
            }
            finally
            {
                _IndexServices.Remove(index_uri);
                _Indexes.Remove(index_uri);
            }
        }

        /// <summary>Закрывает сервис чтения индексов видеоданных и удаляет информацию о нем</summary>
        /// <param name="index">Интерфейса чтения индекса видеоданных</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания запуска сервиса</exception>
        private void RemoveIndex(IVideoIndex index)
        {
            string uri = string.Empty;
            foreach (KeyValuePair<string, IVideoIndex> item in _Indexes)
            {
                if (item.Value == index)
                {
                    uri = item.Key;
                    break;
                }
            }
            if (uri != string.Empty)
            {
                RemoveIndex(uri);
            }
        }

        #endregion

        #region Properties

        /// <summary>Признак активности сервера удаленного доступа к видеохранилища
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorageServer"/>
        /// </summary>
        /// <exception cref="System.Exception">Не удалось запустить/остановить сервис удаленного доступа к хранилищу</exception>
        public new bool Active
        {
            get
            {
                if (_VideoStorageService == null)
                {
                    return false;
                }
                return (_VideoStorageService.State == CommunicationState.Opened) && base.Active;
            }
            set
            {
                if (value)
                {
                    try
                    {
                        string uri = BuildStorageUri(_Address, _Port);
                        _VideoStorageService = new ServiceHost(this, new Uri(uri));
                        NetTcpBinding binding = new NetTcpBinding();
                        binding.Security.Mode = SecurityMode.None;
                        _VideoStorageService.AddServiceEndpoint(
                            typeof(IVideoStorageService),
                            binding,
                            ""
                        );
                        _VideoStorageService.Open();
                    }
                    catch (Exception E)
                    {
                        throw new Exception("Can not start video storage service!", E);
                    }
                }
                else
                {
                    if (_VideoStorageService != null)
                    {
                        try
                        {
                            _VideoStorageService.Close();
                            _VideoStorageService = null;
                        }
                        catch (Exception E)
                        {
                            throw new Exception("Can not stop video storage service!", E);
                        }
                    }
                }
                base.Active = value;
            }
        }

        #endregion

        #region IVideoStorageServer Members

        /// <summary>Сетевой адрес сервера</summary>
        /// <exception cref="System.Exception">Невозможно изменить сетевой адрес у работающего сервера</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом сервера</exception>
        public string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                if (Active)
                {
                    throw new Exception("Video storage server is active!");
                }
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _Address = value;
            }
        }

        /// <summary>Номер сетевого порта сервера
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorageServer"/>
        /// </summary>
        /// <exception cref="System.Exception">Невозможно изменить номер сетевого порта у работающего сервера</exception>
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                if (Active)
                {
                    throw new Exception("Video storage server is active!");
                }
                _Port = value;
            }
        }

        #endregion

        #region IVideoStorageService Members

        /// <summary>Получение URI интерфейса чтения видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorageServer"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном интерфейсе для чтения из хранилища</returns>
        public VideoStorageIntResult GetReaderInt(string id)
        {
            IVideoReader reader = GetReader(id);
            if (reader.Status != VideoStorageIntStat.Ok)
            {
                return new VideoStorageIntResult(reader.Status, string.Empty);
            }
            try
            {
                string uri = AddReader(reader);
                return new VideoStorageIntResult(reader.Status, uri);
            }
            catch
            {
                return new VideoStorageIntResult(reader.Status, string.Empty);
            }
        }

        /// <summary>Получение URI интерфейса чтения видеоданных из хранилища
        /// <see cref="AlfaPribor.VideoStorage.IVideoStorageServer"/>
        /// </summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <param name="part_id">Идентификатор раздела хранилища</param>
        /// <returns>Сведения о запрошенном интерфейсе для чтения из хранилища</returns>
        public VideoStorageIntResult GetReaderInt(string id, int part_id)
        {
            IVideoReader reader = GetReader(id, part_id);
            if (reader.Status != VideoStorageIntStat.Ok)
            {
                return new VideoStorageIntResult(reader.Status, string.Empty);
            }
            try
            {
                string uri = AddReader(reader);
                return new VideoStorageIntResult(reader.Status, uri);
            }
            catch
            {
                return new VideoStorageIntResult(reader.Status, string.Empty);
            }

        }

        /// <summary>Получение интерфейса записи видеоданных в хранилище</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном интерфейсе записи в хранилище</returns>
        public VideoStorageIntResult GetWriterInt(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>Получение интерфейса перезаписи видеоданных в хранилище</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        /// <returns>Сведения о запрошенном интерфейсе для перезаписи</returns>
        /// <remarks>
        /// При открытии ищется видеозапись с указанным идентификатором
        /// Запись ведется во временный файл
        /// При закрытии существующая запись заменяется на содержимое временного файла
        /// (через удаление и переименование)
        /// </remarks>
        public VideoStorageIntResult GetRewriterInt(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>Получение интерфейса чтения индексов видеоданных</summary>
        /// <param name="uri">URI интерфейса чтения видеоданных</param>
        /// <returns>Сведения о запрошенном интерфейсе чтения индексов видеоданных</returns>
        public VideoStorageIntResult GetVideoIndexInt(string uri)
        {
            IVideoReader reader;
            if (!_Readers.TryGetValue(uri, out reader))
            {
                return new VideoStorageIntResult(VideoStorageIntStat.NotFound, string.Empty);
            }
            IVideoIndex index = (reader as IVideoReader).VideoIndex;
            try
            {
                uri = AddIndex(uri, index);
                return new VideoStorageIntResult(index.Status, uri);
            }
            catch
            {
                return new VideoStorageIntResult(index.Status, string.Empty);
            }
        }

        #endregion
    }
}
