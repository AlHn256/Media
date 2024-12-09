using System;
using System.IO;

namespace AlfaPribor.Streams2
{
    /// <summary>Позволяет идентифицировать данные внутри потока по заголовку - 
    /// информации о данных,размещаемой в начале потока
    /// возможно интерфейс и класс следует включить в сборку AlfaPribor.Streams</summary>
    public abstract class StreamWithHeader2 : Stream, IStreamWithHeader2
    {

        #region Fields

        /// <summary>Поток для записи/чтения данных</summary>
        protected Stream _Stream;

        /// <summary>Внутренний буфер данных (универсальный для старых классов)</summary>
        //protected byte[] buffer;

        /// <summary>Внутренний буфер запиис данных</summary>
        protected byte[] buffer_write;

        /// <summary>Внутренний буфер чтения данных</summary>
        protected byte[] buffer_read;

        /// <summary>Размер обязательного заголовка потока данных (байт)</summary>
        int _HeaderSize;

        /// <summary>Признак свершившейся операции записи заголовка потока</summary>
        bool _HeaderWrited;

        /// <summary>Признак свершившейся операции чтения заголовка потока</summary>
        bool _HeaderReaded;

        /// <summary>Подпись данных, содержащаяся в заголовке, для их идентификации</summary>
        protected byte[] _Signature;

        /// <summary>Номер версии формата хранения данных</summary>
        protected int _Version;

        /// <summary>Показывает, освобождались управляемые ресурсы объекта или нет</summary>
        bool disposed;

        /// <summary>Признак, отображающий внесение изменений в открытый поток</summary>
        bool _Modified;

        #endregion

        #region Methods

        /// <summary>Инициализирует объект для записи/чтения данных в поток</summary>
        /// <param name="stream">Указатель на поток, с которым будем работать</param>
        /// <param name="h_size">Размер заголовка потока (байт)</param>
        /// <exception cref="System.ArgumentNullException">Не задан целевой поток</exception>
        public StreamWithHeader2(Stream stream, int h_size)
        {
            if (stream == null) throw new ArgumentNullException();
            _Stream = stream;
            _HeaderSize = h_size;
            _HeaderWrited = false;
            _HeaderReaded = false;
            buffer_read = null;
            buffer_write = null;
            _Signature = null;
            _Version = 1;
            _Modified = false;
            disposed = false;
        }

        /// <summary>Считывает данные заголовка потока</summary>
        /// <returns>Возвращает TRUE в случае успешного чтения или FALSE, если чтение завершилось ошибкой
        /// (заголовок имеет неверный формат)</returns>
        protected abstract bool DoReadHeader();

        /// <summary>Считывает данные заголовка потока и восстанавливает положение курсора
        /// по состоянию, предшествующему началу чтения</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода/вывода в поток</exception>
        /// <exception cref="System.NotSupportedException">Запрошенная операция не поддерживается потоком</exception>
        /// <exception cref="System.ObjectDisposedException">Вызов метода для уничтоженного объекта</exception>
        /// <returns>Возвращает TRUE в случае успешного чтения или FALSE, если чтение завершилось ошибкой
        /// (заголовок имеет неверный формат)</returns>
        protected bool ReadHeader()
        {
            CheckDisposed();
            long position = _Stream.Position;
            bool need_restore = false;
            if (position != 0)
            {
                need_restore = true;
                _Stream.Seek(0, SeekOrigin.Begin);
            }
            bool result;
            try
            {
                result = DoReadHeader();
                if (result) _HeaderReaded = true;
            }
            finally
            {
                if (need_restore) _Stream.Position = position;
            }
            return result;
        }

        /// <summary>Записывает данные заголовка всего потока</summary>
        /// <returns>Возвращает TRUE в случае успешной записи или FALSE, если запись не была произведена</returns>
        protected abstract bool DoWriteHeader();

        /// <summary>Записывает данные заголовка в начало потока и восстанавливает положение курсора
        /// по состоянию предшествующему началу записи</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода/вывода в поток</exception>
        /// <exception cref="System.NotSupportedException">Запрошенная операция не поддерживается потоком</exception>
        /// <exception cref="System.ObjectDisposedException">Вызов метода для уничтоженного объекта</exception>
        /// <returns>Возвращает TRUE в случае успешной записи или FALSE, если запись не была произведена</returns>
        protected bool WriteHeader()
        {
            CheckDisposed();
            long position = _Stream.Position;
            bool need_restore = false;
            if (position != 0)
            {
                need_restore = true;
                _Stream.Seek(0, SeekOrigin.Begin);
            }
            bool result;
            try
            {
                result = DoWriteHeader();
                if (result) _HeaderWrited = true;
            }
            finally
            {
                if (need_restore) _Stream.Position = position;
            }
            return result;
        }

