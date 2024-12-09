using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKIN.Data
{

    /// <summary>Элемент списка натурных листов по прибытию</summary>
    public class ArrivalTrain
    {

        /// <summary>Идентификатор поезда в базе данных</summary>
        int _Id = 0;
        /// <summary>Ориентировочное дата и время прибытия</summary>
        DateTime _Date = DateTime.MinValue;
        /// <summary>Направление списывания (списан с головы 1, с хвоста 2)</summary>
        int _Feature = 1;
        /// <summary>Идентификатор каталога видеозаписи</summary>
        int _DirId = 0;
        /// <summary>Число вагонов (общее)</summary>
        int _WagonsCount = 0;
        /// <summary>Код направления</summary>
        string _CodeDirection;
        /// <summary>Код станции</summary>
        string _CodeStation;
        /// <summary>Номер поезда</summary>
        string _TrainNum;
        /// <summary>Индекс поезда</summary>
        string _TrainIndex;

        /// <summary>Идентификатор поезда</summary>
        [XmlElement("id")]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Направление списывания (1|2)</summary>
        [XmlElement("feature")]
        public int Feature
        {
            get { return _Feature; }
            set 
            { 
                _Feature = value != 0 ? 2 : 1; 
            }
        }
        
        /// <summary>Код направления</summary>
        [XmlElement("code_direction")]
        public string CodeDirection
        {
            get { return _CodeDirection; }
            set { _CodeDirection = value; }
        }

        /// <summary>Код станции</summary>
        [XmlElement("code_station")]
        public string CodeStation
        {
            get { return _CodeStation; }
            set { _CodeStation = value; }
        }
        
        /// <summary>Номер поезда</summary>
        [XmlElement("number")]
        public string TrainNum
        {
            get { return _TrainNum; }
            set { _TrainNum = value; }
        }

        /// <summary>Индекс поезда</summary>
        [XmlElement("index")]
        public string TrainIndex
        {
            get { return _TrainIndex; }
            set { _TrainIndex = value; }
        }

        /// <summary>Ориентировочное дата и время прибытия</summary>
        [XmlElement("date")]
        public DateTime Date
        {
            get { return _Date; }
            set 
            {
                // Округляем до миллисекунд
                // Ограничение связано с форматом хранения значения в базе данных
                _Date = new DateTime(value.Year, value.Month, value.Day, value.Hour, 
                                          value.Minute, value.Second, value.Millisecond);
            }
        }

        /// <summary>Количество вагонов(общее)</summary>
        [XmlElement("wagons_count")]
        public int WagonsCount
        {
            get { return _WagonsCount; }
            set { _WagonsCount = value; }
        }

        /// <summary>Конструктор</summary>
        public ArrivalTrain()
        {
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор поезда в СБВ УВГ</param>
        /// <param name="date">Дата и время начала записи</param>
        /// <param name="end">Дата и время окончания записи</param>
        /// <param name="speed">Скорость (км/ч)</param>
        /// <param name="direction">Направление</param>
        /// <param name="dir_id">Идентификатор каталога записи</param>
        public ArrivalTrain(int id, int point, DateTime date, int feature, 
                            string code_station, string code_direction, 
                            string numder, string index)
        {
            _Id = id;
            _Date = date;
            _Feature = feature;
            _CodeDirection = code_direction;
            _CodeStation = code_station;
            _TrainNum = numder;
            _TrainIndex = index;
        }

        /// <summary>Предоставляет формат представления даты/времени в виде строки</summary>
        /// <returns>Формат представления даты/времени в виде строки</returns>
        protected virtual string GetDateTimeConvFormat()
        {
            return "dd.MM.yyyy HH:mm";
        }

    }

}
