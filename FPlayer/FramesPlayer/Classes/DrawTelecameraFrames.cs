using System; 
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using AlfaPribor.SharpDXVideoRenderer;

namespace FramesPlayer
{

    /// <summary>Класс рисования кадров телекамеры</summary>
    class DrawTelecameraFrames
    {

        /// <summary>Структура заголовка Bitmap</summary>
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

        #region Variables

        /// <summary>Класс рисования кадров Direct3D</summary>
        SharpDXVideoRenderer obj_DrawFrames;

        /// <summary>Временный буфер</summary>
        byte[] ImageBuffer;

        /// <summary>Глобальный номер кадра</summary>
        int frame_index;
        /// <summary>Номер кадра для измерения скорости отрисовки</summary>
        int last_frame_index;
        /// <summary>Скорость отрисовки кадров живого видео</summary>
        float live_fps;
        /// <summary>Скорость отрисовки кадров при воспроизведении</summary>
        float archive_fps;
        /// <summary>Скорость получения с телекамеры кадров</summary>
        float ips;
        /// <summary>Таймер скорости отрисовки</summary>
        System.Timers.Timer TimerFPS;
        /// <summary>Таймер скорости перерисовки в режиме архива</summary>
        System.Timers.Timer TimerRedraw;
        /// <summary>Время прошлого кадра для измерения скорости отрисовки</summary>
        DateTime last_frame_time;
        /// <summary>Отображение темпа передачи кадров</summary>
        bool show_fps = false;
        /// <summary>Отображение темпа передачи кадров</summary>
        bool show_ips = false;
        /// <summary>Идентификатор видеоканала</summary>
        int id;
        /// <summary>Ширина кадра</summary>
        int Width;
        /// <summary>Высота кадра</summary>
        int Height;

        /// <summary>Флаг отрисовки</summary>
        bool draw;
        /// <summary>Текущая отображаемая метка времени кадра</summary>
        int draw_timestamp = 0;

        #endregion

        public DrawTelecameraFrames()
        {

            //Таймер скорости отрисовки
            TimerFPS = new System.Timers.Timer();
            TimerFPS.Interval = 1000;
            TimerFPS.Elapsed += new System.Timers.ElapsedEventHandler(TimerFPS_Tick);
            TimerFPS.Start();

            //Таймер перерисовка кадра
            TimerRedraw = new System.Timers.Timer();
            TimerRedraw.Interval = 1;
            TimerRedraw.Elapsed += new System.Timers.ElapsedEventHandler(TimerRedraw_Elapsed);
            TimerRedraw.Enabled = false;

        }

        /// <summary>Инициализация устройства отрисовкм</summary>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        /// <param name="picture_box">Элемент панель для отрисовки кадров</param>
        /// <param name="hardware_vertex">Использовать аппаратный вертексный буфер (не рекомендуется для интегрированной графики)</param>
        /// <param name="un_fish">Исправлять искажения</param>
        /// <param name="un_fish_coeff">Коэффициент исправлений искажений</param>
        public void Init(int width, int height, PictureBox picture_box, bool hardware_vertex, 
                         bool un_fish, int un_fish_coeff)
        {
            Width = width;
            Height = height;

            //Удаление старого объекта рендеринга
            if (obj_DrawFrames != null) 
            {
                obj_DrawFrames.Dispose();
                obj_DrawFrames = null;
            }

            obj_DrawFrames = new SharpDXVideoRenderer(width, height, picture_box,
                                                 hardware_vertex,
                                                 un_fish, un_fish_coeff);
            obj_DrawFrames.EventDeviceLost += new SharpDXVideoRenderer.DelegateEventDeviceLost(obj_DrawFrames_EventDeviceLost);

            picture_box.Paint -= picture_box_Paint;
            picture_box.Paint += new PaintEventHandler(picture_box_Paint);
        }

        void obj_DrawFrames_EventDeviceLost(bool lost)
        {
        }

        /// <summary>Состояние инициализации графики</summary>
        public bool InitState
        {
            get { return obj_DrawFrames.Init; }
        }

