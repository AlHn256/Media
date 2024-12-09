using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Содержит инвентарные номера вагонов поезда</summary>
    public class NatListData : TrainForecastData, IEquatable<NatListData>
    {
        
        List<string> _WagInvNums = new List<string>();
        bool awaiting = false;

        /// <summary>Конструктор объектов класса</summary>
        public NatListData() { }

        /// <summary>Конструктор объектов класса. Инициализирует свойства объекта класса начальными значениями</summary>
        /// <param name="num">Номер поезда</param>
        /// <param name="index">Индекс поезда</param>
        /// <param name="date">Дата/время прибытия поезда</param>
        public NatListData(string num, string index, DateTime date) : base(num, index, date) { }

        /// <summary>Инвентарные номера вагонов</summary>
        public List<string> WagInvNums
        {
            get { return _WagInvNums; }
            set
            {
                if (value == null) _WagInvNums = new List<string>();
                else _WagInvNums = value;
            }
        }

        /// <summary>Натурный лист ожидаемого состава</summary>
        public bool Awaiting
        {
            get { return awaiting; }
            set { awaiting = value; }
        }

        #region IEquatable<NatListData> members

        /// <summary>Проверяет равенство текущего объекта заданному</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты равны, FALSE - в противном случае</returns>
        public bool Equals(NatListData other)
        {
            bool result = base.Equals(other);
            if (result)
            {
                IEnumerable<string> exceptList1 = _WagInvNums.Except(other._WagInvNums);
                IEnumerable<string> exceptList2 = other._WagInvNums.Except(_WagInvNums);
                result = (exceptList1.Count() == 0) && (exceptList2.Count() == 0);
            }
            return result;
        }

        #endregion

    }

    /// <summary>Содержит инвентарные номера вагонов поезда</summary>

    public class NatListModels : TrainForecastData, IEquatable<NatListModels>
    {

        List<string> _WagInvNums = new List<string>();
        List<string> _WagModels = new List<string>();
        List<int> _WagGabarit = new List<int>();
        bool awaiting = false;

        /// <summary>Конструктор объектов класса</summary>
        public NatListModels() { }

        /// <summary>Конструктор объектов класса. Инициализирует свойства объекта класса начальными значениями</summary>
        /// <param name="num">Номер поезда</param>
        /// <param name="index">Индекс поезда</param>
        /// <param name="date">Дата/время прибытия поезда</param>
        public NatListModels(string num, string index, DateTime date) : base(num, index, date) { }

        /// <summary>Инвентарные номера вагонов</summary>
        public List<string> WagInvNums
        {
            get { return _WagInvNums; }
            set
            {
                if (value == null) _WagInvNums = new List<string>();
                else _WagInvNums = value;
            }
        }
        
        /// <summary>Модели вагонов</summary>
        public List<string> WagModels
        {
            get { return _WagModels; }
            set
            {
                if (value == null) _WagModels = new List<string>();
                else _WagModels = value;
            }
        }
        
        /// <summary>Габарит подвижного состава вагонов</summary>
        public List<int> WagGabarit
        {
            get { return _WagGabarit; }
            set
            {
                if (value == null) _WagGabarit = new List<int>();
                else _WagGabarit = value;
            }
        }
        
        /// <summary>Натурный лист ожидаемого состава</summary>
        public bool Awaiting
        {
            get { return awaiting; }
            set { awaiting = value; }
        }

        #region IEquatable<NatListData> members

        /// <summary>Проверяет равенство текущего объекта заданному</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты равны, FALSE - в противном случае</returns>
        public bool Equals(NatListModels other)
        {
            bool result = base.Equals(other);
            if (result)
            {
                //Инвентарные номера вагонов
                IEnumerable<string> exceptList1 = _WagInvNums.Except(other._WagInvNums);
                IEnumerable<string> exceptList2 = other._WagInvNums.Except(_WagInvNums);
                result = (exceptList1.Count() == 0) && (exceptList2.Count() == 0);
            }
            return result;
        }

        #endregion
    }

}
