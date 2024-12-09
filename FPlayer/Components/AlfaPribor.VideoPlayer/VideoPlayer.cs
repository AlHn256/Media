using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using myControls;
using AlfaPribor.VideoStorage2;

namespace AlfaPribor.VideoPlayer
{

    /// <summary>Класс воспроизведения видео</summary>
    public class VideoPlayerInterface
    {

        #region Enums

        /// <summary>Тип кнопки пользовательского интерфейса</summary>
        public enum PlayerButtonType 
        { 
            /// <summary>Воспроизведение назад</summary>
            PlayBack = 1,
            /// <summary>Пауза</summary>
            Pause = 2,
            /// <summary>Воспроизведение вперед</summary>
            Play = 3,
            /// <summary>Следующий кадр</summary>
            NextFrame = 4,
            /// <summary>Предыдущий кадр</summary>
            PrevFrame = 5,
            /// <summary>Первый кадр</summary>
            FirstFrame = 6,
            /// <summary>Последний кадр</summary>
            LastFrame = 7,
            /// <summary>Следующая метка (вагон)</summary>
            NextMark = 8,
            /// <summary>Предыдущая метка (вагон)</summary>
            PrevMark = 9,
            /// <summary>Первая метка (вагон)</summary>
            FirstMark = 10,
            /// <summary>Последняя метка (вагон)</summary>
            LastMark = 11,
            /// <summary>Маркировать кадр</summary>
            MarkFrame = 12,
            /// <summary>Маркировать начало экспорта</summary>
            MarkStartExport = 13,
            /// <summary>Маркировать окончание экспорта</summary>
            MarkStopExport = 14,
            /// <summary>Запуск экспорта</summary>
            Export = 15
        }

        /// <summary>Тип направления перемещения</summary>
        enum MovingDirection
        {
            /// <summary>К предыдущему</summary>
            Prev = 0,
            /// <summary>К следующему</summary>
            Next = 1,
            /// <summary>К первому</summary>
            First = 2,
            /// <summary>К последнему</summary>
            Last = 3
        }

        /// <summary>Информация о кнопке пользовательского интерфейса</summary>
        public struct PlayerButtonInfo
        {
            /// <summary>Тип кнопки</summary>
            public PlayerButtonType Type;
            /// <summary>Изображение кнопки</summary>
            public Image Image;
            /// <summary>Подсказка кнопки</summary>
            public string ToolTip;

            /// <summary>Конструктор кнопки пользовательского интерфейса</summary>
            /// <param name="type">Тип кнопки</param>
            /// <param name="image">Изображение кнопки</param>
            public PlayerButtonInfo(PlayerButtonType type, Image image)
            {
                Type = type;
                Image = image;
                ToolTip = "";
            }

            /// <summary>Конструктор кнопки пользовательского интерфейса</summary>
            /// <param name="type">Тип кнопки</param>
            /// <param name="image">Изображение кнопки</param>
            /// <param name="tool_tip">Текс подсказки кнопки</param>
            public PlayerButtonInfo(PlayerButtonType type, Image image, string tool_tip)
            {
                Type = type;
                Image = image;
                ToolTip = tool_tip;
            }

        }

        #endregion

        #region Variables

        /// <summary>Объект хранилище</summary>
        IVideoStorage obj_VideoStorage;
        /// <summary>Объект чтения данных</summary>
        IVideoReader obj_VideoReader;

        /// <summary>Таймер воспроизведения</summary>
        Multimedia.Timer TimerPlay;
        //System.Timers.Timer TimerPlay;

        /// <summary>Таймер fps</summary>
        Multimedia.Timer TimerFPS;
        //System.Timers.Timer TimerFPS;

        /// <summary>Интервал таймера воспроизведения</summary>
        const int TimerInterval = 1;
        /// <summary>Точный таймер измеренеия времени</summary>
        Stopwatch stopwatch;
        /// <summary>Таймер перехода к следующему кадру при удерживании кнопки</summary>
        private System.Timers.Timer TimerNextFrame = new System.Timers.Timer();

        /// <summary>Скорость воспроизведения</summary>
        double Speed = 1;
        /// <summary>Переменная сохранения состояния скорости</summary>
        double curSpeed;
        /// <summary>Состояние паузы воспроизведения</summary>
        bool PauseFlag = true;
        /// <summary>Флаг процесса установки ползунка</summary>
        bool set_scroll = false;

        /// <summary>Текущая метка времени воспроизведения</summary>
        int TimeStamp;
        /// <summary>Предыдущее время воспроизводимого видео</summary>
        int LastTimeStamp;
        /// <summary>Мастер канал воспроизведения</summary>
        int master_channel;
        /// <summary>Допуск поиска кадров</summary>
        int _delta = 100;

        /// <summary>Флаг открытия файла</summary>
        bool FOpened = false;
        /// <summary>Длина видеопотока</summary>
        int length = 0;
        /// <summary>Массив меток времени</summary>
        int[] marks;
        /// <summary>Номер текущей метки времени</summary>
        int current_mark = 0;
        /// <summary>Подсказка</summary>
        ToolTip tooltip;

        int last_played_time = 0;

        int[] stream_id;        //Идентификаторы видеопотоков
        int[] last_played_index;//Текущие кадры видеопотоков
        int[] last_fps;         //Номер кадра при прошлой выборе темпа

        object _lock = new object();

        /// <summary>Флаг разрешения экспорта</summary>
        bool active_export;

        ToolStrip tool_strip_export;

        int start_export;
        int stop_export;

        //object frame_lock = new object();

        #endregion

        #region Interface

        /// <summary>Кнопки пользовательского интерфейса</summary>
        Control owner;
        /// <summary>Помещать кнопки на FlowLayoutPanel</summary>
        bool flow_controls;
        /// <summary>Отображение скролл бара</summary>
        bool show_scroll_bar;
        /// <summary>Разрешить регулировку скорости воспроизщведения</summary>
        bool allow_set_speed;
        /// <summary>Разрешить упрощенный (узкий) регулятор скорости воспроизщведения</summary>
        bool simple_speed = false;
        /// <summary>Отображать длительность и текущее время в отдельном GroupBox-е</summary>
        bool show_sep_length;
        /// <summary>Разрешить упрощенный (узкий) индикатор времени воспроизщведения
        /// текущее время отображается в отдельном окне</summary>
        bool simple_length = false;

        SmoothProgressBar scroll;
        FlowLayoutPanel flow_panel;
        /// <summary>Группы кнопок управления видео</summary>
        List<PlayerButtonInfo[]> Buttons;
        /// <summary>Группа кнопок управления экспортом</summary>
        PlayerButtonInfo[] ExportButtons;
        /// <summary>Метка отображения времени над ползунком</summary>
        Label label_time;
        //Метка отображения времени в отдельном комбо боксе
        Label lableTime;
        //Метка "Время"
        Label lableTimeName;
        //Метка отображения времени в отдельном комбо боксе
        Label lableLength;
        //Метка "Длительность"
        Label lableLengthName;

        /// <summary>Панель установки скорости</summary>
        GroupBox gb;
        /// <summary>Контейнер текущего времени и общей длительности</summary>
        GroupBox gbLength;
        
        #endregion

        #region Events

        /// <summary>Делегат события кадра</summary>
        /// <param name="channel_id">Идентификатор видеоканала</param>
        /// <param name="buf">Бефер кадра</param>
        /// <param name="timestamp">Метка времени кадра в мс</param>
        public delegate void DelegateEventImage(int resX, int resY, int channel_id, byte[] buf, int timestamp);

        /// <summary>Событие кадра</summary>
        public event DelegateEventImage EventNewFrame;

        /// <summary>Делегат события смены метки (вагона)</summary>
        /// <param name="mark_index">Номер метки, для которого текущее положение времени больше</param>
        public delegate void DelegateEventMark(int mark_index);
        /// <summary>Событие кадра</summary>
        public event DelegateEventMark EventNewMark;

