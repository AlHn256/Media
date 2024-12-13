using System;
using System.Windows.Forms;
using System.Drawing;

using AlfaPribor.SharpDXVideoRenderer;  //Отрисовщик кадров
using AlfaPribor.IppInterop;            //Импорт функций ipp2010

namespace FramesPlayer
{

    /// <summary>Класс рисования кадров тепловизора</summary>
    class DrawThermoFrames
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

        #region Delegates

        delegate void SetToolTipCallback(string text, int x, int y);

        delegate bool GetPanelPointCallback();

        #endregion

        #region Параметры цветовых палитр

        //Redhot
        float[] RedHotPoints = { 0.0f, 0.5f, 0.75f, 1.0f };
        int[] RedHotValuesB = { 255, 0, 0, 255 };
        int[] RedHotValuesG = { 0, 0, 255, 255 };
        int[] RedHotValuesR = { 0, 255, 255, 255 };

        //Rainbow
        float[] RainbowPoints = { 0.0f, 0.12f, 0.25f, 0.38f, 0.50f, 0.62f, 0.75f, 1.0f };
        int[] RainbowValuesB = { 0, 255, 0, 0, 0, 255, 255, 255 };
        int[] RainbowValuesG = { 0, 0, 0, 255, 255, 255, 0, 255 };
        int[] RainbowValuesR = { 0, 255, 255, 255, 0, 0, 0, 255 };

        #endregion

        #region Variables

        /// <summary>Идентификатор видеоканала</summary>
        int id;
        /// <summary>Класс рисования кадров Direct3D</summary>
        SharpDXVideoRenderer obj_DrawFrames;
        /// <summary>Панель рисования</summary>
        PictureBox pictureBox;
        /// <summary>Буфер кадра из архива</summary>
        byte[] ImageBuffer;
        /// <summary>Флаг изменения размеров панели</summary>
        bool resize = false;
        /// <summary>Флаг состояния рисования текущего кадра</summary>
        bool draw_frame = false;
        /// <summary>Глобальный номер отрисованного кадра</summary>
        int frame_index = 0;
        /// <summary>Номер кадра для измерения скорости отрисовки</summary>
        int last_frame_index;
        /// <summary>Скорость отрисовки кадров живого видео</summary>
        float live_fps;
        /// <summary>Скорость кадров из видеоплеера</summary>
        float archive_fps;
        /// <summary>Скорость получения кадров с телекамеры</summary>
        float ips;
        /// <summary>Таймер скорости отрисовки</summary>
        System.Timers.Timer TimerFPS;
        /// <summary>Таймер скорости перерисовки в режиме архива</summary>
        System.Timers.Timer TimerRedraw;
        /// <summary>Время прошлого кадра для измерения скорости отрисовки</summary>
        DateTime last_frame_time;

        /// <summary>Ширина тепловизионного кадра</summary>
        int FWidth;
        /// <summary>Высота тепловизионного кадра</summary>
        int FHeight;
        /// <summary>Текущая палитра</summary>
        Palettes pPalette;
        /// <summary>Коэффициент мсштабирования изображения по ширине</summary>
        float KoefZoomX;
        /// <summary>Коэффициент мсштабирования изображения по высоте</summary>
        float KoefZoomY;            

        //Параметры графических элементов
        bool draw_borders = false;          //Рисовать границы обработки
        bool draw_horizon = false;          //Рисовать линию горизонта
        bool draw_captions = false;         //Рисовать заголовки
        bool draw_function = false;         //Отображать функцию распределения температур
        bool draw_level = false;            //Рисование уровня загрузки
        bool draw_level_doc = false;        //Рисование уровня загрузки по документам
        bool draw_calibre = false;          //Рисование границ калибровочного типа
        bool draw_scale = false;            //Рисование шкалы температур
        bool show_temperature = false;      //Отображение температуры в заданной точке
        bool show_fps = false;               //Отображение темпа передачи кадров
        bool show_ips = false;              //Отображение темпа передачи кадров

