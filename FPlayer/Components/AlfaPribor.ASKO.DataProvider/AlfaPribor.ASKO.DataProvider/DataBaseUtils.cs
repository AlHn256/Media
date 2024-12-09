using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Management;
using Microsoft.Win32;

using AlfaPribor.ASKO.DataProvider;

namespace AlfaPribor.ASKO.DataBase
{

    /// <summary>Класс работы с базой данных</summary>
    public class DataBaseUtils
    {

        public enum InstallDBStatus
        {
            IncloderctLogicName =-6,
            DublicateDBName = -3,
            ServerNotAvailible = -2,
            UserAbort = -1,
            Success = 0
        }

        #region Variables

        static string db_name = "asko";
        static string sqlInstance = ".\\SQLEXPRESS";
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

        public static bool IsSetConnectionString { get; set; }

        public static bool IsEmptyConnectionString()
        {
            return uselogin ? string.IsNullOrEmpty(sqlInstance) || string.IsNullOrEmpty(db_name) || string.IsNullOrEmpty(db_login) || string.IsNullOrEmpty(db_password) : string.IsNullOrEmpty(sqlInstance) || string.IsNullOrEmpty(db_name);
        }
       
        public static void SetMasterConnectionString(string instanceName, bool autorization, string dbLogin, string dbPassword)
        {
            sqlInstance = instanceName;
            db_name = "master";
            if (autorization)
            {
                db_login = dbLogin;
                db_password = dbPassword; 
            }
        }

        /// <summary>Строка подключения к базе данных</summary>
        public static string ConnectionString
        {
            get
            {
                string s = "Data Source=" + sqlInstance + ";" + "DataBase=" + db_name;
                if (uselogin) s += ";User ID=" + db_login + ";Password=" + db_password;
                else s += ";Persist Security Info=False;Integrated Security=SSPI";
                return s;
            }
        }

        /// <summary>Создать строку подключегния к базе данных</summary>
        /// <param name="instance">Экземпляр сервера</param>
        /// <param name="db">Название базы данных</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        public static string CreateConnectionString(string instance, string db, string login, string password)
        {
            string s = "Data Source=" + instance + ";" + "DataBase=" + db;
            //Авторизация
            if (login != "") s += ";User ID=" + login + ";Password=" + password;
            //Нет авторизации
            else s += ";Persist Security Info=False;Integrated Security=SSPI";
            return s;
        }

        /// <summary>Создать строку подключегния к базе Master</summary>
        /// <param name="instance">Экземпляр сервера</param>
        /// <param name="db">Название базы данных</param>
        /// <param name="login">Логин (не указано - без авторизации)</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        public static string CreateMasterConnectionString(string instance, string login, string password)
        {
            //Авторизация
            if (login != "") return "SERVER=" + instance + ";DATABASE=master;User ID=" + login + ";Password=" + password;
            //Нет авторизации
            else return "SERVER = " + instance + "; DATABASE=master;Persist Security Info=False;Integrated Security=SSPI";
        }
        
        #endregion

        /// <summary>Получение экземпляра сервера по умолчанию</summary>
        /// <returns>Экземпляр сервера по умолчанию</returns>
        public static string[] GetLocalServers()
        {
            //Список экземпляров из реестра
            string[] instancesTmp = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server", "InstalledInstances", null) as string[];
            List<string> instances = new List<string>();
            if (instancesTmp == null)
            {
                instances.Add(".\\SQLEXPRESS");
                return instances.ToArray();
            }
            if (instancesTmp.Length > 0)
            {
                instances.Add(@".\" + instancesTmp[0]);
                for (int i = 0; i < instancesTmp.Length; i++)
                {
                    instances.Add(Environment.MachineName + "\\" + instancesTmp[i]);
                }
            }
            else
            {
                instances.Add(".\\SQLEXPRESS");
            }
            return instances.ToArray();
        }

        /// <summary>Выполнение команды с указанным подключением</summary>
        /// <param name="sql">Строка команды</param>
        /// <param name="connection">Подключение к базе данных</param>
        /// <param name="error">Описание ошибки в случае невыполнения команды</param>
        /// <returns>Результат выполнения</returns>
        public static bool ExecuteCommand(string sql, SqlConnection connection, ref string error)
        {
            if (connection == null) return false;
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                error = e.Source + " " + e.Message;
                return false;
            }
        }

