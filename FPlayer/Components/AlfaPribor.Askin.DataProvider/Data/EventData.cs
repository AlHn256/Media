using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKIN.Data
{
    /// <summary>Данные записи журнала событий</summary>
    public class EventData
    {

        #region Fields

        /// <summary>Серийный номер события</summary>
        private int _Sn = 0;

        private DateTime _time = DateTime.MinValue;

        /// <summary>Дата и время события</summary>
        //private string _Time = string.Empty;

        /// <summary>Текст сообщения</summary>
        private string _Text = string.Empty;

        /// <summary>Данные сообщения</summary>
        private string _Data = string.Empty;

        /// <summary>Источник события</summary>
        private string _Source = string.Empty;

        /// <summary>Имя оператора</summary>
        private string _OpName = string.Empty;

        /// <summary>Идентификатор оператора</summary>
        private int _OpId = 0;

        /// <summary>Идентификатор сообщения</summary>
        private int _MsgId = 0;

        /// <summary>Наличие видео</summary>
        private bool _HasVideo = false;

        /// <summary>Пункт считывания</summary>
        private int _Recog = 0;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        public EventData()
        {
        }

        /// <summary>Конструктор (для выдачи в АСУ)</summary>
        /// <param name="sn">Серийный номер события</param>
        /// <param name="event_time">Дата и время события</param>
        /// <param name="cat">Категория сообщения</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="source">Источник события</param>
        /// <param name="op_name">Имя оператора</param>
        public EventData(int sn, DateTime event_time, string text, int recog_id, string source, string op_name)
        {
            Sn = sn;
            EventTime = event_time;
            Text = text;
            RecogId = recog_id;
            Source = source;
            OpName = op_name;
        }

        /// <summary>Конструктор для добавления в базу</summary>
        /// <param name="event_time">Дата и время события</param>
        /// <param name="msg_id">Идентификатор сообщения, описывающего событие</param>
        /// <param name="source">Данные об источнике события</param>
        /// <param name="data">Дополнительные данные о событии</param>
        /// <param name="op_id">Идентификатор оператора, во время работы которого произошло событие</param>
        public EventData(DateTime event_time, int msg_id, int recog_id, string data, string source, int op_id, bool has_video)
        {
            EventTime = event_time;
            MsgId = msg_id;
            RecogId = recog_id;
            Source = source;
            Data = data;
            OpId = op_id;
            HasVideo = has_video;
        }

        #endregion

        #region Properties

        /// <summary>Дата и время события</summary>
        [XmlElement("eventtime")]
        public DateTime EventTime
        {
            get { return _time; }
            set { _time = value; }
        }

        /// <summary>Серийный номер события</summary>
        [XmlElement("sn")]
        public int Sn
        {
            get { return _Sn; }
            set { _Sn = value; }
        }

        /// <summary>Дата и время события</summary>
        [XmlElement("time")]
        public string Time
        {
            get { return _time.ToString("hh:mm:ss:ms"); }
        }

        /// <summary>Текст сообщения</summary>
        [XmlElement("text")]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        /// <summary>Пункт считывания</summary>
        [XmlElement("recog")]
        public int RecogId
        {
            get { return _Recog; }
            set { _Recog = value; }
        }

        /// <summary>Данные события</summary>
        [XmlElement("data")]
        public string Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        /// <summary>Источник события</summary>
        [XmlElement("source")]
        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        /// <summary>Идентификатор сообщения</summary>
        [XmlIgnore]
        public int MsgId
        {
            get { return _MsgId; }
            set { _MsgId = value; }
        }

        /// <summary>Имя оператора</summary>
        [XmlElement("operator")]
        public string OpName
        {
            get { return _OpName; }
            set { _OpName = value; }
        }

        /// <summary>Идентификатор оператора</summary>
        [XmlIgnore]
        public int OpId
        {
            get { return _OpId; }
            set { _OpId = value; }
        }
        
        /// <summary>Наличие видео</summary>
        [XmlElement("video")]
        public bool HasVideo
        {
            get { return _HasVideo; }
            set { _HasVideo = value; }
        }

        #endregion

    }
}
