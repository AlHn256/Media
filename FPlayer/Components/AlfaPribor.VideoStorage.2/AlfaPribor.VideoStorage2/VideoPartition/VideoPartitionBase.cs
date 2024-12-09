using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Базовый класс "Раздел хранилища"</summary>
    [Serializable]
    public class VideoPartitionBase : IEquatable<VideoPartitionBase>
    {
        #region Fields

        /// <summary>Идентификатор раздела</summary>
        private int _Id;

        /// <summary>Признак активности раздела</summary>
        private bool _Active;

        /// <summary>Путь к каталогу с данными</summary>
        private string _Path;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        public VideoPartitionBase()
        {
            _Id = 0;
            _Active = false;
            _Path = string.Empty;
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор раздела</param>
        /// <param name="active">Признак активности раздела</param>
        /// <param name="path">Путь к каталогу с данными</param>
        public VideoPartitionBase(int id, bool active, string path)
        {
            _Id = id;
            _Active = active;
            _Path = path;
        }

        #endregion

        #region Properties

        /// <summary>Идентификатор раздела</summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Признак активности раздела</summary>
        public bool Active
        {
            get { return _Active; }
            set { _Active = value; }
        }

        /// <summary>Путь к каталогу с данными</summary>
        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }

        #endregion

        #region IEquatable<VideoPartitionBase> Members

        /// <summary>Указывает, равен ли текущий объект другому объекту того же типа</summary>
        /// <param name="other">Сравниваемый объект</param>
        /// <returns>TRUE, если текущий объект равен параметру other, в противном случае — FALSE</returns>
        public bool Equals(VideoPartitionBase other)
        {
            return
                (_Id == other._Id) &&
                (_Active == other._Active) &&
                (_Path == other._Path);
        }

        #endregion
    }
}