        /// <summary>Границы обработки</summary>
        Rectangle borders = Rectangle.Empty;
        /// <summary>Высота кадра</summary>
        int frame_height = 320;             
        /// <summary>Угол наклона тепловизора</summary>
        double tilt_angle = 5.0d;           
        /// <summary>Угол обзора тепловизора</summary>
        int view_angle;                     
        /// <summary>Ширина полосы сканирования</summary>
        int extrems_distant;                
        /// <summary>Порог отсева экстремумов отностительно максимального</summary>
        double extrems_absolute_threshold;
        /// <summary>Минимальное расстояние уровня загрузки от рассчетного значения по калибровочному типу</summary>
        int near_calible;
        /// <summary>Расстояние от цента пути до тепловизора</summary>
        double m_distance;
        /// <summary>Заданное исходная высота устаноки тепловизора от низа цистернвы</summary>
        double m_height;
        /// <summary>Рассчитывать без калибровочного типа</summary>
        bool calc_without_calib;

        /// <summary>Кэш распределения средних сигналов по строкам</summary>
        double[] AverageS;
        /// <summary>Кэш уровня загрузки</summary>
        int LevelY;
        /// <summary>Кэш описание уровня загрузкт</summary>
        string CacheLevelDesc;
        /// <summary>Кэш примечания уровня загрузкт</summary>
        string CacheLevelNote;

        /// <summary>Флаг расчета уровня загрузки</summary>
        bool FlagCashLevel;
        /// <summary>Флаг расчета средних значений</summary>
        bool FlagCashAverageS;

        ToolTip tool_tip_temperature;

        Font tool_tip_font;
        /// <summary>Текущая координата X указателя мыши</summary>
        int MouseX;
        /// <summary>Текущая координата Y указателя мыши</summary>
        int MouseY;
        /// <summary>Таймер обновления температуры в точке</summary>
        Timer timer_show_temerature;
        /// <summary>Значения в цветной палитре</summary>
        byte[,] PaletteRainbow;
        /// <summary>Значения в кросной палитре</summary>
        byte[,] PaletteRedHot;
        /// <summary>Значения текущей выбранной палитры</summary>
        byte[,] CurrentPalette;

        /// <summary>Флаг ручного расчета</summary>
        bool ManualCalc = false;
        /// <summary>уровень загрузки указанный оператором - координата сверху</summary>
        int ManualLevelY;
        /// <summary>Режим ручного указания границ</summary>
        bool ManualSetBordersFlag;
        /// <summary>Верхняя границы цистерны указанная оператором - координата сверху</summary>
        int ManualSetBordersY;

        #endregion

        public DrawThermoFrames()

        {
            //Создание объекта подсказки
            tool_tip_temperature = new ToolTip();
            timer_show_temerature = new Timer();
            timer_show_temerature.Interval = 200;
            timer_show_temerature.Tick += new EventHandler(timer_show_temperature_Tick);
            tool_tip_font = new Font("Tahoma", 8);

            //Создание массивов палитр
            CreatePalettes();

            //Таймер скорости отрисовки
            TimerFPS = new System.Timers.Timer();
            TimerFPS.Interval = 1000;
            TimerFPS.Elapsed += new System.Timers.ElapsedEventHandler(TimerFPS_Tick);
            TimerFPS.Start();

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
            FWidth = width;
            FHeight = height;

            //Удаление старого объекта рендеринга
            if (obj_DrawFrames != null)
            {
                obj_DrawFrames.Dispose();
                obj_DrawFrames = null;
            }

            //Создание нового
            obj_DrawFrames = new SharpDXVideoRenderer(width, height, picture_box,   hardware_vertex, un_fish, un_fish_coeff);

            //Панель рисования
            pictureBox = picture_box;

            picture_box.Resize -= picture_box_Resize;
            picture_box.Resize += new EventHandler(picture_box_Resize);

            picture_box.MouseMove -= picture_box_MouseMove;
            picture_box.MouseMove += new MouseEventHandler(picture_box_MouseMove);

            picture_box.MouseLeave -= picture_box_MouseLeave;
            picture_box.MouseLeave += new EventHandler(picture_box_MouseLeave);

            //picture_box.Paint -= picture_box_Paint;
            //picture_box.Paint += new PaintEventHandler(picture_box_Paint);

            picture_box_Resize(picture_box, EventArgs.Empty);

        }