        /// <summary>Получение подключения к базе</summary>
        /// <returns>Новое подключение к базе</returns>
        static SqlConnection GetDataBaseConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            try { connection.Open(); }
            catch { return null; }
            return connection;
        }

        /// <summary>Получение подключения к базе</summary>
        /// <returns>Новое подключение к базе</returns>
        static SqlConnection GetDataBaseConnection(string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
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

        /// <summary>Выполнение команды с заполнением таблицы</summary>
        /// <param name="sSQL">Команда</param>
        /// <param name="connectionString">Строка подключения</param>
        /// <param name="error">Описание ошибки в случае невыполнения команды</param>
        /// <returns>Dataset</returns>
        static DataTable LoadDataTable(string sSQL, string connectionString, ref string error)
        {
            SqlConnection connection = GetDataBaseConnection(connectionString);
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
            catch (Exception e)
            {
                error = e.Source + " " + e.Message;
                return MyTable;
            }
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

        /// <summary>Проверка подключения к базе</summary>
        /// <returns>Результат проверки</returns>
        public static bool CheckConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            bool connect = false;
            try { connection.Open(); } catch { }
            connect = (connection.State == ConnectionState.Open);
            try { connection.Close(); } catch { }
            return connect;
        }

        /// <summary>Проверка подключения к базе</summary>
        /// <param name="instance">Экземпляр сервера</param>
        /// <param name="db_name">Название базы</param>
        /// <param name="user">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Результат проверки</returns>
        public static bool CheckConnection(string instance, string db_name, string user, string password)
        {
            string conn_str = "Data Source=" + instance + ";" + "DataBase=" + db_name;
            if (user != "") conn_str += ";User ID=" + user + ";Password=" + password;
            else conn_str += ";Persist Security Info=False;Integrated Security=SSPI";
            SqlConnection connection = new SqlConnection(conn_str);
            bool connect = false;
            try { connection.Open(); } catch { }
            connect = (connection.State == ConnectionState.Open);
            try { connection.Close(); } catch { }
            return connect;
        }

        /// <summary>Проверка подключения к MS SQL базе master</summary>
        /// <param name="instance">Экземпляр сервера</param>
        /// <param name="user">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Результат проверки</returns>
        public static bool CheckMasterConnection(string instance, string user, string password)
        {
            //Поключение к серверу (master)
            SqlConnection connection = new SqlConnection(CreateMasterConnectionString(instance, user, password));
            try { connection.Open(); }
            catch { return false; }
            return true;
        }

        #region Install Database

        /// <summary>Создание базы данных по-умолчанию</summary>
        /// <param name="dbname">Имя базы данных</param>
        /// <param name="instance">Имя сервера</param>
        /// <returns>Результат операции</returns>
        public static int InstallDataBase(string path, string dbname, string instance,
                                          string user_id, string password,
                                          string script_tables, string script_data, ref string error)
        {
            //Проверка наличия такой же базы на сервере
            if (CheckDB(dbname, instance)) return -3;
            //Создание базы
            return CreateDatabase(instance, dbname, path, user_id, password, script_tables, script_data, ref error);
        }

        /// <summary>Проверка наличия на сервере базы данных с указанным имененм</summary>
        /// <param name="name">Имя базы данных</param>
        /// <param name="server">Сервер</param>
        /// <returns>Результат операции</returns>
        static bool CheckDB(string name, string server)
        {
            string command = "select count(*) from sysdatabases where name = '" + name + "'";
            SqlConnection connection = new SqlConnection("data source=" + server +
                                                         ";Persist Security Info=False;Integrated Security=SSPI");
            SqlCommand Command = new SqlCommand(command, connection);
            try { connection.Open(); }
            catch { return false; }
            SqlDataAdapter Adapter = new SqlDataAdapter();
            Adapter.SelectCommand = Command;
            DataTable Table = new DataTable();
            Adapter.Fill(Table);
            connection.Close();
            if ((int)Table.Rows[0][0] == 0) return false;
            else return true;
        }

        /// <summary>Создать базу данных</summary>
        /// <param name="instance">Экземпляр сервера</param>
        /// <param name="dbname">Имя базы данных</param>
        /// <param name="path">Путь создания файла базы</param>
        /// <param name="script_tables">Скрипт создания таблиц</param>
        /// <param name="script_data">Скрипт создания базы</param>
        /// <param name="error">Описание ошибки</param>
        /// <returns></returns>
        static int CreateDatabase(string instance, string dbname, string path,
                                  string user_id, string password,
                                  string script_tables, string script_data, ref string error)
        {
            //Поключение к серверу (master)
            SqlConnection connection = new SqlConnection("SERVER = " + instance + "; DATABASE=master;Persist Security Info=False;Integrated Security=SSPI");
            if (user_id != "") connection = new SqlConnection("SERVER=" + instance + ";DATABASE=master;User ID=" + user_id + ";Password=" + password);
            try { connection.Open(); }
            catch (Exception e)
            {
                //Отсутствует подключение к серверу
                error = e.Source + " " + e.Message;
                return -2;
            }

            #region Установка путей к файлам базы
            string path_database;
            if (path == "")
            {
                //Получение пути к базам по умолчанию
                string db_path = "";
                string err1 = "";
                if (!GetDBFilePath("master", instance, ref db_path, ref err1))
                {
                    //Ошибка получнения пути создания файла
                    error = "'" + db_path + "', " + err1;
                    return -4;
                }
                path_database = System.IO.Path.GetDirectoryName(db_path) + "\\" + dbname;
            }
            else path_database = path + "\\" + dbname;
            #endregion

            #region Скрипт создания базы
            string command = " USE [master] " +
                             " CREATE DATABASE [" + dbname + "] ON " +
                             " ( NAME = N'" + dbname + "', FILENAME = N'" + path_database + ".mdf' " +
                             ", SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) " +
                             " LOG ON " +
                             " ( NAME = N'" + dbname + "_log', FILENAME = N'" + path_database + "_log.ldf'" +
                             ", SIZE = 2304KB , MAXSIZE = 2048GB , FILEGROWTH = 10%) ";
            string err2 = "";
            if (!ExecuteCommand(command, connection, ref err2))
            {
                //Ошибка создания базы
                error = err2 + " база: " + path_database;
                return -5;
            }
            #endregion

            //Отключение от вновь созданной базе
            connection.Close();

            #region Пробное подключение к базе данных
            sqlInstance = instance;
            db_name = dbname;
            if (user_id == "")
            {
                uselogin = false;
                db_login = "";
                db_password = "";
                connection = new SqlConnection("SERVER = " + instance + "; DATABASE=master;Persist Security Info=False;Integrated Security=SSPI");
            }
            else
            {
                uselogin = true;
                db_login = user_id;
                db_password = password;
                connection = new SqlConnection("SERVER=" + instance + ";DATABASE=master;User ID=" + user_id + ";Password=" + password);
            }
            try { connection.Open(); }
            catch (Exception e)
            {
                //Отсутствует подключение к серверу
                error = e.Source + " " + e.Message;
                return -6;
            }

            //connection.Close();
            #endregion

            //Создание таблиц
            string sql = script_tables.Replace("[%DBNAME%]", "[" + dbname + "]");
            if (!ExecuteScript(connection, sql, ref error)) return -7;

            //Заполнение таблиц
            sql = script_data.Replace("[%DBNAME%]", "[" + dbname + "]");
            if (!ExecuteScript(connection, sql, ref error)) return -8;

            connection.Close();

            return 0;
        }

        /// <summary>Выполнение скрипта последовательно, разбив на команды</summary>
        /// <param name="connection">Подключение SQL (открытое)</param>
        /// <param name="sql">Скрипт</param>
        /// <param name="error">Описание ошибки</param>
        /// <returns>Результат выполнения</returns>
        static bool ExecuteScript(SqlConnection connection, string sql, ref string error)
        {
            string scriptText = sql;
            //Разделение скрипта между командами GO
            string[] commandTexts = scriptText.Split(new string[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string commandText in commandTexts)
            {
                //Выполнение по одной команде
                if (!ExecuteCommand(commandText, connection, ref error))
                {
                    error += ", команда: " + commandText;
                    return false;
                }
            }
            error = "";
            return true;
        }

        #endregion

        /// <summary>Обновление базы данных</summary>
        /// <param name="instance">Экземпляр сервера</param>
        /// <param name="dbname">Имя базы данных</param>
        /// <param name="user">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="script">Скрипт</param>
        /// <param name="error">Описание ошибки</param>
        /// <returns>Результат операции</returns>
        public static bool UpdateDatabase(string instance, string dbname, string user, string password, string script, ref string error)
        {
            //Подлючение к базе
            SqlConnection connection = new SqlConnection(CreateConnectionString(instance, dbname, user, password));
            try { connection.Open(); }
            catch (Exception e)
            {
                error = "Ошибка подключения к базе " + dbname + ": " + e.Source + " " + e.Message;
                return false;
            }

            //Выполнение скрипта обновления
            if (!ExecuteScript(connection, script, ref error)) return false;

            error = "";
            return true;
        }

        /// <summary>Удаление базы данных</summary>
        /// <returns>Результат операции</returns>
        public static bool DropDatabase(string instance, string dbname, string user, string password, ref string error)
        {
            //Поключение к серверу (master)
            SqlConnection connection = new SqlConnection(CreateMasterConnectionString(instance, user, password));
            try { connection.Open(); }
            catch (Exception e) { error = e.Source + " " + e.Message; return false; }
            //Выполнение скрипта обновления
            if (!ExecuteScript(connection, "ALTER DATABASE [" + dbname + "] SET single_user WITH ROLLBACK IMMEDIATE", ref error)) return false;
            //Выполнение скрипта обновления
            if (!ExecuteScript(connection, "DROP DATABASE " + dbname, ref error)) return false;

            return true;
        }

        #region Backup And Restore Data Base

        /// <summary>Резервирование базы данных</summary>
        /// <param name="path">Путь резервирования</param>
        /// <param name="dbname">Имя базы данных</param>
        /// <returns>Результат операции</returns>
        public static bool BackupDatabase(string instance, string dbname, string user, string password, string path, ref string error)
        {
            if (path == "") return false;

            //Подключение
            string conn_str = CreateConnectionString(instance, dbname, user, password);
            SqlConnection connection = new SqlConnection(conn_str);
            try { connection.Open(); }
            catch (Exception e)
            {
                error = "Ошибка подключения к базе " + dbname + ": " + e.Source + " " + e.Message;
                return false;
            }

            //Выполнение команды
            string filename = dbname + " " + DateTime.Now.ToShortDateString() + " " +
                              "(" + DateTime.Now.ToLongTimeString().Replace(":", "-") + ")" + ".bak";
            SqlCommand command = new SqlCommand("BACKUP DATABASE " + dbname + " TO DISK='" + filename + "'");
            command.Connection = connection;
            try { command.ExecuteNonQuery(); }
            catch (Exception e)
            {
                error = e.Source + " " + e.Message;
                return false;
            };

            //Получение получения пути к резервной копии
            string folder = "";
            if (!GetBackupFolder(conn_str, ref folder, ref error))
            {
                error = "Ошибка получения пути к резервной копии";
                return false;
            }

            //Копирования резервной копии из каталога резервирования по указанному пути
            try
            {
                System.IO.File.Copy(folder + "\\Backup\\" + filename, path + "\\" + filename);
                System.IO.File.Delete(folder + "\\Backup\\" + filename);
            }
            catch (Exception e)
            {
                error = e.Source + " " + e.Message;
                return false;
            }

            error = "";
            return true;
        }

        /// <summary>Получение пути резервирования базы данных</summary>
        /// <param name="folder">Папка резервирования</param>
        /// <returns>Результат операции</returns>
        static bool GetBackupFolder(ref string folder)
        {
            string s = "SELECT filename FROM master..sysdatabases WHERE name LIKE 'master'";
            DataTable table = LoadDataTable(s);
            if (table == null || table.Rows.Count == 0) return false;
            string filepath = table.Rows[0]["filename"].ToString();
            string backuppath = filepath.Substring(0, filepath.LastIndexOf("\\DATA"));
            folder = backuppath;
            return true;
        }

        /// <summary>Получение пути резервирования базы данных</summary>
        /// <param name="connectionstr">Строка подключения к базе</param>
        /// <param name="folder">Папка резервирования</param>
        /// <param name="error">Ошибка</param>
        /// <returns>Результат операции</returns>
        static bool GetBackupFolder(string connectionstr, ref string folder, ref string error)
        {
            string s = "SELECT filename FROM master..sysdatabases WHERE name LIKE 'master'";
            DataTable table = LoadDataTable(s, connectionstr, ref error);
            if (table == null || table.Rows.Count == 0) return false;
            string filepath = table.Rows[0]["filename"].ToString();
            string backuppath = filepath.Substring(0, filepath.LastIndexOf("\\DATA"));
            folder = backuppath;
            return true;
        }

        public static bool GetDBFilePath(string dbName, string instanse, ref string filePath, ref string error)
        {
            string query = string.Format(@"select physical_name from master.sys.master_files where name='{0}'", dbName);
            string connectionString = string.IsNullOrEmpty(instanse) ? ConnectionString : ServerConnectionString(instanse);
            DataTable table = LoadDataTable(query, connectionString, ref error);
            if (table.Rows.Count == 0) return false;
            filePath = table.Rows[0]["physical_name"].ToString();
            return true;
        }

        //static string RenameLogicDBNameCommand = @"BEGIN TRY
	       //                                            ALTER DATABASE @dbNewName SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	       //                                            ALTER DATABASE @dbNewName MODIFY FILE(Name=N'@dbOldName',NEWNAME=N'@dbNewName')
	       //                                            ALTER DATABASE @dbNewName MODIFY FILE(Name=N'@dbOldName_log',NEWNAME=N'@dbNewName_log')
	       //                                            ALTER DATABASE @dbNewName SET MULTI_USER 
        //                                           END TRY
        //                                           BEGIN CATCH
	       //                                            ALTER DATABASE @dbNewName SET MULTI_USER 
        //                                           END CATCH";

        /// <summary>Восстановление базы данных</summary>
        /// <param name="instance">Экземпляр сервера</param>
        /// <param name="database">Имя базы данных</param>
        /// <param name="database_path">Путь создания базы данных</param>
        /// <param name="backup_path">Путь к файлу резервной копии</param>
        /// <param name="rewrite">Перезаписать базу с таким же именем</param>
        public static int RestoreDatabase(string instance, string database, string database_path, string backup_path, bool rewrite,
                                          string CreateNewDatabaseCommand, string RestoreCommand, string RestoreFinal, ref string error)
        {
            string commands;
            string DataFile = "";
            string LogFile = "";
            string server_conn = ServerConnectionString(instance);

            //Проверка присутствия указанной базы данных
            string[] list = GetDataBasesList(server_conn);
            bool exists = false;
            for (int i = 0; i < list.Length; i++) if (list[i] == database) { exists = true; break; }
            bool new_database = true;//По умолчанию - создание новой базы

            #region Если база данных уже присутствует на сервере - выход
            if (exists)
            {
                if (rewrite) new_database = false;//Перезапись базы
                else
                {
                    error = "База с таким именем уже существует";
                    return -1;
                }
            }
            #endregion

            //Создать файл базы данных и log-файл
            if (new_database)
            {
                commands = CreateNewDatabaseCommand;
                commands = commands.Replace("%Database%", database);
                commands = commands.Replace("%DatabasePath%", database_path + "\\");
                commands = commands.Replace("%BackUpPath%", backup_path);
                if (commands == null) return -2;
                if (!ExecuteSQLCommand(server_conn, commands, ref error)) return -3;
            }

            //Восстановить базу из файла резервной копии
            if (GetFileRestored(server_conn, database, database_path, backup_path, ref DataFile, ref LogFile, RestoreCommand))
            {
                commands = RestoreFinal;
                commands = commands.Replace("%Database%", database);
                commands = commands.Replace("%DatabasePath%", database_path + "\\");
                commands = commands.Replace("%BackUpPath%", backup_path);
                if (commands == null) return -4;
                commands = commands.Replace("%OldData%", DataFile).Replace("%OldLog%", LogFile);
                if (!ExecuteSQLCommand(server_conn, commands, ref error)) return -5;
            }

            //if (new_database)
            //{
            //    string oldDbName = System.IO.Path.GetFileNameWithoutExtension(backup_path);
            //    oldDbName = oldDbName.Substring(0, oldDbName.IndexOf(' '));
            //    string renameLogicName = RenameLogicDBNameCommand.Replace("@dbNewName", database).Replace("@dbOldName", oldDbName);
            //    if (!ExecuteSQLCommand(server_conn, renameLogicName, ref error)) return -6;
            //}

            //Сопоставление пользователя
            string s = "USE[" + database + "] \r\n" +
                       "IF EXISTS(SELECT name FROM[sys].[database_principals] WHERE [type] = 'S' AND name = N'asko') \r\n" +
                       "BEGIN \r\n" +
                       "    DROP USER[asko] \r\n" +
                       "END \r\n" +
                       "IF NOT EXISTS(SELECT name FROM master.dbo.syslogins WHERE name = N'asko') " +
                       "BEGIN \r\n" +
                       "    CREATE LOGIN[asko] WITH PASSWORD = '312755', \r\n" +
                       "    DEFAULT_DATABASE = [asko2], \r\n" +
                       "    CHECK_EXPIRATION = OFF, \r\n" +
                       "    CHECK_POLICY = OFF \r\n" +
                       "END \r\n" +
                       "CREATE USER[asko] FOR LOGIN[asko] WITH DEFAULT_SCHEMA =[dbo] \r\n" +
                       "EXEC sp_addrolemember 'db_owner', 'asko'";
            if (!ExecuteSQLCommand(server_conn, s, ref error))
            {
                error = "Ошибка создания пользователя";
                return -6;
            }

            return 0;
        }

        /// <summary>Выполнение SQL-команды</summary>
        /// <param name="ConnectionString">Строка подключения</param>
        /// <param name="Commands">Команда</param>
        /// <param name="error">Описание ошибки</param>
        /// <returns></returns>
        static bool ExecuteSQLCommand(string ConnectionString, string Commands, ref string error)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = Commands;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                error = e.Source + " " + e.Message;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
                cmd.Dispose();
                connection.Dispose();
            }
            return true;
        }