        /// <summary>Делегат события fps</summary>
        /// <param name="channel_id">Идентификатор видеоканала</param>
        /// <param name="fps">Текущий темп воспроизведения (кадров/с)</param>
        public delegate void DelegateEventFPS(int channel_id, int fps);
        /// <summary>Событие fps</summary>
        public event DelegateEventFPS EventFPS;

        /// <summary>Событие открытия видео</summary>
        public delegate void DelegateEventOpenVideo(IList<VideoStreamInfo> info);
        /// <summary>Событие открытия видео</summary>
        public event DelegateEventOpenVideo EventOpenVideo;

        /// <summary>Событие закрытия видео</summary>
        public delegate void DelegateEventCloseVideo();
        /// <summary>Событие закрытия видео</summary>
        public event DelegateEventCloseVideo EventCloseVideo;

        /// <summary>Событие экспорта видео</summary>
        public delegate void DelegateEventEnableExport(bool enable);
        /// <summary>Событие экспорта видео</summary>
        public event DelegateEventEnableExport EventEnableExport;

        /// <summary>Событие экспорта видео</summary>
        public delegate void DelegateEventExport(int start, int stop);
        /// <summary>Событие экспорта видео</summary>
        public event DelegateEventExport EventExport;

        /// <summary>Событие движения мышью</summary>
        public delegate void DelegateEventMouseMove();
        /// <summary>Событие движения мышью</summary>
        public event DelegateEventMouseMove EventMouseMove;

        /// <summary>Событие изменения размера панели</summary>
        public delegate void DelegateEventPanelResize(int width, int height);
        /// <summary>Событие движения мышью</summary>
        public event DelegateEventPanelResize EventPanelResize;

        #endregion

        #region Safe callbacks

        delegate void SetScrollValueCallback(SmoothProgressBar scroll, int value);

        delegate void SetLabelCallback(Label label, int scroll_left, int scroll_width, int posX, int posY, string value);

        delegate void SetLabelTextCallback(Label label, string value);

        delegate void ButtonCallback(ToolStripButton button, bool check);

        delegate void ChangeButtonThreadSafeCallback(PlayerButtonInfo button);

        #endregion

        /// <summary>Конструктор</summary>
        public VideoPlayerInterface()
        {
            //Таймер воспроизведения
            TimerPlay = new Multimedia.Timer();
            TimerPlay.Period = TimerInterval;
            TimerPlay.Tick += new EventHandler(TimerPlay_Tick);
            /*
            TimerPlay = new System.Timers.Timer();
            TimerPlay.Interval = TimerInterval;
            TimerPlay.Elapsed += new System.Timers.ElapsedEventHandler(TimerPlay_Tick);
            TimerPlay.Stop();
            */ 
            //Таймер FPS
            TimerFPS = new Multimedia.Timer();
            TimerFPS.Period = 1000;
            TimerFPS.Tick += new EventHandler(TimerFPS_Tick);
            /*
            TimerFPS = new System.Timers.Timer();
            TimerFPS.Interval = 1000;
            TimerFPS.Elapsed += new System.Timers.ElapsedEventHandler(TimerFPS_Tick);
            TimerFPS.Stop();
            */ 
            //Точный таймер подсчета времени
            stopwatch = new Stopwatch();
            //Подсказка
            tooltip = new ToolTip();
            tooltip.ShowAlways = true;

            CheckExportLength = true;
        }

        #region UI

        /// <summary>Панель-контейнер всех элементов</summary>
        public Control Owner
        {
            set
            {
                owner = value;
            }
            get
            {
                return owner;
            }
        }

        /// <summary>Автоматически упорядочивает элементы управления
        /// (помещает на FlowLayoutPanel)</summary>
        public bool FlowControls
        {
            get { return flow_controls; }
            set { flow_controls = true; }
        }
        
        /// <summary>Отображает ползунок прокрутки</summary>
        public bool ShowScrollBar
        {
            get { return show_scroll_bar; }
            set { show_scroll_bar = true; }
        }

        /// <summary>Разрешить регулировку скорости воспроизведения</summary>
        public bool AllowSetSpeed
        {
            get { return allow_set_speed; }
            set { allow_set_speed = value; }
        }

        /// <summary>Разрешить упрощенный (узкий) резулятор скорости воспроизведения</summary>
        public bool SetSimpleSpeedScroll
        {
            get { return simple_speed; }
            set { simple_speed = value; }
        }

        /// <summary>Отображать длительность и текущее время в отдельном контейнере</summary>
        public bool ShowLengthInGroupBox
        {
            get { return show_sep_length; }
            set { show_sep_length = value; }
        }

        /// <summary>Упрощенный индикатор длительности</summary>
        public bool SimpleLength
        {
            get { return simple_length; }
            set { simple_length = value; }
        }

        /// <summary>Разрешить экспорт</summary>
        public bool EnableExport { get; set; }

        /// <summary>Включить проверку длительности фрагмента для экспорта</summary>
        public bool CheckExportLength { get; set; }

