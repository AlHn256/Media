using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKIN.Data
{
    
    /// <summary>Направление движения поезда относительно опоры СВ (Смотровой Вышки)</summary>
    public enum TrainDirection
    {
        /// <summary>Неизвестно</summary>
        Unknown = -1,
        /// <summary>Состав вошел в зону контроля слева и вышел вправо</summary>
        LeftToRight = 0,
        /// <summary>Состав вошел в зону контроля слева и, после остановки перед опорой СВ, вернулся обратно</summary>
        LeftToLeft = 1,
        /// <summary>Состав вошел в зону контроля справа и вышел влево</summary>
        RightToLeft = 2,
        /// <summary>Состав вошел в зону контроля справа и, после остановки перед опорой СВ, вернулся обратно</summary>
        RightToRight = 3
    }

    /// <summary>Значения переменных блока согласования в текущий момент времени</summary>
    public class BsVariablesData
    {

        int _TrainId = 0;
        int _TimeSpan = 0;
        int _Sensor1 = 0;
        int _Sensor2 = 0;
        int _WagNum = 0;
        int _CurrentDirection = 0;

        public BsVariablesData()
        {
            _TimeSpan = 0;
            _Sensor1 = 0;
            _Sensor2 = 0;
            _WagNum = 0;
            _CurrentDirection = 0;
        }

        /// <summary>Конструктор объектов класса. Инициализирует свойства класса заданными значениями</summary>
        //public BsVariablesData()
        //{
        //}

        /// <summary>Конструктор объектов класса. Инициализирует свойства класса заданными значениями</summary>
        /// <param name="time_span">Метка времени от начала регистрации поезда, мсек</param>
        /// <param name="sensor1">Значение 1-го датчика счета</param>
        /// <param name="sensor2">Значение 2-го датчика счета</param>
        /// <param name="wag_num">Номер вагона в составе</param>
        /// <param name="direction">Направление движения вагона</param>
        public BsVariablesData(int time_span, int sensor1, int sensor2, int wag_num, int direction)
        {
            _TimeSpan = time_span;
            _Sensor1 = sensor1;
            _Sensor2 = sensor2;
            _WagNum = wag_num;
            _CurrentDirection = direction;
        }

        /// <summary>Конструктор объектов класса. Инициализирует свойства класса заданными значениями</summary>
        /// <param name="train_id">Идентификатор поезда в системе АСКО СВ</param>
        /// <param name="time_span">Метка времени от начала регистрации поезда, мсек</param>
        /// <param name="sensor1">Значение 1-го датчика счета</param>
        /// <param name="sensor2">Значение 2-го датчика счета</param>
        /// <param name="wag_num">Номер вагона в составе</param>
        /// <param name="direction">Направление движения вагона</param>
        public BsVariablesData(int train_id, int time_span, int sensor1, int sensor2, int wag_num, int direction)
        {
            _TrainId = train_id;
            _TimeSpan = time_span;
            _Sensor1 = sensor1;
            _Sensor2 = sensor2;
            _WagNum = wag_num;
            _CurrentDirection = direction;
        }

        /// <summary>Направление прохождения вагона</summary>
        [XmlElement("Direction")]
        public int Direction
        { 
            get { return _CurrentDirection; }
            set { _CurrentDirection = value; } 
        }

        /// <summary>Идентификатор поезда</summary>
        [XmlElement("TrainId")]
        public int TrainId  
        { 
            get { return _TrainId; }
            set { _TrainId = value; }
        }

        /// <summary>Метка времени от начала состава</summary>
        [XmlElement("TimeSpan")]
        public int TimeSpan 
        { 
            get { return _TimeSpan; }
            set { _TimeSpan = value; }
        }

        /// <summary>Значение педали 1</summary>
        [XmlElement("Sensor1")]
        public int Sensor1 
        { 
            get { return _Sensor1; }
            set { _Sensor1 = value; }
        }

        /// <summary>Значение педали 2</summary>
        [XmlElement("Sensor2")]
        public int Sensor2 
        { 
            get { return _Sensor2; }
            set { _Sensor2 = value; }
        }

        /// <summary>Номер вагона в составе</summary>
        [XmlElement("WagNum")]
        public int WagNum 
        { 
            get { return _WagNum; }
            set { _WagNum = value; }
        }
        
    }
}
