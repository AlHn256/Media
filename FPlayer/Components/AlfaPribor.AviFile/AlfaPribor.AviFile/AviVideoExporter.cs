using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;
using System.IO;
using AlfaPribor.AviFile;

namespace AlfaPribor.AviFile
{
    /// <summary>Экспортирует все/часть видеоданных из одного AVI-файла в другой</summary>
    public class AviVideoExporter
    {
        #region Fields

        /// <summary>Имя файла-источника</summary>
        private string _SourceFileName;

        /// <summary>Имя файла-приемника</summary>
        private string _DestFileName;

        /// <summary>Индекс первого экспортируемого видеокадра</summary>
        private int _FirstFrame;

        /// <summary>Количество экспортируемых видеокадров</summary>
        private int _FramesCount;

        /// <summary>Признак использования компрессора данных</summary>
        private bool _UseCompressor;

        /// <summary>Настройки компрессора данных</summary>
        private Avi.AVICOMPRESSOPTIONS _CompressOptions;

        /// <summary>Ссылка на исключение, произошедшее в процессе экспорта видеоданных</summary>
        private Exception _ExportException;

        /// <summary>Признак использования трансформации исходных изображений кадров</summary>
        private bool _UseTransformation;

        /// <summary>Высота трансформированного видеокадра</summary>
        private int _FrameDestHeight;

        /// <summary>Ширина трансформированного видеокадра</summary>
        private int _FrameDestWidth;

        /// <summary>Код несжатого изображения</summary>
        private static uint fccDIB = Avi.GetFourCC("DIB");

        /// <summary>Признак изменения частоты следования кадров в целевом видеофайле</summary>
        private bool _UseFpsCorrection;

        /// <summary>Частота следования кадров в файле-приемнике с точностью до тысячных долей</summary>
        private double _FPS;

        #endregion

        #region Methods

        /// <summary>Конструктор объектов класса</summary>
        public AviVideoExporter()
        {
            _SourceFileName = string.Empty;
            _DestFileName = string.Empty;
            _FirstFrame = 0;
            _FramesCount = 0;
            _UseCompressor = false;
            _CompressOptions = new Avi.AVICOMPRESSOPTIONS();
            _ExportException = null;
            _UseTransformation = false;
            _FrameDestHeight = 0;
            _FrameDestWidth = 0;
            _UseFpsCorrection = false;
            _FPS = 0.0;
        }

        /// <summary>Экспортирует видеоданные из файла-источника в файл-приемник</summary>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">AVI-файл уде открыт</exception>
        /// <exception cref="System.ArgumentException">Не задано имя файла</exception>
        /// <exception cref="System.InvalidOperationException">Файл-источник не содержит видеоданных</exception>
        /// <exception cref="AlfaPribor.AviFile.CompressorException">Компрессия данных в заданный формат не поддерживается</exception>
        /// <exception cref="AlfaPribor.AviFile.DecompressorException">Декомпрессия данных из заданного формата не поддерживается</exception>
        public void Export()
        {
            _ExportException = null;
            using (AviFile sourceFile = new AviFile(_SourceFileName, Avi.OF_READ | Avi.OF_SHARE_DENY_WRITE))
            {
                int destFileMode = Avi.OF_CREATE | Avi.OF_SHARE_DENY_WRITE;
                using (AviFile destFile = new AviFile(_DestFileName, destFileMode))
                {
                    // В файле-источнике ищем видеопоток
                    IList<Avi.AVISTREAMINFO> infoList = sourceFile.GetStreamsInfo();
                    Avi.AVISTREAMINFO info = infoList.First(item => item.fccType == Avi.StreamtypeVIDEO);
                    RaiseStreamOpenEvent(ref info);
                    int sourceStreamIndex = infoList.IndexOf(info);
                    // Определяем метод чтения видеопотока из файла-источника
                    CopyMode readMode;
                    if (_UseCompressor && (info.fccHandler != fccDIB))
                    {
                        readMode = CopyMode.Decode;
                    }
                    else
                    {
                        readMode = CopyMode.Original;
                    }
                    // Адаптируем заголовок исходного видеопотока для записи в файл-приемник
                    if (_FirstFrame != 0)
                    {
                        info.dwStart = (uint)_FirstFrame;
                    }
                    else
                    {
                        _FirstFrame = (int)info.dwStart;
                    }
                    if (_FramesCount != 0)
                    {
                        info.dwLength = (uint)_FramesCount;
                    }
                    else
                    {
                        _FramesCount = (int)info.dwLength;
                    }
                    if (_UseTransformation)
                    {
                        info.rcFrame = new Avi.RECT()
                        {
                            right = (uint)_FrameDestWidth,
                            bottom = (uint)_FrameDestHeight
                        };
                    }
                    else
                    {
                        _FrameDestWidth = (int)info.rcFrame.right;
                        _FrameDestHeight = (int)info.rcFrame.bottom;
                    }
                    // Извлекаем из исходного видеопотока информацию о содержащемся в нем видеокадрах
                    Avi.BITMAPINFOHEADER bmiHeader;
                    try
                    {
                        bmiHeader = sourceFile.GetFrameInfo(sourceStreamIndex, (int)info.dwStart);
                    }
                    catch (CodecException e)
                    {
                        throw new DecompressorException(e.Message);
                    }
                    if (_UseFpsCorrection)
                    {
                        info.dwScale = 1000U;
                        info.dwRate = (uint)(_FPS * 1000.0);
                    }
                    // Создаем видеопоток в целевом файле
                    int destStreamIndex;
                    if (_UseCompressor)
                    {
                        info.fccHandler = _CompressOptions.fccHandler;
                        bmiHeader.biCompression = Avi.BI_RGB;
                        bmiHeader.biSizeImage = 0;
                        if (_UseTransformation)
                        {
                            bmiHeader.biWidth = _FrameDestWidth;
                            bmiHeader.biHeight = _FrameDestHeight;
                        }
                        destStreamIndex = destFile.CreateCompressedStream(info, bmiHeader, _CompressOptions);
                    }
                    else
                    {
                        destStreamIndex = destFile.CreateStream(info, bmiHeader);
                    }
                    // Определяем метод записи видеоданных в целевой видеопоток
                    CopyMode writeMode = _UseCompressor ? CopyMode.Encode : CopyMode.Original;
                    // Создаем объект для копирования видеопотоков
                    using (AviFileCopier copier = CreateAviFileCopier(
                        bmiHeader.biHeight * bmiHeader.biWidth * bmiHeader.biBitCount * bmiHeader.biPlanes)
                        )
                    {
                        // Определяем правило копирования
                        CopyRule rule = new CopyRule(
                            sourceFile, destFile, sourceStreamIndex, destStreamIndex, _FirstFrame, _FramesCount, readMode, writeMode
                        );
                        // Приступаем к копированию
                        copier.Copy(new List<CopyRule> { rule });
                    }
                    if (_ExportException != null)
                    {
                        // Если в процессе копирования видеоданных произошла ошибка - повторяем исключение
                        throw _ExportException;
                    }
                }
            }
        }

