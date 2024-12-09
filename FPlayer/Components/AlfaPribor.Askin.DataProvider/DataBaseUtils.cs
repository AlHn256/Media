using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Management;
using Microsoft.Win32;

using AlfaPribor.ASKIN.DataProvider;

namespace AlfaPribor.ASKIN.DataBase
{

    public class DataBaseUtils
    {

        #region Variables

        //static string db_name = "askin";
        //static string sqlInstance = ".\\SQLEXPRESS";
        static string db_name = "";
        static string default_db_name = "askin";
        static string sqlInstance = "";
        static string db_login = "";
        static string db_password = "";
        static bool uselogin = false;

        #endregion

        #region Параметры подключения к базе данных

        /// <summary>Экземпляр сервера</summary>
        public static string SqlInstance
        {
            set { sqlInstance = value; }
            get { return sqlInstance; }
        }

        /// <summary>База данных</summary>
        public static string DataBaseName
        {
            set { db_name = value; }
            get { return db_name; }
        }

        /// <summary>Имя базы данных по-умолчанию</summary>
        public static string DefaultDataBaseName
        {
            set { default_db_name = value; }
            get { return default_db_name; }
        }

        /// <summary>Использовать логин базы данных</summary>
        public static bool UseLogin
        {
            set { uselogin = value; }
            get { return uselogin; }
        }

        /// <summary>Логин базы данных</summary>
        public static string Login
        {
            set { db_login = value; }
            get { return db_login; }
        }

        /// <summary>Пароль базы данных</summary>
        public static string Password
        {
            set { db_password = value; }
            get { return db_password; }
        }

        /// <summary>Строка подключения к базе данных</summary>
        public static string ConnectionString
        {
            get
            {
                string s = "Data Source=" + sqlInstance + ";" + "DataBase=" + db_name;
                if (uselogin) s += ";User ID=" + db_login + ";Password=" + db_password;
                //s += ";User ID=iis_user;Password=1";
                else s += ";Persist Security Info=False;Integrated Security=SSPI";
                return s;
            }
        }

        #endregion
        
        /// <summary>Получение подключения к базе</summary>
        /// <returns>Новое подключение к базе</returns>
        static SqlConnection GetDataBaseConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            try { connection.Open(); }
            catch { return null; }
            return connection;
        }

        /// <summary>Выполнение команды с заполнением таблицы</summary>
        /// <param name="sSQL">Команда</param>
        /// <returns>Dataset</returns>
        static DataTable LoadDataTable(string sSQL)
        {
            SqlConnection connection = GetDataBaseConnection();
            if (connection == null) return null;
            string s = sSQL;
            SqlCommand Command = new SqlCommand(s, connection);
            SqlDataAdapter Adapter = new SqlDataAdapter();
            Adapter.SelectCommand = Command;
            DataTable MyTable = new DataTable();
            try
            {
                Adapter.Fill(MyTable);
            }
            catch { return MyTable; }
            connection.Close();
            return MyTable;
        }

        /// <summary>Получение текущего идентификатора таблицы (последней вставленной строки)</summary>
        /// <param name="table">Имя таблицы</param>
        /// <returns>Максимальное значение</returns>
        public static int GetCurrentIdent(string table)
        {
            string sql = "SELECT IDENT_CURRENT('" + table + "')";
            DataTable Table = LoadDataTable(sql);
            if (Table.Rows.Count == 0) return 0;
            return System.Convert.ToInt32(Table.Rows[0][0]);
        }
        
    }

}
