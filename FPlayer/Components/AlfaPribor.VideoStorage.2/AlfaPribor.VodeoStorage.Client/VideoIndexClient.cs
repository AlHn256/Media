using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.VideoStorage.Server;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AlfaPribor.VideoStorage.Client
{
    public class VideoIndexClient : VideoStorageCommunicationObject, IVideoIndexClient
    {
        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <exception cref="System.Exception">Ошибка создания фабрики каналов к сервису чтения индексов видеоданных хранилища</exception>
        public VideoIndexClient() { }

        #endregion

        #region VideoStorageCommunicationObject Members

        /// <summary>Создает коммуникационный объект - сервис доступа к интерфейсу чтения индексов видеоданных хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        /// <returns>Ссылка на коммуникационный объект</returns>
        protected override CommunicationObject CreateComObject()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.TransferMode = TransferMode.Buffered;
            return new ChannelFactory<IVideoIndexService>(binding);
        }

        #endregion

        #region IVideoIndexClient Members

        /// <summary>Получить интерфейс чтения индексов видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Client.IVideoIndexClient"/>
        /// </summary>
        /// <param name="relative_uri">Относительный сетевой идентификатор интерфейса чтения индексов видеоданных</param>
        /// <returns>Интерфейс чтения индексов видеоданных</returns>
        public IVideoIndexService this[string relative_uri]
        {
            get 
            {
                ChannelFactory<IVideoIndexService> _VideoIndexChannelFactory =
                    (ChannelFactory<IVideoIndexService>)_CommunicationObject;
                EndpointAddress address = new EndpointAddress(
                    VideoStorageCommunicationObject.BuildUrl(Address, Port, relative_uri)
                );
                return _VideoIndexChannelFactory.CreateChannel(address);
            }
        }

        #endregion
    }
}
