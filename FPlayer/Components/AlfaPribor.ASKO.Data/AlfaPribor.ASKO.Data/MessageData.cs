using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Данные сообщения</summary>
    public class MessageData
    {

        #region Fields

        /// <summary>Идентификатор сообщения</summary>
        int _Id;
        /// <summary>Текст сообщения</summary>
        string _Text;

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса значениями по умолчанию</summary>
        public MessageData()
        {
            _Text = string.Empty;
        }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="id">Идентификатор сообщения</param>
        /// <param name="category">Категория сообщения</param>
        /// <param name="text">Текст сообщения</param>
        public MessageData(int id, string text)
        {
            _Id = id;
            _Text = text;
        }

        #endregion

        #region Properties

        /// <summary>Идентификатор сообщения</summary>
        [XmlElement("id")]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Текст сообщения</summary>
        [XmlElement("msgtext")]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        #endregion

    }
}
