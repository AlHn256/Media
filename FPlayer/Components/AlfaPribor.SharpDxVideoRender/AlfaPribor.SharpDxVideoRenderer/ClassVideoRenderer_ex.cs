using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using SharpDX;
using SharpDX.Windows;
using SharpDX.Direct3D9;

using AlfaPribor.IppInterop;

namespace AlfaPribor.SharpDXVideoRenderer
{

    /// <summary>Класс рисования кадров на панели</summary>
    public class SharpDXVideoRenderer_ex : IDisposable
    {

        /// <summary>Угол поворота рисуемой картинки</summary>
        public enum RotationAngle
        {
            /// <summary>0</summary>
            deg_0 = 0,
            /// <summary>90</summary>
            deg_90 = 1,
            /// <summary>180</summary>
            deg_180 = 2,
            /// <summary>270</summary>
            deg_270 = 3
        }

        #region Declaration

        Direct3D direct3D;

        /// <summary>Линия на экране</summary>
        public struct Line
        {
            public int color;
            public int X1;
            public int Y1;
            public int X2;
            public int Y2;

            public Line(int col, int x1, int y1, int x2, int y2)
            {
                color = col;
                X1 = x1;
                Y1 = y1;
                X2 = x2;
                Y2 = y2;
            }
        }

        /// <summary>Стока на экране</summary>
        public struct DrawString
        {
            public string Str;
            public int X;
            public int Y;
            public uint Color;

            public DrawString(string str, int x1, int y1, uint color)
            {
                Str = str;
                X = x1;
                Y = y1;
                Color = color;
            }
        }

        delegate void LogMessageCallbackDrawFrame(byte[] array, int w, int h);

        /// <summary>Делегат безопасной установки текстовой метки</summary>
        delegate bool CreateDeviceThreadSafeCallback(int width, int height, bool hardware_vertex);
        /// <summary>Смена состояния инициалищации</summary>
        /// <param name="sender"></param>
        /// <param name="init">false - инициализация завершена. true - в процессе инициализации</param>
        public delegate void EventChangeInitStateEventHandler(object sender, bool init, bool dispose);

        public event EventChangeInitStateEventHandler EventChangeInitState;


        

        #region API structures

        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        #endregion

        #region Variables

        byte[] BitmapData;
        /// <summary>Устройство отрисовки</summary>
        Device device = null;
        /// <summary>Устройство отрисовки увеличенного окна</summary>
        //Device device_max = null;
        /// <summary>Текстура</summary>
        Texture texture;
        Texture second_texture;
        /// <summary>Поток текстуры</summary>
        DataStream textureData;
        DataStream SecTextureData;
        /// <summary>Флаг блокировки текстуры</summary>
        //bool lock_texture = false;
        /// <summary>Буфер текстуры</summary>
        Texture texture_buff; 
        VertexBuffer vertexBuffer = null;
        PresentParameters presentParams = new PresentParameters();
        public PictureBox Picture_Box;
        public PictureBox Picture_Max;
        byte[] tmp_buf;
        /// <summary>Флаг состояния рисования текущего кадра</summary>
        bool draw_frame = false;
        /// <summary>Ширина изображения</summary>
        public int Width;
        /// <summary>Высота изображения</summary>
        public int Height;
        /// <summary>Угол поворота изображения</summary>
        RotationAngle rotation_angle;
        bool HardwareVertex;
        float middle_point;
        static Thread workThread;
        /// <summary>Включение исправления искажений</summary>
        bool FUnFishEnable = false;
        /// <summary>Коэффициент исправления искажений</summary>
        int FUnFishEyeCoeff = 0;
        /// <summary>Размер сетки</summary>
        int GridSize = 12;
        /// <summary>Флаг завершения инициализации графики</summary>
        bool start;
        List<Line> lines;
        bool on_create_sphere = false;
        Font fnt;
        List<DrawString> captions;
        /// <summary>Флавг процесса инициализации</summary>
        bool init = true;
        /// <summary>Временный контрол захвата изображеня</summary>
        PictureBox tmp_picture;

        bool device_lost = true;
        object lock_obj = new object();
        static object _device_lostLock = new object();
        System.Windows.Forms.Timer _resizeTimer;
              
        int _resizeTimerTimeout;
        IGridSettingInfo _gridSettings;

        //bool re_init_on_resize = true;
        bool re_init_on_resize = true;

        #endregion

        /// <summary>Параметры формирования сетки для исправления искажений</summary>
        IGridSettingInfo GridSettigns
        {
            get
            {
                if (_gridSettings == null) _gridSettings = new DirectGridSetting();
                return _gridSettings;
            }
            set { _gridSettings = value; }
        }
        
        #region Structs

        struct DataToRender
        {
            public byte[] array;
            public int w;
            public int h;
        }

        #endregion

        #region Events

        public delegate void DelegateEventDeviceLost(bool lost);
        public event DelegateEventDeviceLost EventDeviceLost;

        #endregion

        object _lock1 = new object();
        object _lock2 = new object();

        string FontName = "Arial";

        object lock_debug = new object();

        double zoom = 1;
        /// <summary>Текущая центровка картинки по x от 0 до 1</summary>
        double location_x = 0.5d;
        /// <summary>Текущая центровка картинки по y от 0 до 1</summary>
        double location_y = 0.5d;

        #endregion

        /// <summary>Конструктор класса отрисовки изображений</summary>
        /// <param name="width">Ширина изображения</param>
        /// <param name="height">Высота изображения</param>
        /// <param name="picture_box">Элемент отисовки</param>
        /// <param name="hardware_vertex">Использовать аппаратную обработку вершин</param>
        /// <param name="un_fish">Задействовать исправления искажений</param>
        /// <param name="un_fish_coeff">Коэффициент исправления искажений</param>
        public SharpDXVideoRenderer_ex(int width, int height, 
                                    PictureBox picture_box, bool hardware_vertex, 
                                    bool un_fish, int un_fish_coeff)
        {
            Width = width;
            Height = height;
            HardwareVertex = hardware_vertex;
            Picture_Box = picture_box;
            tmp_buf = new byte[width * height * 3];
            FUnFishEnable = un_fish;
            FUnFishEyeCoeff = un_fish_coeff;
            //Таймер перерисовки
            Picture_Box.Resize += new EventHandler(PictureBoxResize);
           // CreateResizeTimer();
            //Инициализация графики DirectDraw
            direct3D = new Direct3D();
            start = InitializeGraphics(width, height, hardware_vertex);
        }

        /// <summary>Конструктор класса отрисовки изображений</summary>
        /// <param name="width">Ширина изображения</param>
        /// <param name="height">Высота изображения</param>
        /// <param name="picture_box">Элемент отисовки</param>
        /// <param name="picture_max">PictureBox увеличенного изображения</param>
        /// <param name="hardware_vertex">Использовать аппаратную обработку вершин</param>
        /// <param name="un_fish">Задействовать исправления искажений</param>
        /// <param name="un_fish_coeff">Коэффициент исправления искажений</param>
        public SharpDXVideoRenderer_ex(int width, int height,
                       PictureBox picture_box, PictureBox picture_max, bool hardware_vertex,
                       bool un_fish, int un_fish_coeff)
        {
            Width = width;
            Height = height;
            HardwareVertex = hardware_vertex;
            Picture_Box = picture_box;
            Picture_Max = picture_max;
            tmp_buf = new byte[width * height * 3];
            FUnFishEnable = un_fish;
            FUnFishEyeCoeff = un_fish_coeff;

            Picture_Box.Resize += new EventHandler(PictureBoxResize);
            //CreateResizeTimer();

            //Инициализация графики DirectDraw
            direct3D = new Direct3D();
            start = InitializeGraphics(width, height, hardware_vertex);
        }

        ///// <summary>Таймер переинициализации по завершении изменения размера</summary>
        //void CreateResizeTimer()
        //{
        //    _resizeTimer = new System.Windows.Forms.Timer();
        //    _resizeTimer.Interval = ResizeTimeTimeout;
        //    _resizeTimer.Enabled = false;
        //    _resizeTimer.Tick += new EventHandler(_resizeTimer_Tick);
        //}

      

        ///// <summary>Остановить таймер переинициализации</summary>
        //void StopResizeTimer()
        //{
        //    _resizeTimer.Stop();
        //    _resizeTimer.Enabled = false;
        //}

        protected void OnEventChangeInitState(object sender, bool init, bool dispose=false)
        {
            if (EventChangeInitState != null)
                EventChangeInitState(sender, init,dispose);
        }

        void StopResizeTimer()
        {
            if (_resizeTimer != null)
            {
                _resizeTimer.Stop();
                _resizeTimer.Tick -= _resizeTimer_Tick;
                _resizeTimer = null;
            }
        }

