using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Shared
{

    /// <summary>Вспомогательный класс для работы с правами операторов</summary>

    public class UserPermissions
    {
        
        private int _Permissions;

        #region Constants

        /// <summary>
        /// Право редактирования конфигурации программы
        /// </summary>
        public static readonly int CanEditAppConfig = 0x1;

        /// <summary>
        /// Право редактирования параметров пользователей
        /// </summary>
        public static readonly int CanEditUsers = 0x2;

        /// <summary>
        /// Право редактирования видеопараметров телекамер
        /// </summary>
        public static readonly int CanEditVideoParameters = 0x4;

        /// <summary>
        /// Право редактирования параметров составов
        /// </summary>
        public static readonly int CanEditTrains = 0x8;

        /// <summary>
        /// Право редактирования параметров вагонов
        /// </summary>
        public static readonly int CanEditWagons = 0x10;

        /// <summary>
        /// Право удаления составов
        /// </summary>
        public static readonly int CanDeleteTrains = 0x20;

        /// <summary>
        /// Право корректирования ошибок счета (добавление/удаление вагонов)
        /// </summary>
        public static readonly int CanAddDeleteWagons = 0x40;

        /// <summary>
        /// Право привязывать составы к натурным листам
        /// </summary>
        public static readonly int CanBindToNatList = 0x80;

        /// <summary>
        /// Право блокировать составы (запрещает увтоматическое удаление)
        /// </summary>
        public static readonly int CanLockTrains = 0x100;

        /// <summary>
        /// Право изменять режим работы ВидеоИнспектор Сервер
        /// </summary>
        public static readonly int CanChangeServerMode = 0x200;

        /// <summary>
        /// Отсутствуют все права
        /// </summary>
        public static readonly int CanNotAnything = 0x0;

        /// <summary>
        /// Разрешены любые действия
        /// </summary>
        public static readonly int CanAnything = Int32.MinValue;

        #endregion

        #region Methods

        /// <summary>Конструктор объектов класса</summary>
        public UserPermissions() : this(UserPermissions.CanNotAnything) { }

        /// <summary>Конструктор объектов класса. Инициирует объект маской прав оператора</summary>
        /// <param name="mask">Маска прав оператора</param>
        public UserPermissions(int mask)
        {
            _Permissions = mask;
        }

        /// <summary>Определяет наличие определенного бита в маске прав</summary>
        /// <param name="mask">Маска прав</param>
        /// <returns>TRUE - пользователь обладает указанным правом, FALSE - в противном случае</returns>
        public bool Contains(int mask)
        {
            return (_Permissions & mask) != 0;
        }

        /// <summary>Добавляет к общей маске прав заданные права</summary>
        /// <param name="mask">Маска добавляемых прав</param>
        public void ResetPermission(int mask)
        {
            _Permissions &= (~mask);
        }

        /// <summary>Исключает из общей маски прав заданные права</summary>
        /// <param name="mask">Маска исключаемых прав</param>
        public void SetPermission(int mask)
        {
            _Permissions |= mask;
        }

        #endregion

        /// <summary>Маска прав оператора</summary>
        public int Permissions
        {
            get { return _Permissions; }
            set { _Permissions = value; }
        }
    }

}
