using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AlfaPribor.AviFile
{
    /// <summary>Реализует чтение/запись данных в AVI-файлы</summary>
    public class AviFile : IDisposable
    {
        #region Fields

        /// <summary>AVI-файл</summary>
        private string _FileName;

        /// <summary>Указатель на интерфейса открытого AVI-файла</summary>
        private IntPtr aviFile;

        /// <summary>Список указателей на интерфейсы потоков, содержащихся в AVI-файле</summary>
        private List<IntPtr> aviStreams;

        /// <summary>Отображение указателей интерфейсов несжатых потоков на указатели интерфейсов их сжатых аналогов</summary>
        private Dictionary<IntPtr, IntPtr> aviCmpsStreams;

        /// <summary>Отображение указателей интерфейсов потоков на блоки данных с информацей о потоках</summary>
        private Dictionary<IntPtr, Avi.AVISTREAMINFO> aviStreamsInfo;

        /// <summary>Отображение указателей интерфейсов потоков на указатели интерфейсов декомпресии данных</summary>
        private Dictionary<IntPtr, IntPtr> aviGetFrameObjects;

        /// <summary>Признак освобождения ресурсов объекта</summary>
        private bool _Disposed = false;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        public AviFile()
        {
            aviFile = IntPtr.Zero;
            aviStreams = new List<IntPtr>();
            aviCmpsStreams = new Dictionary<IntPtr, IntPtr>();
            aviStreamsInfo = new Dictionary<IntPtr,Avi.AVISTREAMINFO>();
            aviGetFrameObjects = new Dictionary<IntPtr, IntPtr>();
            Avi.AVIFileInit();
        }

        /// <summary>Конструктор класса. Открывает AVI-файл для чтени/записи</summary>
        /// <param name="FileName">Имя открываемого AVI-файла, включая путь к нему</param>
        /// <param name="Mode">Флаги, отвечающие за режим открытия файла (чтение/запись/создание/ограничения)</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">AVI-файл уде открыт</exception>
        /// <exception cref="System.ArgumentException">Не задано имя файла</exception>
        public AviFile(string FileName, int Mode)
            : this()
        {
            Open(FileName, Mode);
        }

        /// <summary>Деструктор класса</summary>
        ~AviFile()
        {
            // Освобождаем неуправляемые ресурсы...
            Dispose(false);
        }

        /// <summary>Открывает AVI-файл для чтени/записи</summary>
        /// <param name="FileName">Имя открываемого AVI-файла, включая путь к нему</param>
        /// <param name="Mode">Флаги, отвечающие за режим открытия файла (чтение/запись/создание/ограничения)</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="System.ArgumentException">Не задано имя файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">AVI-файл уде открыт</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public void Open(string FileName, int Mode)
        {
            CheckDisposed();
            if (aviFile != IntPtr.Zero)
            {
                throw new AviFileException("AVI-file already open!");
            }
            if (string.IsNullOrEmpty(FileName))
            {
                throw new ArgumentException("Missing name of file!", "FileName");
            }
            _FileName = FileName;
            try
            {
                CheckAviFileResult(
                    Avi.AVIFileOpen(ref aviFile, FileName, Mode, 0)
                    );
                CheckAviFileResult(FindStreams(Avi.StreamtypeVIDEO));
                CheckAviFileResult(FindStreams(Avi.StreamtypeAUDIO));
                CheckAviFileResult(FindStreams(Avi.StreamtypeMIDI));
                CheckAviFileResult(FindStreams(Avi.StreamtypeTEXT));
            }
            catch
            {
                Close();
                throw;
            }
        }

        /// <summary>Ищет потоки с заданным типом содержимого и добавляет их в список aviStreams</summary>
        /// <param name="fccType">Тип содержимого потока</param>
        /// <returns>Результат последней выполненой операции</returns>
        private uint FindStreams(int fccType)
        {
            IntPtr stream;
            int count = 0;
            uint result;
            Avi.AVISTREAMINFO info = new Avi.AVISTREAMINFO();
            int info_size = Marshal.SizeOf(typeof(Avi.AVISTREAMINFO));

            while ((result = Avi.AVIFileGetStream(aviFile, out stream, fccType, count++)) == Avi.AVIERR_OK)
            {
                aviStreams.Add(stream);
                result = Avi.AVIStreamInfo(stream, ref info, info_size);
                if (result == Avi.AVIERR_OK)
                {
                    aviStreamsInfo.Add(stream, info);
                }
            }
            return result;
        }

        /// <summary>Проверяет код результата выполнения операции с AVI-файлом</summary>
        /// <param name="result">Результат выполнения операции с AVI-файлом</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске или ошибка чтения/записи в файл</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CodecException">Компрессия/декомпрессия заданного формата данных не поддерживается</exception>
        /// <exception cref="AlfaPribor.AviFile.BufferTooSmallException">Недостаточный размер буфера для чтения данных</exception>
        private void CheckAviFileResult(uint result)
        {
            switch (result)
            {
                case Avi.AVIERR_OK:
                    return;

                case Avi.AVIERR_BADFORMAT:
                    throw new BadAviFormatException();

                case Avi.AVIERR_UNSUPPORTED:
                    throw new CodecException("Compression/decompression for this data format is not supported!");

                case Avi.AVIERR_MEMORY:
                    throw new OutOfMemoryException();

                case Avi.AVIERR_FILEREAD:
                    throw new IOException("Can not read a file!");

                case Avi.AVIERR_FILEWRITE:
                    throw new IOException("Can not write a file!");

                case Avi.AVIERR_FILEOPEN:
                    throw new IOException("Can not open a file!");

                case Avi.AVIERR_READONLY:
                    throw new IOException("Can`t write into a file because file is read only!");

                case Avi.AVIERR_BUFFERTOOSMALL:
                    throw new BufferTooSmallException();

                case Avi.AVIERR_NOCOMPRESSOR:
                    throw new CodecException("Compressor not found!");

                case Avi.AVIERR_NODATA:
                    return;

                default:
                    throw new Exception("Unknown error!");
            }
        }

        /// <summary>Закрывает AVI-файл</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public void Close()
        {
            CheckDisposed();
            foreach (var getFrameObject in aviGetFrameObjects)
            {
                Avi.AVIStreamGetFrameClose(getFrameObject.Value);
            }
            aviGetFrameObjects.Clear();
            aviStreamsInfo.Clear();
            foreach (var stream in aviCmpsStreams)
            {
                Avi.AVIStreamRelease(stream.Value);
            }
            aviCmpsStreams.Clear();
            foreach (var stream in aviStreams)
            {
                Avi.AVIStreamRelease(stream);
            }
            aviStreams.Clear();
            if (aviFile != IntPtr.Zero)
            {
                Avi.AVIFileRelease(aviFile);
                aviFile = IntPtr.Zero;
            }
            _FileName = string.Empty;
        }

        /// <summary>Читает из потока заданное количество сэмплов</summary>
        /// <param name="stream_index">Индекс потока в списке потоков, содержащихся в AVI-файле</param>
        /// <param name="start">Индекс первого читаемого сэмпла</param>
        /// <param name="samples">Количество сэмплов, которое нужно прочитать</param>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для чтения данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Неверный индекс первого читаемого сэмпла или количество сэмплов</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="AlfaPribor.AviFile.BufferTooSmallException">Размер буфера слишком мал для чтения одного сэмпла данных</exception>
        /// <returns>Буфер с прочитанными данные</returns>
        public byte[] Read(int stream_index, int start, ref int samples)
        {
            CheckDisposed();
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException("start");
            }
            if (samples < 1)
            {
                throw new ArgumentOutOfRangeException("samples");
            }
            byte[] buffer;
            int readBytes = 0;
            int readSamples = 0;
            // Определяем необходимый размер буфера для чтения сэмплов
            CheckAviFileResult(
                Avi.AVIStreamRead(aviStreams[stream_index], start, samples, IntPtr.Zero, 0, ref readBytes, ref readSamples)
                );
            IntPtr pBuffer = Marshal.AllocHGlobal(readBytes);
            try
            {
                // Читаем сэмплы из AVI-файла
                CheckAviFileResult(
                    Avi.AVIStreamRead(aviStreams[stream_index], start, samples, pBuffer, readBytes, ref readBytes, ref readSamples)
                    );
                buffer = new byte[readBytes];
                Marshal.Copy(pBuffer, buffer, 0, buffer.Length);
            }
            finally
            {
                Marshal.FreeHGlobal(pBuffer);
            }
            samples = readSamples;
            return buffer;
        }

        /// <summary>Читает из потока заданное количество сэмплов</summary>
        /// <param name="stream_index">Индекс потока в списке потоков, содержащихся в AVI-файле</param>
        /// <param name="start">Индекс первого читаемого сэмпла</param>
        /// <param name="samples">Количество сэмплов, которое нужно прочитать</param>
        /// <param name="pBuffer">Указатель на буфер в неуправляемой памяти, куда будут помещены прочитанные данные</param>
        /// <param name="count">Размер буфера/количество прочитанных байт данных</param>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для чтения данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Неверный индекс первого читаемого сэмпла, 
        /// или количество сэмплов, или размер буфера данных
        /// </exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="AlfaPribor.AviFile.BufferTooSmallException">Размер буфера слишком мал для чтения одного сэмпла данных</exception>
        public void Read(int stream_index, int start, ref int samples, IntPtr pBuffer, ref int count)
        {
            CheckDisposed();
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException("start");
            }
            if (samples < 1)
            {
                throw new ArgumentOutOfRangeException("samples");
            }
            if (pBuffer == IntPtr.Zero)
            {
                throw new ArgumentNullException("pBuffer");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            // Читаем сэмплы из AVI-файла
            CheckAviFileResult(
                Avi.AVIStreamRead(aviStreams[stream_index], start, samples, pBuffer, count, ref count, ref samples)
                );
        }

        /// <summary>Читает из потока заданное количество сэмплов</summary>
        /// <param name="stream_index">Индекс потока в списке потоков, содержащихся в AVI-файле</param>
        /// <param name="start">Индекс первого читаемого сэмпла</param>
        /// <param name="samples">Количество сэмплов, которое нужно прочитать</param>
        /// <param name="pBuffer">Указатель на буфер в неуправляемой памяти, куда будут помещены прочитанные данные</param>
        /// <param name="count">Размер буфера</param>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для чтения данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Неверный индекс первого читаемого сэмпла, 
        /// или количество сэмплов, или размер буфера данных
        /// </exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="AlfaPribor.AviFile.BufferTooSmallException">Размер буфера слишком мал для чтения одного сэмпла данных</exception>
        /// <returns>Количество прочитанных байт данных</returns>
        public int Read(int stream_index, int start, ref int samples, IntPtr pBuffer, int count)
        {
            CheckDisposed();
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException("start");
            }
            if (samples < 1)
            {
                throw new ArgumentOutOfRangeException("samples");
            }
            if (pBuffer == IntPtr.Zero)
            {
                throw new ArgumentNullException("pBuffer");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            int bytesRead = 0;
            // Читаем сэмплы из AVI-файла
            CheckAviFileResult(
                Avi.AVIStreamRead(aviStreams[stream_index], start, samples, pBuffer, count, ref bytesRead, ref samples)
                );
            return bytesRead;
        }

        /// <summary>Читает и декодирует видеоданные из видеопотока</summary>
        /// <param name="stream_index">Индекс потока в AVI-файле</param>
        /// <param name="pos">Номер позиции кадра в потоке</param>
        /// <param name="biHeader">Заголовок видеоданных кадра</param>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">Для заданного потока не выбран декомпрессор</exception>
        /// <exception cref="AlfaPribor.AviFile.DecompressorException">Произошла ошибка декомпрессии</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <returns>Прочитанные декодированные видеоданные</returns>
        public byte[] ReadDecompress(int stream_index, int pos, out Avi.BITMAPINFOHEADER biHeader)
        {
            CheckDisposed();
            IntPtr aviStream = aviStreams[stream_index];
            if (!aviGetFrameObjects.ContainsKey(aviStream))
            {
                // Декомпрессия потока еще не запускалась
                throw new AviFileException("Decompression not started!");
            }
            IntPtr pDIB = Avi.AVIStreamGetFrame(aviGetFrameObjects[aviStream], pos);
            if (pDIB == IntPtr.Zero)
            {
                throw new DecompressorException("Decompressor error!");
            }
            biHeader = (Avi.BITMAPINFOHEADER)Marshal.PtrToStructure(pDIB, typeof(Avi.BITMAPINFOHEADER));
            byte[] imageData = new byte[biHeader.biSizeImage];
            Marshal.Copy((IntPtr)(pDIB.ToInt32() + biHeader.biSize), imageData, 0, imageData.Length);
            return imageData;
        }

        /// <summary>Инициализирует декомпрессор для потока с заданным индексом</summary>
        /// <param name="stream_index">Индекс потока в списке потоков AVI-файла</param>
        /// <param name="biHeader">Заголовок видеокадра</param>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока лежит вне допустимого диапазона значений</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">Декомпрессия указанного потока уже начата</exception>
        /// <exception cref="AlfaPribor.AviFile.DecompressorException">Декомпрессор не найден</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public void BeginDecompress(int stream_index, Avi.BITMAPINFOHEADER biHeader)
        {
            CheckDisposed();
            IntPtr aviStream = aviStreams[stream_index];
            if (aviGetFrameObjects.ContainsKey(aviStream))
            {
                // Декомпрессия потока уже инициализирована
                throw new AviFileException("Decompression already started!");
            }
            Avi.BITMAPINFOHEADER bih = new Avi.BITMAPINFOHEADER();
            bih.biBitCount = biHeader.biBitCount;
            bih.biCompression = 0; //BI_RGB;
            bih.biHeight = biHeader.biHeight;
            bih.biWidth = biHeader.biWidth;
            bih.biPlanes = 1;
            bih.biSize = (UInt32)Marshal.SizeOf(bih);

            IntPtr getFrameObject = Avi.AVIStreamGetFrameOpen(aviStream, ref bih);
            if (getFrameObject == IntPtr.Zero)
            {
                throw new DecompressorException("Decompressor not found!");
            }
            aviGetFrameObjects.Add(aviStream, getFrameObject);
        }

        /// <summary>Деинициализирует декомпрессор для заданного потока</summary>
        /// <param name="stream_index">Индекс потока в списке потоков AVI-файла</param>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока лежит вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.DecompressorException">Ошибка декомпрессора данных</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        public void EndDecompress(int stream_index)
        {
            CheckDisposed();
            IntPtr aviStream = aviStreams[stream_index];
            if (!aviGetFrameObjects.ContainsKey(aviStream))
            {
                // Декомпрессия потока не инициализировалась
                return;
            }
            try
            {
                CheckAviFileResult(
                    Avi.AVIStreamGetFrameClose(aviGetFrameObjects[aviStream])
                    );
            }
            catch (CodecException e)
            {
                throw new DecompressorException(e.Message);
            }
            aviGetFrameObjects.Remove(aviStream);
        }

        /// <summary>Записывает в поток заданное количество сэмплов</summary>
        /// <param name="stream_index">Индекс потока в списке потоков, содержащихся в AVI-файле</param>
        /// <param name="start">Индекс первого записываемого в поток сэмпла</param>
        /// <param name="samples">Количество сэмплов, которое нужно записать</param>
        /// <param name="buffer">Ссылка на буфер, в котором размещены записываемые данные</param>
        /// <param name="offset">Смещение байтов (начиная с нуля) в buffer, с которого начинается копирование байтов в видео поток</param>
        /// <param name="count">Количество байтов, которое необходимо записать в видео поток</param>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для записи данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое сочетание параметров offset и count,
        /// которое ведет к выходу за границы массива buffer</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на массив с данными для записи</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <returns>Количество записанных байт данных</returns>
        public int Write(int stream_index, int start, ref int samples, byte[] buffer, int offset, int count)
        {
            CheckDisposed();

            int writeBytes = 0;
            int writeSamples = 0;
            IntPtr data = Marshal.AllocHGlobal(count);
            Marshal.Copy(buffer, offset, data, count);
            try
            {
                CheckAviFileResult(
                    Avi.AVIStreamWrite(aviStreams[stream_index], start, samples, data, count, 0, ref writeSamples, ref writeBytes)
                    );
            }
            finally
            {
                Marshal.FreeHGlobal(data);
            }
            samples = writeSamples;

            return writeBytes;
        }

        /// <summary>Записывает в поток заданное количество сэмплов</summary>
        /// <param name="stream_index">Индекс потока в списке потоков, содержащихся в AVI-файле</param>
        /// <param name="start">Индекс первого записываемого в поток сэмпла</param>
        /// <param name="samples">Количество сэмплов, которое нужно записать</param>
        /// <param name="pBuffer">Адрес буфера, в котором размещены записываемые данные</param>
        /// <param name="count">Количество записываемых байт данных</param>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для записи данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <returns>Количество записанных байт данных</returns>
        public int Write(int stream_index, int start, ref int samples, IntPtr pBuffer, int count)
        {
            CheckDisposed();

            int writeBytes = 0;
            int writeSamples = 0;
            CheckAviFileResult(
                Avi.AVIStreamWrite(aviStreams[stream_index], start, samples, pBuffer, count, 0, ref writeSamples, ref writeBytes)
                );
            samples = writeSamples;

            return writeBytes;
        }

        /// <summary>Записывает в сжатый поток заданное количество сэмплов</summary>
        /// <param name="stream_index">Индекс потока в списке потоков, содержащихся в AVI-файле</param>
        /// <param name="start">Индекс первого записываемого в поток сэмпла</param>
        /// <param name="samples">Количество сэмплов, которое нужно записать</param>
        /// <param name="buffer">Ссылка на буфер, в котором размещены записываемые данные</param>
        /// <param name="offset">Смещение байтов (начиная с нуля) в buffer, с которого начинается копирование байтов в видео поток</param>
        /// <param name="count">Количество байтов, которое необходимо записать в видео поток</param>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для записи данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое сочетание параметров offset и count,
        /// которое ведет к выходу за границы массива buffer</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на массив с данными для записи</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CompressorException">Компрессия заданного формата данных не поддерживается</exception>
        /// <returns>Количество записанных байт данных</returns>
        public int WriteCompress(int stream_index, int start, ref int samples, byte[] buffer, int offset, int count)
        {
            CheckDisposed();

            IntPtr stream;
            try
            {
                stream = aviCmpsStreams[aviStreams[stream_index]];
            }
            catch (KeyNotFoundException)
            {
                throw new IndexOutOfRangeException();
            }
            int writeBytes = 0;
            int writeSamples = 0;
            IntPtr data = Marshal.AllocHGlobal(count);
            Marshal.Copy(buffer, offset, data, count);
            try
            {
                try
                {
                    CheckAviFileResult(
                        Avi.AVIStreamWrite(stream, start, samples, data, count, 0, ref writeSamples, ref writeBytes)
                        );
                }
                finally
                {
                    Marshal.FreeHGlobal(data);
                }
            }
            catch (CodecException e)
            {
                throw new CompressorException(e.Message);
            }
            samples = writeSamples;

            return writeBytes;
        }

        /// <summary>Записывает в сжатый поток заданное количество сэмплов</summary>
        /// <param name="stream_index">Индекс потока в списке потоков, содержащихся в AVI-файле</param>
        /// <param name="start">Индекс первого записываемого в поток сэмпла</param>
        /// <param name="samples">Количество сэмплов, которое нужно записать</param>
        /// <param name="pBuffer">Адрес буфера, в котором размещены записываемые данные</param>
        /// <param name="count">Количество записываемых байт данных</param>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для записи данных</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CompressorException">Компрессия заданного формата данных не поддерживается</exception>
        /// <returns>Количество записанных байт данных</returns>
        public int WriteCompress(int stream_index, int start, ref int samples, IntPtr pBuffer, int count)
        {
            CheckDisposed();

            IntPtr stream;
            try
            {
                stream = aviCmpsStreams[aviStreams[stream_index]];
            }
            catch (KeyNotFoundException)
            {
                throw new IndexOutOfRangeException();
            }
            int writeBytes = 0;
            int writeSamples = 0;
            try
            {
                CheckAviFileResult(
                    Avi.AVIStreamWrite(stream, start, samples, pBuffer, count, 0, ref writeSamples, ref writeBytes)
                    );
            }
            catch (CodecException e)
            {
                throw new CompressorException(e.Message);
            }
            samples = writeSamples;

            return writeBytes;
        }

        /// <summary>Создает новый поток с заданными характеристиками</summary>
        /// <param name="info">Характеристики создаваемого потока</param>
        /// <param name="bmiHeader">Параметры видеоизображения, которое будет записыаться в поток</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске или ошибка чтения/записи в файл</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">Невозможно создать поток</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <returns>Индекс созданного потока в общем списке потоков, содержащихся в AVI-файле</returns>
        public int CreateStream(Avi.AVISTREAMINFO info, Avi.BITMAPINFOHEADER bmiHeader)
        {
            CheckDisposed();
            IntPtr newAviStream = IntPtr.Zero;
            CheckAviFileResult(
                Avi.AVIFileCreateStream(aviFile, out newAviStream, ref info)
                );
            try
            {
                SetStreamFormat(newAviStream, bmiHeader);
            }
            catch
            {
                Avi.AVIStreamRelease(newAviStream);
                throw;
            }
            if (newAviStream == IntPtr.Zero)
            {
                throw new AviFileException("Can not create stream!");
            }
            aviStreams.Add(newAviStream);
            aviStreamsInfo.Add(newAviStream, info);
            return aviStreams.Count - 1;
        }

        /// <summary>Создает новый видеопоток в открытом AVI-файле</summary>
        /// <param name="info">Характеристики создаваемого потока</param>
        /// <param name="bmiHeader">Параметры видеоизображения, которое будет записываться в поток</param>
        /// <param name="options">Настройки сжатия (для компрессора)</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске или ошибка чтения/записи в файл</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CompressorException">Компрессия заданного формата данных не поддерживается</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">Невозможно создать поток</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование невозможно</exception>
        /// <returns>Индекс созданного потока в общем списке потоков, содержащихся в AVI-файле</returns>
        public int CreateCompressedStream(Avi.AVISTREAMINFO info, Avi.BITMAPINFOHEADER bmiHeader, Avi.AVICOMPRESSOPTIONS options)
        {
            IntPtr newAviStream = IntPtr.Zero;
            CheckAviFileResult(
                Avi.AVIFileCreateStream(aviFile, out newAviStream, ref info)
                );
            if (newAviStream == IntPtr.Zero)
            {
                throw new AviFileException("Can not create stream!");
            }
            IntPtr stream = newAviStream;
            IntPtr compressedStream;
            IntPtr clsidHandler = IntPtr.Zero;
            try
            {
                CheckAviFileResult(
                    Avi.AVIMakeCompressedStream(out compressedStream, stream, ref options, clsidHandler)
                    );
            }
            catch (CodecException e)
            {
                throw new CompressorException(e.Message);
            }
            try
            {
                SetStreamFormat(compressedStream, bmiHeader);
            }
            catch
            {
                Avi.AVIStreamRelease(compressedStream);
                throw;
            }

            if (compressedStream == IntPtr.Zero)
            {
                throw new AviFileException("Can not create compressed stream!");
            }
            // запомнить указатель на несжатый поток
            aviStreams.Add(newAviStream);
            aviStreamsInfo.Add(newAviStream, info);
            // запомнить указатель на сжатый поток
            aviCmpsStreams.Add(stream, compressedStream);
            return aviStreams.Count - 1;
        }

        /// <summary>Устанавливает формат видеопотока путем записи данных в AVI-файл</summary>
        /// <param name="stream">Указатель на интерфейс доступа к видеопотоку</param>
        /// <param name="bmiHeader">Информация о видеоизображении, содержащемся в потоке</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске или ошибка чтения/записи в файл</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CodecException">Ошибка кодека данных</exception>
        private void SetStreamFormat(IntPtr stream, Avi.BITMAPINFOHEADER bmiHeader)
        {
            int bmi_size = Marshal.SizeOf(typeof(Avi.BITMAPINFOHEADER));
            IntPtr bmi_ptr = Marshal.AllocHGlobal(bmi_size);
            Marshal.StructureToPtr(bmiHeader, bmi_ptr, false);
            try
            {
                CheckAviFileResult(
                    Avi.AVIStreamSetFormat(stream, 0, bmi_ptr, bmi_size)
                    );
            }
            finally
            {
                Marshal.FreeHGlobal(bmi_ptr);
            }
        }

        /// <summary>Получить информацию по всем потокам, содержащимся в AVI-файле</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены.
        /// Дальнейшее использование невозможно
        /// </exception>
        /// <returns>Список с информацией о всех потоках</returns>
        public IList<Avi.AVISTREAMINFO> GetStreamsInfo()
        {
            CheckDisposed();
            List<Avi.AVISTREAMINFO> infoList = new List<Avi.AVISTREAMINFO>(aviStreamsInfo.Values);
            return infoList.AsReadOnly();
        }

        /// <summary>Получить информацию о потоке с заданным индексом</summary>
        /// <param name="index">Индекс потока в общем списке потоков, содержащихся в AVI-файле</param>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены.
        /// Дальнейшее использование невозможно
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапазона значений</exception>
        /// <returns>Информация о потоке</returns>
        public Avi.AVISTREAMINFO GetStreamInfo(int index)
        {
            CheckDisposed();
            return aviStreamsInfo[aviStreams[index]];
        }

        /// <summary>Получить заголовок изображения заданного кадра</summary>
        /// <param name="stream_index">Индекс потока в общем списке потоков AVI-файла</param>
        /// <param name="frame_pos">Номер кадра, заголовок изображения которого требуется получить</param>
        /// <exception cref="System.Exception">Ошибка чтения данных из потока</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены.
        /// Дальнейшее использование невозможно
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">Индекс потока вне допустимого диапозона значений</exception>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">Заданный поток не содержит видеоданных</exception>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CodecException">Ошибка кодека данных</exception>
        /// <returns>Заголовок изображения кадра</returns>
        public Avi.BITMAPINFOHEADER GetFrameInfo(int stream_index, int frame_pos)
        {
            CheckDisposed();
            IntPtr stream = aviStreams[stream_index];
            if (aviStreamsInfo[stream].fccType != Avi.StreamtypeVIDEO)
            {
                throw new AviFileException("Stream with index '" + stream_index.ToString() + "' is not video type!");
            }
            Avi.BITMAPINFOHEADER bmiHeader;
            int bmihSize = 0;
            CheckAviFileResult(
                Avi.AVIStreamReadFormat(aviStreams[stream_index], frame_pos, IntPtr.Zero, ref bmihSize)
                );
            IntPtr p_bmiHeader = Marshal.AllocHGlobal(bmihSize);
            try
            {
                CheckAviFileResult(
                    Avi.AVIStreamReadFormat(aviStreams[stream_index], frame_pos, p_bmiHeader, ref bmihSize)
                    );
                bmiHeader = (Avi.BITMAPINFOHEADER)Marshal.PtrToStructure(p_bmiHeader, typeof(Avi.BITMAPINFOHEADER));
            }
            finally
            {
                Marshal.FreeHGlobal(p_bmiHeader);
            }
            return bmiHeader;
        }

        /// <summary>Создает объект Bitmap для хранения данных изображения</summary>
        /// <param name="bmiHeader">Заголовок изображения</param>
        /// <param name="data">Видеоданные</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект с данными изображения</exception>
        /// <returns>Объект Bitmap, содержащий изображение</returns>
        public static Bitmap GetBitmap(Avi.BITMAPINFOHEADER bmiHeader, byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }
            int bmiHeaderSize = Marshal.SizeOf(bmiHeader);
            IntPtr pbmiHeader = Marshal.AllocHGlobal(bmiHeaderSize);
            Marshal.StructureToPtr(bmiHeader, pbmiHeader, false);
            byte[] bmih = new byte[bmiHeaderSize];
            Marshal.Copy(pbmiHeader, bmih, 0, bmih.Length);
            Marshal.FreeHGlobal(pbmiHeader);

            Avi.BITMAPFILEHEADER bmfHeader = new Avi.BITMAPFILEHEADER();
            int bmfHeaderSize = Marshal.SizeOf(bmfHeader);
            bmfHeader.bfOffBits = bmfHeaderSize + bmiHeaderSize;
            bmfHeader.bfSize = bmfHeader.bfOffBits + 1 + data.Length;
            bmfHeader.bfType = Avi.BMP_MAGIC_COOKIE;

            IntPtr pbmfHeader = Marshal.AllocHGlobal(bmfHeaderSize);
            Marshal.StructureToPtr(bmfHeader, pbmfHeader, false);
            byte[] bmfh = new byte[bmfHeaderSize];
            Marshal.Copy(pbmfHeader, bmfh, 0, bmfh.Length);
            Marshal.FreeHGlobal(pbmfHeader);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(bmfh, 0, bmfh.Length);
                stream.Write(bmih, 0, bmih.Length);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;

                return new Bitmap(stream);
            }
        }

        /// <summary>Проверка на освобождение ресурсов объекта</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены.
        /// Дальнейшее использование невозможно</exception>
        protected void CheckDisposed()
        {
            if (!_Disposed) return;
            throw new ObjectDisposedException("AviFile");
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
            Avi.AVIFileExit();
            if (disposing)
            {
                aviFile = IntPtr.Zero;
                aviStreams = null;
                aviCmpsStreams = null;
                aviStreamsInfo = null;
                aviGetFrameObjects = null;
            }
            _Disposed = true;
        }

        #endregion

        #region Properties

        /// <summary>Количество существующих потоков в AVI-файле</summary>
        public int StreamsCount
        {
            get { return aviStreams.Count; }
        }

        /// <summary>Имя открытого AVI-файла</summary>
        public string FileName
        {
            get { return _FileName; }
        }

        /// <summary>Отображает состояние файла - открыт/закрыт</summary>
        public bool IsOpen
        {
            get { return !string.IsNullOrEmpty(_FileName); }
        }

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
