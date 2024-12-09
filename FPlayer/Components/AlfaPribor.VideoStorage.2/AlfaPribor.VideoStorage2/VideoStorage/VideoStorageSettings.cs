using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{

    /// <summary>Настройки раздела хранилища видеоданных</summary>
    [Serializable]
    public class VideoPartitionSettings : VideoPartitionBase, IEquatable<VideoPartitionSettings>
    {
        #region Fields

        /// <summary>Лимит свободного места на разделе (байтов)</summary>
        private long _FreeSpaceLimit;

        #endregion 

        #region Methods

        /// <summary>Конструктор</summary>
        public VideoPartitionSettings()
            : base()
        {
            _FreeSpaceLimit = 0;
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор раздела хранилища</param>
        /// <param name="active">Признак активности раздела</param>
        /// <param name="path">Путь к каталогу с данными</param>
        /// <param name="free_space_limit">Лимит свободного места на разделе (байтов)</param>
        public VideoPartitionSettings(int id, bool active, string path, long free_space_limit)
            : base(id, active, path)
        {
            _FreeSpaceLimit = free_space_limit;
        }

        #endregion

        #region Properties

        /// <summary>Лимит свободного места на разделе (байтов)</summary>
        public long FreeSpaceLimit
        {
            get { return _FreeSpaceLimit; }
            set { _FreeSpaceLimit = value; }
        }

        #endregion

        #region IEquatable<VideoPartitionSettings> Members

        /// <summary>Указывает, равен ли текущий объект другому объекту того же типа</summary>
        /// <param name="other">Сравниваемый объект</param>
        /// <exception cref="System.NullReferenceException">
        /// Параметр other имеет значение null
        /// </exception>
        /// <returns>TRUE, если текущий объект равен параметру other, в противном случае — FALSE</returns>
        public bool Equals(VideoPartitionSettings other)
        {
            return
                base.Equals(other) &&
                (_FreeSpaceLimit == other._FreeSpaceLimit);
        }

        #endregion
    }

    /// <summary>Настройки хранилища видеоданных</summary>
    [Serializable]
    public class VideoStorageSettings : IEquatable<VideoStorageSettings>
    {
        #region Fields

        /// <summary>Настройки разделов хранилища</summary>
        private List<VideoPartitionSettings> _Partitions;

        /// <summary>Интервал проверки кольцевого буфера в секундах</summary>
        private int _CircleBufferCheckInterval;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        public VideoStorageSettings()
        {
            _Partitions = new List<VideoPartitionSettings>();
            _CircleBufferCheckInterval = 60;
        }

        #endregion

        #region Properties

        /// <summary>Настройки разделов хранилища</summary>
        public List<VideoPartitionSettings> Partitions
        {
            get { return _Partitions; }
            set { _Partitions = value; }
        }

        /// <summary>Интервал проверки кольцевого буфера в секундах</summary>
        public int CircleBufferCheckInterval
        {
            get { return _CircleBufferCheckInterval; }
            set { _CircleBufferCheckInterval = value; }
        }

        #endregion

        #region IEquatable<VideoStorageSettings> Members

        /// <summary>Указывает, равен ли текущий объект другому объекту того же типа</summary>
        /// <param name="other">Сравниваемый объект</param>
        /// <exception cref="System.NullReferenceException">
        /// Параметр other имеет значение null
        /// </exception>
        /// <returns>TRUE, если текущий объект равен параметру other, в противном случае — FALSE</returns>
        public bool Equals(VideoStorageSettings other)
        {
            if (_Partitions.Equals(other._Partitions))
            {
                return true;
            }
            if (_Partitions.Count != other._Partitions.Count)
            {
                return false;
            }
            bool result = true;
            for (int i = 0; i < _Partitions.Count; i++)
            {
                if (_Partitions[i].Equals(other.Partitions[i]) == false)
                {
                    result = false;
                    break;
                }
            }
            return
                result && (_CircleBufferCheckInterval == other._CircleBufferCheckInterval);
        }

        #endregion
    }

}
