using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKO.Data
{
    /// <summary>Предоставляет доступ к состояниям датчиков педалей</summary>
    public class SensorState
    {

        int maxIndex = 31;////// !!!!!! переделано из за бит не по порядку
        int minIndex = 0;
        int _StateMask = 0;

        public SensorState()
        {
        }

        /// <summary>Конструктор класса. Инициализирует объект класса ссылкой на данные о состоянии датчиков педалей</summary>
        /// <param name="owner">Объект с данными о состоянии датчиков педалей</param>
        /// <exception cref="System.ArgumentNullException">Не задан владелец объекта</exception>
        public SensorState(int StateMask)
        {
            _StateMask = StateMask;
        }

        /// <summary>Проверяет правильность индекса датчика педали</summary>
        /// <param name="index">Индекс датчика педалей</param>
        /// <exception cref="System.IndexOutOfRangeException">Индекс датчика педалей вне допустимого диапазона значений</exception>
        void CheckSensorIndex(int index)
        {
            if (index < minIndex || index > maxIndex) throw new IndexOutOfRangeException();
        }

        /// <summary>Читает/изменяет состояние отдельного датчика педали</summary>
        /// <param name="i">Индекс датчика педали (0..3, 0 - первый датчик)</param>
        /// <exception cref="System.IndexOutOfRangeException">Индекс датчика педалей вне допустимого диапазона значений</exception>
        /// <returns>Состояние датчика (TRUE - датчик замкнут, FALSE - датчик разомкнут)</returns>
        public bool this[int i]
        {
            get
            {
                CheckSensorIndex(i);
                return ((_StateMask >> i) & 1) != 0;
                //int mask = 0x10000000 << maxIndex - i;
                //return (_StateMask & mask) != 0;
            }
            /*
            set
            {
                CheckSensorIndex(i);
                int mask = 0x10000000 << maxIndex - i;
                if (value)
                {
                    _StateMask |= mask;
                }
                else
                {
                    mask = ~mask;
                    _StateMask &= mask;
                }
            }
            */
        }
        [XmlElement("StateMask")]
        public int StateMask 
        { 
            get { return this._StateMask; }
            set { this._StateMask = value; }
        }

    }

    /// <summary>информацию о состоянии датчиков педалей</summary>
    public class BsSensorsData
    {

        SensorState _Sensor;
        int _TrainId = 0;
		int _TimeSpan = 0;
		int _StateMask = 0;
        //TrainData _TrainData;


        /// <summary>Конструктор объектов класса. Инициализирует объект класса меткой времени и маской состояния датчиков счета</summary>
        public BsSensorsData()
        {
            _TrainId = 0;
            _TimeSpan = 0;
            _StateMask = 0;
            _Sensor = new SensorState(_StateMask);
        }

        /// <summary>Конструктор объектов класса. Инициализирует объект класса меткой времени и маской состояния датчиков счета</summary>
        /// <param name="time_span">Метка времени от начала регистрации поезда, мсек</param>
        /// <param name="state_mask">Маска состояний датчиков счета</param>
        public BsSensorsData(int time_span, int state_mask) 
        {
            _TimeSpan = time_span;
            _StateMask = state_mask;
            _Sensor = new SensorState(_StateMask);
        }

        /// <summary>Конструктор объектов класса. Инициализирует свойства объект заданными значениями</summary>
        /// <param name="train_id">Идентификатор поезда в системе АСКО ПС</param>
        /// <param name="time_span">Метка времени от начала регистрации поезда, мсек</param>
        /// <param name="state_mask">Маска состояний датчиков счета</param>
        public BsSensorsData(int train_id, int time_span, int state_mask)
        {
            _TrainId = train_id;
            _TimeSpan = time_span;
            _StateMask = state_mask;
            _Sensor = new SensorState(_StateMask);
        }

        /// <summary>Состояния датчиков педалей</summary>
        [XmlElement("Sensor")]
        public SensorState Sensor 
        { 
            get { return this._Sensor; }
            set { this._Sensor = value; }
        }

        /// <summary>Идентификатор состава</summary>
        [XmlElement("TrainId")]
        public int TrainId
        {
            get { return this._TrainId; }
            set { this._TrainId = value; }
        }

        [XmlElement("TimeSpan")]
        public int TimeSpan
        {
            get { return this._TimeSpan; }
            set { this._TimeSpan = value; }
        }


    }
}
