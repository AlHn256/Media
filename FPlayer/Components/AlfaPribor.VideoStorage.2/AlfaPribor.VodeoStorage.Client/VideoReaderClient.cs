using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Client
{
    /// <summary>Класс предназначен для взаимодействия с удаленным сервисом чтения данных хранилища</summary>
    public class VideoReaderClient : VideoStorageCommunicationObject, IVideoReaderClient
    {
        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <exception cref="System.Exception">Ошибка создания фабрики каналов к сервису чтения данных хранилища</exception>
        public VideoReaderClient() { }

        #endregion

        #region VideoStorageCommunicationObject Members

        /// <summary>Создает коммуникационный объект - сервис доступа к интерфейсу чтения видеоданных хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        /// <returns>Ссылка на коммуникационный объект</returns>
        protected override CommunicationObject CreateComObject()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.TransferMode = TransferMode.Streamed;
            return new ChannelFactory<IVideoReaderService>(binding);
        }

        #endregion

        #region IVideoReaderClient Members

        /// <summary>Получить интерфейс чтения видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Client.IVideoReaderClient"/>
        /// </summary>
        /// <param name="relative_uri">Относительный сетевой идентификатор интерфейса чтения данных</param>
        /// <returns>Интерфейс чтения данных</returns>
        public IVideoReaderService this[string relative_uri]
        {
            get 
            {
                ChannelFactory<IVideoReaderService> _VideoReaderChannelFactory =
                    (ChannelFactory<IVideoReaderService>)_CommunicationObject;
                EndpointAddress address = new EndpointAddress(
                    VideoStorageCommunicationObject.BuildUrl(Address, Port, relative_uri)
                );
                return _VideoReaderChannelFactory.CreateChannel(address);
            }
        }

        #endregion
    }
}