        /// <summary>Проверяет структуру заголовка потока</summary>
        /// <returns>Возвращает результат проверки заголовка потока</returns>
        public abstract StreamHeaderState CheckHeader();

        /// <summary>Сравнивает массивы по содержимому</summary>
        /// <param name="array1">Ссылка на первый массив</param>
        /// <param name="offset1">Индекс первого элемента в массиве array1, с которого следует начинать сравнение</param>
        /// <param name="array2">Ссылка на второй массив</param>
        /// <param name="offset2">Индекс первого элемента в массиве array2, с которого следует начинать сравнение</param>
        /// <param name="count">Количество элементов , которые требуется сравнить</param>
        /// <exception cref="System.ArgumentNullException">Отсутствуют ссылки на один или оба массива</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значения параметра offset1 или offset2 выходят за границы допустимых значений</exception>
        /// <exception cref="System.ArgumentException">Недопустимое сочетание элементов offset1/offset2 и count</exception>
        /// <returns>Возвращает следующие значения:
        /// меньше 0 = первый массив меньше второго;
        /// больше 0 = первый массив больше второго;
        /// 0 = содержимое массивов равно</returns>
        public static int Compare(byte[] array1, int offset1, byte[] array2, int offset2, int count)
        {
            // Проверяем значения параметров
            if (array1 == null)                                 throw new ArgumentNullException("array1");
            if ((offset1 < 0) || (offset1 >= array1.Length))    throw new ArgumentOutOfRangeException("offset1");
            if ((offset1 + count > array1.Length))              throw new ArgumentException();
            if (array2 == null)                                 throw new ArgumentNullException("array2");
            if ((offset2 < 0) || (offset2 >= array2.Length))    throw new ArgumentOutOfRangeException("offset2");
            if ((offset2 + count > array2.Length))              throw new ArgumentException();
            // Сравниваем массивы по значениям элементов
            for (int i = 0; i < count; ++i)
            {
                int index1 = offset1 + i;
                int index2 = offset2 + i;
                if (array1[index1] != array2[index2]) return array1[index1] - array2[index2];
            }
            return 0;
        }

        /// <summary>Считывает данные из потока и сравнивает с эталонной последовательностью байтов</summary>
        /// <param name="stream">Указатель на поток, из которого будут читаться данные</param>
        /// <param name="cmp_data">Последовательность байт, с которой требуется провести сравнение</param>
        /// <param name="buffer">Массив байт, в который будет проводится считывание из потока</param>
        /// <param name="offset">Смещение в массиве buffer, начиная с которого будут записываться прочитанные из потока данные</param>
        /// <param name="count">Количество прочитанных байтов</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentException">Сумма значений параметра offset и массива cmp_data больше длины буфера buffer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение параметра offset (меньше нуля или больше длины буфера buffer)</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства значению null одного из параметров</exception>
        /// <returns>Возвращает TRUE, если прочитанная последовательность байтов идентична заданной, в противном случае возвращает FALSE</returns>
        public static bool ReadAndCompare(Stream stream, byte[] cmp_data, byte[] buffer, int offset, ref int count)
        {
            count = 0;
            if (stream == null) throw new ArgumentNullException("stream");
            if (cmp_data == null) throw new ArgumentNullException("cmp_data");
            // Считываем данные из буфера в поток и проверяем количество прочитанных байтов
            count = stream.Read(buffer, offset, cmp_data.Length);
            if (cmp_data.Length > count) return false;
            // Делаем побайтовое сравнение
            for (int i = 0; i < cmp_data.Length; i++)
            {
                if (cmp_data[i] != buffer[offset + i]) return false;
            }
            return true;
        }

