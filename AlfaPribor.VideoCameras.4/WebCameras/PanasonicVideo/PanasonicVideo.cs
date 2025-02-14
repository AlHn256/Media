using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Получение видеопотока MJPEG от камеры Panasonic</summary>
    public class PanasonicVideo : IDisposable
    {

        enum ParserStage { StreamHeader, SearchingBoundary, FrameHeader, FrameData };

        #region Variables

        bool _Active = false;
        PanasonicVideoStatus _Status = PanasonicVideoStatus.Inactive;
        
        string _Host = "192.168.221.121";
        int _Port = 80;
        string _UserName = "admin";
        string _Password = "root_1234";
        string _Resolution = "640x480";
        string _Camera = string.Empty;
        string _Compression = string.Empty;
        string _Fps = string.Empty;
        int _ReceiveTimeout = 5000;
        int _SendTimeout = 5000;

        object _Lock = new object();
        TcpClient _Client = null;
        Thread _ClientThread = null;

        #endregion

        /// <summary>Событие "Изменился статус клиента Panasonic видео"</summary>
        public event EventHandler<PanasonicVideoStatusEventArgs> OnStatus;
        /// <summary>Событие "Получен кадр от камеры Panasonic"</summary>
        public event EventHandler<PanasonicVideoFrameEventArgs> OnFrame;

        /// <summary>Конструктор</summary>
        public PanasonicVideo() { }

        /// <summary>Конструктор</summary>
        /// <param name="host">Адрес</param>
        /// <param name="user_name">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="resolution">Разрешение</param>
        /// <param name="fps">Частота кадров</param>
        public PanasonicVideo(string host, string user_name, string password, string resolution, string fps)
        {
            _Host = host;
            _UserName = user_name;
            _Password = password;
            _Resolution = resolution;
            _Fps = fps;
        }

        /// <summary>Конструктор</summary>
        /// <param name="host">Адрес</param>
        /// <param name="user_name">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="resolution">Разрешение</param>
        /// <param name="fps">Частота кадров</param>
        /// <param name="compression">Сжатие</param>
        public PanasonicVideo(string host, string user_name, string password, string resolution, string fps, string compression)
        {
            _Host = host;
            _UserName = user_name;
            _Password = password;
            _Resolution = resolution;
            _Fps = fps;
            _Compression = compression;
        }

        /// <summary>Активен</summary>
        public bool Active
        {
            get { lock (_Lock) { return _Active; } }
            set { SetActive(value); }
        }

        /// <summary>Текущий статус</summary>
        public PanasonicVideoStatus Status
        {
            get { lock (_Lock) { return _Status; } }
            protected set
            {
                lock (_Lock)
                {
                    if (_Status == value) return;
                    _Status = value;
                }
                try
                {
                    if (OnStatus != null)
                    {
                        PanasonicVideoStatusEventArgs args = new PanasonicVideoStatusEventArgs(value);
                        OnStatus(this, args);
                    }
                }
                catch (Exception) { }
            }
        }

        /// <summary>Адрес камеры</summary>
        public string Host
        {
            get { return _Host; }
            set
            {
                if (_Host == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _Host = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Порт камеры (по умолчанию 80)</summary>
        public int Port
        {
            get { return _Port; }
            set
            {
                if (_Port == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _Port = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Логин</summary>
        public string UserName
        {
            get { return _UserName; }
            set
            {
                if (_UserName == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _UserName = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Пароль</summary>
        public string Password
        {
            get { return _Password; }
            set
            {
                if (_Password == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _Password = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Видеоканал (1, 2, 3, 4, quad), пустая строка - по умолчанию</summary>
        public string Camera
        {
            get { return _Camera; }
            set
            {
                if (_Camera == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _Camera = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Компрессия видеопотока (0...100), пустая строка - то что установлено на камере</summary>
        public string Compression
        {
            get { return _Compression; }
            set
            {
                if (_Compression == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _Compression = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Разрешение (например 640х480, CIF), пустая строка - то что на камере</summary>
        public string Resolution
        {
            get { return _Resolution; }
            set
            {
                if (_Resolution == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _Resolution = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Количество кадров в секунду (1...30), пустая строка - то что на камере</summary>
        public string Fps
        {
            get { return _Fps; }
            set
            {
                if (_Fps == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _Fps = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Таймаут приема данных в миллисекундах (0 - бесконечность)</summary>
        public int ReceiveTimeout
        {
            get { return _ReceiveTimeout; }
            set
            {
                if (value < 0) value = 0;
                if (_ReceiveTimeout == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _ReceiveTimeout = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Таймаут передачи данных в миллисекундах (0 - бесконечность)</summary>
        public int SendTimeout
        {
            get { return _SendTimeout; }
            set
            {
                if (value < 0) value = 0;
                if (_SendTimeout == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _SendTimeout = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Изменить активность</summary>
        /// <param name="value"></param>
        void SetActive(bool value)
        {
            lock (_Lock)
            {
                if (_Active == value) return;
                _Active = value;
            }
            if (value)
            {
                try
                {
                    _Client = new TcpClient();
                    _Client.ReceiveTimeout = _ReceiveTimeout;
                    _Client.SendTimeout = _SendTimeout;
                    _ClientThread = new Thread(new ThreadStart(Communicating));
                    _ClientThread.Priority = ThreadPriority.AboveNormal;
                    _ClientThread.Start();
                }
                catch (Exception) { }
            }
            else
            {
                if (_Client != null)
                {
                    _Client.Close();
                    _Client = null;
                }
                if (_ClientThread != null)
                {
                    if (_ClientThread.IsAlive)
                    {
                        _ClientThread.Abort();
                        _ClientThread.Join();
                    }
                    _ClientThread = null;
                }
                Status = PanasonicVideoStatus.Inactive;
            }
        }

        void Communicating()
        {
            try
            {
                Status = PanasonicVideoStatus.Connecting;
                _Client.Connect(_Host, _Port);
                NetworkStream stream = _Client.GetStream();
                SendRequest(stream);
                int bytes_read;
                byte[] data = new byte[1024];//Исходное значение
                MemoryStream ms = new MemoryStream();
                string acc = string.Empty;
                ParserStage stage = ParserStage.StreamHeader;
                int header_pos = 0;
                int header_len = 0;
                int frame_len = 0;
                while (true)
                {
                    // Прием порции данных
                    bytes_read = stream.Read(data, 0, data.Length);
                    if (bytes_read == 0) { break; }
                    ms.Write(data, 0, bytes_read);

                    //НЕ этап разбора кадра
                    if (stage != ParserStage.FrameData )
                    {
                        acc += Encoding.Default.GetString(data, 0, bytes_read);
                    }

                    #region Обработка заголовка потока
                    if (stage == ParserStage.StreamHeader)
                    {
                        // Принят весь заголовок?
                        if (acc.IndexOf("\r\n\r\n") != -1)
                        {
                            if (acc.IndexOf("200 OK") != -1)
                            {
                                Status = PanasonicVideoStatus.Online;
                                if (acc.IndexOf("\r\n--myboundary") != -1)
                                {
                                    stage = ParserStage.SearchingBoundary;
                                }
                            }
                            else
                            {
                                if (acc.IndexOf("400 Bad Request") != -1)
                                {
                                    Status = PanasonicVideoStatus.BadRequest;
                                }
                                else if (acc.IndexOf("401 Unauthorized") != -1)
                                {
                                    Status = PanasonicVideoStatus.AccessDenied;
                                }
                                else
                                {
                                    Status = PanasonicVideoStatus.Error;
                                }
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Поиск границы между кадрами
                    if (stage == ParserStage.SearchingBoundary)
                    {
                        header_pos = acc.IndexOf("\r\n--myboundary");
                        if (header_pos != -1)
                        {
                            header_pos += 14;
                            acc = acc.Substring(header_pos);
                            stage = ParserStage.FrameHeader; // перейти в состояние вычитывания заголовка фрейма
                        }
                    }
                    #endregion

                    #region Чтение и разбор заголовка кадра
                    if (stage == ParserStage.FrameHeader)
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        header_len = acc.IndexOf("\r\n\r\n");
                        if (header_len > -1)
                        {
                            header_len += 4;
                            string boundary_header = acc.Substring(0, header_len);
                            string[] split = boundary_header.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in split)
                            {
                                string[] split_row = s.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                                if (split_row.Length > 1)
                                {
                                    dict.Add(split_row[0], split_row[1]);
                                }
                            }
                            if ((dict["Content-type"] == "image/jpeg") &&
                                int.TryParse(dict["Content-length"], out frame_len) &&
                                (frame_len > 0))
                            {
                                stage = ParserStage.FrameData;
                            }
                            else
                            {
                                // Пропускаем кадр
                                stage = ParserStage.SearchingBoundary;
                            }
                        }
                    }
                    #endregion

                    #region Чтение данных кадра
                    if (stage == ParserStage.FrameData)
                    {
                        if ((ms.Length - header_pos - header_len) >= frame_len)
                        {
                            byte[] frame_buf = new byte[frame_len];
                            ms.Position = header_pos + header_len;
                            ms.Read(frame_buf, 0, frame_len);
                            if (IsValidFrame(frame_buf))
                            {
                                RaiseFrameEvent(frame_buf);
                            }
                            stage = ParserStage.SearchingBoundary;
                            byte[] rest = new byte[ms.Length - header_pos - header_len - frame_len];
                            ms.Read(rest, 0, (int)(ms.Length - ms.Position));
                            ms.Close();
                            acc = Encoding.Default.GetString(rest);
                            ms = new MemoryStream();
                            ms.Write(rest, 0, rest.Length);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception e) 
            { }
            RestartClient();
        }

        bool IsValidFrame(byte[] frame_buf)
        {
            if (frame_buf == null) return false;
            int length = frame_buf.Length;
            if (length < 4) return false;

            //return (frame_buf[0] == 0xff) && (frame_buf[1] == 0xd8) && 
            //    (frame_buf[length - 2] == 0xff) && (frame_buf[length - 1] == 0xd9);

            return (frame_buf[0] == 0xff) && (frame_buf[1] == 0xd8);
        }

        void RaiseFrameEvent(byte[] frame_buf)
        {
            try
            {
                if (OnFrame != null)
                {
                    PanasonicVideoFrameEventArgs args = new PanasonicVideoFrameEventArgs(frame_buf);
                    OnFrame(this, args);
                }
            }
            catch (Exception) { }
        }

        void RestartClient()
        {
            if (Active == false) return;
            try
            {
                if (_Client != null)
                {
                    _Client.Close();
                    _Client = null;
                }
            }
            catch (Exception) { }
            if ((Status == PanasonicVideoStatus.Online) || (Status == PanasonicVideoStatus.Connecting))
            {
                Status = PanasonicVideoStatus.Offline;
            }
            int count = 30;
            while (Active && (count-- > 0))
            {
                Thread.Sleep(100);
            }
            if (Active)
            {
                _Client = new TcpClient();
                _Client.ReceiveTimeout = _ReceiveTimeout;
                _Client.SendTimeout = _SendTimeout;
                _ClientThread = new Thread(new ThreadStart(Communicating));
                _ClientThread.Start();
            }
        }

        void SendRequest(NetworkStream stream)
        {
            string auth = Convert.ToBase64String(Encoding.Default.GetBytes(_UserName + ":" + _Password));
            string request = "GET /nphMotionJpeg" + GetRequestParams() + " HTTP/1.1\r\n";
            request += "Connection: Keep-Alive\r\n";
            request += "Host: " + _Host + ":" + _Port.ToString() + "\r\n";
            request += "Authorization: Basic " + auth + "\r\n";
            request += "\r\n";
            byte[] buf = Encoding.Default.GetBytes(request);
            stream.Write(buf, 0, buf.Length);
            stream.Flush();
        }

        string GetRequestParams()
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(_Resolution) == false)
            {
                if (string.IsNullOrEmpty(result)) result += "?";
                result += "Resolution=" + _Resolution;
            }
            if (string.IsNullOrEmpty(_Camera) == false)
            {
                if (string.IsNullOrEmpty(result)) result += "?"; else result += "&";
                result += "camera=" + _Camera;
            }
            if (string.IsNullOrEmpty(_Compression) == false)
            {
                if (string.IsNullOrEmpty(result)) result += "?"; else result += "&";
                //result += "Quality=" + _Compression;
                result += "Quality=Standard";
            }
            if (string.IsNullOrEmpty(_Fps) == false)
            {
                if (string.IsNullOrEmpty(result)) result += "?"; else result += "&";
                result += "Framerate=" + _Fps;
            }
            return result;
        }

        #region Члены IDisposable

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Active = false;
            }
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        ~PanasonicVideo()
        {
            Dispose(false);
        }

        #endregion

    }
}
