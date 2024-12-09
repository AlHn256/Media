using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.DataHelper
{
    /// <summary>Упрощает построение SQL-запроса удаления данных из базы данных</summary>
    public sealed class SqlDeleteBuilder
    {
        #region Fields

        /// <summary>Имя таблицы</summary>
        private string _Table = string.Empty;

        /// <summary>Условие выборки данных</summary>
        private string _Where = string.Empty;

        /// <summary>Символ разделения строк</summary>
        private static string NL = "\n";

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса значениями по умолчанию</summary>
        public SqlDeleteBuilder() { }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="table">Имя таблицы</param>
        /// <exception cref="System.ArgumentException">Не задано имя таблицы</exception>
        public SqlDeleteBuilder(string table)
        {
            Table = table;
        }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса заданными значениями</summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="where">Условие выборки данных</param>
        /// <exception cref="System.ArgumentException">Не задано имя таблицы</exception>
        public SqlDeleteBuilder(string table, string where)
        {
            Table = table;
            Where = where;
        }

        /// <summary>Представляет объект в виде строки запроса к базе данных</summary>
        /// <returns>Строка запроса к серверу базы данных</returns>
        public override string ToString()
        {
            string sql =
                "DELETE FROM" + NL +
                _Table;
            if (!string.IsNullOrEmpty(_Where))
            {
                sql +=
                    NL +
                    "WHERE" + NL +
                    _Where;
            }
            return sql;
        }

        /// <summary>Оператор приведения типа SqlDeleteBuilder в тип string</summary>
        /// <param name="obj">Объект типа SqlDeleteBuilder</param>
        /// <returns>Строка запроса к серверу базы данных</returns>
        public static explicit operator string(SqlDeleteBuilder obj)
        {
            return obj.ToString();
        }

        #endregion

        #region Properties

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

        /// <summary>Условие выборки данных</summary>
        public string Where
        {
            get { return _Where; }
            set { _Where = value; }
        }

        #endregion
    }
}
