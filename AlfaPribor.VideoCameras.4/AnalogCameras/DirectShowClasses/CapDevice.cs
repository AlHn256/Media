using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using Microsoft.Win32;

using AlfaPribor.VideoCameras4;

namespace AnalogNamespace
{
    internal class CapDevice : IDisposable
    {

        #region Variables

        ManualResetEvent stopSignal;
        Thread worker;
        //IAMCrossbar pXBar;
        IGraphBuilder g_pGraph;
        ICaptureGraphBuilder2 g_pCapture;
        ISampleGrabber grabber;
        //IAMStreamConfig pConfig;
        IBaseFilter sourceObject, grabberObject;
        IMediaControl control;
        CapGrabber capGrabber;
        static string deviceMoniker;
        //IntPtr map;
        //IntPtr section;

        int DeviceNo;
        int DeviceInput;
        int Width;
        int Height;

        public delegate void DelegateEventNewFrame(byte[] frame);
        public event DelegateEventNewFrame NewFrameArrived;

        System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
        //double frames;

        AnalogVideoStandard VideoStandard = AnalogVideoStandard.PAL_D;
        System.Guid Sybtype = MediaSubType.YUY2;

        object _lock = new object();
        object _lock2 = new object();

        #endregion

        #region Events

        public event EventHandler OnNewBitmapReady;
        public event EventHandler OnThreadStart;

        #endregion

        /// <summary>Конструктор класса устройства видеозахвата</summary>
        /// <param name="device">Номер устройства</param>
        /// <param name="input">Вход</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        /// <param name="video_standard">Видеостандарт</param>
        /// <param name="video_subtype">Цветовое пространство</param>
        public CapDevice(int device, int input, int width, int height,
                         AnalogVideoStandard video_standard, System.Guid video_subtype)
        {
            deviceMoniker = DeviceMonikes[device].MonikerString;
            DeviceNo = device;
            DeviceInput = input;
            Width = width;
            Height = height;
            VideoStandard = video_standard;
            Sybtype = video_subtype;
        }

        public CapDevice(string moniker)
        {
            deviceMoniker = moniker;
        }

        public void Start()
        {
            if (worker == null)
            {
                capGrabber = new CapGrabber();
                capGrabber.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(capGrabber_PropertyChanged);
                capGrabber.NewFrameArrived += new CapGrabber.DelegateEventNewFrame(capGrabber_NewFrameArrived);
                stopSignal = new ManualResetEvent(false);
                //Запуск потока инициализации устройства и старт видеозахвата
                worker = new Thread(new ThreadStart(RunWorker));
                worker.Priority = ThreadPriority.Lowest;
                worker.Start();
            }
            else
            {
                Stop();
                Start();
            }
        }

        void capGrabber_NewFrameArrived(byte[] frame)
        {
            if (NewFrameArrived != null) NewFrameArrived(frame);
        }

        /// <summary>Событие изменения свойств видеозахвата</summary>
        void capGrabber_PropertyChanged(object sender, 
                                        System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (capGrabber.Width != default(int) && capGrabber.Height != default(int))
            {
                if (OnNewBitmapReady != null) OnNewBitmapReady(this, null);
            }
        }

        /// <summary>Остановка потока видеозахвата</summary>
        public void Stop()
        {
            if (IsRunning)
            {
                stopSignal.Set();
                worker.Abort();
                if (worker != null)
                {
                    worker.Join();
                    Release();
                }
            }
        }

        /// <summary>Флаг активности потока видеозахвата</summary>
        public bool IsRunning
        {
            get
            {
                if (worker != null)
                {
                    if (worker.Join(0) == false)
                        return true;

                    Release();
                }
                return false;
            }
        }

        void Release()
        {
            worker = null;
            stopSignal.Close();
            stopSignal = null;
        }

