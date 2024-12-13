using AlfaPribor.JpegCodec;
using AlfaPribor.SharpDXVideoRenderer;
//Внешние проекты
using AlfaPribor.VideoPlayer;
using AlfaPribor.VideoStorage2;
using FramesPlayer.ExportConfiguration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FramesPlayer
{
    public partial class FormMain : Form
    {

        #region Variables

        /// <summary>Объект видеоплеер</summary>
        VideoPlayerInterface obj_VideoPlayer;
        /// <summary>Объект видеохранилище</summary>
        VideoStorage obj_VideoStorage;
        /// <summary>Класс рисования кадров телекамер</summary>
        DrawTelecameraFrames obj_DrawFrames_TK;
        /// <summary>Объект распаковки jpeg-кадров</summary>
        JpegDecoder obj_JpegDecoder;
        /// <summary>Класс рисования кадров тепловизора</summary>
        DrawThermoFrames obj_DrawFrames_TV;

        /// <summary>Состояние изменения размера панели</summary>
        bool ProcessResize;
        /// <summary>Выбранный видеопоток</summary>
        VideoStreamInfo SelectedChannel;
        /// <summary>Тип кодека</summary>
        CodecType codec_type = CodecType.WPF;// CodecType.VCM;
        //CodecType codec_type = CodecType.VCM;

        string FilePath;


        VideoImageExporter _imageExporter;
        ImageSaveResizeSettings _autoSaveSettings;

        #endregion

        public FormMain(string file)
        {
            InitializeComponent();
            _imageExporter = new VideoImageExporter();
            _autoSaveSettings = new ImageSaveResizeSettings();
            CreateVideoPlayer();
            this.Text = "FramesPlayer " + Application.ProductVersion;
            if (file != "") OpenFile(file);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (obj_VideoPlayer != null)
            {
                obj_VideoPlayer.Close();
                obj_VideoPlayer = null;
            }
            if (obj_VideoStorage != null)
            {
                obj_VideoStorage.Active = false;
                obj_VideoStorage = null;
            }
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;
            AdjustPlayerWindow();
        }

        void AdjustPlayerWindow()
        {
            if (obj_VideoPlayer != null && obj_VideoPlayer.Opened)
            {
                Rectangle rect = new Rectangle();
                rect.X = 0;
                rect.Y = 0;
                rect.Width = this.ClientRectangle.Width;
                rect.Height = this.ClientRectangle.Height - panelPlay.Height - statusStrip1.Height -
                              menuStrip1.Height - toolStripMain.Height;

                if (SelectedChannel != null)
                {
                    double ratio = 1.0d * SelectedChannel.Width / SelectedChannel.Height;
                    if (SelectedChannel.Rotation == 90 || SelectedChannel.Rotation == 270) ratio = 1.0d / ratio;
                    pictureBoxVideo.Width = (int)Math.Round(rect.Height * ratio);
                    pictureBoxVideo.Height = rect.Height;
                    pictureBoxVideo.Left = (rect.Width - pictureBoxVideo.Width) / 2;
                }
            }
        }

        /// <summary>Создание видеоплеера</summary>
        void CreateVideoPlayer()
        {
            //Если объект уже есть оставить его
            if (obj_VideoPlayer != null) return;

            //Создать объект видеоплеер
            obj_VideoPlayer = new VideoPlayerInterface();
            obj_VideoPlayer.Owner = panelPlay;
            obj_VideoPlayer.ShowScrollBar = true;
            obj_VideoPlayer.FlowControls = true;
            obj_VideoPlayer.AllowSetSpeed = true;
            obj_VideoPlayer.SetSimpleSpeedScroll = true;
            obj_VideoPlayer.EnableExport = true;
            obj_VideoPlayer.ShowLengthInGroupBox = true;
            obj_VideoPlayer.SimpleLength = true;
            obj_VideoPlayer.CheckExportLength = false;

            //Создание кнопок
            List<VideoPlayerInterface.PlayerButtonInfo[]> buttons = new List<VideoPlayerInterface.PlayerButtonInfo[]>();

            //Кнопки воспроизведения
            VideoPlayerInterface.PlayerButtonInfo[] play_buttons = new VideoPlayerInterface.PlayerButtonInfo[3];
            play_buttons[0] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.PlayBack,  Properties.Resources.play_back);
            play_buttons[1] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.Pause, Properties.Resources.pause32);
            play_buttons[2] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.Play, Properties.Resources.play32);

            //Кнопки перехода по кадрам
            VideoPlayerInterface.PlayerButtonInfo[] frames_buttons = new VideoPlayerInterface.PlayerButtonInfo[2];
            frames_buttons[0] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.PrevFrame, Properties.Resources.cadr_prev32);
            frames_buttons[1] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.NextFrame, Properties.Resources.cadr_next32);

            //Кнопки перехода в начало и конец записи
            VideoPlayerInterface.PlayerButtonInfo[] last_buttons = new VideoPlayerInterface.PlayerButtonInfo[2];
            last_buttons[0] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.FirstFrame, Properties.Resources.begin32);
            last_buttons[1] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.LastFrame, Properties.Resources.end32);

            //Добавление кнопок плееру
            buttons.Add(play_buttons);
            buttons.Add(frames_buttons);
            buttons.Add(last_buttons);

            obj_VideoPlayer.SetButtons(buttons);

            //Кнопка экспорта
            VideoPlayerInterface.PlayerButtonInfo[] export_button = new VideoPlayerInterface.PlayerButtonInfo[3];
            export_button[0] = new VideoPlayerInterface.PlayerButtonInfo
                                  (VideoPlayerInterface.PlayerButtonType.MarkStartExport, Properties.Resources.begin_kadr32);
            export_button[1] = new VideoPlayerInterface.PlayerButtonInfo
                                  (VideoPlayerInterface.PlayerButtonType.MarkStopExport, Properties.Resources.end_kadr32);
            export_button[2] = new VideoPlayerInterface.PlayerButtonInfo
                                  (VideoPlayerInterface.PlayerButtonType.Export, Properties.Resources.exp32);
            obj_VideoPlayer.SetExportButtons(export_button);

            //Создание интерфейса
            obj_VideoPlayer.CreateUI();

            //Подписка на события видеоплеера 
            //Получение кадра
            obj_VideoPlayer.EventNewFrame += new VideoPlayerInterface.DelegateEventImage(obj_VideoPlayer_EventNewFrame);
            //Событие темпа воспроизведения
            obj_VideoPlayer.EventFPS += new VideoPlayerInterface.DelegateEventFPS(obj_VideoPlayer_EventFPS);
            //Событие открытия видео
            obj_VideoPlayer.EventOpenVideo += new VideoPlayerInterface.DelegateEventOpenVideo(obj_VideoPlayer_EventOpenVideo);
            //Событие закрытия видео
            obj_VideoPlayer.EventCloseVideo += new VideoPlayerInterface.DelegateEventCloseVideo(obj_VideoPlayer_EventCloseVideo);
            //Событие измеения размера панели
            obj_VideoPlayer.EventPanelResize += new VideoPlayerInterface.DelegateEventPanelResize(obj_VideoPlayer_EventPanelResize);
            //Событие экспорта
            obj_VideoPlayer.EventExport += new VideoPlayerInterface.DelegateEventExport(obj_VideoPlayer_EventExport);
        }

        /// <summary>Возвращает тип потока с указанным идентификатором</summary>
        /// <param name="id">Идентификатор потока</param>
        /// <returns>Тип изображения потока</returns>
        ImageType GetCameraType(int id)
        {
            if (obj_VideoPlayer == null) return ImageType.Unknown;
            if (obj_VideoPlayer.ObjVideoReader == null) return ImageType.Unknown;
            if (obj_VideoPlayer.ObjVideoReader.VideoIndex == null) return ImageType.Unknown;
            //Чтение инормации о потоках
            IVideoIndex index = obj_VideoPlayer.ObjVideoReader.VideoIndex;
            for (int i = 0; i < index.StreamInfoList.Count; i++)
            {
                if (index.StreamInfoList[i].ContentType == "image/jpeg") return ImageType.Jpeg;
                if (index.StreamInfoList[i].ContentType == "image/raw8") return ImageType.Raw8;
            }
            return ImageType.Unknown;
        }

        #region Video Player Events

        /// <summary>Событие получения кадра от плеера</summary>
        /// <param name="buf">Данные кадра</param>
        /// <param name="timestamp">Метка времени от начала видеопотока</param>
        void obj_VideoPlayer_EventNewFrame(int resX, int resY, int channel_id, byte[] frame, int timestamp)
        {
            //if (this.WindowState == FormWindowState.Minimized) return;

            if (this.WindowState == FormWindowState.Minimized) return;

            if (GetCameraType(channel_id) == ImageType.Unknown) return;

            //Если пришел кадр не от выбранного видеопотока - выход
            if (SelectedChannel.Id != channel_id) return;

            //Отображение кадра телевизионной камеры
            if (SelectedChannel.ContentType == "image/jpeg")
                if (obj_DrawFrames_TK != null && !ProcessResize &&
                    obj_DrawFrames_TK.ChannelID == channel_id && !obj_DrawFrames_TK.InitState)
                    DrawJpegFrame(channel_id, frame, timestamp, resX, resY);

            //Отображение тепловизионного кадра raw8
            if (SelectedChannel.ContentType == "image/raw8")
                if (obj_DrawFrames_TV != null && !ProcessResize &&
                    obj_DrawFrames_TV.ChannelID == channel_id && !obj_DrawFrames_TV.InitState)
                    obj_DrawFrames_TV.NewFrame(frame);

        }

        /// <summary>Событие темпа воспроизведения</summary>
        /// <param name="stream_id">Идентификатор видеоканала</param>
        /// <param name="fps">Текущий темп</param>
        void obj_VideoPlayer_EventFPS(int stream_id, int fps)
        {
            if (GetCameraType(stream_id) == ImageType.Unknown) return;
            if (GetCameraType(stream_id) == ImageType.Jpeg)
            {
                if (this.WindowState == FormWindowState.Normal)
                    if (obj_DrawFrames_TK != null) obj_DrawFrames_TK.SetFPS(fps);
            }
            if (GetCameraType(stream_id) == ImageType.Raw8)
            {
                if (this.WindowState == FormWindowState.Normal)
                    if (obj_DrawFrames_TV != null) obj_DrawFrames_TV.SetFPS(fps);
            }
        }

        /// <summary>Обработка открытия видео</summary>
        /// <param name="info">Информация о видео</param>
        void obj_VideoPlayer_EventOpenVideo(IList<VideoStreamInfo> info)
        {
            //Если в видеопотоке нет видеоканалов
            if (info.Count == 0) return;
            if (obj_JpegDecoder == null)
            {
                obj_JpegDecoder = new JpegDecoder();
                obj_JpegDecoder.OpenVCMDecoder();
            }
            //Выбираем первый видеоканал
            int id = info[0].Id;
            SelectedChannel = info[0];
            for (int i = 0; i < info.Count; i++)
            {
                if (info[i].Id < id) 
                {
                    id = info[i].Id;
                    SelectedChannel = info[i];
                }
            }
            if (obj_DrawFrames_TK == null || (obj_DrawFrames_TK != null &&
                SelectedChannel.Width != obj_DrawFrames_TK.FrameWidth ||
                SelectedChannel.Height != obj_DrawFrames_TK.FrameHeight))
                {
                    //Переинициализация объекта отрисовки
                    CreateDrawFramesTK(SelectedChannel.Width, SelectedChannel.Height, SelectedChannel.Id, false, 0, SelectedChannel.Rotation);
                }
            for (int i = 0; i < info.Count; i++)
                if (info[i].ContentType == "image/raw8")
                {
                    CreateDrawFramesTV(info[i].Width, info[i].Height, info[i].Id, false, 0);
                    break;
                }

            //Кнопки выбора видеопотоков
           // tsmiChannels.Visible = true;
            //tsmiChannels.DropDownItems.Clear();

            //Параметры видеопотока
            //ShowChannelParams();

            //Удаление кнопок выбора видеопотоков с панели
            for (int i = toolStripMain.Items.Count - 1; i > 1; i--)toolStripMain.Items.RemoveAt(i);

            //Кнопки выбора видеопотоков
            toolStripMain.Items.Insert(2, new ToolStripSeparator());
            ToolStripLabel lbl = new ToolStripLabel();
            lbl.Text = "Видеопотоки: ";
            toolStripMain.Items.Insert(2, lbl);

            //Сортировка видеопотоков по индексу
            IEnumerable<VideoStreamInfo> sort_list = info.OrderByDescending(inf => inf.Id);

            foreach (VideoStreamInfo item in sort_list)
            {
                //Кнопка
                ToolStripButton button = new ToolStripButton();
                button.Text = item.Id.ToString();
                button.Font = new Font("Arial", 14, FontStyle.Bold);
                button.Tag = item.Id;
                if (item == SelectedChannel) button.Checked = true;
                button.Click += new EventHandler(button_Click);
                toolStripMain.Items.Insert(3, button);
                //Меню
                ToolStripMenuItem mi = new ToolStripMenuItem();
                string s = "Видеопоток " + (item.Id).ToString();
                if (item.ContentType == "image/raw8") s += " (тепловизор)";
                mi.Text = s;
                mi.CheckOnClick = true;
                mi.Tag = item.Id;
                mi.Click += new EventHandler(mi_Click);
                tsmiChannels.DropDownItems.Insert(0, mi);
            }

            obj_VideoPlayer.Play();
            obj_VideoPlayer.ButtonDown(VideoPlayerInterface.PlayerButtonType.Play);
            SelectChannel(SelectedChannel.Id);
        }

        void ShowChannelParams()
        {
            statusStrip1.Enabled = true;
            tsslResolution.Text = SelectedChannel.Width.ToString() + "x" + SelectedChannel.Height.ToString();
            tsslRotation.Text = SelectedChannel.Rotation + "°";
            tsslStream.Text = SelectedChannel.Id.ToString();
            tsslFormat.Text = SelectedChannel.ContentType;
        }

        /// <summary>Обработка закрытия видео</summary>
        void obj_VideoPlayer_EventCloseVideo()
        {
            CloseVideo();
        }

        void obj_VideoPlayer_EventPanelResize(int width, int height)
        {
            /*            panelPlay.Height = height;
                        panelPlay.Top = this.ClientSize.Height - 
                                        panelPlay.Height - 
                                        toolStripMain.Height;*/
        }

        #endregion

        #region Выбор видеопотока

        /// <summary>Смена видеопотока меню</summary>
        void mi_Click(object sender, EventArgs e)
        {
            SelectChannel(int.Parse(((ToolStripMenuItem)sender).Tag.ToString()));
        }

        /// <summary>Смена видеопотока кнопкой</summary>
        void button_Click(object sender, EventArgs e)
        {
            SelectChannel(int.Parse(((ToolStripButton)sender).Tag.ToString()));
        }

        /// <summary>Смена видеопотока</summary>
        void SelectChannel(int index)
        {
            //Меню
            for (int i = 0; i < tsmiChannels.DropDownItems.Count; i++)
            {
                ((ToolStripMenuItem)tsmiChannels.DropDownItems[i]).Checked =
                    (index == int.Parse(((ToolStripMenuItem)tsmiChannels.DropDownItems[i]).Tag.ToString()));
            }
            //Кнопка
            for (int i = 0; i < toolStripMain.Items.Count; i++)
            {
                if (toolStripMain.Items[i].GetType().Name == "ToolStripButton")
                    if (((ToolStripButton)toolStripMain.Items[i]).Tag != null)
                    {
                        ((ToolStripButton)toolStripMain.Items[i]).Checked =
                            (index == int.Parse(((ToolStripButton)toolStripMain.Items[i]).Tag.ToString()));
                    }
            }
            //Выбор видепотока
            for (int i = 0; i < obj_VideoPlayer.ObjVideoReader.VideoIndex.StreamInfoList.Count; i++)
            {
                if (index == obj_VideoPlayer.ObjVideoReader.VideoIndex.StreamInfoList[i].Id)
                {
                    SelectedChannel = obj_VideoPlayer.ObjVideoReader.VideoIndex.StreamInfoList[i];
                    CreateDrawFramesTK(SelectedChannel.Width, SelectedChannel.Height,
                                       SelectedChannel.Id, false, 0, SelectedChannel.Rotation);
                    ShowChannelParams();
                    //Если на паузе - обновить кадры
                    if (obj_VideoPlayer.Paused) obj_VideoPlayer.RefreshFrames();
                    return;
                }
            }
        }

        #endregion

        #region Открыть/закрыть файл

        /// <summary>Открыть файл по указанному пути</summary>
        /// <param name="file">Путь к файлу (видеопотока или индексного)</param>
        void OpenFile(string file)
        {
            FilePath = file;

            SettingContainer.VideoFileFullName = file;
            SettingContainer.VideoFileName = Path.GetFileNameWithoutExtension(FilePath);
            //Обновить список вагонов
            SettingContainer.RefreshWagonList(SettingContainer.VideoFileName);
            SettingContainer.SelectedWagons = null;
            
            if (obj_VideoStorage != null)
            {
                obj_VideoStorage.Active = false;
                obj_VideoStorage = null;
            }
            if (obj_VideoStorage == null) obj_VideoStorage = new VideoStorage();
            //Открытие видео
            VideoStorageSettings vs_settings = new VideoStorageSettings();
            vs_settings.Partitions.Add(new VideoPartitionSettings(0, true, Path.GetDirectoryName(file), 0));
            //Интервал проверки хранилища циклическим буфером
            vs_settings.CircleBufferCheckInterval = 1800;
            obj_VideoStorage.SetSettings(vs_settings);
            obj_VideoStorage.Active = true;
            //Присвоение плееру объекта хранилища
            obj_VideoPlayer.ObjVideoStorage = obj_VideoStorage;
            if (obj_VideoPlayer.Open(Path.GetFileNameWithoutExtension(file), 0))
            {
                obj_VideoPlayer.Delta = 100;
                obj_VideoPlayer.Position = 0;
                obj_VideoPlayer.ActiveExport = true;
                //Имя файла
                //this.Text = System.IO.Path.GetFileNameWithoutExtension(file) + 
                //    ".frames - " + Application.ProductName;
                pictureBoxVideo.Visible = true;
                //try
                {
                    if (this.Created)
                    {
                        this.Invoke((MethodInvoker)(() =>{this.Text = "FramesPlayer " + Application.ProductVersion + " состав № " + Path.GetFileNameWithoutExtension(file);}));
                    }
                }
                //catch { };
                AdjustPlayerWindow();
                _imageExporter.InitData(file, obj_VideoPlayer.ObjVideoStorage.Info.Partitions[0].RecordCount, _autoSaveSettings);
            }
            else
            {
                this.Text = Application.ProductName;
                MessageBox.Show("Ошибка открытия файла " + Path.GetFileNameWithoutExtension(file), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            /*if (SelectedChannel != null)
            {
                //Установка размера окна по размеру видео при первом открытии видео
                pictureBoxVideo.Left = 0;
                pictureBoxVideo.Width = SelectedChannel.Width;
                pictureBoxVideo.Height = SelectedChannel.Height;
                this.ClientSize = new Size(pictureBoxVideo.Width,
                                           menuStrip1.Height + toolStripMain.Height + pictureBoxVideo.Height + 
                                           panelPlay.Height + statusStrip1.Height + 4);
            }*/
        }
        public string _currentFileName;

        /// <summary>Открытие файла</summary>
        void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Диалог открытия файла
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Файлы видеопотоков (*.frames, *.index)|*.frames; *.index|" + "Все файлы (*.*)|*.*";
            string init_dir = Application.ExecutablePath;
            if (obj_VideoPlayer != null && obj_VideoPlayer.Opened)
            {
                for (int i=0; i<obj_VideoPlayer.ObjVideoStorage.Info.Partitions.Count; i++)
                {
                    if (obj_VideoPlayer.ObjVideoStorage.Info.Partitions[i].Id == 
                        obj_VideoPlayer.ObjVideoReader.PartitionId)
                    {
                        init_dir = obj_VideoPlayer.ObjVideoStorage.Info.Partitions[i].Path;
                        break;
                    }
                }
              
            }
            openFile.InitialDirectory = init_dir;
            openFile.Multiselect = false;

            tmsiTimeMetric.Visible = true;

            if (openFile.ShowDialog() != DialogResult.OK) return;
            if (!File.Exists(openFile.FileName)) return;
            _currentFileName = openFile.FileName;
            OpenFile(openFile.FileName);
        }

        /// <summary>Открытие файла</summary>
        void tsbOpen_Click(object sender, EventArgs e)
        {
            открытьToolStripMenuItem_Click(открытьToolStripMenuItem, EventArgs.Empty);
        }

        /// <summary>Закрытие файла</summary>
        void tsmiClose_Click(object sender, EventArgs e)
        {
            if (obj_VideoPlayer != null)
            {
                obj_VideoPlayer.Close();
            }
            this.Refresh();
            pictureBoxVideo.Visible = false;
        }

        void CloseVideo()
        {
            if (obj_VideoStorage != null)
            {
                obj_VideoStorage.Active = false;
            }            
            if (obj_JpegDecoder != null)
            {
                obj_JpegDecoder.CloseVCMDecoder();
                obj_JpegDecoder = null;
            }
            if (obj_DrawFrames_TK != null)
            {
                obj_DrawFrames_TK.Dispose();
                obj_DrawFrames_TK = null;
            }

            if (obj_DrawFrames_TV != null)
            {
                obj_DrawFrames_TV.Dispose();
                obj_DrawFrames_TV = null;
            }

            //Кнопка выбора телекамер
            tsmiChannels.Visible = false;

            //
            while (toolStripMain.Items.Count > 2) toolStripMain.Items.RemoveAt(2);
        }

        #endregion

        /// <summary>Отрисовка сжатого jpeg кадра</summary>
        /// <param name="channel_id">Идентификатор видеоканала</param>
        /// <param name="frame">Данные кадра</param>
        void DrawJpegFrame(int channel_id, byte[] frame, int timestamp, int resX, int resY)
        {
            byte[] out_data = new byte[resX * resY * 4];//32 бита
            //Декомпрессия кадра
            if (codec_type == CodecType.VCM)
            {
                obj_JpegDecoder.DecodeFrame(frame, resX, resY, out_data);
            }
            if (codec_type == CodecType.WPF)
            {
                obj_JpegDecoder.DecodeWPF(frame, resX, resY, out_data);
                //Переворачивание кадра относительно горизонтельной оси
                byte[] img = new byte[out_data.Length];
                AlfaPribor.IppInterop.IppiSize size;
                size.width = resX;
                size.height = resY;
                AlfaPribor.IppInterop.IppFunctions.ippiMirror_8u_C3R(out_data, resX * 3, img, resX * 3, size, AlfaPribor.IppInterop.IppiAxis.ippAxsHorizontal);
                out_data = img;
            }
            //Отрисовка декодированного кадра
            int ts = 0;
            if (tsmiTimeStamp.Checked) ts = timestamp;
            if (obj_DrawFrames_TK != null) obj_DrawFrames_TK.NewFrame(out_data, ts);
            try { _imageExporter.ExportImage(frame, channel_id, timestamp); } catch { }
        }

        /// <summary>Инициализация объекта отрисовки кадров телекамер</summary>
        /// <param name="resX">Ширина кадра</param>
        /// <param name="resY">Высота кадра</param>
        /// <param name="id">Идентификатор телекамеры</param>
        /// <param name="AniFish">Исправлять искажения</param>
        /// <param name="Coeff">КОэффициент исправления искажений</param>
        void CreateDrawFramesTK(int resX, int resY, int id, bool AniFish, int Coeff, int Rotation)
        {
            Rotation = 0;
            if (obj_DrawFrames_TK == null) obj_DrawFrames_TK = new DrawTelecameraFrames();
            if (obj_DrawFrames_TK.FrameWidth != resX ||
                obj_DrawFrames_TK.FrameHeight != resY)
            {
                obj_DrawFrames_TK.Init(resX, resY, pictureBoxVideo, false, AniFish, Coeff);
            }
            obj_DrawFrames_TK.ChannelID = id;
            if (Rotation == 0) obj_DrawFrames_TK.Rotation = SharpDXVideoRenderer.RotationAngle.deg_0;
            if (Rotation == 90) obj_DrawFrames_TK.Rotation = SharpDXVideoRenderer.RotationAngle.deg_90;
            if (Rotation == 180) obj_DrawFrames_TK.Rotation = SharpDXVideoRenderer.RotationAngle.deg_180;
            if (Rotation == 270) obj_DrawFrames_TK.Rotation = SharpDXVideoRenderer.RotationAngle.deg_270;
            obj_DrawFrames_TK.ShowGraphicElement(GraphicElement.DrawFPS, false);
        }

        /// <summary>Инициализация объекта отрисовки тепловизионных кадров</summary>
        void CreateDrawFramesTV(int resX, int resY, int id, bool AniFish, int Coeff)
        {
            if (obj_DrawFrames_TV == null) obj_DrawFrames_TV = new DrawThermoFrames();
            obj_DrawFrames_TV.Init(resX, resY, pictureBoxVideo, false, AniFish, Coeff);
            obj_DrawFrames_TV.ChannelID = id;
            obj_DrawFrames_TV.SetPalette(Palettes.Gray);
        }

        #region Изменение размера панели

        void pictureBoxVideo_Resize(object sender, EventArgs e)
        {
            ProcessResize = true;
        }

        #endregion

        #region Menu Click

        /// <summary>Регистрация расширений файлов</summary>
        void зарегистрироватьРасширенияФайловToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FileAssociation.Associate(".frames", "FramesPlayer", "Файл видеопотоков", Application.StartupPath + "\\arhiv_mode.ico", Application.ExecutablePath);
                FileAssociation.Associate(".index", "FramesPlayer", "Файл индексов видеопотоков", Application.StartupPath + "\\arhiv_mode.ico", Application.ExecutablePath);
            }
            catch { };
        }

        void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormAbout frm = new FormAbout())
            {
                frm.ShowDialog();
            }
        }
        #endregion

        //*
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MessageHelper.WM_COPYDATA:
                    MessageHelper.COPYDATASTRUCT mystr = new MessageHelper.COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (MessageHelper.COPYDATASTRUCT)m.GetLParam(mytype);
                    OpenFile(mystr.lpData);
                    break;
            }
            try
            {
                base.WndProc(ref m);
            }
            catch { };
        }
        //*/

        #region Export

        /// <summary>Экспорт</summary>
        /// <param name="start">Стартовая метка видео</param>
        /// <param name="stop">Стоповая метка видео</param>
        void obj_VideoPlayer_EventExport(int start, int stop)
        {
            obj_VideoPlayer.Pause();

            int begin = start;
            int end = stop;
            //Не выделено - значит выбран весь поток
            if (begin == 0 && end == 0)
            {
                begin = 0;
                end = obj_VideoPlayer.Length;
            }
            using (FormExport frm = new FormExport(Path.GetFileNameWithoutExtension(FilePath),
                   obj_VideoStorage, begin, end))
            {
                frm.ShowDialog();
            }
        }

        #endregion

        void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        void tmsiTimeMetric_Click(object sender, EventArgs e)
        {
            List<IVideoIndex> indexes = new List<IVideoIndex>();
            indexes.Add(obj_VideoPlayer.ObjVideoReader.VideoIndex);
            //int partitionID = obj_VideoStorage.Info.Partitions[0].Id;
            //for (int i = 0; i < 4; i++)
            //{
            //    try
            //    {

            //        var reader = obj_VideoStorage.GetReader(i.ToString(), partitionID);
            //        VideoFrame frame = null;
            //        reader.ReadFirstFrame(i, out frame);
            //        indexes.Add(reader.VideoIndex);
            //    }
            //    catch { }
            //}
            // ChannelGraphForm cForm = new ChannelGraphForm(indexes, FilePath);
           // cForm.Show();

            FormGraph gForm = new FormGraph(obj_VideoPlayer.ObjVideoReader.VideoIndex);
            gForm.Show();
        }

        void tsmiSpeedMetric_Click(object sender, EventArgs e)
        {
            try
            {
                int trainID = -1;
                if (!int.TryParse(Path.GetFileNameWithoutExtension(_currentFileName), out trainID))
                {
                    MessageBox.Show("Неверное расширение файла");
                    return;
                }
                string speedFileName = Path.GetDirectoryName(_currentFileName) + "\\" + trainID + ".speed";
                if (!File.Exists(speedFileName))
                {
                    MessageBox.Show("Файл со скоростной информацией не обнаружен.");
                    return;
                }
                FormThreadGraph ftg = new FormThreadGraph(speedFileName);
                ftg.ShowDialog();

            }
            catch { }
        }

        void tsmiWagonSpeed_Click(object sender, EventArgs e)
        {

            try
            {
                int trainID = -1;

                if (!int.TryParse(Path.GetFileNameWithoutExtension(_currentFileName), out trainID))
                {
                    MessageBox.Show("Неверное расширение файла");
                    return;
                }
                if (!SettingContainer.WagonDataProvider.CheckConnection())
                {
                    ShowDBConnectionForm();
                }
               //Проверить выбор базы данных
                SettingContainer.RefreshWagonList(_currentFileName);
                //Отобразить форму выбора вагонов
                FormSpeedGraph form = new FormSpeedGraph(obj_VideoPlayer.ObjVideoReader.VideoIndex);
                form.Show();
            }
            catch { }
        }

        static void ShowDBConnectionForm()
        {
            FormConnectionDB selectConnectionForm = new FormConnectionDB();
            selectConnectionForm.ShowDialog();
        }

        void tsmiDBConnect_Click(object sender, EventArgs e)
        {
            ShowDBConnectionForm();
        }

        string _folderName = string.Empty;

        void tsmiSaveFrame_Click(object sender, EventArgs e)
        {
            SaveCurrentFrame();
        }

        void SaveCurrentFrame()
        {
            try
            {
                string fileName = string.Empty;
                if (!string.IsNullOrEmpty(_folderName))
                {
                    try
                    {
                        int fileCount = Directory.GetFiles(_folderName).Count()+1;
                        fileName = _folderName + string.Format("{0}.jpeg", fileCount);
                    }
                    catch { fileName = string.Empty; }
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    using (SaveFileDialog dialog = new SaveFileDialog() )
                    {
                        dialog.Filter = "jpeg(*.jpeg) | *.jpeg";
                        dialog.FilterIndex = 0;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                _folderName = Path.GetFullPath(Path.GetDirectoryName(dialog.FileName));
                                if (!_folderName.EndsWith("\\"))
                                    _folderName += "\\";

                            }
                            catch { }
                            fileName = dialog.FileName;
                        }
                    }
                }

                obj_DrawFrames_TK.BitmapImage.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                MessageBox.Show("Изображение сохранено");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения изображения");
            }
        }

        private void tsmiTimeStamp_Click(object sender, EventArgs e)
        {
            try
            {
                List<IVideoIndex> indexes = new List<IVideoIndex>();
                indexes.Add(obj_VideoPlayer.ObjVideoReader.VideoIndex);
                //int partitionID = obj_VideoStorage.Info.Partitions[0].Id;
                //for (int i = 0; i < 4; i++)
                //{
                //    try
                //    {

                //        var reader = obj_VideoStorage.GetReader(i.ToString(), partitionID);
                //        VideoFrame frame = null;
                //        reader.ReadFirstFrame(i, out frame);
                //        indexes.Add(reader.VideoIndex);
                //    }
                //    catch { }
                //}
                // ChannelGraphForm cForm = new ChannelGraphForm(indexes, FilePath);
                // cForm.Show();

                FormGraph gForm = new FormGraph(obj_VideoPlayer.ObjVideoReader.VideoIndex);
                gForm.Show();
            }
            catch {
                MessageBox.Show("Откройте файл с расширением *.frames или *.index");
            }
        }

        private void autoSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormAutoExportSettings form = new FormAutoExportSettings())
            {
                form.AutoExportSettingsChanged += AutoExportSettingsChangedHandler;
                form.ShowDialog();
            }
        }

        private void AutoExportSettingsChangedHandler(object sender, AutoExprortChangeSettingsEventArgs e)
        {
            _autoSaveSettings = e.AutoExportSettings;
        }

        private async void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string serchingDir = "E:\\ImageArchive\\Exampels\\18";

            FileEdit fileEdit = new FileEdit(); 
            var list = fileEdit.SearchFiles(serchingDir);
            var FileList = list.Select(f => f.FullName).ToList();

            FileInfo[] fileList = fileEdit.SearchFiles(serchingDir);
            if (fileList.Length == 0) return;
           
            Task<byte[]>[] byteArray = fileList.Select(async x => await ReadAsync(x.FullName)).ToArray();
            var sfsd = byteArray[1];

            Bitmap[] dataArray = fileList.Select(x => { return new Bitmap(x.FullName); }).ToArray();
            dataArray[0].Save("asd");
            //DrawJpegFrame(int channel_id, byte[] frame, int timestamp, int resX, int resY);
        }


        async Task<byte[]> ReadAsync(string path)
        {
            using (FileStream sourceStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                byte[] buffer = new byte[sourceStream.Length];
                //int numRead;
                //StringBuilder sb = new StringBuilder();
                
                int numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length);

                //while (() != 0)
                //{
                //    string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                //    sb.Append(text);
                //}
                return buffer;
            }
        }

        private void saveFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentFrame();
        }
    }

}