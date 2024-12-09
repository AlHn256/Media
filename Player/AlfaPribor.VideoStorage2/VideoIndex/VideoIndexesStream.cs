using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.Streams2;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Обеспечивает чтение/запись индексов видеоданных в поток</summary>
    class VideoIndexesStream : StreamWithHeader2, IVideoIndexesStream
    {

        #region Fields

        /// <summary>Размер класса данных SingleStreamFrameIndex (байт)</summary>
        protected int IndexSize;

        #endregion

        #region Methods

        /// <summary>Инициализирует объект для записи/чтения индексов видеоданных в поток</summary>
        /// <param name="stream">Указатель на поток, с которым будем работать</param>
        /// <exception cref="System.ArgumentNullException">Не задан целевой поток</exception>
        public VideoIndexesStream(Stream stream) : base(stream, 16)
        {
            if (stream == null) throw new ArgumentNullException();
            IndexSize = new SingleStreamFrameIndex(0).SIZE();
            buffer_read = new byte[(IndexSize < 16) ? 16 : IndexSize];
            buffer_write = new byte[(IndexSize < 16) ? 16 : IndexSize];
            _Version = 1;
            _Signature = Encoding.Default.GetBytes("FIDX"); // 0x58444946
        }

        /// <summary>Читает очередное значение индекса видеоданных из потока</summary>
        /// <param name="index">Ссылка на объект, принимающий данные</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный индекс содержит ошибку</exception>
        /// <returns>Возвращает количество прочитанных из потока байтов, если видеоиндекс был прочитан успешно.
        /// В случае достижения конца потока возвращает 0</returns>
        protected virtual int DoReadIndex(out SingleStreamFrameIndex index)
        {
            index = null;
            int read_bytes = Read(buffer_read, 0, IndexSize);
            if ((read_bytes == 0) || (read_bytes < IndexSize)) return 0;

            SingleStreamFrameIndex result;
            try
            {
                result = new SingleStreamFrameIndex(BitConverter.ToInt32(buffer_read, 0));
                result.TimeStamp = BitConverter.ToInt32(buffer_read, 4);
                result.FileOffset = BitConverter.ToInt64(buffer_read, 8);
            }
            catch
            {
                throw new InvalidStreamDataException();
            }
            index = result;
            return read_bytes;
        }

        /// <summary>Читает очередное значение индекса видеоданных из потока.
        /// Вызывает метод чтения и проверки заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="index">Ссылка на объект, принимающий данные</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamHeaderException">Заголовок потока содержит ошибку</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный индекс содержит ошибку</exception>
        /// <returns>Возвращает количество прочитанных из потока байтов, если видеоиндекс был прочитан успешно.
        /// В случае достижения конца потока возвращает 0</returns>
        public int ReadIndex(out SingleStreamFrameIndex index)
        {
            CheckDisposed();
            if (!HeaderReaded && !HeaderWrited)
            {
                StreamHeaderState header_state = CheckHeader();
                if (header_state != StreamHeaderState.Ok) { throw new InvalidStreamHeaderException(header_state); }
            }
            return DoReadIndex(out index);
        }

        /// <summary>Считывает значения индексов видеоданных из потока.
        /// Вызывает метод чтения и проверки заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="indexes">Ссылка на массив, принимающий данные</param>
        /// <param name="offset">Индекс первого элемента в массиве, начиная с которого данные видеоиндексов будут записываться в массив</param>
        /// <param name="count">Количество видеоиндексов, которое нужно прочитать из потока</param>
        /// <param name="bytes_readed">По завершении метода этот параметр будет содержать число байтов, прочитанных из потока</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства параметра indexes значению null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение параметра offset выходит из границ диапозона допустимых значений</exception>
        /// <exception cref="System.ArgumentException">Недопустимое сочетание параметров offset и count</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamHeaderException">Заголовок потока содержит ошибку</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный индекс содержит ошибку</exception>
        /// <returns>Возвращает число прочитанных индексов</returns>
        public int Read(SingleStreamFrameIndex[] indexes, int offset, int count, ref int bytes_readed)
        {
            CheckDisposed();
            if (indexes == null) { throw new ArgumentNullException(); }
            if ((offset < 0) || (offset >= indexes.Length)) { throw new ArgumentOutOfRangeException("offset"); }
            if (offset + count > indexes.Length) { throw new ArgumentException(); }
            bytes_readed = 0;
            if (!HeaderReaded && !HeaderWrited)
            {
                StreamHeaderState header_state = CheckHeader();
                if (header_state != StreamHeaderState.Ok) { throw new InvalidStreamHeaderException(header_state); }
            }
            int items_count = 0;
            count += offset;
            for (int i = offset; i < count; ++i)
            {
                int bytes = DoReadIndex(out indexes[i]);
                if (bytes == 0) break;
                bytes_readed += bytes;
                ++items_count;
            }
            return items_count;
        }

        /// <summary>Записывает значение индекса видеоданных в топок</summary>
        /// <param name="index">Ссылка на объект, содержащий данные индекса</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Недопустимое значение аргумента index</exception>
        /// <returns>Возвращает количество записанных в поток байтов</returns>
        protected virtual int DoWriteIndex(SingleStreamFrameIndex index)
        {
            if (index == null) throw new ArgumentNullException();
            byte[] data;
            int offset = 0;
            try
            {
                data = BitConverter.GetBytes(index.StreamId);
                Array.Copy(data, 0, buffer_write, 0, data.Length);
                offset += data.Length;

                data = BitConverter.GetBytes(index.TimeStamp);
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = BitConverter.GetBytes(index.FileOffset);
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                Write(buffer_write, 0, offset);
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw; }
            catch { }
            return offset;
        }

        /// <summary>Записывает значение индекса видеоданных в топок.
        /// Вызывает метод записи заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="index">Ссылка на объект, содержащий данные индекса</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Недопустимое значение аргумента index</exception>
        /// <returns>Возвращает количество записанных в поток байтов</returns>
        public int WriteIndex(SingleStreamFrameIndex index)
        {
            CheckDisposed();
            if (index == null) throw new ArgumentNullException();
            if (!HeaderReaded && !HeaderWrited) WriteHeader();
            return DoWriteIndex(index);
        }
        
        /// <summary>Записывает значения массива индексов видеоданных в топок
        /// Вызывает метод записи заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="indexes">Ссылка на массив, содержащий данные видеоиндексов</param>
        /// <param name="offset">Индекс первого элемента в массиве, начиная с которого данные видеоиндексов будут читаться из массива и записываться в поток</param>
        /// <param name="count">Количество видеоиндексов, которое нужно записать в потока</param>
        /// <param name="bytes_wrote">По окончании процесса записи этот параметр будет содержать количество записанных байтов в поток</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства параметра indexes значению null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение параметра offset выходит из границ диапозона допустимых значений</exception>
        /// <exception cref="System.ArgumentException">Недопустимое сочетание параметров offset и count</exception>
        /// <returns>Возвращает количество записанных в поток индексов</returns>
        public int Write(SingleStreamFrameIndex[] indexes, int offset, int count, ref int bytes_wrote)
        {
            CheckDisposed();
            if (indexes == null) { throw new ArgumentNullException(); }
            if ((offset < 0) || (offset > indexes.Length)) { throw new ArgumentOutOfRangeException("offset"); }
            if (offset + count > indexes.Length) { throw new ArgumentException(); }
            bytes_wrote = 0;
            if (!HeaderReaded && !HeaderWrited) { if (!WriteHeader()) return 0; }
            int items_count = 0;
            count += offset;
            for (int i = offset; i < count; ++i)
            {
                int bytes = DoWriteIndex(indexes[i]);
                if (bytes == 0) break;
                bytes_wrote += bytes;
                ++items_count;
            }
            return items_count;
        }

        /// <summary>Записывает данные заголовка индексов в поток</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <returns>Возвращает TRUE в случае успешной записи или FALSE, если запись не была произведена</returns>
        protected override bool DoWriteHeader()
        {
            buffer_write.Initialize();
            int offset = 0;

            try
            {
                byte[] data = _Signature;
                Array.Copy(data, 0, buffer_write, 0, data.Length);
                offset += data.Length;

                data = BitConverter.GetBytes(_Version);
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                long reserved = 0;
                data = BitConverter.GetBytes(reserved);
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                Write(buffer_write, 0, buffer_write.Length);
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw; }
            catch { return false; }
            return true;
        }

        /// <summary>Считывает данные заголовка индексов из потока</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <returns>Возвращает TRUE в случае успешного чтения или FALSE, если чтение завершилось ошибкой
        /// (заголовок имеет неверный формат)</returns>
        protected override bool DoReadHeader()
        {
            try
            {
                // Читаем данные заголовка
                int count = Read(buffer_read, 0, buffer_read.Length);
                // Проверяем количество прочитанных данных
                if (count < buffer_read.Length) return false;

                _Signature = new byte[4];
                Array.Copy(buffer_read, 0, _Signature, 0, 4);
                _Version = BitConverter.ToInt32(buffer_read, 4);
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw; }
            catch { return false; }
            return true;
        }

        /// <summary>По информации из заголовка потока проверяет совместимость с текущей версией формата видеоиндекса</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <returns>Возвращает результат проверки заголовка потока</returns>
        public override StreamHeaderState CheckHeader()
        {
            CheckDisposed();
            // Если заголовок не прочитан и не записан - пробуем его прочитать
            if (!HeaderReaded && !HeaderWrited) { if (!ReadHeader()) return StreamHeaderState.Invalid; }
            // Проверяем правильность заголовка
            if (Compare(_Signature, 0, Encoding.Default.GetBytes("FIDX"), 0, 4) != 0) { return StreamHeaderState.Invalid; }
            else if (_Version != 1) { return StreamHeaderState.NotSupported; }
            else { return StreamHeaderState.Ok; }
        }

        #endregion

        #region Stream Members

        /// <summary>Освобождает управляемые/неуправляемые ресурсы потока</summary>
        /// <param name="disposing">
        /// Значение true позволяет освободить управляемые и неуправляемые ресурсы;
        /// значение false позволяет освободить только неуправляемые ресурсы
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Нечего освобождать...
            base.Dispose(disposing);
        }

        /// <summary>Длина потока в количестве индексов</summary>
        /// <exception cref="System.NotSupportedException">
        /// Возникает в случае, если базовый поток не поддерживает возможность поиска
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Возникает в случае, если метод был вызван после закрытия потока
        /// </exception>
        public override long Length
        {
            get 
            {
                CheckDisposed();
                return base.Length / IndexSize;
            }
        }

        /// <summary>Получает или задает позицию чтения/записи видеоиндексов в потоке</summary>
        /// <exception cref="System.IO.IOException">Возникает в случае ошибки ввода/вывода</exception>
        /// <exception cref="System.NotSupportedException">Базовый поток не поддерживает операции поиска</exception>
        /// <exception cref="System.ObjectDisposedException">Возникает в случае, если метод был вызван после закрытия потока</exception>
        public override long Position
        {
            get
            {
                CheckDisposed();
                return base.Position / IndexSize;
            }
            set
            {
                CheckDisposed();
                base.Position = value * IndexSize;
            }
        }

        /// <summary>Задает позицию чтения/записи видеоиндексов в потоке</summary>
        /// <param name="offset">Смещение в количестве элементов SingleStreamFrameIndex относительно параметра origin</param>
        /// <param name="origin">Определяет точку отсчета, которая используется для получения новой позиции</param>
        /// <returns>Новая позиция в текущем потоке</returns>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает поиск</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();
            return base.Seek(offset * IndexSize, origin) / IndexSize;
        }

        /// <summary>Задает длину текущего потока</summary>
        /// <param name="value">Новая длина потока в количестве элементов типа SingleStreamFrameIndex</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает ни поиск, ни запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        public override void SetLength(long value)
        {
            CheckDisposed();
            base.SetLength(value * IndexSize);
        }

        #endregion

    }
}