        /// <summary>Получение массива устроств видеозахвата</summary>
        public static FilterInfo[] DeviceMonikes
        {
            get
            {
                List<FilterInfo> filters = new List<FilterInfo>();
                IMoniker[] ms = new IMoniker[1];
                ICreateDevEnum enumD = Activator.CreateInstance(Type.GetTypeFromCLSID(SystemDeviceEnum)) as ICreateDevEnum;
                IEnumMoniker moniker;
                Guid g = VideoInputDevice;
                if (enumD.CreateClassEnumerator(ref g, out moniker, 0) == 0)
                {
                    while (true)
                    {
                        int r = moniker.Next(1, ms, IntPtr.Zero);
                        if (r != 0 || ms[0] == null)
                            break;
                        filters.Add(new FilterInfo(ms[0]));
                        Marshal.ReleaseComObject(ms[0]);
                        ms[0] = null;
                        //break;
                    }
                }
                return filters.ToArray();
            }
        }

        void GetInterfaces()
        {
            g_pCapture = Activator.CreateInstance(Type.GetTypeFromCLSID(GraphBuilder)) as ICaptureGraphBuilder2;
            g_pGraph = Activator.CreateInstance(Type.GetTypeFromCLSID(FilterGraph)) as IGraphBuilder;
            grabber = Activator.CreateInstance(Type.GetTypeFromCLSID(SampleGrabber)) as ISampleGrabber;//Интерфейс Sample Grabber
            grabberObject = grabber as IBaseFilter;//Фильтр Sample Grabber
        }

