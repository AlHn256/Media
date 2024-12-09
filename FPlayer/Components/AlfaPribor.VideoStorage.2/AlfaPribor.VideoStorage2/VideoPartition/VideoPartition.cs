using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Раздел хранилища видеоданных</summary>
    public class VideoPartition
    {

        /// <summary>Вспомогательный класс для поиска объектов VideoPartition с заданным идентификатором</summary>
        public class IdComparer
        {
            /// <summary>Идентификатор объекта</summary>
            private int _Id;

            /// <summary>Конструктор класса</summary>
            /// <param name="id">Идентификатор искомого объекта</param>
            public IdComparer(int id)
            {
                _Id = id;
            }

            /// <summary>Метод сравнения по идентификатору объекта</summary>
            /// <param name="obj">Сравниваемый объект</param>
            /// <returns>
            /// Возвращает TRUE, если сравниваемый объект имеет искомый идентификатор, иначе возвращает FALSE
            /// </returns>
            public bool Equal(VideoPartition obj)
            {
                return _Id == obj._Id;
            }
        }

        #region Fields

        /// <summary>Идентификатор раздела</summary>
        private int _Id;

        /// <summary>Признак активности раздела</summary>
        private bool _Active;

        /// <summary>Путь к каталогу, в котором хранятся видеозаписи</summary>
        private string _Path;

        /// <summary>Лимит свободного места, которое должно обеспечиваться на данном разделе (байт)</summary>
        private long _FreeSpaceLimit;

        /// <summary>Тип хранилища видеоданных</summary>
        private VideoStorageType _StorageType;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <param name="id">Идентификатор раздела хранилища</param>
        /// <param name="active">Признак активность раздела</param>
        /// <param name="path">Путь к каталогу, в котором хранятся видеозаписи</param>
        /// <exception cref="System.ArgumentNullException">Параметр path имеет значение null</exception>
        /// <exception cref="System.ArgumentException">Строка path имеет недопустимый формат или содержит недопустимые символы</exception>
        public VideoPartition(VideoStorageType storage_type, int id, bool active, string path)
        {
            CheckPath(path);
            _StorageType = storage_type;
            _Id = id;
            _Active = active;
            _Path = path;
        }

        /// <summary>Конструктор</summary>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <param name="id">Идентификатор раздела хранилища</param>
        /// <param name="active">Признак активность раздела</param>
        /// <param name="path">Путь к каталогу, в котором хранятся видеозаписи</param>
        /// <param name="free_space_limit">Лимит свободного места на разделе</param>
        /// <exception cref="System.ArgumentNullException">Параметр path имеет значение null</exception>
        /// <exception cref="System.ArgumentException">Строка path имеет недопустимый формат или содержит недопустимые символы</exception>
        public VideoPartition(VideoStorageType storage_type, int id, bool active, string path, long free_space_limit)
            : this(storage_type, id, active, path)
        {
            _FreeSpaceLimit = free_space_limit;
        }

        /// <summary>Конструктор класса. Инициализирует объект класса по данным настроек</summary>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <param name="settings">Настройки раздела хранилища</param>
        /// <exception cref="System.ArgumentNullException">Параметр settings имеет значение null</exception>
        /// <exception cref="System.ArgumentException">Путь к разделу хранилища не задан, имеет недопустимый формат или содержит недопустимые символы</exception>
        public VideoPartition(VideoStorageType storage_type, VideoPartitionSettings settings)
        {
            if (settings == null)
	        {
                throw new ArgumentNullException();
	        }
            if (settings.Path == null)
            {
                throw new ArgumentException();
            }
            CheckPath(settings.Path);
            _StorageType = storage_type;
            _Id = settings.Id;
            _Active = settings.Active;
            _Path = settings.Path;
            _FreeSpaceLimit = settings.FreeSpaceLimit;
        }

        #endregion

        #region Methods Static

        /// <summary>Проверяет правильность заданного пути. Проверка на существование пути не производится!</summary>
        /// <param name="path">Строка, содержащая путь к месту хранения данных</param>
        /// <exception cref="System.ArgumentNullException">Параметр path имеет значение null</exception>
        /// <exception cref="System.ArgumentException">Строка path имеет недопустимый формат или содержит недопустимые символы</exception>
        public static void CheckPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (path == string.Empty)
            {
                throw new ArgumentException();
            }
            char[] symbols = System.IO.Path.GetInvalidPathChars();
            if (path.IndexOfAny(symbols) >= 0)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>Получить настройки раздела хранилища видеоданных</summary>
        /// <param name="partition">Раздел хранилища видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задан раздел хранилища видеоданных</exception>
        /// <returns>Возвращает настройки раздела хранилища видеоданных</returns>
        public static VideoPartitionSettings GetSettings(VideoPartition partition)
        {
            if (partition == null)
            {
                throw new ArgumentNullException();
            }
            VideoPartitionSettings settings = new VideoPartitionSettings(partition.Id, partition.Active, partition.Path, partition.FreeSpaceLimit);
            return settings;
        }

        /// <summary>Получить информацию о разделе хранилища видеоданных</summary>
        /// <param name="partition">Раздел хранилища видеоданных</param>
        /// <exception cref="System.ArgumentNullException">Не задан раздел хранилища видеоданных</exception>
        /// <returns>Возвращает информацию о разделе хранилища видеоданных</returns>
        public static VideoPartitionInfo GetInfo(VideoPartition partition)
        {
            if (partition == null)
            {
                throw new ArgumentNullException();
            }
            // Определяем статус раздела хранилища видеоданных
            VideoPartitionState status = partition.Status;
            VideoPartitionInfo info;
            // Если раздел недоступен - возвращаем только основные данные раздела...
            if (status == VideoPartitionState.Fault)
            {
                info = new VideoPartitionInfo(
                    partition.Id, partition.Active, partition.Path,
                    0, 0, 0, 0, VideoPartitionState.Fault
                );
            }
            // ...в противном случае - возвращаем полную информацию о разделе
            else
            {
                long total_space = 0;
                try
                {
                    total_space = partition.TotalSpace;
                }
                catch (VideoStorageException) { }
                long free_space = 0;
                try
                {
                    free_space = partition.FreeSpace;
                }
                catch (VideoStorageException) { }
                long used_space = 0;
                try
                {
                    used_space = partition.UsedSpace;
                }
                catch (VideoStorageException) { }
                int rec_count = 0;
                try
                {
                    rec_count = partition.RecordCount;
                }
                catch (VideoStorageException) { }
                info = new VideoPartitionInfo(
                    partition.Id, partition.Active, partition.Path,
                    total_space, used_space, free_space,
                    rec_count, status
                );
            }
            return info;
        }

        /// <summary>Позволяет определить, является ли раздел хранилища активным</summary>
        /// <param name="obj">Ссылка на объект с информацией о разделе хранилища видеоданных</param>
        /// <returns>Возвращает TRUE, если указанный раздел является активным, FALSE - в противном случае</returns>
        public static bool IsActive(VideoPartition obj)
        {
            return obj.Active;
        }

        #endregion

        #region Properties

        /// <summary>Лимит свободного места, которое должно обеспечиваться на данном разделе (байт)</summary>
        public long FreeSpaceLimit
        {
            get { return _FreeSpaceLimit; }
            set { _FreeSpaceLimit = value; }
        }

        /// <summary>Признак активности раздела</summary>
        public bool Active
        {
            get { return _Active; }
            set { _Active = value; }
        }

        /// <summary>Идентификатор раздела</summary>
        public int Id
        {
            get { return _Id; }
        }

        /// <summary>Путь к каталогу, в котором хранятся видеозаписи</summary>
        public string Path
        {
            get { return _Path; }
        }

        /// <summary> Свободное место на данном разделе (байт)</summary>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Ошибка при обращении к папке раздела
        /// </exception>
        public long FreeSpace
        {
            get
            {
                long result;
                try
                {
                    string drive_name = System.IO.Path.GetPathRoot(_Path);
                    DriveInfo drive = new DriveInfo(drive_name);
                    result = drive.AvailableFreeSpace;
                }
                catch (Exception E)
                {
                    throw new VideoStorageException(E.Message, E);
                }
                return result;
            }
        }

        /// <summary>Признак того, что раздел заполнен данными до установленного предела (или даже переполнен)</summary>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Ошибка при обращении к папке раздела
        /// </exception>
        public bool Full
        {
            get
            {
                return FreeSpace <= _FreeSpaceLimit;
            }
        }

         /// <summary>Количество видеозаписей в данном разделе</summary>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Ошибка при обращении к папке раздела
        /// </exception>
        public int RecordCount
        {
            get
            {
                try
                {
                    IList<string> records = VideoRecord.GetRecords(_Path, _StorageType);
                    return records.Count;
                }
                catch(Exception E)
                {
                    throw new VideoStorageException(E.Message, E);
                }
            }
        }

        /// <summary>Всего места на разделе (байт)</summary>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Ошибка при обращении к папке раздела
        /// </exception>
        public long TotalSpace
        {
            get
            {
                long result;
                try
                {
                    string drive_name = System.IO.Path.GetPathRoot(_Path);
                    DriveInfo drive = new DriveInfo(drive_name);
                    result = drive.TotalSize;
                }
                catch (Exception E)
                {
                    throw new VideoStorageException(E.Message, E);
                }
                return result;
            }
        }

        /// <summary>Использовано места для хранения видеозаписей (байт)</summary>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Ошибка при обращении к папке раздела
        /// </exception>
        public long UsedSpace
        {
            get
            {
                long result = 0;
                try
                {
                    DirectoryInfo directory = new DirectoryInfo(_Path);
                    IList<string> records = VideoRecord.GetRecords(directory, _StorageType);
                    foreach (string record in records)
                    {
                        result += VideoRecord.GetRecordSize(directory, record, _StorageType);
                    }
                }
                catch (Exception E)
                {
                    throw new VideoStorageException(E.Message, E);
                }
                return result;
            }
        }

        /// <summary>Статус раздела хранилища видеоданных. Вычисляется каждый раз при запросе.</summary>
        public VideoPartitionState Status
        {
            get
            {
                VideoPartitionState status;
                if (_Active)
                {
                    if (Directory.Exists(_Path))
                    {
                        status = VideoPartitionState.Ok;
                    }
                    else
                    {
                        status = VideoPartitionState.Fault;
                    }
                }
                else
                {
                    status = VideoPartitionState.Inactive;
                }
                return status;
            }
        }

        /// <summary>Тип хранилища видеоданных</summary>
        public VideoStorageType StorageType
        {
            get { return _StorageType; }
        }

        #endregion
    }

}
