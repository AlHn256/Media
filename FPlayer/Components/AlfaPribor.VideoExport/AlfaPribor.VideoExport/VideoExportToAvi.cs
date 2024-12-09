using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AlfaPribor.AviFile;

namespace AlfaPribor.VideoExport
{
    /// <summary>Определяет класс, позволяющий экспортировать видеокадры в AVI-файл</summary>
    public class VideoExportToAvi : IVideoExport, IDisposable
    {
        #region Fields

        /// <summary>Признак освобождения ресурсов объекта</summary>
        private bool _Disposed = false;

        /// <summary>Имя AVI-файла для экспорта данных</summary>
        private string _FileName;

        /// <summary>Границы кадра результирующего изображения</summary>
        private RectangleF _FrameRect;

        /// <summary>Графическое изображение кадра</summary>
        private Bitmap _Frame;

        /// <summary>Объект для работы с AVI-файлом</summary>
        private AlfaPribor.AviFile.AviFile aviFile;

        /// <summary>Список графических объектов, размещаемых в кадре</summary>
        private IList<BaseGraphicRegion> _Regions;

        /// <summary>Скорость воспроизведения потока (кадров/сек)</summary>
        private double _FPS;

        /// <summary>Индекс видеопотока в AVI-файле</summary>
        private int StreamIndex;

        /// <summary>Позиция очередного записываемого кадра в видеопотоке</summary>
        private int _Pos;

        /// <summary>Признак, который означает, сжимаются ли данные при записи в AVI-файл</summary>
        private bool _Compressed;

        #endregion

        #region Methods

        /// <summary>Конструктор класса.
        /// Создает пустой объект экспорта в AVI-файл с настройками по умолчанию
        /// </summary>
        public VideoExportToAvi()
            : this(25.0, new RectangleF(0.0F, 0.0F, 1024.0F, 768.0F)) { }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса скоростью воспроизведения видеопотока и границами результирующего кадра
        /// </summary>
        /// <param name="fps">Скорость воспроизведения потока (кадров/сек)</param>
        /// <param name="frame_rect">Описывает границы кадра результирующего изображения</param>
        public VideoExportToAvi(double fps, RectangleF frame_rect)
            : this(fps, frame_rect, new List<BaseGraphicRegion>()) { }

        /// <summary>Конструктор класса. Инициализирует свойства объекта класса</summary>
        /// <param name="fps">Скорость воспроизведения потока (кадров/сек)</param>
        /// <param name="frame_rect">Описывает границы кадра результирующего изображения</param>
        /// <param name="regions">Объекты, участвующие в формировании изображения результирующег кадра</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на список графических объектов</exception>
        public VideoExportToAvi(double fps, RectangleF frame_rect, IList<BaseGraphicRegion> regions)
        {
            _FileName = string.Empty;
            _Frame = null;
            FPS = fps;
            _Pos = 0;
            StreamIndex = -1;
            _Compressed = false;
            aviFile = new AlfaPribor.AviFile.AviFile();
            FrameRect = frame_rect;
            Regions = regions;
        }

        /// <summary>Деструктор класса</summary>
        ~VideoExportToAvi()
        {
            // Освобождаем неуправляемые ресурсы...
            Dispose(false);
        }

        /// <summary>Проверка на освобождение ресурсов объекта</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        protected void CheckDisposed()
        {
            if (!_Disposed) return;
            throw new ObjectDisposedException("VideoExportToAvi");
        }