        /// <summary>Старт потока посторения графа</summary>
        void RunWorker()
        {
            try
            {
                int res = 0;
                lock (_lock)
                {
                    //Получение интерфейсов
                    GetInterfaces();
                    res = g_pCapture.SetFiltergraph(g_pGraph);
                    //sourceObject = FilterInfo.CreateFilter(deviceMoniker);//Установка устройства видеозахвата
                    sourceObject = GetCaptureDevice(DeviceNo);
                    SetResolution(Width, Height);//Установка разрешения видеозахвата
                    res = g_pGraph.AddFilter(sourceObject, "source");     //Добавление фильтра устройства
                    res = g_pGraph.AddFilter(grabberObject, "grabber");   //Добавление фильтра Sample Grabber
                    AMMediaType mediaType = new AMMediaType();
                    mediaType.majorType = MediaTypes.Video;
                    mediaType.subType = MediaSubType.RGB24;
                    res = grabber.SetMediaType(mediaType);
                    //Отсоединение пина
                    res = g_pGraph.Disconnect(GetPinThis(sourceObject, PinDirection.Output, 0));
                    //Соединение выходного пина устройства видеозахвата
                    //со входным пином SampleGrabber
                    res = g_pGraph.Connect(GetPinThis(sourceObject, PinDirection.Output, 0), GetPinThis(grabberObject, PinDirection.Input, 0));
                    if (res >= 0)
                    {
                        if (grabber.GetConnectedMediaType(mediaType) == 0)
                        {
                            VideoInfoHeader header = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));
                            capGrabber.Width = header.BmiHeader.Width;
                            capGrabber.Height = header.BmiHeader.Height;
                        }
                    }
                    res = g_pGraph.Render(GetPinThis(grabberObject, PinDirection.Output, 0));
                    res = grabber.SetBufferSamples(false);
                    //res = grabber.SetOneShot(false);
                    res = grabber.SetOneShot(true);
                    res = grabber.SetCallback(capGrabber, 1);
                    SetCrossbarRoute(DeviceInput);  //Установка номера входа
                    SetVideoStandard(VideoStandard);//Установка видеостандарта
                    SetColorSpace(Sybtype);         //Установка цветового пространства
                    //Запретить собственное окно отображения
                    IVideoWindow wnd = (IVideoWindow)g_pGraph;
                    res = wnd.put_AutoShow(false);
                    wnd = null;
                    //Запуск графа
                    control = (IMediaControl)g_pGraph;
                }
                res = control.Run();
                //Событие запуска потока видеозахвата
                if (OnThreadStart != null) OnThreadStart(this, EventArgs.Empty);
                while (!stopSignal.WaitOne(0, true)) {   Thread.Sleep(10); }
                control.StopWhenReady();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                //Остановка графа
                lock (_lock2)
                {
                    int res;
                    //res = control.StopWhenReady();
                    //try { res = control.Stop(); } catch { };
                    res = g_pGraph.Disconnect(GetPinThis(sourceObject, PinDirection.Output, 0));
                    res = g_pGraph.RemoveFilter(sourceObject);
                    res = g_pGraph.RemoveFilter(grabberObject);
                    capGrabber.PropertyChanged -= capGrabber_PropertyChanged;
                    capGrabber.NewFrameArrived -= capGrabber_NewFrameArrived;
                    capGrabber = null;
                    Marshal.ReleaseComObject(g_pCapture);
                    g_pCapture = null;
                    Marshal.ReleaseComObject(g_pGraph);
                    g_pGraph = null;
                    Marshal.ReleaseComObject(sourceObject);
                    sourceObject = null;
                    Marshal.ReleaseComObject(grabberObject);
                    grabberObject = null;
                    Marshal.ReleaseComObject(grabber);
                    grabber = null;
                    Marshal.ReleaseComObject(control);
                    control = null;
                }
            }
           
        }

        IBaseFilter GetCaptureDevice(int DeviceNo)
        {
            IMoniker[] ms = new IMoniker[1];
            ICreateDevEnum enumD = Activator.CreateInstance(Type.GetTypeFromCLSID(SystemDeviceEnum)) as ICreateDevEnum;
            IEnumMoniker moniker;
            Guid g = VideoInputDevice;
            if (enumD.CreateClassEnumerator(ref g, out moniker, 0) == 0)
            {
                for (int i = 0; i <= DeviceNo; i++)
                {
                    int r = moniker.Next(1, ms, IntPtr.Zero);
                    if (r != 0 || ms[0] == null) return null;
                }
                object o = null;
                Guid fg = typeof(IBaseFilter).GUID;
                ms[0].BindToObject(null, null, ref fg, out o);//Выбор нужного устройства
                if (o == null) return null;
                IBaseFilter filter = o as IBaseFilter;
                return filter;
            }
            return null;
        }

        /// <summary>Получение пина фильтра</summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="dir">Напрваление пина (ввхо/выход)</param>
        /// <param name="num">Номер пина</param>
        /// <returns>Пин</returns>
        static IPin GetPinThis(IBaseFilter filter, PinDirection dir, int num)
        {
            IPin[] pin = new IPin[1];
            IEnumPins pinsEnum = null;

            if (filter.EnumPins(out pinsEnum) == 0)
            {
                PinDirection pinDir;
                int n;

                while (pinsEnum.Next(1, pin, out n) == 0)
                {
                    pin[0].QueryDirection(out pinDir);

                    if (pinDir == dir)
                    {
                        if (num == 0)
                            return pin[0];
                        num--;
                    }

                    Marshal.ReleaseComObject(pin[0]);
                    pin[0] = null;
                }
            }
            return null;
        }

        bool SetResolution(int Width, int Height)
        {
            AMMediaType media;
            object iconfig;
            g_pCapture.FindInterface(PinCategory.Capture, MediaType.Video, sourceObject, 
                                     typeof(IAMStreamConfig).GUID, out iconfig);
            IAMStreamConfig videoStreamConfig = iconfig as IAMStreamConfig;
            try
            {
                if (videoStreamConfig == null) return false;
                videoStreamConfig.GetFormat(out media);
                // copy out the videoinfoheader
                VideoInfoHeader v = new VideoInfoHeader();
                object s = new object();
                try
                {
                    Marshal.PtrToStructure(media.formatPtr, v);
                }
                catch { };
                if (Width > 0) v.BmiHeader.Width = Width;   //Ширина кадра
                if (Height > 0) v.BmiHeader.Height = Height;//Высота кадра
                // Copy the media structure back
                Marshal.StructureToPtr(v, media.formatPtr, false);
                // Set the new format
                videoStreamConfig.SetFormat(media);
                DsUtils.FreeAMMediaType(media);
                media = null;
            }
            finally
            {
                Marshal.ReleaseComObject(videoStreamConfig);
            }
            return true;
        }

        /// <summary>Установка маршрута трансляции видеосигнала</summary>
        /// <param name="input">Номер входа</param>
        /// <returns>Результат операции</returns>
        bool SetCrossbarRoute(int input)
        {
            //Поиск соответствующего кроссбара
            object o;
            int hr = g_pCapture.FindInterface(FindDirection.UpstreamOnly, null, sourceObject,
                                              typeof(IAMCrossbar).GUID, out o);
            IAMCrossbar pXBar1 = o as IAMCrossbar;
            if (pXBar1 != null)
            {
                int OutPinsCount;
                int InputPinsCount;
                pXBar1.get_PinCounts(out OutPinsCount, out InputPinsCount);
                if (pXBar1.CanRoute(0, input) == 0) pXBar1.Route(0, input);
            }
            //xbar = pXBar1;
            if (hr == 0) return true;
            else return false;
        }

        /// <summary>Установка видеостандарта для устройства видеозахвата по имени</summary>
        /// <param name="VideoStandard">Видеостандарт трансляции телекамеры</param>
        /// <returns>Результат операции</returns>
        public bool SetVideoStandard(AnalogVideoStandard VideoStandard)
        {
            //Видеостандарт
            try
            {
                IAMAnalogVideoDecoder pAMAnalogVideoDecoder = (IAMAnalogVideoDecoder)sourceObject;
                if (pAMAnalogVideoDecoder == null) return false;
                if (pAMAnalogVideoDecoder.put_TVFormat(VideoStandard) != 0) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>Установка цветового пространства</summary>
        public bool SetColorSpace(System.Guid subtype)
        {
            AMMediaType media = null;
            object iconfig;
            g_pCapture.FindInterface(PinCategory.Capture, MediaType.Video, sourceObject,
                                     typeof(IAMStreamConfig).GUID, out iconfig);
            IAMStreamConfig videoStreamConfig = iconfig as IAMStreamConfig;
            if (videoStreamConfig == null) return false;
            try
            {
                int iFormat = 0;
                IntPtr scc = Marshal.AllocHGlobal(128);
                videoStreamConfig.GetStreamCaps(iFormat, out media, scc);
                media.subType = subtype;
                videoStreamConfig.SetFormat(media);
                DsUtils.FreeAMMediaType(media);
                Marshal.FreeHGlobal(scc);
                media = null;
            }
            catch
            {

            }
            finally
            {
                Marshal.ReleaseComObject(videoStreamConfig);
            }
            return true;
        }

        public static int GetCaptureDevicesCount()
        {
            return 0;
        }

        #region Видеопараметры

        /// <summary>Установка яркости</summary>
        /// <param name="Value">Значение в процентах</param>
        /// <returns>Результат операции</returns>
        public bool SetBrightness(int Value)
        {
            return SetParam(VideoProcAmpProperty.Brightness, Value);
        }

        /// <summary>Установка яркости по умолчанию</summary>
        /// <returns>Результат операции</returns>
        public bool SetDefaultBrightness()
        {
            return SetDefaultParam(VideoProcAmpProperty.Brightness);
        }

        /// <summary>Установка контрастности</summary>
        /// <param name="Value">Значение в процентах</param>
        /// <returns>Результат операции</returns>
        public bool SetContrast(int Value)
        {
            return SetParam(VideoProcAmpProperty.Contrast, Value);
        }

        /// <summary>Установка контрастности по умолчанию</summary>
        /// <returns>Результат операции</returns>
        public bool SetDefaultContrast()
        {
            return SetDefaultParam(VideoProcAmpProperty.Contrast);
        }

        /// <summary>Установка оттенка</summary>
        /// <param name="Value">Значение в процентах</param>
        /// <returns>Результат операции</returns>
        public bool SetHue(int Value)
        {
            return SetParam(VideoProcAmpProperty.Hue, Value);
        }

        /// <summary>Установка насыщенности</summary>
        /// <param name="Value">Значение в процентах</param>
        /// <returns>Результат операции</returns>
        public bool SetSaturation(int Value)
        {
            return SetParam(VideoProcAmpProperty.Saturation, Value);
        }

        /// <summary>Установка четкости</summary>
        /// <param name="Value">Значение в процентах</param>
        /// <returns>Результат операции</returns>
        public bool SetSharpness(int Value)
        {
            return SetParam(VideoProcAmpProperty.Sharpness, Value);
        }

        /// <summary>Установка гаммы</summary>
        /// <param name="Value">Значение в процентах</param>
        /// <returns>Результат операции</returns>
        public bool SetGamma(int Value)
        {
            return SetParam(VideoProcAmpProperty.Gamma, Value);
        }

        /// <summary>Установка параметра устройства видеозахвата</summary>
        /// <param name="property">Свойство</param>
        /// <param name="Value">Значение в процентах</param>
        /// <returns>Результат операции</returns>
        bool SetParam(VideoProcAmpProperty property, int Value)
        {
            try
            {
                IAMVideoProcAmp pVideoProcAmp = (IAMVideoProcAmp)sourceObject;
                if (pVideoProcAmp == null) return false;
                int Min;
                int Max;
                int SteppingDelta;
                int Default;
                VideoProcAmpFlags CapsFlags;
                pVideoProcAmp.GetRange(property, out Min, out Max, out SteppingDelta, out Default, out CapsFlags);
                int ParamValue = (int)(((Max - Min) / 100.0f) * Value + Min);
                pVideoProcAmp.Set(property, ParamValue, VideoProcAmpFlags.Manual);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Установка параметра по умолчанию устройства видеозахвата</summary>
        /// <param name="property">Свойство</param>
        /// <returns>Результат операции</returns>
        bool SetDefaultParam(VideoProcAmpProperty property)
        {
            try
            {
                IAMVideoProcAmp pVideoProcAmp = (IAMVideoProcAmp)sourceObject;
                if (pVideoProcAmp == null) return false;
                int Min;
                int Max;
                int SteppingDelta;
                int Default;
                VideoProcAmpFlags CapsFlags;
                pVideoProcAmp.GetRange(property, out Min, out Max, out SteppingDelta, out Default, out CapsFlags);
                pVideoProcAmp.Set(property, Default, VideoProcAmpFlags.Manual);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Guids

        static readonly Guid GraphBuilder = new Guid("BF87B6E1-8C27-11d0-B3F0-00AA003761C5");

        static readonly Guid FilterGraph = new Guid(0xE436EBB3, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        static readonly Guid SampleGrabber = new Guid(0xC1F400A0, 0x3F08, 0x11D3, 0x9F, 0x0B, 0x00, 0x60, 0x08, 0x03, 0x9E, 0x37);

        static readonly Guid StreamConfig = new Guid(0xC6E13340, 0x30AC, 0x11D0, 0xA1, 0x8C, 0x00, 0xA0, 0xC9, 0x11, 0x89, 0x56);

        public static readonly Guid SystemDeviceEnum = new Guid(0x62BE5D10, 0x60EB, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        public static readonly Guid VideoInputDevice = new Guid(0x860BB310, 0x5D01, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        #endregion

        [ComVisible(false)]
        internal class MediaTypes
        {
            public static readonly Guid Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid Interleaved = new Guid(0x73766169, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid Audio = new Guid(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid Text = new Guid(0x73747874, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid Stream = new Guid(0xE436EB83, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
        }

        #region API

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion 
    }
}
