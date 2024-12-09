using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.DataHelper
{
    /// <summary>Упрощает построение SQL-запроса изменения данных в базе данных</summary>
    public sealed class SqlUpdateBuilder
    {
        #region Fields

        /// <summary>Имя таблицы</summary>
        private string _Table = string.Empty;

        /// <summary>Наименование полей для выборки</summary>
        private string[] _Fields = new string[0];

        /// <summary>Условие выборки данных</summary>
        private string _Where = string.Empty;

        /// <summary>Значения полей</summary>
        private string[] _Values = new string[0];

        /// <summary>Символ разделения строк</summary>
        private static string NL = "\n";

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса значениями по умолчанию</summary>
        public SqlUpdateBuilder() { }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="fields">Наименование полей для изменения значений</param>
        /// <param name="values">Значения полей</param>
        /// <exception cref="System.ArgumentException">Не задано значение аргумента</exception>
        public SqlUpdateBuilder(string table, string[] fields, string[] values)
        {
            Table = table;
            Fields = fields;
            Values = values;
        }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="fields">Наименование полей для изменения значений</param>
        /// <param name="values">Значения полей</param>
        /// <param name="where">Условие выборки данных</param>
        /// <exception cref="System.ArgumentException">Не задано значение аргумента</exception>
        public SqlUpdateBuilder(string table, string[] fields, string[] values, string where)
        {
            Table = table;
            Fields = fields;
            Values = values;
            Where = where;
        }

        /// <summary>Представляет объект в виде строки запроса к базе данных</summary>
        /// <returns>Строка запроса к серверу базы данных</returns>
        public override string ToString()
        {
            string pairs = string.Empty;
            int count = _Fields.Length;
            if (_Values.Length < count)
            {
                count = _Values.Length;
            }
            for (int i = 0; i < count; ++i )
            {
                if (string.IsNullOrEmpty(_Fields[i]) || string.IsNullOrEmpty(_Values[i]))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(pairs))
                {
                    pairs += ",";
                }
                pairs += _Fields[i] + " = " + _Values[i] + NL;
            }
            string sql =
                "UPDATE" + NL +
                _Table + NL +
                "SET" + NL +
                pairs;
            if (!string.IsNullOrEmpty(_Where))
            {
                sql +=
                    "WHERE" + NL +
                    _Where;
            }
            return sql;
        }

        /// <summary>Оператор приведения типа SqlUpdateBuilder в тип string</summary>
        /// <param name="obj">Объект типа SqlUpdateBuilder</param>
        /// <returns>Строка запроса к серверу базы данных</returns>
        public static explicit operator string(SqlUpdateBuilder obj)
        {
            return obj.ToString();
        }

        #endregion

        #region Properties

        /// <summary>Наименование полей для изменения значений</summary>
        /// <exception cref="System.ArgumentException">Не заданы поля для вставки значений</exception>
        public string[] Fields
        {
            get { return _Fields; }
            set
            {
                if (value == null || value.Length == 0)
                {
                    throw new ArgumentException("Fields");
                }
                _Fields = value;
            }
        }

        /// <summary>Имя таблицы</summary>
        /// <exception cref="System.ArgumentException">Не задано имя таблицы</exception>
        public string Table
        {
            get { return _Table; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Table");
                }
                _Table = value;
            }
        }

        /// <summary>Значения полей</summary>
        /// <exception cref="System.ArgumentException">Не заданы значения полей таблицы</exception>
        public string[] Values
        {
            get { return _Values; }
            set
            {
                if (value == null || value.Length == 0)
                {
                    throw new ArgumentException("Values");
                }
                _Values = value;
            }
        }

        /// <summary>Условие выборки данных</summary>
        public string Where
        {
            get { return _Where; }
            set { _Where = value; }
        }

        #endregion
    }
}