        /// <summary>Создание пользовательского интерфейса плеера</summary>
        public void CreateUI()
        {
            if (owner == null) return;
            //Создание ползунка прокрутки
            int offset = 3;
            int left = 7;

            #region Полоса прокрутки видео

            if (show_scroll_bar)
            {
                //Метка времени
                if (!show_sep_length)
                {
                    //Если отображать над ползунком
                    label_time = new Label();
                    //label_time.BorderStyle = BorderStyle.Fixed3D;
                    label_time.Text = "00:00";
                    label_time.Left = left;
                    label_time.Top = offset;
                    label_time.AutoSize = false;
                    label_time.Height = 15;
                    label_time.Width = 36;
                    label_time.TextAlign = ContentAlignment.MiddleCenter;
                    label_time.Parent = owner;
                    offset += label_time.Height;
                }
                //Ползунок
                scroll = new SmoothProgressBar();
                scroll.Parent = owner;
                scroll.Left = left;
                if (!show_sep_length) scroll.Top = offset;
                else scroll.Top = offset;
                scroll.Width = owner.Width - left * 2;
                scroll.Height = 13;
                scroll.ProgressBarColor1 = Color.LightSteelBlue;
                scroll.ProgressBarColor2 = Color.RoyalBlue;
                scroll.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                scroll.Maximum = scroll.Width;
                scroll.SizeChanged += new EventHandler(scroll_SizeChanged);
                scroll.MouseDown += new MouseEventHandler(scroll_MouseDown);
                scroll.MouseMove += new MouseEventHandler(scroll_MouseMove);
                SetScrollBarThreadSafe(scroll, 0);
                
                //Отступ до следующего контрола
                offset += scroll.Height;
            }

            #endregion

            #region Кнопки управления видео

            if (flow_controls)
            {
                flow_panel = new FlowLayoutPanel();
                flow_panel.Left = left;
                flow_panel.Top = offset;
                flow_panel.Width = owner.Width - left;
                flow_panel.Height = owner.Height - offset - 3;
                flow_panel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                flow_panel.Resize += new EventHandler(flow_panel_Resize);    
                flow_panel.AutoSize = true;
                flow_panel.Parent = owner;
            }
            foreach (PlayerButtonInfo[] button_group in Buttons)
            {
                ToolStrip tool_strip = new ToolStrip();
                tool_strip.BackColor = Color.Transparent;
                tool_strip.Dock = DockStyle.None;
                tool_strip.GripStyle = ToolStripGripStyle.Hidden;
                tool_strip.RenderMode = ToolStripRenderMode.System;
                foreach (PlayerButtonInfo button in button_group)
                {
                    ToolStripButton item = new ToolStripButton(button.Image);
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    item.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    item.Name = button.Type.ToString();
                    item.Tag = button.Type;
                    item.Click += new EventHandler(item_Click);
                    //Установка фокуса при движении мышью
                    item.MouseMove += new MouseEventHandler(item_MouseMove);
                    if (button.Type == PlayerButtonType.PrevFrame ||
                        button.Type == PlayerButtonType.NextFrame)
                    {
                        item.MouseDown += new MouseEventHandler(item_MouseDown);
                        item.MouseUp += new MouseEventHandler(item_MouseUp);
                    }
                    tool_strip.Items.Add(item);
                }
                if (flow_controls) tool_strip.Parent = flow_panel;
                else tool_strip.Parent = owner;
            }

            #endregion

            #region Кнопки экспорта

            if (EnableExport)
            {
                tool_strip_export = new ToolStrip();
                tool_strip_export.BackColor = Color.Transparent;
                tool_strip_export.Dock = DockStyle.None;
                tool_strip_export.GripStyle = ToolStripGripStyle.Hidden;
                tool_strip_export.RenderMode = ToolStripRenderMode.System;
                tool_strip_export.Visible = false;

                CreateSeparator(tool_strip_export);
                for (int i = 0; i < ExportButtons.Length; i++)
                {
                    CreateButton(tool_strip_export, ExportButtons[i].Image, ExportButtons[i].Type, ExportButtons[i].ToolTip);
                }
                CreateSeparator(tool_strip_export);

                if (flow_controls) tool_strip_export.Parent = flow_panel;
                else tool_strip_export.Parent = owner;
            }

            #endregion

            #region Ползунок скорости

            if (allow_set_speed)
            {
                gb = new GroupBox();
                gb.Width = 185;
                gb.Height = 51;
                //gb.Text = "Скорость воспроизведения";
                gb.Text = "";
                gb.MouseHover += new EventHandler(h_scroll_MouseHover);
                gb.MouseLeave += new EventHandler(h_scroll_MouseLeave);
                //Ползунок
                HScrollBar h_scroll = new HScrollBar();
                h_scroll.Minimum = 0;
                h_scroll.Maximum = 109;
                h_scroll.Top = 12;
                h_scroll.Width = gb.Width - 20;
                h_scroll.Left = (gb.ClientSize.Width - h_scroll.Width) / 2;
                h_scroll.Parent = gb;
                h_scroll.SmallChange = 25;
                h_scroll.Value = 50;
                h_scroll.Scroll += new ScrollEventHandler(h_scroll_Scroll);
                h_scroll.MouseHover += new EventHandler(h_scroll_MouseHover);
                h_scroll.MouseLeave += new EventHandler(h_scroll_MouseLeave);
                h_scroll.Name = "h_scrollSpeed";
                //Подписи
                if (simple_speed)
                {
                    gb.Height = 36;
                }
                else
                {
                    /*
                    Bitmap image = new Bitmap(gb.ClientSize.Width, gb.ClientSize.Height);
                    Graphics g = Graphics.FromImage(image);
                    //Pen p = new Pen(Brushes.Blue, 1);
                    Font fnt = new Font("Arial", 8, FontStyle.Regular);
                    g.DrawString("1/4x", fnt, Brushes.Gray,
                                 h_scroll.Left + 20, h_scroll.Top + h_scroll.Height + 3);
                    g.DrawString("1/2x", fnt, Brushes.Gray,
                                 h_scroll.Left + 44, h_scroll.Top + h_scroll.Height + 3);
                    g.DrawString("2x", fnt, Brushes.Gray,
                                 h_scroll.Left + h_scroll.Width / 2 + 25, h_scroll.Top + h_scroll.Height + 3);
                    g.DrawString("4x", fnt, Brushes.Gray,
                                 h_scroll.Left + h_scroll.Width / 2 + 50, h_scroll.Top + h_scroll.Height + 3);
                    gb.BackgroundImage = image;
                    */
                    CreateLabel(gb, "1/4x", h_scroll.Left + 8, h_scroll.Top + h_scroll.Height + 3, 30, 17);
                    CreateLabel(gb, "1/2x", h_scroll.Left + 35, h_scroll.Top + h_scroll.Height + 3, 30, 17);
                    CreateLabel(gb, "2x", h_scroll.Left + h_scroll.Width / 2 + 25, h_scroll.Top + h_scroll.Height + 3, 20, 17);
                    CreateLabel(gb, "4x", h_scroll.Left + h_scroll.Width / 2 + 50, h_scroll.Top + h_scroll.Height + 3, 20, 17);
                    //Кнопка единичной скорости
                    Button button = new Button();
                    button.Width = 26;
                    button.Height = 20;
                    button.Left = h_scroll.Left + h_scroll.Width / 2 - button.Width / 2;
                    button.Top = h_scroll.Top + h_scroll.Height;
                    button.Text = "1x";
                    button.TextAlign = ContentAlignment.MiddleCenter;
                    button.Click += new EventHandler(button_Click);
                    button.Parent = gb;
                }
                //Добавление резулятора скорости
                if (flow_controls) gb.Parent = flow_panel;
                else gb.Parent = owner;
            }

            #endregion

            #region Отображать длительность и текущее время видео отдельно

            if (show_sep_length)
            {
                gbLength = new GroupBox();
                gbLength.Width = 185;
                gbLength.Top = 0;
                //Добавление резулятора скорости
                if (flow_controls) gbLength.Parent = flow_panel;
                else gbLength.Parent = owner;

                if (simple_length)
                {
                    gbLength.Height = 36;
                    //Метка "Время"
                    lableTimeName = new Label();
                    lableTimeName.Text = "Время:";
                    lableTimeName.AutoSize = true;
                    lableTimeName.Parent = gbLength;
                    lableTimeName.Left = 10;
                    lableTimeName.Top = 12;
                    //Метка текущего времени
                    lableTime = new Label();
                    lableTime.Parent = gbLength;
                    lableTime.AutoSize = false;
                    lableTime.Width = 132;
                    lableTime.Left = 50;
                    lableTime.Top = 12;
                    lableTime.Height = 13;
                    lableTime.TextAlign = ContentAlignment.MiddleRight;
                }
                else
                {
                    gbLength.Height = 51;
                    //Метка "Время"
                    lableTimeName = new Label();
                    lableTimeName.Text = "Время:";
                    lableTimeName.AutoSize = true;
                    lableTimeName.Parent = gbLength;
                    lableTimeName.Left = 10;
                    lableTimeName.Top = 12;
                    //Метка текущего времени
                    lableTime = new Label();
                    lableTime.Parent = gbLength;
                    lableTime.AutoSize = false;
                    lableTime.Width = 60;
                    lableTime.Left = 116;
                    lableTime.Top = 12;
                    lableTime.Height = 13;
                    lableTime.TextAlign = ContentAlignment.MiddleRight;
                    //Метка длительности
                    lableLengthName = new Label();
                    lableLengthName.Parent = gbLength;
                    lableLengthName.Text = "Длительность:";
                    lableLengthName.Left = 10;
                    lableLengthName.Top = 30;
                    lableLengthName.AutoSize = true;
                    //Метка длительности
                    lableLength = new Label();
                    lableLength.Parent = gbLength;
                    lableLength.Width = 60;
                    lableLength.Left = 116;
                    lableLength.Top = 30;
                    lableLength.Height = 13;
                    lableLength.AutoSize = false;
                    lableLength.TextAlign = ContentAlignment.MiddleRight;
                }
            }

            #endregion

            SetEnabled(false);

        }

        Label CreateLabel(Control parent, string s, int x, int y, int w, int h)
        {
            Label lbl = new Label();
            lbl.Text = s;
            lbl.AutoSize = false;
            lbl.Parent = parent;
            lbl.Left = x;
            lbl.Top = y;
            lbl.Width = w;
            lbl.Height = h;
            return lbl;
        }

