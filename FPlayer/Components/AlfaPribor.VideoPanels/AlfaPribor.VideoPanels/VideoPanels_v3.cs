using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace AlfaPribor.VideoPanels
{

    /// <summary>
    /// Класс создания и управления интерфейса для ПО АСКО и подобных
    /// Создание видеопанелей, размещение панелей информации и управления
    /// версия: 2, дата: апрель 2020
    /// Возможность установки от 2 до 6 видеоокон
    /// Интерфейс с поворотом для 4 телекамер и 6 телекамер
    /// !!!!
    /// to do: Сделать интерфейс для 3 телекамер с поворотом
    /// </summary>
    public class VideoPanels_v3
    {

        #region Declare

        #region Delegates

        delegate void SetEnableToolStripCallback(ToolStrip toolstrip, bool enable);
        delegate void SetVisibleToolStripCallback(ToolStrip toolstrip, bool visible);
        delegate void ShowToolTipCallback(int panel_no);

        #endregion

        #region Events

        /// <summary>Делегат события нажатия кнопки тулбара (сохранение, печать...)</summary>
        /// <param name="panel">Номер панели (с 0)</param>
        /// <param name="type">Тип кнопки</param>
        public delegate void DelegateEventButtonClick(int panel, ToolbarButton type);
        /// <summary>Cобытие нажатия кнопки</summary>
        public event DelegateEventButtonClick EventButtonClick;

        /// <summary>Делегат события нажатия кнопки тулбара (сохранение, печать...)</summary>
        /// <param name="panel">Номер панели (с 0)</param>
        /// <param name="type">Тип кнопки</param>
        public delegate void DelegateEventCustomButtonClick(int panel, object sender);
        /// <summary>Cобытие нажатия кнопки</summary>
        public event DelegateEventCustomButtonClick EventCustomButtonClick;

        /// <summary>Делегат события перемещения сплиттера</summary>
        /// <param name="pos">Положение сплиттера (от 0 до 1)</param>
        public delegate void DelegateEventSplitterMoved(double pos);
        /// <summary>Cобытие перемещения сплиттера</summary>
        public event DelegateEventSplitterMoved EventSplitterMoved;

        /// <summary>Делегат события перемещения второго сплиттера</summary>
        /// <param name="pos">Положение сплиттера (от 0 до 1)</param>
        public delegate void DelegateEventSplitterSecond(double pos);
        /// <summary>Cобытие перемещения сплиттера</summary>
        public event DelegateEventSplitterSecond EventSplitterSecond;

        /// <summary>Делегат события перемещения курсора над панелью</summary>
        /// <param name="panel">Номер панели (с 0)</param>
        public delegate void DelegateEventPanelMouseHover(int panel);
        /// <summary>Cобытие перемещения курсора над панелью</summary>
        public event DelegateEventPanelMouseHover EventPanelMouseHover;

        /// <summary>Делегат события клика на панель</summary>
        /// <param name="panel">Номер панели (с 0)</param>
        /// <param name="left_button">Нажата левая кнопка мыши</param>
        public delegate void DelegateEventPanelMouseClock(int panel, bool left_button);
        /// <summary>Cобытие события клика на панель</summary>
        public event DelegateEventPanelMouseClock EventPanelMouseClick;

        /// <summary>Делегат события двойного клика на панель</summary>
        /// <param name="panel">Номер панели (с 0)</param>
        public delegate void DelegateEventPanelDoubleClock(int panel);
        /// <summary>Cобытие события двойного клика на панель</summary>
        public event DelegateEventPanelDoubleClock EventPanelDoubleClick;

        /// <summary>Делегат события создания стоп-кадра</summary>
        /// <param name="panel">Номер панели (с 0)</param>
        public delegate void DelegateShowStopFrame(int panel);
        /// <summary>Событие создания стоп-кадра</summary>
        public event DelegateShowStopFrame EventPanelStopFrame;

        /// <summary>Делегат события увеличения панели</summary>
        /// <param name="panel">Номер панели (с 0)</param>
        /// <param name="maximize">Увеличить окно</param>
        public delegate void DelegatePanelMaximize(int panel, bool maximize);
        /// <summary>Cобытие увеличения панели</summary>
        public event DelegatePanelMaximize EventPanelMaximize;

        /// <summary>Делегат события изменения размера панели</summary>
        public delegate void DelegatePanelResize(int panel);
        /// <summary>Cобытие изменения размера панели</summary>
        public event DelegatePanelResize EventPanelResize;

        public delegate void DelegateEventPanelMouseWheel(int panel, int delta, double x, double y);
        public event DelegateEventPanelMouseWheel EventPanelMouseWheel;

        /// <summary>Перемещение курсора с нажатой левой кнопкой мыши над панелью</summary>
        /// <param name="panel">Номер панели</param>
        /// <param name="x1">Стартовая x</param>
        /// <param name="y1">Стартовая y</param>
        /// <param name="x2">Конечная x</param>
        /// <param name="y2">Конечная y</param>
        public delegate void DelegateEventPanelMouseDrag(int panel, double x1, double y1, double x2, double y2);
        public event DelegateEventPanelMouseDrag EventPanelMouseDrag;

        #endregion

        #region Enums

        /// <summary>Режим</summary>
        public enum ScreenMode
        {
            /// <summary>Полиэкран</summary>
            PolyScreen = 1,
            /// <summary>Одно видеоокна</summary>
            OneScreen = 2
        }

        /// <summary>Названия картинок кнопок</summary>
        enum ToolbarImagesNames
        {
            /// <summary>Печать кадра</summary>
            PrintImage = 1,
            /// <summary>Сохранение кадра</summary>
            SaveImage = 2,
            /// <summary>Мастер канал</summary>
            Master = 3,
            /// <summary>Увеличение окна</summary>
            Maximize = 4
        };

        /// <summary>Доступные варианты интерфейса (число видеоокон)</summary>
        public enum VideoStyle
        {
            /// <summary>Видеоокна отсутствуют</summary>
            None = 0,
            /// <summary>Одно видеокно</summary>
            One = 1,
            /// <summary>Два видеоокна</summary>
            Double = 2,
            /// <summary>Три видеоокна в двух строках</summary>
            TripleTwoRows = 3,
            /// <summary>Четыре видеоокна</summary>
            Quadro = 4,
            /// <summary>Пять видеоокон</summary>
            Pentad = 5,
            /// <summary>Шесть видеоокон</summary>
            Hexagon = 6,
            /// <summary>Три видеоокна одна строка (повернутые)</summary>
            TripleOneRow = 7,
            /// <summary>Два видеоокна с поворотом</summary>
            DoubleRotation = 8,
            /// <summary>Шесть видеоокон повернутых картинка в картинке</summary>
            HexagonRotation = 9
        };

        /// <summary>Названия кнопок</summary>
        public enum ToolbarButton
        {
            /// <summary>Печать кадра</summary>
            PrintImage = 1,
            /// <summary>Сохранение кадра</summary>
            SaveImage = 2,
            /// <summary>Мастер канал</summary>
            Master = 3,
            /// <summary>Увеличение/уменьшение окна окна</summary>
            Maximize = 4,
            /// <summary>
            /// Произвольный тип
            /// </summary>
            Custom = 5
        };

        /// <summary>Единицы измерения</summary>
        public enum MestureUnit
        {
            /// <summary>Проценты</summary>
            Persantage = 0,
            /// <summary>Пиксели</summary>
            Pixel = 1
        };

        #endregion

        #region Variables

        /// <summary>Окно на втором мониторе</summary>
        FormSecondVideo f;
        /// <summary>Картинки панели</summary>
        //ImageList ToolBarImages;
        /// <summary>Панель воспроизведения</summary>
        Panel PanelPlay;
        /// <summary>Панель информации</summary>
        Panel PanelInfo;
        /// <summary>Панели отображения видео</summary>
        PictureBox[] PictureBoxVideo;
        /// <summary>Панели отображения видео</summary>
        Panel[] PanelVideo;
        /// <summary>Видео размещать на панелях</summary>
        bool enable_panels = false;
        /// <summary>Панели отображения видео на дополнительном мониторе</summary>
        PictureBox ExternalPictureBoxVideo;
        /// <summary>Панели оборажения видео и заголовка</summary>
        //Panel[] Panels;
        /// <summary>Заголовки видеоокон</summary>
        ToolStrip[] ToolStrip;
        /// <summary>Заголовок скрытой панели</summary>
        ToolStrip tsHidden = new ToolStrip();
        /// <summary>Заголовок видеоокон скрытый</summary>
        Panel pnBtns;

        /// <summary>Контекстное меню</summary>
        ContextMenu[] ContextStrip;

        /// <summary>Основной горизонтальный разделитель</summary>
        SplitContainer splitCtrlMain;
        /// <summary>Второй разделитель на правой половине</summary>
        SplitContainer splitRight;

        TableLayoutPanel tspFirst;
        TableLayoutPanel tspSecond;

        /// <summary>Панель заголовка</summary>
        Panel PanelTitle;

        /// <summary>Высота заголовка</summary>
        int title_panel_height = 24;

        /// <summary>Таймер разрешения отрисовки кадров</summary>
        System.Timers.Timer TimerEnableProcess;
        /// <summary>Таймер перерисовка</summary>
        System.Timers.Timer TimerRedraw;

        /// <summary>Отображение заголовков видеоокон</summary>
        bool show_captions = true;

        /// <summary>Отображение скрытых заголовков видеоокон</summary>
        bool hidden_captions = false;

        /// <summary>Флаг разрешения отрисовки кадров</summary>
        bool process_resize = false;

        /// <summary>Флаг отображения заголовка</summary>
        bool show_title = false;

        /// <summary>Разрешить увеличение окна одним кликов</summary>
        bool enable_maximize = false;

        /// <summary>Высота панели кнопок</summary>
        public static int ToolBarHeight = 24;

        float defaultVideoAspectRatio = 4.0f / 5;

        List<float> VideoAspectRatio;

        //float VideoAspectRatio = 5.0f / 4;
        /// <summary>Положение сплиттера по-умолчанию</summary>
        double FDefaultSplitterSize;

        /// <summary>Минимальное в процентах положение сплиттера видеоокон</summary>
        float FMinSplitterPercent;
        /// <summary>Максимальное в процентах положение сплиттера видеоокон</summary>
        float FMaxSplitterPercent;

        /// <summary>Режим полиэкран или оконный</summary>
        ScreenMode Mode;
        /// <summary>Индекс увеличенного окна</summary>
        int MaxWindowIndex;

        /// <summary>Размер правой панели - фиксирован</summary>
        bool fix_right_panel = false;

        /// <summary>Стиль видеоокон (количество)</summary>
        VideoStyle videostyle;

        bool on_panel = false;

        bool drag_panel = false;
        int start_x = 0;
        int start_y = 0;

        /// <summary> Переменная для свойства RotationState </summary>
        List<bool> _rotationStates;

        /// <summary> Шаг корректировки положения панели </summary>
        private int splitterStepSize = 20;

        #endregion

        #endregion

        #region Properties

        /// <summary> Минимальная высота панелей переменная</summary>
        int _minimalPanelHeight;

        /// <summary> Минимальная высота панелей </summary>
        public int MinimalPanelHeight
        {
            get
            {
                if (_minimalPanelHeight == 0) _minimalPanelHeight = default_panelHeight;
                return _minimalPanelHeight;
            }
            set
            {
                _minimalPanelHeight = value;
            }
        }

        /// <summary>Разрешить панели перемещаться в режиме двух окон</summary>
        public bool EnableShiftInfoPanel { get; set; }

        /// <summary>Необходимость проверки высоты панелей</summary>
        public bool CanCheckPanelHeight { get; set; }

        #region Panel Height Propery

        int default_panelHeight = 20;

        int default_incrementStepValue = 20;

        PositionArray _tablePositionArray;

        PositionArray _rightTablePositionArray;

        /// <summary> Связь позиций элентов в компоненте TableLayoutPanel и индексов видеокон для левой панели</summary>
        public PositionArray TablePositionArray
        {
            get
            {
                if (_tablePositionArray == null) _tablePositionArray = new PositionArray();
                return _tablePositionArray;
            }
        }

        /// <summary>  Связь позиций элентов в компоненте TableLayoutPanel и индексов видеокон для правой панели</summary>
        public PositionArray RightTablePositionArray
        {
            get
            {
                if (_rightTablePositionArray == null) _rightTablePositionArray = new PositionArray();
                return _rightTablePositionArray;
            }
        }

        #endregion

        #region Rotation

        /// <summary> Признаки поворота окон</summary>
        public List<bool> WindowRotationState
        {
            get
            {
                return RotationStates;
            }
            set
            {
                _rotationStates = value;
                SetVideoProportions();
            }
        }

        /// <summary> Признаки поворота окон</summary>
        List<bool> RotationStates
        {
            get
            {
                if (_rotationStates == null)
                {
                    _rotationStates = new List<bool>();
                }
                if (_rotationStates.Count < VideoContainer.Length)
                {
                    _rotationStates.Clear();
                    for (int i = 0; i < VideoContainer.Length; i++)
                    {
                        _rotationStates.Add(false);
                    }
                }
                return _rotationStates;
            }
            set
            {
                _rotationStates = value;
            }

        }

        #endregion

        #region Unit property

        /// <summary>Единица измерения положений сплиттера </summary>
        public MestureUnit Unit { get; set; }

        #endregion

        #endregion

        /// <summary>Активация разделителя и всего что внутри</summary>
        /// <param name="enable">Активность</param>
        public void SetEnable(bool enable)
        {
            splitCtrlMain.Enabled = enable;
        }

        #region VideoAspectRatio

        /// <summary>Задать значение одно значение всем элементам масссива пропорций</summary>
        void PushProportions(float proportions, int windowsCount)
        {
            VideoAspectRatio = new List<float>();
            for (int i = 0; i < windowsCount; i++) VideoAspectRatio.Add(proportions);
        }

        /// <summary>Определения числа окон по типу</summary>
        int GetWindowCountByVideoStyle(VideoStyle style)
        {
            int windowCount = 0;
            switch (style)
            {
                case VideoStyle.One: windowCount = 1; break;
                case VideoStyle.Double: windowCount = 2; break;
                case VideoStyle.DoubleRotation: windowCount = 2; break;
                case VideoStyle.TripleTwoRows: windowCount = 3; break;
                case VideoStyle.TripleOneRow: windowCount = 3; break;
                case VideoStyle.Quadro: windowCount = 4; break;
                case VideoStyle.Pentad: windowCount = 5; break;
                case VideoStyle.Hexagon: windowCount = 6; break;
                case VideoStyle.HexagonRotation: windowCount = 6; break;//Пока три окна - потом 6
            }
            return windowCount;
        }

        /// <summary>Получить соотношение сторон для строки таблицы видеоокон</summary>
        /// <param name="rowIndex">Номер строки таблицы</param>
        /// <returns>Соотношение сторон</returns>
        float GetVideoAspectRatio(int rowIndex)
        {
            float min = VideoAspectRatio[0];
            float max = VideoAspectRatio[0];
            for (int i = 0; i < VideoAspectRatio.Count; i++)
            {
                if (VideoAspectRatio[i] > max) max = VideoAspectRatio[i];
                if (VideoAspectRatio[i] < min) min = VideoAspectRatio[i];
            }
            float m = _rotationStates.Contains(true) && videostyle == VideoStyle.TripleOneRow ? min : max;
            return m;
        }

        float GetAvgRatio()
        {
            if (VideoAspectRatio.Count == 0) return 1;
            float avg = 0;
            for (int i = 0; i < VideoAspectRatio[i]; i++) avg += VideoAspectRatio[i];
            return avg / VideoAspectRatio.Count;
        }

        #endregion

        #region Constructors

        /// <summary>Пустой конструктор</summary>
        public VideoPanels_v3()
        {
            _rotationStates = new List<bool>();
            VideoAspectRatio = new List<float>();
        }

        /// <summary>Конструктор класса видеопанелей</summary>
        /// <param name="style">Число отображаемых одновременно телекамер</param>
        /// <param name="Owner">Родительский элемент</param>
        /// <param name="SplitterSize">Пропорциональный размер левой части разделителя (от 0 до 1)</param>
        /// <param name="ExternalWindow">Создавать окно на дополнительном мониторе</param>
        /// <param name="MinVideoPanelHeight">Минимальный размер панели видеоплеера</param>
        public VideoPanels_v3(VideoStyle style, Control Owner, double SplitterSize,
                           bool ExternalWindow, int MinVideoPlayerPanelHeight, bool EnablePanels)
            : this()
        {
            CanCheckPanelHeight = true;
            PushProportions(defaultVideoAspectRatio, GetWindowCountByVideoStyle(style));
            //_rotationStates = new List<bool>();
            //Минимальная высота панели видеоплеера
            //FMinVideoPanelHeight = MinVideoPlayerPanelHeight;
            //Создание панелей
            CreatePanels(style, Owner, SplitterSize, ExternalWindow, true, EnablePanels);
        }

        /// <summary>Конструктор класса видеопанелей</summary>
        /// <param name="style">Число отображаемых одновременно телекамер</param>
        /// <param name="Owner">Родительский элемент</param>
        /// <param name="SplitterSize">Пропорциональный размер левой части разделителя (от 0 до 1)</param>
        /// <param name="ExternalWindow">Создавать окно на дополнительном мониторе</param>
        /// <param name="MinVideoPanelHeight">Минимальный размер панели видеоплеера</param>
        /// <param name="MinPanelInfoHeight">Минимальный размер панели информации</param>
        /// <param name="VideoProportions">Пропорции видеоокон</param>
        /// <param name="ShowCaptions">Отображать заголовки</param>
        /// <param name="EnablePanels">Размещать видео на контроле Panel</param>
        public VideoPanels_v3(VideoStyle style, Control Owner,
                          double SplitterSize, bool ExternalWindow,
                          int MinSplitterPercent, int MaxSplitterPercent,
                          float VideoProportions, bool ShowCaptions, bool EnablePanels,
                          MestureUnit unit = MestureUnit.Persantage)
            : this()
        {
            CanCheckPanelHeight = true;
            _rotationStates = new List<bool>();
            // VideoAspectRatio =  VideoProportions;
            PushProportions(VideoProportions, GetWindowCountByVideoStyle(style));
            //Минимальная ширина панели видеоплеера
            InitMinMaxBarier(MinSplitterPercent, MaxSplitterPercent);
            //Создание панелей
            CreatePanels(style, Owner, SplitterSize, ExternalWindow, ShowCaptions, EnablePanels);
        }

        /// <summary>Конструктор класса видеопанелей</summary>
        /// <param name="style">Число отображаемых одновременно телекамер</param>
        /// <param name="Owner">Родительский элемент</param>
        /// <param name="SplitterSize">Пропорциональный размер левой части разделителя (от 0 до 1)</param>
        /// <param name="ExternalWindow">Создавать окно на дополнительном мониторе</param>
        /// <param name="MinVideoPanelHeight">Минимальный размер панели видеоплеера</param>
        /// <param name="MinPanelInfoHeight">Минимальный размер панели информации</param>
        /// <param name="VideoProportions">Пропорции видеоокон</param>
        /// <param name="ShowCaptions">Отображать заголовки</param>
        /// <param name="EnablePanels">Размещать видео на контроле Panel</param>
        public VideoPanels_v3(VideoStyle style, Control Owner,
                              double SplitterSize, bool ExternalWindow,
                              int MinSplitterPercent, int MaxSplitterPercent,
                              float VideoProportions, bool ShowCaptions, bool EnablePanels,
                              List<bool> RotationStates, MestureUnit unit = MestureUnit.Persantage)
        {
            CanCheckPanelHeight = true;
            _rotationStates = RotationStates;
            PushProportions(VideoProportions, GetWindowCountByVideoStyle(style));
            //Минимальная ширина панели видеоплеера
            InitMinMaxBarier(MinSplitterPercent, MaxSplitterPercent);
            //Создание панелей
            CreatePanels(style, Owner, SplitterSize, ExternalWindow, ShowCaptions, EnablePanels);
        }

        /// <summary>Конструктор класса видеопанелей</summary>
        /// <param name="style">Число отображаемых одновременно телекамер</param>
        /// <param name="Owner">Родительский элемент</param>
        /// <param name="SplitterSize">Пропорциональный размер левой части разделителя (от 0 до 1)</param>
        /// <param name="ExternalWindow">Создавать окно на дополнительном мониторе</param>
        /// <param name="MinVideoPanelHeight">Минимальный размер панели видеоплеера</param>
        /// <param name="MinPanelInfoHeight">Минимальный размер панели информации</param>
        /// <param name="VideoProportions">Пропорции видеоокон</param>
        /// <param name="ShowCaptions">Отображать заголовки</param>
        /// <param name="EnablePanels">Размещать видео на контроле Panel</param>
        public VideoPanels_v3(VideoStyle style, Control Owner, double SplitterSize,
                          bool ExternalWindow, int MinSplitterPercent, int MaxSplitterPercent,
                          float VideoProportions, bool ShowCaptions, bool EnablePanels, List<float> videoProportions, MestureUnit unit = MestureUnit.Persantage)
        {
            CanCheckPanelHeight = true;
            VideoAspectRatio = videoProportions;
            //Минимальная ширина панели видеоплеера
            InitMinMaxBarier(MinSplitterPercent, MaxSplitterPercent);
            //Создание панелей
            CreatePanels(style, Owner, SplitterSize, ExternalWindow, ShowCaptions, EnablePanels);
        }

        /// <summary>Конструктор класса видеопанелей с видео на контроле Panel</summary>
        /// <param name="style">Число отображаемых одновременно телекамер</param>
        /// <param name="Owner">Родительский элемент</param>
        /// <param name="SplitterSize">Пропорциональный размер левой части разделителя (от 0 до 1)</param>
        /// <param name="ExternalWindow">Создавать окно на дополнительном мониторе</param>
        /// <param name="MinVideoPanelHeight">Минимальный размер панели видеоплеера</param>
        /// <param name="EnablePanels">true - Видеоокна на основе Panel, false - Видеоокна на основе PictureBox</param>
        public VideoPanels_v3(VideoStyle style, Control Owner, double SplitterSize,
                           bool ExternalWindow, bool showCaptions, bool EnablePanels)
        {
            CanCheckPanelHeight = true;
            _rotationStates = new List<bool>();
            PushProportions(defaultVideoAspectRatio, GetWindowCountByVideoStyle(style));
            //Создание панелей
            CreatePanels(style, Owner, SplitterSize, ExternalWindow, showCaptions, EnablePanels);
        }

        /// <summary>Конструктор класса видеопанелей</summary>
        /// <param name="style">Число отображаемых одновременно телекамер</param>
        /// <param name="Owner">Родительский элемент</param>
        /// <param name="SplitterSize">Пропорциональный размер левой части разделителя (от 0 до 1)</param>
        /// <param name="HiddenCaptions">Скрыть заголовки</param>
        /// <param name="MinSplitterPercent">Минимальный размер панели видеоплеера</param>
        /// <param name="MaxSplitterPercent">Максимальный размер панели видеоплеера</param>
        /// <param name="VideoProportions">Пропорции видео</param>
        /// <param name="unit">Единицы измерения</param>
        public VideoPanels_v3(VideoStyle style, Control Owner, double SplitterSize, bool HiddenCaptions,
                           float MinSplitterPercent, float MaxSplitterPercent,
                           float VideoProportions, MestureUnit unit = MestureUnit.Persantage)
        {
            CanCheckPanelHeight = true;
            hidden_captions = HiddenCaptions;
            show_captions = false;
            PushProportions(VideoProportions, GetWindowCountByVideoStyle(style));
            //Минимальная ширина панели видеоплеера
            InitMinMaxBarier(MinSplitterPercent, MaxSplitterPercent);
            CreatePanels(style, Owner, SplitterSize, false, false, false);
        }

        /// <summary>Инициализация предельных положений сплиттера.</summary>
        /// <param name="MinSplitterPercent"></param>
        /// <param name="MaxSplitterPercent"></param>
        void InitMinMaxBarier(float MinSplitterPercent, float MaxSplitterPercent)
        {
            if (Unit == MestureUnit.Persantage)
            {
                //Минимальная высота панели видеоплеера
                if (MinSplitterPercent < 0 && MinSplitterPercent > 100) FMinSplitterPercent = 20;
                else FMinSplitterPercent = MinSplitterPercent;
                //Минимальная высота панели панели информации
                if (FMaxSplitterPercent < 0 && MaxSplitterPercent > 100) FMaxSplitterPercent = 80;
                else FMaxSplitterPercent = MaxSplitterPercent;
                if (MinSplitterPercent >= MaxSplitterPercent)
                {
                    FMinSplitterPercent = 20;
                    FMaxSplitterPercent = 80;
                }
            }
            else
            {
                FMinSplitterPercent = Math.Min(MinSplitterPercent, MaxSplitterPercent);
                FMaxSplitterPercent = Math.Max(MinSplitterPercent, MaxSplitterPercent);
            }
        }

        #endregion

        #region Create

        /// <summary>Создание интерфейса</summary>
        /// <param name="style">Число отображаемых одновременно телекамер</param>
        /// <param name="Owner">Родительский элемент</param>
        /// <param name="SplitterSize">Пропорциональный размер левой части разделителя (от 0 до 1)</param>
        /// <param name="ExternalWindow">Создавать окно на дополнительном мониторе</param>
        /// <param name="MinVideoPanelHeight">Минимальный размер панели видеоплеера</param>
        /// <param name="showCaptions">Отображать заголовки видеопанелей</param>
        /// <param name="enablePanels">Размещать видео на панели (иначе на PictureBox)</param>
        public void CreatePanels(VideoStyle style, Control Owner, double SplitterSize,
                                 bool ExternalWindow, bool showCaptions, bool enablePanels)
        {
            enable_panels = enablePanels;
            show_captions = showCaptions;
            Mode = ScreenMode.PolyScreen;

            //Создание вспомогательных таймеров
            CreateTimers();

            #region Создание объектов видеоокон

            //Число видеоокон
            int windows_count = GetWindowCountByVideoStyle(style);

            //Создание контейнеров видео
            if (enablePanels) PanelVideo = new Panel[windows_count];
            else PictureBoxVideo = new PictureBox[windows_count];

            if (show_captions) ToolStrip = new ToolStrip[windows_count];

            //Контекстное меню
            ContextStrip = new ContextMenu[windows_count];

            //Создание видеоокон и контекстных меню
            for (int i = 0; i < windows_count; i++)
            {
                //Показать заголовки видеоокон
                if (show_captions)
                {
                    ToolStrip[i] = new ToolStrip();
                    ToolStrip[i].RenderMode = ToolStripRenderMode.ManagerRenderMode;
                    ToolStrip[i].GripStyle = ToolStripGripStyle.Hidden;
                    ToolStrip[i].Tag = i;
                }

                //Контрол отрисовки - Panel или PictureBox
                if (enablePanels)
                {
                    PanelVideo[i] = new Panel();
                }
                else
                {
                    PictureBoxVideo[i] = new PictureBox();
                    ((PictureBox)VideoContainer[i]).SizeMode = PictureBoxSizeMode.StretchImage;
                }

                //Контекстное меню по правой кнопке мыши
                ContextStrip[i] = new ContextMenu();
                ContextStrip[i].MenuItems.Add(new MenuItem("Просмотр изображения"));
                ContextStrip[i].MenuItems[0].Tag = i;
                ContextStrip[i].MenuItems[0].Click += new EventHandler(ContextMenu_Click);

                VideoContainer[i].Dock = DockStyle.Fill;
                VideoContainer[i].BackColor = System.Drawing.Color.AliceBlue;
                VideoContainer[i].MouseHover += new EventHandler(VideoPanels_MouseHover);
                VideoContainer[i].Tag = i;

                VideoContainer[i].Margin = new Padding(0, 0, 1, 1);
                // VideoContainer[i].Resize += new EventHandler(VideoPanels_Resize);

                //Интерфейс с поворотом телекамер - особые отступы
                if (style == VideoStyle.TripleOneRow || style == VideoStyle.HexagonRotation)
                {
                    if (i == 0) VideoContainer[i].Margin = new Padding(0, 0, 4, 0);
                    if (i == 1) VideoContainer[i].Margin = new Padding(2, 0, 2, 0);
                    if (i == 2) VideoContainer[i].Margin = new Padding(4, 0, 0, 0);
                }
                if (!RotationStates.Contains(false))
                {
                    if (i == 0) VideoContainer[i].Margin = new Padding(0, 0, 2, 0);
                    if (i == 1) VideoContainer[i].Margin = new Padding(2, 0, 0, 0);
                }

                //Контекстное меню на видеоокне
                VideoContainer[i].ContextMenu = ContextStrip[i];

                //Двойной щелчок мышью
                VideoContainer[i].MouseDoubleClick += new MouseEventHandler(VideoPanels_MouseDoubleClick);
                VideoContainer[i].MouseEnter += new EventHandler(VideoPanels_MouseEnter);
                VideoContainer[i].MouseLeave += new EventHandler(VideoPanels_MouseLeave);
                VideoContainer[i].MouseMove += new MouseEventHandler(VideoPanels_MouseMove);

                //Колесо мыши
                VideoContainer[i].MouseWheel += new MouseEventHandler(VideoPanels_MouseWheel);
                VideoContainer[i].MouseUp += new MouseEventHandler(VideoPanels_MouseUp);
                VideoContainer[i].MouseDown += new MouseEventHandler(VideoPanels_MouseDown);
            }

            #endregion

            #region Сплиттеры разделители

            //Главный горизонтальный разделитель
            splitCtrlMain = new SplitContainer();
            splitCtrlMain.BorderStyle = BorderStyle.Fixed3D;
            splitCtrlMain.Parent = Owner;
            splitCtrlMain.Dock = DockStyle.Fill;
            splitCtrlMain.SplitterWidth = 2;
            splitCtrlMain.Panel1MinSize = 100;

            //Положение сплиттера - по умолчанию
            FDefaultSplitterSize = SplitterSize;
            //Установка текущей пропорций окон
            int size = (int)(SplitterSize * splitCtrlMain.Width);
            if (size == 0) size = splitCtrlMain.Width / 2;
            int distance = (int)Math.Round(FDefaultSplitterSize * splitCtrlMain.Width);
            if (distance > 0 && distance < Screen.PrimaryScreen.WorkingArea.Width)
                splitCtrlMain.SplitterDistance = distance;

            //События изменения сплиттера
            splitCtrlMain.SplitterMoved += new SplitterEventHandler(splitCtrlMain_SplitterMoved);
            splitCtrlMain.SizeChanged += new EventHandler(splitCtrlMain_SizeChanged);
            splitCtrlMain.SplitterMoving += new SplitterCancelEventHandler(splitCtrlMain_SplitterMoving);
            splitCtrlMain.MouseDoubleClick += new MouseEventHandler(splitCtrlMain_MouseDoubleClick);

            #endregion

            videostyle = style;

            //Создание интерфейса
            if (style == VideoStyle.One) SetOneWindow();                            //Одно видеоокно
            if (style == VideoStyle.Double) SetDoubleWindows();                     //Два видеоокна
            if (style == VideoStyle.DoubleRotation) SetDoubleRotationWindows();     //Два видеоокна с поворотом
            if (style == VideoStyle.TripleTwoRows) SetTripleWindows();              //Три видеоокна
            if (style == VideoStyle.Quadro) SetFourWindows();                       //Четыре видеоокна
            if (style == VideoStyle.Pentad) SetFiveWindows();                       //Пять окон
            if (style == VideoStyle.Hexagon) SetSixWindows();                       //Шесть окон
            if (style == VideoStyle.TripleOneRow) SetTripleRotationWindows();       //Три окна с поворотом
            if (style == VideoStyle.HexagonRotation) SetSixRotationWindows();   //Шесть окон с поворотом

            //Скрытые заголовки - появляется при наведении мыши
            if (hidden_captions)
            {
                pnBtns = new Panel();
                pnBtns.Width = 72;
                pnBtns.Height = 24;
                pnBtns.Parent = VideoContainer[0].FindForm();
                pnBtns.Visible = false;
                pnBtns.BackColor = Color.FromArgb(10, 88, 44, 55);
                pnBtns.MouseLeave += new EventHandler(pnBtns_MouseLeave);
                //pnBtns.BackColor = Color.AliceBlue;

                tsHidden = new ToolStrip();
                tsHidden.RenderMode = ToolStripRenderMode.System;
                tsHidden.GripStyle = ToolStripGripStyle.Hidden;
                tsHidden.Parent = pnBtns;
                tsHidden.MouseLeave += new EventHandler(pnBtns_MouseLeave);
            }

            //Внешнее видеоокно
            if (ExternalWindow) ShowSecondWindow();

            //Установка размеров видеоокон
            SetVideoProportions();
        }

        /// <summary>Создание интерфейса с 1-м видеоокном</summary>
        void SetOneWindow()
        {
            //Первая коллекция видеоокон
            CreateTable(1);
            //Размещение панели плеера
            SetPanelPlayer(show_captions, 2, 1, 0);
            //Панель информации на правую часть сплиттера
            PanelInfo.Parent = splitCtrlMain.Panel2;
        }

        /// <summary>Создание интерфейса с 2-мя видеоокнами</summary>
        void SetDoubleWindows()
        {
            //Первая коллекция видеоокон
            if (IsRotationDoubleMode()) CreateTable(2);
            else CreateTable(1);

            if (!IsRotationDoubleMode())
            {
                //Вторая коллекция видеоокон
                tspSecond = new TableLayoutPanel();
                //Одна колонка (заголовок, видео и плеер) и три строки
                tspSecond.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));
                if (show_captions) tspSecond.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                tspSecond.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                tspSecond.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                tspSecond.Parent = splitCtrlMain.Panel2;
                tspSecond.Dock = DockStyle.Fill;

                //Показать заголовки
                if (show_captions)
                {
                    tspSecond.Controls.Add(ToolStrip[1]);
                    tspSecond.SetRow(ToolStrip[1], 0);
                    tspSecond.SetColumn(ToolStrip[1], 0);
                }

                tspSecond.Controls.Add(VideoContainer[1]);

                //Добавление второй видеопанели с заголовком и без
                int row = 0;
                if (show_captions) row = 1;
                tspSecond.SetRow(VideoContainer[1], row);
                tspSecond.SetColumn(VideoContainer[1], 0);

                RightTablePositionArray[row, 0] = 1;

                //Панели видеоплеера и информации

                //Положение панели плеера
                if (show_captions) tspFirst.SetRow(PanelPlay, 2);
                else tspFirst.SetRow(PanelPlay, 1);
                tspFirst.SetColumn(PanelPlay, 0);

                //Панель информации на вторую таблицу
                tspSecond.Controls.Add(PanelInfo);

                //Добавление панели информации с заголовком и без - на вторую половину
                if (show_captions) tspSecond.SetRow(PanelInfo, 2);
                else tspSecond.SetRow(PanelInfo, 1);

                tspSecond.SetColumn(PanelInfo, 0);
            }
            else
            {
                //Режим повёрнутых окон
                //С заголовками
                if (show_captions)
                {
                    tspFirst.SetRow(PanelPlay, 2);
                    tspFirst.SetColumn(PanelPlay, 0);
                    tspFirst.SetColumnSpan(PanelPlay, 2);
                }
                //Без заголовков
                else
                {
                    tspFirst.SetRow(PanelPlay, 1);
                    tspFirst.SetColumn(PanelPlay, 0);
                    tspFirst.SetColumnSpan(PanelPlay, 2);
                }

                //Панель информации на вторую половину сплиттера
                splitCtrlMain.Panel2.Controls.Add(PanelInfo);
            }

            //Установка размеров окна
            SetVideoProportions();
        }

        /// <summary>Создание интерфейса с 2-мя повернутыми видеоокнами</summary>
        void SetDoubleRotationWindows()
        {
            //Два окна слева
            CreateTable(2);

            //Сплит панель справа
            //Главный горизонтальный разделитель
            splitRight = new SplitContainer();
            splitRight.Orientation = Orientation.Horizontal;
            splitRight.BorderStyle = BorderStyle.Fixed3D;
            splitRight.Parent = splitCtrlMain.Panel2;
            splitRight.Dock = DockStyle.Fill;
            splitRight.SplitterWidth = 2;
            splitRight.Panel1MinSize = 300;
            splitRight.SplitterMoved += new SplitterEventHandler(splitRight_SplitterMoved);

            //Панель информации на правую часть сплиттера
            PanelInfo.Parent = splitRight.Panel1;
            //Положение панели плеера - внизу правой панели
            PanelPlay.Parent = splitRight.Panel2;

        }

        /// <summary>Создание интерфейса с 3-мя видеоокнами без поворота</summary>
        void SetTripleWindows()
        {
            CreateTable(3);
            //Размещение панели плеера
            SetPanelPlayer(show_captions, 3, 1, 1);
            //Панель информации на вторую половину сплиттера
            splitCtrlMain.Panel2.Controls.Add(PanelInfo);
            //Установка размеров окна
            SetVideoProportions();
        }

        /// <summary>Три окна с поворотом</summary>
        void SetTripleRotationWindows()
        {
            //Первая коллекция видеоокон
            CreateTable(3);
            //Размещение панели плеера
            SetPanelPlayer(show_captions, 2, 1, 0);
            //Панель информации на вторую половину сплиттера
            splitCtrlMain.Panel2.Controls.Add(PanelInfo);
            //Установка размеров окна
            SetVideoProportions();
        }

        /// <summary>Создание интерфейса с 4-мя видеоокнами</summary>
        void SetFourWindows()
        {
            //Создание таблицы окон
            CreateTable(4);
            //Размещение панели плеера
            SetPanelPlayer(show_captions, 4, 2, 0);
            //Панель информации на вторую половину сплиттера
            splitCtrlMain.Panel2.Controls.Add(PanelInfo);
            //Установка размеров окна
            SetVideoProportions();
        }

        /// <summary>Создание интерфейса с 5-ю видеоокнами</summary>
        void SetFiveWindows()
        {
            //Первая коллекция видеоокон
            CreateTable(4);

            //Вторая коллекция видеоокон
            tspSecond = new TableLayoutPanel();
            //Одна колонка и три строки
            tspSecond.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));
            if (show_captions) tspSecond.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            tspSecond.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            tspSecond.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            tspSecond.Parent = splitCtrlMain.Panel2;
            tspSecond.Dock = DockStyle.Fill;

            if (show_captions)
            {
                tspSecond.Controls.Add(ToolStrip[4]);
                tspSecond.SetRow(ToolStrip[4], 0);
                tspSecond.SetColumn(ToolStrip[4], 0);
            }

            tspSecond.Controls.Add(VideoContainer[4]);

            //Добавление пятой видеопанели с заголовком и без
            if (show_captions) tspSecond.SetRow(VideoContainer[4], 1);
            else tspSecond.SetRow(VideoContainer[4], 0);

            tspSecond.SetColumn(VideoContainer[4], 0);

            RightTablePositionArray[show_captions ? 1 : 0, 0] = 4;

            //Панели видеоплеера и информации

            //Положение панели плеера
            if (show_captions) tspFirst.SetRow(PanelPlay, 4);
            else tspFirst.SetRow(PanelPlay, 2);
            tspFirst.SetColumn(PanelPlay, 0);
            tspFirst.SetColumnSpan(PanelPlay, 2);

            //Панель информации на вторую таблицу
            tspSecond.Controls.Add(PanelInfo);

            //Добавление панели информации с заголовком и без
            if (show_captions) tspSecond.SetRow(PanelInfo, 2);
            else tspSecond.SetRow(PanelInfo, 1);

            tspSecond.SetColumn(PanelInfo, 0);
        }

        /// <summary>Режим шести окон</summary>
        void SetSixWindows()
        {
            //Первая коллекция видеоокон
            CreateTable(6);
            //Размещение панели плеера
            SetPanelPlayer(show_captions, 4, 2, 0);
            //Панель информации на вторую половину сплиттера
            splitCtrlMain.Panel2.Controls.Add(PanelInfo);
        }

        /// <summary>Режим шести окон с поворотом</summary>
        void SetSixRotationWindows()
        {
            //Коллекция видеоокон
            CreateTableHexaRotation();
            //Размещение панели плеера
            SetPanelPlayer(show_captions, 2, 1, 0);
            //Панель информации на вторую половину сплиттера
            splitCtrlMain.Panel2.Controls.Add(PanelInfo);
            //Установка размеров окна
            SetVideoProportions();
        }

        /// <summary>Размещение панели плеера</summary>
        /// <param name="show_captions"></param>
        /// <param name="pos_captions">Строка плеера для интерфейса заголовками</param>
        /// <param name="without_captions">Строка плеера для интерфейса без заголовков</param>
        void SetPanelPlayer(bool show_captions, int pos_captions, int without_captions, int column)
        {
            //Размещение панели плеера
            if (show_captions) tspFirst.SetRow(PanelPlay, pos_captions);//С заголовками
            else tspFirst.SetRow(PanelPlay, without_captions);
            tspFirst.SetColumn(PanelPlay, column);
            tspFirst.SetColumnSpan(PanelPlay, tspFirst.RowStyles.Count);
        }

        /// <summary>Отображение видеоокна на втором мониторе</summary>
        bool ShowSecondWindow()
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            if (sc.Length == 1) return false;

            //Окно на втором мониторе
            f = new FormSecondVideo();
            f.Left = sc[1].Bounds.Left;
            f.Top = 0; // sc[0].Bounds.Height; 
            f.StartPosition = FormStartPosition.Manual;
            f.FormBorderStyle = FormBorderStyle.None;

            //Видеопанель в дополнительном окне
            ExternalPictureBoxVideo = new PictureBox();
            ExternalPictureBoxVideo.Parent = f;
            ExternalPictureBoxVideo.BackColor = System.Drawing.Color.AliceBlue;

            ExternalPictureBoxVideo.Dock = DockStyle.Fill;

            f.Show();
            return true;
        }

        /// <summary>Создание таблицы окон</summary>
        /// <param name="windows_count">Число видеоокон</param>
        void CreateTable(int windows_count)
        {

            #region Интерфейс БЕЗ поворота

            if (videostyle != VideoStyle.TripleOneRow && videostyle != VideoStyle.HexagonRotation && !IsRotationDoubleMode())
            {
                //Первая коллекция видеоокон
                if (tspFirst == null)
                {
                    tspFirst = new TableLayoutPanel();
                    int columnCount = videostyle == VideoStyle.Hexagon ? 3 : 2;
                    for (int i = 0; i < columnCount; i++) tspFirst.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));
                    if (show_captions)
                        for (int i = 0; i < 5; i++) tspFirst.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                    else
                        for (int i = 0; i < 3; i++) tspFirst.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                    tspFirst.Parent = splitCtrlMain.Panel1;
                    tspFirst.Dock = DockStyle.Fill;
                }

                //Размещение элементов
                if (videostyle != VideoStyle.Hexagon)
                {
                    #region Classic

                    for (int i = 0; i < windows_count; i++)
                    {
                        int col = 0;//Колонка
                        int row = 0;//Ряд

                        if (show_captions) row = (i / 2) * 2;
                        else row = (i / 2);

                        if (i == 1 || i == 3) col = 1;

                        if (show_captions)
                        {
                            ToolStrip[i].Visible = true;
                            tspFirst.Controls.Add(ToolStrip[i]);
                            tspFirst.SetRow(ToolStrip[i], row);
                            tspFirst.SetColumn(ToolStrip[i], col);
                        }

                        VideoContainer[i].Visible = true;
                        tspFirst.Controls.Add(VideoContainer[i]);

                        if (show_captions)
                        {
                            tspFirst.SetRow(VideoContainer[i], row + 1);
                            tspFirst.SetColumn(VideoContainer[i], col);

                            TablePositionArray[row + 1, col] = i;
                        }
                        else
                        {
                            tspFirst.SetRow(VideoContainer[i], row);
                            tspFirst.SetColumn(VideoContainer[i], col);

                            TablePositionArray[row, col] = i;
                        }
                    }

                    #endregion
                }
                else
                {
                    #region Hexagon

                    int row = 0;
                    int col = 0;//Колонка

                    //AssignWindowsWidthByMinimalWindows();

                    for (int i = 0; i < windows_count; i++)
                    {


                        if (i % 3 == 0 && i != 0)
                        {
                            if (show_captions) row += 2;
                            else row++;
                        }

                        col = i % 3;

                        if (show_captions)
                        {
                            ToolStrip[i].Visible = true;
                            tspFirst.Controls.Add(ToolStrip[i]);
                            tspFirst.SetRow(ToolStrip[i], row);
                            tspFirst.SetColumn(ToolStrip[i], col);
                        }

                        VideoContainer[i].Visible = true;
                        tspFirst.Controls.Add(VideoContainer[i]);

                        if (show_captions)
                        {
                            tspFirst.SetRow(VideoContainer[i], row + 1);
                            tspFirst.SetColumn(VideoContainer[i], col);

                            TablePositionArray[row + 1, col] = i;
                        }
                        else
                        {
                            tspFirst.SetRow(VideoContainer[i], row);
                            tspFirst.SetColumn(VideoContainer[i], col);

                            TablePositionArray[row, col] = i;
                        }
                    }

                    #endregion
                }
                AddPanels();
            }

            #endregion

            #region Интерфейс с поворотом

            //Два, три или шесть окон
            if (IsRotationDoubleMode() || (videostyle == VideoStyle.TripleOneRow || videostyle == VideoStyle.HexagonRotation))
            {
                //Первая коллекция видеоокон
                if (tspFirst == null)
                {
                    int columnCount = videostyle == VideoStyle.TripleOneRow ? 3 : 3;
                    tspFirst = new TableLayoutPanel();
                    for (int i = 0; i < columnCount; i++) tspFirst.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));
                    for (int i = 0; i < 2; i++) tspFirst.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                    tspFirst.Parent = splitCtrlMain.Panel1;
                    tspFirst.Dock = DockStyle.Fill;
                }
                //Размещение элементов
                for (int i = 0; i < windows_count; i++)
                {
                    int col = 0;//Колонка
                    int row = 0;//Ряд

                    row = 0;
                    col = i;

                    if (show_captions)
                    {
                        ToolStrip[i].Visible = true;
                        tspFirst.Controls.Add(ToolStrip[i]);
                        tspFirst.SetRow(ToolStrip[i], 0);
                        tspFirst.SetColumn(ToolStrip[i], col);
                    }

                    VideoContainer[i].Visible = true;
                    tspFirst.Controls.Add(VideoContainer[i]);
                    //Добавление окна
                    if (show_captions)
                    {
                        tspFirst.SetRow(VideoContainer[i], row + 1);
                        tspFirst.SetColumn(VideoContainer[i], col);
                        //Задать позицию в словарь для определения поворота окна
                        TablePositionArray[row + 1, col] = i;
                    }
                    else
                    {
                        tspFirst.SetRow(VideoContainer[i], row);
                        tspFirst.SetColumn(VideoContainer[i], col);
                        //Задать позицию в словарь для определения поворота окна
                        TablePositionArray[row, col] = i;
                    }
                }
                //Добавляем панель плеера
                if (PanelPlay == null)
                {
                    PanelPlay = new Panel();
                    PanelPlay.Dock = DockStyle.Fill;
                    tspFirst.Controls.Add(PanelPlay);
                }
                //Добавляем панель информации на вторую половину сплиттера
                if (PanelInfo == null)
                {
                    PanelInfo = new Panel();
                    PanelInfo.Dock = DockStyle.Fill;
                }
            }

            #endregion
        }

        /// <summary>Создание таблицы 6 окон - повернутый интерфейс - 3 больших окна, в каждом малое</summary>
        void CreateTableHexaRotation()
        {
            //Создание таблицы окон слева от разделителя
            if (tspFirst == null)
            {
                //Создать таблицу 3 колонки / 2 столбца
                int columnCount = 3;
                tspFirst = new TableLayoutPanel();
                for (int i = 0; i < columnCount; i++) tspFirst.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));
                for (int i = 0; i < 2; i++) tspFirst.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                tspFirst.Parent = splitCtrlMain.Panel1;
                tspFirst.Dock = DockStyle.Fill;
            }

            //Размещение элементов
            for (int i = 0; i < 3; i++)
            {
                int col = i;    //Колонка

                //Если есть заголовки - добавить их в первую строку
                if (show_captions)
                {
                    ToolStrip[i].Visible = true;
                    tspFirst.Controls.Add(ToolStrip[i]);
                    tspFirst.SetRow(ToolStrip[i], 0);
                    tspFirst.SetColumn(ToolStrip[i], col);
                }

                //Добавление окна в таблицу
                int row = show_captions ? 1 : 0;

                //Общая панель в ячейке таблицы
                Panel panel = new Panel();
                panel.Dock = DockStyle.Fill;
                //Добавить панель в ячейку таблицы
                tspFirst.Controls.Add(panel);
                tspFirst.SetRow(panel, row);
                tspFirst.SetColumn(panel, col);

                //Большая видеопанель
                VideoContainer[i * 2].Visible = true;
                panel.Controls.Add(VideoContainer[i * 2]);

                //Добавить панель для размещения малой видеопанели
                //Добавляется для рамки вокруг малого видео
                //Panel panel_small = new Panel();
                //panel_small.BackColor = Color.LightGray;
                //panel.Controls.Add(panel_small);
                //panel_small.Visible = false;
                //Малая видеопанель

                panel.Controls.Add(VideoContainer[i * 2 + 1]);
                VideoContainer[i * 2 + 1].Visible = true;
                VideoContainer[i * 2 + 1].BackColor = Color.LightGray;
                VideoContainer[i * 2 + 1].BringToFront();
                VideoContainer[i * 2 + 1].Dock = DockStyle.None;

                //VideoContainer[i * 2 + 1].Left = 1;
                //VideoContainer[i * 2 + 1].Top = 1;
                //VideoContainer[i * 2 + 1].Width = panel_small.Width - 2;
                //VideoContainer[i * 2 + 1].Height = panel_small.Height - 2;

                //panel_small.Controls.Add(VideoContainer[i * 2 + 1]);

                //TablePositionArray[row, col] = i; //Задать позицию в словарь для определения поворота окна
            }
            //Добавляем панель плеера
            if (PanelPlay == null)
            {
                PanelPlay = new Panel();
                PanelPlay.Dock = DockStyle.Fill;
                tspFirst.Controls.Add(PanelPlay);
            }
            //Добавляем панель информации на вторую половину сплиттера
            if (PanelInfo == null)
            {
                PanelInfo = new Panel();
                PanelInfo.Dock = DockStyle.Fill;
            }
        }

        void AssignWindowsWidthByMinimalWindows()
        {
            int minWidth = VideoContainer[0].Width;
            for (int i = 0; i < VideoContainer.Length; i++)
            {
                if (minWidth > VideoContainer[i].Width)
                    minWidth = VideoContainer[i].Width;
            }
            for (int i = 0; i < VideoContainer.Length; i++)
                VideoContainer[i].Width = minWidth;
        }

        /// <summary>Добавление панелей классического для классического режима</summary>
        void AddPanels()
        {
            //Добавляем панель плеера
            if (PanelPlay == null)
            {
                PanelPlay = new Panel();
                PanelPlay.Dock = DockStyle.Fill;
                tspFirst.Controls.Add(PanelPlay);
            }
            //Добавляем панель информации на вторую половину сплиттера
            if (PanelInfo == null)
            {
                PanelInfo = new Panel();
                PanelInfo.Dock = DockStyle.Fill;
            }
        }

        /// <summary>Переставить видеоокна местами</summary>
        public bool ResetElement(int oldPosition, int newPosition)
        {
            if (videostyle != VideoStyle.TripleOneRow || videostyle != VideoStyle.Double)
            {
                return false;
            }
            if (videostyle == VideoStyle.TripleOneRow || IsRotationDoubleMode())
            {
                TableLayoutPanelCellPosition positionOld = tspFirst.GetCellPosition(VideoContainer[oldPosition]);
                TableLayoutPanelCellPosition positionNew = tspFirst.GetCellPosition(VideoContainer[newPosition]);
                Size oldSize = VideoContainer[oldPosition].Size;
                Size newSize = VideoContainer[newPosition].Size;

                VideoContainer[newPosition].Parent = null;
                VideoContainer[oldPosition].Parent = null;

                VideoContainer[newPosition].Size = oldSize;
                VideoContainer[oldPosition].Size = newSize;

                tspFirst.Controls.Add(VideoContainer[newPosition]);
                tspFirst.Controls.Add(VideoContainer[oldPosition]);

                lock (tspFirst)
                {
                    tspFirst.SetCellPosition(VideoContainer[oldPosition], positionNew);

                    tspFirst.SetCellPosition(VideoContainer[newPosition], positionOld);
                }
                // Написать код для перестановки соответсвия индексов и строк TableLayoutPanel
                int oldIndex = TablePositionArray[positionNew.Row, positionNew.Column];
                TablePositionArray[positionNew.Row, positionNew.Column] = TablePositionArray[positionOld.Row, positionOld.Column];
                TablePositionArray[positionNew.Row, positionNew.Column] = oldIndex;
            }
            else
            {
                if (videostyle == VideoStyle.Double)
                {
                    if (oldPosition == newPosition)
                    {
                        return true;
                    }
                    TableLayoutPanelCellPosition positionOld = oldPosition == 0 ? tspFirst.GetCellPosition(VideoContainer[oldPosition]) : tspSecond.GetCellPosition(VideoContainer[oldPosition]);
                    TableLayoutPanelCellPosition positionNew = newPosition == 0 ? tspFirst.GetCellPosition(VideoContainer[newPosition]) : tspSecond.GetCellPosition(VideoContainer[newPosition]);

                    Size oldSize = VideoContainer[oldPosition].Size;
                    Size newSize = VideoContainer[newPosition].Size;

                    VideoContainer[newPosition].Parent = null;
                    VideoContainer[oldPosition].Parent = null;

                    VideoContainer[newPosition].Size = oldSize;
                    VideoContainer[oldPosition].Size = newSize;

                    // VideoContainer[newPosition].Parent = tspFirst;
                    if (newPosition == 0)
                    {//Переставить со второй панели на первую
                        tspFirst.Controls.Add(VideoContainer[oldPosition]);
                        tspFirst.SetCellPosition(VideoContainer[oldPosition], positionNew);

                        tspSecond.Controls.Add(VideoContainer[newPosition]);
                        tspSecond.SetCellPosition(VideoContainer[newPosition], positionOld);
                        //Переставить позиции
                        int pos = TablePositionArray[positionNew.Row, positionNew.Column];
                        TablePositionArray[positionNew.Row, positionNew.Column] = RightTablePositionArray[positionOld.Row, positionOld.Column];
                        RightTablePositionArray[positionOld.Row, positionOld.Column] = pos;
                    }
                    else
                    {//Переставить с певой панели на вторую
                        tspSecond.Controls.Add(VideoContainer[oldPosition]);
                        tspSecond.SetCellPosition(VideoContainer[oldPosition], positionNew);

                        tspFirst.Controls.Add(VideoContainer[newPosition]);
                        tspFirst.SetCellPosition(VideoContainer[newPosition], positionOld);

                        //Переставить позиции
                        int pos = RightTablePositionArray[positionNew.Row, positionNew.Column];
                        RightTablePositionArray[positionNew.Row, positionNew.Column] = TablePositionArray[positionOld.Row, positionOld.Column];
                        TablePositionArray[positionOld.Row, positionOld.Column] = pos;
                    }
                }
            }
            //Переставить элементы в массиве окон 
            Control ctr = VideoContainer[newPosition];
            VideoContainer[newPosition] = VideoContainer[oldPosition];
            VideoContainer[oldPosition] = ctr;
            //Признак инвертирования окна
            bool rotationState = RotationStates[newPosition];
            RotationStates[newPosition] = RotationStates[oldPosition];
            RotationStates[oldPosition] = rotationState;
            //Перестановка в массиве пропорций
            float proportion = VideoAspectRatio[newPosition];
            VideoAspectRatio[newPosition] = VideoAspectRatio[oldPosition];
            VideoAspectRatio[oldPosition] = proportion;

            return true;
        }

        #endregion

        #region Video Proportions

        /// <summary>Интерфейс с поворотом двух окон</summary>
        /// <returns></returns>
        bool IsRotationDoubleMode()
        {
            return !RotationStates.Contains(false) && videostyle == VideoStyle.Double;
        }

        /// <summary>Установка размеров видеоокон</summary>
        public void SetVideoProportions()
        {
            process_resize = true;
            try
            {
                //Для полиэкрана
                if (Mode == ScreenMode.PolyScreen)
                {
                    if ((splitCtrlMain.Width - splitCtrlMain.SplitterDistance) < 0) return;

                    //Число колонок видеоокон
                    int columns = CalcColumnCount(2);

                    #region Обработка предельных положений сплиттера
                    if (Unit == MestureUnit.Persantage)
                    {
                        if (splitCtrlMain.SplitterDistance < (int)Math.Round(splitCtrlMain.Width * (FMinSplitterPercent / 100.0f)))
                            splitCtrlMain.SplitterDistance = (int)Math.Round(splitCtrlMain.Width * (FMinSplitterPercent / 100.0f)) + 1;
                        if (splitCtrlMain.SplitterDistance > (int)Math.Round(splitCtrlMain.Width * (FMaxSplitterPercent / 100.0f)))
                            splitCtrlMain.SplitterDistance = (int)Math.Round(splitCtrlMain.Width * (FMaxSplitterPercent / 100.0f)) - 1;
                    }
                    else
                    {
                        if (splitCtrlMain.SplitterDistance < FMinSplitterPercent) splitCtrlMain.SplitterDistance = (int)FMinSplitterPercent;
                        if (splitCtrlMain.SplitterDistance > FMaxSplitterPercent) splitCtrlMain.SplitterDistance = (int)FMaxSplitterPercent;

                    }
                    //Запрет передвижений
                    if (CanCheckPanelHeight)
                    {
                        int rightPanelWidth = tspSecond == null ? -1 : tspSecond.Width;
                        int position = GetSplitterPosition(splitCtrlMain.SplitterDistance, tspFirst.Width, rightPanelWidth);
                        if (position != splitCtrlMain.SplitterDistance)
                        {
                            splitCtrlMain.SplitterDistance = position;
                        }
                    }
                    #endregion

                    //Ширина видеоокон на первой половине сплиттера
                    int column_width = tspFirst.DisplayRectangle.Width / columns;
                    for (int i = 0; i < columns; i++) tspFirst.ColumnStyles[i].Width = column_width;

                    //Новый вариант
                    if (show_captions) tspFirst.RowStyles[0].Height = ToolBarHeight;    //Кнопки верхних окон

                    //Высота строки видеоокон
                    int row_height = (int)Math.Round(column_width * GetVideoAspectRatio(0));
                    //Повернутые телекамеры
                    if (videostyle == VideoStyle.TripleOneRow || videostyle == VideoStyle.DoubleRotation || videostyle == VideoStyle.HexagonRotation ||
                        CanRotateDoubleWinows(0) || IsRotationDoubleMode() || RotationStates.Contains(true))
                        row_height = (int)Math.Round(column_width * (1.0f / GetVideoAspectRatio(0)));

                    int row = 0;
                    if (show_captions) row = 1;
                    else tspFirst.RowStyles[row].Height = row_height;

                    //Если есть 2-й ряд окон
                    if (VideoContainer.Length >= 3 && videostyle != VideoStyle.TripleOneRow && videostyle != VideoStyle.HexagonRotation)
                    {
                        if (show_captions)
                        {
                            tspFirst.RowStyles[2].Height = ToolBarHeight;   //Кнопки нижних окон 3 строка
                            tspFirst.RowStyles[3].Height = row_height;      //Второй ряд видеоокон 4 строка
                            tspFirst.RowStyles[4].Height = splitCtrlMain.ClientSize.Height -
                                                           (2 * ToolBarHeight) - (2 * row_height);  //Высота панели плеера 5 строка
                        }
                        else
                        {
                            tspFirst.RowStyles[1].Height = row_height;      //Второй ряд видеоокон
                            if (tspFirst.RowStyles.Count > 2) tspFirst.RowStyles[2].Height = splitCtrlMain.ClientSize.Height - (2 * row_height);//Высота панели плеера
                        }
                    }

                    //Размеры маленьких видеоокон в случае интерфейса 6 видеоокон повернутых
                    if (videostyle == VideoStyle.HexagonRotation)
                    {
                        float coeff = 0.25f;
                        int offset = 15;
                        for (int i = 0; i < 6; i = i + 2)
                        {
                            ////Панель рамки
                            //Panel panel_small = (Panel)VideoContainer[i + 1].Parent;
                            //panel_small.Width = (int)Math.Round(column_width * coeff);
                            //panel_small.Height = (int)Math.Round(row_height * coeff);
                            //panel_small.Left = column_width - (int)Math.Round(column_width * coeff) - offset;
                            //panel_small.Top = row_height - (int)Math.Round(row_height * coeff) - offset;
                            ////Панель видео
                            //VideoContainer[i + 1].Width = panel_small.Width - 2;
                            //VideoContainer[i + 1].Height = panel_small.Height - 2;
                            //VideoContainer[i + 1].Left = 1;
                            //VideoContainer[i + 1].Top = 1;
                            //VideoContainer[i + 1].Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;

                            //panel_small.Visible = true;
                            //panel_small.BringToFront();

                            //Вариант без рамки
                            VideoContainer[i + 1].Width = (int)Math.Round(column_width * coeff);
                            VideoContainer[i + 1].Height = (int)Math.Round(row_height * coeff);
                            VideoContainer[i + 1].Left = column_width - (int)Math.Round(column_width * coeff) - offset;
                            VideoContainer[i + 1].Top = row_height - (int)Math.Round(row_height * coeff) - offset;
                            VideoContainer[i + 1].Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                        }
                        //for (int i = 0; i < 6; i = i + 2)
                        //{
                        //    Panel panel_small = (Panel)VideoContainer[i + 1].Parent;
                        //    panel_small.Visible = true;
                        //    panel_small.BringToFront();
                        //}
                    }
                    #region Второе видеоокно - вторая половина сплиттера
                    if (tspSecond != null)
                    {
                        tspSecond.ColumnStyles[0].Width = splitCtrlMain.Width - splitCtrlMain.SplitterDistance;
                        //Окна с заголовками (обычный вариант)
                        if (show_captions)
                        {
                            tspSecond.RowStyles[0].Height = ToolBarHeight;
                            //Высота видеоокна
                            float row_height_2 = tspSecond.ColumnStyles[0].Width * GetVideoAspectRatio(0);
                            //Повернутые телекамеры
                            if (videostyle == VideoStyle.TripleOneRow || CanRotateDoubleWinows(1))
                                row_height_2 = tspSecond.ColumnStyles[0].Width * (1.0f / GetVideoAspectRatio(0));

                            tspSecond.RowStyles[1].Height = row_height_2;
                        }
                        //Окна без заголовков
                        else
                        {
                            tspSecond.RowStyles[0].Height = tspSecond.ColumnStyles[0].Width * GetVideoAspectRatio(0);
                            if (videostyle == VideoStyle.TripleOneRow || CanRotateDoubleWinows(1))
                                tspSecond.RowStyles[0].Height = tspSecond.ColumnStyles[0].Width * (1.0f / GetVideoAspectRatio(0));
                        }
                    }
                    #endregion

                }
                if (Mode == ScreenMode.OneScreen) MaximizeWindow(MaxWindowIndex);

                SetReversiveView();
            }
            catch { };
            process_resize = false;
        }

        /// <summary>Отрисовка повёрнутых окон на основе списка</summary>
        void SetReversiveView()
        {
            for (int i = 0; i < VideoContainer.Length; i++)
            {
                {
                    if (videostyle == VideoStyle.TripleOneRow || videostyle == VideoStyle.DoubleRotation || !RotationStates[i] || IsRotationDoubleMode())
                    {
                        if ((videostyle == VideoStyle.TripleOneRow || videostyle == VideoStyle.DoubleRotation) && !RotationStates[i])
                        {
                            VideoContainer[i].Dock = DockStyle.None;
                            VideoContainer[i].Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                        }
                        else
                        {
                            VideoContainer[i].Dock = DockStyle.Fill;
                        }
                        //Для поиска в таблице
                        if (videostyle == VideoStyle.Hexagon)
                            VideoContainer[i].Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                        if (RotationStates[i])
                            VideoContainer[i].Height = (int)(VideoContainer[i].Width * (1.0f / VideoAspectRatio[i]));
                        else
                            VideoContainer[i].Height = (int)(VideoContainer[i].Width * VideoAspectRatio[i]);
                    }
                    else
                    {
                        int controlWidth = (int)(VideoContainer[i].Height * (VideoAspectRatio[i]));

                        TableLayoutPanel tPanel = VideoContainer[i].Parent as TableLayoutPanel;
                        TableLayoutPanelCellPosition cellPosition = tPanel.GetCellPosition(VideoContainer[i]);
                        Point oldControlLocation = VideoContainer[i].Location;

                        VideoContainer[i].Dock = DockStyle.None;
                        VideoContainer[i].Anchor = AnchorStyles.Top | AnchorStyles.Bottom;

                        if (cellPosition.Column > -1 && cellPosition.Row > -1)
                        {
                            float cellHeight = tPanel.RowStyles[cellPosition.Row].Height;
                            float width = tPanel.ColumnStyles[cellPosition.Column].Width;
                            int seed = cellPosition.Column;
                            int startX = tPanel.Location.X + seed * (int)width;
                            int locationX = startX + (int)(width / 2 - controlWidth / 2);
                            if (locationX > oldControlLocation.X)
                            {
                                VideoContainer[i].Location = new Point(locationX, oldControlLocation.Y);
                                VideoContainer[i].BringToFront();
                            }
                        }
                        VideoContainer[i].Width = controlWidth;

                    }
                }
            }
        }

        #region Calculate Height By Splitter Pos and Table Array and Rotattion State

        /// <summary>Требуется ли повернуть окно</summary>
        /// <param name="index">Индекс окна (с 0)</param>
        /// <returns>Результат</returns>
        bool CanRotateDoubleWinows(int index)
        {
            return RotationStates[index] && videostyle == VideoStyle.Double;
        }

        #region OldSetSplitterPositionStyle

        /// <summary>Рассчитать высоту которую занимают элементы PictureBox на панели</summary>
        int CalcHeightImages(TableLayoutPanel tlp, int panelIndex)
        {
            int columns = tlp.RowStyles.Count;
            columns = CalcColumnCount(columns);
            int columnWidsth = tlp.DisplayRectangle.Width / columns;
            int summ = 0;
            int rowCount = tlp.RowStyles.Count - 1;
            if (videostyle == VideoStyle.Double || videostyle == VideoStyle.TripleTwoRows)
            {
                rowCount = show_captions ? 2 : 1;
            }
            for (int i = 0; i < rowCount; i++)
            {
                if (show_captions && i % 2 == 0)//Тулбары
                {
                    summ += ToolBarHeight;
                }
                else
                {
                    if (panelIndex > 0)
                    {
                        summ += (int)(columnWidsth * RightTablePositionArray.GetMaxRatio(i, VideoAspectRatio,
                                                                                         RotationStates, videostyle == VideoStyle.Double));
                    }
                    else
                    {
                        //Для первой панели
                        summ += (int)(columnWidsth * TablePositionArray.GetMaxRatio(i, VideoAspectRatio, RotationStates,
                                                                                    videostyle == VideoStyle.TripleOneRow || videostyle == VideoStyle.Double));
                    }
                }
            }
            return summ;
        }

        /// <summary>Рассчитать выcоту левой панели</summary>
        int CalcPanelHeightLeftPanel()
        {
            int totalLeftPanlelHeight = CalcHeightImages(tspFirst, 0);
            return tspFirst.DisplayRectangle.Height - totalLeftPanlelHeight;
        }

        /// <summary>Рассчитать выоту правой панели</summary>
        public int CalcHeightRightPanel()
        {
            //Необходимо задать значени больше минимальной ширины для того чтобы условие было истинно 
            int totalRightPanelHeight = Math.Max((int)Math.Round(splitCtrlMain.Width * (FMaxSplitterPercent / 100.0f)) + 1, MinimalPanelHeight + 1);
            if (videostyle == VideoStyle.Double || videostyle == VideoStyle.Pentad)
            {
                if (tspSecond != null)
                    totalRightPanelHeight = tspSecond.DisplayRectangle.Height - CalcHeightImages(tspSecond, 1);
            }
            return totalRightPanelHeight;
        }

        /// <summary>Проверить высоту панели плеера</summary>
        public bool CheckLeftSplitPosition()
        {
            if (splitCtrlMain.SplitterDistance <= (int)(Math.Round(splitCtrlMain.Width * (FMinSplitterPercent / 100.0f)) + 1) + splitterStepSize)
                return true;
            return CalcPanelHeightLeftPanel() > (MinimalPanelHeight == 0 ? default_panelHeight : MinimalPanelHeight);
        }

        /// <summary>Проверить высоту панели информации</summary>
        public bool CheckRightSplitterPosition()
        {
            if (splitCtrlMain.SplitterDistance >= (Math.Round(splitCtrlMain.Width * (FMaxSplitterPercent / 100.0f)) + 1) - splitterStepSize)
                return true;
            return CalcHeightRightPanel() > (MinimalPanelHeight == 0 ? default_panelHeight : MinimalPanelHeight);
        }

        #endregion

        /// <summary>Возвращает число колонок видеоокон слева от сплиттера</summary>
        /// <param name="cnt">Число колонок по умолчанию</param>
        int CalcColumnCount(int cnt)
        {
            if ((VideoContainer.Length == 2 || VideoContainer.Length == 1) && videostyle != VideoStyle.DoubleRotation) return 1;
            if (IsRotationDoubleMode() && videostyle != VideoStyle.Hexagon) return 2;
            //6 окон обычных, 6 повернутых, 3 повернутых - три колонки окон
            if (videostyle == VideoStyle.Hexagon || videostyle == VideoStyle.TripleOneRow || videostyle == VideoStyle.HexagonRotation) return 3;
            if (videostyle == VideoStyle.DoubleRotation) return 2;
            return cnt;
        }

        /// <summary> Рассчитать высоту которую занимают элементы PictureBox на панели</summary>
        int CalcHeightImages(TableLayoutPanel tlp, int panelIndex, int panelWidth)
        {
            int columns = tlp.RowStyles.Count;
            columns = CalcColumnCount(columns);
            int columnWidsth = panelWidth / columns;
            int summ = 0;
            int rowCount = tlp.RowStyles.Count - 1;
            if (videostyle == VideoStyle.Double || videostyle == VideoStyle.TripleTwoRows || videostyle == VideoStyle.DoubleRotation)
            {
                rowCount = show_captions ? 2 : 1;
            }
            for (int i = 0; i < rowCount; i++)
            {
                if (show_captions && i % 2 == 0)//Тулбары
                {
                    summ += ToolBarHeight;
                }
                else
                {
                    if (panelIndex > 0)
                    {
                        summ += (int)(columnWidsth * RightTablePositionArray.GetMaxRatio(i, VideoAspectRatio, RotationStates,
                                                                                         videostyle == VideoStyle.Double));
                    }
                    else
                    {
                        //Для первой панели
                        summ += (int)(columnWidsth * TablePositionArray.GetMaxRatio(i, VideoAspectRatio, RotationStates,
                                                                                    videostyle == VideoStyle.TripleOneRow ||
                                                                                    videostyle == VideoStyle.Double ||
                                                                                    videostyle == VideoStyle.DoubleRotation));
                    }
                }
            }
            return summ;
        }

        /// <summary> Определение поизиции сплиттера</summary>
        /// <param name="positionSplitter">Позиция сплиттера</param>
        /// <param name="leftPanelWidth">Высота левой панели</param>
        /// <param name="rightPanelWidth">Высота правой панели если панель не создана то равна -1</param>
        /// <returns></returns>
        public int GetSplitterPosition(int positionSplitter, int leftPanelWidth, int rightPanelWidth)
        {
            bool isNeedCorrected = false;
            int splitterPosition = positionSplitter;
            do
            {
                int totalWidth = leftPanelWidth + (rightPanelWidth == -1 ? 0 : rightPanelWidth);
                float spliterRatio = (float)splitterPosition / totalWidth;
                if (spliterRatio > 1)
                {
                    spliterRatio = 1 / spliterRatio;
                }
                int newLeftPanelWidth = splitterPosition;
                int newRightPanelWidth = totalWidth - splitterPosition;

                if (FMinSplitterPercent / 100.0f >= spliterRatio && spliterRatio <= FMaxSplitterPercent / 100.0f)
                {
                    return splitterPosition;
                }

                int leftPictureBoxesHeight = CalcHeightImages(tspFirst, 0, newLeftPanelWidth);
                int rightPictureBoxesHeight = rightPanelWidth != -1 ? CalcHeightImages(tspSecond, 1, newRightPanelWidth) : 0;

                int leftPanelHeight = tspFirst.ClientRectangle.Height - leftPictureBoxesHeight;

                int rightPanelHeight = (rightPanelWidth != -1 ? (tspSecond.ClientRectangle.Height - rightPictureBoxesHeight) : (MinimalPanelHeight + 1));

                isNeedCorrected = leftPanelHeight < MinimalPanelHeight && rightPanelHeight > MinimalPanelHeight || rightPanelHeight < MinimalPanelHeight && leftPanelHeight > MinimalPanelHeight;

                if (leftPanelHeight <= (MinimalPanelHeight + default_incrementStepValue) && rightPanelHeight <= (MinimalPanelHeight + default_incrementStepValue))
                {
                    isNeedCorrected = false;
                }

                if (isNeedCorrected)
                {
                    int step = leftPanelHeight <= MinimalPanelHeight ? -default_incrementStepValue : default_incrementStepValue;
                    splitterPosition += step;
                }
            } while (isNeedCorrected);

            return splitterPosition;
        }

        #endregion

        #endregion

        #region Clear

        /// <summary>Очистка видеоокон</summary>
        public void Clear()
        {
            TimerEnableProcess.Stop();
            TimerEnableProcess = null;

            TimerRedraw.Stop();
            TimerRedraw = null;

            if (VideoContainer != null)
            {
                for (int i = 0; i < VideoContainer.Length; i++)
                {
                    VideoContainer[i] = null;
                }
                //VideoContainer = null;
            }

            if (ToolStrip != null)
            {
                for (int i = 0; i < ToolStrip.Length; i++) ToolStrip[i] = null;
                ToolStrip = null;
            }

            /*
            if (VideoWindows != null)
            {
                for (int i = 0; i < VideoWindows.Length; i++) VideoWindows[i] = null;
                //VideoContainer = null;
            }
            */

            splitCtrlMain.Panel1.Controls.Clear();
            splitCtrlMain.Panel2.Controls.Clear();

            splitCtrlMain.SplitterMoved -= splitCtrlMain_SplitterMoved;
            splitCtrlMain.SizeChanged -= splitCtrlMain_SizeChanged;
            splitCtrlMain.MouseDoubleClick -= splitCtrlMain_MouseDoubleClick;

            splitCtrlMain.Parent = null;

            splitCtrlMain = null;

            if (tspFirst != null)
            {
                tspFirst.Controls.Clear();
                tspFirst.Parent = null;
                tspFirst = null;
            }

            if (tspSecond != null)
            {
                tspSecond.Controls.Clear();
                tspSecond.Parent = null;
                tspSecond = null;
            }

            if (PanelPlay != null)
            {
                PanelPlay.Controls.Clear();
                PanelPlay.Parent = null;
                PanelPlay = null;
            }

            if (PanelInfo != null)
            {
                PanelInfo.Controls.Clear();
                PanelInfo.Parent = null;
                PanelInfo = null;
            }
        }

        /// <summary>Удалить указатель на панель плеера</summary>
        public void RemovePleerPanelPointer()
        {
            if (PanelPlay != null) PanelPlay.Parent = null;
            PanelPlay = null;
        }

        #endregion

        #region Size Splitter Title

        /// <summary>Установить одинаковый размер окон</summary>
        public void SetSameSize()
        {
            process_resize = true;
            try { splitCtrlMain.SplitterDistance = splitCtrlMain.Width / 2; }
            catch { };
            process_resize = false;
        }

        /// <summary>Установить одинаковый размер окон</summary>
        public void SetSplitterPos(double pos)
        {
            process_resize = true;
            try { splitCtrlMain.SplitterDistance = (int)(splitCtrlMain.Width * pos); }
            catch { };
            process_resize = false;
        }

        /// <summary>Панель отображения заголовка</summary>
        public void ShowTitle(bool show, string text, Color color)
        {
            show_title = show;
            if (show_title)
            {
                Label lbl = null;
                if (PanelTitle == null)
                {
                    PanelTitle = new Panel();
                    PanelTitle.Parent = splitCtrlMain.Panel1;
                    PanelTitle.Dock = DockStyle.Top;
                    PanelTitle.Height = title_panel_height;
                    tspFirst.Top = title_panel_height;
                    //Метка на панели
                    lbl = new Label();
                    lbl.Parent = PanelTitle;
                    lbl.Dock = DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Font = new Font("Segoe UI", 11.0f, FontStyle.Regular);
                    lbl.ForeColor = color;
                }
                else
                {
                    PanelTitle.Invoke((MethodInvoker)(() => PanelTitle.Visible = true));
                    if (PanelTitle.Controls.Count > 0)
                        lbl = (Label)PanelTitle.Controls[0];
                }
                if (lbl != null) lbl.Invoke((MethodInvoker)(() =>
                {
                    lbl.Text = text;
                    lbl.ForeColor = color;
                }));
            }
            else
            {
                if (PanelTitle != null)
                {
                    PanelTitle.Visible = false;
                    PanelTitle = null;
                    tspFirst.Top = 0;
                }
            }
        }

        /// <summary>Установить положение второго сплиттера</summary>
        public void SetSplitterSecondPos(double pos)
        {
            try { splitRight.SplitterDistance = (int)(splitRight.Height * pos); }
            catch { };
        }

        #endregion

        #region Properties

        /// <summary>Задать или получить текущее соотношение сторон видео</summary>
        public float AspectRatio
        {
            get { return GetAvgRatio(); }
            set
            {
                PushProportions(value, GetWindowCountByVideoStyle(videostyle));
                //VideoAspectRatio = value;
                SetVideoProportions();
            }
        }

        /// <summary>Массив пропорций</summary>
        public List<float> GetVideoAspectRatioList
        {
            get
            {
                return VideoAspectRatio;
            }
            set
            {
                VideoAspectRatio = value;
                SetVideoProportions();
            }
        }

        /// <summary>Панель информации</summary>
        public Panel InfoPanel
        {
            get { return PanelInfo; }
        }

        /// <summary>Панель упрвления воспроизведением</summary>
        public Panel VideoPlayPanel
        {
            get { return PanelPlay; }
        }

        /// <summary>Статус процесса изменения размеров и положения панелей</summary>
        public bool ProcessResize
        {
            get { return process_resize; }
        }

        /*
        /// <summary>Возвращает массив видеопанелей основного монитора</summary>
        public PictureBox[] VideoWindows
        {
            get { return PictureBoxVideo; }
        }
        */

        /// <summary>Возвращает массив контейнеров размещения видео
        /// массив PictureBox или Panel</summary>
        public Control[] VideoContainer
        {
            get
            {
                if (enable_panels) return PanelVideo;
                else return PictureBoxVideo;
            }
        }

        /// <summary>Возвращает видеопанель выводимого на второй монитор</summary>
        public PictureBox ExternalVideoWindow
        {
            get { return ExternalPictureBoxVideo; }
        }

        /// <summary>Отображение заголовков видеоокон</summary>
        public bool ShowCaptions
        {
            get { return show_captions; }
            set { show_captions = value; }
        }

        /// <summary>Размер правой панели (сплиттера) фиксирован при зменении размера окна</summary>
        public bool FixRightPanel
        {
            get { return fix_right_panel; }
            set
            {
                fix_right_panel = value;
                if (fix_right_panel) splitCtrlMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            }
        }

        /// <summary>Скрытая панель кнопок</summary>
        public bool HiddenCaptions
        {
            get { return hidden_captions; }
            set { hidden_captions = value; }
        }

        /// <summary>Разрешить увеличение окна одним кликов</summary>
        public bool EnableMaximize
        {
            get { return enable_maximize; }
            set { enable_maximize = value; }
        }

        /// <summary>Положение ползунка по умолчанию</summary>
        double DefaultSplitterSize
        {
            get { return FDefaultSplitterSize; }
            set { FDefaultSplitterSize = value; }
        }

        #endregion

        #region Panel Caption

        /// <summary>Удаление элементов заголовка над видеопанелью</summary>
        public void ClearPanelCaption(int panel_no)
        {
            if (ToolStrip == null) return;
            if (ToolStrip[panel_no] == null) return;
            if (panel_no < ToolStrip.Length)
            {
                while (ToolStrip[panel_no].Items.Count > 0)
                {
                    ToolStrip[panel_no].Items.RemoveAt(0);
                }
            }
        }

        /// <summary>Установка текста над видеопанелью</summary>
        /// <param name="panel_no">Номер панели</param>
        /// <param name="text">Текст</param>
        public void SetPanelText(int panel_no, string text)
        {
            if (ToolStrip == null) return;
            if (panel_no < ToolStrip.Length)
            {
                ToolStripLabel tsl = new ToolStripLabel(text);
                tsl.Alignment = ToolStripItemAlignment.Left;
                tsl.Name = "ChannelName";

                ToolStripItem[] items = ToolStrip[panel_no].Items.Find("ChannelName", false);
                if (items != null)
                    for (int i = 0; i < items.Length; i++)
                        ToolStrip[panel_no].Items.Remove(items[i]);

                ToolStrip[panel_no].Items.Add(tsl);
            }
        }

        /// <summary>Изменение текста над видеопанелью</summary>
        /// <param name="panel_no">Номер панели</param>
        /// <param name="text">Текст</param>
        public void ChangePanelText(int panel_no, string text)
        {
            if (ToolStrip == null) return;
            if (panel_no < ToolStrip.Length)
                for (int i = 0; i < ToolStrip[panel_no].Items.Count; i++)
                    if (ToolStrip[panel_no].Items[i].GetType().Name == "ToolStripLabel")
                    {
                        ((ToolStripLabel)ToolStrip[panel_no].Items[i]).Text = text;
                        return;
                    }
        }

        /// <summary>Установка кнопки панели</summary>
        /// <param name="panel_no">Номер панеди</param>
        /// <param name="type">Тип кнопки</param>
        /// <param name="button_image">Картинка кнопки</param>
        /// <param name="text">Подсказка кнопки</param>
        public void SetPanelButtons(int panel_no, ToolbarButton type, Image button_image, string text)
        {
            if (show_captions)
            {
                if (ToolStrip == null) return;
                if (panel_no < ToolStrip.Length)
                {
                    ToolStripButton btn = new ToolStripButton(button_image);
                    btn.Alignment = ToolStripItemAlignment.Right;
                    btn.Tag = type;
                    btn.ToolTipText = text;
                    btn.CheckOnClick = false;
                    btn.Click += new EventHandler(btn_Click);
                    ToolStrip[panel_no].Items.Add(btn);
                }
            }

            if (hidden_captions)
            {
                if (!SearchBtn(type))
                {
                    ToolStripButton btn = new ToolStripButton(button_image);
                    btn.Alignment = ToolStripItemAlignment.Right;
                    btn.Tag = type;
                    btn.ToolTipText = text;
                    btn.CheckOnClick = false;
                    btn.Click += new EventHandler(btn_Click);
                    tsHidden.Items.Add(btn);
                    //Ширина панели
                    pnBtns.Width = tsHidden.Items.Count * 24;
                }
            }
        }

        #endregion

        #region Custom Button Item

        /// <summary>Получить номер панели</summary>
        string GetNumberPart(string str)
        {
            string number = string.Empty;
            string separator = "__";
            int position = str.IndexOf(separator);
            if (position > -1)
            {
                number = str.Substring(position).Replace(separator, string.Empty);
            }
            return number;
        }

        /// <summary>Переименовать кнопки</summary>
        void RenameChildControllsAndAddEvent(ToolStripItemCollection items, int panelNo)
        {
            if (items.Count > 0)
            {
                foreach (ToolStripItem item in items)
                {
                    item.Name += ("__" + panelNo);
                    item.MouseDown += new MouseEventHandler(item_MouseDown);
                }
            }
        }

        /// <summary>Установить пользовательскый элемент на панели</summary>
        /// <param name="panelNumber">Номер панели</param>
        /// <param name="control">Элемент</param>
        public void SetPanelCustomItem(int panelNumber, ToolStripItem control)
        {
            //Событие
            //control.Click += new EventHandler(btn_CustomClick);

            //Видимые контролы
            if (show_captions)
            {
                if (ToolStrip == null) return;
                if (panelNumber < ToolStrip.Length) ToolStrip[panelNumber].Items.Add(control);
            }

            //Скрытые контролы
            if (hidden_captions)
            {
                control.Enabled = false;
                tsHidden.Items.Add(control);
                pnBtns.Width = tsHidden.Items.Count * 24;   //Ширина панели
            }
        }

        /// <summary>Удалить пользовательский элемент с панели</summary>
        /// <param name="panelNumber">Номер панели</param>
        public void RemoveCustomButton(int panelNumber)
        {
            string endPrefix = "__" + panelNumber;
            for (int i = 0; i < ToolStrip[panelNumber].Items.Count; i++)
            {
                if (i > ToolStrip[panelNumber].Items.Count - 1)
                {
                    return;
                }
                if (ToolStrip[panelNumber].Items[i].Name.EndsWith(endPrefix))
                {
                    ToolStrip[panelNumber].Items.RemoveAt(i);
                    RemoveCustomButton(panelNumber);
                }
            }
        }

        /// <summary>Удалить все произвольные кнопки с загловков всех панелей</summary>
        public void RemoveCustomButtonFromAllPannel()
        {
            for (int i = 0; i < ToolStrip.Length; i++)
            {
                RemoveCustomButton(i);
            }
        }

        /// <summary>Удалить призвольные кнопки с панели по ключу необходим для обновления заголовков тулбаров</summary>
        public void RemoveCustomButtonByKeys(int panelNumber)
        {
            string endPrefix = "__" + panelNumber;
            List<string> names = new List<string>();
            for (int i = 0; i < ToolStrip[panelNumber].Items.Count; i++)
            {
                if (ToolStrip[panelNumber].Items[i].Name.EndsWith(endPrefix))
                {
                    names.Add(ToolStrip[panelNumber].Items[i].ImageKey);
                }
            }
            foreach (var name in names)
            {
                ToolStrip[panelNumber].Items.RemoveByKey(name);
            }
        }

        /// <summary> Нажатие на произвольную кнопку </summary>
        protected void OnEventCustomButtonClick(int panel, object sender)
        {
            if (EventCustomButtonClick != null)
            {
                EventCustomButtonClick(panel, sender);
            }
        }

        /// <summary> Нажатие на кнопку </summary>
        void item_MouseDown(object sender, MouseEventArgs e)
        {
            var dropdown = sender as ToolStripDropDown;
            // var button = Sender as ;
            var item = sender as ToolStripItem;

            string pannelNumber = dropdown != null ? GetNumberPart(dropdown.Name) : string.Empty;
            pannelNumber = item != null ? GetNumberPart(item.Name) : pannelNumber;

            int panel_no = 0;
            int.TryParse(pannelNumber, out panel_no);
            OnEventCustomButtonClick(panel_no, sender);
        }

        #endregion

        #region Panel Buttons And ToolTips

        /// <summary>Отобразить панель инструментов</summary>
        void ShowButtonsPanel(Control pb, bool show)
        {
            if (show)
            {
                Point locationOnScreen = pb.Parent.PointToScreen(pb.Location);//Координаты окна на экране
                Point locationOnForm = pb.FindForm().PointToClient(locationOnScreen);//Координаты окна на форме
                pnBtns.Left = locationOnForm.X + pb.Width - pnBtns.Width - 2;
                pnBtns.Top = locationOnForm.Y + 2;
                tsHidden.Tag = pb.Tag;
                pnBtns.Tag = pb.Tag;
                pnBtns.Visible = true;
                pnBtns.BringToFront();
            }
            else
            {
                pnBtns.Visible = false;
            }
        }

        /// <summary>Найти кнопку </summary>
        bool SearchBtn(ToolbarButton type)
        {
            for (int i = 0; i < tsHidden.Items.Count; i++)
                if ((int)tsHidden.Items[i].Tag == (int)type) return true;
            return false;
        }

        /// <summary>Активация контролов на панели</summary>
        /// <param name="panel_no">Номер панели</param>
        /// <param name="enable">Активность</param>
        public void EnableButtons(int panel_no, bool enable)
        {
            if (ToolStrip == null) return;
            if (panel_no < ToolStrip.Length)
            {
                EnableButtonsThreadSafe(ToolStrip[panel_no], enable);
            }
        }

        /// <summary>Видимость контролов на панели</summary>
        /// <param name="panel_no">Номер панели</param>
        /// <param name="visible">Видимость</param>
        public void VisibleButtons(int panel_no, bool visible)
        {
            if (ToolStrip == null) return;
            if (panel_no < ToolStrip.Length)
            {
                VisibleButtonsThreadSafe(ToolStrip[panel_no], visible);
            }
        }

        /// <summary>Нажатие кнопки на панели</summary>
        void btn_Click(object Sender, EventArgs e)
        {
            int panel_no = (int)((ToolStripButton)Sender).Owner.Tag;
            ToolbarButton type = (ToolbarButton)((ToolStripButton)Sender).Tag;
            if (EventButtonClick != null) EventButtonClick(panel_no, type);
            //Обработка кнопки максимизации
            if (type == ToolbarButton.Maximize) MaximizeClick(panel_no);
        }

        /// <summary>Отображение сообщения о сохранении изображения</summary>
        /// <param name="type">Тип канала</param>
        public void ShowToolTip(int panel_no)
        {
            int index_tk = panel_no;
            ToolTip tip = new ToolTip();
            string s = "Кадр телевизионной камеры сохранен ..";
            int x = 4;
            int y = 4;
            tip.BackColor = Color.FromKnownColor(KnownColor.LightYellow);
            tip.ShowAlways = true;
            VideoContainer[panel_no].Focus();
            tip.Show(s, VideoContainer[panel_no], x, y, 1500);
        }

        #endregion

        #region Context Menu and Static Images

        /// <summary>Активация контекстного меню</summary>
        /// <param name="enable">Активность</param>
        public void EnableContextMenu(bool enable)
        {
            if (ContextStrip == null) return;
            for (int i = 0; i < ContextStrip.Length; i++)
            {
                VideoContainer[i].Invoke((MethodInvoker)(() => VideoContainer[i].ContextMenu.MenuItems[0].Enabled = enable));
            }
        }

        /// <summary>Обработка клика </summary>
        void ContextMenu_Click(object sender, EventArgs e)
        {
            if (EventPanelStopFrame != null)
                EventPanelStopFrame(Convert.ToInt32((sender as MenuItem).Tag));
        }

        /// <summary>Установить статическое изображение вместо видео</summary>
        /// <param name="window">Номер окна (с 0)</param>
        /// <param name="bmp">Картинка</param>
        public void SetStaticImage(int window, Bitmap bmp)
        {
            try
            {
                if (enable_panels)
                {
                    if (PanelVideo.Length > window)
                        PanelVideo[window].Invoke((MethodInvoker)(() => PanelVideo[window].BackgroundImage = bmp));
                }
                else
                {
                    if (PictureBoxVideo.Length > window)
                        PictureBoxVideo[window].Invoke((MethodInvoker)(() => PictureBoxVideo[window].Image = bmp));
                }
            }
            catch { }
        }

        #endregion

        #region Thread Safe Button and Tooltip

        /// <summary>Безопасная активация заголовка</summary>
        void EnableButtonsThreadSafe(ToolStrip toolstrip, bool enable)
        {
            if (toolstrip.InvokeRequired)
            {
                SetEnableToolStripCallback d = new SetEnableToolStripCallback(EnableButtonsThreadSafe);
                try { toolstrip.Invoke(d, new object[] { toolstrip, enable }); } catch { };
            }
            else
            {
                toolstrip.Enabled = enable;
            }
        }

        /// <summary>Безопасная активация заголовка</summary>
        void VisibleButtonsThreadSafe(ToolStrip toolstrip, bool visible)
        {
            if (toolstrip.InvokeRequired)
            {
                SetVisibleToolStripCallback d = new SetVisibleToolStripCallback(VisibleButtonsThreadSafe);
                try { toolstrip.Invoke(d, new object[] { toolstrip, visible }); } catch { };
            }
            else
            {
                if (toolstrip == null) return;
                for (int i = 0; i < toolstrip.Items.Count; i++) toolstrip.Items[i].Visible = visible;
            }
        }

        /// <summary>Потокобезопасное отображение подскказки</summary>
        public void ShowToolTipThreadSafe(int panel_no)
        {
            if (VideoContainer[panel_no].InvokeRequired)
            {
                ShowToolTipCallback d = new ShowToolTipCallback(ShowToolTipThreadSafe);
                try { VideoContainer[panel_no].Invoke(d, new object[] { panel_no }); } catch { };
            }
            else
            {
                ShowToolTip(panel_no);
            }
        }

        #endregion

        #region Master Channel

        /// <summary>Включить мастер-канал</summary>
        public void MasterChannelCheck(int panel, Image CheckedImage, Image UnCheckedImage)
        {
            if (ToolStrip == null) return;
            for (int i = 0; i < ToolStrip.Length; i++)
                for (int j = 0; j < ToolStrip[i].Items.Count; j++)
                    if (ToolStrip[i].Items[j].Tag != null &&
                        ((ToolbarButton)ToolStrip[i].Items[j].Tag) == ToolbarButton.Master)
                    {
                        ((ToolStripButton)ToolStrip[i].Items[j]).Checked = panel == i;
                        if (panel == i)
                        {
                            if (CheckedImage != null)
                                ((ToolStripButton)ToolStrip[i].Items[j]).Image = CheckedImage;
                            ((ToolStripButton)ToolStrip[i].Items[j]).ToolTipText = "Мастер канал";
                        }
                        else
                        {
                            if (UnCheckedImage != null)
                                ((ToolStripButton)ToolStrip[i].Items[j]).Image = UnCheckedImage;
                            ((ToolStripButton)ToolStrip[i].Items[j]).ToolTipText = "Включить мастер канал";
                        }
                    }
        }

        /// <summary>Задействовать кнопку мастер-канал</summary>
        public void MasterChannelVisible(bool visible)
        {
            if (ToolStrip == null) return;
            for (int i = 0; i < ToolStrip.Length; i++)
                for (int j = 0; j < ToolStrip[i].Items.Count; j++)
                    if (ToolStrip[i].Items[j].Tag != null &&
                        ((ToolbarButton)ToolStrip[i].Items[j].Tag) == ToolbarButton.Master)
                    {
                        ((ToolStripButton)ToolStrip[i].Items[j]).Visible = visible;
                    }
        }

        #endregion

        #region Вспомогательные таймеры

        /// <summary>Создание вспомогательных таймеров</summary>
        void CreateTimers()
        {
            //Таймер разрешения отрисовки кадров
            TimerEnableProcess = new System.Timers.Timer();
            TimerEnableProcess.Interval = 300;
            TimerEnableProcess.Elapsed += new System.Timers.ElapsedEventHandler(TimerEnableProcess_Elapsed);
            TimerEnableProcess.Enabled = false;

            //Таймер перерисовки
            TimerRedraw = new System.Timers.Timer();
            TimerRedraw.Interval = 100;
            TimerRedraw.Elapsed += new System.Timers.ElapsedEventHandler(TimerRedraw_Elapsed);
            TimerRedraw.Enabled = false;
        }

        /// <summary>Обработка события таймера разрешения отрисовки кадров</summary>
        void TimerEnableProcess_Elapsed(object sender, EventArgs e)
        {
            TimerEnableProcess.Stop();
            process_resize = false;
        }

        /// <summary>Обработка события таймера перерисовки кадров</summary>
        void TimerRedraw_Elapsed(object sender, EventArgs e)
        {
            if (process_resize) return;
            //Repaint();
        }

        #endregion

        #region Splitter Events

        /// <summary>Изменение общего размера панели сплиттера - установка размеров видеоокон</summary>
        void splitCtrlMain_SizeChanged(object sender, EventArgs e)
        {
            process_resize = true;

            try { SetVideoProportions(); } catch { };

            process_resize = false;
        }

        /// <summary>Завершение перемещения ползунка сплиттера</summary>
        void splitCtrlMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            process_resize = true;

            try
            {
                SetVideoProportions();
                if (EventSplitterMoved != null)
                    EventSplitterMoved(1.0d * splitCtrlMain.SplitterDistance / splitCtrlMain.Width);
            }
            catch { };

            process_resize = false;
        }

        /// <summary>Начало перемещения ползунка сплиттера</summary>
        void splitCtrlMain_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            process_resize = true;
            process_resize = false;
        }

        /// <summary>Установка ползунка по умолчанию при двойном клике</summary>
        public void splitCtrlMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            splitCtrlMain.SplitterDistance = (int)Math.Round(FDefaultSplitterSize * splitCtrlMain.Width);
        }

        /// <summary>Завершение перемещения ползунка второго сплиттера (справа)</summary>
        void splitRight_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (EventSplitterSecond != null)
                EventSplitterSecond(1.0d * ((SplitContainer)sender).SplitterDistance / ((SplitContainer)sender).Height);
        }

        #endregion

        #region Maximize windows

        int GetElementRowByIndex(int index)
        {
            if (videostyle == VideoStyle.Hexagon) return index % 3;
            return index % 2;
        }

        /// <summary>Увеличение окна</summary>
        /// <param name="index">Номер окна (с 0)</param>
        /// <param name="VideoPanelHeight">Высота панели управления видеоплеером (под окном)</param>
        public void MaximizeWindow(int index)
        {
            try
            {


                Mode = ScreenMode.OneScreen;
                MaxWindowIndex = index;
                process_resize = true;

                #region Оставить видимой только 1 видеоокно
                for (int i = 0; i < VideoContainer.Length; i++)
                {
                    if (i == index)
                    {
                        VideoContainer[i].Visible = true;
                        if (ToolStrip != null && ToolStrip[i] != null) ToolStrip[i].Visible = true;
                    }
                    else
                    {
                        VideoContainer[i].Visible = false;
                        if (ToolStrip != null && ToolStrip[i] != null) ToolStrip[i].Visible = false;
                    }
                }
                #endregion

                //Расчет размера окна
                int w = splitCtrlMain.SplitterDistance;                 //Ширина
                int h = (int)Math.Round(w * VideoAspectRatio[index]);   //Высота (по пропорции)
                if (videostyle == VideoStyle.Hexagon)
                {
                    int columns = 2;
                    columns = CalcColumnCount(columns);
                    int width = (int)Math.Round(1f * tspFirst.DisplayRectangle.Width / columns);//Ширина одного видеоокна
                    //Расчет высоты окна
                    h = (int)Math.Round((width * GetVideoAspectRatio(0)) + (width * GetVideoAspectRatio(1)));//Высота двух видеоокон
                }

                #region Hexagon Single Window Proportions

                /*
                if (videostyle == VideoStyle.Hexagon)
                {
                    //Высота видеоокна
                    float rowHeight = 0;
                    //tspFirst.RowStyles.Count-1 - панель плеера необходимо исключить из рассмотрения
                    for (int i = 0; i < tspFirst.RowStyles.Count - 1; i++) rowHeight += tspFirst.RowStyles[i].Height;
                    h = (int)rowHeight;
                    w = (int)Math.Round(h * 1 / VideoAspectRatio[index]);
                }
                */

                #endregion

                #region Ширина окон
                int col = GetMaxColumn(index);
                if (col >= 0)
                    for (int i = 0; i < tspFirst.ColumnStyles.Count; i++)
                    {
                        if (col == i) tspFirst.ColumnStyles[i].Width = w; //Установлена ширина нужной колонки
                        else tspFirst.ColumnStyles[i].Width = 0;          //Ширина остальных колонок = 0
                    }
                //Переместить панель плеера на нужную колонку (под видеоокно)
                tspFirst.SetColumn(PanelPlay, col);

                /*
                if (videostyle != VideoStyle.Hexagon)
                {
                    //четная колонка
                    if (index % 2 == 0)
                    {
                        tspFirst.ColumnStyles[0].Width = w; //Установлена ширина 0 колонки
                        tspFirst.ColumnStyles[1].Width = 0; //Ширина 1 колонки = 0
                        tspFirst.SetColumn(PanelPlay, 0);   //Переместить панель плеера на 0 колонку (под видеоокно)
                    }
                    else
                    {
                        tspFirst.ColumnStyles[0].Width = 0; //Ширина 0 колонки = 0
                        tspFirst.ColumnStyles[1].Width = w; //Установлена ширина 1 колонки
                        tspFirst.SetColumn(PanelPlay, 1);   //Переместить панель плеера на 1 колонку (под видеоокно)
                    }
                }
                else
                {
                   MatrixPosition position = _tablePositionArray.GetPositionByValue(index);
                   for (int i = 0; i < tspFirst.ColumnStyles.Count; i++)
                   {
                       tspFirst.ColumnStyles[i].Width = i == position.Col ? w : 0;
                       if (i == position.Col) CentrateHexagon(w, h, i);
                   }
                }
                */
                #endregion

                #region Отображение заголовков видеоокон
                if (show_captions)
                {
                    if (videostyle != VideoStyle.Hexagon)
                    {
                        //Строка
                        if (index == 0 || index == 1)
                        {
                            tspFirst.RowStyles[0].Height = ToolBarHeight;
                            tspFirst.RowStyles[1].Height = h;   //Ряд верхних окон
                            tspFirst.RowStyles[2].Height = 0;
                            tspFirst.RowStyles[3].Height = 0;   //Ряд нижних окон
                            tspFirst.SetRow(PanelPlay, 3);      //Ряд плеера
                        }
                        if (index == 2 || index == 3)
                        {
                            tspFirst.RowStyles[0].Height = 0;
                            tspFirst.RowStyles[1].Height = 0;   //Ряд верхних окон
                            tspFirst.RowStyles[2].Height = ToolBarHeight;
                            tspFirst.RowStyles[3].Height = h;   //Ряд нижних окон
                            tspFirst.SetRow(PanelPlay, 4);      //Ряд плеера
                        }
                    }
                    else
                    {
                        //Иной порядок камер можно сделать через массив позиций
                        if (index == 0 || index == 1 || index == 2)
                        {
                            tspFirst.RowStyles[0].Height = ToolBarHeight;
                            tspFirst.RowStyles[1].Height = h;   //Ряд верхних окон
                            tspFirst.RowStyles[2].Height = 0;
                            tspFirst.RowStyles[3].Height = 0;   //Ряд нижних окон
                            tspFirst.SetRow(PanelPlay, 3);      //Ряд плеера
                        }
                        if (index == 3 || index == 4 || index == 5)
                        {
                            tspFirst.RowStyles[0].Height = 0;
                            tspFirst.RowStyles[1].Height = 0;   //Ряд верхних окон
                            tspFirst.RowStyles[2].Height = ToolBarHeight;
                            tspFirst.RowStyles[3].Height = h;   //Ряд нижних окон
                            tspFirst.SetRow(PanelPlay, 4);      //Ряд плеера
                        }
                    }
                }
                #endregion

                #region Высота окон
                int row = GetMaxRow(index);
                if (row >= 0)
                    for (int i = 0; i < tspFirst.RowStyles.Count; i++)
                    {
                        if (row == i) tspFirst.RowStyles[i].Height = h; //Установлена высота нужной строки
                        else tspFirst.RowStyles[i].Height = 0;          //Высота остальных колонок = 0
                    }

                /*
                if (hidden_captions)
                {
                    //tspFirst.SuspendLayout();
                    //tspFirst.Visible = false;
                    
                    if (videostyle != VideoStyle.Hexagon)
                    {
                        if (index == 0 || index == 1)
                        {
                            tspFirst.RowStyles[0].Height = h;   //Строка 0
                            tspFirst.RowStyles[1].Height = 0;   //Строка 1
                            tspFirst.RowStyles[2].Height = 0;   //Строка 2
                        }
                        if (index == 2 || index == 3)
                        {
                            tspFirst.RowStyles[0].Height = 0;   //Строка 0
                            tspFirst.RowStyles[1].Height = h;   //Строка 1
                            tspFirst.RowStyles[2].Height = 0;   //Строка 2
                        }
                    }
                    else
                    {
                        MatrixPosition position = _tablePositionArray.GetPositionByValue(index);
                        for (int i = 0; i < 3; i++)
                        {
                            tspFirst.RowStyles[i].Height = position.Row == i ? h : 0;
                            if (position.Row == i) CentrateHexagon(w, h, i);
                        }
                    }

                    //tspFirst.Visible = true;
                    //tspFirst.ResumeLayout();
                }
                */
                #endregion

                //Центровка видеоокна в случае 6-ти видеоокон
                if (videostyle == VideoStyle.Hexagon)
                {
                    VideoContainer[index].Dock = DockStyle.None;      //Отвязка границ
                    VideoContainer[index].Anchor = AnchorStyles.None; //Отвязка границ
                    int height = h;//Высота остается
                    int width = (int)Math.Round(h * (1 / VideoAspectRatio[index]));//Ширина видеоконтейнера - по пропорции
                    VideoContainer[index].Size = new Size(width, height);
                    VideoContainer[index].Location = new Point((tspFirst.Width - width) / 2, 0);
                    //VideoWindows[index].Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;   //Привязка границ
                }
                else
                {
                    //Размеры видеоокна
                    VideoContainer[index].Width = w;
                    VideoContainer[index].Height = h;
                }

                if (pnBtns != null) pnBtns.Visible = false;
                //Ряд плеера на 1-ю строку
                tspFirst.SetRowSpan(PanelPlay, 1);
                process_resize = false;

                //if (EventPanelMaximize != null) EventPanelMaximize(index, true);
                if (EventPanelResize != null) EventPanelResize(index);
            }
            catch { };
        }

        /// <summary>Вернуть номер увеличиваемой колонки</summary>
        /// <param name="index">Индекс окна (с 0)</param>
        int GetMaxColumn(int index)
        {
            if (videostyle != VideoStyle.Hexagon)
            {
                if (index == 0) return 0;
                if (index == 1) return 1;
                if (index == 2) return 0;
                if (index == 3) return 1;
            }
            else
            {
                //6 окон
                if (index == 0) return 0;
                if (index == 1) return 1;
                if (index == 2) return 2;
                if (index == 3) return 0;
                if (index == 4) return 1;
                if (index == 5) return 2;
            }
            return -1;
        }

        /// <summary>Вернуть номер увеличиваемой строки</summary>
        /// <param name="index">Индекс окна (с 0)</param>
        int GetMaxRow(int index)
        {
            if (videostyle != VideoStyle.Hexagon)
            {
                if (index == 0) return 0;
                if (index == 1) return 0;
                if (index == 2) return 1;
                if (index == 3) return 1;
            }
            else
            {
                //6 окон
                if (index == 0) return 0;
                if (index == 1) return 0;
                if (index == 2) return 0;
                if (index == 3) return 1;
                if (index == 4) return 1;
                if (index == 5) return 1;
            }
            return -1;
        }

        void CentrateHexagon(int w, int h, int i)
        {
            int len = VideoContainer[i].Width;
            Size vwSize = new Size(w, h);
            tspFirst.ColumnStyles[i].Width = tspFirst.Width;

            VideoContainer[i].Anchor = AnchorStyles.None;
            //VideoWindows[i].Dock = DockStyle.None;

            VideoContainer[i].Size = vwSize;
            VideoContainer[i].Location = new Point((tspFirst.Width - w) / 2, VideoContainer[i].Location.Y);

            VideoContainer[i].Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            //VideoWindows[i].Dock = DockStyle.None;


        }

        public int GetMaximizedWindows()
        {
            return MaxWindowIndex;
        }

        /// <summary>Режим полиэкрана</summary>
        public void PolyScreen(int windows_count)
        {
            Mode = ScreenMode.PolyScreen;

            process_resize = true;

            if (windows_count == 3) SetTripleWindows();
            if (windows_count == 4) SetFourWindows();
            if (windows_count == 5) SetFiveWindows();
            //Нужен ли
            if (windows_count == 6) { SetSixWindows(); SetVideoProportions(); }
            process_resize = false;

            //if (EventPanelMaximize != null) EventPanelMaximize(MaxWindowIndex, false);
            if (EventPanelResize != null) EventPanelResize(MaxWindowIndex);
        }

        void MaximizeClick(int panel_no)
        {
            //tspFirst.SuspendLayout();
            //tspFirst.Visible = false;
            if (Mode == ScreenMode.PolyScreen)
            {
                MaximizeWindow(panel_no);
            }
            else//if (Mode == ScreenMode.OneScreen)
            {
                PolyScreen((int)videostyle);
            }
            //tspFirst.Visible = true;
            //tspFirst.ResumeLayout();
        }

        /// <summary>Режим максимизации окна / полиэкран</summary>
        public ScreenMode MaximizeMode { get { return Mode; } }

        /// <summary>Номер увеличенного окна</summary>
        public int MaximumWindow { get { return MaxWindowIndex; } }

        #endregion

        #region Mouse

        void VideoPanels_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int panel_no = -1;
            if (sender.GetType().Name == "PictureBox") panel_no = (int)((PictureBox)sender).Tag;
            if (sender.GetType().Name == "Panel") panel_no = (int)((Panel)sender).Tag;

            if (EventPanelDoubleClick != null) EventPanelDoubleClick(panel_no);

            if (enable_maximize)
            {
                if (panel_no > -1) MaximizeClick(panel_no);
            }
        }

        void VideoPanels_MouseHover(object sender, EventArgs e)
        {
            if (EventPanelMouseHover != null) EventPanelMouseHover((int)((Control)sender).Tag);
            if (hidden_captions)
            {
                ShowButtonsPanel((Control)sender, GetOnTop((Control)sender, Cursor.Position));
            }
        }

        void VideoPanels_MouseEnter(object sender, EventArgs e)
        {
            if (hidden_captions)
            {
                ShowButtonsPanel((Control)sender, GetOnTop((Control)sender, Cursor.Position));
            }
            //((PictureBox)sender).Focus();
        }

        void VideoPanels_MouseMove(object sender, MouseEventArgs e)
        {
            if (hidden_captions)
            {
                ShowButtonsPanel((Control)sender, GetOnTop((Control)sender, Cursor.Position));
            }
            if (drag_panel)
            {
                if (EventPanelMouseDrag != null)
                    EventPanelMouseDrag((int)((Control)sender).Tag,
                                        (1.0d * start_x / ((Control)sender).Width),
                                        (1.0d * start_y / ((Control)sender).Height),
                                        (1.0d * e.X / ((Control)sender).Width),
                                        (1.0d * e.Y / ((Control)sender).Height));
                start_x = e.X;
                start_y = e.Y;
            }
        }

        /// <summary>Колесо мыши</summary>
        void VideoPanels_MouseWheel(object sender, MouseEventArgs e)
        {
            if (EventPanelMouseWheel != null) EventPanelMouseWheel((int)((Control)sender).Tag, e.Delta,
                                                                   (1.0d * e.X / ((Control)sender).Width),
                                                                   (1.0d * e.Y / ((Control)sender).Height));
        }

        /// <summary>Нажатие кнопки мыши на панели</summary>
        void VideoPanels_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drag_panel = true;
                start_x = e.X;
                start_y = e.Y;
            }

            int panel_no = -1;
            if (sender.GetType().Name == "PictureBox") panel_no = (int)((PictureBox)sender).Tag;
            if (sender.GetType().Name == "Panel") panel_no = (int)((Panel)sender).Tag;

            if (EventPanelMouseClick != null) EventPanelMouseClick(panel_no, e.Button == MouseButtons.Left);
        }

        void VideoPanels_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drag_panel = false;
                start_x = 0;
                start_y = 0;
            }
        }

        void VideoPanels_MouseLeave(object sender, EventArgs e)
        {
            if (hidden_captions)
            {
                Control pb = (Control)sender;
                Point locationOnScreen = pb.Parent.PointToScreen(pb.Location);
                if (!OnForm()) pnBtns.Visible = false;
            }
        }

        bool GetOnTop(Control pb, Point cursor)
        {
            //Координаты окна на экране
            Point location = pb.PointToScreen(Point.Empty);

            //Старый вариант - наведение только на область кнопок
            //if (cursor.X >= location.X + pb.Width - pnBtns.Width - 10 &&
            //    cursor.X <= location.X + pb.Width &&
            //    cursor.Y >= location.Y && cursor.Y <= location.Y + 30)

            //Новый вариант - наведение на любую часть окна
            if (cursor.X >= location.X && cursor.X <= location.X + pb.Width &&
                cursor.Y >= location.Y && cursor.Y <= location.Y + pb.Height)
                return true;

            return false;
        }

        /// <summary>Проверка движения курсора над формой</summary>
        /// <returns>Результат операции</returns>
        bool OnForm()
        {
            Point locationOnScreen = pnBtns.Parent.PointToScreen(pnBtns.Location);
            return (locationOnScreen.X <= Cursor.Position.X &&
                    locationOnScreen.X + pnBtns.Width >= Cursor.Position.X &&
                    locationOnScreen.Y <= Cursor.Position.Y &&
                    locationOnScreen.Y + pnBtns.Height >= Cursor.Position.Y);
        }

        void pnBtns_MouseLeave(object sender, EventArgs e)
        {
            pnBtns.Visible = false;
        }

        #endregion

    }

}
