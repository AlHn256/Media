using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;

namespace AlfaPribor.DataHelper
{

    /// <summary>Вспомогательный класс, упрощающий взаимодействие с базой данных</summary>
    public abstract class DataHelper : IDisposable
    {

        #region Fields

        /// <summary>Представляет подключение к базе данных</summary>
        protected DbConnection _Connection;

        /// <summary>Представляет транзакцию базы данных</summary>
        protected DbTransaction _Transaction;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <param name="connection_str">Строка, описывающая параметры подключения к базе данных</param>
        /// <exception cref="System.Exception">Ошибка соединения с базой данных</exception>
        public DataHelper(string connection_str)
        {
            _disposed = false;
            try
            {
                _Connection = NewConnection();
                _Connection.ConnectionString = connection_str;
                _Connection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Connection to DB failed", ex);
            }
        }

        /// <summary>Создает объект для доступа к базе данных</summary>
        /// <returns>Объект, представлющий подключение к базе данных</returns>
        protected abstract DbConnection NewConnection();

        /// <summary>Создать новую команду для чтения/изменения данных в базе данных</summary>
        /// <returns>Объект, представляющий команду к базе данных</returns>
        protected abstract DbCommand NewCommand();

        /// <summary>Начинает транзакцию базы данных.
        /// Применяется уровень изоляции по умолчанию для конкретного типа используемого подключения
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.InvalidOperationException">Транзакция активна в текущий момент времени.
        /// Параллельные транзакции не поддерживаются.
        /// </exception>
        /// <exception cref="System.Exception">Невозможно начать транзакцию</exception>
        public void BeginTransaction()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            _Transaction = _Connection.BeginTransaction();
        }