        /// <summary>Высвобождает ресурсы объекта</summary>
        /// <param name="disposing">Если равен FALSE - освобождаются только неуправляемые ресурсы,
        /// иначе - освобождаются все ресурсы объекта
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed) return;
            try
            {
                Close();
            }
            catch { }
            if (_Regions != null)
            {
                foreach (var region in _Regions)
                {
                    region.Dispose();
                }
                _Regions.Clear();
            }
            if (disposing)
            {
                _FileName = null;
                if (aviFile != null)
                {
                    aviFile.Dispose();
                    aviFile = null;
                }
                if (_Frame != null)
                {
                    _Frame.Dispose();
                    _Frame = null;
                }
                _Regions = null;
            }
            _Disposed = true;
        }

        /// <summary>Открывыет AVI-файл для экспорта</summary>
        /// <param name="filename">Имя открываемого AVI-файла</param>
        /// <param name="mode">Режим доступа к файлу</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="System.ArgumentException">Не задано имя файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">AVI-файл уде открыт</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public void Open(string filename, int mode)
        {
            CheckDisposed();
            aviFile.Open(filename, mode);
            CreateVideoStream();
            _Pos = 0;
            _FileName = filename;
            _Compressed = false;
        }

        /// <summary>Открывыет AVI-файл для экспорта</summary>
        /// <param name="filename">Имя открываемого AVI-файла</param>
        /// <param name="mode">Режим доступа к файлу</param>
        /// <param name="opts">Настройки компрессора видеоданных</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="System.ArgumentException">Не задано имя файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">AVI-файл уде открыт</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public void Open(string filename, int mode, Avi.AVICOMPRESSOPTIONS opts)
        {
            CheckDisposed();
            aviFile.Open(filename, mode);
            CreateVideoStream(opts);
            _Pos = 0;
            _FileName = filename;
            _Compressed = true;
        }

        /// <summary>Закрывает открытый ранее AVI-файл</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public void Close()
        {
            CheckDisposed();
            aviFile.Close();
            _FileName = string.Empty;
        }

        /// <summary>Создает видеопоток в открытом AVI-файле и запоминает его индекс для последующего обращения к потоку</summary>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске или ошибка чтения/записи в файл</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="System.NullReferenceException">Отсутствует ссылка на изображение кадра</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">Невозможно создать поток</exception>
        private void CreateVideoStream()
        {
            Avi.AVISTREAMINFO streamInfo = new Avi.AVISTREAMINFO();
            streamInfo.fccType = Avi.StreamtypeVIDEO;
            streamInfo.dwSampleSize = (uint)(_Frame.Height * _Frame.Width * 24);
            streamInfo.dwSuggestedBufferSize = streamInfo.dwSampleSize;
            streamInfo.dwScale = 10000U;
            streamInfo.dwRate = (uint)(_FPS * 10000.0D);
            streamInfo.rcFrame = new Avi.RECT()
            {
                left = 0,
                top = 0,
                right = (uint)_Frame.Width,
                bottom = (uint)_Frame.Height
            };
            Avi.BITMAPINFOHEADER bmiHeader = GetBitmapHeader(Avi.BI_RGB);
            StreamIndex = aviFile.CreateStream(streamInfo, bmiHeader);
        }

        /// <summary>Создает видеопоток в открытом AVI-файле и запоминает его индекс для последующего обращения к потоку</summary>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске или ошибка чтения/записи в файл</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="System.NullReferenceException">Отсутствует ссылка на изображение кадра</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">Невозможно создать поток</exception>
        private void CreateVideoStream(Avi.AVICOMPRESSOPTIONS compressOpts)
        {
            Avi.AVISTREAMINFO streamInfo = new Avi.AVISTREAMINFO();
            streamInfo.fccType = Avi.StreamtypeVIDEO;
            streamInfo.fccHandler = compressOpts.fccHandler;
            streamInfo.dwScale = 10000U;
            streamInfo.dwRate = (uint)(_FPS * 10000.0D);
            streamInfo.rcFrame = new Avi.RECT()
            {
                left = 0,
                top = 0,
                right = (uint)_Frame.Width,
                bottom = (uint)_Frame.Height
            };
            streamInfo.dwQuality = compressOpts.dwQuality;
            Avi.BITMAPINFOHEADER bmiHeader = GetBitmapHeader(Avi.BI_RGB);
            StreamIndex = aviFile.CreateCompressedStream(streamInfo, bmiHeader, compressOpts);
        }

        /// <summary>Формирует заголовок к существующему изображению кадра, необходимый для его записи в AVI-файл</summary>
        /// <exception cref="System.NullReferenceException">Отсутствует ссылка на изображение кадра</exception>
        /// <returns>Заголовок изображения кадра</returns>
        private Avi.BITMAPINFOHEADER GetBitmapHeader(uint compression)
        {
            Avi.BITMAPINFOHEADER bmiHeader = new Avi.BITMAPINFOHEADER();
            bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(Avi.BITMAPINFOHEADER));
            bmiHeader.biWidth = _Frame.Width;
            bmiHeader.biHeight = _Frame.Height;
            bmiHeader.biPlanes = 1;
            bmiHeader.biXPelsPerMeter = 0;// (int)(_Frame.HorizontalResolution / 0.0254);
            bmiHeader.biYPelsPerMeter = 0;// (int)(_Frame.VerticalResolution / 0.0254);
            bmiHeader.biCompression = compression;

            switch (_Frame.PixelFormat)
            {
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                    bmiHeader.biBitCount = 16;
                    break;

                case PixelFormat.Format1bppIndexed:
                    bmiHeader.biBitCount = 1;
                    break;

                case PixelFormat.Format24bppRgb:
                    bmiHeader.biBitCount = 24;
                    break;

                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    bmiHeader.biBitCount = 32;
                    break;

                case PixelFormat.Format48bppRgb:
                    bmiHeader.biBitCount = 48;
                    break;

                case PixelFormat.Format4bppIndexed:
                    bmiHeader.biBitCount = 4;
                    break;

                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    bmiHeader.biBitCount = 64;
                    break;

                case PixelFormat.Format8bppIndexed:
                    bmiHeader.biBitCount = 8;
                    break;

                default:
                    bmiHeader.biBitCount = 0;
                    break;
            }
            
            return bmiHeader;
        }

        /// <summary>Создает изображение пустого кадра</summary>
        private void CreateFrameImage()
        {
            if (_Frame == null)
            {
                _Frame = new Bitmap((int)_FrameRect.Width, (int)_FrameRect.Height,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
        }

        #endregion

        #region Properties

        /// <summary>Полное имя AVI-файла, включая путь к нему</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public string FileName
        {
            get 
            {
                CheckDisposed();
                return _FileName; 
            }
        }

        #endregion

        #region IVideoExport members

        #region Methods

        /// <summary>Подготавливает изображение кадра
        /// <see cref="AlfaPribor.VideoExport.IVideoExport"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="AlfaPribor.VideoExport.VideoExportException">Ошибка при отрисовке фрагмента изображения</exception>
        public void MakeFrame()
        {
            CheckDisposed();
            try
            {
                CreateFrameImage();
                using (Graphics surface = Graphics.FromImage(_Frame))
                {
                    surface.Clear(Color.Black);
                    var descRegions = from region in _Regions
                                  where region.Visible
                                  orderby region.Layer descending
                                  select region;
                    foreach (var region in descRegions)
                    {
                        region.Draw(surface);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new VideoExportException("Can not draw frame region!", ex);
            }
        }

        /// <summary>Помещает изображение кадра в контейнер для хранения кадров
        /// <see cref="AlfaPribor.VideoExport.IVideoExport"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для записи данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CodecException">Ошибка компрессора видеоданных</exception>
        public void SaveFrame()
        {
            this.SaveFrame(_Frame);
        }

        /// <summary>Помещает изображение кадра в контейнер для хранения кадров
        /// <see cref="AlfaPribor.VideoExport.IVideoExport"/>
        /// </summary>
        /// <param name="frame">Экспортируемое в видеопоток изображение</param>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для записи данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на экспортируемое изображение</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CodecException">Ошибка компрессора видеоданных</exception>
        public void SaveFrame(Image frame)
        {
            CheckDisposed();
            if (frame == null)
            {
                throw new ArgumentNullException();
            }
            using (Bitmap bitmap = new Bitmap(frame))
            {
                SaveFrame(bitmap);
            }
        }

        /// <summary>Помещает изображение кадра в контейнер для хранения кадров
        /// <see cref="AlfaPribor.VideoExport.IVideoExport"/>
        /// </summary>
        /// <param name="frame">Экспортируемое в видеопоток изображение</param>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для записи данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на экспортируемое изображение</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CodecException">Ошибка компрессора видеоданных</exception>
        public void SaveFrame(Bitmap frame)
        {
            CheckDisposed();
            if (frame == null)
            {
                throw new ArgumentNullException();
            }
            using (Bitmap image = (Bitmap)frame.Clone())
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                int Samples = 1;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
                try
                {
                    if (_Compressed)
                    {
                        aviFile.WriteCompress(StreamIndex, _Pos, ref Samples, bmpData.Scan0, bmpData.Stride * bmpData.Height);
                    }
                    else
                    {
                        aviFile.Write(StreamIndex, _Pos, ref Samples, bmpData.Scan0, bmpData.Stride * bmpData.Height);
                    }
                    _Pos += 1;
                }
                finally
                {
                    image.UnlockBits(bmpData);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>Описывает границы кадра результирующего изображения
        /// <see cref="AlfaPribor.VideoExport.IVideoExport"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public RectangleF FrameRect
        {
            get
            {
                CheckDisposed();
                return _FrameRect;
            }
            set
            {
                CheckDisposed();
                if (!string.IsNullOrEmpty(_FileName))
                {
                    throw new VideoExportException("Can not change this property while AVI-file is open!");
                }
                if (_Frame != null)
                {
                    _Frame.Dispose();
                    _Frame = null;
                }
                _FrameRect = value;
                CreateFrameImage();
            }
        }

        /// <summary>Список графических объектов, размещаемых в кадре
        /// <see cref="AlfaPribor.VideoExport.IVideoExport"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект</exception>
        public IList<BaseGraphicRegion> Regions
        {
            get
            {
                CheckDisposed();
                return _Regions;
            }
            set
            {
                CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("Regions");
                }
                if (_Regions != null)
                {
                    foreach (var region in _Regions)
                    {
                        region.Dispose();
                    }
                }
                // Клонируем элементы списка
                _Regions = new List<BaseGraphicRegion>(value.Count);
                foreach (var item in value)
                {
                    _Regions.Add((BaseGraphicRegion)item.Clone());
                }
            }
        }

        /// <summary>Графическое изображение кадра
        /// <see cref="AlfaPribor.VideoExport.IVideoExport"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public Image Frame
        {
            get
            {
                CheckDisposed();
                return _Frame;
            }
            set { _Frame = (Bitmap)value; }
        }

        /// <summary>Скорость воспроизведения потока (кадров/сек)
        /// <see cref="AlfaPribor.VideoExport.IVideoExport"/>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="AlfaPribor.VideoExport.VideoExportException">Попытка изменения свойства при открытом AVI-файле</exception>
        public double FPS
        {
            get 
            {
                CheckDisposed();
                return _FPS; 
            }
            set
            {
                CheckDisposed();
                if (!string.IsNullOrEmpty(_FileName))
                {
                    throw new VideoExportException("Can not change this property while AVI-file is open!");
                }
                _FPS = value;
            }
        }

        /// <summary>Признак, который определяет, как записываются данные в поток - в исходном виде, или в сжатом</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public bool Compressed
        {
            get 
            {
                CheckDisposed();
                return _Compressed; 
            }
        }

        #endregion

        #endregion

        #region IDisposable members

        /// <summary>Высвобождает управляемые и неуправляемые ресурсы
        /// <see cref="System.IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