        /// <summary>Считывает данные из потока и сравнивает с эталонной последовательностью байтов</summary>
        /// <param name="cmp_data">Последовательность байт, с которой требуется провести сравнение</param>
        /// <param name="buffer">Массив байт, в который будет проводится считывание из потока</param>
        /// <param name="offset">Смещение в массиве buffer, начиная с которого будут записываться прочитанные из потока данные</param>
        /// <param name="count">Количество прочитанных байтов</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentException">Сумма значений параметра offset и массива cmp_data больше длины буфера buffer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение параметра offset (меньше нуля или больше длины буфера buffer)</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства значению null одного из параметров</exception>
        /// <returns>Возвращает TRUE, если прочитанная последовательность байтов идентична заданной, в противном случае возвращает FALSE</returns>
        protected bool ReadAndCompare(byte[] cmp_data, byte[] buffer, int offset, ref int count)
        {
            count = 0;
            if (cmp_data == null) throw new ArgumentNullException("cmp_data");
            // Считываем данные из буфера в поток и проверяем количество прочитанных байтов
            count = _Stream.Read(buffer, offset, cmp_data.Length);
            if (cmp_data.Length > count) return false;
            // Делаем побайтовое сравнение
            for (int i = 0; i < cmp_data.Length; i++)
            {
                if (cmp_data[i] != buffer[offset + i]) return false;
            }
            return true;
        }

        /// <summary>Считывает данные из потока и сравнивает с эталонной последовательностью байтов</summary>
        /// <param name="cmp_data">Последовательность байт, с которой требуется провести сравнение</param>
        /// <param name="buffer">Массив байт, в который будет проводится считывание из потока</param>
        /// <param name="offset">Смещение в массиве buffer, начиная с которого будут записываться прочитанные из потока данные</param>
        /// <param name="count">Количество прочитанных байтов</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentException">Сумма значений параметра offset и массива cmp_data больше длины буфера buffer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение параметра offset (меньше нуля или больше длины буфера buffer)</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства значению null одного из параметров</exception>
        /// <returns>Возвращает TRUE, если прочитанная последовательность байтов идентична заданной, в противном случае возвращает FALSE</returns>
        protected bool ReadAndCompare(byte[] cmp_data, byte[] buffer, int offset, ref int count, ref string err)
        {
            count = 0;
            if (cmp_data == null) { throw new ArgumentNullException("cmp_data"); }
            // Считываем данные из буфера в поток и проверяем количество прочитанных байтов
            count = _Stream.Read(buffer, offset, cmp_data.Length);
            if (cmp_data.Length > count) return false;
            // Делаем побайтовое сравнение
            for (int i = 0; i < cmp_data.Length; i++)
            {
                if (cmp_data[i] != buffer[offset + i])
                {
                    err = cmp_data[i].ToString() + "!=" + buffer[offset + i].ToString();
                    return false;
                }
            }
            return true;
        }

        /// <summary>Считывает данные из потока до тех пор, пока не встретится заданная последовательность байтов
        /// или пока не закончится поток</summary>
        /// <param name="stream">Указатель на поток, из которого будут читаться данные</param>
        /// <param name="boundary">Последовательность байт, которая служит признаком окончания чтения</param>
        /// <param name="buffer">Массив байт, принимающий прочитанные из потока данные</param>
        /// <param name="offset">Смещение в массиве buffer, начиная с которого будут записываться прочитанные из потока данные</param>
        /// <param name="count">Количество прочитанных байтов</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение параметра offset (меньше нуля или больше длины буфера buffer)</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства значению null одного из параметров</exception>
        /// <exception cref="System.IndexOutOfRangeException">В процессе чтения произошел выход за границы массива buffer</exception>
        /// <returns>Возвращает TRUE, если заданная последовательность байтов найдена в потоке и прочитана,
        /// в противном случае возвращает FALSE</returns>
        public static bool ReadUntil(Stream stream, byte[] boundary, byte[] buffer, int offset, ref int count)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (boundary == null) throw new ArgumentNullException("boundary");
            if (buffer == null) throw new ArgumentNullException("buffer");
            if ((offset < 0) || (offset >= buffer.Length)) throw new ArgumentOutOfRangeException("offset");
            bool equal = false;
            count = 0;
            int cmp_index = 0;
            while (!equal)
            {
                if (stream.Read(buffer, offset + count, 1) == 0) break;
                try
                {
                    if (buffer[offset + count] == boundary[cmp_index])
                    {
                        ++cmp_index;
                        // Проверяем на предмет полного совпадения последовательности прочитанных байтов
                        if (cmp_index == boundary.Length) equal = true;
                    }
                    else cmp_index = 0;
                }
                finally { ++count; }
            }
            return equal;
        }