        /// <summary>Создание кнопки</summary>
        /// <param name="st">Панель кнопки</param>
        /// <param name="image">Картинка</param>
        /// <param name="type">Тип кнопки</param>
        void CreateButton(ToolStrip st, Image image, PlayerButtonType type, string ToolTip)
        {
            ToolStripButton item = new ToolStripButton(image);
            item.ImageScaling = ToolStripItemImageScaling.None;
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            item.Tag = (int)type;
            item.Click += new EventHandler(item_Click);
            //item.Width = 52;
            //item.Height = 52;
            item.AutoSize = true;
            item.ToolTipText = ToolTip;
            //Установка фокуса при движении мышью
            item.MouseMove += new MouseEventHandler(item_MouseMove);
            st.Items.Add(item);
        }

        void CreateSeparator(ToolStrip st)
        {
            ToolStripSeparator item = new ToolStripSeparator();
            st.Items.Add(item);
        }

        /// <summary>Установка кнопок пользовательского интерфейса</summary>
        /// <param name="groupsbuttons">Массив групп кнопок</param>
        public void SetButtons(List<PlayerButtonInfo[]> groupsbuttons)
        {
            Buttons = groupsbuttons;
        }

        /// <summary>Установка кнопок пользовательского интерфейса</summary>
        /// <param name="groupsbuttons">Массив групп кнопок</param>
        public void SetExportButtons(PlayerButtonInfo[] groupsbuttons)
        {
            ExportButtons = groupsbuttons;
        }

        /// <summary>Изменение кнопки</summary>
        public void ChangeButtonThreadSafe(PlayerButtonInfo button)
        {
            if (owner.InvokeRequired)
            {
                ChangeButtonThreadSafeCallback d = new ChangeButtonThreadSafeCallback(ChangeButtonThreadSafe);
                owner.Invoke(d, new object[] { button });
            }
            else
            {
                ChangeButton(button);
            }
        }

        /// <summary>Изменение кнопки</summary>
        void ChangeButton(PlayerButtonInfo button)
        {
            Control parent = owner;
            if (flow_controls) parent = flow_panel;
            for (int i = 0; i < parent.Controls.Count; i++)
            {
                if (parent.Controls[i].GetType().Name == "ToolStrip")
                {
                    for (int j = 0; j < ((ToolStrip)parent.Controls[i]).Items.Count; j++)
                    {
                        if (((ToolStrip)parent.Controls[i]).Items[j].GetType().Name == "ToolStripButton")
                        {
                            ToolStripButton btn = (ToolStripButton)((ToolStrip)parent.Controls[i]).Items[j];
                            if ((PlayerButtonType)btn.Tag == button.Type)
                            {
                                btn.Image = button.Image;
                                btn.ToolTipText = button.ToolTip;
                                return;
                            }
                        }
                    }
                }
            }
        }

        #region Speed scroll

        void h_scroll_MouseHover(object sender, EventArgs e)
        {
            if (tooltip.Active) return;
            tooltip.Active = true;
            tooltip.Show("Скорость воспроизведения " + Math.Abs(Speed).ToString("0.00") + "x", gb, 5, -6, 5000);
        }

        void h_scroll_MouseLeave(object sender, EventArgs e)
        {
            tooltip.Active = false;
        }

        /// <summary>Перемещение ползунка скорости</summary>
        void h_scroll_Scroll(object sender, ScrollEventArgs e)
        {
            tooltip.Active = true;
            //int val = e.NewValue;//Новое значение ползунка
            //double s = 1.0d;
            /*
            int center = 50;            //Центральная точка скроллбара - скорость 1х
            double center_speed = 1.0d; //Центральная скорость - 1х
            double max_speed = 4.0d;    //Максимальная скорость
            double min_speed = 0.25d;   //Минимальная скорость
            if (val > center)//Скорость более 1х
            {
                s = center_speed + ((1.0d * val - center) / (1.0d * 100 - center)) * 
                    (max_speed - center_speed);
            }
            if (val < center)//Скорость менее 1х
            {
                s = min_speed + ((1.0d * val) / (1.0d * center)) * 
                    (center_speed - min_speed);
            }
            if (val == center) s = 1.0d;
            */
            double s = GetScrollSpeed(e.NewValue);
            tooltip.Show("Скорость воспроизведения " + s.ToString("0.00") + "x", gb, 5, -6, 5000);
            SetSpeed(s * ((Math.Abs(Speed)) / Speed));
        }

        double GetScrollSpeed(int ScrollValue)
        {
            double s = 1.0d;
            int center = 50;            //Центральная точка скроллбара - скорость 1х
            double center_speed = 1.0d; //Центральная скорость - 1х
            double max_speed = 4.0d;    //Максимальная скорость
            double min_speed = 0.25d;   //Минимальная скорость
            if (ScrollValue > center)//Скорость более 1х
            {
                s = center_speed + ((1.0d * ScrollValue - center) / (1.0d * 100 - center)) *
                    (max_speed - center_speed);
            }
            if (ScrollValue < center)//Скорость менее 1х
            {
                s = min_speed + ((1.0d * ScrollValue) / (1.0d * center)) *
                    (center_speed - min_speed);
            }
            if (ScrollValue == center) s = 1.0d;
            return s;
        }

        /// <summary>Кнопка установки единичной скорости</summary>
        void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            HScrollBar scroll = (HScrollBar)button.Parent.Controls.Find("h_scrollSpeed", false)[0];
            scroll.Value = 50;
            h_scroll_Scroll(scroll, new ScrollEventArgs(ScrollEventType.EndScroll, 50));
        }

        #endregion

        #region Position scroll

        /// <summary>Изменение размера ползунка</summary>
        void scroll_SizeChanged(object sender, EventArgs e)
        {
            scroll.Maximum = scroll.Width;
            //Установка ползунка при изменении его размера 
            scroll.Refresh();
            //Изменение положения ползунка при изменении общего размера полосы прокрутки
            int pos = (int)Math.Round((1.0d * TimeStamp * scroll.Maximum) / this.Length);
            SetScrollBarThreadSafe(scroll, pos);
        }

        /// <summary>Клик на ползунок</summary>
        void scroll_MouseDown(object sender, MouseEventArgs e)
        {
            SetScrollPos(e.X);
        }

