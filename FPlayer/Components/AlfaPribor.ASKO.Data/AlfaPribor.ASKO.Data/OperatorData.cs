using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.ASKO.Data
{
    /// <summary>Данные учетной записи оператора</summary>
    public class OperatorData
    {

        #region Fields

        /// <summary>Идентификатор</summary>
        int _Id;
        /// <summary>Логин</summary>
        string _Login;
        /// <summary>Пароль</summary>
        string _Password;
        /// <summary>Имя оператора</summary>
        string _OpName;
        /// <summary>Статус активен/отключен</summary>
        bool _Status;
        /// <summary>Уровень доступа</summary>
        int _Permissions;
        /// <summary>Код USB ключа</summary>
        int _UsbKey;
        /// <summary>Тип USB ключа</summary>
        int _UsbType;
        
        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства значениями по умолчанию</summary>
        public OperatorData()
        {
            _Id = 0;
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
        /// <param name="permissions">Уровень доступа</param>
        public OperatorData(int id, string login, string password, string name, bool status, int permissions, int usb_key, int usb_type)
        {
            _Id = id;
            _Login = login;
            _Password = password;
            _OpName = name;
            _Status = status;
            _Permissions = permissions;
            _UsbKey = usb_key;
            _UsbType = usb_type;
        }

        #endregion

        #region Properties

        /// <summary>Идентификатор опреатора</summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

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
        public bool Status
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

        /// <summary>Код USB ключа</summary>
        public int UsbKey
        {
            get { return _UsbKey; }
            set { _UsbKey = value; }
        }
        
        /// <summary>Тип USB ключа</summary>
        public int UsbType
        {
            get { return _UsbType; }
            set { _UsbType = value; }
        }

        #endregion

    }
}
