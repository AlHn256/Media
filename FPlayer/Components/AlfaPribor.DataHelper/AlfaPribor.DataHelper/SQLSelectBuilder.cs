using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.DataHelper
{
    /// <summary>Упрощает построение SQL-запроса выборки из базы данных</summary>
    public sealed class SqlSelectBuilder
    {
        #region Fields

        /// <summary>Источник выборки данных (таблица{ы}, вид{ы} или объединение{я})</summary>
        private string _From = string.Empty;

        /// <summary>Порядок и тип сортировки выборки данных</summary>
        private string _OrderBy = string.Empty;

        /// <summary>Наименование полей для выборки</summary>
        private string[] _Fields = new string[0];

        /// <summary>Условие выборки данных</summary>
        private string _Where = string.Empty;

        /// <summary>Ограничение числа записей, попавших в выборку</summary>
        private int _Top = 0;

        /// <summary>Признак извлечения только неповторяющихся записей</summary>
        private bool _Distinct = false;

        /// <summary>Символ разделения строк</summary>
        private static string NL = "\n";

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса значениями по умолчанию</summary>
        public SqlSelectBuilder() { }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="from">Источник выборки данных</param>
        /// <param name="fields">Наименование полей для выборки</param>
        /// <exception cref="System.ArgumentException">Не задан источник выборки данных</exception>
        public SqlSelectBuilder(string from, string[] fields)
        {
            From = from;
            Fields = fields;
        }

        /// <param name="from">Источник выборки данных</param>
        /// <param name="fields">Наименование полей для выборки</param>
        /// <param name="top">Ограничение числа записей, попавших в выборку</param>
        /// <exception cref="System.ArgumentException">Не задан источник выборки данных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение аргумента вне допустимого диапазона значений</exception>
        public SqlSelectBuilder(string from, string[] fields, int top)
        {
            From = from;
            Fields = fields;
            Top = top;
        }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="from">Источник выборки данных</param>
        /// <param name="where">Условие выборки данных</param>
        /// <param name="fields">Наименование полей для выборки</param>
        /// <exception cref="System.ArgumentException">Не задан источник выборки данных</exception>
        public SqlSelectBuilder(string from, string where, string[] fields)
        {
            From = from;
            Where = where;
            Fields = fields;
        }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="from">Источник выборки данных</param>
        /// <param name="where">Условие выборки данных</param>
        /// <param name="fields">Наименование полей для выборки</param>
        /// <param name="top">Ограничение числа записей, попавших в выборку</param>
        /// <exception cref="System.ArgumentException">Не задан источник выборки данных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение аргумента вне допустимого диапазона значений</exception>
        public SqlSelectBuilder(string from, string where, string[] fields, int top)
        {
            From = from;
            Where = where;
            Fields = fields;
            Top = top;
        }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="from">Источник выборки данных</param>
        /// <param name="where">Условие выборки данных</param>
        /// <param name="order">Порядок и тип сортировки выборки данных</param>
        /// <param name="fields">Наименование полей для выборки</param>
        /// <exception cref="System.ArgumentException">Не задан источник выборки данных</exception>
        public SqlSelectBuilder(string from, string where, string order, string[] fields)
        {
            From = from;
            Where = where;
            OrderBy = order;
            Fields = fields;
        }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="from">Источник выборки данных</param>
        /// <param name="where">Условие выборки данных</param>
        /// <param name="order">Порядок и тип сортировки выборки данных</param>
        /// <param name="fields">Наименование полей для выборки</param>
        /// <param name="top">Ограничение числа записей, попавших в выборку</param>
        /// <exception cref="System.ArgumentException">Не задан источник выборки данных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение аргумента вне допустимого диапазона значений</exception>
        public SqlSelectBuilder(string from, string where, string order, string[] fields, int top)
        {
            From = from;
            Where = where;
            OrderBy = order;
            Fields = fields;
            Top = top;
        }

        /// <summary>Представляет объект в виде строки запроса к базе данных</summary>
        /// <returns>Строка запроса к серверу базы данных</returns>
        public override string ToString()
        {
            string fields = string.Empty;
            if (_Fields == null || _Fields.Length == 0)
            {
                fields = "*" + NL;
            }
            else
            {
                foreach (var field in _Fields)
                {
                    if (!string.IsNullOrEmpty(fields))
                    {
                        fields += ",";
                    }
                    fields += field + NL;
                }
            }
            string sql = "SELECT" + NL;
            if (_Distinct)
            {
                sql += "DISTINCT" + NL;
            }
            if (_Top > 0)
            {
                sql += "TOP " + _Top.ToString() + NL;
            }
            sql += 
                fields +
                "FROM" + NL +
                _From;
            if (!string.IsNullOrEmpty(_Where))
            {
                sql += 
                    NL + 
                    "WHERE" + NL +
                    _Where;
            }
            if (!string.IsNullOrEmpty(_OrderBy))
            {
                sql +=
                    NL +
                    "ORDER BY" + NL +
                    _OrderBy;
            }
            return sql;
        }

        /// <summary>Оператор приведения типа SqlSelectBuilder в тип string</summary>
        /// <param name="obj">Объект типа SqlSelectBuilder</param>
        /// <returns>Строка запроса к серверу базы данных</returns>
        public static explicit operator string(SqlSelectBuilder obj)
        {
            return obj.ToString();
        }

        #endregion

        #region Properties

        /// <summary>Источник выборки данных (таблица{ы}, вид{ы} или объединение{я})</summary>
        /// <exception cref="System.ArgumentException">Не задан источник выборки данных</exception>
        public string From
        {
            get { return _From; }
            set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("From");
                }
                _From = value; 
            }
        }

        /// <summary>Порядок и тип сортировки выборки данных</summary>
        public string OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value; }
        }

        /// <summary>Наименование полей для выборки</summary>
        /// <remarks>Если не задано - выбираются все поля из источника выборки данных</remarks>
        public string[] Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }

        /// <summary>Условие выборки данных</summary>
        public string Where
        {
            get { return _Where; }
            set { _Where = value; }
        }

        /// <summary>Ограничение числа записей, попавших в выборку</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение аргумента вне допустимого диапазона значений</exception>
        /// <remarks>Серевер возвратит первые Top записей, вошедших в выборку</remarks>
        public int Top
        {
            get { return _Top; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Top");
                }
                _Top = value;
            }
        }

        /// <summary>Признак извлечения только неповторяющихся записей</summary>
        public bool Distinct
        {
            get { return _Distinct;  }
            set { _Distinct = value; }
        }

        #endregion
    }
}
