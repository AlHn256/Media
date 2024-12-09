using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Информация о разделе хранилища видеоданных</summary>
    [Serializable]
    [DataContract]
    public class VideoPartitionInfo : VideoPartitionBase, IEquatable<VideoPartitionInfo>
    {
        #region Fields

        /// <summary>Количество байтов, доступное для хранения хранения данных</summary>
        private long _TotalSpace;
        
        /// <summary>Количество использованного места для храниения видеозаписей (байт)</summary>
        private long _UsedSpace;

        /// <summary>Количество свободного места на разделе хранилища (байт)</summary>
        private long _FreeSpace;
        
        /// <summary>Количество видеозаписей в разделе хранилища</summary>
        private int _RecordCount;

        /// <summary>Статус раздела хранилища</summary>
        VideoPartitionState _Status;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        public VideoPartitionInfo()
            : base()
        {
            _TotalSpace = 0;
            _UsedSpace = 0;
            _FreeSpace = 0;
            _RecordCount = 0;
            _Status = VideoPartitionState.Ok;
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор раздела хранилища</param>
        /// <param name="active">Признак активности раздела</param>
        /// <param name="path">Путь к каталогу с данными</param>
        /// <param name="total_space">Всего места на разделе (байтов)</param>
        /// <param name="used_space">Использовано места под данные (байтов)</param>
        /// <param name="free_space">Свободно места для хранения данных (байтов)</param>
        /// <param name="record_count">Количество видеозаписей</param>
        /// <param name="status">Статус раздела хранилища видеоданных</param>
        public VideoPartitionInfo(int id, bool active, string path, long total_space, 
            long used_space, long free_space, int record_count, VideoPartitionState status)
            : base(id, active, path)
        {
            _TotalSpace = total_space;
            _UsedSpace = used_space;
            _FreeSpace = free_space;
            _RecordCount = record_count;
            _Status = status;
        }

        #endregion

        #region Properties

        /// <summary>Количество байтов, доступное для хранения хранения данных</summary>
        [DataMember]
        public long TotalSpace
        {
            get { return _TotalSpace; }
            set { _TotalSpace = value; }
        }

        /// <summary>Количество использованного места для храниения видеозаписей (байт)</summary>
        [DataMember]
        public long UsedSpace
        {
            get { return _UsedSpace; }
            set { _UsedSpace = value; }
        }

        /// <summary>Количество свободного места на разделе хранилища (байт)</summary>
        [DataMember]
        public long FreeSpace
        {
            get { return _FreeSpace; }
            set { _FreeSpace = value; }
        }

        /// <summary>Количество видеозаписей в данном разделе</summary>
        [DataMember]
        public int RecordCount
        {
            get { return _RecordCount; }
            set { _RecordCount = value; }
        }

        /// <summary>Статус раздела хранилища</summary>
        [DataMember]
        public VideoPartitionState Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        #endregion

        #region IEquatable<VideoPartitionInfo> Members

        /// <summary>Указывает, равен ли текущий объект другому объекту того же типа</summary>
        /// <param name="other">Сравниваемый объект</param>
        /// <exception cref="System.NullReferenceException">
        /// Параметр other имеет значение null
        /// </exception>
        /// <returns>TRUE, если текущий объект равен параметру other, в противном случае — FALSE</returns>
        public bool Equals(VideoPartitionInfo other)
        {
            return
                base.Equals(other) &&
                (_TotalSpace == other._TotalSpace) &&
                (_UsedSpace == other._UsedSpace) &&
                (_FreeSpace == other._FreeSpace) &&
                (_RecordCount == other.RecordCount) &&
                (_Status == other._Status);
        }

        #endregion
    }

    /// <summary>Информация о хранилище видеоданных</summary>
    [Serializable]
    [DataContract]
    public class VideoStorageInfo : IEquatable<VideoStorageInfo>
    {
        #region Fields

        /// <summary>Информация о разделах хранилища</summary>
        private List<VideoPartitionInfo> _Partitions;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        public VideoStorageInfo()
        {
            _Partitions = new List<VideoPartitionInfo>();
        }

        #endregion

        #region Properies

        /// <summary>Информация о разделах хранилища</summary>
        [DataMember]
        public List<VideoPartitionInfo> Partitions
        {
            get { return _Partitions; }
            set { _Partitions = value; }
        }

        #endregion

        #region IEquatable<VideoStorageInfo> Members

        /// <summary>Указывает, равен ли текущий объект другому объекту того же типа</summary>
        /// <param name="other">Сравниваемый объект</param>
        /// <exception cref="System.NullReferenceException">
        /// Параметр other имеет значение null
        /// </exception>
        /// <returns>TRUE, если текущий объект равен параметру other, в противном случае — FALSE</returns>
        public bool Equals(VideoStorageInfo other)
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
            return result;
        }

        #endregion
    }
}
