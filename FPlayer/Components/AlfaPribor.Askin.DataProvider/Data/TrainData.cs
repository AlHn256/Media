using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKIN.Data
{

    /// <summary>Элемент списка поездов</summary>
    public class TrainData
    {

        /// <summary>Идентификатор поезда в базе данных</summary>
        int _Id = 0;
        /// <summary>Пункт считывания</summary>
        int _Point = 0;
        /// <summary>Дата время начала записи</summary>
        DateTime _BeginTime = DateTime.MinValue;
        /// <summary>Дата время окончания записи</summary>
        DateTime _EndTime = DateTime.MinValue;
        /// <summary>Направление (0 - прямое, 1 - обратное)</summary>
        int _Direction = 0;
        /// <summary>Идентификатор каталога видеозаписи</summary>
        int _DirId = 0;
        /// <summary>Путь каталога видеозаписи</summary>
        string _DirPath = string.Empty;
        /// <summary>Число вагонов (общее)</summary>
        int _WagonsCount = 0;
        /// <summary>Число локомотивов</summary>
        int _LocoCount = 0;
        /// <summary>Номер поезда</summary>
        string _TrainNum;
        /// <summary>Индекс поезда</summary>
        string _TrainIndex;
        /// <summary>Статус поезда</summary>
        int _Status;
        /// <summary>Скорость поезда</summary>
        int _Speed;
        /// <summary>Виртуальный "склеенный" состав</summary>
        bool _Virtual = false;

        /// <summary>Идентификаторы реальных составов из которых поезд был собран</summary>
        string _VirtualTrainID = string.Empty;

        /// <summary>Идентификатор поезда</summary>
        [XmlElement("id")]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Пункт считывания</summary>
        [XmlElement("point")]
        public int RecId
        {
            get { return _Point; }
            set { _Point = value; }
        }

        /// <summary>Строка дата и время начала записи</summary>
        [XmlElement("begin")]
        public string RecordBegin
        {
            get
            {
                string result = string.Empty;
                if (_BeginTime != DateTime.MinValue)
                {
                    try
                    {
                        result = _BeginTime.ToString(GetDateTimeConvFormat());
                    }
                    catch (Exception) { }
                }
                return result;
            }
            set
            {
                _BeginTime = DateTime.MinValue;
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        _BeginTime = Convert.ToDateTime(value);
                    }
                    catch { }
                }
            }
        }

        /// <summary>Строка дата и время окончания записи</summary>
        [XmlElement("end")]
        public string RecordEnd
        {
            get
            {
                string result = string.Empty;
                if (_EndTime != DateTime.MinValue)
                {
                    try
                    {
                        result = _EndTime.ToString(GetDateTimeConvFormat());
                    }
                    catch (Exception) { }
                }
                return result;
            }
            set
            {
                _EndTime = DateTime.MinValue;
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        _EndTime = Convert.ToDateTime(value);
                    }
                    catch { }
                }
            }
        }

        /// <summary>Направление (0|1)</summary>
        [XmlElement("direction")]
        public int Direction
        {
            get { return _Direction; }
            set 
            { 
                _Direction = value != 0 ? 1 : 0; 
            }
        }

        /// <summary>Идентификатор каталога видеоархива</summary>
        [XmlIgnore]
        public int DirId
        {
            get { return _DirId; }
            set { _DirId = value; }
        }

        /// <summary>Номер поезда</summary>
        [XmlElement("trainnum")]
        public string TrainNum
        {
            get { return _TrainNum; }
            set { _TrainNum = value; }
        }

        /// <summary>Индекс поезда</summary>
        [XmlElement("trainindex")]
        public string TrainIndex
        {
            get { return _TrainIndex; }
            set { _TrainIndex = value; }
        }

        /// <summary>Статус передачи информации в АСУ СТ</summary>
        [XmlElement("status")]
        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        
        /// <summary>Путь каталога видеоархива</summary>
        [XmlIgnore]
        public string DirPath
        {
            get { return _DirPath; }
            set { _DirPath = value; }
        }

        /// <summary>Дата и время начала записи</summary>
        [XmlElement("start")]
        public DateTime BeginTime
        {
            get { return _BeginTime; }
            set 
            {
                // Округляем до миллисекунд
                // Ограничение связано с форматом хранения значения в базе данных
                _BeginTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, 
                                          value.Minute, value.Second, value.Millisecond);
            }
        }

        /// <summary>Дата и время окончания записи</summary>
        [XmlElement("stop")]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set 
            {
                // Округляем до миллисекунд
                // Ограничение связано с форматом хранения значения в базе данных
                _EndTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, 
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

        /// <summary>Количество вагонов без локомотивов</summary>
        [XmlElement("pure_wagons_count")]
        public int PureWagonsCount
        {
            get { return _WagonsCount - _LocoCount; }
            set { _LocoCount = _WagonsCount - value; }
        }

        /// <summary>Количество локомотивов</summary>
        [XmlElement("loco_count")]
        public int LocoCount
        {
            get { return _LocoCount; }
            set { _LocoCount = value; }
        }

        /// <summary>Количество локомотивов</summary>
        [XmlElement("speed")]
        public int Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        /// <summary>Виртуальный "склеенный" состав</summary>
        [XmlElement("virtual")]
        public bool Virtual
        {
            get { return _Virtual; }
            set { _Virtual = value; }
        }

        //Реальные составы из которых был склеен поезд
        [XmlElement("virtualTrainIDList")]
        public string VirtualTrainIDList
        {
            get { return _VirtualTrainID; }
            set { _VirtualTrainID = value; }
        }

        /// <summary>Конструктор</summary>
        public TrainData()
        {
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор поезда в СБВ УВГ</param>
        /// <param name="begin">Дата и время начала записи</param>
        /// <param name="end">Дата и время окончания записи</param>
        /// <param name="speed">Скорость (км/ч)</param>
        /// <param name="direction">Направление</param>
        /// <param name="dir_id">Идентификатор каталога записи</param>
        public TrainData(int id, int point, DateTime begin, DateTime end, int direction, int dir_id)
        {
            _Id = id;
            _Point = point;
            _BeginTime = begin;
            _EndTime = end;
            Direction = direction;
            _DirId = dir_id;
            _Virtual = false;
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор поезда в СБВ УВГ</param>
        /// <param name="begin">Дата и время начала записи</param>
        /// <param name="end">Дата и время окончания записи</param>
        /// <param name="speed">Скорость (км/ч)</param>
        /// <param name="direction">Направление</param>
        /// <param name="dir_id">Идентификатор каталога записи</param>
        public TrainData(DateTime begin, DateTime end, int direction, int status, bool virt, int wagon_count)
        {
            _BeginTime = begin;
            _EndTime = end;
            Direction = direction;
            _Virtual = virt;
            _Status = status;
            _WagonsCount = wagon_count;
        }

        /// <summary>Предоставляет формат представления даты/времени в виде строки</summary>
        /// <returns>Формат представления даты/времени в виде строки</returns>
        protected virtual string GetDateTimeConvFormat()
        {
            return "dd.MM.yyyy HH:mm";
        }

    }

}
