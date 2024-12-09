using System;
using System.Collections.Generic;
using System.IO;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Видеозапись.
    /// Класс, предоставляющий свойства для записи/чтения видеоданных
    /// </summary>
    public class VideoRecord : BaseVideoRecord
    {

        /// <summary>Конструктор класса</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition">Раздел хранилища, к которому относится видеозапись</param>
        /// <param name="mode">Режим открытия видеозаписи</param>
        /// <exception cref="System.ArgumentException">Идентификатор видеозаписи имеет недопустимое значение</exception>
        /// <exception cref="System.ArgumentNullException">Не указан раздел хранилища, к которому относится видеозапись</exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">Возникла ошибка при создании или чтении потоков ввода/вывода</exception>
        public VideoRecord(string id, VideoPartition partition, VideoRecordOpenMode mode)
            : base(id, partition, mode) { }

        /// <summary>Конструктор класса.
        /// Используется только для в случае, если статус запрашиваемого интерфейса заранее известен</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="status">Статус запрашиваемого интерфейса</param>
        public VideoRecord(string id, VideoStorageIntStat status)
            : base(id, status) { }

        /// <summary>Конструктор класса.
        /// Используется только для в случае, если статус запрашиваемого интерфейса заранее известен</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздела хранилища, к которому относится видеозапись</param>
        /// <param name="status">Статус запрашиваемого интерфейса</param>
        public VideoRecord(string id, int partition_id, VideoStorageIntStat status)
            : base(id, partition_id, status) { }

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
        protected override void OpenStreams(string id, string path, VideoRecordOpenMode mode, out IVideoIndexesStream indexes, out IVideoFramesStream frames)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id");
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path");
            }
            try
            {
                if (path[path.Length - 1] != '\\')
                {
                    path += "\\";
                }
                Stream IndexesStream;
                switch (mode)
                {
                    case VideoRecordOpenMode.Read:
                        IndexesStream = new BufferedStream(new FileStream(path + id + ".index", FileMode.Open, FileAccess.Read, FileShare.Read),4096);
                        break;

                    case VideoRecordOpenMode.ReadWrite:
                        IndexesStream = new BufferedStream(new FileStream(path + id + ".index", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite),4096);
                        break;

                    default:
                        IndexesStream = null;
                        break;
                }
                indexes = new VideoIndexesStream(IndexesStream);

                Stream FramesStream;
                switch (mode)
                {
                    case VideoRecordOpenMode.Read:
                        FramesStream = new BufferedStream(
                            new FileStream(path + id + ".frames", FileMode.Open, FileAccess.Read, FileShare.Read),65536);
                        break;

                    case VideoRecordOpenMode.ReadWrite:
                        FramesStream = new BufferedStream(
                            new FileStream(path + id + ".frames", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), 5536);
                        break;

                    default:
                        FramesStream = null;
                        break;
                }
                frames = new VideoFramesStream(FramesStream);
            }
            catch (Exception E)
            {
                throw new VideoStorageException(E.Message, E);
            }
        }

        /// <summary>Читаут информацию о видеопотоках, содержащихся в видеозаписи</summary>
        /// <param name="info">Список с информацией о видеопотоках</param>
        /// <param name="indexes">Интерфейс для чтения индексов видеозаписи</param>
        /// <param name="frames">Интерфейс для чтения видеокадров</param>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Возникла ошибка при чтении информации о видеопотоках
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан идентификатор видеозаписи или путь к ней</exception>
        protected override void ReadStreamInfo(
            IList<VideoStreamInfo> info, IVideoIndexesStream indexes, IVideoFramesStream frames)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (frames == null)
            {
                throw new ArgumentNullException("frames");
            }
            try
            {
                frames.ReadStreamInfo(info);
            }
            catch (Exception e)
            {
                throw new VideoStorageException("Can not read video streams info!", e);
            }
        }

        /// <summary>
        /// Записывает информацию о видеопотоках, содержащихся в видеозаписи
        /// </summary>
        /// <param name="info">Список с информацией о видеопотоках</param>
        /// <param name="indexes">Интерфейс для чтения индексов видеозаписи</param>
        /// <param name="frames">Интерфейс для чтения видеокадров</param>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Возникла ошибка при записи информации о видеопотоках
        /// </exception>
        /// <exception cref="System.ArgumentException">Не задан идентификатор видеозаписи или путь к ней</exception>
        protected override void WriteStreamInfo(
            IList<VideoStreamInfo> info, IVideoIndexesStream indexes, IVideoFramesStream frames)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (frames == null)
            {
                throw new ArgumentNullException("frames");
            }
            try
            {
                frames.WriteStreamInfo(info);
            }
            catch (Exception e)
            {
                throw new VideoStorageException("Can not write video streams info!", e);
            }
        }
    }
}
