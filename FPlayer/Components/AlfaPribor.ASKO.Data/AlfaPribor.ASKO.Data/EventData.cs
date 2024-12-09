using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKO.Data
{
    /// <summary>Данные записи журнала событий</summary>
    public class EventData
    {

        #region Fields

        /// <summary>Серийный номер события</summary>
        int _Sn = 0;

        DateTime _time = DateTime.MinValue;

        /// <summary>Текст сообщения</summary>
        string _Text = string.Empty;

        /// <summary>Данные сообщения</summary>
        string _Data = string.Empty;

        /// <summary>Источник события</summary>
        string _Source = string.Empty;

        /// <summary>Имя оператора</summary>
        string _OpName = string.Empty;

        /// <summary>Идентификатор оператора</summary>
        int _OpId = 0;

        /// <summary>Идентификатор сообщения</summary>
        int _MsgId = 0;

        /// <summary>Наличие видео</summary>
        bool _HasVideo = false;

        /// <summary>Идентификатор пути осмотра</summary>
        int _WayId = 0;
        
        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        public EventData()
        {
        }

        /*
        /// <summary>Конструктор</summary>
        /// <param name="sn">Серийный номер события</param>
        /// <param name="event_time">Дата и время события</param>
        /// <param name="cat">Категория сообщения</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="source">Источник события</param>
        /// <param name="op_name">Имя оператора</param>
        /// <param name="way_id">Идентификатор пути осмотра</param>
        public EventData(int sn, DateTime event_time, string text, string source, string op_name)
        {
            Sn = sn;
            EventTime = event_time;
            Text = text;
            Source = source;
            OpName = op_name;
        }
        */

        /// <summary>Конструктор для добавления в базу</summary>
        /// <param name="event_time">Дата и время события</param>
        /// <param name="msg_id">Идентификатор сообщения, описывающего событие</param>
        /// <param name="source">Данные об источнике события</param>
        /// <param name="data">Дополнительные данные о событии</param>
        /// <param name="op_id">Идентификатор оператора, во время работы которого произошло событие</param>
        /// <param name="way_id">Идентификатор пути осмотра</param>
        public EventData(DateTime event_time, int msg_id, string data, string source, int op_id, bool has_video, int way_id)
        {
            EventTime = event_time;
            MsgId = msg_id;
            Source = source;
            Data = data;
            OpId = op_id;
            HasVideo = has_video;
            WayId = way_id;
        }

        #endregion

        #region Properties

        /// <summary>Дата и время события</summary>
        public DateTime EventTime
        {
            get { return _time; }
            set { _time = value; }
        }

        /// <summary>Серийный номер события</summary>
        public int Sn
        {
            get { return _Sn; }
            set { _Sn = value; }
        }

        /// <summary>Дата и время события</summary>
       public string Time
        {
            get { return _time.ToString("hh:mm:ss:ms"); }
        }

        /// <summary>Текст сообщения</summary>
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        /// <summary>Данные события</summary>
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
        public int MsgId
        {
            get { return _MsgId; }
            set { _MsgId = value; }
        }

        /// <summary>Имя оператора</summary>
        public string OpName
        {
            get { return _OpName; }
            set { _OpName = value; }
        }

        /// <summary>Идентификатор оператора</summary>
        public int OpId
        {
            get { return _OpId; }
            set { _OpId = value; }
        }
        
        /// <summary>Наличие видео</summary>
        public bool HasVideo
        {
            get { return _HasVideo; }
            set { _HasVideo = value; }
        }

        /// <summary>Идентификатор пути осмотра</summary>
        public int WayId
        {
            get { return _WayId; }
            set { _WayId = value; }
        }

        #endregion

    }
}
