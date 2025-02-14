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
    /// <summary>Получение видеопотока MJPEG от камеры Basler</summary>
    public class BaslerVideo : IDisposable
    {
        
        enum ParserStage { StreamHeader, SearchingBoundary, FrameHeader, FrameData };

        #region Variables

        bool _Active = false;
        BaslerVideoStatus _Status = BaslerVideoStatus.Inactive;
        string _Host = "192.168.221.101";
        int _Port = 80;
        string _UserName = "root";
        string _Password = "root";
        string _Resolution = "1280x1024";
        string _Compression = string.Empty;

        /// <summary>Скорость захвата кадров (кадров/с)</summary>
        string _Fps = string.Empty;
        
        /// <summary>Номер видеоканала</summary>
        int _Channel = 1;
        
        /// <summary>Таймаут приема данных в миллисекундах (0 - бесконечность)</summary>
        int _ReceiveTimeout = 5000;

        /// <summary>Таймаут передачи данных в миллисекундах (0 - бесконечность)</summary>
        int _SendTimeout = 5000;

        object _Lock = new object();
        TcpClient _Client = null;
        Thread _ClientThread = null;

        #endregion

        #region Events

        /// <summary>Событие "Изменился статус клиента Basler видео"</summary>
        public event EventHandler<BaslerVideoStatusEventArgs> OnStatus;
        /// <summary>Событие "Получен кадр от камеры Basler"</summary>
        public event EventHandler<BaslerVideoFrameEventArgs> OnFrame;

        #endregion

        /// <summary>Конструктор</summary>
        public BaslerVideo() { }

        /// <summary>Конструктор</summary>
        /// <param name="host">Адрес</param>
        /// <param name="user_name">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="resolution">Разрешение</param>
        /// <param name="fps">Частота кадров</param>
        public BaslerVideo(string host, string user_name, string password, string resolution, string fps)
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
        public BaslerVideo(string host, string user_name, string password, string resolution, string fps, string compression)
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
            get
            {
                lock (_Lock)
                {
                    return _Active;
                }
            }
            set
            {
                SetActive(value);
            }
        }

        /// <summary>Текущий статус</summary>
        public BaslerVideoStatus Status
        {
            get
            {
                lock (_Lock)
                {
                    return _Status;
                }
            }
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
                        BaslerVideoStatusEventArgs args = new BaslerVideoStatusEventArgs(value);
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

        /// <summary>Видеоканал (1, 2, 3)</summary>
        public int Channel
        {
            get { return _Channel; }
            set
            {
                if (_Channel == value) return;
                bool restart = Active;
                if (restart) Active = false;
                _Channel = value;
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
                Status = BaslerVideoStatus.Inactive;
            }
        }

        void Communicating()
        {
            try
            {
                Status = BaslerVideoStatus.Connecting;
                _Client.Connect(_Host, _Port);
                NetworkStream stream = _Client.GetStream();
                SendRequest(stream);
                //SendRequest2(stream);
                int bytes_read;
                byte[] data = new byte[1024];
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
                    if (bytes_read == 0)
                    {
                        break;
                    }

                    ms.Write(data, 0, bytes_read);

                    if (stage != ParserStage.FrameData )
                    {
                        acc += Encoding.Default.GetString(data, 0, bytes_read);
                    }

                    // Обработка заголовка потока
                    if (stage == ParserStage.StreamHeader)
                    {
                        // Принят весь заголовок?
                        if (acc.IndexOf("\r\n\r\n") != -1)
                        {
                            if (acc.IndexOf("200 OK") != -1)
                            {
                                Status = BaslerVideoStatus.Online;
                                if (acc.IndexOf("\r\n--myboundary") != -1)
                                {
                                    stage = ParserStage.SearchingBoundary;
                                }
                            }
                            else
                            {
                                if (acc.IndexOf("400 Bad Request") != -1)
                                {
                                    Status = BaslerVideoStatus.BadRequest;
                                }
                                else if (acc.IndexOf("401 Unauthorized") != -1)
                                {
                                    Status = BaslerVideoStatus.AccessDenied;
                                }
                                else
                                {
                                    Status = BaslerVideoStatus.Error;
                                }
                                break;
                            }
                        }
                    }

                    // Поиск границы между кадрами
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

                    // Чтение и разбор заголовка кадра
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
                                    try
                                    {
                                        dict.Add(split_row[0], split_row[1]);
                                    }
                                    catch { };
                                }
                            }
                            if ((dict["Content-Type"] == "image/jpeg") &&
                                int.TryParse(dict["Content-Length"], out frame_len) &&
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

                    // Чтение данных кадра
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
                }
            }
            catch (Exception) { }
            RestartClient();
        }

        bool IsValidFrame(byte[] frame_buf)
        {
            if (frame_buf == null) return false;
            int length = frame_buf.Length;
            if (length < 4) return false;
            return (frame_buf[0] == 0xff) && (frame_buf[1] == 0xd8) && 
                (frame_buf[length - 2] == 0xff) && (frame_buf[length - 1] == 0xd9);
        }

        void RaiseFrameEvent(byte[] frame_buf)
        {
            try
            {
                if (OnFrame != null)
                {
                    BaslerVideoFrameEventArgs args = new BaslerVideoFrameEventArgs(frame_buf);
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
            if ((Status == BaslerVideoStatus.Online) || (Status == BaslerVideoStatus.Connecting))
            {
                Status = BaslerVideoStatus.Offline;
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
            string request = "GET /cgi-bin/mjpeg" + GetRequestParams() + " HTTP/1.1\r\n";
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

            //Номер видеопотока
            if (_Channel > 0)
            {
                if (string.IsNullOrEmpty(result)) result += "?"; else result += "&";
                result += "stream=" + (_Channel - 1).ToString();
            }             
            //Количество кадров
            if (string.IsNullOrEmpty(_Fps) == false)
            {
                if (string.IsNullOrEmpty(result)) result += "?"; else result += "&";
                result += "fps=" + _Fps;
            }          
            
            //Разрешение - на Баслере не работает
            //if (string.IsNullOrEmpty(_Resolution) == false)
            //{
            //    if (string.IsNullOrEmpty(result)) result += "?";
            //    result += "resolution=" + _Resolution;
            //}
            //Сжатие - на Баслере не работает
            //if (string.IsNullOrEmpty(_Compression) == false)
            //{
            //    if (string.IsNullOrEmpty(result)) result += "?"; else result += "&";
            //    result += "compression=" + _Compression;
            //}
            
            return result;
        }

        #region Члены IDisposable

        /// <summary>Освободить ресурсы</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                Active = false;
            }
        }

        /// <summary>Деструктор</summary>
        ~BaslerVideo()
        {
            Dispose(false);
        }

        #endregion

    }
}
