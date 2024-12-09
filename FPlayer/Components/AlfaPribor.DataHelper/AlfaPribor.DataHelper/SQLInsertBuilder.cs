using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.DataHelper
{
    /// <summary>Упрощает построение SQL-запроса занесания значений в базу данных</summary>
    public sealed class SqlInsertBuilder
    {
        #region Fields

        /// <summary>Наименование полей для вставки значений</summary>
        private string[] _Fields = new string[0];

        /// <summary>Имя таблицы</summary>
        private string _Table = string.Empty;

        /// <summary>Значения полей</summary>
        private string[] _Values = new string[0];

        /// <summary>Символ разделения строк</summary>
        private static string NL = "\n";

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса значениями по умолчанию</summary>
        public SqlInsertBuilder() { }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="fields">Наименование полей для вставки значений</param>
        /// <param name="values">Значения полей</param>
        /// <exception cref="System.ArgumentException">Не задано значение аргумента</exception>
        public SqlInsertBuilder(string table, string[] fields, string[] values)
        {
            Table = table;
            Fields = fields;
            Values = values;
        }

        /// <summary>Представляет объект в виде строки запроса к базе данных</summary>
        public override string ToString()
        {
            string fields = string.Empty;
            foreach (var field in _Fields)
            {
                if (!string.IsNullOrEmpty(fields))
                {
                    fields += ",";
                }
                fields += field + NL;
            }
            int index = fields.LastIndexOf(NL);
            if (index >= 0)
            {
                fields = fields.Substring(0, index);
            }
            string values = string.Empty;
            foreach (var value in _Values)
            {
                if (!string.IsNullOrEmpty(values))
                {
                    values += ",";
                }
                values += value + NL;
            }
            index = values.LastIndexOf(NL);
            if (index >= 0)
            {
                values = values.Substring(0, index);
            }
            string sql =
                "INSERT INTO " + _Table + NL +
                "(" + fields + ")" + NL +
                "VALUES" + NL +
                "(" + values + ")";
            return sql;
        }

        /// <summary>Оператор приведения типа SqlInsertBuilder в тип string</summary>
        /// <param name="obj">Объект типа SqlInsertBuilder</param>
        /// <returns>Строка запроса к серверу базы данных</returns>
        public static explicit operator string(SqlInsertBuilder obj)
        {
            return obj.ToString();
        }

        #endregion

        #region Properties

        /// <summary>Наименование полей для вставки значений</summary>
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

        #endregion
    }
}
