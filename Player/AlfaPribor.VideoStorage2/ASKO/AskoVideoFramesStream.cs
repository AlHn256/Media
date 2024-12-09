using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AlfaPribor.Streams2;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Поток stream, элементами которого являюдся видеокадры</summary>
    class AskoVideoFramesStream : StreamWithHeader2, IVideoFramesStream
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

        /// <summary>Последовательность байт, символизирующая границу кадров</summary>
        protected byte[] _Boundary;

        /// <summary>Разделитель между заголовком кадра и видеоданными кадра</summary>
        protected byte[] FrameHeaderBoundary;

        /// <summary>Разделитель между кадрами</summary>
        protected byte[] FramesBoundary;

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

        /// <summary>Метка начала поля с информацией о ширине кадра изображения видеоданных</summary>
        protected byte[] FrameWidthField;

        /// <summary>Метка начала поля с информацией о высоте кадра изображения видеоданных</summary>
        protected byte[] FrameHeightField;

        /// <summary>Метка начала поля с информацией о частоте следования кадров</summary>
        protected byte[] FpsField;

        protected byte[] ImageResStream0Field;
        protected byte[] ImageResStream1Field;
        protected byte[] ImageResStream2Field;
        protected byte[] ImageResStream3Field;
        protected byte[] ImageResStream4Field;
        protected byte[] ImageResStream5Field;

        protected byte[] RepackedField;

        /// <summary>Средний размер видеоданных кадра (байт)</summary>
        private int _AverageVideoDataSize;

        /// <summary>Ширина кадра изображения видеоданных</summary>
        private int _FrameWidth;

        /// <summary>Высота кадра изображения видеоданных</summary>
        private int _FrameHeight;

        /// <summary>Частота следования кадров</summary>
        private int _FPS;

        /// <summary>Смещение информации о содержимом потока в заголовке, байт</summary>
        private int _ContentTypeOffset;

        private string[] _Resolutions = new string[6] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

        #endregion

        #region Methods

        /// <summary>Инициализирует объект для записи/чтения видеокадров в поток</summary>
        /// <param name="stream">Указатель на поток, с которым будем работать</param>
        /// <exception cref="System.ArgumentNullException">Не задан целевой поток</exception>
        /// <remarks>Заголовок потока может иметь переменную длину, поэтому при создании объекта укажем максимальнно допустимую
        /// длину заголовка, которая скорректируется в процессе его чтения/записи</remarks>
        public AskoVideoFramesStream(Stream stream)
            : base(stream, 1024)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            _RecordStarted = DateTime.MinValue;
            _RecordFinished = DateTime.MinValue;
            _Signature = Encoding.Default.GetBytes("MJPG/");
            _Boundary = Encoding.Default.GetBytes("--myboundary");
            _Version = 10;
            _AverageVideoDataSize = 1024 * 15;
            HeaderBoundary = Encoding.Default.GetBytes("\r\n");
            FrameHeaderBoundary = HeaderBoundary;
            FramesBoundary = HeaderBoundary;
            FieldsBoundary = FrameHeaderBoundary;
            StreamIdField = Encoding.Default.GetBytes("StreamId: ");
            TimeStampField = Encoding.Default.GetBytes("TimeStamp: ");
            ContentTypeField = Encoding.Default.GetBytes("Content-Type: ");
            ContentLenField = Encoding.Default.GetBytes("Content-Length: ");
            RecordStartedField = Encoding.Default.GetBytes("Begin: ");
            RecordFinishedField = Encoding.Default.GetBytes("End: ");
            FpsField = Encoding.Default.GetBytes("FPS: ");
            FrameHeightField = Encoding.Default.GetBytes("Height: ");
            FrameWidthField = Encoding.Default.GetBytes("Width: ");
            ImageResStream0Field = Encoding.Default.GetBytes("Image-Resolution-Stream0: ");
            ImageResStream1Field = Encoding.Default.GetBytes("Image-Resolution-Stream1: ");
            ImageResStream2Field = Encoding.Default.GetBytes("Image-Resolution-Stream2: ");
            ImageResStream3Field = Encoding.Default.GetBytes("Image-Resolution-Stream3: ");
            ImageResStream4Field = Encoding.Default.GetBytes("Image-Resolution-Stream4: ");
            ImageResStream5Field = Encoding.Default.GetBytes("Image-Resolution-Stream5: ");
            RepackedField = Encoding.Default.GetBytes("Repacked: ");
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
            _FPS = 0;
            _FrameHeight = 0;
            _FrameWidth = 0;
            _ContentTypeOffset = 0;
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
            if (Position == Length)
            {
                // Достигнут конец потока
                return 0;
            }
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
                offset += bytes_readed;

                data = FieldsBoundary;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
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
                offset += bytes_readed;

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

                // Идентификатор потока видеоданных
                data = StreamIdField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

                data = FieldsBoundary;
                if (!ReadUntil(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                try
                {
                    result.CameraId = Int32.Parse(
                        Encoding.Default.GetString(buffer_read, offset, bytes_readed - data.Length)
                    );
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
                offset += bytes_readed;

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

                // Добавляем высоту/ширину кадра основываясь на данных заголовка потока
                if (result.ContentType.Width <= 0)
                {
                    result.ContentType.Width = GetXResolution(result.CameraId);
                }
                if (result.ContentType.Height <= 0)
                {
                    result.ContentType.Height = GetYResolution(result.CameraId);
                }

                // Длина бинарных видеокадра
                data = ContentLenField;
                if (!ReadAndCompare(data, buffer_read, offset, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Data header is invalid!");
                }
                offset += bytes_readed;

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
                offset += bytes_readed;

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
                }

                // Разделитель между кадрами
                data = FramesBoundary;
                if (!ReadAndCompare(data, buffer_read, 0, ref bytes_readed))
                {
                    throw new InvalidStreamDataException("Frame is invalid!");
                }
                offset += bytes_readed;
            }
            catch (IOException)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch (InvalidStreamDataException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidStreamDataException("Stream error!", e);
            }
            result.FrameData = video_data;
            frame = result;
            return offset;
        }

        int GetXResolution(int cam_id)
        {
            if (cam_id < 0) return _FrameWidth;
            if (cam_id >= _Resolutions.Length) return _FrameWidth;
            string value = _Resolutions[cam_id];
            if (string.IsNullOrEmpty(value)) return _FrameWidth;
            try
            {
                string[] strings = value.Split(new char[] { 'x' });
                return Convert.ToInt32(strings[0]);
            }
            catch { }
            return _FrameWidth;
        }

        int GetYResolution(int cam_id)
        {
            if (cam_id < 0) return _FrameHeight;
            if (cam_id >= _Resolutions.Length) return _FrameHeight;
            string value = _Resolutions[cam_id];
            if (string.IsNullOrEmpty(value)) return _FrameHeight;
            try
            {
                string[] strings = value.Split(new char[] { 'x' });
                return Convert.ToInt32(strings[1]);
            }
            catch { }
            return _FrameHeight;
        }

        /// <summary>Читает очередное значение видеокадра из потока.
        /// Вызывает метод чтения и проверки заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="frame">Ссылка на объект, принимающий данные</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает чтение</exception>
        /// <exception cref="System.ObjectDisposedException">Метод был вызван после закрытия потока</exception>
        /// <exception cref="AlfaPribor.VideoStorage.InvalidStreamHeaderException">Заголовок потока содержит ошибку</exception>
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

        public int ReadFrame(out VideoFrame frame, out string message)
        {
            frame = null;
            message = "";
            return 0;
        }
        
        /// <summary>Читает видеокадры из потока в массив.
        /// Вызывает метод чтения и проверки заголовка, если он не записывался и не был прочитан</summary>
        /// <param name="frames">Ссылка на массив, принимающий данные</param>
        /// <param name="offset">Индекс начального элемента массива frames, начиная с которого прочитанные из потока данные будут записываться в массив</param>
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
            if (frame == null) throw new ArgumentNullException();
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

                // Разделитель кадров
                data = FramesBoundary;
                Array.Copy(data, 0, buffer_write, 0, data.Length);
                offset += data.Length;

                Write(buffer_write, 0, data.Length);
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw; }
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
        /// <exception cref="System.ArgumentException">Недопустимое сочетание значений параметров offset и count/// </exception>
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

                data = Encoding.Default.GetBytes(" Multipart JPEG stream");
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Ширина видеокадра изображения
                data = FrameWidthField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(_FrameWidth.ToString());
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Высота видеокадра изображения
                data = FrameHeightField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(_FrameHeight.ToString());
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = FieldsBoundary;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                // Частота следования кадров
                data = FpsField;
                Array.Copy(data, 0, buffer_write, offset, data.Length);
                offset += data.Length;

                data = Encoding.Default.GetBytes(_FPS.ToString());
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

                Write(buffer_write, 0, offset);

                // Запоминаем текущую позицию курсора в заголовке
                long pos = Stream.Position;

                // Тип содержимого потока
                offset = 0;
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

                if (_ContentTypeOffset != 0)
                {
                    Stream.Seek(_ContentTypeOffset - pos, SeekOrigin.Current);
                }
                else
                {
                    _ContentTypeOffset = (int)Stream.Position;
                    HeaderSize = (int)Stream.Position + offset;
                }
                
                Write(buffer_write, 0, offset);
            }
            catch (IOException) { throw; }
            catch (NotSupportedException) { throw; }
            catch (ObjectDisposedException) { throw; }
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
            int header_len = 0;
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
                    if (!ReadAndCompare(stream, _Signature, tmp_buf, 0, ref bytes_readed)) return false;
                    header_len += bytes_readed;
                    // Версия структуры данных
                    stream.Read(tmp_buf, 0, 3);
                    string vesrion = Encoding.Default.GetString(tmp_buf, 0, 3);
                    int dot_index = vesrion.IndexOf(".");
                    string hi_part = vesrion.Substring(0, dot_index);
                    string low_part = vesrion.Substring(dot_index + 1);
                    _Version = Int32.Parse(hi_part) * 10 + Int32.Parse(low_part);
                    header_len += 3;
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed)) return false;
                    header_len += bytes_readed;
                    // Ширина изображения видеокадра
                    if (!ReadAndCompare(stream, FrameWidthField, tmp_buf, 0, ref bytes_readed)) return false;
                    header_len += bytes_readed;
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed)) return false;
                    _FrameWidth = Int32.Parse(Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length));
                    header_len += bytes_readed;
                    // Высота изображения видеокадра
                    if (!ReadAndCompare(stream, FrameHeightField, tmp_buf, 0, ref bytes_readed)) return false;
                    header_len += bytes_readed;
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed)) return false;
                    _FrameHeight = Int32.Parse(Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length));
                    header_len += bytes_readed;
                    // Частота следования кадров
                    if (!ReadAndCompare(stream, FpsField, tmp_buf, 0, ref bytes_readed)) return false;
                    header_len += bytes_readed;
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed)) return false;
                    _FPS = Int32.Parse(Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length));
                    header_len += bytes_readed;
                    // Разрешения
                    long position = stream.Position;
                    if (ReadAndCompare(stream, ImageResStream0Field, tmp_buf, 0, ref bytes_readed))
                    {
                        header_len += bytes_readed;
                        if (ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                        {
                            _Resolutions[0] = Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length).Trim();
                            header_len += bytes_readed;
                        }
                    }
                    else
                    {
                        stream.Position = position;
                    }
                    position = stream.Position;
                    if (ReadAndCompare(stream, ImageResStream1Field, tmp_buf, 0, ref bytes_readed))
                    {
                        header_len += bytes_readed;
                        if (ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                        {
                            _Resolutions[1] = Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length).Trim();
                            header_len += bytes_readed;
                        }
                    }
                    else
                    {
                        stream.Position = position;
                    }
                    position = stream.Position;
                    if (ReadAndCompare(stream, ImageResStream2Field, tmp_buf, 0, ref bytes_readed))
                    {
                        header_len += bytes_readed;
                        if (ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                        {
                            _Resolutions[2] = Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length).Trim();
                            header_len += bytes_readed;
                        }
                    }
                    else
                    {
                        stream.Position = position;
                    }
                    position = stream.Position;
                    if (ReadAndCompare(stream, ImageResStream3Field, tmp_buf, 0, ref bytes_readed))
                    {
                        header_len += bytes_readed;
                        if (ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                        {
                            _Resolutions[3] = Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length).Trim();
                            header_len += bytes_readed;
                        }
                    }
                    else
                    {
                        stream.Position = position;
                    }
                    position = stream.Position;
                    if (ReadAndCompare(stream, ImageResStream4Field, tmp_buf, 0, ref bytes_readed))
                    {
                        header_len += bytes_readed;
                        if (ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                        {
                            _Resolutions[4] = Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length).Trim();
                            header_len += bytes_readed;
                        }
                    }
                    else
                    {
                        stream.Position = position;
                    }
                    position = stream.Position;
                    if (ReadAndCompare(stream, ImageResStream5Field, tmp_buf, 0, ref bytes_readed))
                    {
                        header_len += bytes_readed;
                        if (ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                        {
                            _Resolutions[5] = Encoding.Default.GetString(tmp_buf, 0, bytes_readed - FieldsBoundary.Length).Trim();
                            header_len += bytes_readed;
                        }
                    }
                    else
                    {
                        stream.Position = position;
                    }

                    // Время начала записи
                    if (!ReadAndCompare(stream, RecordStartedField, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    header_len += bytes_readed;
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    header_len += bytes_readed;
                    if (!GetDateTime(tmp_buf, 0, ref _RecordStarted))
                    {
                        return false;
                    }

                    // Время окончания записи
                    if (!ReadAndCompare(stream, RecordFinishedField, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    header_len += bytes_readed;
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    header_len += bytes_readed;
                    if (!GetDateTime(tmp_buf, 0, ref _RecordFinished))
                    {
                        return false;
                    }

                    position = stream.Position;
                    if (ReadAndCompare(stream, RepackedField, tmp_buf, 0, ref bytes_readed))
                    {
                        header_len += bytes_readed;
                        if (ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                        {
                            header_len += bytes_readed;
                        }
                    }
                    else
                    {
                        stream.Position = position;
                    }

                    // Тип содержимого
                    if (!ReadUntil(stream, ContentTypeField, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    header_len += bytes_readed;
                    // Запоминаем расположение в теле заголовка
                    _ContentTypeOffset = header_len - ContentTypeField.Length;
                    if (!ReadUntil(stream, Encoding.Default.GetBytes("boundary="), tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    header_len += bytes_readed;
                    if (!ReadUntil(stream, FieldsBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    _Boundary = new byte[bytes_readed - FieldsBoundary.Length];
                    Array.Copy(tmp_buf, 0, _Boundary, 0, _Boundary.Length);
                    header_len += bytes_readed;

                    // Разделитель заголовка потока от данных, содержащихся в потоке
                    if (!ReadAndCompare(stream, HeaderBoundary, tmp_buf, 0, ref bytes_readed))
                    {
                        return false;
                    }
                    header_len += bytes_readed;

                    // Корректируем размер заголовка потока
                    this.HeaderSize = header_len;
                    // Перемещаем курсор к концу заголовка потока
                    Stream.Seek(header_len, SeekOrigin.Begin);
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
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>По информации из заголовка потока проверяет совместимость с текущей версией формата видеокадра</summary>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает перемещение курсора чтения/записи</exception>
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

            if (bytes == null) { throw new ArgumentNullException("bytes"); }
            if (offset < 0 || offset > bytes.Length) { throw new ArgumentOutOfRangeException("offset"); }
            if (offset + format_str_len > bytes.Length) { throw new ArgumentException(); }
            try
            {
                string date_str = Encoding.Default.GetString(bytes, offset, format_str_len);
                date = DateTime.Parse(date_str);
            }
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
            if (info == null || info.Count == 0)
            {
                throw new ArgumentNullException();
            }
            _FrameWidth = info[0].Width;
            _FrameHeight = info[0].Height;

            return WriteHeader();
        }

        /// <summary>Читает информацию о видеопотоках</summary>
        /// <param name="info">Ссылка список с информацией о видеопотоках</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает перемещение курсора чтения/записи</exception>
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
            long pos = this.Position;
            this.Position = 0;
            VideoFrame frame;
            List<int> ids = new List<int>();
            List<VideoContent> contents = new List<VideoContent>();
            try
            {
                try
                {
                    // Читаем последовательно по кадру
                    while (ReadFrame(out frame) != 0)
                    {
                        int index = ids.BinarySearch(frame.CameraId);
                        if (index < 0)
                        {
                            index = ~index;
                            ids.Insert(index, frame.CameraId);
                            frame.ContentType.Height = GetYResolution(frame.CameraId);
                            frame.ContentType.Width = GetXResolution(frame.CameraId);
                            contents.Insert(index, frame.ContentType);
                        }
                    }
                    for (int i = 0; i < ids.Count; i++)
                    {
                        info.Add(new VideoStreamInfo(ids[i], contents[i].ToString()));
                    }
                }
                finally
                {
                    // Восстанавливаем первоначальную позицию курсора
                    this.Position = pos;
                }
                return true;
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

            return false;
        }

        /// <summary>Читает информацию о видеопотоках</summary>
        /// <param name="info">Ссылка список с информацией о видеопотоках</param>
        /// <exception cref="System.IO.IOException">Ошибка ввода-вывода</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает перемещение курсора чтения/записи</exception>
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
            return DoReadStreamInfo(info);
        }

        /// <summary>Возвращает макисмальный размер буфера в байтах, 
        /// который потребуется для размещения данных о кадре (без учета бинарных видеоданных кадра)</summary>
        /// <exception cref="System.NullReferenceException">Ру заданы значения полей, участвующих в формировании заголовка потока</exception>
        private int MaxBufferSizeOfFrame()
        {
            int result = _Boundary.Length;
            result += StreamIdField.Length + Encoding.Default.GetByteCount(Int32.MaxValue.ToString()) + FieldsBoundary.Length;
            result += TimeStampField.Length + Encoding.Default.GetByteCount(Int32.MaxValue.ToString()) + FieldsBoundary.Length;
            result +=
                ContentTypeField.Length +
                Encoding.Default.GetByteCount("image/raw16; resolution=320x240; rotation=270") +
                FieldsBoundary.Length;
            result += ContentLenField.Length + Encoding.Default.GetByteCount(Int32.MaxValue.ToString()) + FieldsBoundary.Length;
            result += HeaderBoundary.Length;
            // Возвращаем с двойным запасом (на всякий случай)
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
                FramesBoundary = null;
                HeaderBoundary = null;
                FieldsBoundary = null;
                StreamIdField = null;
                TimeStampField = null;
                ContentTypeField = null;
                ContentLenField = null;
                RecordStartedField = null;
                RecordFinishedField = null;
                FrameHeightField = null;
                FrameWidthField = null;
                FpsField = null;
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
