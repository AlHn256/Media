using System;
using System.Collections.Generic;
using System.Threading;

namespace AlfaPribor.VideoStorage2
{

    /// <summary>Класс буферизованной записи видео</summary>
    public class BufferedVideoRecord : VideoRecord
    {
        #region Variables

        /// <summary>Очередь кадров</summary>
        Queue<VideoFrame> _bufferFrames = null;
        object lockBufferObject = new object();
        bool _isRunning;
        bool _stopThread;
        Thread _writerThread;

        #endregion

        #region Constructors

        /// <summary>Конструктор класса</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition">Раздел хранилища, к которому относится видеозапись</param>
        /// <param name="mode">Режим открытия видеозаписи</param>
        /// <exception cref="System.ArgumentException">
        /// Идентификатор видеозаписи имеет недопустимое значение
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Не указан раздел хранилища, к которому относится видеозапись
        /// </exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">
        /// Возникла ошибка при создании или чтении потоков ввода/вывода
        /// </exception>
        public BufferedVideoRecord(string id, VideoPartition partition, VideoRecordOpenMode mode) : base(id, partition, mode) 
        { _BufferFrames = new Queue<VideoFrame>(); }

        /// <summary>Конструктор класса.
        /// Используется только для в случае, если статус запрашиваемого интерфейса заранее известен
        /// </summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="status">Статус запрашиваемого интерфейса</param>
        public BufferedVideoRecord(string id, VideoStorageIntStat status) : base(id, status) 
        { _BufferFrames = new Queue<VideoFrame>(); }

        /// <summary>Конструктор класса.
        /// Используется только для в случае, если статус запрашиваемого интерфейса заранее известен
        /// </summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздела хранилища, к которому относится видеозапись</param>
        /// <param name="status">Статус запрашиваемого интерфейса</param>
        public BufferedVideoRecord(string id, int partition_id, VideoStorageIntStat status) : base(id, partition_id, status) 
        { _BufferFrames = new Queue<VideoFrame>(); }

        #endregion

        /// <summary>Очередь кадров</summary>
        Queue<VideoFrame> _BufferFrames
        {
            get
            {
                if (_bufferFrames == null) _bufferFrames = new Queue<VideoFrame>();
                return _bufferFrames;
            }
            set { _bufferFrames = value; }
        }

        /// <summary>Поместить кадр в очередь</summary>
        /// <param name="frame">Кадр</param>
        void PushFrame(VideoFrame frame)
        {
            lock (lockBufferObject) { _BufferFrames.Enqueue(frame); }
        }

        /// <summary>Получить первый кадр из очереди</summary>
        /// <returns>Кадр</returns>
        VideoFrame GetFrame()
        {
            VideoFrame frame = null;
            lock (lockBufferObject) { if (_BufferFrames.Count > 0) frame = _BufferFrames.Dequeue(); }
            return frame;
        }

        int GetFrameCount()
        {
            int count = 0;
            lock (lockBufferObject) { count = _BufferFrames.Count; }
            return count;
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
        protected override void OpenStreams(string id, string path, VideoRecordOpenMode mode,
                                            out IVideoIndexesStream indexes, out IVideoFramesStream frames){
                base.OpenStreams(id, path, mode, out indexes, out frames);
                StartThread();
        }

        /// <summary>Запуск потока записи кадров</summary>
        void StartThread()
        {
            _isRunning = true;
            _stopThread = false;
            _writerThread = new Thread(_doWriteFrame);
            _writerThread.Priority = ThreadPriority.Normal;
            _writerThread.Start();
        }
        
        /// <summary>Поток записи кадров</summary>
        void _doWriteFrame()
        {
            while (_isRunning || GetFrameCount()>0)
            {
                VideoFrame frame = GetFrame();
                if (frame != null) WriteFrameObject(frame);
                Thread.Sleep(1);
            }
            _stopThread = true;
        }

        /// <summary>Закрытие потоков объекта</summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _isRunning = false;
            while (!_stopThread) { Thread.Sleep(1); }
            _writerThread = null;
        }

        #region Public

        /// <summary>Записать кадр
        /// <see cref="AlfaPribor.VideoStorage.IVideoWriter"/>
        /// </summary>
        /// <param name="camera_id">Номер камеры (видеопотока)</param>
        /// <param name="time_stamp">Время от начала записи в миллисекундах</param>
        /// <param name="content_type">Тип содержимого кадра, например: image/jpeg или image/raw</param>
        /// <param name="frame_data">Бинарные данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Объект уничтожен
        /// </exception>
        /// <returns>Результат выполнения операции</returns>
        public override VideoStorageResult WriteFrame(int camera_id, int time_stamp, string content_type, byte[] frame_data)
        {
            VideoFrame frame = new VideoFrame(camera_id, time_stamp, content_type, frame_data);
            return WriteFrame(frame);
        }

        /// <summary>Записать кадр<see cref="AlfaPribor.VideoStorage.IVideoWriter"/></summary>
        /// <param name="frame">Данные кадра</param>
        /// <exception cref="System.ObjectDisposedException">Объект уничтожен</exception>
        /// <exception cref="System.ArgumentException">Не задан видеокадр</exception>
        /// <returns>Результат выполнения операции</returns>
        public override VideoStorageResult WriteFrame(VideoFrame frame)
        {
            PushFrame(frame);
            return VideoStorageResult.Ok;
        }

        /// <summary>Запись кадра</summary>
        /// <param name="frame">Данные кадра</param>
        /// <returns></returns>
        public VideoStorageResult WriteFrameObject(VideoFrame frame)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (frame == null)
            {
                throw new ArgumentNullException();
            }
            long Position;
            long Length;
            try
            {
                // Запоминаем длину потока и позицию курсора в нем, чтобы можно было
                // вернуть поток к этому состоянию при неудачной записи индекса
                Length = Frames.Length;
                Position = Frames.Position;
                int k = Frames.WriteFrame(frame);
                if (k == 0)
                {
                }
            }
            catch
            {
                return VideoStorageResult.Fault;
            }
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
            IndexesCashe.Add(index, true);
            AddToStreamInfo(frame);
            return VideoStorageResult.Ok;
        }

        /// <summary>Завершить чтение / закрыть поток
        /// <see cref="AlfaPribor.VideoStorage.IVideoWriter"/>
        /// </summary>
        /// <returns>Результат выполнения операции</returns>
        public override VideoStorageResult Close()
        {
            Dispose(false);
            return VideoStorageResult.Ok;
        }

        /// <summary>Аварийное закрытие записи
        /// <see cref="AlfaPribor.VideoStorage.IVideoWriter"/>
        /// </summary>
        /// <returns>Результат выполнения операции</returns>
        public override VideoStorageResult Abort()
        {
            _Aborted = true;
            Dispose(false);
            return VideoStorageResult.Ok;
        }

        #endregion

    }
}