        void scroll_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) SetScrollPos(e.X);
        }

        #endregion

        /// <summary>Установка кадров от позиции ползунка</summary>
        /// <param name="pos">Позиция ползунка</param>
        void SetScrollPos(int pos)
        {
            if (set_scroll) return;
            set_scroll = true;
            Pause();
            //Ограничение границ
            if (pos < 0) pos = 0;
            if (pos > scroll.Width) pos = scroll.Width;
            //Расчет текущей позиции
            int timestamp = (int)(pos * this.Length / scroll.Maximum);
            if (TimeStamp == timestamp)
            {
                set_scroll = false;
                return;
            }
            TimeStamp = timestamp;
            GetFrames(TimeStamp, _delta);
            //Установка положения ползунка
            SetScrollBarThreadSafe(scroll, pos);
            //Проверка изменения метки
            AnalyzeMarks();
            set_scroll = false;
        }

        /// <summary>Обработка нажатий на кнопки</summary>
        void item_Click(object sender, EventArgs e)
        {
            //Воспроизведение назад
            string type = Enum.GetName((new PlayerButtonType()).GetType(), ((ToolStripButton)sender).Tag);
            if (type == PlayerButtonType.PlayBack.ToString())
            {
                SetSpeed(Math.Abs(Speed) * (-1));//Установка скорости с отрицательным знаком
                //Pause();
                Play();
                ButtonDown(PlayerButtonType.PlayBack);
            }
            //Пауза
            if (type == PlayerButtonType.Pause.ToString())
            {
                Pause();
            }
            //Воспроизведение вперед
            if (type == PlayerButtonType.Play.ToString())
            {
                SetSpeed(Math.Abs(Speed));//Установка скорости с положительным знаком
                //Pause();
                Play();
                ButtonDown(PlayerButtonType.Play);
            }
            //Предыдущий кадр
            if (type == PlayerButtonType.PrevFrame.ToString())
            {
                Pause();
                MoveToFrame(MovingDirection.Prev);
            }
            //Следующий кадр
            if (type == PlayerButtonType.NextFrame.ToString())
            {
                Pause();
                MoveToFrame(MovingDirection.Next);
            }
            //Первые кадры
            if (type == PlayerButtonType.FirstFrame.ToString())
            {
                Pause();
                MoveToFrame(MovingDirection.First);
            }
            //Последние кадры
            if (type == PlayerButtonType.LastFrame.ToString())
            {
                Pause();
                MoveToFrame(MovingDirection.Last);
            }
            if (type == PlayerButtonType.PrevMark.ToString())
            {
                Pause();
                MoveToMark(MovingDirection.Prev);
            }
            if (type == PlayerButtonType.NextMark.ToString())
            {
                Pause();
                MoveToMark(MovingDirection.Next);
            }
            //Марка старт экспорта
            if (type == PlayerButtonType.MarkStartExport.ToString())
            {
                start_export = TimeStamp;
                int value = (int)(scroll.Maximum * ((1.0d * TimeStamp) / this.Length));
                scroll.MarkStart = value;
                scroll.Mark = true;
            }
            //Марка стоп экспорта
            if (type == PlayerButtonType.MarkStopExport.ToString())
            {
                stop_export = TimeStamp;
                int value = (int)(scroll.Maximum * ((1.0d * TimeStamp) / this.Length));
                scroll.MarkStop = value;
                scroll.Mark = true;
            }
            //Экспорт
            if (type == PlayerButtonType.Export.ToString())
            {
                if (!CheckExportLength || (CheckExportLength && (start_export < stop_export)))
                    if (EventExport != null) EventExport(start_export, stop_export);
            }
        }

        /// <summary>Нажатие на кнопку следующий/предыдущий кадр</summary>
        void item_MouseDown(object sender, MouseEventArgs e)
        {
            ((ToolStripButton)sender).Checked = true;
            //Запуск таймера удержания кнопки
            TimerNextFrame = new System.Timers.Timer();
            TimerNextFrame.Interval = 200;
            TimerNextFrame.Elapsed += new System.Timers.ElapsedEventHandler(TimerNextFrame_Elapsed);
            TimerNextFrame.Start();
        }

        /// <summary>Отпускание кнопки  следующий/предыдущий кадр</summary>
        void item_MouseUp(object sender, MouseEventArgs e)
        {
            if (curSpeed != 0.0d) Speed = curSpeed;
            TimerNextFrame.Stop();
            ((ToolStripButton)sender).Checked = false;
            Pause();
        }

        void item_MouseMove(object sender, MouseEventArgs e)
        {
            if (EventMouseMove != null) EventMouseMove();
            //((ToolStripButton)sender).Owner.Focus();
        }

        /// <summary>Активация элементов управления</summary>
        void SetEnabled(bool enabled)
        {
            if (scroll != null)
            {
                if (label_time != null) label_time.Visible = enabled;
                scroll.Enabled = enabled;
                SetScrollBarThreadSafe(scroll, 0);
            }
            if (label_time != null) label_time.Enabled = enabled;
            if (flow_panel != null) flow_panel.Enabled = enabled;
            if (!enabled) ButtonsUp();//"Поднятие" кнопок
            else ButtonDown(PlayerButtonType.Pause);//Нажатие паузы при активации панели
        }

        /// <summary>Поиск ToolStripButton заданного типа PlayerButtonType</summary>
        /// <param name="type">Тип кнопки PlayerButtonType</param>
        /// <returns>Объект кнопки</returns>
        ToolStripButton SearchToolStripButton(PlayerButtonType type)
        {
            if (owner == null) return null;
            Control parent;
            if (flow_controls) parent = flow_panel;
            else parent = owner;
            foreach (Control control in parent.Controls)
                if (control.GetType().Name == "ToolStrip")
                    foreach (ToolStripButton button in ((ToolStrip)control).Items)
                        if (button.Tag.ToString() == type.ToString()) return button;
            return null;
        }

        /// <summary>"Нажатие" кнопки</summary>
        public void ButtonDown(PlayerButtonType type)
        {
            if (owner == null) return;
            Control parent;
            if (flow_controls) parent = flow_panel;
            else parent = owner;
            foreach (Control control in parent.Controls)
                if (control.GetType().Name == "ToolStrip")
                    foreach (ToolStripItem button in ((ToolStrip)control).Items)
                    {
                        if (button.GetType().Name == "ToolStripButton")
                            ButtonThreadSafe((ToolStripButton)button, button.Tag.ToString() == type.ToString());
                    }
        }

        /// <summary>"Снятие" нажатия всех кнопок</summary>
        void ButtonsUp()
        {
            if (owner == null) return;
            Control parent;
            if (flow_controls) parent = flow_panel;
            else parent = owner;
            foreach (Control control in parent.Controls)
                if (control.GetType().Name == "ToolStrip")
                    foreach (ToolStripItem button in ((ToolStrip)control).Items)
                        if (button.GetType().Name == "ToolStripButton")
                            ((ToolStripButton)button).Checked = false;
        }

        void flow_panel_Resize(object sender, EventArgs e)
        {
            if (EventPanelResize != null) 
                EventPanelResize(flow_panel.Width, flow_panel.Height + flow_panel.Top);
        }

        #endregion

        #region Objects

        /// <summary>Объект видеохранилище, с которым работает плеер</summary>
        public IVideoStorage ObjVideoStorage
        {
            get { return obj_VideoStorage;}
            set { obj_VideoStorage = value;}
        }

        /// <summary>Получить теекущий объект чтения видео</summary>
        public IVideoReader ObjVideoReader
        {
            get { return obj_VideoReader; }
        }

        #endregion

        #region Public Properties

        /// <summary>Мастер канал воспроизведения</summary>
        public int MasterChannel
        {
            get { return master_channel; }
            set { master_channel = value; }
        }

        /// <summary>Допуск поиска кадров</summary>
        public int Delta
        {
            get { return _delta; }
            set { _delta = value; }
        }

        /// <summary>Состояние открытия видеофайла</summary>
        public bool Opened
        {
            get { return FOpened; }
            set { FOpened = value; }
        }

        /// <summary>Состояние паузы</summary>
        public bool Paused
        {
            get { return PauseFlag; }
            set { PauseFlag = value; }
        }

        /// <summary>Общая длительность видеопотока в миллисекундах (0 в случае ошибки)</summary>
        public int Length
        {
            get 
            { 
                if (obj_VideoReader != null) return length;
                else return -1;
            }
        }

        /// <summary>Массив меток времени (в миллисекундах)</summary>
        public int[] Marks
        {
            get { return marks; }
            set 
            {
                //if (value != null && value.Length > 0)
                //if (value != null)
                //{
                marks = value;
                current_mark = GetCurrentMark();//Текущая метка
                //}
            }
        }

        /// <summary>Разрешение функций экспорта</summary>
        public bool ActiveExport
        {
            get 
            { 
                return active_export; 
            }
            set 
            { 
                active_export = value;
                if (tool_strip_export != null)
                {
                    tool_strip_export.Visible = active_export;
                    scroll.Mark = value;
                    if (!value)
                    {
                        start_export = 0;
                        stop_export = 0;
                        scroll.MarkStart = 0;
                        scroll.MarkStop = 0;
                    }
                    if (EventEnableExport != null) EventEnableExport(active_export);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Открыть файл</summary>
        /// <param name="id">Идентификатор видеозаписи</param>
        /// <param name="partition_id">Идентификатор раздела</param>
        /// <returns>Результат операции</returns>
        public bool Open(string id, int partition_id)
        {
            TimerPlay.Stop();
            stopwatch.Reset();
            FOpened = false;
            if (obj_VideoStorage == null) return false;
            obj_VideoReader = obj_VideoStorage.GetReader(id, partition_id);
            if (obj_VideoReader.Status != VideoStorageIntStat.Ok)
            {
                obj_VideoReader.Close();
                obj_VideoReader = null;
                return false;
            }
            //Установка на начало
            TimeStamp = 0;
            Speed = 1;
            FOpened = true;
            IList<VideoStreamInfo> info = obj_VideoReader.VideoIndex.StreamInfoList;
            //Сохранение идентификаторов потоков
            stream_id = new int[info.Count];
            last_played_index = new int[info.Count];
            last_fps = new int[info.Count];
            for (int i = 0; i < info.Count; i++)
                stream_id[i] = obj_VideoReader.VideoIndex.StreamInfoList[i].Id;
            //Вычисление длины видеопотока
            length = obj_VideoReader.VideoIndex.GetFinishTime(0) - obj_VideoReader.VideoIndex.GetStartTime(0);
            //Корректировка мастер-канала
            bool is_master = false;
            for (int i = 0; i < info.Count; i++)
                if (info[i].Id == master_channel)
                {
                    is_master = true;
                    break;
                }
            //Если указанный мастер-канал отсутствует среди каналов
            //то выбираем первый видеопоток
            if (!is_master && info.Count > 0) master_channel = info[info.Count-1].Id;

            //Активация контролов
            SetEnabled(true);

            //Установка скорости
            HScrollBar hscroll = (HScrollBar)owner.Controls.Find("h_scrollSpeed", true)[0];
            double s = GetScrollSpeed(hscroll.Value);
            tooltip.Show("Скорость воспроизведения " + s.ToString("0.00") + "x", gb, 5, -6, 5000);
            SetSpeed(s * ((Math.Abs(Speed)) / Speed));

            //Вывод длительности потока
            ShowLength(length);

            //Сброс экспорта
            active_export = false;

            //Запуск таймера fps
            TimerFPS.Start();
            //Событие открытия
            if (EventOpenVideo != null) EventOpenVideo(info);
            return true;
        }

        /// <summary>Закрытие файла</summary>
        public bool Close()
        {
            FOpened = false;
            TimerFPS.Stop();
            if (obj_VideoStorage == null) return false;
            if (obj_VideoReader != null)
            {
                if (obj_VideoReader.Close() == VideoStorageResult.Ok)
                {
                    //Деактивация контролов
                    SetEnabled(false);
                    //Сброс экспорта
                    ActiveExport = false;
                    //Метки времени
                    if (lableTime != null) SetLabelTextThreadSafe(lableTime, "");
                    if (lableLength != null) SetLabelTextThreadSafe(lableLength, "");
                    //SetScrollBarThreadSafe(scroll, 0);
                    //Событие закрытия видео
                    if (EventCloseVideo != null) EventCloseVideo();
                    return true;
                }
            }
            return false;
        }

        /// <summary>Установка кадра на метку</summary>
        /// <param name="mark">Номер метки с 0</param>
        /// <param name="checkchange">Проверять изменение текущей метки</param>
        public void SetMark(int mark, bool checkchange)
        {
            if (!FOpened) return;
            MoveToMark(mark, checkchange);
        }

        #endregion

        /// <summary>Вывод длительности потока</summary>
        /// <param name="length">Длительность потока</param>
        void ShowLength(int length)
        {
            //Длительность потока
            int min = (int)((int)(length / 1000) / 60);
            int sec = (length - min * 60 * 1000) / 1000;
            int ms = length - min * 60 * 1000 - sec * 1000;
            string text = "";
            if (min > 0) text += min.ToString() + ":";
            text += sec.ToString("00") + "." + ms.ToString("000");
            if (lableLength != null) SetLabelTextThreadSafe(lableLength, text);
        }

        /// <summary>Установка скорости воспроизведения
        /// отрицательное значение - воспроизведение назад</summary>
        /// <param name="value">Значение скорости</param>
        void SetSpeed(double value)
        {
            Speed = value;
            if (Double.IsNaN(Speed)) 
                Speed = 1.0d;
        }

        /// <summary>Воспроизвести файл</summary>
        public void Play()
        {
            if (!FOpened) return;
            TimerPlay.Stop();
            stopwatch.Reset();
            stopwatch.Stop();
            LastTimeStamp = 0;
            PauseFlag = false;
            TimerPlay.Start();
            stopwatch.Start();
        }

        /// <summary>Пауза воспроизведения</summary>
        public void Pause()
        {
            if (!FOpened) return;
            if (PauseFlag) return;
            ButtonDown(PlayerButtonType.Pause);
            TimerPlay.Stop();
            stopwatch.Reset();
            stopwatch.Stop();
            LastTimeStamp = 0;
            PauseFlag = true;
            for (int i = 0; i < stream_id.Length; i++)
                if (EventFPS != null) EventFPS(stream_id[i], 0);
        }

        /// <summary>Установка или получение текущей позиции видеопотока</summary>
        public long Position
        {
            get
            {
                return TimeStamp;
            }
            set
            {
                if (!FOpened) return;
                if (value != TimeStamp || value == 0) GetFrames(value, _delta);
                if (show_scroll_bar) 
                    SetScrollBarThreadSafe(scroll, (int)(scroll.Maximum * ((1.0d * value) / this.Length)));
                //Проверка текущей метки
                AnalyzeMarks();
                //Предыдущее время воспроизводимого видео 
                LastTimeStamp = 0;
            }
        }

        /// <summary>Запрос текущих кадров</summary>
        public void RefreshFrames()
        {
            GetFrames(TimeStamp, _delta);
        }

        /// <summary>Срабатывание таймера воспроизведения кадров</summary>
        void TimerPlay_Tick(object sender, EventArgs e)
        {
            TimerPlay.Stop();
            if (PauseFlag) return;
            try
            {
                //Текущий момент времени по таймеру
                int now = (int)stopwatch.ElapsedMilliseconds;
                //Интервал с прошлого воспроизведения срабатывания таймера
                int interval = now - LastTimeStamp;
                //Сохранение прошлого времени воспроизведения
                LastTimeStamp = now;
                //Установка новой позиции видео
                SetPosition((int)TimeStamp, (int)(TimeStamp + Speed * interval), _delta);
                TimeStamp += (int)(Speed * interval);
                //Проверка текущей метки
                AnalyzeMarks();
                //Проверка достижения начала или конца видеозаписи
                int start = obj_VideoReader.VideoIndex.GetStartTime(0);
                if (TimeStamp < start && Speed < 0)
                {
                    TimeStamp = start;
                    Pause();
                    return;
                }
                int finish = obj_VideoReader.VideoIndex.GetFinishTime(0);
                if (TimeStamp > finish && Speed > 0)
                {
                    TimeStamp = finish;
                    Pause();
                    return;
                }
            }
            catch
            {
                //Если произошло закрытие видео - установить ползунок прогресса в 0
                if (!FOpened && show_scroll_bar) SetScrollBarThreadSafe(scroll, 0);
            }
            //Перезапуск таймера если не на паузе и не достугнуты границы видео
            if (!PauseFlag) TimerPlay.Start();
        }

        /// <summary>Срабатывание таймера fps (кадров в секунду)</summary>
        void TimerFPS_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < stream_id.Length; i++)
            {
                int fps = last_played_index[i] - last_fps[i];
                if (PauseFlag) fps = 0;
                if (EventFPS != null) EventFPS(stream_id[i], Math.Abs(fps));
                last_fps[i] = last_played_index[i];
            }
        }

        /// <summary>Запрос кадров для всех видеопотоков</summary>
        /// <param name="current_time">Предыдущая метка времени</param>
        /// <param name="next_time">Следующая метка времени</param>
        /// <param name="delta">Допуск поиска кадров</param>
        void SetPosition(int current_time, int next_time, int delta)
        {
            if (obj_VideoReader == null) return;
            IList<VideoStreamInfo> info = obj_VideoReader.VideoIndex.StreamInfoList;
            //Перебор видеопотоков
            for (int i = 0; i < info.Count; i++)
            {
                //Воспроизведение вперед
                if (next_time > current_time)
                {
                    //Получение метки времени следующего кадра для видеопотока
                    int next_frame_time = obj_VideoReader.VideoIndex.GetNextFrameTime(info[i].Id, current_time);
                    //Метка следующего кадра достигнута 
                    if (next_time >= next_frame_time && last_played_time != next_frame_time)
                    {
                        GetFrame(info[i].Id, next_time, delta);
                        last_played_time = next_frame_time;
                        last_played_index[i] = obj_VideoReader.VideoIndex.GetFrameIndex(info[i].Id, current_time);
                    }
                }
                //Воспроизведение назад
                if (next_time < current_time)
                {
                    //Получение метки времени предыдущего кадра для видеопотока
                    int prev_frame_time = obj_VideoReader.VideoIndex.GetFrameTime(info[i].Id, current_time);
                    //Метка следующего кадра достигнута
                    if (next_time <= prev_frame_time && last_played_time != prev_frame_time)
                    {
                        GetFrame(info[i].Id, next_time, delta);
                        last_played_time = prev_frame_time;
                        last_played_index[i] = obj_VideoReader.VideoIndex.GetFrameIndex(info[i].Id, current_time);
                    }
                }
                if (PauseFlag) last_fps[i] = last_played_index[i];
            }
            //Ползунок
            if (show_scroll_bar)
            {
                int value = (int)(scroll.Maximum * ((1.0d * next_time) / this.Length));//Ползунок
                SetScrollBarThreadSafe(scroll, value);
            }
        }

        #region Get Frames

        /// <summary>Запрос кадров в режиме паузы</summary>
        void GetFrames(long timestamp, int delta)
        {
            //Запрос кадров всех телекамер
            IList<VideoStreamInfo> info = obj_VideoReader.VideoIndex.StreamInfoList;
            //Перебор видеопотоков
            for (int i = 0; i < info.Count; i++)
            {
                GetFrame(info[i].Id, timestamp, delta);
                last_played_index[i] = obj_VideoReader.VideoIndex.GetFrameIndex(info[i].Id, (int)timestamp);
                last_fps[i] = last_played_index[i];
            }
        }

        /// <summary>Запрос кадров без мастер-канала</summary>
        void GetFramesWithoutMaster(long timestamp, int delta)
        {
            IList<VideoStreamInfo> info = obj_VideoReader.VideoIndex.StreamInfoList;
            //Запрос кадров без мастер канала
            foreach (VideoStreamInfo stream in info)
                if (stream.Id != master_channel) GetFrame(stream.Id, timestamp, delta);
        }

        /// <summary>Запрос для канала</summary>
        /// <param name="channel_id">Идентификатор видеопотока</param>
        /// <param name="timestamp">Метка времени видеопотока от начала воспроизведения</param>
        /// <param name="delta">Допуск поиска кадров в мс</param>
        public void GetFrame(int channel_id, long timestamp, int delta)
        {
            if (obj_VideoReader == null) return;
            VideoFrame frame;
            if (obj_VideoReader.ReadFrame(channel_id, (int)timestamp, delta, out frame) == VideoStorageResult.Ok)
                if (EventNewFrame != null) EventNewFrame(frame.ContentType.Width, frame.ContentType.Height,
                                                         channel_id, frame.FrameData, frame.TimeStamp);
        }

        #endregion

        /// <summary>Таймер перехода к следующим кадрам при удерживании кнопки</summary>
        void TimerNextFrame_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimerNextFrame.Stop();
            ToolStripButton button;
            button = SearchToolStripButton(PlayerButtonType.NextFrame);
            if (button != null && button.CheckState == CheckState.Checked)
            {
                curSpeed = Speed;
                SetSpeed(Math.Abs(1/*Speed*/));
                Play();
                return;
            }
            button = SearchToolStripButton(PlayerButtonType.PrevFrame);
            if (button != null && button.CheckState == CheckState.Checked) Play();
            {
                curSpeed = Speed;
                SetSpeed(Math.Abs(1/*Speed*/) * (-1));
                Play();
                return;
            }
        }

        /// <summary>Следующий, предыдущий, первый или последний кадр</summary>
        void MoveToFrame(MovingDirection dir)
        {
            if (obj_VideoReader == null) return;
            //Чтение кадра мастер-канала
            VideoFrame frame = null;
            //Читаем кадр для мастер канала
            VideoStorageResult res = VideoStorageResult.Fault;
            if (dir == MovingDirection.Prev) res = obj_VideoReader.ReadPrevFrame(master_channel, 1, out frame);
            if (dir == MovingDirection.Next) res = obj_VideoReader.ReadNextFrame(master_channel, 1, out frame);
            if (dir == MovingDirection.First) res = obj_VideoReader.ReadFirstFrame(master_channel, out frame);
            if (dir == MovingDirection.Last) res = obj_VideoReader.ReadLastFrame(master_channel, out frame);
            //Неудачное чтение кадра - выход
            if (res != VideoStorageResult.Ok) return;
            if (frame == null) return;
            if (EventNewFrame != null) EventNewFrame(frame.ContentType.Width, frame.ContentType.Height,
                                                     master_channel, frame.FrameData, frame.TimeStamp);
            //Сохранение метки времени мастер-канала
            TimeStamp = frame.TimeStamp;
            //Чтение кадров остальных телекамер
            GetFramesWithoutMaster(frame.TimeStamp, _delta);    
            //Установка ползунка
            if (show_scroll_bar) 
                SetScrollBarThreadSafe(scroll, (int)(scroll.Maximum * ((1.0d * frame.TimeStamp) / this.Length)));
        }

        #region Marks

        /// <summary>Следующая/предыдущая метка</summary>
        void MoveToMark(MovingDirection dir)
        {
            if (marks == null) return;
            if (marks.Length == 0) return;
            int i = GetCurrentMark();
            int mark = i;
            if (dir == MovingDirection.Prev)
            {
                //Если текущая метка времени равна текущей (метка находится непосредсвенно на марке)
                if (TimeStamp == marks[i] && i > 0) mark = i - 1;
                //Если текущая метка более текущей - переход на текущую (к началу вагона)
                if (TimeStamp > marks[i]) mark = i;
                //Если текущая метка менее текущей с допуском
                if (TimeStamp - _delta < marks[i]) if (i > 0) mark = i - 1;
            }
            if (dir == MovingDirection.Next)
            {
                //Если текущая метка равна следующей и не достигнута последняя
                if (i <= marks.Length - 2)
                {
                    //Если текущая метка равна или менее следующей
                    if (TimeStamp <= marks[i + 1]) mark = i + 1;
                }
            }
            MoveToMark(mark, false);
        }

        /// <summary>Перемещение на указанный номер метки</summary>
        /// <param name="mark">Номер метки</param>
        /// <param name="checkchange">Проверять изменение текущей метки</param>
        void MoveToMark(int mark, bool checkchange)
        {
            if (marks == null) return;
            if (checkchange) if (current_mark == mark) return;
            if (mark > marks.Length - 1) return;
            GetFrames(marks[mark], _delta);
            TimeStamp = marks[mark];
            if (show_scroll_bar) SetScrollBarThreadSafe(scroll, (int)(scroll.Maximum * ((1.0d * TimeStamp) / this.Length)));
            AnalyzeMarks();//Событие смены метки
        }

        /// <summary>Проверка текущей метки</summary>
        void AnalyzeMarks()
        {
            int mark = GetCurrentMark();
            if (current_mark != mark)
            {
                current_mark = mark;
                if (EventNewMark != null) EventNewMark(current_mark);
            }
        }

        /// <summary>Возвращает номер текущей метки</summary>
        /// <returns></returns>
        int GetCurrentMark()
        {
            if (marks == null) return 0;
            if (marks.Length == 0) return 0;
            //Поиск по меткам
            for (int i = 0; i < marks.Length - 1; i++)
            {
                if (TimeStamp >= marks[i] && TimeStamp < marks[i + 1])//Текущее положение между метками
                    return i;
            }
            //Последняя метка
            if (TimeStamp >= marks[marks.Length - 1]) return marks.Length - 1;
            return 0;
        }

        #endregion

        #region Get frame parameters

        /// <summary>Опредение размера кадра</summary>
        /// <param name="channel_id">Идентификатор видеопотока</param>
        /// <param name="timestamp">Метка времени видеопотока от начала воспроизведения</param>
        public int GetFrameSize(int channel_id, int timestamp)
        {
            if (obj_VideoReader == null) return -1;
            IList<VideoStreamInfo> info = obj_VideoReader.VideoIndex.StreamInfoList;
            for (int i = 0; i < info.Count; i++)
                if (info[i].Id == channel_id)
                {
                    VideoFrame frame;
                    obj_VideoReader.ReadFrame(channel_id, timestamp, out frame);
                    return frame.FrameData.Length;
                }
            return -1;
        }

        /// <summary>Получение ширины кадра видеопотока</summary>
        /// <param name="channel_id">Идентификатор видеопотока</param>
        public int GetFrameWidth(int channel_id)
        {
            if (obj_VideoReader == null) return -1;
            IList<VideoStreamInfo> info = obj_VideoReader.VideoIndex.StreamInfoList;
            for (int i = 0; i < info.Count; i++)
                if (info[i].Id == channel_id) return info[i].Width;
            return -1;
        }

        /// <summary>Получение высоты кадра видопотока</summary>
        /// <param name="channel_id">Идентификатор видеопотока</param>
        public int GetFrameHeight(int channel_id)
        {
            if (obj_VideoReader == null) return -1;
            IList<VideoStreamInfo> info = obj_VideoReader.VideoIndex.StreamInfoList;
            for (int i = 0; i < info.Count; i++)
                if (info[i].Id == channel_id) return info[i].Height;
            return -1;
        }

        #endregion

        #region Thread safe functions

        /// <summary>Безопасная установка значения ползунка</summary>
        /// <param name="scroll">Ползунок</param>
        /// <param name="value">Значение</param>
        void SetScrollBarThreadSafe(SmoothProgressBar scroll, int value)
        {
            if (scroll.InvokeRequired)
            {
                SetScrollValueCallback d = new SetScrollValueCallback(SetScrollBarThreadSafe);
                scroll.Invoke(d, new object[] { scroll, value });
            }
            else
            {
                scroll.Value = value;
                string text = LengthToString(TimeStamp);
                //Метка над скролл баром
                if (label_time != null) 
                    SetLabelTextThreadSafe(label_time, scroll.Left, scroll.Width, value, label_time.Top, text);
                //Метка времени
                if (simple_length)
                {
                    //if (text != "00.000")
                    {
                        string str = text + " / " + LengthToString(Length);
                        if (lableTime != null) SetLabelTextThreadSafe(lableTime, str);
                    }
                }
                else
                {
                    //if (text != "00.000")
                        if (lableTime != null) SetLabelTextThreadSafe(lableTime, text);
                }
            }
        }

        string LengthToString(int length)
        {
            int min = (int)((int)(length / 1000) / 60);
            int sec = (length - min * 60 * 1000) / 1000;
            int ms = length - min * 60 * 1000 - sec * 1000;
            string text = "";
            if (min > 0) text += min.ToString() + ":";
            text += sec.ToString("00") + "." + ms.ToString("000");
            return text;
        }

        /// <summary>Безопасная установка значения метки</summary>
        /// <param name="label">Ползунок</param>
        /// <param name="value">Текст</param>
        void SetLabelTextThreadSafe(Label label, string value)
        {
            if (label.InvokeRequired)
            {
                SetLabelTextCallback d = new SetLabelTextCallback(SetLabelTextThreadSafe);
                label.Invoke(d, new object[] { label, value });
            }
            else
            {
                lock (_lock) label.Text = value;
            }
        }

        /// <summary>Безопасная установка значения метки</summary>
        /// <param name="label">Ползунок</param>
        /// <param name="scroll_left">Левая координата ползунка</param>
        /// <param name="scroll_width">Ширина ползунка</param>
        /// <param name="posX">Координата X относительно начала ползунка</param>
        /// <param name="posY">Координата Y</param>
        /// <param name="value">Текст</param>
        void SetLabelTextThreadSafe(Label label, int scroll_left, int scroll_width, int posX, int posY, string value)
        {
            if (label.InvokeRequired)
            {
                SetLabelCallback d = new SetLabelCallback(SetLabelTextThreadSafe);
                label.Invoke(d, new object[] { label, scroll_left, posX, posY, value });
            }
            else
            {
                lock (_lock)
                {
                    label.Top = posY;
                    label.Text = value;
                    label.Width = TextRenderer.MeasureText(value, label.Font).Width;
                    int pos = scroll_left + posX - (label.Width / 2);
                    //Обработка левого предела
                    int left = 0;
                    if (pos < scroll_left) left = scroll_left;
                    else left = pos;
                    //Обработка правого предела
                    if (left + label.Width > scroll_left + scroll_width)
                        left = scroll_left + scroll_width - label.Width;
                    label.Left = left;
                    label.ForeColor = Color.White;
                    Bitmap image = new Bitmap(label.Width, label.Height);
                    Graphics g = Graphics.FromImage(image);
                    Pen p = new Pen(Brushes.Blue, 1);
                    DrawRoundRect(g, p, 0, 0, label.Width - 1, label.Height - 1, (label.Height - 1) / 2);
                    label.Image = image;
                }
            }
        }

        /// <summary>Безопасное нажатие / отжатие кнопки</summary>
        /// <param name="button">кнопка</param>
        /// <param name="check">состояние нажатия</param>
        void ButtonThreadSafe(ToolStripButton button, bool check)
        {
            if (button.Owner.InvokeRequired)
            {
                ButtonCallback d = new ButtonCallback(ButtonThreadSafe);
                button.Owner.Invoke(d, new object[] { button, check });
            }
            else
            {
                button.Checked = check;
            }
        }

        #endregion

        void DrawRoundRect(Graphics g, Pen p, float x, float y, float width, float height, float radius)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddLine(x + radius, y, x + width - (radius * 2), y); // Line
            gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90); // Corner
            gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2)); // Line
            gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
            gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height); // Line
            gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner
            gp.AddLine(x, y + height - (radius * 2), x, y + radius); // Line
            gp.AddArc(x, y, radius * 2, radius * 2, 180, 90); // Corner
            gp.CloseFigure();
            //Brush b = Brushes.Coral;
            LinearGradientBrush grad = new LinearGradientBrush(new Point(0, 0), new Point(0, (int)height), Color.Blue, Color.LightBlue);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.FillPath(grad, gp);
            //g.DrawPath(grad, p, gp);
            gp.Dispose();
        }

        #region Export

        /// <summary>Начало диапазона экспорта</summary>
        public int ExportStartMark
        {
            get { return scroll.MarkStart; }
        }

        /// <summary>Окончание диапазона экспорта</summary>
        public int ExportStopMark
        {
            get { return scroll.MarkStop; }
        }

        #endregion

    }

}
