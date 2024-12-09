using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{
    /// <summary>Индексы поезда, которые соответствуют его номеру</summary>
   
    public class TrainIndexesData
    {

        List<string> _TrainIndexes = new List<string>();

        /// <summary>Конструктор объектов класса</summary>
        public TrainIndexesData()
        {
            TrainIndexes = new List<string>();
            TrainID = 0;
            TrainNum = string.Empty;
            Contractor = Shared.AskoContractor.Unknown;
        }

        /// <summary>Конструктор объектов класса. Инициализирует свойства объекта начальными значениями</summary>
        /// <param name="num">Номер поезда</param>
        public TrainIndexesData(int id, string num, ASKO.Shared.AskoContractor contractor)
        {
            TrainID = id;
            TrainNum = num;
            Contractor = contractor;
        }

        /// <summary>Индексы поезда</summary>
       
        public List<string> TrainIndexes 
        {
            get { return _TrainIndexes; }
            set
            {
                if (value == null) _TrainIndexes = new List<string>();
                else _TrainIndexes = value;
            }
        }

        /// <summary>Идентификатор поезда</summary>
       
        public int TrainID { get; set; }

        /// <summary>Номер поезда</summary>
       
        public string TrainNum { get; set; }

        /// <summary>АСУ</summary>
       
        public ASKO.Shared.AskoContractor Contractor { get; set; }

    }
}