        /// <summary>Считывает данные из потока до тех пор, пока не встретится заданная последовательность байтов
        /// или пока не закончится поток</summary>
        /// <param name="boundary">Последовательность байт, которая служит признаком окончания чтения</param>
        /// <param name="buffer">Массив байт, принимающий прочитанные из потока данные</param>
        /// <param name="offset">Смещение в массиве buffer, начиная с которого будут записываться прочитанные из потока данные</param>
        /// <param name="count">Количество прочитанных байтов</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение параметра offset (меньше нуля или больше длины буфера buffer)</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства значению null одного из параметров</exception>
        /// <exception cref="System.IndexOutOfRangeException">В процессе чтения произошел выход за границы массива buffer</exception>
        /// <returns>Возвращает TRUE, если заданная последовательность байтов найдена в потоке и прочитана, в противном случае возвращает FALSE</returns>
        protected bool ReadUntil(byte[] boundary, byte[] buffer, int offset, ref int count)
        {
            if (boundary == null) throw new ArgumentNullException("boundary");
            if (buffer == null) throw new ArgumentNullException("buffer");
            if ((offset < 0) || (offset >= buffer.Length)) throw new ArgumentOutOfRangeException("offset");
            count = 0; 
            bool equal = false;
            int cmp_index = 0;
            while (!equal)
            {
                if (_Stream.Read(buffer, offset + count, 1) == 0) break;
                try
                {
                    if (buffer[offset + count] == boundary[cmp_index])
                    {
                        ++cmp_index;
                        // Проверяем на предмет полного совпадения последовательности прочитанных байтов
                        if (cmp_index == boundary.Length) equal = true;
                    }
                    else cmp_index = 0;
                }
                finally { ++count; }
            }
            return equal;
        }

        /// <summary>Проверяет факт освобождения объектом ресурсов</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        protected void CheckDisposed()
        {
            if (disposed) throw new ObjectDisposedException(this.ToString());
        }

        #endregion

        #region Properties

        /// <summary>Признак освобождения объектом ресурсов. TRUE - ресурсы освобождены</summary>
        public bool Disposed
        {
            get { return disposed; }
        }

        /// <summary>Поток для записи/чтения данных</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        protected Stream Stream 
        { 
            get 
            {
                CheckDisposed();
                return _Stream; 
            } 
        }

        /// <summary>Подпись данных, содержащаяся в заголовке, для их идентификации</summary>
        /// <exception cref="System.ObjectDisposedException">Возникает в случае, если метод был вызван после закрытия потока</exception>
        public byte[] Signature
        {
            get
            {
                CheckDisposed();
                return _Signature;
            }
        }

        /// <summary>Номер версии формата хранения данных</summary>
        /// <exception cref="System.ObjectDisposedException">Возникает в случае, если метод был вызван после закрытия потока</exception>
        public int Version
        {
            get
            {
                CheckDisposed();
                return _Version;
            }
        }

        /// <summary>Признак, отображающий внесение изменений в открытый поток</summary>
        /// <exception cref="System.ObjectDisposedException">Возникает в случае, если метод был вызван после закрытия потока</exception>
        public bool Modified
        {
            get 
            {
                CheckDisposed();
                return _Modified; 
            }
        }

        /// <summary>Признак свершившейся операции записи заголовка потока</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        protected bool HeaderWrited
        {
            get 
            {
                CheckDisposed();
                return _HeaderWrited; 
            }
        }

        /// <summary>Признак свершившейся операции чтения заголовка потока</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        protected bool HeaderReaded
        {
            get 
            {
                CheckDisposed();
                return _HeaderReaded;
            }
        }

        /// <summary>Размер обязательного заголовка потока данных (байт)</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        public int HeaderSize
        {
            get 
            {
                CheckDisposed();
                return _HeaderSize;
            }
            protected set { _HeaderSize = value; }
        } 

        #endregion

        #region Stream Members