        /// <summary>Состояние инициализации графики</summary>
        public bool InitState
        {
            get { return obj_DrawFrames.Init; }
        }

        /// <summary>Освобождение ресурсов объекта</summary>
        public void Dispose()
        {
            pictureBox = null;

            tool_tip_temperature.Dispose();
            tool_tip_temperature = null;

            timer_show_temerature.Tick -= timer_show_temperature_Tick;
            timer_show_temerature.Dispose();
            timer_show_temerature = null;

            obj_DrawFrames.Dispose();
            obj_DrawFrames = null;

            TimerFPS.Elapsed -= TimerFPS_Tick;
            TimerFPS.Dispose();
            TimerFPS = null;

            TimerRedraw.Elapsed -= TimerRedraw_Elapsed;
            TimerRedraw.Dispose();
            TimerRedraw = null;
        }

        /// <summary>Идентификтор видеоканала</summary>
        public int ChannelID
        {
            get { return id; }
            set { id = value; }
        }

        #region Draw frames

        /// <summary>Рисование кадра в режиме архива</summary>
        /// <param name="frame">Данные кадра</param>
        public bool NewFrame(byte[] frame)
        {
            ImageBuffer = frame;
            return OnFrame(frame, false);
        }

        /// <summary>Функция рисования кадра и наложения графических элементов</summary>
        /// <param name="frame">Данные кадра</param>
        /// <param name="redraw">Флаг перерисовки</param>
        bool OnFrame(byte[] frame, bool redraw)
        {
            if (resize) return true;
            if (frame == null) return false;
            if (draw_frame) return true;
            if (obj_DrawFrames == null) return false;
            draw_frame = true;
            //Очистка линий и строк в объекте отрисовки
            obj_DrawFrames.ClearLines();
            obj_DrawFrames.ClearStrings();

            FlagCashAverageS = false;
            //Рисование элементов
            if (show_fps || show_ips) DrawFPS();//Скорость кадров

            //Рисование кадра
            try { obj_DrawFrames.DrawRGB24(ApplyPalette(frame)); }
            catch
            {
                draw_frame = false;
                return false;
            }

            if (!redraw) frame_index++;
            draw_frame = false;
            return true;
        }

        /// <summary>Срабатывание таймера перерисовки кадра</summary>
        void TimerRedraw_Elapsed(object sender, EventArgs e)
        {
            TimerRedraw.Stop();
            TimerRedraw.Interval = 1;
            OnFrame(ImageBuffer, true);
        }

        void RedrawStart(int interval)
        {
            if (TimerRedraw == null) return;
            if (TimerRedraw.Enabled) return;
            TimerRedraw.Interval = interval;
            TimerRedraw.Start();
        }

        #endregion