        void StartResizeTimer()
        {
            StopResizeTimer();
            _resizeTimer = new System.Windows.Forms.Timer();
            _resizeTimer.Interval = ResizeTimeTimeout;
            _resizeTimer.Enabled = false;
            _resizeTimer.Tick += new EventHandler(_resizeTimer_Tick);
            _resizeTimer.Start();

        }
        
        void _resizeTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!PrepareForRender()) return;

                _resizeTimer.Enabled = false;
                _resizeTimer.Stop();
                if ((re_init_on_resize || _resizeOnce) && !init) 
                { 
                    ReInitDevice(); 
                    _resizeOnce = false; 
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    string message = ex.Message;
                }
            }
        }

        /// <summary>Изменение размера окна</summary>
        void PictureBoxResize(object sender, EventArgs e)
        {
            //StopResizeTimer();
            //_resizeTimer.Interval = ResizeTimeTimeout;
            //_resizeTimer.Enabled = true;
            //_resizeTimer.Start();
           
            StartResizeTimer();
        }

        /// <summary>Direct3D устройство отрисовки</summary>
        Device Device { get { return device; } }
        
        /// <summary>Отдать новый PictureBox устройству отрисовки</summary>
        /// <param name="pb">PictureBox</param>
        public void SetPictureBox(PictureBox pb)
        {
            Picture_Box = pb;
            ReInitDevice();
        }

        public void SetPictureBox(PictureBox pb, int width, int height )
        {
            Picture_Box = pb;
            pb.Width = width;
            pb.Height = height;

            Width = width;
            Height = height;
            
            ReInitDevice();
        }
        
        /// <summary>Таймаут переинициализации устройства при изменении размера</summary>
        public int ResizeTimeTimeout
        {
            get
            {
                //if (_resizeTimerTimeout == 0) _resizeTimerTimeout = 500;
                if (_resizeTimerTimeout == 0) _resizeTimerTimeout = 1000;
                return _resizeTimerTimeout;
            }
            set { _resizeTimerTimeout = value; }
        }

        /// <summary>Переинициализировать при изменении размера (неободимо при перерисовке DrawTextureData)</summary>
        public bool ReInitOnResize
        {
            get { return re_init_on_resize; }
            set { re_init_on_resize = value; }
        }

        bool _resizeOnce;
        
        public void Resize()
        {
            _resizeOnce = true;
            StartResizeTimer();
        }

        #region Init graphics

        /// <summary>Переинициализация устройства</summary>
        void ReInitDevice()
        {
            try
            {
                InitializeGraphics(this.Width, this.Height, this.HardwareVertex);
            }
            catch
            {
                int err = 0;
            }
        }
     

        bool InitializeGraphics(int width, int height, bool hardware_vertex)
        {
            if (!init)
                return false;
               bool result = false;
           
               try
               {
                   //Уже в процессе инициализации
                   if (start && init) return false;

                   while (draw_frame) { Thread.Sleep(20); }

                   init = true;//Процесс инициализации начался
                   OnEventChangeInitState(this, init);

                   lock (_lock1)
                   {
                       StopResizeTimer();
                       int size = width * height * 4;  //Размер текстуры
                       if (BitmapData != null) BitmapData = null;
                       BitmapData = new byte[size];    //Битмэп данные кадров
                   }

                   //Удаление устройства если оно существует
                   if (device != null)
                   {
                       device.Dispose();
                       device = null;
                   }

                   //Открытие декодера

                   try { result = CreateDeviceThreadSafe(width, height, hardware_vertex); }
                   catch { return false; }

                   init = false;//Процесс инициализации завершился
                   //OnEventChangeInitState(this, init);
               }
               catch { int tr = 0; }
            return result;
        }
        object _fontLocker = new object();

        /// <summary>Потокобезопасное создание устройства Direct3D</summary>
        bool CreateDeviceThreadSafe(int width, int height, bool hardware_vertex)
        {
            if (Picture_Box.InvokeRequired)
            {
                CreateDeviceThreadSafeCallback d = new CreateDeviceThreadSafeCallback(CreateDeviceThreadSafe);
                object o = Picture_Box.Invoke(d, new object[] { width, height, hardware_vertex });
                return (bool)o;
            }
            else
            {
                lock (_lock2)
                {
                    presentParams.Windowed = true; //Оконный режим
                    presentParams.SwapEffect = SwapEffect.Discard; // Discard the frames 
                    presentParams.EnableAutoDepthStencil = true; // Turn on a Depth stencil
                    presentParams.AutoDepthStencilFormat = Format.D16; //DepthFormat.D16; // And the stencil format
                    presentParams.PresentFlags |= PresentFlags.LockableBackBuffer;
                    CreateFlags flag = CreateFlags.SoftwareVertexProcessing;
                    if (hardware_vertex) flag = CreateFlags.HardwareVertexProcessing;
                    //Пауза перед созданием
                    Thread.Sleep(100);
                    //Повторное создание
                    if (device != null)
                    {
                        if (!CheckDevice())
                        {
                            System.Threading.Thread.Sleep(1);
                            return false;
                        }
                    }
                    else
                    {
                        try
                        {
                            device = new Device(direct3D, 0, DeviceType.Hardware, Picture_Box.Handle, flag, presentParams);
                            //device.DeviceReset += new EventHandler(OnResetDevice);
                            //device.DeviceLost += new EventHandler(OnDeviceLost);
                            device.Disposing += new EventHandler<EventArgs>(device_Disposing);
                            OnCreateDevice(device, null);
                            OnResetDevice(device, null);
                            CreateFont(device);
                        }
                        catch
                        {
                            Dispose();
                            return false;
                        }
                    }

                    //Создание текстуры
                    if (texture != null)
                    {
                        texture.Dispose();
                        texture = null;
                    }
                    try
                    {
                        texture = new Texture(device, width, height, 1,
                                              Usage.AutoGenerateMipMap, Format.A8R8G8B8, Pool.Managed);
                    }
                    catch { return false; }

                    //Временный контрол захвата изображения
                    if (tmp_picture != null)
                    {
                        tmp_picture.Dispose();
                        tmp_picture = null;
                    }
                    tmp_picture = new PictureBox();
                    tmp_picture.Width = width;
                    tmp_picture.Height = height;
                    tmp_picture.Visible = false;

                    device.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf <CustomVertex.PositionNormalTextured>());
                    device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

                    device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                    device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                    device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Current);
                    device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);

                    device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
                    device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
                    device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Anisotropic);
                    //Возможно проблема.
                    OnEventChangeInitState(this, false);

                    return true;
                }
            }
        }

        /// <summary>Состояние процесса инициализации</summary>
        public bool Init { 
            get 
            {
                bool isInit = true;
                try { isInit = init; }
                catch { isInit = true; }
                return isInit;
            } 
        }
        
        void OnDeviceLost(object sender, EventArgs e)
        {
            lock (_device_lostLock) { device_lost = true; }
            if (EventDeviceLost != null)
                EventDeviceLost(device_lost);
        }

        void OnResetDevice(object sender, EventArgs e)
        {
            init = true;
            OnEventChangeInitState(this, init);
            Device dev = (Device)sender;
            dev.SetRenderState(RenderState.CullMode, Cull.None);
            dev.SetRenderState(RenderState.Lighting, false);
            dev.SetRenderState(RenderState.ZEnable, true);
            device_lost = false;
            if (EventDeviceLost != null) EventDeviceLost(device_lost);
        }

        int number_verts = 0;

        void OnCreateDevice(object sender, EventArgs e)
        {
            Device dev = (Device)sender;
            if (vertexBuffer != null) return;
            number_verts = (((GridSize * 2) + 2) * GridSize) - (GridSize - 1);
            vertexBuffer = new VertexBuffer(dev, Utilities.SizeOf<CustomVertex.PositionNormalTextured>() * number_verts,
                                            Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Managed);
            //vertexBuffer.Created += new System.EventHandler(this.OnCreateVertexBufferSphere);
            this.OnCreateVertexBufferSphere(vertexBuffer, null);
            //this.OnCreateVertexBuffer(vertexBuffer, null);
        }

        /// <summary>Фигура - плоскость</summary>
        void OnCreateVertexBuffer(object sender, EventArgs e)
        {
            VertexBuffer vb = (VertexBuffer)sender;
            //Блокировка буфера возвращающего структуру вершин
            using (DataStream gStream = vb.Lock(0, 0, LockFlags.None))
            {
                CustomVertex.PositionNormalTextured[] verts = new CustomVertex.PositionNormalTextured[4];
                    //(CustomVertex.PositionNormalTextured[])vb.Lock(0, 0);
                //------
                System.Drawing.Size displaySize = new System.Drawing.Size(640, 480);
                System.Drawing.Size vmrTexSize = new System.Drawing.Size(64, 64);     //Условный размер текстуры
                System.Drawing.Size vmrVidSize = new System.Drawing.Size(640, 480);

                RectangleF videoClientRectangle = Rectangle.Empty;

                if (vmrVidSize.Width >= vmrVidSize.Height)
                {
                    // Compute the video aspect-ratio for a landscape proportioned image
                    float videoAR = (float)vmrVidSize.Height / (float)vmrVidSize.Width;

                    videoClientRectangle.X = 0.0f;
                    videoClientRectangle.Width = (float)displaySize.Width;
                    videoClientRectangle.Height = (float)displaySize.Width * videoAR;
                    videoClientRectangle.Y = ((float)displaySize.Height - videoClientRectangle.Height) / 2;
                }
                else
                {
                    // Compute the video aspect-ratio for a portrait proportioned image
                    float videoAR = (float)vmrVidSize.Width / (float)vmrVidSize.Height;

                    videoClientRectangle.Y = 0.0f;
                    videoClientRectangle.Width = (float)displaySize.Height * videoAR;
                    videoClientRectangle.Height = (float)displaySize.Height;
                    videoClientRectangle.X = ((float)displaySize.Width - videoClientRectangle.Width) / 2;
                }

                // The Quad is built using a triangle fan of 2 triangles : 0,1,2 and 0, 2, 3
                // 0 *-------------------* 1
                //   |\                  |
                //   |   \               |
                //   |      \            |
                //   |         \         |
                //   |            \      |
                //   |               \   |
                //   |                  \|
                // 3 *-------------------* 2

                //Создание вершин
                verts[0].X = videoClientRectangle.X;
                verts[0].Y = videoClientRectangle.Y;
                verts[1].X = videoClientRectangle.Width;
                verts[1].Y = videoClientRectangle.Y;
                verts[2].X = videoClientRectangle.Width;
                verts[2].Y = videoClientRectangle.Height + videoClientRectangle.Y;
                verts[3].X = videoClientRectangle.X;
                verts[3].Y = videoClientRectangle.Height + videoClientRectangle.Y;
                // See the allocator source to see why the texture size is not necessary the video size
                verts[0].Tu = 0.0f;
                verts[0].Tv = 0.0f;
                verts[1].Tu = (float)vmrVidSize.Width / (float)vmrTexSize.Width;
                verts[1].Tv = 0.0f;
                verts[2].Tu = (float)vmrVidSize.Width / (float)vmrTexSize.Width;
                verts[2].Tv = (float)vmrVidSize.Height / (float)vmrTexSize.Height;
                verts[3].Tu = 0.0f;
                verts[3].Tv = (float)vmrVidSize.Height / (float)vmrTexSize.Height;
                gStream.WriteRange(verts);
                //Разблокировка буфера
                vb.Unlock();
                
            }
        }

        /// <summary>Фигура - шар</summary>
        public void OnCreateVertexBufferSphere(object sender, EventArgs e)
        {

            on_create_sphere = true;

            VertexBuffer vb = (VertexBuffer)sender;
            if (Object.ReferenceEquals(vb, null))
                return;//Просто негде рендерить. Как так поучается вопрос? Но иногда такое бывает.
            //Блокировка буфера возвращающего структуру вершин
            using (DataStream dStream = vb.Lock(0, 0, LockFlags.None))
            {
                CustomVertex.PositionNormalTextured[] verts = new CustomVertex.PositionNormalTextured[number_verts];
                    //dStream.ReadRange<CustomVertex.PositionNormalTextured>(number_verts);
                //------
                RectangleF videoClientRectangle = Rectangle.Empty;
                // Создание фигуры из n треугольников (TriangleStrip)

                #region Иллюстрация создания вершин треугольников

                //
                // 3 клетки
                //
                // ^ y
                // |
                // | 15----17----19----21  3    
                // |  |\    |\    |\    |
                // |  | \   | \   | \   |       rows
                // |  |  \  |  \  |  \  |       2
                // |  |   \ |   \ |   \ |
                // |  |    \|    \|    \|
                // | 14----16----18----20
                // | 14----12----10-----8  2
                // |  |    /|    /|    /|
                // |  |12 / |10 / |8  / |
                // |  |  /  |  /  |  /  |       1
                // |  | /11 | / 9 | /7  |
                // |  |/    |/    |/    |
                // | 13----11-----9-----7
                // |  1-----3-----5-----7  1
                // |  |\    |\    |\    |
                // |  | \ 2 | \ 4 | \ 6 |
                // |  |  \  |  \  |  \  |       0
                // |  | 1 \ | 3 \ | 5 \ |
                // |  |    \|    \|    \|
                // |  0-----2-----4-----6  0
                // |
                // |  0     1     2     3   
                // |     1     2     3  columns
                // 0-----------------------------> x
                //
                //  4 клетки
                //
                // ^ y
                // |
                // | 36----34----32----30----28
                // |  |    /|    /|    /|    /|
                // |  |12 / |10 / |8  / |   / |
                // |  |  /  |  /  |  /  |  /  |   1
                // |  | /11 | / 9 | /7  | /   |
                // |  |/    |/    |/    |/    |
                // | 35----33----31----29----27
                // | 19----21----23----25----27  3    
                // |  |\    |\    |\    |\    |
                // |  | \   | \   | \   | \   |  rows
                // |  |  \  |  \  |  \  |  \  |  2
                // |  |   \ |   \ |   \ |   \ |
                // |  |    \|    \|    \|    \|
                // | 18----20----22----24----26
                // | 18----16----14----12----10
                // |  |    /|    /|    /|    /|
                // |  |12 / |10 / |8  / |   / |
                // |  |  /  |  /  |  /  |  /  |   1
                // |  | /11 | / 9 | /7  | /   |
                // |  |/    |/    |/    |/    |
                // | 17----15-----13---11-----9
                // |  1-----3-----5-----7-----9
                // |  |\    |\    |\    |\    |
                // |  | \ 2 | \ 4 | \ 6 | \   |
                // |  |  \  |  \  |  \  |  \  |   0
                // |  | 1 \ | 3 \ | 5 \ |   \ |
                // |  |    \|    \|    \|    \|
                // |  0-----2-----4-----6-----8  0
                // |
                // |  0     1     2     3   
                // |     1     2     3  columns
                // 0-----------------------------> x


                #endregion

                //Размер сетки
                //double a = 64.0f;
                //double b = 48.0f;
                 double a = GridSettigns.a;
                 double b = GridSettigns.b;

                //Перевод градусов в радианы
                double corner = (Math.PI / 180) * FUnFishEyeCoeff;
                if (!FUnFishEnable) corner = (Math.PI / 180) * 0;

                double R = (a / 2) / Math.Sin(corner / 2);
                int VerticesUnderRow = ((GridSize * 2) + 2);

                double rx = a / 2.0d;
                double ry = b / 2.0d;

                double r = Math.Min(rx, ry);

                double correctedR = r * UnFishCoeff / 100.0d;

                double c = a / GridSize;
                double d = b / GridSize;

                //Создание координат вершин
                for (int i = 0; i < verts.Length; i++)
                {
                    int Y = 0;
                    int row_num = FindRow(i, GridSize);
                    if (row_num % 2 == 0 || row_num == 0) //Ряд четный или 0-й
                    {
                        //Снизу четные вершины, сверху нечетные
                        if (i % 2 == 0 || i == 0) Y = row_num; //Четные
                        else Y = row_num + 1;
                    }
                    else //Ряд нечетный
                    {
                        //Сверху четные вершины, снизу нечетные
                        if (i % 2 == 0 || i == 0) Y = row_num + 1;//Четные
                        else Y = row_num;
                    }
                    int X = FindX(row_num, i, GridSize);
                    //Координа X вершины
                    double x = X * c;
                    //Координа Y вершины
                    double y = Y * d;//c;

                    ////Координа X вершины
                    //x = X * Math.Asin(c / 2 * correctedR) * correctedR;
                    ////Координа Y вершины
                    //y = Y * Math.Asin(d / 2 * correctedR) * correctedR;//c
                   
                    //if (double.IsNaN(x) || double.IsInfinity(x))
                    //{
                    //    x = X * c; ;
                    //}

                    //if (double.IsNaN(b) || double.IsInfinity(b))
                    //{
                    //    y = Y * d;
                    //}


                    //z - вычисляется по формуле шара
                    double z = 0;
                    z = Math.Sqrt((R * R) - ((x - a / 2) * (x - a / 2)) - ((y - b / 2) * (y - b / 2)));

                    //z = Math.Sqrt((R * R) - ((x - a / 2) * (x - a / 2)) - ((y - a / 2) * (y - a / 2)));
                    if (System.Double.IsNaN(z) || System.Double.IsInfinity(z)) z = 0;
                    //z = -1 * z;//Инверсия
                    //u, v
                    double u = x / a;
                    double v = y / b;

                    verts[i].X = (float)x;
                    verts[i].Y = (float)y;
                    verts[i].Z = (float)z;
                    //Положение точки относительно левого верхнего угла от 0 до 1
                    verts[i].Tu = (float)u;
                    verts[i].Tv = (float)v;
                    if (a > b)
                    {
                        if (verts[i].Y == 0 && verts[i].X == (int)(a / 2))
                            middle_point = (float)z;
                        //middle_point = (float)100;
                    }
                    else
                    {
                        if (verts[i].X == 0 && verts[i].Y == (int)(b / 2))
                            middle_point = (float)z;
                    }

                }
                dStream.WriteRange(verts);
                //Разблокировка буфера
                vb.Unlock();
                on_create_sphere = false;
            }
        }

        /// <summary>Создание шрифта написания</summary>
        /// <param name="dev">Устройство Direct3D</param>
        void CreateFont(Device dev)
        {
            lock (_fontLocker)
            {
                if (fnt != null) fnt.Dispose();

                fnt = new Font(dev, 14, 6, FontWeight.Normal, 0,
                               false, FontCharacterSet.Default, FontPrecision.Default,
                               FontQuality.ClearType, FontPitchAndFamily.DontCare, FontName);
            }
        }

        #endregion

        #region Draw

        /// <summary>Отрисовка байтового массива в формате RGB24</summary>
        /// <param name="frame"></param>
        public bool DrawRGB24(byte[] frame)
        {
            bool res = CheckDeviceLost();
            if (res)
            {
                if (!PrepareForRender()) return false;
                if (draw_frame) return true;
                draw_frame = true;
                try { res = Render24(frame, Width, Height, true); }//Рисование кадра
                catch { draw_frame = false; return false; }
                draw_frame = false;
            }
            return res;
        }

        /// <summary>Отрисовка байтового массива в формате RGB24</summary>
        /// <param name="frame"></param>
        public bool DrawRGB32(byte[] frame)
        {
            bool res = CheckDeviceLost();
            if (res)
            {
                if (!PrepareForRender()) return false;
                if (draw_frame) return true;
                draw_frame = true;
                try { res = Render32(frame, Width, Height, true); }    //Рисование кадра
                catch { draw_frame = false; return false; }
                draw_frame = false;
            }
            return res;
        }
        
        /// <summary>Отрисовка внутренней текстуры</summary>
        /// <param name="frame"></param>
        public bool DrawTextureData()
        {
            bool res = CheckDeviceLost();
            if (res)
            {
                if (!PrepareForRender()) return false;
                if (draw_frame) return true;
                draw_frame = true;
                try { res = RenderInternalData(true); }     //Рисование кадра
                catch { draw_frame = false; return false; }
                draw_frame = false;
            }
            return res;
        }

        /// <summary>Отрисовка внутренней текстуры</summary>
        /// <param name="frame"></param>
        public bool DrawTextureDataEx()
        {
            bool res = CheckDeviceLost();
            if (res)
            {
                if (!PrepareForRender()) return false;
                if (draw_frame) return true;
                draw_frame = true;
                try { res = UpdateTexture(); }  //Рисование кадра
                catch { draw_frame = false; return false; }
                draw_frame = false;
            }
            return res;
        }

        /// <summary>Рисование массива по указателю</summary>
        /// <param name="frame">Указатель на массив кадра RGB24</param>
        public bool DrawPointer(IntPtr frame)
        {
            bool res = CheckDeviceLost();
            if (res)
            {
                if (!PrepareForRender()) return false;
                if (draw_frame) return true;
                draw_frame = true;
                try { res = Render(frame, Width, Height, true); } //Рисование кадра
                catch { }
                draw_frame = false;
            }
            return res;
        }

        /// <summary>Рисование кадра в формате текстуры Direct3D</summary>
        /// <param name="texture"></param>
        public bool DrawTexture(Texture texture)
        {
            texture_buff = texture;
            bool res = CheckDeviceLost();
            if (res)
            {
                if (!PrepareForRender()) return false;
                if (draw_frame) return true;
                draw_frame = true;
                try { res = RenderTexture(texture, true); }
                catch { }
                draw_frame = false;
            }
            return res;
        }

        /// <summary>Потокобезопасная отрисовка 24-битного изображения (низкая производительность)</summary>
        /// <param name="array">Массив изображения 24 бит</param>
        /// <param name="w">Ширина изображения</param>
        /// <param name="h">Высота изображения</param>
        public void DrawFrameThreadSafe(byte[] array, int w, int h)
        {
            if (Picture_Box.InvokeRequired)
            {
                LogMessageCallbackDrawFrame d = new LogMessageCallbackDrawFrame(DrawFrameThreadSafe);
                Picture_Box.Invoke(d, new object[] { array, w, h });
            }
            else
            {   
                Render24(array, w, h, true);
            }
        }

        /// <summary>Проверка потерянного устройства</summary>
        /// <returns>true - устройство не потеряно</returns>
        public bool CheckDeviceLost()
        {
            //Потеряно устройство
            if (device_lost)
            {
                InitializeGraphics(Width, Height, HardwareVertex);
                return false;
            }
            return true;
        }

        /// <summary>Перерисовка текстура из буфера</summary>
        public void ReDrawTexture()
        {
            RenderTexture(texture_buff, true);
        }

        /// <summary>Отрисовка на Direct3D устройстве 24-битный битмап данных</summary>
        /// <param name="data">Bitmap данные</param>
        /// <param name="width">ширина</param>
        /// <param name="height">высота</param>
        /// <param name="tk">номер телекамеры с 0</param>
        bool Render24(byte[] data, int width, int height, bool graphics_elements)
        {
            if (!PrepareForRender()) return false;
            //Запрос устройства
            Device dev = device;
            if (GetDeviceForRender(dev))
            {
                if (!BeginScene(dev)) return false; //Начало отрисовки
                SetupMatrices(dev);                 //Установка матриц проекций
                Texture texture_n = null;
                if (Copy24bitArray(data, width, height, ref texture_n, texture))
                    return SetTexture(dev, texture_n, graphics_elements); //Отрисовать текстуру
            }
            return false;
        }

        /// <summary>Отрисовка на Direct3D устройстве битмап 32 бита данных</summary>
        /// <param name="data">Bitmap данные</param>
        /// <param name="width">ширина</param>
        /// <param name="height">высота</param>
        /// <param name="tk">номер телекамеры с 0</param>
        bool Render32(byte[] data, int width, int height, bool graphics_elements)
        {
            if (!PrepareForRender()) return false;
            //Запрос устройства
            Device dev = device;
            if (GetDeviceForRender(dev))
            {
                if (!BeginScene(dev)) return false; //Начало отрисовки
                SetupMatrices(device);              //Установка матриц проекций
                Texture texture_n = null;
                if (Copy32bitArray(data, width, height, ref texture_n, texture))
                    return SetTexture(dev, texture_n, graphics_elements); //Отрисовать текстуру
            }
            return true;
        }

        /// <summary>Отрисовка на Direct3D устройстве 24-битный битмап данных</summary>
        /// <param name="data">Указатель на данные 24-бит</param>
        /// <param name="width">ширина</param>
        /// <param name="height">высота</param>
        /// <param name="tk">номер телекамеры с 0</param>
        bool Render(IntPtr data, int width, int height, bool graphics_elements)
        {
            if (!PrepareForRender()) return false;
            //Запрос устройства
            Device dev = device;
            if (GetDeviceForRender(dev))
            {
                if (!BeginScene(dev)) return false; //Начало отрисовки
                SetupMatrices(device);              //Установка матриц проекций
                Texture texture_n = null;
                if (Copy24bitArray(data, width, height, ref texture_n, texture))
                    return SetTexture(dev, texture_n, graphics_elements); //Отрисовать текстуру
            }
            return false;
        }

        /// <summary>Отрисовка готовой текстуры</summary>
        /// <param name="texture">Текстура</param>
        bool RenderTexture(Texture texture, bool graphics_elements)
        {
            if (!PrepareForRender()) return false;
            //Запрос устройства
            Device dev = device;
            if (GetDeviceForRender(dev))
            {
                if (!BeginScene(dev)) return false; //Начало отрисовки
                SetupMatrices(device);
                return SetTexture(dev, texture, graphics_elements); //Отрисовать текстуру
            }
            return false;
        }

        /// <summary>Отрисовка данных текстуры</summary>
        /// <param name="graphics_elements">Рисовать графические элементы</param>
        /// <returns>Результат операции</returns>
        bool RenderInternalData(bool graphics_elements)
        {
            if (!PrepareForRender()) return false;
            //Запрос устройства
            Device dev = device;
            if (GetDeviceForRender(dev))
            {
                if (!BeginScene(dev)) return false; //Начало отрисовки
                SetupMatrices(device);//Установка матриц проекций
                return SetTexture(dev, texture, graphics_elements); //Отрисовать текстуру
            }
            return false;
        }

        /// <summary>Отрисовка данных текстуры обновлением текстур</summary>
        /// <param name="graphics_elements">Рисовать графические элементы</param>
        /// <returns>Результат операции</returns>
        bool RenderInternalDataEx(bool graphics_elements)
        {
            if (!PrepareForRender()) return false;
            //Запрос устройства
            Device dev = device;
            if (GetDeviceForRender(dev))
            {
                if (!BeginScene(dev)) return false; //Начало отрисовки
                SetupMatrices(dev);//Установка матриц проекций
                try
                {
                    dev.UpdateTexture(second_texture, texture);
                    dev.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                    dev.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                    dev.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Current);
                    dev.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
                    dev.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
                    dev.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
                    dev.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Anisotropic);
                    dev.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<CustomVertex.PositionNormalTextured>());
                    dev.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                    int triangles_count = GridSize * 2 * GridSize + GridSize - 1;
                    dev.DrawPrimitives(PrimitiveType.TriangleStrip, 0, triangles_count);//Sphere
                    if (graphics_elements)
                    {
                        RenderText(dev);    //Отрисовка строк
                        RenderLines(dev);   //Отрисовка линий
                    }
                    //End the scene
                    dev.EndScene();
                }
                catch
                {
                    dev.EndScene();
                    dev.Present();
                    return false;
                }
                // Update the screen
                try { dev.Present(); }
                catch { return false; };
                return true;
            }
            return false;
        }

        /// <summary>Подготовка к отрисовке</summary>
        /// <returns>Результат подготовки</returns>
        bool PrepareForRender()
        {
            if (init) return false;  //Идет процесс инициализации графики
            if (on_create_sphere) return false;//Если продолжается процесс создания вершин - выход
            if (!start) return false;
            return true;
        }

        /// <summary>Проверка инициализации устройства при рисовании</summary>
        /// <returns>Результат операции</returns>
        bool GetDeviceForRender(Device dev)
        {
            if (dev == null)
            {
                InitializeGraphics(Width, Height, HardwareVertex);
                return false;
            }
            try
            {
                dev.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            }
            catch
            {
                InitializeGraphics(Width, Height, HardwareVertex);
                return false;
            }

            if (!CheckDevice())
            {
                System.Threading.Thread.Sleep(20);
                OnResetDevice(device, EventArgs.Empty);
                return false;
            }
            return true;
        }

        /// <summary>Начало отрисовки</summary>
        /// <param name="dev">Устройство</param>
        /// <returns>Результат операции</returns>
        bool BeginScene(Device dev)
        {
            if (dev == null) return false;
            //Rendering
            try { dev.BeginScene(); } //Begin the scene (здесь иногда падает если незавершен рендеринг)
            catch
            {
                dev.EndScene();
                dev.Present();
                dev.EvictManagedResources();
                return false;
            }
            return true;
        }

        /// <summary>Отрисовать текстуру</summary>
        /// <param name="dev">Устройство</param>
        /// <param name="texture_n">Текстура</param>
        /// <param name="graphics_elements">Рисовать грифические элементы</param>
        /// <returns>Результат операции</returns>
        bool SetTexture(Device dev, Texture texture_n, bool graphics_elements)
        {
            try
            {
                dev.SetTexture(0, texture_n);

                dev.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                dev.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                dev.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Current);
                dev.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
                dev.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
                dev.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
                dev.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Anisotropic);
                dev.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<CustomVertex.PositionNormalTextured>());
                dev.VertexFormat = CustomVertex.PositionNormalTextured.Format;

                int triangles_count = GridSize * 2 * GridSize + GridSize - 1;
                dev.DrawPrimitives(PrimitiveType.TriangleStrip, 0, triangles_count);
                if (graphics_elements)
                {
                    RenderText(dev);    //Отрисовка строк
                    RenderLines(dev);   //Отрисовка линий
                }
                dev.EndScene();
            }
            catch
            {
                dev.EndScene();
                dev.Present();
                dev.EvictManagedResources();
                return false;
            }
            try { dev.Present(); }
            catch { return false; };
            return true;
        }

        /// <summary>Копирование массива байт 24 бита в текстуру</summary>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        /// <param name="data">Массив данных</param>
        /// <returns></returns>
        bool Copy24bitArray(byte[] data, int width, int height, ref Texture texture_n, Texture texture)
        {
            texture_n = texture;        //Присваиваем эталонную текстуру
            //Заполнение текстуры битмэпом
            DataStream textureData;
            try
            {
                texture_n.LockRectangle(0, LockFlags.None, out textureData);
                IppiSize size;
                size.width = width;
                size.height = height;
                if (data.Length / (3.0 * width) < height)
                    return false;
                try { IppFunctions.ippiCopy_8u_C3AC4R(data, width * 3, textureData.DataPointer, (width) * 4, size); }
                catch { return false; }
            }
            catch { }
            finally
            {
                try
                {
                    texture_n.UnlockRectangle(0);
                }
                catch { }
            }
            return true;
        }
        
        /// <summary>Копирование массива байт 24 бита в текстуру</summary>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        /// <param name="data">Массив данных</param>
        /// <returns></returns>
        bool Copy24bitArray(IntPtr data, int width, int height, ref Texture texture_n, Texture texture)
        {
            try
            {
                texture_n = texture;        //Присваиваем эталонную текстуру
                //Заполнение текстуры битмэпом
                DataStream textureData;
                texture_n.LockRectangle(0, LockFlags.None, out textureData);
                IppiSize size;
                size.width = width;
                size.height = height;
                try { IppFunctions.ippiCopy_8u_C3AC4R(data, width * 3, textureData.DataPointer, (width) * 4, size); }
                catch { return false; }
            }
            catch { }
            finally
            {
                try { texture_n.UnlockRectangle(0); }
                catch { }
            }
            return true;
        }
        
        /// <summary>Копирование массива байт 32 бита в текстуру</summary>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        /// <param name="data">Массив данных</param>
        /// <returns></returns>
        bool Copy32bitArray(byte[] data, int width, int height, ref Texture texture_n, Texture texture)
        {
            texture_n = texture;        //Присваиваем эталонную текстуру
            try
            {
                //Заполнение текстуры битмэпом
                DataStream textureData;
                texture_n.LockRectangle(0, LockFlags.None, out textureData);
                IppiSize size;
                size.width = width;
                size.height = height;
                try { IppFunctions.ippiCopy_8u_C4R(data, width * 4, textureData.DataPointer, (width) * 4, size); }
                catch { return false; }
              
            }
            catch { }
            finally
            {
                try { texture_n.UnlockRectangle(0); }
                catch { }
            }
            return true;
        }

        /// <summary>Обновление текстуры текущего устройства</summary>
        /// <returns>Результат операции</returns>
        bool UpdateTexture()
        {
            Device dev = device;
            dev.UpdateTexture(second_texture, texture);
            return true;
        }

        #region Отрисовка графических элементов

        void RenderLines(Device dev)
        {
            if (lines == null) return;
            for (int i = 0; i < lines.Count; i++) RenderLine(dev, lines[i]);
        }

        /// <summary>Отрисовка линии на устройстве</summary>
        /// <param name="dev">Устройство Direct3D</param>
        /// <param name="line">Линия</param>
        void RenderLine(Device dev, Line line)
        {
            CustomVertex.TransformedColored[] vertexes = new CustomVertex.TransformedColored[2];
            int color = System.Drawing.Color.FromArgb(line.color).ToArgb();
            vertexes[0].Position = new Vector4(line.X1, line.Y1, 0, 1.0f);
            vertexes[0].Color = color;
            vertexes[1].Position = new Vector4(line.X2, line.Y2, 0, 1.0f);
            vertexes[1].Color = color;
            dev.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
            dev.VertexFormat = CustomVertex.TransformedColored.Format;
            dev.DrawUserPrimitives(PrimitiveType.LineList, 1, vertexes);
        }

        void RenderText(Device dev)
        {
            if (captions == null) return;
            for (int i = 0; i < captions.Count; i++) RenderString(dev, captions[i]);
        }

        void RenderString(Device dev, DrawString s)
        {
            try
            {
                lock (_fontLocker)
                {
                    if (!fnt.IsDisposed/* && !PrepareForRender()*/)
                        fnt.DrawText(null, s.Str, s.X, s.Y, new Color(s.Color));
                }
            }
            catch { }
        }

        #endregion

        /// <summary>Указатель на данные текстуры, </summary>
        public IntPtr TextureData
        {
            get
            {
                try
                {
                    texture.LockRectangle(0, LockFlags.None, out  textureData);
                    return textureData.DataPointer;
                }
                catch
                {
                    //Сбой получения текстуры - переинициализировать графику и текстуру
                    InitializeGraphics(Width, Height, HardwareVertex);
                    return IntPtr.Zero;
                }
            }
        }

        public IntPtr SecondTextureData
        {
            get { return SecTextureData.DataPointer; }
        }

        /// <summary>Разблокировка текстуры</summary>
        public void Unlock()
        {
            texture.UnlockRectangle(0);
        }

        #endregion

        #region Графические элементы

        /// <summary>Добавление линии на экран</summary>
        /// <param name="line">Структура линии</param>
        public void AddLine(Line line)
        {
            if (lines == null) lines = new List<Line>();
            lines.Add(line);
        }

        /// <summary>Добавление прямоугольника на экран</summary>
        /// <param name="line">Структура прямоугольника</param>
        public void AddRectangle(int color, Rectangle rect)
        {
            AddLine(new Line(color, rect.X, rect.Y, rect.Right, rect.Y));
            AddLine(new Line(color, rect.Right, rect.Y, rect.Right, rect.Bottom));
            AddLine(new Line(color, rect.Right, rect.Bottom, rect.X, rect.Bottom));
            AddLine(new Line(color, rect.X, rect.Bottom, rect.X, rect.Y));
        }

        /// <summary>Добавление прямоугольника на экран</summary>
        /// <param name="line">Структура прямоугольника</param>
        public void AddRectangle(int color, System.Drawing.Rectangle rect)
        {
            AddLine(new Line(color, rect.X, rect.Y, rect.Right, rect.Y));
            AddLine(new Line(color, rect.Right, rect.Y, rect.Right, rect.Bottom));
            AddLine(new Line(color, rect.Right, rect.Bottom, rect.X, rect.Bottom));
            AddLine(new Line(color, rect.X, rect.Bottom, rect.X, rect.Y));
        }

        /// <summary>Очистка всех линий на экране</summary>
        public void ClearLines()
        {
            if (lines == null) return;
            lines.Clear();
            lines = null;
        }

        /// <summary>Добавление строки на экран</summary>
        /// <param name="s">МСтруктура строки для добавления</param>
        public void AddString(DrawString s)
        {
            if (captions == null) captions = new List<DrawString>();
            captions.Add(s);
        }

        /// <summary>Очистка всех строк на экране</summary>
        public void ClearStrings()
        {
            if (captions == null) return;
            captions.Clear();
            captions = null;
        }

        /// <summary>Установить название шрифта</summary>
        /// <param name="name"></param>
        public void SetFontName(string name)
        {
            FontName = name;
        }

        #endregion
        
        /// <summary>Матрицы перемещений</summary>
        void SetupMatrices(Device device)
        {
            //Отступ точки взгляда для вписывания изображения в экран
            float VEz =  middle_point - (float)Math.Min(GridSettigns.b,GridSettigns.a)/2.0f + 0.3f; // 24.0f + 0.3f;
            //Вектор поворота угла обзора
            Vector3 vector_rotate = new Vector3(0.0f, 1.0f, 0.0f);
            //Поворот
            if (rotation_angle == RotationAngle.deg_0) vector_rotate = new Vector3(0.0f, 1.0f, 0.0f);
            if (rotation_angle == RotationAngle.deg_90) vector_rotate = new Vector3(1.0f, 0.0f, 0.0f);
            if (rotation_angle == RotationAngle.deg_180) vector_rotate = new Vector3(0.0f, -1.0f, 0.0f);
            if (rotation_angle == RotationAngle.deg_270) vector_rotate = new Vector3(-1.0f, 0.0f, 0.0f);

            float loc_x = (float)GridSettigns.a / 2.0f; //32.0f;
            float loc_y = (float)GridSettigns.b / 2.0f;   //24.0f;//32.0f;

            if (zoom > 1.0d && location_x != 0.5d && location_y != 0.5d)
            {
                loc_x = (float)GridSettigns.a * (float)location_x;
                loc_y = (float)GridSettigns.b * (float)location_y;
            }
            device.SetTransform(TransformState.View, Matrix.LookAtLH(new Vector3(loc_x, loc_y, VEz), //Eye
                                                   new Vector3((float)(GridSettigns.a/2.0f), (float)(GridSettigns.b/2.0f)/*(32.0f)*/, (float)(100000.0f)),  //Цель
                                                   vector_rotate)); //Up
            float fieldOfViewY = (float)Math.PI / 2.0f;
            if (zoom > 0) fieldOfViewY = ((float)Math.PI /2.0f) / (float)zoom;
            device.SetTransform(TransformState.Projection, Matrix.PerspectiveFovLH(fieldOfViewY, 4.0f / 3.0f, 1.0f, 1000.0f));
        }

        public void Zoom(int delta, double x, double y)
        {
            if (delta < 0) zoom += 0.1f;
            if (delta > 0) zoom -= 0.1f;
            if (zoom < 1) zoom = 1;
        }

        public void Move(double x1, double y1, double x2, double y2)
        {
            location_x = location_x + ((x2 - x1) / zoom);
            location_y = location_y + ((y2 - y1) / zoom);
        }

        /// <summary>Угол поворота изображения</summary>
        public RotationAngle Rotation
        {
            get { return rotation_angle; }
            set 
            { 
                rotation_angle = value;
                lock (GridSettigns) { GridSettigns = new DirectGridSetting(); }
                if (rotation_angle == RotationAngle.deg_90 || rotation_angle == RotationAngle.deg_270)
                {
                    lock (GridSettigns) { GridSettigns = new RotationGridSetting(); }
                }
                OnCreateVertexBufferSphere(this.vertexBuffer, EventArgs.Empty);
            }
        }

        #region GetBitmap

        /// <summary>Получение System.Drawing.Bitmap</summary>
        public System.Drawing.Bitmap GetBitmapImage()
        {
            int w = Width;
            int h = Height;
            byte[] bmp = new byte[w * h * 3];

            //Заголовок
            BITMAPINFOHEADER bmih = new BITMAPINFOHEADER();
            bmih.biSize = 40;
            bmih.biWidth = w;
            bmih.biHeight = h;
            bmih.biPlanes = 1;
            bmih.biBitCount = 24;
            bmih.biCompression = 0;
            bmih.biSizeImage = w * h * 3;
            bmih.biXPelsPerMeter = 100;
            bmih.biYPelsPerMeter = 100;
            bmih.biClrUsed = 0;
            bmih.biClrImportant = 0;
            byte[] bmi_header = new byte[40];
            bmi_header = ClassByteUtils.StructureToByteArray(bmih);
            //Заголовок файла
            byte[] fileheader = new byte[14];
            fileheader[0] = 0x42;
            fileheader[1] = 0x4D;
            fileheader[2] = 0x46;
            fileheader[3] = 0x1C;
            fileheader[4] = 0x03;
            fileheader[5] = 0x0;
            fileheader[6] = 0x0;
            fileheader[7] = 0x0;
            fileheader[8] = 0x0;
            fileheader[8] = 0x0;
            fileheader[9] = 0x0;
            fileheader[10] = 0x36;
            fileheader[11] = 0x0;
            fileheader[12] = 0x0;
            fileheader[13] = 0x0;

            int size = w * h * 3;
            //Создание потока в памяти
            System.IO.MemoryStream ms = new System.IO.MemoryStream(fileheader.Length + bmi_header.Length + size);
            ms.Write(fileheader, 0, 14);
            ms.Write(bmi_header, 0, 40);
            ms.Write(bmp, 0, size);

            //Создание битмэпа из потока в памяти
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(ms);
            return bitmap;
        }

        /// <summary>Получение картинки отрисованной с заданными параметрами</summary>
        /// <param name="data">Несжатый массив кадра в RGB24</param>
        /// <returns>Текущее изображение</returns>
        public System.Drawing.Bitmap GetScene(byte[] data)
        {
            //Создание временного устройства отрисовки
            PictureBox picture = new PictureBox();
            picture.Width = Width;
            picture.Height = Height;
            picture.Visible = false;

            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true; //Оконный режим
            presentParams.SwapEffect = SwapEffect.Discard; // Discard the frames 
            presentParams.EnableAutoDepthStencil = true; // Turn on a Depth stencil
            presentParams.AutoDepthStencilFormat = Format.D16; //DepthFormat.D16; // And the stencil format


            //Создание временного устройства Direct3D
            Device dx = new Device(direct3D, 0, DeviceType.Hardware, picture.Handle, 
                                   CreateFlags.SoftwareVertexProcessing, presentParams);
            dx.SetRenderState(RenderState.CullMode, Cull.None);
            dx.SetRenderState(RenderState.Lighting, false);
            dx.SetRenderState(RenderState.ZEnable, true);

            //Присваиваем эталонную текстуру
            Texture texture_n = new Texture(dx, Width + 1, Height + 1, 1,
                                            Usage.AutoGenerateMipMap, Format.A8R8G8B8, Pool.Managed);
            //Заполнение текстуры битмэпом
            //GraphicsStream textureData = texture_n.LockRectangle(0, LockFlags.None, out pitch);
            DataStream textureData;
            texture_n.LockRectangle(0, LockFlags.None, out textureData);

            IppiSize size;
            size.width = Width;
            size.height = Height;
            IppFunctions.ippiCopy_8u_C3AC4R(data, Width * 3, textureData.DataPointer, (Width + 1) * 4, size);
            texture_n.UnlockRectangle(0);
            try
            {
                dx.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Aqua, 1.0f, 0);
                dx.BeginScene();
                SetupMatrices(dx);//Установка матриц проекций
                dx.SetTexture(0, texture_n);
                dx.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                dx.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                dx.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Current);
                dx.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
                dx.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
                dx.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
                dx.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
                dx.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<CustomVertex.PositionNormalTextured>());
                dx.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                int triangles_count = GridSize * 2 * GridSize + GridSize - 1;
                dx.DrawPrimitives(PrimitiveType.TriangleStrip, 0, triangles_count);
                dx.EndScene();
            }
            catch
            {
                dx.EndScene();
                dx.Present();
                dx.Dispose();
                return null;
            }
            Surface renderTarget = dx.GetRenderTarget(0);
            DataStream gs = Surface.ToStream(renderTarget, ImageFileFormat.Bmp);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(gs);
            //Клонирование картинки
            System.Drawing.Bitmap new_bmp = new System.Drawing.Bitmap(bmp);

            //Очищение объектов
            renderTarget.Dispose();
            renderTarget = null;
            dx.Dispose();
            dx = null;
            picture.Dispose();
            picture = null;
            presentParams  = new PresentParameters();
            texture_n.Dispose();
            texture_n = null;
            gs.Close();
            gs.Dispose();
            gs = null;

            return new_bmp;
        }

        /// <summary>Получение картинки отрисованной с заданными параметрами</summary>
        /// <returns>Текущее изображение</returns>
        public System.Drawing.Bitmap GetTextureScene()
        {
            IppiSize size;
            size.width = Width;
            size.height = Height;
            byte[] data = new byte[Width * Height * 4];
            try
            {
                //Блокировка текстуры для захвата ее содержимого
                lock (lock_obj)
                {
                    DataRectangle gs = texture.LockRectangle(0,LockFlags.None);
                    IppFunctions.ippiCopy_8u_C4R(gs.DataPointer, Width * 4, data, Width * 4, size);
                    texture.UnlockRectangle(0);
                }
            }
            catch { };
            //Unlock();
            return new System.Drawing.Bitmap(GetBitmapStream(data));
            //return new Bitmap(bmp);//Клонирование картинки
        }

        /// <summary>Получение массива картинки текущей текстуры</summary>
        /// <returns></returns>
        public byte[] GetTextureData()
        {
            IppiSize size;
            size.width = Width;
            size.height = Height;
            byte[] data = new byte[Width * Height * 4];
            try
            {
                //Блокировка текстуры для захвата ее содержимого
                lock (lock_obj)
                {
                    DataRectangle gs = texture.LockRectangle(0, LockFlags.None);
                    IppFunctions.ippiCopy_8u_C4R(gs.DataPointer, Width * 4, data, Width * 4, size);
                    texture.UnlockRectangle(0);
                }
            }
            catch { };
            return data;
        }

        /// <summary>Получение картинки отрисованной с заданными параметрами</summary>
        /// <param name="data">Несжатый массив кадра в RGB24</param>
        /// <returns>Текущее изображение</returns>
        public System.Drawing.Bitmap GetTextureScene(int unfishRatio)
        {
            //Заполнение текстуры битмэпом
            IppiSize size;
            size.width = Width;
            size.height = Height;
            byte[] data = new byte[Width * Height * 4];
            try
            {
                //Блокировка текстуры для захвата ее содержимого
                lock (lock_obj)
                {
                    DataRectangle gs = texture.LockRectangle(0, LockFlags.None);

                    IppFunctions.ippiCopy_8u_C4R(gs.DataPointer, Width * 4, data, Width * 4, size);
                    texture.UnlockRectangle(0);
                }
                byte[] unfishSource = new byte[Width * Height * 3];
                int j = 0;
                //Перевести изображение из 32 бит в 24 бита
                for (int i = 0; i < data.Length;i++ )
                {
                    if ((i+1) % 4 == 0 && i > 0)
                        continue;
                    if (j < (unfishSource.Length - 1))
                        unfishSource[j++] = data[i];
                }

                float[,] xMap=null, yMap=null;
                byte[] newData = new byte[Width * Height * 4];
                IppFunctions.CreateUnFishMap24bit(unfishRatio/100.0f/*unfishRatio*/, Width, Height, out xMap, out yMap);
                byte[] outputArray = IppFunctions.AntiFishEye24bit(unfishSource, Width, Height, xMap, yMap);
                GCHandle dataPinned = GCHandle.Alloc(newData, GCHandleType.Pinned);
                IntPtr unmanageData = dataPinned.AddrOfPinnedObject();
                //Marshal.Copy(data, 0, unmanageData, data.Length);
                IppFunctions.ippiCopy_8u_C3AC4R(outputArray, Width * 3, unmanageData, Width * 4, size);
                Marshal.Copy(unmanageData, newData, 0, data.Length);
                data = null;
                data = newData;
                dataPinned.Free();
            }
            catch { };
            //Unlock();
            return new System.Drawing.Bitmap(GetBitmapStream(data));
            //return new Bitmap(bmp);//Клонирование картинки
        }

        /// <summary>Получение картинки отрисованной с заданными параметрами</summary>
        /// <param name="data">Несжатый массив кадра в RGB24</param>
        /// <returns>Текущее изображение</returns>
        public System.Drawing.Bitmap GetSurfaceScene(int unfishRatio)
        {
            OnCreateVertexBuffer(vertexBuffer, null);
            OnCreateVertexBufferSphere(this.vertexBuffer, EventArgs.Empty);
            return GetTextureScene(unfishRatio);
            ////Заполнение текстуры битмэпом
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height);
            //System.Drawing.Bitmap new_bmp = new System.Drawing.Bitmap(Width, Height);
            //try
            //{
            //    Surface renderTarget = device.GetRenderTarget(0);
            //    DataStream gs = Surface.ToStream(renderTarget, ImageFileFormat.Bmp);
            //    bmp = new System.Drawing.Bitmap(gs);
            //    //Клонирование картинки
            //    new_bmp = new System.Drawing.Bitmap(bmp);
            //    //Очищение объектов
            //}
            //catch { };
            ////Unlock();
            //return new_bmp;

            //IppiSize size;
            //size.width = Width;
            //size.height = Height;
            //byte[] data = new byte[Width * Height * 4];
            //try
            //{
            //    //Блокировка текстуры для захвата ее содержимого
            //    lock (lock_obj)
            //    {
            //        DataRectangle gs = texture.LockRectangle(0, LockFlags.None);

            //        IppFunctions.ippiCopy_8u_C4R(gs.DataPointer, Width * 4, data, Width * 4, size);
            //        texture.UnlockRectangle(0);
            //    }
            //}
            //catch { };
            //byte[] restData = new byte[3 * Width * Height];
            //int j=0;
            //for (int i = 0; i < data.Length; i++)
            //{
            //   //if (i % 4 == 0 && i > 0)
            //   if ((i+1) % 4 == 0)
            //        continue;
            //    restData[j++] = data[i];
            //}

            ////Unlock();
            //return GetScene(restData);

         }

        /// <summary>Получение System.Drawing.Bitmap</summary>
        public System.IO.MemoryStream GetBitmapStream(byte[] bitmap)
        {
            BITMAPINFOHEADER bmih = new BITMAPINFOHEADER();
            bmih.biSize = 40;
            bmih.biWidth = Width;
            bmih.biHeight = Height;
            bmih.biPlanes = 1;
            bmih.biBitCount = 32;
            bmih.biCompression = 0;
            bmih.biSizeImage = Width * Height * 4;
            bmih.biXPelsPerMeter = 100;
            bmih.biYPelsPerMeter = 100;
            bmih.biClrUsed = 0;
            bmih.biClrImportant = 0;
            byte[] bmi_header = new byte[40];
            bmi_header = ClassByteUtils.StructureToByteArray(bmih);
            //Заголовок файла
            byte[] fileheader = new byte[14];
            fileheader[0] = 0x42;
            fileheader[1] = 0x4D;
            fileheader[2] = 0x46;
            fileheader[3] = 0x1C;
            fileheader[4] = 0x03;
            fileheader[5] = 0x0;
            fileheader[6] = 0x0;
            fileheader[7] = 0x0;
            fileheader[8] = 0x0;
            fileheader[8] = 0x0;
            fileheader[9] = 0x0;
            fileheader[10] = 0x36;
            fileheader[11] = 0x0;
            fileheader[12] = 0x0;
            fileheader[13] = 0x0;

            int size = Width * Height * 4;
            //Создание потока в памяти
            System.IO.MemoryStream ms = new System.IO.MemoryStream(fileheader.Length + bmi_header.Length + size);
            ms.Write(fileheader, 0, 14);
            ms.Write(bmi_header, 0, 40);
            ms.Write(bitmap, 0, size);

            return ms;
        }

        /// <summary>Получение текущей отрисованной картинки</summary>
        /// <returns>Текущее изображение</returns>
        public System.Drawing.Bitmap GetCurrentScene()
        {
            Surface renderTarget = device.GetRenderTarget(0);
            DataStream gs = Surface.ToStream(renderTarget, ImageFileFormat.Bmp);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(gs);
            //Очищение объектов
            renderTarget.Dispose();
            gs.Close();
            gs.Dispose();
            gs = null;
            return bmp;
        }

        /// <summary>Получение массива текущего рисуемого изображения 
        /// (с исправленными искажениями)</summary>
        /// <returns>Текущее изображение</returns>
        public byte[] GetDrawingBuffer8bpp(byte[] data)
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
            presentParams.EnableAutoDepthStencil = true;
            presentParams.AutoDepthStencilFormat = Format.D16; //DepthFormat.D16;

            //Создание временного устройства Direct3D
            Device dx = new Device(direct3D, 0, DeviceType.Hardware, tmp_picture.Handle,
                                   CreateFlags.SoftwareVertexProcessing, presentParams);
            
            dx.SetRenderState(RenderState.CullMode, Cull.None);
            dx.SetRenderState(RenderState.Lighting, false);
            dx.SetRenderState(RenderState.ZEnable, true);

            //Создание текстуры
            Texture texture_n = new Texture(dx, Width + 1, Height + 1, 1,
                                            Usage.AutoGenerateMipMap, Format.A8R8G8B8, Pool.Managed);

            //int pitch;
            //Заполнение текстуры битмэпом
            //GraphicsStream textureData = texture_n.LockRectangle(0, LockFlags.None, out pitch);
            DataStream textureData;
            texture_n.LockRectangle(0, LockFlags.None, out textureData);
            IppiSize size;
            size.width = Width;
            size.height = Height;
            IppFunctions.ippiCopy_8u_C3AC4R(data, Width * 3, textureData.DataPointer, (Width + 1) * 4, size);
            texture_n.UnlockRectangle(0);

            try
            {
                dx.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Aqua, 1.0f, 0);
                dx.BeginScene();
                SetupMatrices(dx);//Установка матриц проекций
                dx.SetTexture(0, texture_n);
                dx.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                dx.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                dx.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Current);
                dx.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
                dx.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
                dx.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
                dx.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
                dx.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<CustomVertex.PositionNormalTextured>());
                dx.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                int triangles_count = GridSize * 2 * GridSize + GridSize - 1;
                dx.DrawPrimitives(PrimitiveType.TriangleStrip, 0, triangles_count);
                dx.EndScene();
            }
            catch
            {
                dx.EndScene();
                dx.Present();
                dx.Dispose();
                return null;
            }
            Surface renderTarget = dx.GetRenderTarget(0);
            DataStream gs = Surface.ToStream(renderTarget, ImageFileFormat.Bmp);
            dx.Dispose();
            byte[] buff = new byte[gs.Length];
            gs.Read(buff, 0, buff.Length);
            byte[] out_buff = new byte[Width * Height];
            int pos = 0;
            for (int i = 54; i < buff.Length && pos < out_buff.Length; i = i + 4)
            {
                out_buff[pos] = buff[i];
                pos++;
            }
            //gs.
            //return new Bitmap(gs);
            return out_buff;
        }

        /// <summary>Проверка устройства</summary>
        bool CheckDevice()
        {
            const int deviceLost = -2005530520, deviceNotReset = -2005530519;
            var r = device.TestCooperativeLevel();
            if (r.Success) return true;
            switch (r.Code)
            {
                case deviceLost: return false;
                case deviceNotReset: device.Reset(presentParams); break;
            }
            return false;
        }

        #endregion

        #region DrawThead

        public void DrawFrameThread(byte[] array, int w, int h)
        {
            workThread = new Thread(new ParameterizedThreadStart(doWork));
            workThread.SetApartmentState(ApartmentState.STA);
            workThread.Priority = ThreadPriority.Lowest;
            workThread.IsBackground = true;

            DataToRender render_data = new DataToRender();
            render_data.array = array;
            render_data.w = w;
            render_data.h = h;
            workThread.Start(render_data);
        }

        private void doWork(object obj)
        {
            DataToRender data = (DataToRender)obj;
            DrawFrameThreadSafe(data.array, data.w, data.h);
        }

        #endregion

        #region Grid functions

        /// <summary>Поиск ряда змейки нахождения вершины</summary>
        /// <param name="vertex">Номер вершины</param>
        /// <param name="gridsize">Размер грида</param>
        /// <returns>Номер ряда змейки</returns>
        private int FindRow(int vertex, int gridsize)
        {
            for (int i = 1; i <= gridsize; i++)
            {
                if ((i - 1) * (gridsize * 2 + 2) - (i) < vertex &&
                    (i) * (gridsize * 2 + 2) - (i) >= vertex)
                    return (i - 1);
            }
            return 0;
        }

        /// <summary>Поиск колонки </summary>
        private int FindX(int row, int vertex, int gridsize)
        {
            //Первая вершина ряда
            int v = (((gridsize * 2) + 2) * row) - row;
            int X = 0;
            //Ряд четный или нулевой
            if (row % 2 == 0 || row == 0)
            {
                //Нумерация прямая от 0
                X = ((vertex - v) / 2);
            }
            else
            {
                //Нумерация обратная от gridsize + 1
                X = (gridsize) - ((vertex - v) / 2);
            }
            return X;
        }

        #endregion

        #region UnFish

        /// <summary>Установить коэффициент исправления искажений</summary>
        public int UnFishCoeff
        {
            get { return FUnFishEyeCoeff; }
            set 
            { 
                FUnFishEyeCoeff = value;
                this.OnCreateVertexBufferSphere(vertexBuffer, null);
            }
        }

        /// <summary>Включение исправления искажений</summary>
        public bool UnFishEnable
        {
            get { return FUnFishEnable; }
            set
            {
                FUnFishEnable = value;
                this.OnCreateVertexBufferSphere(vertexBuffer, null);
            }
        }

        #endregion

        #region IDispose
        
        object _disposeLocker = new object();

        public void Dispose()
        {
            lock (_disposeLocker)
            {
                start = false;
                init = true;
                OnEventChangeInitState(this, init, true);
                //Нельзя уничтожать поку рисуется объект
                //Может поэтому проблема
                while (draw_frame) //или draw_frame
                {
                    Thread.Sleep(1);
                }
                BitmapData = null;
                try
                {
                    if (device != null)
                    {
                        device.Dispose();
                        device = null;
                    }
                    if (texture != null)
                    {
                        texture.Dispose();
                        texture = null;
                    }
                    if (texture_buff != null)
                    {
                        texture_buff.Dispose();
                        texture_buff = null;
                    }
                    if (vertexBuffer != null)
                    {
                        vertexBuffer.Dispose();
                        vertexBuffer = null;
                    }
                    if (fnt != null)
                    {
                        lock (_fontLocker)
                        {
                            fnt.Dispose();
                            fnt = null;
                        }
                    }
                }
                catch { };
                GC.SuppressFinalize(this);
            }
        }

        void device_Disposing(object sender, EventArgs e)
        {

        }

        #endregion
        
    }
    
}