        /// <summary>Создает объект копирования видеопотоков</summary>
        /// <param name="buffer_size">Размер буфера для хранения несжатого изображения</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для создания буфера</exception>
        /// <returns>Объект копирования</returns>
        private AviFileCopier CreateAviFileCopier(int buffer_size)
        {
            AviFileCopier result = new AviFileCopier(buffer_size);
            result.CopySample += new EventHandler<CopySampleEventArgs>(CopySampleHandler);
            result.CopyException += new EventHandler<CopyExceptionEventArgs>(CopyExceptionHandler);
            result.ReadSample += new EventHandler<ReadSampleEventArgs>(ReadSampleHandler);
            return result;
        }

        /// <summary>Обработчик события "Чтение сэмпла данных"</summary>
        /// <param name="sender">Объект, породивший событие</param>
        /// <param name="e">Дополнительные данные о событии</param>
        void ReadSampleHandler(object sender, ReadSampleEventArgs e)
        {
            Avi.BITMAPINFOHEADER bmiHeader = (Avi.BITMAPINFOHEADER)e.Info;
            int number = e.Number;
            Avi.BITMAPINFOHEADER header = (Avi.BITMAPINFOHEADER)e.Info;
            byte[] data = e.Data;
            RaiseTransformFrameEvent(number, ref header, ref data);
            e.Info = header;
            e.Data = data;
        }

        /// <summary>Обработчик исключительных ситуаций в процессе копирования данных</summary>
        /// <param name="sender">Объект, породивший событие</param>
        /// <param name="e">Дополнительные данные о событии</param>
        void CopyExceptionHandler(object sender, CopyExceptionEventArgs e)
        {
            _ExportException = e.InnerException;
        }

        /// <summary>Обработчик события "Копирование сэмпла данных"</summary>
        /// <param name="sender">Объект, породивший событие</param>
        /// <param name="e">Дополнительные данные о событии</param>
        void CopySampleHandler(object sender, CopySampleEventArgs e)
        {
            RaiseExportFrameEvent(e.Number);
        }

        /// <summary>Генерирует событие "Экспорт видеокадра"</summary>
        /// <param name="number">Номер экспортируемого видеокадра</param>
        private void RaiseExportFrameEvent(int number)
        {
            if (ExportFrame != null)
            {
                try
                {
                    ExportFrame(this, new ExportFrameEventArgs(number));
                }
                catch { }
            }
        }

        /// <summary>Генерирует событие "Трансформация видеокадра"</summary>
        /// <param name="number">Номер видеокадра</param>
        /// <param name="header">Заголовок данных изображения</param>
        /// <param name="data">Данные изображения видеокадра</param>
        /// <exception cref="System.Exception">Ошибка трансформации данных</exception>
        private void RaiseTransformFrameEvent(int number, ref Avi.BITMAPINFOHEADER header, ref byte[] data)
        {
            if (TransformFrame != null)
            {
                TransformFrameEventArgs args = new TransformFrameEventArgs(number, header, data);
                TransformFrame(this, args);
                header = args.Header;
                data = args.Data;
            }
        }

