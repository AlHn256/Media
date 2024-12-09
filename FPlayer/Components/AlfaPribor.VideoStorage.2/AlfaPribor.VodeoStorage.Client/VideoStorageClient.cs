using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Client
{
    /// <summary>Класс предназначен для удаленного взаимодействия с хранилищем видеоданных</summary>
    public class VideoStorageClient : VideoStorageCommunicationObject, IVideoStorageClient
    {
        #region Fields

        /// <summary>Интерфейс взаимодействия с хранилищем видеоданных</summary>
        private Server.IVideoStorageService _VideoStorageService;

        /// <summary>Относительный идентификатор сетевого ресурса (хранилища видеоданных)</summary>
        private string _RelativeUri;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <exception cref="System.Exception">Ошибка создания фабрики каналов к сервису хранилища видеоданных</exception>
        public VideoStorageClient()
        {
            //_VideoStorageChannelFactory = null;
            _VideoStorageService = null;
            _RelativeUri = string.Empty;
        }

        /// <summary>Конструктор класса</summary>
        /// <param name="address">Сетевой адрес сервера хранилища видеоданных</param>
        /// <param name="port">Номер сетевого порта</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на строку с сетевым адресом</exception>
        /// <exception cref="System.ArgumentException">Не задан сетевой адрес</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение номера сетевого порта</exception>
        public VideoStorageClient(string address, int port)
            : this()
        {
            Address = address;
            Port = port;
        }

        #endregion

        #region IVideoStorageClient Members

        /// <summary>Интерфейс взаимодействия с сервером хранилища видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Client.IVideoStorageClient"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Объект больше не существует</exception>
        public Server.IVideoStorageService VideoStorageService
        {
            get 
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageClient");
                }
                return _VideoStorageService; 
            }
        }

        /// <summary>Относительный идентификатор хранилища в сети
        /// <see cref="AlfaPribor.VideoStorage.Client.IVideoStorageClient"/>
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
                    throw new ObjectDisposedException("VideoStorageClient");
                }
                return _RelativeUri; 
            }
            set
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("VideoStorageClient");
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

        #region VideoStorageCommunicationObject Members

        /// <summary>Создает коммуникационный объект - канал доступа к удаленному сервису хранилища видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        /// <returns>Ссылка на коммуникационный объект</returns>
        protected override CommunicationObject CreateComObject()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            return new ChannelFactory<Server.IVideoStorageService>(binding);
        }

        /// <summary>Действия после открытия коммуникационного оъекта
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        protected override void AfterOpen()
        {
            base.AfterOpen();
            ChannelFactory<Server.IVideoStorageService> _VideoStorageChannelFactory = 
                (ChannelFactory<Server.IVideoStorageService>)_CommunicationObject;
            EndpointAddress address = new EndpointAddress(
                VideoStorageCommunicationObject.BuildUrl(Address, Port, _RelativeUri)
            );
            _VideoStorageService = _VideoStorageChannelFactory.CreateChannel(address);
        }

        /// <summary>Закрывает канал к сервису удаленного доступа хранилища видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        protected override void AfterClose()
        {
            base.AfterClose();
            _VideoStorageService = null;
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
            if (disposing)
            {
                _VideoStorageService = null;
                _RelativeUri = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
