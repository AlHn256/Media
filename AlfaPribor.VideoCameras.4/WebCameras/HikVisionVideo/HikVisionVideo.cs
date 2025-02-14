using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

//Компоненты приема RTPS потока
using AlfaPribor.RTSP.Client;
using AlfaPribor.RTSP.Client.Rtsp;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Получение видеопотока MJPEG от камеры HikVision</summary>
    public class HikVisionVideo : IDisposable
    {
        
        enum ParserStage { StreamHeader, SearchingBoundary, FrameHeader, FrameData };

        #region Variables

        bool active = false;
        HikVisionVideoStatus status = HikVisionVideoStatus.Inactive;
        string host = "192.168.221.101";
        int port = 554;
        string login = "admin";
        string password = "admin123";
        string resolution = "1920x1080";
        string compression = string.Empty;

        /// <summary>Скорость захвата кадров (кадров/с)</summary>
        string fps = string.Empty;
        
        /// <summary>Номер видеоканала</summary>
        int channel = 1;

        /// <summary>Объект блокировки</summary>
        object Lock = new object();

        /// <summary>Поток подключения</summary>
        Thread clientThread = null;

        #endregion

        #region Events

        /// <summary>Событие "Изменился статус клиента Basler видео"</summary>
        public event EventHandler<HikVisionVideoStatusEventArgs> OnStatus;
        /// <summary>Событие "Получен кадр от камеры Basler"</summary>
        public event EventHandler<HikVisionVideoFrameEventArgs> OnFrame;

        #endregion

        /// <summary>Конструктор</summary>
        public HikVisionVideo() { }

        /// <summary>Конструктор</summary>
        /// <param name="host">Адрес</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="resolution">Разрешение</param>
        /// <param name="fps">Частота кадров</param>
        public HikVisionVideo(string host, string login, string password, string resolution, string fps)
        {
            this.host = host;
            this.login = login;
            this.password = password;
            this.resolution = resolution;
            this.fps = fps;
        }

        /// <summary>Конструктор</summary>
        /// <param name="host">Адрес</param>
        /// <param name="user_name">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="resolution">Разрешение</param>
        /// <param name="fps">Частота кадров</param>
        /// <param name="compression">Сжатие</param>
        public HikVisionVideo(string host, string user_name, string password, string resolution, string fps, string compression)
        {
            this.host = host;
            login = user_name;
            this.password = password;
            this.resolution = resolution;
            this.fps = fps;
            this.compression = compression;
        }

        /// <summary>Активен</summary>
        public bool Active
        {
            get { lock (Lock) { return active; } }
            set { SetActive(value); }
        }

        /// <summary>Текущий статус</summary>
        public HikVisionVideoStatus Status
        {
            get { lock (Lock) { return status; } }
            protected set
            {
                lock (Lock)
                {
                    if (status == value) return;
                    status = value;
                }
                try
                {
                    if (OnStatus != null)
                    {
                        HikVisionVideoStatusEventArgs args = new HikVisionVideoStatusEventArgs(value);
                        OnStatus(this, args);
                    }
                }
                catch (Exception) { }
            }
        }

        /// <summary>Адрес камеры</summary>
        public string Host
        {
            get { return host; }
            set
            {
                if (host == value) return;
                bool restart = Active;
                if (restart) Active = false;
                host = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Порт камеры (по умолчанию RTSP порт 554)</summary>
        public int Port
        {
            get { return port; }
            set
            {
                if (port == value) return;
                bool restart = Active;
                if (restart) Active = false;
                port = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Логин</summary>
        public string Login
        {
            get { return login; }
            set
            {
                if (login == value) return;
                bool restart = Active;
                if (restart) Active = false;
                login = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Пароль</summary>
        public string Password
        {
            get { return password; }
            set
            {
                if (password == value) return;
                bool restart = Active;
                if (restart) Active = false;
                password = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Видеоканал (1, 2, 3, 4)</summary>
        public int Channel
        {
            get { return channel; }
            set
            {
                if (channel == value) return;
                bool restart = Active;
                if (restart) Active = false;
                channel = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Компрессия видеопотока (0...100), пустая строка - то что установлено на камере</summary>
        public string Compression
        {
            get { return compression; }
            set
            {
                if (compression == value) return;
                bool restart = Active;
                if (restart) Active = false;
                compression = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Разрешение (например 640х480, CIF), пустая строка - то что на камере</summary>
        public string Resolution
        {
            get { return resolution; }
            set
            {
                if (resolution == value) return;
                bool restart = Active;
                if (restart) Active = false;
                resolution = value;
                if (restart) Active = true;
            }
        }

        /// <summary>Количество кадров в секунду (1...30), пустая строка - то что на камере</summary>
        public string Fps
        {
            get { return fps; }
            set
            {
                if (fps == value) return;
                bool restart = Active;
                if (restart) Active = false;
                fps = value;
                if (restart) Active = true;
            }
        }
        
        /// <summary>Изменить активность</summary>
        /// <param name="value"></param>
        void SetActive(bool value)
        {
            lock (Lock)
            {
                if (active == value) return;
                active = value;
            }
            if (value)
            {
                try
                {
                    clientThread = new Thread(new ThreadStart(Communicating));
                    clientThread.Priority = ThreadPriority.AboveNormal;
                    clientThread.Start();
                }
                catch (Exception) { }
            }
            else
            {
                if (clientThread != null)
                {
                    if (clientThread.IsAlive)
                    {
                        clientThread.Abort();
                        clientThread.Join();
                    }
                    clientThread = null;
                }
                Status = HikVisionVideoStatus.Inactive;
            }
        }

        /// <summary>Получение видеопотока</summary>
        void Communicating()
        {
            try
            {
                Status = HikVisionVideoStatus.Connecting;
                Uri serverUri = new Uri("rtsp://" + host + ":" + port.ToString());
                NetworkCredential credentials = new NetworkCredential(login, password);
                ConnectionParameters connectionParameters = new ConnectionParameters(serverUri, credentials);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                Task connectTask = ConnectAsync(connectionParameters, cancellationTokenSource.Token);
                while (active) { Thread.Sleep(10); }
            }
            catch (Exception) { }
            RestartClient();
        }
        
        /// <summary>Асинхронное подключение</summary>
        /// <param name="connectionParameters"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        async Task ConnectAsync(ConnectionParameters connectionParameters, CancellationToken token)
        {
            try
            {
                TimeSpan delay = TimeSpan.FromSeconds(5);
                using (var rtspClient = new RtspClient(connectionParameters))
                {
                    rtspClient.FrameReceived -= RtspClient_FrameReceived;
                    rtspClient.FrameReceived += RtspClient_FrameReceived;
                    while (true)
                    {
                        //Console.WriteLine("Connecting...");
                        try { await rtspClient.ConnectAsync(token); }
                        catch (OperationCanceledException) { return; }
                        catch (RtspClientException e)
                        {
                            //Console.WriteLine(e.ToString());
                            await Task.Delay(delay, token);
                            continue;
                        }
                        //Console.WriteLine("Connected.");
                        try { await rtspClient.ReceiveAsync(token); }
                        catch (OperationCanceledException) { return; }
                        catch (RtspClientException e)
                        {
                            //Console.WriteLine(e.ToString());
                            await Task.Delay(delay, token);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>Получение кадра от RTSP клиента</summary>
        /// <param name="sender">RTSP клиент</param>
        /// <param name="e">Данные кадра</param>
        void RtspClient_FrameReceived(object sender, AlfaPribor.RTSP.Client.RawFrames.RawFrame e)
        {
            DateTime ts = e.Timestamp;
            AlfaPribor.RTSP.Client.RawFrames.FrameType typ = e.Type;
            ArraySegment<byte> seg = e.FrameSegment;
            Array a = seg.Array;
            byte[] b = (byte[])a;
            Array.Resize(ref b, e.FrameSegment.Count);
            RaiseFrameEvent(b);
        }

        /// <summary>Генерировать событие кадра</summary>
        /// <param name="frame_buf">Буфер кадра</param>
        void RaiseFrameEvent(byte[] frame_buf)
        {
            try { if (OnFrame != null) OnFrame(this, new HikVisionVideoFrameEventArgs(frame_buf)); }
            catch (Exception) { }
        }

        /// <summary>Перезапуск получения видеопотока</summary>
        void RestartClient()
        {
            if (Active == false) return;

            if ((Status == HikVisionVideoStatus.Online) || Status == HikVisionVideoStatus.Connecting) Status = HikVisionVideoStatus.Offline;

            int count = 20;
            while (Active && (count-- > 0)) Thread.Sleep(100);

            if (Active)
            {
                clientThread = new Thread(new ThreadStart(Communicating));
                clientThread.Start();
            }
        }

        /// <summary>Закрыть объект видеозахвата (надо ли?)</summary>
        void CloseCapture()
        {

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
        ~HikVisionVideo()
        {
            Dispose(false);
        }

        #endregion

    }
}