        /// <summary>Освобождает управляемые/неуправляемые ресурсы потока</summary>
        /// <param name="disposing">
        /// Значение true позволяет освободить управляемые и неуправляемые ресурсы;
        /// значение false позволяет освободить только неуправляемые ресурсы</param>
        protected override void Dispose(bool disposing)
        {
            try { _Stream.Dispose(); }
            catch { }
            if (disposing)
            {
                _Signature = null;
                _Stream = null;
                buffer_read = null;
                buffer_write = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>Возвращает значение, показывающее, поддерживает ли текущий поток возможность чтения</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        public override bool CanRead
        {
            get 
            {
                CheckDisposed();
                return _Stream.CanRead;
            }
        }

        /// <summary>Возвращает значение, показывающее, поддерживает ли текущий поток возможность поиска</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        public override bool CanSeek
        {
            get 
            {
                CheckDisposed();
                return _Stream.CanSeek;
            }
        }

        /// <summary>Возвращает значение, показывающее, поддерживает ли текущий поток возможность записи</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        public override bool CanWrite
        {
            get 
            {
                CheckDisposed();
                return _Stream.CanWrite;
            }
        }

        /// <summary>Вызывает запись буферов в базовое устройство</summary>
        /// <exception cref="System.IO.IOException">Возникает в случае ошибки ввода/вывода</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы освобождены. Дальнейшее использование объекта невозможно</exception>
        public override void Flush()
        {
            CheckDisposed();
            _Stream.Flush();
        }

        /// <summary>Длина потока без заголовка</summary>
        /// <exception cref="System.NotSupportedException">Возникает в случае, если базовый поток не поддерживает возможность поиска</exception>
        /// <exception cref="System.ObjectDisposedException">Возникает в случае, если метод был вызван после закрытия потока</exception>
        public override long Length
        {
            get
            {
                CheckDisposed();
                long len = _Stream.Length - _HeaderSize;
                if (len < 0) return 0;
                else return len;
            }
        }

        /// <summary>Получает или задает позицию чтения/записи данных в потоке</summary>
        /// <exception cref="System.IO.IOException">Возникает в случае ошибки ввода/вывода</exception>
        /// <exception cref="System.NotSupportedException">Базовый поток не поддерживает операции поиска</exception>
        /// <exception cref="System.ObjectDisposedException">Возникает в случае, если метод был вызван после закрытия потока</exception>
        public override long Position
        {
            get
            {
                CheckDisposed();
                if (_Stream.Position < _HeaderSize) return 0;
                else return _Stream.Position - _HeaderSize;
            }
            set
            {
                CheckDisposed();
                _Stream.Position = _HeaderSize + value;
            }
        }

        /// <summary>Cчитывает последовательность байтов из базового потока</summary>
        /// <param name="buffer">Массив байт, в которых будут помещены прочитанные данные</param>
        /// <param name="offset">Смещение байтов (начиная с нуля) в buffer, с которого начинается сохранение данных,
        /// считанных из базового потока</param>
        /// <param name="count">Количество байт, которые нужно прочитать из потока</param>
        /// <returns>Общее количество байтов, считанных в буфер.Это число может быть меньше количества запрошенных байтов,
        /// если столько байтов в настоящее время недоступно, а также равняться нулю (0),
        /// если был достигнут конец потока</returns>
        /// <exception cref="System.ArgumentException">Сумма значений параметров offset и count больше длины буфера</exception>
        /// <exception cref="System.ArgumentNullException">Значение параметра buffer — null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Параметры offset или count являются отрицательными</exception>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            return _Stream.Read(buffer, offset, count);
        }

        /// <summary>Задает позицию для чтения/записи данных</summary>
        /// <param name="offset">Смещение в количестве элементов SingleStreamFrameIndex относительно параметра origin</param>
        /// <param name="origin">Определяет точку отсчета, которая используется для получения новой позиции</param>
        /// <returns>Новая позиция в текущем потоке</returns>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает поиск</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();
            long len = _Stream.Length;
            long pos = _Stream.Position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset > 0) offset += _HeaderSize;
                    else throw new IOException("Попытка переместить указатель на данные перед началом данных!");
                    break;
                case SeekOrigin.Current:
                    if ((offset < 0) && (offset < -pos + _HeaderSize))
                        throw new IOException("Попытка переместить указатель на данные перед началом данных!");
                    break;
                case SeekOrigin.End:
                    if ((offset < 0) && (offset < -len + _HeaderSize))
                        throw new IOException("Попытка переместить указатель на данные перед началом данных!");
                    break;
                default: break;
            }
            return _Stream.Seek(offset, origin) - _HeaderSize;
        }

        /// <summary>Задает длину текущего потока в количестве данных, которые он способен уместить</summary>
        /// <param name="value">Новая длина потока в количестве элементов типа SingleStreamFrameIndex</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает ни поиск, ни запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        public override void SetLength(long value)
        {
            CheckDisposed();
            _Stream.SetLength(_HeaderSize + value);
        }

        /// <summary>Записывает последовательность байтов в базовый поток</summary>
        /// <param name="buffer">Массив байтов, которые будут помещены в поток</param>
        /// <param name="offset">Смещение байтов (начиная с нуля) в buffer, с которого начинается копирование байтов в текущий поток</param>
        /// <param name="count">Количество байт, которые нужно записать в поток</param>
        /// <exception cref="System.ArgumentException">Сумма значений параметров offset и count больше длины буфера</exception>
        /// <exception cref="System.ArgumentNullException">Значение параметра buffer — null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Параметры offset или count являются отрицательными</exception>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            _Stream.Write(buffer, offset, count);
            _Modified = true;
        }

        #endregion
    }
}
