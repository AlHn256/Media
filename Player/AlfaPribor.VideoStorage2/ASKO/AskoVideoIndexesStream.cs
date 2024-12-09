using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AlfaPribor.Streams2;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Обеспечивает чтение/запись индексов видеоданных в поток</summary>
    class AskoVideoIndexesStream : StreamWithHeader2, IVideoIndexesStream
    {
        byte[] _Boundary;

        /// <summary>Инициализирует объект для записи/чтения индексов видеоданных в поток</summary>
        /// <param name="stream">Указатель на поток, с которым будем работать</param>
        /// <exception cref="System.ArgumentNullException">Не задан целевой поток</exception>
        public AskoVideoIndexesStream(Stream stream) :
            base(stream, 0)
        {
            _Boundary = new byte[] { 0x0D, 0x0A };
            buffer_read = new byte[1024];
            buffer_write = new byte[1024];
            _Version = 1;
            _Signature = null;
        }

        /// <summary>Считывает данные заголовка потока</summary>
        /// <returns>Возвращает TRUE в случае успешного чтения или FALSE, если чтение завершилось ошибкой
        /// (заголовок имеет неверный формат)</returns>
        /// <remarks>Поток видеоиндексов в ранних системах не имеет заголовка</remarks>
        protected override bool DoReadHeader()
        {
            return true;
        }

        /// <summary>Записывает данные заголовка всего потока</summary>
        /// <returns>Возвращает TRUE в случае успешной записи или FALSE, если запись не была произведена</returns>
        /// <remarks>Поток видеоиндексов в ранних системах  не имеет заголовка</remarks>
        protected override bool DoWriteHeader()
        {
            return true;
        }

        /// <summary>Проверяет структуру заголовка потока</summary>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <returns>Возвращает результат проверки заголовка потока</returns>
        /// <remarks>Поток видеоиндексов в ранних системах  не имеет заголовка</remarks>
        public override StreamHeaderState CheckHeader()
        {
            CheckDisposed();
            return StreamHeaderState.Ok;
        }

        #region Члены IVideoIndexesStream

        /// <summary>
        /// Прочитать индексы из потока
        /// </summary>
        /// <param name="indexes">Буфер для размещения прочитанных индексов</param>
        /// <param name="offset">Смещение от начала буфера, по которому будут записаны прочитанные из потока индексы</param>
        /// <param name="count">Количество читаемых индексов, шт</param>
        /// <param name="bytes_readed">Количество прочитанных байт данных из потока</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства параметра indexes значению null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение параметра offset выходит из границ диапозона допустимых значений</exception>
        /// <exception cref="System.ArgumentException">Недопустимое сочетание параметров offset и count</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный индекс содержит ошибку</exception>
        /// <returns>Прочитанное количество индексов из потока, шт</returns>
        public int Read(SingleStreamFrameIndex[] indexes, int offset, int count, ref int bytes_readed)
        {
            CheckDisposed();
            if (indexes == null)
            {
                throw new ArgumentNullException();
            }
            if ((offset < 0) || (offset >= indexes.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (offset + count > indexes.Length)
            {
                throw new ArgumentException();
            }

            bytes_readed = 0;
            int items_count = 0;
            count += offset;
            for (int i = offset; i < count; ++i)
            {
                int bytes = ReadIndex(out indexes[i]);
                if (bytes == 0)
                {
                    break;
                }
                bytes_readed += bytes;
                ++items_count;
            }
            return items_count;
        }

        /// <summary>Читать очередное значение индекса из потока</summary>
        /// <param name="index">Ссылка на объект, принимающий данные</param>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <returns>
        /// Возвращает количество прочитанных из потока байтов, если индекс был прочитан успешно.
        /// В случае достижения конца потока возвращает 0
        /// </returns>
        public int ReadIndex(out SingleStreamFrameIndex index)
        {
            CheckDisposed();
            index = null;

            int read_bytes = buffer_read.Length;
            bool found_eos = ReadUntil(_Boundary, buffer_read, 0, ref read_bytes);
            if (read_bytes == 0 || !found_eos) return 0;

            string[] parts = Encoding.ASCII.GetString(buffer_read, 0, read_bytes).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            SingleStreamFrameIndex result;
            try
            {
                result = new SingleStreamFrameIndex(Int32.Parse(parts[1]));
                result.TimeStamp = Int32.Parse(parts[2]);
                result.FileOffset = Int64.Parse(parts[3]);
            }
            catch
            {
                throw new InvalidStreamDataException();
            }
            
            index = result;
            return read_bytes;
        }

        /// <summary>Записывает значения массива индексов видеоданных в топок</summary>
        /// <param name="indexes">Ссылка на массив, содержащий данные видеоиндексов</param>
        /// <param name="offset">Индекс первого элемента в массиве, начиная с которого данные видеоиндексов будут читаться из массива
        /// и записываться в поток
        /// </param>
        /// <param name="count">Количество видеоиндексов, которое нужно записать в потока</param>
        /// <param name="bytes_wrote"> По окончании процесса записи этот параметр будет содержать количество
        /// записанных байтов в поток
        /// </param>
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
            if (indexes == null)
            {
                throw new ArgumentNullException();
            }
            if ((offset < 0) || (offset > indexes.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (offset + count > indexes.Length)
            {
                throw new ArgumentException();
            }

            bytes_wrote = 0;
            int items_count = 0;
            count += offset;
            for (int i = offset; i < count; ++i)
            {
                int bytes = WriteIndex(indexes[i]);
                if (bytes == 0)
                {
                    break;
                }
                bytes_wrote += bytes;
                ++items_count;
            }
            return items_count;
        }

        /// <summary>Записать значение индекса видеоданных в топок</summary>
        /// <param name="index">Ссылка на объект, содержащий данные индекса</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Недопустимое значение аргумента index</exception>
        /// <returns>Возвращает количество записанных в поток байтов</returns>
        public int WriteIndex(SingleStreamFrameIndex index)
        {
            CheckDisposed();
            if (index == null)
                throw new ArgumentNullException();

            string data_str = string.Format("{0} {1} {2} {3}\r\n", 0, index.StreamId, index.TimeStamp, index.FileOffset);
            byte[] data;
            int offset = 0;
            try
            {
                data = Encoding.ASCII.GetBytes(data_str);
                offset += data.Length;
                Write(data, 0, offset);
            }
            catch (IOException)
            {
                throw;
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch { }
            return offset;
        }

        #endregion

        #region Члены IDisposable

        /// <summary>Освобождает управляемые/неуправляемые ресурсы потока</summary>
        /// <param name="disposing">
        /// Значение true позволяет освободить управляемые и неуправляемые ресурсы;
        /// значение false позволяет освободить только неуправляемые ресурсы
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (this.Disposed)
            {
                return;
            }
            if (disposing)
            {
                _Boundary = null;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