        static string ServerConnectionString(string server)
        {
            string s = string.Format("Server={0};", server);
            s += "Trusted_Connection=True;";
            s += "Connect Timeout=1;Max Pool Size=1;Connection Lifetime=10;";
            return s;
        }

        static bool GetFileRestored(string ConnectionString, string DatabaseName, string DataBasePath,
                                    string BackUpPath, ref string DataFile, ref string LogFile,
                                    string RestoreCommandString)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand cmd = connection.CreateCommand();
            SqlDataReader reader;
            string Commands = RestoreCommandString;
            Commands = Commands.Replace("%Database%", DatabaseName);
            Commands = Commands.Replace("%DatabasePath%", DataBasePath + "\\");
            Commands = Commands.Replace("%BackUpPath%", BackUpPath);
            cmd.CommandText = Commands;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(2) == "D") DataFile = reader.GetString(0);
                    else LogFile = reader.GetString(0);
                }
                reader.Close();
                (reader as IDisposable).Dispose();
            }
            catch { return false; }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
                cmd.Dispose();
                connection.Dispose();
            }
            return true;
        }

        public static string[] GetDataBasesList(string ConnectionString)
        {
            List<string> DatabaseList = new List<string>();
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand cmd = connection.CreateCommand();
            SqlDataReader reader;
            cmd.CommandText = "Select Name From sysDatabases";
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                string Database = "";
                while (reader.Read())
                {
                    Database = reader.GetString(0);
                    if (Database != "master" && Database != "model" && Database != "msdb" && Database != "tempdb")
                        DatabaseList.Add(Database);
                }
                reader.Close();
                (reader as IDisposable).Dispose();
            }
            catch { return null; }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
                cmd.Dispose();
                connection.Dispose();
            }
            return DatabaseList.ToArray();
        }

        #endregion

    }

}