        /// <summary>Генерирует событие "Открыт видеопоток в файле-источнике"</summary>
        /// <param name="info">Детальная информация о видеопотоке</param>
        private void RaiseStreamOpenEvent(ref Avi.AVISTREAMINFO info)
        {
            if (StreamOpen != null)
            {
                try
                {
                    StreamEventArgs args = new StreamEventArgs(info);
                    StreamOpen(this, args);
                    info = args.Info;
                }
                catch { }
            }
        }

        /// <summary>Получить индекс первого видеокадра</summary>
        /// <param name="file_name">Имя AVI-файла</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">AVI-файл уде открыт</exception>
        /// <exception cref="System.ArgumentException">Не задано имя файла</exception>
        /// <exception cref="System.InvalidOperationException">Файл не содержит потока с видеоданными</exception>
        /// <returns>Индекс первого видеокадра</returns>
        public static int GetFirstFrameIndex(string file_name)
        {
            using (AviFile file = new AviFile(file_name, Avi.OF_READ))
            {
                IList<Avi.AVISTREAMINFO> infoList = file.GetStreamsInfo();
                Avi.AVISTREAMINFO info = infoList.First(item => item.fccType == Avi.StreamtypeVIDEO);
                return (int)info.dwStart;
            }
        }

        /// <summary>Получить длину видеопотока в кадрах</summary>
        /// <param name="file_name">Имя AVI-файла</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">AVI-файл уде открыт</exception>
        /// <exception cref="System.ArgumentException">Не задано имя файла</exception>
        /// <exception cref="System.InvalidOperationException">Файл не содержит потока с видеоданными</exception>
        /// <returns>Длина видеопотока</returns>
        public static int GetFramesCount(string file_name)
        {
            using (AviFile file = new AviFile(file_name, Avi.OF_READ))
            {
                IList<Avi.AVISTREAMINFO> infoList = file.GetStreamsInfo();
                Avi.AVISTREAMINFO info = infoList.First(item => item.fccType == Avi.StreamtypeVIDEO);
                return (int)info.dwLength;
            }
        }

        #endregion

        #region Properties

        /// <summary>Полное имя файла-источника, включая путь к нему</summary>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойства</exception>
        public string SourceFileName
        {
            get { return _SourceFileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("SourceFileName");
                }
                _SourceFileName = value;
            }
        }

        /// <summary>Полное имя файла-приемника, включая путь к нему</summary>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойства</exception>
        public string DestFileName
        {
            get { return _DestFileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("DestFileName");
                }
                _DestFileName = value;
            }
        }

        /// <summary>Индекс первого экспортируемого видеокадра</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение свойства вне допустимого диапазона значение</exception>
        public int FirstFrame
        {
            get { return _FirstFrame; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("FirstFrame");
                }
                _FirstFrame = value;
            }
        }

        /// <summary>Количество экспортируемых видеокадров</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение свойства вне допустимого диапазона значение</exception>
        public int FramesCount
        {
            get { return _FramesCount; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("FramesCount");
                }
                _FramesCount = value;
            }
        }

        /// <summary>Признак использования компрессора данных при сохранении данных в файл-приемник</summary>
        public bool UseCompressor
        {
            get { return _UseCompressor; }
            set { _UseCompressor = value; }
        }

        /// <summary>Настройки компрессора данных</summary>
        public Avi.AVICOMPRESSOPTIONS CompressOptions
        {
            get { return _CompressOptions; }
            set { _CompressOptions = value; }
        }

        /// <summary>Признак использования трансформации исходных видеокадров</summary>
        public bool UseTransformation
        {
            get { return _UseTransformation; }
            set { _UseTransformation = value; }
        }

        /// <summary>Высота трансформированного видеокадра</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение свойства вне допустимого диапазона значение</exception>
        public int FrameDestHeight
        {
            get { return _FrameDestHeight; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("FrameDestHeight");
                }
                _FrameDestHeight = value;
            }
        }

        /// <summary>Ширина трансформированного видеокадра</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение свойства вне допустимого диапазона значение</exception>
        public int FrameDestWidth
        {
            get { return _FrameDestWidth; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("FrameDestWidth");
                }
                _FrameDestWidth = value;
            }
        }

        /// <summary>Признак изменения частоты следования кадров в целевом видеофайле</summary>
        public bool UseFpsCorrection
        {
            get { return _UseFpsCorrection; }
            set { _UseFpsCorrection = value; }
        }

        /// <summary>Частота следования кадров в файле-приемнике с точностью до тысячных долей</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение аргумента вне допустимого диапазона значений</exception>
        public double FPS
        {
            get { return _FPS; }
            set
            {
                if (value <= 0.0 || value > UInt32.MaxValue / 1000U)
                {
                    throw new ArgumentOutOfRangeException("FPS");
                }
                _FPS = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Событие "Экспорт видеокадра"</summary>
        public event EventHandler<ExportFrameEventArgs> ExportFrame;

        /// <summary>Событие "Трансформация видеокадра"</summary>
        public event EventHandler<TransformFrameEventArgs> TransformFrame;

        /// <summary>Событие "Открыт видеопоток в файле-источнике"</summary>
        public event EventHandler<StreamEventArgs> StreamOpen;

        #endregion
    }
}