        /// <summary>Начинает транзакцию базы данных с заданным уровнем изоляции</summary>
        /// <param name="iso_level">Уровень изоляции транзакции</param>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.InvalidOperationException">Транзакция активна в текущий момент времени.
        /// Параллельные транзакции не поддерживаются.
        /// </exception>
        /// <exception cref="System.Exception">Невозможно начать транзакцию</exception>
        public void BeginTransaction(IsolationLevel iso_level)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (_Transaction != null)
            {
                throw new InvalidOperationException("Transaction already open!");
            }
            _Transaction = _Connection.BeginTransaction(iso_level);
        }

        /// <summary>Применить изменения, внесенные в базу данных в контексте открытой транзакции</summary>
        /// <exception cref="System.Exception">Ошибка при попытке подтверждения транзакции</exception>
        public void Commit()
        {
            if (_Transaction != null)
            {
                _Transaction.Commit();
                _Transaction.Dispose();
                _Transaction = null;
            }
        }

        /// <summary>Отменить изменения, внесенные в базу данных в контексте открытой транзакции</summary>
        /// <exception cref="System.Exception">Ошибка при попытке отката транзакции</exception>
        public void Rollback()
        {
            if (_Transaction != null)
            {
                _Transaction.Rollback();
                _Transaction.Dispose();
                _Transaction = null;
            }
        }

        /// <summary>Выполняет оператор SQL применительно к подключенной базе данных</summary>
        /// <param name="query">Строка с текстом команды на языке SQL</param>
        /// <param name="parList">Список дополнительных параметров оператора SQL</param>
        /// <exception cref="System.ArgumentException">Не задан текст команды SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Число подвергшихся воздействию строк или минус 1, если данные не изменялись</returns>
        public int ExecuteNoneQuery(string query, params DbParameter[] parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Выполняет оператор SQL применительно к подключенной базе данных</summary>
        /// <param name="sql">Строка выполнения и параметры</param>
        public int ExecuteNoneQuery(SqlParams sql)
        {
            return ExecuteNoneQuery(sql.SQL, sql.Params);
        }

        /// <summary>Выполняет оператор SQL применительно к подключенной базе данных</summary>
        /// <param name="query">Строка с текстом команды на языке SQL</param>
        /// <param name="parList">Список дополнительных параметров оператора SQL</param>
        /// <exception cref="System.ArgumentException">Не задан текст команды SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Число подвергшихся воздействию строк или минус 1, если данные не изменялись</returns>
        public int ExecuteNoneQuery(string query, IList<DbParameter> parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Выполняет запрос и возвращает первый столбец первой строки результирующего набора,
        /// возвращаемого запросом. Все другие столбцы и строки игнорируются.
        /// </summary>
        /// <param name="query">Строка с текстом команды на языке SQL</param>
        /// <param name="parList">Список дополнительных параметров оператора SQL</param>
        /// <exception cref="System.ArgumentException">Не задан текст команды SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Первый столбец первой строки результирующего набора, возвращаемого запросом</returns>
        public object ExecuteScalar(string query, params DbParameter[] parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return cmd.ExecuteScalar();
        }

        /// <summary>Выполняет запрос и возвращает первый столбец первой строки результирующего набора,
        /// возвращаемого запросом. Все другие столбцы и строки игнорируются.
        /// </summary>
        /// <param name="query">Строка с текстом команды на языке SQL</param>
        /// <param name="parList">Список дополнительных параметров оператора SQL</param>
        /// <exception cref="System.ArgumentException">Не задан текст команды SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Первый столбец первой строки результирующего набора, возвращаемого запросом</returns>
        public object ExecuteScalar(string query, IList<DbParameter> parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return cmd.ExecuteScalar();
        }

        /// <summary>Выполняет оператор SQL и возвращает результирующий набор данных</summary>
        /// <param name="cmd">Оператор SQL</param>
        /// <exception cref="System.ArgumentNullException">Не задан объект с командой SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Результирующий набор данных</returns>
        public DataTable ExecuteCommand(DbCommand cmd)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (cmd == null)
            {
                throw new ArgumentNullException();
            }
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                DataTable table = new DataTable();
                table.Load(reader);
                return table;
            }
        }

        /// <summary>Выполняет оператор SQL и возвращает результирующий набор данных</summary>
        /// <param name="query">Строка с текстом команды на языке SQL</param>
        /// <param name="parList">Список дополнительных параметров оператора SQL</param>
        /// <exception cref="System.ArgumentException">Не задан текст команды SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Результирующий набор данных</returns>
        public DataTable ExecuteCommand(string query, params DbParameter[] parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return ExecuteCommand(cmd);
        }

        /// <summary>Выполняет оператор SQL и возвращает результирующий набор данных</summary>
        /// <param name="query">Строка с текстом команды на языке SQL</param>
        /// <param name="parList">Список дополнительных параметров оператора SQL</param>
        /// <exception cref="System.ArgumentException">Не задан текст команды SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Результирующий набор данных</returns>
        public DataTable ExecuteCommand(string query, IList<DbParameter> parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return ExecuteCommand(cmd);
        }

        /// <summary>Выполняет оператор SQL и возвращает результирующий набор данных</summary>
        /// <param name="query">Строка с текстом команды на языке SQL</param>
        /// <param name="parList">Список дополнительных параметров оператора SQL</param>
        /// <param name="TimeOut">Тайм-аут выполнения</param>
        /// <exception cref="System.ArgumentException">Не задан текст команды SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Результирующий набор данных</returns>
        public DataTable ExecuteCommand(string query, IList<DbParameter> parList, int TimeOut)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = TimeOut;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return ExecuteCommand(cmd);
        }

        /// <summary>Выполняет оператор SQL и возвращает результирующий набор данных</summary>
        /// <param name="proc_name">Имя хранимой процедуры в базе данных</param>
        /// <param name="parList">Список дополнительных параметров хранимой процедуры</param>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.ArgumentException">Не задано имя хранимой процедуры</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Результирующий набор данных</returns>
        public DataTable ExecuteStoredProcedure(string proc_name, params DbParameter[] parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(proc_name))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = proc_name;
            cmd.CommandType = CommandType.StoredProcedure;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return ExecuteCommand(cmd);
        }

        /// <summary>Выполняет хранимую процедуру SQL применительно к подключенной базе данных</summary>
        /// <param name="proc_name">Строка с текстом команды на языке SQL</param>
        /// <param name="parList">Список дополнительных параметров оператора SQL</param>
        /// <exception cref="System.ArgumentException">Не задан текст команды SQL</exception>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Число подвергшихся воздействию строк или минус 1, если данные не изменялись</returns>
        public int ExecuteNoneQueryStoredProcedure(string proc_name, IList<DbParameter> parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(proc_name))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = proc_name;
            cmd.CommandType = CommandType.StoredProcedure;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Выполняет оператор SQL и возвращает результирующий набор данных</summary>
        /// <param name="proc_name">Имя хранимой процедуры в базе данных</param>
        /// <param name="parList">Список дополнительных параметров хранимой процедуры</param>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        /// <exception cref="System.ArgumentException">Не задано имя хранимой процедуры</exception>
        /// <exception cref="System.Exception">Ошибка в процессе выполнения команды</exception>
        /// <returns>Результирующий набор данных</returns>
        public DataTable ExecuteStoredProcedure(string proc_name, IList<DbParameter> parList)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (string.IsNullOrEmpty(proc_name))
            {
                throw new ArgumentException();
            }
            DbCommand cmd = NewCommand();
            cmd.Connection = _Connection;
            cmd.Transaction = _Transaction;
            cmd.CommandText = proc_name;
            cmd.CommandType = CommandType.StoredProcedure;
            if (parList != null)
            {
                foreach (DbParameter param in parList)
                {
                    if (param != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
            }
            return ExecuteCommand(cmd);
        }

        /// <summary>
        /// Проверяет состояние транзакции базы данных. 
        /// Если транзакция завершена или не создавалась методом BeginTransaction - генерируется исключение
        /// </summary>
        /// <param name="descr">Строка с описанием исключительной ситуации</param>
        /// <exception cref="System.Exception">Транзакция закрыта или не существует</exception>
        public void СheckTransaction(string descr)
        {
            if (_Transaction == null)
            {
                throw new Exception(descr);
            }
        }

        #endregion

        #region Properties

        /// <summary>Время ожидания при попытке установки подключения,
        /// по истечении которого попытка подключения завершается и генерируется ошибка
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Попытка обращения к удаленному объекту</exception>
        int ConnectionTimeout
        {
            get 
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                return _Connection.ConnectionTimeout; 
            }
        }

        #endregion

        #region IDisposable Members

        #region Fields

        /// <summary>Признак освобождения ресурсов объекта</summary>
        private bool _disposed;

        #endregion 

        #region Methods

        /// <summary>Высвобождает ресурсы объекта</summary>
        public void Dispose()
        {
            Dispose(true);
            // Т.к. ресурсы объекта высвобождаются методом Dispose, отключаем вызов метода
            // Finalize сборщиком "мусора"
            GC.SuppressFinalize(this);
        }

        /// <summary>Высвобождает ресурсы объекта</summary>
        /// <param name="disposing">
        /// Если равен FALSE - освобождаются только неуправляемые ресурсы,
        /// иначе - освобождаются все ресурсы объекта
        /// </param>
        private void Dispose(bool disposing)
        {
            // Предотвращаем повтороное высвобождение ресурсов
            if (!this._disposed)
            {
                // Отменяем транзакцию, если она не была подтверждена методом Commit
                try
                {
                    if (_Transaction != null)
                    {
                        Rollback();
                    }
                }
                catch { }
                // Закрываем соединение с сервером
                try
                {
                    _Connection.Close();
                }
                catch { }
                // Освобождаем управляемые ресурсы
                if (disposing)
                {
                    _Transaction = null;
                    _Connection = null;
                }
            }
            _disposed = true;
        }

        /// <summary>Деструктор класса</summary>
        ~DataHelper()
        {
            // Освобождаем неуправляемые ресурсы
            Dispose(false);
        }

        #endregion

        #endregion

    }

    /// <summary>Реализация класса DataHelper для подключения к базе SQL server</summary>
    public class SqlDataHelper : DataHelper
    {
        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <param name="connection_str">Строка, описывающая параметры подключения к базе данных</param>
        /// <exception cref="System.Exception">Ошибка соединения с базой данных</exception>
        public SqlDataHelper(string connection_str)
            : base(connection_str) { }

        /// <summary>Создает объект для доступа к базе данных</summary>
        /// <returns>Объект, представлющий подключение к базе данных</returns>
        protected override DbConnection NewConnection()
        {
            return new SqlConnection();
        }

        /// <summary>Создать новую команду для чтения/изменения данных в базе данных</summary>
        /// <returns>Объект, представляющий команду к базе данных</returns>
        protected override DbCommand NewCommand()
        {
            return new SqlCommand();
        }

        #endregion
    }

    /// <summary>Реализация класса DataHelper для подключения к базе данных посредством технологии OLE</summary>
    public class OleDbDataHelper : DataHelper
    {
        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <param name="connection_str">Строка, описывающая параметры подключения к базе данных</param>
        /// <exception cref="System.Exception">Ошибка соединения с базой данных</exception>
        public OleDbDataHelper(string connection_str)
            : base(connection_str) { }

        /// <summary>Создает объект для доступа к базе данных</summary>
        /// <returns>Объект, представлющий подключение к базе данных</returns>
        protected override DbConnection NewConnection()
        {
            return new OleDbConnection();
        }

        /// <summary>Создать новую команду для чтения/изменения данных в базе данных</summary>
        /// <returns>Объект, представляющий команду к базе данных</returns>
        protected override DbCommand NewCommand()
        {
            return new OleDbCommand();
        }

        #endregion
    }

    /// <summary>Реализация класса DataHelper для подключения к базе данных перез ODBC</summary>
    public class OdbcDataHelper : DataHelper
    {
        #region Methods

        /// <summary>Конструктор класса</summary>
        /// <param name="connection_str">Строка, описывающая параметры подключения к базе данных</param>
        /// <exception cref="System.Exception">Ошибка соединения с базой данных</exception>
        public OdbcDataHelper(string connection_str)
            : base(connection_str) { }

        /// <summary>Создает объект для доступа к базе данных</summary>
        /// <returns>Объект, представлющий подключение к базе данных</returns>
        protected override DbConnection NewConnection()
        {
            return new OdbcConnection();
        }

        /// <summary>Создать новую команду для чтения/изменения данных в базе данных</summary>
        /// <returns>Объект, представляющий команду к базе данных</returns>
        protected override DbCommand NewCommand()
        {
            return new OdbcCommand();
        }

        #endregion
    }

}
