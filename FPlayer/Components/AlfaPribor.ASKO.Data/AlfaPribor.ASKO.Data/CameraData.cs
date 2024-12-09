using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Информация о статусе телекамеры</summary>
    public class CameraStatData : IEquatable<CameraStatData>, ICloneable
    {
        
        int _Id = 0;
        string _Name = string.Empty;
        DevStat _Status = DevStat.unknown;
        string _Resolution = string.Empty;

        /// <summary>Конструктор</summary>
        public CameraStatData()
        {
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор камеры (номер)</param>
        public CameraStatData(int id)
        {
            _Id = id;
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор камеры (номер)</param>
        /// <param name="name">Наименование камеры</param>
        /// <param name="stat">Статус камеры</param>
        public CameraStatData(int id, string name, DevStat stat)
        {
            _Id = id;
            _Name = name;
            _Status = stat;
        }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса начальными значениями идентификатора видеокамеры, наименованием видеокамеры,
        /// состоянием устройства и графическим разрешением кадров видеоизображения
        /// </summary>
        /// <param name="id">Идентификатор камеры (номер)</param>
        /// <param name="name">Наименование камеры</param>
        /// <param name="stat">Статус камеры</param>
        /// <param name="resolution">Графическое разрешение кадров видеоизображения</param>
        public CameraStatData(int id, string name, DevStat stat, string resolution) : this(id, name, stat)
        {
            _Resolution = resolution;
        }

        /// <summary>Идентификатор камеры (номер)</summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Наименование камеры</summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>Статус камеры</summary>
        public DevStat Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>Графическое разрешение кадров видеоизображения</summary>
        public string Resolution
        {
            get { return _Resolution; }
            set { _Resolution = value; }
        }

        #region IEquatable<CameraStatData> members

        /// <summary>
        /// Проверяет равенство текущего объекта заданному
        /// </summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты равны, FALSE - в противном случае</returns>
        public bool Equals(CameraStatData other)
        {
            if (other == null)
            {
                return false;
            }
            return
                _Id == other._Id &&
                _Name == other._Name &&
                _Status == other._Status &&
                _Resolution == other.Resolution;
        }

        #endregion

        #region Члены ICloneable

        /// <summary>
        /// Создает полную копию объекта
        /// </summary>
        /// <returns>Копия объекта</returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        public enum DevStat : int
        {
            /// <summary>Не известно</summary>
            unknown = -1,
            /// <summary>Нет связи</summary>
            offline = 0,
            /// <summary>Есть связь</summary>
            online = 1,
            /// <summary>Нет видеосигнала (только для телекамер)</summary>
            novideo = 2,
            /// <summary>Выключено / не активно</summary>
            none = 3,
            /// <summary>Изменение настроек</summary>
            changeSettings = 4
        };
    }
}