        /// <summary>Таймер скорости отрисовки</summary>
        public void TimerFPS_Tick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - last_frame_time;
            live_fps = (float)(1.0f * (frame_index - last_frame_index) / span.TotalSeconds);
            last_frame_index = frame_index;
            last_frame_time = DateTime.Now;
        }

        /// <summary>Установка скорости получения кадров</summary>
        /// <param name="Ips">Скорость получения кадров</param>
        public void SetIPS(float Ips)
        {
            ips = Ips;
        }

        /// <summary>Установка скорости кадров из видеоплеера</summary>
        /// <param name="fps">Скорость получения кадров</param>
        public void SetFPS(float Fps)
        {
            archive_fps = Fps;
            RedrawStart(1);// TimerRedraw.Start();// OnFrame(ImageBuffer, true);
        }

        #region UnFishEye

        /// <summary>Коэффициент исправлений искажений</summary>
        public int UnFishCoeff
        {
            get { return obj_DrawFrames.UnFishCoeff; }
            set 
            { 
                obj_DrawFrames.UnFishCoeff = value;
                //Создание маски исправлений искажений
                //IppFunctions.CreateUnFishMap24bit((value )/ 100.0f, FWidth, FHeight);
            }
        }

        /// <summary>Исправлять искажения</summary>
        public bool UnFish
        {
            get { return obj_DrawFrames.UnFishEnable; }
            set { obj_DrawFrames.UnFishEnable = value; }
        }

        #endregion

        #region Палитры

        /// <summary>Создание массивов палитр</summary>
        void CreatePalettes()
        {
            PaletteRedHot = CreatePalette(RedHotPoints, RedHotValuesB, RedHotValuesG, RedHotValuesR);
            PaletteRainbow = CreatePalette(RainbowPoints, RainbowValuesB, RainbowValuesG, RainbowValuesR);
        }
         
        /// <summary>Создание палитры</summary>
        /// <param name="palette"></param>
        /// <returns>Массив [0..255] со значениями цвета для уровня</returns>
        byte[,] CreatePalette(float[] points, int[] valuesB, int[] valuesG, int[] valuesR)
        {
            byte[,] pal = new byte[256, 3];
            int max = 255;
            for (int val=0; val<256; val++) //Градации серого входного тепловизионного изображения
                {
                    float f = 1.0f * val / max;
                    for (int p = 1; p < points.Length; p++)
                        if (points[p] >= f && points[p - 1] <= f)
                        {
                            float a = (f - points[p - 1]) / (points[p] - points[p - 1]);
                            pal[val, 0] = (byte)(Math.Min(valuesB[p], valuesB[p - 1]) + (valuesB[p] - valuesB[p - 1]) * a);
                            pal[val, 1] = (byte)(Math.Min(valuesG[p], valuesG[p - 1]) + (valuesG[p] - valuesG[p - 1]) * a);
                            pal[val, 2] = (byte)(Math.Min(valuesR[p], valuesR[p - 1]) + (valuesR[p] - valuesR[p - 1]) * a);
                            break;
                        }
                }
            return pal;
        }

        /// <summary>Применение палитры</summary>
        /// <param name="buffer">Входной буфер термограммы 8 бит на пиксель</param>
        byte[] ApplyPalette(byte[] buffer)
        {
            int step = 3;//шаг в результирующем изображении
            byte[] drawbuf = new byte[FWidth * FHeight * step];
            //Черно белая палитра
            if (pPalette == Palettes.Gray)    //Без учета палитры
            {
                //Замена всех каналов цветного изображения значениями из серого
                IppiSize size = new IppiSize();
                size.width = FWidth;
                size.height = FHeight;
                IppFunctions.ippiDup_8u_C1C3R(buffer, FWidth, drawbuf, FWidth * step, size);
            }
            //Раскаленная и цветная палитра
            if (pPalette != Palettes.Gray)
            {
                if (CurrentPalette != null)
                    for (int row = 0; row < FHeight; row++)     //строки
                        for (int col = 0; col < FWidth; col++)  //столбцы
                        {
                            int t = buffer[row * FWidth + col]; //Сигнал в точке
                            int point = row * FWidth * step + col * step;//Точка в выходном массиве
                            drawbuf[point + 0] = CurrentPalette[t, 0];//B
                            drawbuf[point + 1] = CurrentPalette[t, 1];//G
                            drawbuf[point + 2] = CurrentPalette[t, 2];//R
                        }
            }
            return drawbuf;
        }

        byte[] GetColorFromPalette(int value, Palettes pPalette, float[] points, int[] valuesB, int[] valuesG, int[] valuesR)
        {
            byte R = 0, G = 0, B = 0;
            /*
            float[] points = null;
            int[] valuesB = null;
            int[] valuesG = null;
            int[] valuesR = null;
            if (pPalette == Palettes.RedHot)
            {
                points = RedHotPoints;
                valuesB = RedHotValuesB;
                valuesG = RedHotValuesG;
                valuesR = RedHotValuesR;
            }

            if (pPalette == Palettes.Rainbow)
            {
                points = RainbowPoints;
                valuesB = RainbowValuesB;
                valuesG = RainbowValuesG;
                valuesR = RainbowValuesR;
            }
            */ 
            int max = 255;//Максимальное значение передаваемого уровня 
            float f = 1.0f * value / max;
            for (int p = 1; p < points.Length; p++)
                if (points[p] >= f && points[p - 1] <= f)
                {
                    float a = (f - points[p - 1]) / (points[p] - points[p - 1]);
                    B = (byte)(Math.Min(valuesB[p], valuesB[p - 1]) + (valuesB[p] - valuesB[p - 1]) * a);
                    G = (byte)(Math.Min(valuesG[p], valuesG[p - 1]) + (valuesG[p] - valuesG[p - 1]) * a);
                    R = (byte)(Math.Min(valuesR[p], valuesR[p - 1]) + (valuesR[p] - valuesR[p - 1]) * a);
                    return new byte[] { B, G, R };// Color.FromArgb(R, G, B);
                }
            return new byte[] { 0, 0, 0 };
        }

        /// <summary>Установка палитры</summary>
        /// <param name="palette">Тип палитры</param>
        public void SetPalette(Palettes palette)
        {
            pPalette = palette;
            if (pPalette == Palettes.Rainbow) CurrentPalette = PaletteRainbow;
            if (pPalette == Palettes.RedHot) CurrentPalette = PaletteRedHot;
        }

        #endregion

        #region События панели

        /// <summary>Изменение размера панели</summary>
        void picture_box_Resize(object sender, EventArgs e)
        {
            //Изменение коэффициента масштабирования
            KoefZoomX = (float)((PictureBox)sender).Width / FWidth;
            KoefZoomY = (float)((PictureBox)sender).Height / FHeight;
            RedrawStart(200);
        }

        /// <summary>Перерисовка панели</summary>
        void picture_box_Paint(object sender, PaintEventArgs e)
        {
            //if (((PictureBox)sender).Parent))
            RedrawStart(200);
        }

        /// <summary>Движение курсора над панелью</summary>
        void picture_box_MouseMove(object sender, MouseEventArgs e)
        {
            //Отображение температуры в указанной точке
            if (show_temperature)
            {
                MouseX = e.X;
                MouseY = e.Y;
                ShowToolTipTemp();
                //Перезапуск таймера
                timer_show_temerature.Stop();
                timer_show_temerature.Start();
                tool_tip_temperature.Active = true;
                RedrawStart(10);
            }
        }

        /// <summary>Покидание курсора панели</summary>
        void picture_box_MouseLeave(object sender, EventArgs e)
        {
            if (show_temperature)
            {
                timer_show_temerature.Stop();
                tool_tip_temperature.Active = false;
            }
            RedrawStart(1);
        }

        #endregion

        #region Подсказка температуры

        void ShowToolTipTemp()
        {
            if (show_temperature)
            {
                if (MouseX < 0 || MouseY < 0) return;
                Point dot = GetDot(MouseX, MouseY);
                if (dot == null) return;
                int signal = GetDotTemperature(dot);
                double mTline = GetSignalLine(dot.Y);
                SetThreadSafeToolTip("(x:" + (dot.X).ToString() + ",y:" + (dot.Y).ToString() + ") " +
                                     "S = " + signal.ToString() + " (~" + mTline.ToString("0.00") + ")", 
                                     MouseX, MouseY - 21);
            }
        }

        void timer_show_temperature_Tick(object sender, EventArgs e)
        {
            //timer_show_temerature.Stop();
            ShowToolTipTemp();
            //timer_show_temerature.Start();
        }

        /// <summary>Потокобезопасный вывод подсказки температуры в заданной точке</summary>
        /// <param name="text">Текст подсказки</param>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата </param>
        void SetThreadSafeToolTip(string text, int x, int y)
        {
            if (pictureBox == null) return;
            if (pictureBox.InvokeRequired)
            {
                SetToolTipCallback d = new SetToolTipCallback(SetThreadSafeToolTip);
                pictureBox.Invoke(d, new object[] { text, x, y });
            }
            else
            {
                SetToolTip(text, x, y);
            }
        }

        void SetToolTip(string text, int x, int y)
        {
            if (pictureBox == null) return;
            int X = x;
            int Y = y;
            if (tool_tip_font != null)
            {
                int string_width = TextRenderer.MeasureText(text, tool_tip_font).Width;
                if (x > pictureBox.Width - string_width) X = pictureBox.Width - string_width - 5;
            }
            try
            {
                tool_tip_temperature.Show(text, pictureBox, X, Y, 30000);
            }
            catch { };
        }

        /// <summary>Получение координат тепловизионной матрицы по указателю</summary>
        /// <param name="mouseX">Координата X указателя</param>
        /// <param name="mouseY">Координата Y указателя</param>
        /// <returns>Координаты точки тепловизионной матрицы</returns>
        Point GetDot(int mouseX, int mouseY)
        {
            int x = 0;
            int y = 0;
            if (pictureBox == null) return new Point(x, y);
            if (mouseX >= pictureBox.Width) x = FWidth;
            if (mouseY >= pictureBox.Height) y = FHeight;
            //Координата X тепловизионного изображения
            if (mouseX < pictureBox.Width && mouseX >= 0) 
                x = (int)Math.Round((float)mouseX / pictureBox.Width * FWidth);
            //Координата Y тепловизионного изображения
            if (mouseY < pictureBox.Height && mouseY >= 0)
                y = (int)Math.Round((float)mouseY / pictureBox.Height * FHeight);
            if (x >= FWidth) x = FWidth - 1;
            if (y >= FHeight) y = FHeight - 1;
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            return new Point(x, y);
        }

        /// <summary>Получить температуру в указанных координатах матрицы</summary>
        int GetDotTemperature(Point dot)
        {
            if (ImageBuffer == null) return 0;
            return ImageBuffer[(FHeight - dot.Y - 1) * FWidth + dot.X];//Температура в точке
            
            //byte[] b = obj_DrawFrames.GetDrawingBuffer8bpp(ApplyPalette(ImageBuffer));
            //return b[(FHeight - dot.Y - 1) * FWidth + dot.X];//Температура в точке
        }

        /// <summary>Получить среднюю температуру в строке матрицы с учетом области анализа</summary>
        /// <param name="y">Y-координата</param>
        /// <returns>Средняя температура в строке в заданных границах</returns>
        double GetSignalLine(int y)
        {
            //Анализ строки, входящей в заданные пределы
            if (FlagCashAverageS) return AverageS[y];
            else
            {
                int l = borders.Right - borders.Left;
                double mTA = 0;
                for (int i = borders.Left; i < borders.Right; i++) mTA += GetDotTemperature(new Point(i, y));
                if (l != 0) return (1.0d * mTA) / l;
                else return 0;
            }
        }

        #endregion

        #region Получение текущего изображения из буфера

        /// <summary>Получение текущего кадра в выбранной палитре</summary>
        /// <param name="GraphicElements">Рисовать графические элементы</param>
        /// <param name="zoom">Коэффициент увеличения изображения</param>
        public Bitmap GetBitmapImage(bool GraphicElements, double zoom)
        {
            int w = FWidth;
            int h = FHeight;
            int size = w * h * 3;
            byte[] bmp = new byte[size];

            BITMAPINFOHEADER bmih = new BITMAPINFOHEADER();
            bmih.biSize = 40;
            bmih.biWidth = w;
            bmih.biHeight = h;
            bmih.biPlanes = 1;
            bmih.biBitCount = 24;
            bmih.biCompression = 0;
            bmih.biSizeImage = (int)(w * h * 3);
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

            System.IO.MemoryStream ms = new System.IO.MemoryStream(14 + 40 + size);
            ms.Write(fileheader, 0, 14);
            ms.Write(bmi_header, 0, 40);

            Bitmap bitmap = obj_DrawFrames.GetScene(ApplyPalette(ImageBuffer));

            if (bitmap == null) return null;
            //Итоговое изображение
            Bitmap result = new Bitmap((int)Math.Round(bitmap.Width * zoom), 
                                       (int)Math.Round(bitmap.Height * zoom));
            if (GraphicElements) DrawGraphicsElemets(result, bitmap, zoom);
            else
            {
                Graphics g = Graphics.FromImage(result);
                g.DrawImage(bitmap, 0, 0, result.Width, result.Height);
            }
            return result;
        }

        /// <summary>Рисование текста с тенью</summary>
        void DrawCaption(Graphics g, string s, int x, int y)
        {
            //Тень
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.Black, x - 1, y - 1);
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.Black, x, y - 1);
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.Black, x + 1, y - 1);
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.Black, x - 1, y);
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.Black, x + 1, y);
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.Black, x - 1, y + 1);
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.Black, x, y + 1);
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.Black, x + 1, y + 1);
            //Строка
            g.DrawString(s, new Font(new FontFamily("Arial"), 10, GraphicsUnit.Pixel), Brushes.White, x, y);
        }

        void DrawGraphicsElemets(Bitmap img, Bitmap source, double zoom)
        {
            //Рисование элементов
            Graphics g = Graphics.FromImage(img);
            g.DrawImage(source, 0, 0, img.Width, img.Height);
            //Рисование шкалы температур
            if (draw_scale)
            {
                Pen penScale = new Pen(Color.White, 1);
                Point point1 = new Point(10, img.Height / 2);//Верхняя левая точка
                Point point2 = new Point(30, img.Height - 20);//Нижняя правая точка
                //Рамка
                g.DrawRectangle(new Pen(Brushes.Black), new Rectangle(point1.X, point1.Y, point2.X - point1.X, point2.Y - point1.Y));
                //Рисование шкалы
                for (int y = point1.Y + 1; y < point2.Y; y++)
                {
                    //Градация серого
                    int grey = 255 - (int)(255 * ((1.0f * (y - point1.Y)) / (point2.Y - point1.Y)));
                    //RGB в градациях серого
                    int color = 0;
                    if (pPalette == Palettes.Gray)
                        color = grey + grey * 256 + grey * 256 * 256;
                    if (pPalette == Palettes.RedHot)
                        color = PaletteRedHot[grey, 0] +
                                PaletteRedHot[grey, 1] * 256 +
                                PaletteRedHot[grey, 2] * 256 * 256;
                    if (pPalette == Palettes.Rainbow)
                        color = PaletteRainbow[grey, 0] +
                                PaletteRainbow[grey, 1] * 256 +
                                PaletteRainbow[grey, 2] * 256 * 256;
                    //Добавление линии
                    g.DrawLine(new Pen(Color.FromArgb(255, Color.FromArgb(color))),
                               point1.X + 1, y, point2.X - 1, y);
                }
                //Рисование букв
                DrawCaption(g, "255", 10, img.Height / 2 - 12);
                DrawCaption(g, "0", 16, img.Height - 10 - 9);//Максимальный уровень
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

        #endregion

        #region Графические элементы

        /// <summary>Рисование скорости кадров</summary>
        void DrawFPS()
        {
            if (!show_fps && !show_ips) return;
            string s = "";
            if (show_fps) s += "FPS:" + archive_fps.ToString("0.0") + "  ";
            //текст
            DrawCaption(s, 5, 5, Color.White, true);
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
                obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X - 1, Y - 1, (uint)Color.Black.ToArgb()));
                obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X - 1, Y + 1, (uint)Color.Black.ToArgb()));
                obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X + 1, Y - 1, (uint)Color.Black.ToArgb()));
                obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X + 1, Y + 1, (uint)Color.Black.ToArgb()));
            }
            //текст
            obj_DrawFrames.AddString(new SharpDXVideoRenderer.DrawString(S, X, Y, (uint)color.ToArgb()));
        }

        #endregion

    }

}