        /// <summary>Освобождение ресурсов объекта</summary>
        public void Dispose()
        {
            obj_DrawFrames.Dispose();
            obj_DrawFrames = null;

            TimerFPS.Dispose();
            TimerFPS = null;

            TimerRedraw.Dispose();
            TimerRedraw = null;
        }

        /// <summary>Ширина кадра</summary>
        public int FrameWidth
        {
            get { return Width; }
            set { Width = value; }
        }

        /// <summary>Высота кадра</summary>
        public int FrameHeight
        {
            get { return Height; }
            set { Height = value; }
        }

        /// <summary>Идентификтор видеоканала</summary>
        public int ChannelID
        {
            get { return id;}
            set { id = value; }
        }

        /// <summary>Рисование кадра</summary>
        /// <param name="frame">Данные кадра</param>
        /// <param name="timestamp">Метка времени кадра в мс (0 - не выводить)</param>
        public bool NewFrame(byte[] frame, int timestamp)
        {
            //Копирование кадра во временный буфер - в режиме архива
            if (ImageBuffer == null || ImageBuffer.Length != frame.Length)
            {
                ImageBuffer = null;
                ImageBuffer = new byte[frame.Length];
            }
            ImageBuffer = frame;
            draw_timestamp = timestamp;
            return OnFrame(frame, false, timestamp);
        }

        /// <summary>Рисование кадра в режиме архива</summary>
        /// <param name="frame">Данные кадра</param>
        public bool NewFrame32(byte[] frame)
        {
            //Копирование кадра во временный буфер - в режиме архива
            if (ImageBuffer == null || ImageBuffer.Length != frame.Length)
            {
                ImageBuffer = null;
                ImageBuffer = new byte[frame.Length];
            }
            ImageBuffer = frame;
            return OnFrame32(frame, false);
        }

        bool OnFrame(byte[] frame, bool redraw, int timestamp)
        {
            bool res = false;
            if (draw) return true;
            draw = true;
            try
            {
                //Очистить графически элементы видеоокна
                obj_DrawFrames.ClearStrings();
                //Показать скорость отрисовки кадров
                DrawFPS();
                //Показать метку времени кадра
                DrawTimeStamp(timestamp);
            }
            catch
            {
                res = false; 
            }
            try
            {
                res = obj_DrawFrames.DrawRGB24(frame);
            }
            catch
            {
                res = false; 
            }
            if (!redraw) frame_index++;
            draw = false;
            return res;
        }

        bool OnFrame32(byte[] frame, bool redraw)
        {
            bool res = false;
            if (draw) return true;
            draw = true;
            try
            {
                //Очистить графически элементы видеоокна
                obj_DrawFrames.ClearStrings();
                //Показать скорость отрисовки кадров
                DrawFPS();


            }
            catch
            {
                res = false;
            }
            try
            {
                res = obj_DrawFrames.DrawRGB32(frame);
            }
            catch
            {
                res = false;
            }
            if (!redraw) frame_index++;
            draw = false;
            return res;
        }

        void picture_box_Paint(object sender, PaintEventArgs e)
        {
            RedrawStart(200);
        }

        void picture_box_Resize(object sender, EventArgs e)
        {
            RedrawStart(200);
        }

        void RedrawStart(int interval)
        {
            if (TimerRedraw != null)
            {
                TimerRedraw.Interval = interval;
                TimerRedraw.Start();
            }
        }

        /// <summary>Срабатывание таймера перерисовки кадра</summary>
        void TimerRedraw_Elapsed(object sender, EventArgs e)
        {
            TimerRedraw.Stop();
            TimerRedraw.Interval = 1;
            OnFrame(ImageBuffer, true, draw_timestamp);
        }

        #region UnFishEye

        /// <summary>Коэффициент исправления искажений</summary>
        public int UnFishCoeff
        {
            get { return obj_DrawFrames.UnFishCoeff; }
            set { obj_DrawFrames.UnFishCoeff = value; }
        }

