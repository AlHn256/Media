using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using AlfaPribor.Streams2;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Поток stream, элементами которого являюдся видеокадры</summary>
    class VideoFramesStream : StreamWithHeader2, IVideoFramesStream
    {
        #region Fields

        /// <summary>Размер буфера данных, достаточного, чтобы разместить объект VideoFrame в таком виде,
        /// в котором он хранится в потоке (байт). 
        /// <para>Бинарные данные видеокадра объекта VideoFrame не учитываются</para>
        /// </summary>
        protected int FrameBufferSize;

        /// <summary>Время начала записи потока</summary>
        protected DateTime _RecordStarted;

        /// <summary>Время окончания записи потока</summary>
        protected DateTime _RecordFinished;

        /// <summary>Смещение (от конца заголовка потока) блока данных с информацией о видеопотоках (байт)</summary>
        protected long _StreamInfoOffset;

        /// <summary>Последовательность байт, символизирующая границу кадров</summary>
        protected byte[] _Boundary;

        /// <summary>Разделитель между заголовком кадра и видеоданными кадра</summary>
        protected byte[] FrameHeaderBoundary;

        /// <summary>Разделитель между заголовком потока и остальными данными, содержащимися в потоке</summary>
        protected byte[] HeaderBoundary;

        /// <summary>Метка, отделяющая поля заголовка друг от друга</summary>
        protected byte[] FieldsBoundary;

        /// <summary>Метка начала поля с информацией об идентификаторе видеопотока</summary>
        protected byte[] StreamIdField;

        /// <summary>Метка начала поля с информацией об метке времени кадра</summary>
        protected byte[] TimeStampField;

        /// <summary>Метка начала поля с информацией о содержимом кадра видеоданных</summary>
        protected byte[] ContentTypeField;

        /// <summary>Метка начала поля с информацией о длине видеоданных кадра</summary>
        protected byte[] ContentLenField;

        /// <summary>Метка начала поля с информацией о дате/времени начала записи</summary>
        protected byte[] RecordStartedField;

        /// <summary>Метка начала поля с информацией о дате/времени окончания записи</summary>
        protected byte[] RecordFinishedField;

        /// <summary>Метка начала поля с информацией о смещении блока данных, описывающего видеопотоки</summary>
        protected byte[] OffsetField;

        /// <summary>Средний размер видеоданных кадра (байт)</summary>
        private int _AverageVideoDataSize;

        #endregion

        #region Methods

        /// <summary>Инициализирует объект для записи/чтения видеокадров в поток</summary>
        /// <param name="stream">Указатель на поток, с которым будем работать</param>
        /// <exception cref="System.ArgumentNullException">Не задан целевой поток</exception>
        public VideoFramesStream(Stream stream) : base(stream, 184)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            _RecordStarted = DateTime.MinValue;
            _RecordFinished = DateTime.MinValue;
            _Signature = Encoding.Default.GetBytes("FRAMES/");
            _Boundary = Encoding.Default.GetBytes("--myboundary");
            _Version = 10;
            _StreamInfoOffset = 0L;
            _AverageVideoDataSize = 1024 * 15;
            HeaderBoundary = Encoding.Default.GetBytes("\r\n");
            FrameHeaderBoundary = HeaderBoundary;
            FieldsBoundary = FrameHeaderBoundary;
            StreamIdField = Encoding.Default.GetBytes("StreamId: ");
            TimeStampField = Encoding.Default.GetBytes("TimeStamp: ");
            ContentTypeField = Encoding.Default.GetBytes("Content-Type: ");
            ContentLenField = Encoding.Default.GetBytes("Content-Length: ");
            RecordStartedField = Encoding.Default.GetBytes("Begin: ");
            RecordFinishedField = Encoding.Default.GetBytes("End: ");
            OffsetField = Encoding.Default.GetBytes("Channels-Offset: ");
            FrameBufferSize = MaxBufferSizeOfFrame();
            if (FrameBufferSize < HeaderSize)
            {
                buffer_read = new byte[HeaderSize];
                buffer_write = new byte[HeaderSize];
            }
            else
            {
                buffer_read = new byte[FrameBufferSize];
                buffer_write = new byte[FrameBufferSize];
            }
        }

        /// <summary>Проверяет величину смещение блока с информацией о потоках 
        /// относительно текущей позиции курсора в потоке</summary>
        void CheckStreamInfoOffset()
        {
            long position = base.Position;
            if (_StreamInfoOffset < position) _StreamInfoOffset = position;
        }

        /// <summary>Читает очередное значение видеокадра из потока</summary>
        /// <param name="frame">Ссылка на объект, принимающий данные</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный видеокадр содержит ошибку</exception>
        /// <returns>Возвращает количество прочитанных из потока байтов, если видеокадр был прочитан успешно.
        /// В случае достижения конца потока возвращает 0</returns>
        protected virtual int DoReadFrame(out VideoFrame frame)
        {
            frame = null;
            VideoFrame result = new VideoFrame(); // Информация о видеокадре
            byte[] video_data = null;
            byte[] data;
            int offset = 0;
            int bytes_readed = 0;
            int content_len = 0; // Длина содержимого видеоданных в кадре
            try
            {
                // Считываем заголовок кадра
                // Маркер границы кадра
                data = _Boundary;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                data = FieldsBoundary;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                // Идентификатор потока видеоданных
                data = StreamIdField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    result.CameraId = Int32.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length));
                }
                catch
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                // Метка времени кадра
                data = TimeStampField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    result.TimeStamp = Int32.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length));
                }
                catch
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                // Тип содержимого
                data = ContentTypeField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    result.ContentType.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length));
                }
                catch
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                // Длина бинарных видеокадра
                data = ContentLenField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    content_len = Int32.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length));
                }
                catch
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                // Читаем разделитель заголовка и данных
                data = FrameHeaderBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                // Читаем видеоданные кадра
                if (content_len >= 0)
                {
                    video_data = new byte[content_len];
                    bytes_readed = Read(video_data, 0, content_len);
                    if (content_len > bytes_readed)
                    {
                        throw new InvalidStreamDataException("Content length in data header not equal reading bytes count!");
                    }
                    offset += bytes_readed;

                    //if (video_data != null)
                    //{
                    //    using (MemoryStream ms = new MemoryStream(video_data))
                    //    {
                            
                    //        Bitmap bmp = new Bitmap(ms);
                    //        //bmp.Save(stream, ImageFormat.png);
                    //    }
                    //}
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch
            {
                throw new InvalidStreamDataException();
            }
            result.FrameData = video_data;
            frame = result;
            return offset;
        }

        /// <summary>Читает очередное значение видеокадра из потока</summary>
        /// <param name="frame">Ссылка на объект, принимающий данные</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный видеокадр содержит ошибку</exception>
        /// <returns>Возвращает количество прочитанных из потока байтов, если видеокадр был прочитан успешно.
        /// В случае достижения конца потока возвращает 0</returns>
        protected virtual int DoReadFrame(out VideoFrame frame, out string message)
        {
            frame = null;
            VideoFrame result = new VideoFrame(); // Информация о видеокадре
            byte[] video_data = null;
            byte[] data;
            int offset = 0;
            int bytes_readed = 0;
            int content_len = 0; // Длина содержимого видеоданных в кадре
            try
            {
                // Считываем заголовок кадра
                // Маркер границы кадра
                data = _Boundary;
                string err = "";
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed, ref err))
                {
                    message = "ReadAndCompare, offset=" + offset.ToString() + ", _Stream.Position=" + _Stream.Position.ToString() + ", bytes_readed:" + bytes_readed.ToString() + "err:" + err;
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                message = "2";
                data = FieldsBoundary;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                // Идентификатор потока видеоданных
                message = "3";
                data = StreamIdField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                message = "4";
                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    result.CameraId = Int32.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length));
                }
                catch
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                // Метка времени кадра
                message = "5";
                data = TimeStampField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                message = "6";
                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    result.TimeStamp = Int32.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length));
                }
                catch
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                // Тип содержимого
                message = "7";
                data = ContentTypeField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                message = "8";
                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    result.ContentType.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length));
                }
                catch
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                // Длина бинарных видеокадра
                message = "9";
                data = ContentLenField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                message = "10";
                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    content_len = Int32.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length));
                }
                catch
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                // Читаем разделитель заголовка и данных
                message = "11";
                data = FrameHeaderBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += data.Length;

                // Читаем видеоданные кадра
                message = "12";
                if (content_len >= 0)
                {
                    video_data = new byte[content_len];
                    bytes_readed = Read(video_data, 0, content_len);
                    if (content_len > bytes_readed)
                    {
                        throw new InvalidStreamDataException("Content length in data header not equal reading bytes count!");
                    }
                    offset += bytes_readed;
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch
            {
                throw new InvalidStreamDataException();
            }
            result.FrameData = video_data;
            frame = result;

            return offset;
        }

        /// <summary>Читает очередное значение видеокадра из потока.
        /// Вызывает метод чтения и проверки заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="frame">Ссылка на объект, принимающий данные</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamHeaderException">Заголовок потока содержит ошибку/// </exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный видеокадр содержит ошибку</exception>
        /// <returns>Возвращает количество прочитанных из потока байтов, если видеокадр был прочитан успешно.
        /// В случае достижения конца потока возвращает 0</returns>
        public int ReadFrame(out VideoFrame frame)
        {
            CheckDisposed();
            if (!HeaderReaded && !HeaderWrited)
            {
                StreamHeaderState header_state = CheckHeader();
                if (header_state != StreamHeaderState.Ok)
                {
                    throw new InvalidStreamHeaderException(header_state);
                }
            }
            return DoReadFrame(out frame);
        }

        /// <summary>Читает очередное значение видеокадра из потока.
        /// Вызывает метод чтения и проверки заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="frame">Ссылка на объект, принимающий данные</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamHeaderException">Заголовок потока содержит ошибку/// </exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный видеокадр содержит ошибку</exception>
        /// <returns>Возвращает количество прочитанных из потока байтов, если видеокадр был прочитан успешно.
        /// В случае достижения конца потока возвращает 0</returns>
        public int ReadFrame(out VideoFrame frame, out string message)
        {
            CheckDisposed();
            if (!HeaderReaded && !HeaderWrited)
            {
                StreamHeaderState header_state = CheckHeader();
                if (header_state != StreamHeaderState.Ok)
                {
                    throw new InvalidStreamHeaderException(header_state);
                }
            }
            return DoReadFrame(out frame, out message);
        }

        /// <summary>Читает видеокадры из потока в массив.
        /// Вызывает метод чтения и проверки заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="frames">Ссылка на массив, принимающий данные</param>
        /// <param name="offset">Индекс начального элемента массива frames, начиная с которого прочитанные из потока данные будутзаписываться в массив</param>
        /// <param name="count">Количество видеокадров, которые требуется прочитать из потока</param>
        /// <param name="bytes_readed">По завершении метода этот параметр будет содержать число байтов, прочитанных из потока</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Недопустимое значение аргумента frames</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение параметра offset</exception>
        /// <exception cref="System.ArgumentException">недопустимое сочетание значений параметров offset и count</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamHeaderException">Заголовок потока содержит ошибку</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamDataException">Прочитанный видеокадр содержит ошибку</exception>
        /// <returns>Возвращает количество прочитанных видеокадров из потока</returns>
        public int Read(VideoFrame[] frames, int offset, int count, ref int bytes_readed)
        {
            CheckDisposed();
            if (frames == null)
            {
                throw new ArgumentNullException();
            }
            if ((offset < 0) || (offset > frames.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (offset + count > frames.Length)
            {
                throw new ArgumentException();
            }
            bytes_readed = 0;
            if (!HeaderReaded && !HeaderWrited)
            {
                StreamHeaderState header_state = CheckHeader();
                if (header_state != StreamHeaderState.Ok)
                {
                    throw new InvalidStreamHeaderException(header_state);
                }
            }
            int items_count = 0;
            count += offset;
            for (int i = offset; i < count; ++i)
            {
                int bytes_count = DoReadFrame(out frames[i]);
                if (bytes_count == 0)
                {
                    break;
                }
                bytes_readed += bytes_count;
                ++items_count;
            }
            return items_count;
        }

        /// <summary>Записывает видеокадр в топок</summary>
        /// <param name="frame">Ссылка на объект, содержащий данные о видеокадре</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Недопустимое значение аргумента frame</exception>
        /// <returns>Возвращает количество записанных в поток байтов</returns>
        protected virtual int DoWriteFrame(VideoFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException();

            byte[] data;
            int offset = 0;
            // Запоминаем смещение пишущего курсора от конца заголовка потока
            long CurrentOffset = base.Position;
            try
            {
                // Верхняя граница кадра
                data = _Boundary;
                Array.Copy(data, 0, buffer_write, 0, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Идентификатор видеопотока
                data = StreamIdField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(frame.CameraId.ToString());
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Метка времени кадра
                data = TimeStampField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(frame.TimeStamp.ToString());
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Тип содержимого видеоданных
                data = ContentTypeField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(frame.ContentType.ToString());
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Длина содержимого видеоданных
                data = ContentLenField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                int frame_data_len = 0;
                if (frame.FrameData != null)
                {
                    frame_data_len = frame.FrameData.Length;
                }
                data = Encoding.Default.GetBytes(frame_data_len.ToString());
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Разделитель заголовка кадра и видеоданных
                data = FrameHeaderBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Записываем заголовок кадра
                Write(buffer_write, 0, offset);
                if (frame_data_len != 0)
                {
                    // Записываем бинарные данные видеокадра
                    Write(frame.FrameData, 0, frame_data_len);
                    offset += frame_data_len;
                }
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

        /// <summary>Записывает видеокадра в поток.
        /// Вызывает метод записи заголовка, если он не записывался</summary>
        /// <param name="frame">Ссылка на объект, принимающий данные</param>
        /// <returns>Возвращает 1, если данные были прочитаны успешно. В случае достижения конца потока возвращает 0</returns>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Недопустимое значение аргумента frame</exception>
        /// <returns>Возвращает количество записанных в поток байтов в случае успеха, в противном случае возвращает 0</returns>
        public int WriteFrame(VideoFrame frame)
        {
            CheckDisposed();
            if (frame == null)
                throw new ArgumentNullException();

            if (!HeaderReaded && !HeaderWrited)
            {
                if (!WriteHeader()) return 0;
            }
            int result = DoWriteFrame(frame);
            CheckStreamInfoOffset();
            return result;
        }

        /// <summary>Записывает значения массива видеокадров в топок.
        /// Вызывает метод записи заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="frames">Ссылка на массив, содержащий данные индексов</param>
        /// <param name="offset">Индекс первого элемента в массиве frames, с которого следует начинать запись элементов в поток</param>
        /// <param name="count">Количество элементов, которые требуется записать в поток</param>
        /// <param name="bytes_wrote">По окончании процесса записи этот параметр будет содержать количество записанных байтов в поток</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Возникает в случае равенства параметра frames значению null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение параметра offset</exception>
        /// <exception cref="System.ArgumentException">Недопустимое сочетание значений параметров offset и count</exception>
        /// <returns>Возвращает количество записанных в поток видеокадров</returns>
        public int Write(VideoFrame[] frames, int offset, int count, ref int bytes_wrote)
        {
            CheckDisposed();
            if (frames == null)
            {
                throw new ArgumentNullException();
            }
            if ((offset < 0) || (offset > frames.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (offset + count > frames.Length)
            {
                throw new ArgumentException();
            }
            bytes_wrote = 0;
            if (!HeaderReaded && !HeaderWrited)
            {
                if (!WriteHeader()) return 0;
            }
            int items_count = 0;
            count += offset;
            for (int i = offset; i < count; ++i)
            {
                int bytes = DoWriteFrame(frames[i]);
                if (bytes <= 0) break;
                bytes_wrote += bytes;
                ++items_count;
                CheckStreamInfoOffset();
            }
            return items_count;
        }

        /// <summary>Записывает данные заголовка потока</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <returns>Возвращает TRUE в случае успешной записи или FALSE, если запись не была произведена</returns>
        protected override bool DoWriteHeader()
        {
            byte[] data;
            int offset = 0;

            try
            {
                // Сигнатура заголовка
                data = _Signature;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                int hi_part = _Version / 10;
                int low_part = _Version % 10;
                data = Encoding.Default.GetBytes(hi_part.ToString());
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(".");
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(low_part.ToString());
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(" Multipart stream");
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Дата и время начала записи
                data = RecordStartedField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = GetDateTimeStr(_RecordStarted);
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Дата и время окончания записи
                data = RecordFinishedField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = GetDateTimeStr(_RecordFinished);
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Смещение информации о каналах
                data = OffsetField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes((_StreamInfoOffset + HeaderSize).ToString("0000000000000000"));
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Тип содержимого потока
                data = ContentTypeField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes("multipart/x-mixed-replace; boundary=");
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = _Boundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Разделитель заголовка потока от остальных данных
                data = HeaderBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                Write(buffer_write, 0, offset);
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw;}
            catch { return false; }
            return true;
        }

        /// <summary>Считывает данные заголовка потока</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <returns>Возвращает TRUE в случае успешного чтения или FALSE, если чтение завершилось ошибкой
        /// (заголовок имеет неверный формат)</returns>
        protected override bool DoReadHeader()
        {
            int bytes_readed = 0;
            // Создаем внутренний буфер для разбора заголовка потока
            byte[] tmp_buf = new byte[HeaderSize];
            try
            {
                // Читаем данные заголовка
                int count = Read(buffer_read, 0, HeaderSize);
                // Проверяем количество прочитанных данных
                if (count < HeaderSize) return false;

                using (MemoryStream stream = new MemoryStream(buffer_read))
                {
                    // Сигнатура данных
                    if (!ReadAndCompare(stream, _Signature, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }

                    // Версия структуры данных
                    stream.Read(tmp_buf, 0, 3);
                    string vesrion = Encoding.Default.GetString(tmp_buf, 0, 3);
                    int dot_index = vesrion.IndexOf(".");
                    string hi_part = vesrion.Substring(0, dot_index);
                    string low_part = vesrion.Substring(dot_index + 1);
                    _Version = Int32.Parse(hi_part) * 10 + Int32.Parse(low_part);

                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }

                    // Время начала записи
                    if (!ReadAndCompare(stream, RecordStartedField, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    if (!GetDateTime(tmp_buf, 0, ref _RecordStarted))
                    {
                        return false;
                    }

                    // Время окончания записи
                    if (!ReadAndCompare(stream, RecordFinishedField, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    if (!GetDateTime(tmp_buf, 0, ref _RecordFinished))
                    {
                        return false;
                    }

                    // Смещение (относительно начала потока) блока данных, описывающих видеопотоки
                    if (!ReadAndCompare(stream, OffsetField, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    _StreamInfoOffset = Int64.Parse(Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length));
                    _StreamInfoOffset -= HeaderSize;

                    // Тип содержимого
                    if (!ReadAndCompare(stream, ContentTypeField, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    if (!ReadUntil(stream, Encoding.Default.GetBytes("boundary="), tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    _Boundary = new byte[bytes_readed - FieldsBoundary.Length];
                    Array.Copy(tmp_buf, 0, _Boundary, 0, _Boundary.Length);

                    // Разделитель заголовка потока от данных, содержащихся в потоке
                    if (!ReadAndCompare(stream, HeaderBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                }
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw; }
            catch { return false; }
            return true;
        }

        /// <summary>По информации из заголовка потока проверяет совместимость с текущей версией формата видеокадра</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <returns>Возвращает результат проверки заголовка потока</returns>
        public override StreamHeaderState CheckHeader()
        {
            CheckDisposed();
            // Если заголовок не прочитан и не записан - пробуем его прочитать
            if (!HeaderReaded && !HeaderWrited)
            {
                if (!ReadHeader())
                {
                    return StreamHeaderState.Invalid;
                }
            }
            // Проверяем номер версии структуры данных, содержащихся в потоке
            if (_Version != 10)
            {
                return StreamHeaderState.NotSupported;
            }
            else
            {
                return StreamHeaderState.Ok;
            }
        }

        /// <summary>Преобразует дату/время в строковое представление</summary>
        /// <param name="date">Дата/время для кодировки в строковое представление</param>
        /// <exception cref="System.Text.EncoderFallbackException">Использована альтернативная кодировка </exception>
        /// <returns>Возвращает массив байт со строковым представлением заданной даты/времени</returns>
        protected byte[] GetDateTimeStr(DateTime date)
        {
            string date_str = date.ToString("dd.MM.yyyy HH:mm:ss");
            if (DateTimeFormatInfo.CurrentInfo.TimeSeparator != ":")
            {
                date_str.Replace(DateTimeFormatInfo.CurrentInfo.TimeSeparator, ":");
            }
            return Encoding.Default.GetBytes(date_str);
        }

        /// <summary>Преобразует строковое представление даты/времени в переменную типа DateTime</summary>
        /// <param name="bytes">Массив байт, содержащий строковое представление даты/времени</param>
        /// <param name="offset">Индекс элемента массива, с начиная с которого хранятся данные о дате/времени</param>
        /// <param name="date">Полученное в результате преобразования значение даты/времени</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на нассив байт, содержащий символьное представление даты/времени</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение параметра offset лежит за границами диапозона допустимых значений</exception>
        /// <exception cref="System.ArgumentException">Исходный массив байт содержит недостаточно данных</exception>
        /// <returns>В случае успеха возвращает TRUE, иначе - FALSE</returns>
        protected bool GetDateTime(byte[] bytes, int offset, ref DateTime date)
        {
            const int format_str_len = 19;

            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (offset < 0 || offset > bytes.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (offset + format_str_len > bytes.Length)
            {
                throw new ArgumentException();
            }

            try
            {
                string date_str = Encoding.Default.GetString(bytes, offset, format_str_len);
                int day = Int32.Parse(date_str.Substring(0, 2));
                int month = Int32.Parse(date_str.Substring(3, 2));
                int year = Int32.Parse(date_str.Substring(6, 4));
                int hour = Int32.Parse(date_str.Substring(11, 2));
                int min = Int32.Parse(date_str.Substring(14, 2));
                int sec = Int32.Parse(date_str.Substring(17, 2));
                date = new DateTime(year, month, day, hour, min, sec);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>Рассчитывает размер блока данных информации о потоках (байт)</summary>
        /// <param name="info">Ссылка на информацию о потоках</param>
        /// <returns>Размер блока данных (байт)</returns>
        protected virtual int GetStreamInfoBlockSize(IList<VideoStreamInfo> info)
        {
            if (info == null)
            {
                throw new ArgumentNullException();
            }
            int result = 0;
            // Длина заголовка блока данных
            result += 
                _Boundary.Length + FieldsBoundary.Length + 
                ContentTypeField.Length + 10 /* text/lpain */ + FieldsBoundary.Length +
                HeaderBoundary.Length;
            // Длина самих данных с информацией о видеопотоках
            foreach (VideoStreamInfo stream_info in info)
            {
                result += StreamIdField.Length + stream_info.Id.ToString().Length + FieldsBoundary.Length +  ContentTypeField.Length + 
                    stream_info.ToString().Length + FieldsBoundary.Length;
            }
            // Разделитель
            result += FieldsBoundary.Length;
            
            return result;
        }

        /// <summary>Записывает информацию о видеопотоках</summary>
        /// <param name="info">Ссылка список с информацией о видеопотоках</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Отсутствует ссылка на список с информацией о потоках</exception>
        /// <returns>Возвращает TRUE в случае успешной записи данных в поток, в противном случае возвращает FALSE</returns>
        protected virtual bool DoWriteStreamInfo(IList<VideoStreamInfo> info)
        {
            if (info == null)
            {
                throw new ArgumentNullException();
            }
            int offset = 0;
            byte[] date;
            // Проверяем размер буфера и в cлучае необходимости - увеличиваем его
            int block_size = GetStreamInfoBlockSize(info);
            if (buffer_write.Length < block_size)
            {
                buffer_write = new byte[block_size];
            }
            try
            {
                date = _Boundary;
                Array.Copy(date, 0, buffer_write, offset, date.Length);
                offset += date.Length;

                date = FieldsBoundary;
                Array.Copy(date, 0, buffer_write, offset, date.Length);
                offset += date.Length;

                date = ContentTypeField;
                Array.Copy(date, 0, buffer_write, offset, date.Length);
                offset += date.Length;

                date = Encoding.Default.GetBytes("text/plain");
                Array.Copy(date, 0, buffer_write, offset, date.Length);
                offset += date.Length;

                date = FieldsBoundary;
                Array.Copy(date, 0, buffer_write, offset, date.Length);
                offset += date.Length;

                date = HeaderBoundary;
                Array.Copy(date, 0, buffer_write, offset, date.Length);
                offset += date.Length;

                foreach (VideoStreamInfo stream_info in info)
                {
                    date = StreamIdField;
                    Array.Copy(date, 0, buffer_write, offset, date.Length);
                    offset += date.Length;

                    date = Encoding.Default.GetBytes(stream_info.Id.ToString());
                    Array.Copy(date, 0, buffer_write, offset, date.Length);
                    offset += date.Length;

                    date = FieldsBoundary;
                    Array.Copy(date, 0, buffer_write, offset, date.Length);
                    offset += date.Length;

                    date = ContentTypeField;
                    Array.Copy(date, 0, buffer_write, offset, date.Length);
                    offset += date.Length;

                    date = Encoding.Default.GetBytes(stream_info.ToString());
                    Array.Copy(date, 0, buffer_write, offset, date.Length);
                    offset += date.Length;

                    date = FieldsBoundary;
                    Array.Copy(date, 0, buffer_write, offset, date.Length);
                    offset += date.Length;
                }
                date = FieldsBoundary;
                Array.Copy(date, 0, buffer_write, offset, date.Length);
                offset += date.Length;

                Write(buffer_write, 0, offset);
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw; }
            catch { return false; }
            return true;
        }

        /// <summary>Записывает информацию о видеопотоках в конец потока и перезаписывает заголовок потока</summary>
        /// <param name="info">Ссылка список с информацией о видеопотоках</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Отсутствует ссылка на список с информацией о потоках</exception>
        public bool WriteStreamInfo(IList<VideoStreamInfo> info)
        {
            CheckDisposed();
            if (!WriteHeader())
            {
                return false;
            }

            long position = base.Position;
            if (position != _StreamInfoOffset)
            {
                base.Seek(_StreamInfoOffset, SeekOrigin.Begin);
            }
            bool result;
            try
            {
                result = DoWriteStreamInfo(info);
            }
            finally
            {
                base.Position = position;
            }
            return result;
        }

        /// <summary>Читает информацию о видеопотоках</summary>
        /// <param name="info">Ссылка список с информацией о видеопотоках</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Отсутствует ссылка на список с информацией о потоках</exception>
        /// <returns>Возвращает TRUE, если чтение завершилось успешно и список с информацией о видеопотоках
        /// сформирован; в противном случае возвращает FALSE</returns>
        protected virtual bool DoReadStreamInfo(IList<VideoStreamInfo> info)
        {
            if (info == null)
            {
                throw new ArgumentNullException();
            }
            long len = Length;
            // Проверяем размер буфера и в cлучае необходимости - увеличиваем его
            int block_size = GetStreamInfoBlockSize(info);
            if (buffer_read.Length < block_size)
            {
                buffer_read = new byte[block_size];
            }
            int bytes_readed = 0;
            int offset = 0;
            try
            {
                if (!ReadAndCompare(_Boundary, buffer_read, offset, ref bytes_readed))
                {
                    return false;
                }
                if (!ReadAndCompare(FieldsBoundary, buffer_read, offset, ref bytes_readed))
                {
                    return false;
                }
                if (!ReadAndCompare(ContentTypeField, buffer_read, offset, ref bytes_readed))
                {
                    return false;
                }
                if (!ReadAndCompare(Encoding.Default.GetBytes("text/plain"), buffer_read, offset, ref bytes_readed))
                {
                    return false;
                }
                if (!ReadAndCompare(FieldsBoundary, buffer_read, offset, ref bytes_readed))
                {
                    return false;
                }
                if (!ReadAndCompare(HeaderBoundary, buffer_read, offset, ref bytes_readed))
                {
                    return false;
                }
                while (true)
                {
                    offset = 0;
                    if (!ReadAndCompare(StreamIdField, buffer_read, offset, ref bytes_readed))
                    {
                        break;
                    }
                    if (!ReadUntil(FieldsBoundary, buffer_read, offset, ref bytes_readed))
                    {
                        return false;
                    }
                    // Идентификатор видеопотока
                    int id = Int32.Parse(Encoding.Default.GetString(buffer_read, offset, bytes_readed - FieldsBoundary.Length));
                    if (!ReadAndCompare(ContentTypeField, buffer_read, offset, ref bytes_readed))
                    {
                        return false;
                    }
                    if (!ReadUntil(FieldsBoundary, buffer_read, offset, ref bytes_readed))
                    {
                        return false;
                    }
                    // Идентификатор видеопотока
                    String content  = Encoding.Default.GetString(buffer_read, offset, bytes_readed - FieldsBoundary.Length);
                    // Добавляем информацию о видеопотоке в список
                    info.Add(new VideoStreamInfo(id, content));
                }
                return true;
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw; }
            catch { }
            return false;
        }

        /// <summary>Читает информацию о видеопотоках</summary>
        /// <param name="info">Ссылка список с информацией о видеопотоках</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="System.ArgumentNullException">Отсутствует ссылка на список с информацией о потоках</exception>
        /// <returns>Возвращает TRUE, если чтение завершилось успешно и список с информацией о видеопотоках
        /// сформирован; в противном случае возвращает FALSE</returns>
        public bool ReadStreamInfo(IList<VideoStreamInfo> info)
        {
            CheckDisposed();
            if (!HeaderReaded && !HeaderWrited)
            {
                if (CheckHeader() != StreamHeaderState.Ok)
                {
                    return false;
                }
            }
            long position = base.Position;
            if (base.Position != _StreamInfoOffset)
            {
                base.Seek(_StreamInfoOffset, SeekOrigin.Begin);
            }
            try
            {
                return DoReadStreamInfo(info);
            }
            finally
            {
                base.Position = position;
            }
        }

        /// <summary>Возвращает макисмальный размер буфера в байтах, 
        /// который потребуется для размещения данных о кадре (без учета бинарных видеоданных кадра)
        /// </summary>
        /// <exception cref="System.NullReferenceException">Ру заданы значения полей, участвующих в формировании заголовка потока</exception>
        int MaxBufferSizeOfFrame()
        {
            int result = _Boundary.Length;
            result += StreamIdField.Length + Encoding.Default.GetByteCount(Int32.MaxValue.ToString()) + FieldsBoundary.Length;
            result += TimeStampField.Length + Encoding.Default.GetByteCount(Int32.MaxValue.ToString()) + FieldsBoundary.Length;
            result += ContentTypeField.Length +  Encoding.Default.GetByteCount("image/raw16; resolution=320x240; rotation=270") + 
                FieldsBoundary.Length;
            result += ContentLenField.Length + Encoding.Default.GetByteCount(Int32.MaxValue.ToString()) + FieldsBoundary.Length;
            result += HeaderBoundary.Length;
            // Возвращаем двойной с двойным запасом (на всякий случай)
            return result * 2;
        }

        #endregion

        #region Properties

        /// <summary>Время начала записи потока</summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Метод был вызван после закрытия потока
        /// </exception>
        public DateTime RecordStarted
        {
            get 
            {
                CheckDisposed();
                return _RecordStarted;
            }
            set
            {
                CheckDisposed();
                DateTime old_value = _RecordStarted;
                try
                {
                    _RecordStarted = value;
                    WriteHeader();
                }
                catch
                {
                    // Если запись завершилась ошибкой - восстанавливаем предыдущее значение свойства
                    _RecordStarted = old_value;
                    throw;
                }
            }
        }

        /// <summary>Время окончания записи потока</summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Метод был вызван после закрытия потока
        /// </exception>
        public DateTime RecordFinished
        {
            get
            {
                CheckDisposed();
                return _RecordFinished;
            }
            set 
            {
                CheckDisposed();
                DateTime old_value = _RecordFinished;
                try
                {
                    _RecordFinished = value;
                    WriteHeader();
                }
                catch
                {
                    // Если запись завершилась ошибкой - восстанавливаем предыдущее значение свойства
                    _RecordFinished = old_value;
                    throw;
                }
            }
        }

        /// <summary>Последовательность байт, символизирующая границу кадров</summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Метод был вызван после закрытия потока
        /// </exception>
        public byte[] Boundary
        {
            get
            {
                CheckDisposed();
                return _Boundary;
            }
        }

        /// <summary>Средний размер видеоданных кадра (байт)</summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Метод был вызван после закрытия потока
        /// </exception>
        public int AverageFrameSize
        {
            get 
            {
                CheckDisposed();
                return _AverageVideoDataSize; 
            }
            set 
            {
                CheckDisposed();
                _AverageVideoDataSize = value;
            }
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
            if (disposing)
            {
                _Boundary = null;
                FrameHeaderBoundary = null;
                HeaderBoundary = null;
                FieldsBoundary = null;
                StreamIdField = null;
                TimeStampField = null;
                ContentTypeField = null;
                ContentLenField = null;
                RecordStartedField = null;
                RecordFinishedField = null;
                OffsetField = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>Задает длину текущего потока</summary>
        /// <param name="value">
        /// Новая длина потока в количестве элементов типа SingleStreamFrameIndex
        /// </param>
        /// <exception cref="System.IO.IOException">
        /// Ошибка ввода-вывода
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Поток не поддерживает ни поиск, ни запись
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Метод был вызван после закрытия потока
        /// </exception>
        public override void SetLength(long value)
        {
            CheckDisposed();
            // Вычисляем размер потока опираясь на средний размер видеоданных кадра, т.к.
            // считаем, что самих кадров еще не имеем
            base.SetLength(value * (FrameBufferSize + _AverageVideoDataSize));
        }

        #endregion
    }
}
