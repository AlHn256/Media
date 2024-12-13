using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage2
{

    /// <summary>Видеозапись. Версия 2.
    /// Класс, предоставляющий свойства для записи/чтения видеоданных</summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public abstract class BaseVideoRecord : IVideoWriter, IVideoReader, IVideoIndex, IVideoInterface, IDisposable
    {

        #region Fields

        /// <summary>Информация о видеопотоках, присутствующих в записи</summary>
        private List<VideoStreamInfo> _StreamInfoList;

        /// <summary>Кэш коллекции видеоиндексов для каждого видеопотока</summary>
        protected VideoIndexesCache IndexesCashe;

        /// <summary>Идентификатор видеозаписи</summary>
        private string _Id;

        /// <summary>Раздел хранилища, к которому относится видеозапись</summary>
        private VideoPartition _Partition;

        /// <summary>Идентификатор раздела хранилища, к которому относится видеозапись.
        /// Используется только в случае, если ссылка на раздел хранилища отсутствует.
        /// </summary>
        private int _PartitionId;

        /// <summary>Количество предполагаемых по умолчанию видеопотоков</summary>
        public const int DefaultStreamsCount = 4;

        /// <summary>Количество предполагаемых по умослчанию кадров в каждом из потоков
        /// (25 кадров/сек * 1800 сек = 45000 кадров)
        /// </summary>
        public const int DefaultStreamFramesCount = 45000;

        /// <summary>Поток, предназначеный для чтения/записи видеоиндексов</summary>
        protected IVideoIndexesStream Indexes;

        /// <summary>Поток, предназначеный для чтения/записи видеокадров</summary>
        protected IVideoFramesStream Frames;

        /// <summary>Статус запрошенного интерфейса объекта (IVideoWriter, IVideoReader или IVideoIndex)</summary>
        VideoStorageIntStat _Status;

        /// <summary>Признак освобождения ресурсов объекта</summary>
        protected bool disposed;

        /// <summary>Коллекция номеров проследних запрошенных видеокадров для каждого видеопотока</summary>
        Dictionary<int, int> LastIndexNumbers;

        /// <summary>Режим открытия видеозаписи</summary>
        private VideoRecordOpenMode _OpenMode;

        /// <summary>Признак аварийного закрытия видеозаписи</summary>
        protected bool _Aborted;

        /// <summary>Объект блокировки доступа к кэшу индексов</summary>
        object lock_index = new object();

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует значения внутренних переменных</summary>
        BaseVideoRecord()
        {
            _Id = null;
            _Partition = null;
            _PartitionId = -1;
            _StreamInfoList = null;
            Indexes = null;
            Frames = null;
            IndexesCashe = null;
            LastIndexNumbers = null;
            OnDisposed = null;
            disposed = false;
            _OpenMode = VideoRecordOpenMode.ReadWrite;
            _Aborted = false;
        }

        /// <summary>Конструктор класса</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition">Раздел хранилища, к которому относится видеозапись</param>
        /// <param name="mode">Режим открытия видеозаписи</param>
        /// <exception cref="System.ArgumentException">Идентификатор видеозаписи имеет недопустимое значение</exception>
        /// <exception cref="System.ArgumentNullException">Не указан раздел хранилища, к которому относится видеозапись</exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">Возникла ошибка при создании или чтении потоков ввода/вывода</exception>
        public BaseVideoRecord(string id, VideoPartition partition, VideoRecordOpenMode mode)
            : this()
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id");
            }
            if (partition == null)
            {
                throw new ArgumentNullException("partition");
            }
            _Id = id;
            _Partition = partition;
            _StreamInfoList = new List<VideoStreamInfo>(DefaultStreamsCount);
            IndexesCashe = new VideoIndexesCache();
            LastIndexNumbers = new Dictionary<int, int>(DefaultStreamsCount);
            _OpenMode = mode;
            OpenStreams(_Id, _Partition.Path, mode, out Indexes, out Frames);
            try
            {
                ReadStreamInfo(_StreamInfoList, Indexes, Frames);
                // Если индексы существуют - читаем их в кэш
                if (Indexes.Length != 0)
                {
                    IndexesCashe.Read(Indexes);
                    IndexesCashe.Sort();
                }
            }
            catch (Exception E)
            {
                throw new VideoStorageException(E.Message, E);
            }
            _Status = VideoStorageIntStat.Ok;
        }

        /// <summary>Конструктор класса.
        /// Используется только для в случае, если статус запрашиваемого интерфейса заранее известен</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="status">Статус запрашиваемого интерфейса</param>
        public BaseVideoRecord(string id, VideoStorageIntStat status):this()
        {
            // запрещаем использовать методы и свойства объекта...
            disposed = true;
            _Id = id;
            _Status = status;
        }

        /// <summary>Конструктор класса.
        /// Используется только для в случае, если статус запрашиваемого интерфейса заранее известен
        /// </summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздела хранилища, к которому относится видеозапись</param>
        /// <param name="status">Статус запрашиваемого интерфейса</param>
        public BaseVideoRecord(string id, int partition_id, VideoStorageIntStat status)
        {
            // запрещаем использовать методы и свойства объекта...
            disposed = true;
            _Id = id;
            _PartitionId = partition_id;
            _Status = status;
        }

        /// <summary>Деструктор класса</summary>
        ~BaseVideoRecord()
        {
            // Освобождаем неуправляемые ресурсы...
            Dispose(false);
        }

        /// <summary>Создает потоки для чтения/записи видеоиндексов и видеокадров</summary>
        /// <param name="id">Идентификатор открываемой видеозаписи</param>
        /// <param name="path">Путь к каталогу хранения видеозаписи</param>
        /// <param name="mode">Режим открытия потоков</param>
        /// <param name="indexes">Интерфейс потока для чтения/записи видеоиндексов</param>
        /// <param name="frames">Интерфейс потока для чтения/записи видеокадров</param>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Возникла ошибка при создании потоков ввода/вывода
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан идентификатор видеозаписи или путь к ней</exception>
        protected abstract void OpenStreams(string id, string path, VideoRecordOpenMode mode,
                                            out IVideoIndexesStream indexes, out IVideoFramesStream frames);

        /// <summary>Читает информацию о видеопотоках, содержащихся в видеозаписи</summary>
        /// <param name="info">Список с информацией о видеопотоках</param>
        /// <param name="indexes">Интерфейс для чтения индексов видеозаписи</param>
        /// <param name="frames">Интерфейс для чтения видеокадров</param>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Возникла ошибка при чтении информации о видеопотоках
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан идентификатор видеозаписи или путь к ней</exception>
        protected abstract void ReadStreamInfo(IList<VideoStreamInfo> info, IVideoIndexesStream indexes, IVideoFramesStream frames);

        /// <summary>Записывает информацию о видеопотоках, содержащихся в видеозаписи</summary>
        /// <param name="info">Список с информацией о видеопотоках</param>
        /// <param name="indexes">Интерфейс для чтения индексов видеозаписи</param>
        /// <param name="frames">Интерфейс для чтения видеокадров</param>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Возникла ошибка при записи информации о видеопотоках
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан идентификатор видеозаписи или путь к ней</exception>
        protected abstract void WriteStreamInfo(IList<VideoStreamInfo> info, IVideoIndexesStream indexes, IVideoFramesStream frames);

        /// <summary>Высвобождает ресурсы объекта</summary>
        /// <param name="disposing">
        /// Если равен FALSE - освобождаются только неуправляемые ресурсы,
        /// иначе - освобождаются все ресурсы объекта
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            try { if (OnDisposing != null) { OnDisposing(this, EventArgs.Empty); } }
            catch { }
            disposed = true;
            if (_OpenMode == VideoRecordOpenMode.ReadWrite && _Aborted == false)
            {
                // Сохраняем кэш индексов в поток
                try
                {
                    if (Indexes != null)
                    {
                        Indexes.SetLength(0);
                        IndexesCashe.Write(Indexes);
                    }
                }
                catch { }
                // Записываем информацию о видеопотоках в поток с видеокадрами
                try { if (Frames != null) WriteStreamInfo(_StreamInfoList, Indexes, Frames); }
                catch { }
            }
            // Закрываем поток записи/чтения индексов
            try { if (Indexes != null) Indexes.Close(); }
            catch { }
            // Закрываем поток записи/чтения видеокадров
            try { if (Frames != null) Frames.Close(); }
            catch { }
            // Освобождаем управляемые ресурсы
            if (disposing)
            {
                _StreamInfoList = null;
                IndexesCashe = null;
                LastIndexNumbers = null;
                _Partition = null;
                Indexes = null;
                Frames = null;
            }
            try { if (OnDisposed != null) { OnDisposed(this, EventArgs.Empty); } }
            catch { }
        }

        /// <summary>Ищет видеоиндекс для видеопотока cam_id 
        /// с меткой времени видеокадра, наиболее приближенной к заданной параметром current_time
        /// <para>Поиск ведется сначала справа (для значений, больших или равных current_time), а если видеоиндекс не найден, то слева</para>
        /// <para>(для значений, меньших current_time)</para>
        /// </summary>
        /// <param name="cam_id">Идентификатор видеопотока</param>
        /// <param name="current_time">Метка времени видеокадра</param>
        /// <param name="list">Ссылка на список видеоиндексов для заданного видеопотока</param>
        /// <returns>Возвращает порядковый номер видеоиндекса в списке list или минус 1, если ничего не найдено</returns>
        int FindIndex(int cam_id, int current_time, out List<SingleStreamFrameIndex> list)
        {
            return FindIndex(cam_id, current_time, -1, out list);
        }

        /// <summary>Ищет видеоиндекс для видеопотока cam_id 
        /// с меткой времени видеокадра, значение которой укладывается в диапозон [current_time-delta_time/2, current_time+delta_time/2]
        /// <para>Поиск ведется сначала справа (для значений, больших или равных current_time), а если видеоиндекс не найден, то слева</para>
        /// <para>(для значений, меньших current_time)</para>
        /// </summary>
        /// <param name="cam_id">Идентификатор видеопотока</param>
        /// <param name="current_time">Метка времени видеокадра</param>
        /// <param name="delta_time">Интервал времени относительно time_stamp, в пределах которого будет искаться видеокадр</param>
        /// <param name="list">Ссылка на список видеоиндексов для заданного видеопотока</param>
        /// <returns>Возвращает порядковый номер видеоиндекса в списке list или минус 1, если ничего не найдено</returns>
        int FindIndex(int cam_id, int current_time, int delta_time, out List<SingleStreamFrameIndex> list)
        {
            if (!IndexesCashe.Indexes.TryGetValue(cam_id, out list))
            {
                list = null;
                return -1;
            }
            if (list.Count == 0) return -1;
            long half_delta = (delta_time < 0) ? -1 : delta_time / 2;
            SingleStreamFrameIndex model = new SingleStreamFrameIndex(cam_id, current_time, 0);
            // Ищем видеокадр с меткой времени, болше заданной
            int index = list.FindIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampLessOrEqualThen));
            if (index >= 0)
            {
                if (list[index].TimeStamp == current_time) return index;
                if (CheckFindedIndexFromRight(list, index - 1, current_time, half_delta)) return index - 1;
                else if (CheckFindedIndexFromLeft(list, index, current_time, half_delta)) return index;
            }
            else if (CheckFindedIndexFromRight(list, list.Count - 1, current_time, half_delta)) return list.Count - 1;
            // Ничего не найдено....
            return -1;
        }

        /// <summary>Вспомогательная функция поиска видеоиндекса. Проверяет значение метки времени видеоиндекса</summary>
        /// <param name="list">Список видеоиндексов</param>
        /// <param name="index">Номер проверяемого видеоиндекса в списке list</param>
        /// <param name="value">Значение метки времени видеоиндекса</param>
        /// <param name="delta">Допустимое положительное приращение value</param>
        /// <returns>
        /// Возвращает TRUE в случае, если метка времени видеонидекса принадлежит диапозону [value-delta, delta]
        /// </returns>
        bool CheckFindedIndexFromRight(List<SingleStreamFrameIndex> list, int index, int value, long delta)
        {
            if (index >= 0)
            {
                if (delta < 0)
                {
                    return true;
                }
                else if ((list[index].TimeStamp >= value - delta) && (list[index].TimeStamp <= value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Вспомогательная функция поиска видеоиндекса. Проверяет значение метки времени видеоиндекса</summary>
        /// <param name="list">Список видеоиндексов</param>
        /// <param name="index">Номер проверяемого видеоиндекса в списке list</param>
        /// <param name="value">Значение метки времени видеоиндекса</param>
        /// <param name="delta">Допустимое отрицательное приращение value</param>
        /// <returns>
        /// Возвращает TRUE в случае, если метка времени видеонидекса принадлежит диатозону [value, value+delta]
        /// </returns>
        bool CheckFindedIndexFromLeft(List<SingleStreamFrameIndex> list, int index, int value, long delta)
        {
            if (index >= 0)
            {
                if (delta < 0) return true;
                else if ((list[index].TimeStamp >= value) && (list[index].TimeStamp <= value + delta)) return true;
            }
            return false;
        }

        /// <summary>Добавляет информацию о видеопотоке,
        /// если упоминание о видеопотоке, к которому принадлежит видеокадр, еще не встречалось...
        /// </summary>
        /// <param name="frame">Видеокадр</param>
        /// <exception cref="System.ArgumentNullException">Не задан видеокадр</exception>
        /// <returns>Возвращает TRUE если информация была добавлена, иначе возвращает FALSE</returns>
        protected bool AddToStreamInfo(VideoFrame frame)
        {
            if (frame == null) return false;
            VideoStreamInfo model = new VideoStreamInfo(frame.CameraId, frame.ContentType.ToString());
            if (!_StreamInfoList.Exists(new Predicate<VideoStreamInfo>(model.EqualId)))
            {
                _StreamInfoList.Add(model);
                return true;
            }
            return false;
        }

        #endregion

        #region Methods Static

        /// <summary>Получить расширение файлов с видеоданными для заданной реализации хранилища</summary>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <returns>Расширение файлов с видеоданными</returns>
        protected static string GetFramesFileExt(VideoStorageType storage_type)
        {
            switch (storage_type)
            {
                case VideoStorageType.ASKO:
                    return ".mjpg";

                case VideoStorageType.Original:
                    return ".frames";

                default:
                    return string.Empty;
            }
        }

        /// <summary>Получить расширение файлов с индексами кадров видеозаписи для заданной реализации хранилища</summary>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <returns>Расширение файлов с индексами кадров видеозаписи</returns>
        protected static string GetIndexesFileExt(VideoStorageType storage_type)
        {
            switch (storage_type)
            {
                case VideoStorageType.ASKO:
                case VideoStorageType.Original:
                    return ".index";

                default:
                    return string.Empty;
            }
        }

        /// <summary>Возвращает массив идентификаторов видеозаписей, расположенных по заданному пути</summary>
        /// <param name="path">Путь, по которому будет выполнятся поиск видеозаписей</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentException">
        /// Параметр path представляет собой строку нулевой длины, содержащую только пробелы 
        /// или один или несколько недопустимых символов
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Параметр path имеет значение null
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">
        /// Длина указанного пути, имени файла или обоих параметров превышает установленный системой предел
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Указанный путь недопустим
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// Недостаточно прав для получения списка файлов
        /// </exception>
        /// <returns>Список идентификаторов найденных видеозаписей или пустой список, если ничего не найдено</returns>
        public static IList<string> GetRecords(string path, VideoStorageType storage_type)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            return GetRecords(directory, storage_type);
        }

        /// <summary>Возвращает массив идентификаторов видеозаписей, расположенных по заданному пути</summary>
        /// <param name="directory">Директория, в которой будет выполнятся поиск видеозаписей</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentNullException">
        /// Параметр directory имеет значение null
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Указанный путь недопустим
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// Недостаточно прав для получения списка файлов
        /// </exception>
        /// <returns>Список идентификаторов найденных видеозаписей или пустой список, если ничего не найдено</returns>
        public static IList<string> GetRecords(DirectoryInfo directory, VideoStorageType storage_type)
        {
            if (directory == null)
            {
                throw new ArgumentNullException();
            }
            IList<string> result = new List<string>();
            FileInfo[] files = directory.GetFiles("*" + BaseVideoRecord.GetFramesFileExt(storage_type));
            if (files.Length == 0)
            {
                return result;
            }
            for (int i = 0; i < files.Length; i++)
            {
                string file_name = files[i].Name;
                result.Add(file_name.Substring(0, file_name.IndexOf('.')));
            }
            return result;
        }

        /// <summary>Возвращает размер в байтах, используемый для хранения записи с заданным идентификатором</summary>
        /// <param name="path">Путь, по которому размещена запись</param>
        /// <param name="id">Идентификотор записи</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentException">
        /// Параметр path представляет собой строку нулевой длины, содержащую только пробелы 
        /// или один или несколько недопустимых символов; параметр id равен пустой строке
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Параметр path или id имеет значение null
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">
        /// Длина указанного пути, имени файла или обоих параметров превышает установленный системой предел
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Указанный путь недопустим
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// Недостаточно прав для получения списка файлов
        /// </exception>
        /// <returns>Размер в байтах, занимаемый записью, или минус 1, если запись не найдена</returns>
        public static long GetRecordSize(string path, string id, VideoStorageType storage_type)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            return GetRecordSize(directory, id, storage_type);
        }

        /// <summary>Возвращает размер в байтах, используемый для хранения записи с заданным идентификатором</summary>
        /// <param name="directory">Директория, в которой размещена запись</param>
        /// <param name="id">Идентификотор записи</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentException">
        /// Параметр id равен пустой строке
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Параметр directory или id имеет значение null
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Указанный путь недопустим
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// Недостаточно прав для получения списка файлов
        /// </exception>
        /// <returns>Размер в байтах, занимаемый записью, или минус 1, если запись не найдена</returns>
        public static long GetRecordSize(DirectoryInfo directory, string id, VideoStorageType storage_type)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id == string.Empty)
            {
                throw new ArgumentException();
            }
            long result = 0;
            FileInfo[] files = directory.GetFiles(id + BaseVideoRecord.GetFramesFileExt(storage_type));
            if (files.Length != 0)
            {
                result += files[0].Length;
            }
            files = directory.GetFiles(id + BaseVideoRecord.GetIndexesFileExt(storage_type));
            if (files.Length != 0)
            {
                result += files[0].Length;
            }
            return result;
        }

        /// <summary>Удаляет видеозапись с физического или виртуального носителя информации</summary>
        /// <param name="path">Путь, по которому размещена запись</param>
        /// <param name="id">Идентификотор записи</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentException">
        /// Параметр path представляет собой строку нулевой длины, содержащую только пробелы 
        /// или один или несколько недопустимых символов; параметр id равен пустой строке
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Параметр path или id имеет значение null
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">
        /// Длина указанного пути, имени файла или обоих параметров превышает установленный системой предел
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Указанный путь недопустим
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// Недостаточно прав для получения списка или удаления файлов
        /// </exception>
        public static void Delete(string path, string id, VideoStorageType storage_type)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            Delete(directory, id, storage_type);
        }

        /// <summary>Удаляет видеозапись с физического или виртуального носителя информации</summary>
        /// <param name="directory">Директория, в которой размещена запись</param>
        /// <param name="id">Идентификотор записи</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentException">
        /// Параметр id равен пустой строке
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Параметр directory или id имеет значение null
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Указанный путь недопустим
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// Недостаточно прав для получения списка или удаления файлов
        /// </exception>
        public static void Delete(DirectoryInfo directory, string id, VideoStorageType storage_type)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id == string.Empty)
            {
                throw new ArgumentException();
            }
            FileInfo[] files = directory.GetFiles(id + BaseVideoRecord.GetFramesFileExt(storage_type));
            if (files.Length != 0)
            {
                files[0].Delete();
            }
            files = directory.GetFiles(id + BaseVideoRecord.GetIndexesFileExt(storage_type));
            if (files.Length != 0)
            {
                files[0].Delete();
            }
        }

        /// <summary>Изменяет идентификатор существующей записи</summary>
        /// <param name="path">Директория, в которой расположена видеозапись</param>
        /// <param name="old_id">Текущий идентификатор записи</param>
        /// <param name="new_id">Новый идентификатор записи</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentNullException">
        /// Один или несколько параметров имеют значение null
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Параметр path, old_id и/или new_id является пустой скрокой, содержит только пробелы или включает недопустимые знаки
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Не найдена видеозапись с идентификатором old_id или она повреждена (отсутствует файл индексов)
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// Отказано в доступе к файлам видеозаписи
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">
        /// Длина указанного пути, идентификатора видиозаписи или обоих параметров превышает установленный системой предел
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Идентификатор видеозаписи содержит запрещенный симвом ":"
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Видеозапись с идентификатором new_id уже существует
        /// </exception>
        /// <remarks>
        /// Обязательным условием успешного переименования является отсутствие записи с номым
        /// идентификатором в указанной директории на момент переименования
        /// </remarks>
        public static void Rename(string path, string old_id, string new_id, VideoStorageType storage_type)
        {
            if (path == null)
            {
                throw new ArgumentNullException("directory");
            }
            if (old_id == null)
            {
                throw new ArgumentNullException("old_id");
            }
            if (old_id == string.Empty)
            {
                throw new ArgumentException("old_id");
            }
            if (new_id == null)
            {
                throw new ArgumentNullException("new_id");
            }
            if (new_id == string.Empty)
            {
                throw new ArgumentException("new_id");
            }
            // Корректируем путь в файлу
            int path_len = path.Length;
            if ((path_len != 0) && (path[path_len - 1] != '\\'))
            {
                path += "\\";
            }
            // Переименовываем файл с видеоданными, создавая копию старого файла
            string file_ext = BaseVideoRecord.GetFramesFileExt(storage_type);
            File.Move(path + old_id + file_ext, path + new_id + file_ext);
            try
            {
                // Переименовываем индексный файл
                file_ext = BaseVideoRecord.GetIndexesFileExt(storage_type);
                File.Move(path + old_id + file_ext, path + new_id + file_ext);
            }
            catch
            {
                // Если при переименовании индексов произошло исключение - пытаемся вернуть
                // вайлу с видеоданными прежнее имя
                try
                {
                    file_ext = BaseVideoRecord.GetFramesFileExt(storage_type);
                    File.Move(path + new_id + file_ext, path + old_id + file_ext);
                }
                catch { }
                throw;
            }
        }

        /// <summary>Изменяет идентификатор существующей записи</summary>
        /// <param name="directory">Директория, в которой расположена видеозапись</param>
        /// <param name="old_id">Текущий идентификатор записи</param>
        /// <param name="new_id">Новый идентификатор записи</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentNullException">
        /// Один или несколько параметров имеют значение null
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Параметр path, old_id и/или new_id является пустой скрокой, содержит только пробелы или включает недопустимые знаки
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Не найдена видеозапись с идентификатором old_id или она повреждена (отсутствует файл индексов)
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// Отказано в доступе к файлам видеозаписи
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">
        /// Длина указанного пути, идентификатора видиозаписи или обоих параметров превышает установленный системой предел
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Идентификатор видеозаписи содержит запрещенный симвом ":"
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Видеозапись с идентификатором new_id уже существует
        /// </exception>
        /// <remarks>
        /// Обязательным условием успешного переименования является отсутствие записи с номым
        /// идентификатором в указанной директории на момент переименования
        /// </remarks>
        public static void Rename(DirectoryInfo directory, string old_id, string new_id, VideoStorageType storage_type)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }
            Rename(directory.FullName, old_id, new_id, storage_type);
        }

        /// <summary>Возвращает признак существования видеозаписи с заданным идентификатором
        /// в указанной директории
        /// </summary>
        /// <param name="path">Директория для поиска видеозаписи</param>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="ArgumentException">
        /// Параметр id равен пустой строке или параметр path содержит недопустимые знаки
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Параметр path или id имеет значение null
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// Недостаточно прав для доступа к директории
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// Длина указанного пути превышает установленный системой предел
        /// </exception>
        /// <returns>
        /// Возвращает TRUE если такая видеозапись существует и FALSE - если такой записи
        /// не существует в заданной директории
        /// </returns>
        public static bool Exist(string path, string id, VideoStorageType storage_type)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            return Exist(directory, id, storage_type);
        }

        /// <summary>Возвращает признак существования видеозаписи с заданным идентификатором
        /// в указанной директории
        /// </summary>
        /// <param name="directory">Директория для поиска видеозаписи</param>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="storage_type">Тип хранилища видеоданных</param>
        /// <exception cref="System.ArgumentException">
        /// Параметр id равен пустой строке
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Параметр directory или id имеет значение null
        /// </exception>
        /// <returns>
        /// Возвращает TRUE если такая видеозапись существует и FALSE - если такой записи
        /// не существует в заданной директории
        /// </returns>
        public static bool Exist(DirectoryInfo directory, string id, VideoStorageType storage_type)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            try
            {
                FileInfo[] files = directory.GetFiles(id + BaseVideoRecord.GetFramesFileExt(storage_type));
                bool frames_exist = files.Length == 1;
                files = directory.GetFiles(id + BaseVideoRecord.GetIndexesFileExt(storage_type));
                bool index_exist = files.Length == 1;
                return frames_exist & index_exist;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Properties

        /// <summary>Место на диске, используемое для хранения видеозаписи (байт)</summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Попытка обращения к свойству или методу уничтоженного объекта
        /// </exception>
        public long UserSpace
        {
            get
            {
                return Indexes.Length + Indexes.HeaderSize + Frames.Length + Frames.HeaderSize;
            }
        }

        /// <summary>Ссылка на раздел, к которому принадлежит видеозапись</summary>
        public VideoPartition Partition
        {
            get { return _Partition; }
        }

        /// <summary>Признак аварийного закрытия записи</summary>
        public bool Aborted
        {
            get { return _Aborted; }
        }

        #endregion

        #region Events

        /// <summary>Извещает об окончании работы с видеозаписью</summary>
        public event EventHandler OnDisposed;

        /// <summary>Извещает о начале процедуры освобождения ресурсов объекта</summary>
        public event EventHandler OnDisposing;

        #endregion

        #region Variables

        object lock_read = new object();

        #endregion

        #region IDisposable Members

        /// <summary>Высвобождает управляемые и неуправляемые ресурсы
        /// <see cref="System.IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IVideoWriter Members

        /// <summary>Записать кадр
        /// <see cref="AlfaPribor.VideoStorage.IVideoWriter"/></summary>
        /// <param name="camera_id">Номер камеры (видеопотока)</param>
        /// <param name="time_stamp">Время от начала записи в миллисекундах</param>
        /// <param name="content_type">Тип содержимого кадра, например: image/jpeg или image/raw</param>
        /// <param name="frame_data">Бинарные данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Результат выполнения операции</returns>
        public virtual VideoStorageResult WriteFrame(int camera_id, int time_stamp, string content_type, byte[] frame_data)
        {
            VideoFrame frame = new VideoFrame(camera_id, time_stamp, content_type, frame_data);
            return WriteFrame(frame);
        }

        /// <summary>Записать кадр
        /// <see cref="AlfaPribor.VideoStorage.IVideoWriter"/></summary>
        /// <param name="frame">Данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <exception cref="System.ArgumentException">Не задан видеокадр</exception>
        /// <returns>Результат выполнения операции</returns>
        public virtual VideoStorageResult WriteFrame(VideoFrame frame)
        {
            lock (lock_read)    // !!! С блокировкой чтения/записи
            {
                if (disposed) { throw new ObjectDisposedException(this.ToString()); }
                if (frame == null) { throw new ArgumentNullException(); }
                long Position;
                long Length;
                try
                {
                    //Корректировка позиции в файле
                    if (Frames.Length != Frames.Position) Frames.Position = Frames.Length;
                    // Запоминаем длину потока и позицию курсора в нем, чтобы можно было
                    // вернуть поток к этому состоянию при неудачной записи индекса
                    Length = Frames.Length;
                    Position = Frames.Position;
                    int k = Frames.WriteFrame(frame);
                    if (k == 0) { }
                }
                catch { return VideoStorageResult.Fault; }
                SingleStreamFrameIndex index;
                try
                {
                    index = new SingleStreamFrameIndex(frame.CameraId, frame.TimeStamp, Position + Frames.HeaderSize);
                    Indexes.WriteIndex(index);
                }
                catch
                {
                    // Если не получилось записать индекс в поток, пытаемся вернуть поток с видеокадрами в предыдущее состояние
                    try
                    {
                        Frames.SetLength(Length);
                        Frames.Position = Position;
                    }
                    catch { }
                    return VideoStorageResult.Fault;
                }

                lock (lock_index)   //!!! блокировка кэша индексов
                {
                    IndexesCashe.Add(index, true);
                }

                AddToStreamInfo(frame);
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Аварийное закрытие записи
        /// <see cref="AlfaPribor.VideoStorage.IVideoWriter"/>
        /// </summary>
        /// <returns>Результат выполнения операции</returns>
        public virtual VideoStorageResult Abort()
        {
            _Aborted = true;
            Dispose(false);
            return VideoStorageResult.Ok;
        }

        #endregion

        #region IVideoInterface Members

        /// <summary>Идентификатор видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.IVideoInterface"/>
        /// </summary>
        public string Id
        {
            get { return _Id; }
        }

        /// <summary>Раздел хранилища, к которому относится видеозапись
        /// <see cref="AlfaPribor.VideoStorage.IVideoInterface"/>
        /// </summary>
        public int PartitionId
        {
            get
            {
                return (_Partition == null) ? _PartitionId : _Partition.Id;
            }
        }

        /// <summary>Статус запрошенного интерфейса объекта (IVideoWriter, IVideoReader или IVideoIndex)
        /// <see cref="AlfaPribor.VideoStorage.IVideoInterface"/>
        /// </summary>
        public VideoStorageIntStat Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>Завершить чтение / закрыть поток
        /// <see cref="AlfaPribor.VideoStorage.IVideoWriter"/>
        /// </summary>
        /// <returns>Результат выполнения операции</returns>
        public virtual VideoStorageResult Close()
        {
            Dispose(false);
            return VideoStorageResult.Ok;
        }

        #endregion


        #region IVideoReader Members

        /// <summary>Получить интерфейс индекса видеоданных
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/></summary>
        public IVideoIndex VideoIndex
        {
            get { return (IVideoIndex)this; }
        }

        /// <summary>Прочитать кадр указанной камеры, соответствующий указанному моменту времени
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/></summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="frame">Данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFrame(int cam_id, int time_stamp, out VideoFrame frame)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                List<SingleStreamFrameIndex> IndexesList;
                int i = FindIndex(cam_id, time_stamp, out IndexesList);
                if (i < 0)
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                try
                {
                    long offset = IndexesList[i].FileOffset;
                    Frames.Position = offset - Frames.HeaderSize;
                    Frames.ReadFrame(out frame);
                    LastIndexNumbers[cam_id] = i;
                }
                catch
                {
                    frame = null;
                    return VideoStorageResult.Fault;
                }
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Прочитать кадр указанной камеры, соответствующий указанному моменту времени
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="delta_time">Интервал времени относительно time_stamp, в пределах которого будет искаться видеокадр</param>
        /// <param name="frame">Данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFrame(int cam_id, int time_stamp, int delta_time, out VideoFrame frame)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                List<SingleStreamFrameIndex> IndexesList;
                int i = FindIndex(cam_id, time_stamp, delta_time, out IndexesList);
                if (i < 0)
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                try
                {
                    long offset = IndexesList[i].FileOffset;
                    Frames.Position = offset - Frames.HeaderSize;
                    Frames.ReadFrame(out frame);
                    LastIndexNumbers[cam_id] = i;
                }
                catch
                {
                    frame = null;
                    return VideoStorageResult.Fault;
                }
                return VideoStorageResult.Ok;
            }            
        }

        /// <summary>Прочитать кадры всех каналов соответствующие указанному моменту времени
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/>
        /// </summary>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="frames">Список кадров</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFrames(int time_stamp, out IList<VideoFrame> frames)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                // Создаем список для хранения видеокадров с емкостью, равной количеству видеопотоков
                frames = new List<VideoFrame>(IndexesCashe.Indexes.Count);
                // Если индексы не были прочитаны - видеозапись не существует
                if (IndexesCashe.Indexes.Count == 0)
                {
                    return VideoStorageResult.NotFound;
                }
                try
                {
                    foreach (KeyValuePair<int, List<SingleStreamFrameIndex>> item in IndexesCashe.Indexes)
                    {
                        List<SingleStreamFrameIndex> IndexesList;
                        int i = FindIndex(item.Key, time_stamp, out IndexesList);
                        // Если индекс не существует - нет кадра, удовлетворяющего
                        // заданному критерию. Продолжаем поиск для следующего видеопотока
                        if (i < 0)
                        {
                            continue;
                        }
                        // Если индекс существует - читаем видеокадр
                        else
                        {
                            try
                            {
                                VideoFrame frame;
                                long offset = IndexesList[i].FileOffset;
                                Frames.Position = offset - Frames.HeaderSize;
                                Frames.ReadFrame(out frame);
                                LastIndexNumbers[item.Key] = i;
                                frames.Add(frame);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                    //Восстановление курсора после записи
                    Frames.Position = Frames.Length;
                }
                catch
                {
                    return VideoStorageResult.Fault;
                }
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Прочитать кадры всех каналов соответствующие указанному моменту времени
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/></summary>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="frames">Список кадров</param>
        /// <param name="error">Текст ошибки</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFrames(int time_stamp, out IList<VideoFrame> frames, ref string error)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed) { throw new ObjectDisposedException(this.ToString()); }
                // Создаем список для хранения видеокадров с емкостью, равной количеству видеопотоков
                frames = new List<VideoFrame>(IndexesCashe.Indexes.Count);
                // Если индексы не были прочитаны - видеозапись не существует
                if (IndexesCashe.Indexes.Count == 0) return VideoStorageResult.NotFound;
                try
                {
                    string message = "";
                    foreach (KeyValuePair<int, List<SingleStreamFrameIndex>> item in IndexesCashe.Indexes)
                    {
                        List<SingleStreamFrameIndex> IndexesList;
                        int i = FindIndex(item.Key, time_stamp, out IndexesList);
                        // Если индекс не существует - нет кадра, удовлетворяющего
                        // заданному критерию. Продолжаем поиск для следующего видеопотока
                        if (i < 0) continue;
                        // Если индекс существует - читаем видеокадр
                        else
                        {
                            try
                            {
                                VideoFrame frame;
                                long offset = IndexesList[i].FileOffset;
                                Frames.Position = offset - Frames.HeaderSize;
                                error += "Frames.Position:" + Frames.Position.ToString() + " index:" + i.ToString();
                                //for (int j = 0; j <= i; j++)
                                //    error += "IndexesList[" + j.ToString() + "] TimeStamp:" + IndexesList[j].TimeStamp + " FileOffset:" + IndexesList[j].FileOffset + "\r\n";
                                message = "";
                                Frames.ReadFrame(out frame, out message);
                                //error += " mess:" + message + "\r\n";
                                //error += "cam_id:" + frame.CameraId.ToString() + ", framedata:" + frame.FrameData.Length.ToString() + ", timestamp:" + frame.TimeStamp.ToString() + "\r\n";
                                LastIndexNumbers[item.Key] = i;
                                frames.Add(frame);
                            }
                            catch (Exception e)
                            {
                                error += "\r\n" + e.Source + " " + e.Message + ", mess:" + message + "\r\n";
                                return VideoStorageResult.Fault;
                            }
                        }
                    }
                    //Восстановление курсора после записи
                    Frames.Position = Frames.Length;
                }
                catch
                {
                    return VideoStorageResult.Fault;
                }
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Прочитать кадры всех каналов соответствующие указанному моменту времени
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/>
        /// </summary>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="delta_time">Интервал времени относительно time_stamp, в пределах которого будет искаться видеокадр</param>
        /// <param name="frames">Список кадров</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFrames(int time_stamp, int delta_time, out IList<VideoFrame> frames)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                // Создаем список для хранения видеокадров с емкостью, равной количеству видеопотоков
                frames = new List<VideoFrame>(IndexesCashe.Indexes.Count);
                // Если индексы не были прочитаны - видеозапись не существует
                if (IndexesCashe.Indexes.Count == 0)
                {
                    return VideoStorageResult.NotFound;
                }
                try
                {
                    foreach (KeyValuePair<int, List<SingleStreamFrameIndex>> item in IndexesCashe.Indexes)
                    {
                        List<SingleStreamFrameIndex> IndexesList;
                        int i = FindIndex(item.Key, time_stamp, delta_time, out IndexesList);
                        // Если индекс не существует - нет кадра, удовлетворяющего
                        // заданному критерию. Продолжаем поиск для следующего видеопотока
                        if (i < 0)
                        {
                            continue;
                        }
                        // Если индекс существует - читаем видеокадр
                        else
                        {
                            try
                            {
                                VideoFrame frame;
                                long offset = IndexesList[i].FileOffset;
                                Frames.Position = offset - Frames.HeaderSize;
                                Frames.ReadFrame(out frame);
                                LastIndexNumbers[item.Key] = i;
                                frames.Add(frame);
                            }
                            catch
                            {
                            }
                        }
                    }
                    //Восстановление курсора после записи
                    Frames.Position = Frames.Length;
                }
                catch
                {
                    return VideoStorageResult.Fault;
                }
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Прочитать первый кадр указанного видеопотока
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="frame">Данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Объект уничтожен
        /// </exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadFirstFrame(int cam_id, out VideoFrame frame)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                if (!IndexesCashe.Indexes.ContainsKey(cam_id))
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                List<SingleStreamFrameIndex> list = IndexesCashe.Indexes[cam_id];
                if (list.Count == 0)
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                else
                {
                    try
                    {
                        long offset = list[0].FileOffset;
                        Frames.Position = offset - Frames.HeaderSize;
                        Frames.ReadFrame(out frame);
                        Frames.Position = Frames.Length;    //Восстановление курсора
                        LastIndexNumbers[cam_id] = 0;
                    }
                    catch
                    {
                        frame = null;
                        Frames.Position = Frames.Length;    //Восстановление курсора
                        return VideoStorageResult.Fault;
                    }
                }
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Прочитать последний кадр указанного видеопотока
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="frame">Данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadLastFrame(int cam_id, out VideoFrame frame)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                if (!IndexesCashe.Indexes.ContainsKey(cam_id))
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                List<SingleStreamFrameIndex> list = IndexesCashe.Indexes[cam_id];
                if (list.Count == 0)
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                else
                {
                    int index = list.Count - 1;
                    try
                    {
                        long offset = list[index].FileOffset;
                        Frames.Position = offset - Frames.HeaderSize;
                        Frames.ReadFrame(out frame);
                        Frames.Position = Frames.Length;    //Восстановление курсора
                        LastIndexNumbers[cam_id] = index;
                    }
                    catch
                    {
                        frame = null;
                        Frames.Position = Frames.Length;    //Восстановление курсора
                        return VideoStorageResult.Fault;
                    }
                }
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Прочитать следующий кадр (от последнего прочитанного) указанного видеопотока
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="step">Инкремент номера кадра</param>
        /// <param name="frame">Данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Объект уничтожен
        /// </exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadNextFrame(int cam_id, int step, out VideoFrame frame)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                if (!IndexesCashe.Indexes.ContainsKey(cam_id))
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                List<SingleStreamFrameIndex> list = IndexesCashe.Indexes[cam_id];
                if (list.Count == 0)
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                int index;
                if (!LastIndexNumbers.TryGetValue(cam_id, out index))
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                index += step;
                if (index >= list.Count)
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                try
                {
                    long offset = list[index].FileOffset;
                    Frames.Position = offset - Frames.HeaderSize;
                    Frames.ReadFrame(out frame);
                    Frames.Position = Frames.Length;    //Восстановление курсора
                    LastIndexNumbers[cam_id] = index;
                }
                catch
                {
                    frame = null;
                    Frames.Position = Frames.Length;    //Восстановление курсора
                    return VideoStorageResult.Fault;
                }
                return VideoStorageResult.Ok;
            }
        }

        /// <summary>Прочитать предыдущий кадр (от последнего прочитанного) указанного видеопотока
        /// <see cref="AlfaPribor.VideoStorage.IVideoReader"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="step">Инкремент номера кадра</param>
        /// <param name="frame">Данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Результат выполнения операции</returns>
        public VideoStorageResult ReadPrevFrame(int cam_id, int step, out VideoFrame frame)
        {
            lock (lock_read)// !!! С блокировкой чтения/записи
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                if (!IndexesCashe.Indexes.ContainsKey(cam_id))
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                List<SingleStreamFrameIndex> list = IndexesCashe.Indexes[cam_id];
                if (list.Count == 0)
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                int index;
                if (!LastIndexNumbers.TryGetValue(cam_id, out index))
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                index -= step;
                if (index < 0)
                {
                    frame = null;
                    return VideoStorageResult.NotFound;
                }
                try
                {
                    long offset = list[index].FileOffset;
                    Frames.Position = offset - Frames.HeaderSize;
                    Frames.ReadFrame(out frame);
                    Frames.Position = Frames.Length;    //Восстановление курсора
                    LastIndexNumbers[cam_id] = index;
                }
                catch
                {
                    frame = null;
                    Frames.Position = Frames.Length;    //Восстановление курсора
                    return VideoStorageResult.Fault;
                }
                return VideoStorageResult.Ok;
            }
        }

        #endregion

        #region IVideoIndex Members

        #region Properties

        /// <summary>Возвращает информацию о видеопотоках, содержащихся в видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Объект был уничтожен
        /// </exception>
        public IList<VideoStreamInfo> StreamInfoList
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                return _StreamInfoList.AsReadOnly();
            }
        }

        /// <summary>Дата/время начала видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Объект был уничтожен
        /// </exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Ошибка чтения/записи информации о времени начала видеозаписи
        /// </exception>
        public DateTime RecordStarted
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                try
                {
                    return Frames.RecordStarted;
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception E)
                {
                    throw new VideoStorageException("Can not set 'RecordStarted' value!", E);
                }
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                try
                {
                    Frames.RecordStarted = value;
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception E)
                {
                    throw new VideoStorageException("Can not set 'RecordStarted' value!", E);
                }
            }
        }

        /// <summary>Дата/время окончания видеозаписи
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Объект был уничтожен
        /// </exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Ошибка чтения/записи информации о времени окончания видеозаписи
        /// </exception>
        public DateTime RecordFinished
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                try
                {
                    return Frames.RecordFinished;
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception E)
                {
                    throw new VideoStorageException("Can not set 'RecordFinished' value!", E);
                }
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                try
                {
                    Frames.RecordFinished = value;
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception E)
                {
                    throw new VideoStorageException("Can not set 'RecordFinished' value!", E);
                }
            }
        }

        /// <summary>Режим открытия</summary>
        public VideoRecordOpenMode OpenMode
        {
            get { return _OpenMode; }
        }

        #endregion

        #region Methods

        /// <summary>Получить метку времени самого первого кадра (по всем видеопотокам)
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
        /// </summary>
        /// <param name="delta">Время от начала записи, в пределах которого должен находится первый кадр</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        ///<returns>Метка времени или -1 если не найдено</returns>
        public int GetStartTime(int delta)
        {
            if (disposed) { throw new ObjectDisposedException(this.ToString()); }
            // Выделяем память для хранения результатов поиска кадров для каждого видеопотока
            List<int> TimeStamps = new List<int>(IndexesCashe.Indexes.Count);
            // Ищем метки времени первых кадров для всех потоков и выбираем самую минимальную
            foreach (KeyValuePair<int, List<SingleStreamFrameIndex>> item in IndexesCashe.Indexes)
            {
                if (item.Value.Count > 0)
                {
                    SingleStreamFrameIndex index = item.Value[0];
                    if (index != null) TimeStamps.Add(index.TimeStamp);
                }
            }
            int limit = TimeStamps.Count > 0 ? TimeStamps.Min() + delta : delta;
            TimeStamps.Clear();
            // Ищем...
            foreach (KeyValuePair<int,List<SingleStreamFrameIndex>> item in IndexesCashe.Indexes)
            {
                if (item.Value == null) continue;
                SingleStreamFrameIndex model = new SingleStreamFrameIndex(item.Key, limit, 0);
                int i = item.Value.FindIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampGreaterOrEqualThen));
                if (i >= 0) TimeStamps.Add(item.Value[i].TimeStamp);
            }
            // Если есть результаты - возвращаем самое большое временное смещение кадра
            if (TimeStamps.Count != 0) return TimeStamps.Max();
            else return -1;
        }

        /// <summary>Получить метку времени самого последнего кадра (по всем видеопотокам)
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/></summary>
        /// <param name="delta">Допуск на разность меток времени кадров для каждого видеопотока</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Метка времени или -1 если не найдено</returns>
        public int GetFinishTime(int delta)
        {
            if (disposed) { throw new ObjectDisposedException(this.ToString()); }
            //Список меток времени видеопотоков
            List<int> TimeStamps = new List<int>(IndexesCashe.Indexes.Count);
            // Ищем метки времени последних кадров для всех потоков и выбираем самую максимальную
            foreach (KeyValuePair<int, List<SingleStreamFrameIndex>> item in IndexesCashe.Indexes)
            {
                if (item.Value.Count > 0)
                {
                    SingleStreamFrameIndex index = item.Value[item.Value.Count - 1];//Индекс последнего кадра
                    if (index != null) TimeStamps.Add(index.TimeStamp);
                }
            }

            int limit = TimeStamps.Count > 0 ? TimeStamps.Max() - delta : delta;
            if (limit < 0) limit = 0;

            TimeStamps.Clear();

            // Ищем...
            foreach (KeyValuePair<int, List<SingleStreamFrameIndex>> item in IndexesCashe.Indexes)
            {
                if (item.Value == null) continue;
                SingleStreamFrameIndex model = null;
                List<SingleStreamFrameIndex> list = null;
                model = new SingleStreamFrameIndex(item.Key, limit, 0);

                try
                {
                    lock (lock_index)
                    {
                        //Падает здесь при частом вызове у объекта, который записывается, вероятно конфликт чтения/записи
                        //Блокировка помогла
                        list = item.Value.ToList(); 
                    }
                }
                catch (Exception e)
                {
                }

                if (list != null) list.Reverse();
                if (list != null && model != null)
                {
                    //Поиск индекса минимального
                    int i = list.FindIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampLessOrEqualThen));
                    if (i >= 0) TimeStamps.Add(list[i].TimeStamp);
                }
            }
            // Если есть результаты - возвращаем самое большое временное смещение кадра
            if (TimeStamps.Count != 0) return TimeStamps.Min();
            else return -1;
        }

        /// <summary>Получить кэшированную метку времени самого последнего кадра (по всем видеопотокам)
        /// Ускоренный метод</summary>
        /// <param name="delta">Допуск на разность меток времени кадров для каждого видеопотока</param>
        /// <returns>Метка времени или -1 если не найдено</returns>
        public int GetFinishTimeFast(int delta)
        {

            return 0;
        }

        /// <summary>Получить метку времени следующего кадра (мсек)
            /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
            /// </summary>
            /// <param name="cam_id">Номер камеры / видеопотока</param>
            /// <param name="current_time">Метка времени текущего кадра</param>
            /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
            /// <returns>Метка времени или минус 1 если следующий кадр не найден</returns>
        public int GetNextFrameTime(int cam_id, int current_time)
        {
            if (disposed) { throw new ObjectDisposedException(this.ToString()); }
            List<SingleStreamFrameIndex> IndexesList;
            if (!IndexesCashe.Indexes.TryGetValue(cam_id, out IndexesList)) return -1;
            if (IndexesList.Count == 0) return -1;

            SingleStreamFrameIndex model = new SingleStreamFrameIndex(cam_id, current_time, 0);
            // Ищем видеокадр с меткой времени, болше заданной
            int index = IndexesList.FindIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampLessThen));
            if (index < 0) return -1;
            else return IndexesList[index].TimeStamp;
        }

        /// <summary>Получить метку времени следующего кадра (мсек) для всех телекамер<see cref="AlfaPribor.VideoStorage.IVideoIndex"/></summary>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Метка времени или -1 если следующий кадр не найден</returns>
        public int GetNextFrameTime(int current_time)
        {
            int timestamp = -1;
            try
            {
                if (disposed) { throw new ObjectDisposedException(this.ToString()); }
                for (int cam_id = 0; cam_id < IndexesCashe.Indexes.Count; cam_id++)
                {
                    List<SingleStreamFrameIndex> IndexesList;
                    if (!IndexesCashe.Indexes.TryGetValue(cam_id, out IndexesList)) continue;
                    if (IndexesList.Count == 0) continue;
                    SingleStreamFrameIndex model = new SingleStreamFrameIndex(cam_id, current_time, 0);
                    // Ищем видеокадр с меткой времени, больше заданной
                    int index = IndexesList.FindIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampLessThen));
                    if (index < 0) continue;
                    //Первая метка
                    if (timestamp == -1) timestamp = IndexesList[index].TimeStamp;
                    else if (IndexesList[index].TimeStamp < timestamp) timestamp = IndexesList[index].TimeStamp;
                }
            }
            catch { }
            return timestamp;
        }

        /// <summary>Получить метку времени предыдущего кадра<see cref="AlfaPribor.VideoStorage.IVideoIndex"/></summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Метка времени или минус 1, если предыдущий кадр не найден</returns>
        public int GetPrevFrameTime(int cam_id, int current_time)
        {
            if (disposed) { throw new ObjectDisposedException(this.ToString()); }

            List<SingleStreamFrameIndex> IndexesList;
            if (!IndexesCashe.Indexes.TryGetValue(cam_id, out IndexesList)) return -1;
            if (IndexesList.Count == 0) return -1;

            // Ищем видеокадр с меткой времени, болше заданной
            SingleStreamFrameIndex model = new SingleStreamFrameIndex(cam_id, current_time, 0);
            int index = IndexesList.FindLastIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampGreaterOrEqualThen));
            --index;
            if (index < 0) return -1;
            else return IndexesList[index].TimeStamp;
        }

        /// <summary>Получить метку времени предыдущего кадра (мсек) для всех телекамер
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
        /// </summary>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Метка времени или минус 1, если предыдущий кадр не найден</returns>
        public int GetPrevFrameTime(int current_time)
        {
            if (disposed) { throw new ObjectDisposedException(this.ToString()); }
            int timestamp = -1;
            for (int cam_id = 0; cam_id < IndexesCashe.Indexes.Count; cam_id++)
            {
                List<SingleStreamFrameIndex> IndexesList;
                if (!IndexesCashe.Indexes.TryGetValue(cam_id, out IndexesList)) continue;
                if (IndexesList.Count == 0) continue;
                // Ищем видеокадр с меткой времени, больше заданной
                SingleStreamFrameIndex model = new SingleStreamFrameIndex(cam_id, current_time, 0);
                int index = IndexesList.FindLastIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampGreaterOrEqualThen));
                --index;
                if (index < 0) continue;
                //Первая метка
                if (timestamp == -1) timestamp = IndexesList[index].TimeStamp;
                else if (IndexesList[index].TimeStamp > timestamp) timestamp = IndexesList[index].TimeStamp;
            }
            return timestamp;
        }
        
        /// <summary>Получить метку времени начала кадра (мсек)
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Метка времени или минус 1, если кадр не найден</returns>
        public int GetFrameTime(int cam_id, int current_time)
        {
            if (disposed) { throw new ObjectDisposedException(this.ToString()); }
            List<SingleStreamFrameIndex> IndexesList;
            if (!IndexesCashe.Indexes.TryGetValue(cam_id, out IndexesList)) return -1;
            if (IndexesList.Count == 0) return -1;
            SingleStreamFrameIndex model = new SingleStreamFrameIndex(cam_id, current_time, 0);
            // Ищем видеокадр с меткой времени, меньшей или равной заданной
            int index = IndexesList.FindLastIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampGreaterOrEqualThen));
            if (index < 0) return -1;
            else return IndexesList[index].TimeStamp;
        }

        /// <summary>Получить номер кадра в видеопотоке
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Индекс кадра или минус 1, если кадр не найден</returns>
        public int GetFrameIndex(int cam_id, int current_time)
        {
            if (disposed) { throw new ObjectDisposedException(this.ToString()); }
            List<SingleStreamFrameIndex> IndexesList;
            if (!IndexesCashe.Indexes.TryGetValue(cam_id, out IndexesList)) return -1;
            if (IndexesList.Count == 0) return -1;
            SingleStreamFrameIndex model = new SingleStreamFrameIndex(cam_id, current_time, 0);
            // Ищем видеокадр с меткой времени, меньшей или равной заданной
            int index = IndexesList.FindLastIndex(new Predicate<SingleStreamFrameIndex>(model.TimeStampGreaterOrEqualThen));
            return index;
        }

        /// <summary>Получить общее количество кадров в видеопотоке
        /// <see cref="AlfaPribor.VideoStorage.IVideoIndex"/>
        /// </summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <returns>Количество кадров в видеопотоке или минус 1,
        /// если видеопоток с заданным идентификатором не существует</returns>
        public int GetFramesCount(int cam_id)
        {
            if (disposed) { throw new ObjectDisposedException(this.ToString()); }
            List<SingleStreamFrameIndex> IndexesList;
            if (!IndexesCashe.Indexes.TryGetValue(cam_id, out IndexesList)) { return -1; }
            return IndexesList.Count;
        }

        #endregion

        #endregion
    }
}
