using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Client
{
    /// <summary>Класс предназначен для взаимодействия с удаленным сервисом записи данных в хранилище</summary>
    public class VideoWriterClient : VideoStorageCommunicationObject, IVideoWriterClient
    {
        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <exception cref="System.Exception">Ошибка создания фабрики каналов к сервису записи данных в хранилище</exception>
        public VideoWriterClient() { }

        #endregion

        #region VideoStorageCommunicationObject Members

        /// <summary>Создает коммуникационный объект - сервис доступа к интерфейсу записи видеоданных хранилища
        /// <see cref="AlfaPribor.VideoStorage.Server.VideoStorageCommunicationObject"/>
        /// </summary>
        /// <returns>Ссылка на коммуникационный объект</returns>
        protected override CommunicationObject CreateComObject()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.TransferMode = TransferMode.Streamed;
            return new ChannelFactory<IVideoWriterService>(binding);
        }

        #endregion

        #region IVideoWriterClient Members

        /// <summary>Получить интерфейс записи видеоданных
        /// <see cref="AlfaPribor.VideoStorage.Client.IVideoWriterClient"/>
        /// </summary>
        /// <param name="relative_uri">Относительный сетевой идентификатор интерфейса записи данных</param>
        /// <returns>Интерфейс записи данных</returns>
        public IVideoWriterService this[string relative_uri]
        {
            get
            {
                ChannelFactory<IVideoWriterService> _VideoWriterChannelFactory =
                    (ChannelFactory<IVideoWriterService>)_CommunicationObject;
                EndpointAddress address = new EndpointAddress(
                    VideoStorageCommunicationObject.BuildUrl(Address, Port, relative_uri)
                );
                return _VideoWriterChannelFactory.CreateChannel(address);
            }
        }

        #endregion
    }
}
