using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Данные о прогнозируемом проходе поезда</summary>
   
    public class TrainForecastData : IEquatable<TrainForecastData>, ICloneable
    {

        /// <summary>Конструктор объектов класса</summary>
        public TrainForecastData()
        {
            TrainNum = string.Empty;
            TrainIndex = string.Empty;
            TrainDateTime = DateTime.MinValue;
        }

        /// <summary>Конструктор объектов класса. Инициализирует свойства объекта класса начальными значениями</summary>
        /// <param name="num">Номер поезда</param>
        /// <param name="index">Индекс поезда</param>
        /// <param name="date">Дата/время прибытия поезда</param>
        public TrainForecastData(string num, string index, DateTime date)
        {
            TrainNum = num;
            TrainIndex = index;
            TrainDateTime = date;
        }

        /// <summary>Дата и время начала прохождения</summary>
       
        public DateTime TrainDateTime { get; set; }

        /// <summary>Номер поезда</summary>
       
        public string TrainNum { get; set; }

        /// <summary>Индекс поезда</summary>
       
        public string TrainIndex { get; set; }

        #region IEquatable<TrainForecastData> members

        /// <summary>Проверяет равенство текущего объекта заданному</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты равны, FALSE - в противном случае</returns>
        public bool Equals(TrainForecastData other)
        {
            if (other == null) { return false; }
            return  (TrainDateTime == other.TrainDateTime) &&
                    (TrainNum == other.TrainNum) &&
                    (TrainIndex == other.TrainIndex);
        }

        #endregion

        #region Члены ICloneable

        /// <summary>Клонировать</summary>
        /// <returns></returns>
        public object Clone()
        {
            return new TrainForecastData(TrainNum, TrainIndex, TrainDateTime);
        }

        #endregion

    }
}
