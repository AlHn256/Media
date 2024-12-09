using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using AlfaPribor.VideoStorage;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Данные о запрошенном интерфейсе, предназначенные для удаленного клиента</summary>
    [DataContract]
    public class VideoStorageIntResult
    {
        #region Fields

        /// <summary>Статус интерфейса объекта</summary>
        private VideoStorageIntStat _Status;

        /// <summary>Унифицированный идентификатор ресурса (объекта) в сети</summary>
        private string _URI;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        public VideoStorageIntResult()
        {
            _URI = string.Empty;
            _Status = VideoStorageIntStat.Ok;
        }

        /// <summary>Конструктор класса. Инициализирует объект класса значениями статуса интерфейса и URI интерфейса</summary>
        public VideoStorageIntResult(VideoStorageIntStat status, string uri)
        {
            _URI = uri;
            _Status = status;
        }

        #endregion

        #region Properties

        /// <summary>Статус интерфейса объекта</summary>
        [DataMember]
        public VideoStorageIntStat Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>Унифицированный идентификатор ресурса (объекта) в сети</summary>
        [DataMember]
        public string URI
        {
            get { return _URI; }
            set { _URI = value; }
        }

        #endregion
    }

}