        /// <summary>Исправлять искажения</summary>
        public bool UnFish
        {
            get { return obj_DrawFrames.UnFishEnable; }
            set { obj_DrawFrames.UnFishEnable = value; }
        }

        #endregion

        //Таймер скорости отрисовки
        public void TimerFPS_Tick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - last_frame_time;
            live_fps = (float)(1.0f * (frame_index - last_frame_index) / span.TotalSeconds);
            last_frame_index = frame_index;
            last_frame_time = DateTime.Now;
        }

        /// <summary>Установка скорости передачи кадров с телекамеры</summary>
        /// <param name="_ips">Скорость передачи кадров с телекамеры</param>
        public void SetIPS(float Ips)
        {
            live_fps = Ips;
        }

        /// <summary>Установка скорости кадров из видеоплеера</summary>
        /// <param name="fps">Скорость получения кадров</param>
        public void SetFPS(float Fps)
        {
            archive_fps = Fps;
        }

        /// <summary>Рисование скорости отрисовки</summary>
        void DrawFPS()
        {
            if (!show_fps && !show_ips) return;
            string s = "";
            if (show_fps) s += "FPS:" + archive_fps.ToString("0.0") + "  ";
            //текст
            DrawCaption(s, 5, 5, Color.White, true);
        }

        /// <summary>Рисование метки времени кадра</summary>
        void DrawTimeStamp(int timestamp)
        {
            if (timestamp > 0)
            {
                //TimeSpan span = new TimeSpan(timestamp * 10);
                TimeSpan span = new TimeSpan(0, 0, 0, 0, timestamp);
                string s = span.Minutes.ToString() + "." + span.Seconds.ToString("00") + "," + span.Milliseconds.ToString("000");
                DrawCaption(s, 5, 5, Color.White, true);
            }
        }

        /// <summary>Рисование строки</summary>
        /// <param name="S">Строка</param>
        /// <param name="X">х-координата</param>
        /// <param name="Y">y-координата</param>
        /// <param name="FontSize">Размер шрифта</param>
        void DrawCaption(string S, int X, int Y, Color color, bool shadow)
        {
            if (shadow)//Рисовать тень
            {
                uint c = (uint)Color.Black.ToArgb();
                obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X + 1, Y, c));
                obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X + 1, Y + 1, c));
                obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X, Y + 1, c));
            }
            //текст
            obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X, Y, (uint)color.ToArgb()));
        }

        /// <summary>Отобразить графический элемент</summary>
        /// <param name="type">Тип элемента</param>
        /// <param name="show">Статус отображения</param>
        public void ShowGraphicElement(GraphicElement type, bool show)
        {
            if (type == GraphicElement.DrawFPS) show_fps = show;
            if (type == GraphicElement.DrawIPS) show_ips = show;
        }

        /// <summary>Получение текущего кадра</summary>
        public Bitmap BitmapImage
        {
            get
            {
                int w = Width;
                int h = Height;

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
                bmi_header = StructureToByteArray(bmih);

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

                System.IO.MemoryStream ms = new System.IO.MemoryStream(14 + 40 + size);
                ms.Write(fileheader, 0, 14);
                ms.Write(bmi_header, 0, 40);
                ms.Write(ImageBuffer, 0, size);

                Bitmap bitmap = obj_DrawFrames.GetScene(ImageBuffer);
                if (bitmap == null) return null;

                //Bitmap bitmap = new Bitmap(ms);
                return bitmap;
            }
        }

        static byte[] StructureToByteArray(object obj)
        {
            int len = System.Runtime.InteropServices.Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(len);
            System.Runtime.InteropServices.Marshal.StructureToPtr(obj, ptr, true);
            System.Runtime.InteropServices.Marshal.Copy(ptr, arr, 0, len);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>Угол поворота картинки</summary>
        public SharpDXVideoRenderer.RotationAngle Rotation
        {
            get
            {
                if (obj_DrawFrames != null) return obj_DrawFrames.Rotation;
                else return 0;
            }
            set
            {
                if (obj_DrawFrames != null) obj_DrawFrames.Rotation = value;
            }
        }


    }

}
