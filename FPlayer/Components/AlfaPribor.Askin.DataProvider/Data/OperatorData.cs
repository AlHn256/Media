using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.ASKIN.Data
{
    /// <summary>Данные учетной записи оператора</summary>
    public class OperatorData : IdetifiedData
    {

        #region Fields

        /// <summary>Логин</summary>
        private string _Login;

        /// <summary>Пароль</summary>
        private string _Password;

        /// <summary>Имя оператора</summary>
        private string _OpName;

        /// <summary>Статус активен/отключен</summary>
        private int _Status;

        /// <summary>Уровень доступа</summary>
        private int _Permissions;

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства значениями по умолчанию</summary>
        public OperatorData()
        {
            _Login = string.Empty;
            _Password = string.Empty;
            _OpName = string.Empty;
            _Permissions = 0;
        }

        /// <summary>Конструктор класса. Инициализирует свойства заданными значениями</summary>
        /// <param name="id">Идентификатор оператора</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Имя оператора</param>
        /// <param name="level">Уровень доступа</param>
        public OperatorData(int id, string login, string password, string name, int status, int permissions)
            : base(id)
        {
            _Login = login;
            _Password = password;
            _OpName = name;
            _Status = status;
            _Permissions = permissions;
        }

        /// <summary>Конструктор класса. Инициализирует свойства заданными значениями</summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Имя оператора</param>
        /// <param name="level">Уровень доступа</param>
        public OperatorData(string login, string password, string name, int level)
            : base()
        {
            _Login = login;
            _Password = password;
            _OpName = name;
            _Permissions = level;
        }

        #endregion

        #region Properties

        /// <summary>Имя доступа к работе с программой</summary>
        public string Login
        {
            get { return _Login; }
            set { _Login = value; }
        }

        /// <summary>Пароль доступа к работе с программой</summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        /// <summary>Имя оператора</summary>
        public string OpName
        {
            get { return _OpName; }
            set { _OpName = value; }
        }

        /// <summary>Статус оператора</summary>
        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>Полномочия оператора (уровень доступа)</summary>
        public int Permissions
        {
            get { return _Permissions; }
            set { _Permissions = value; }
        }

        #endregion
    }
}
