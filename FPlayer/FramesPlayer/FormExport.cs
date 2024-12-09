using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

using AlfaPribor.VideoExport;
using AlfaPribor.JpegCodec;
using AlfaPribor.AviFile;
using AlfaPribor.IppInterop;
using AlfaPribor.ASKIN.Data;
using FramesPlayer.ExportConfiguration;
using FramesPlayer.DataTypes;
using AlfaPribor.VideoStorage2;            //Импорт функций ipp2010

namespace FramesPlayer
{
    public partial class FormExport : Form
    {

        enum ValueType { Value = 0, Minimum = 1, Maximum = 2 };

        public enum ExportState
        {
            Start = 0,
            InProcess=1,
            Stoped = 2,
            Pause = 3,
            End = 4,
            Error=5,
            SkipCadr=6
        };

        public enum ExportType
        {
            ExportByStream,
            ExportByWagon
        };

        class BitmapFrameData
        {
            public Bitmap bmp;
            public int channel_id;

            public BitmapFrameData(Bitmap bitmap, int id)
            {
                bmp = bitmap;
                channel_id = id;
            }

            public void Clear()
            {
                //bmp = null;
                bmp.Dispose();
                channel_id = -1;
            }
        }

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

        /// <summary>Параметры сжатия AVI</summary>
        Avi.COMPVARS compressSettings;
        /// <summary>Идентификатор видеозаписи</summary>
        string id_video;
        /// <summary>Видеохранилище</summary>
        IVideoStorage obj_VideoStorage;
        /// <summary>Стартовая метка</summary>
        int StartExport;
        /// <summary>Стоповая метка</summary>
        int StopExport;
        /// <summary>Объекты декодера</summary>
        JpegDecoder decoder;
        /// <summary>Тип кодека</summary>
        CodecType codec_type = CodecType.WPF;// CodecType.VCM;
        /// <summary>Ридер видео</summary>
        IVideoReader reader;
        /// <summary>Массив закешированных изображений готовых для отрисовки или экспорта</summary>
        List<BitmapFrameData> bitmaps;
        /// <summary>Флаг процесса экспорта</summary>
        bool process = false;
        /// <summary>Статус паузы</summary>
        bool pause = false;
        /// <summary>Флаг остановки</summary>
        bool stop = false;
        /// <summary>Флаг закрытия формы</summary>
        bool close = false;

        #endregion

        #region Const

        /// <summary>Код сигнатуры VIDC</summary>
        const int vidc_type = 0x63646976;
        /// <summary>Код сигнатуры mjpg</summary>
        const int mjpg = 0x67706A6D;

        #endregion

        #region Delegates

        delegate uint GetComboBoxSelectedValueCallback(ComboBox cb);
        delegate void ControlEnabledCallback(Control b, bool value);
        delegate void ToolStripButtonEnabledCallback(ToolStripButton b, bool value);
        delegate string GetTextBoxTextCallback(TextBox tb);
        delegate decimal GetNumericUpDownValueCallback(NumericUpDown nud);
        delegate void SetProgessBarValueCallback(ProgressBar pb, int value, ValueType v_type);
        delegate bool GetCheckBoxValueCallback(CheckBox cb);
        delegate void CloseFormCallback(Form form);
        delegate void SetFormCaptionCallback(Form form, string caption);
        delegate void SetGroupBoxCaptionCallback(GroupBox gb, string caption);

        #endregion

        #region Form


        /// <summary>Конструктор формы</summary>
        /// <param name="VideoId">Идентификатор видеозаписи</param>
        /// <param name="VideoStorage">Видеохранилище</param>
        /// <param name="StartMS">Стартовый маркер экспорта в мс</param>
        /// <param name="StopMS">Стоповый маркер экспорта в мс</param>
        public FormExport(string VideoId, IVideoStorage VideoStorage, int StartMS, int StopMS)
        {
            InitializeComponent();
            compressSettings = new Avi.COMPVARS();
            id_video = VideoId;
            obj_VideoStorage = VideoStorage;
            StartExport = StartMS;
            StopExport = StopMS;
        }

        void FormExport_Load(object sender, EventArgs e)
        {
            //Получение ридера
            reader = obj_VideoStorage.GetReader(id_video);
            //Сортировка видеопотоков
            IEnumerable<VideoStreamInfo> sortedEnum = reader.VideoIndex.StreamInfoList.OrderBy(f => f.Id);
            IList<VideoStreamInfo> sortedList = sortedEnum.ToList();

            //Декодеры кадров
            decoder = new JpegDecoder();

            for (int i = 0; i < sortedList.Count; i++)
            {
                //Виртуальная панель для отрисовки
                PictureBox pb = new PictureBox();
                pb.Width = sortedList[i].Width;
                pb.Height = sortedList[i].Height;
            }

            //Jpeg декодер
            decoder = new JpegDecoder();
            decoder.OpenVCMDecoder();

            //Буферы кадров
            bitmaps = new List<BitmapFrameData>();

            cbCompress_CheckedChanged(cbCompress, EventArgs.Empty);
            cbInstallSystemType.SelectedIndex = (int)SettingContainer.DatabaseProviderType;
        }

        void FormExport_Shown(object sender, EventArgs e)
        {
            //Список кодеков
            ShowCodecsList(0);
            //Путь экспорта
            fbdpath.SelectedPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            tbExportPath.Text = fbdpath.SelectedPath;

            if (obj_VideoStorage != null)
            {
                //Список видеопотоков в записи
                IList<VideoStreamInfo> list = reader.VideoIndex.StreamInfoList;

                //Сортировка видеопотоков
                IEnumerable<VideoStreamInfo> sortedEnum = list.OrderBy(f => f.Id);
                IList<VideoStreamInfo> sortedList = sortedEnum.ToList();

                //Вывод списка
                for (int i = 0; i < sortedList.Count; i++)
                {
                    //Формирование массива данных для строки грида
                    object[] values = 
                        new object[] { true, 
                                       sortedList[i].Id, 
                                       sortedList[i].Resolution,
                                       sortedList[i].Id.ToString() + ".avi" };

                    //Добавление строки в грид
                    dgvTelecameras.Rows.Add(values);
                }

            }
            SetWagonExportState();
        }

        void FormExport_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (reader != null) reader.Close();
        }

        void FormExport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (process && !close) e.Cancel = true;
            if (process)
            {
                if (process) tsbStop_Click(tsbStop, EventArgs.Empty);
                else Close();

                if (stop)
                {
                    close = true;
                    Close();
                }
            }
        }

        #endregion

        #region Настройка кодека

        void ShowCodecsListThread(object selectedItem)
        {
            try
            {
                uint selected = (uint)selectedItem;
                List<Avi.ICINFO> list = Avi.GetCodecs();
                DataTable codecs = new DataTable();
                codecs.Columns.Add("handler", typeof(UInt32));
                codecs.Columns.Add("name", typeof(String));
                for (int i = 0; i < list.Count; i++)
                codecs.Rows.Add(new object[] { (uint)list[i].fccHandler, list[i].szDescription });
                cbCodecs.BeginInvoke((Action)(() =>
                {
                    cbCodecs.DataSource = codecs;
                    if (selected != 0) cbCodecs.SelectedValue = selected;
                }));
            }
            catch { }
        }

        /// <summary>Вывод списка доступных кодеков</summary>
        /// <param name="selected">Дескриптор выбранного кодека</param>
        void ShowCodecsList(uint selected)
        {
            Thread thread = new Thread(ShowCodecsListThread);
            thread.Start(selected);
            //List<Avi.ICINFO> list = Avi.GetCodecs();
            //DataTable codecs = new DataTable();
            //codecs.Columns.Add("handler", typeof(UInt32));
            //codecs.Columns.Add("name", typeof(String));
            //for (int i = 0; i < list.Count; i++)
            //    codecs.Rows.Add(new object[] { (uint)list[i].fccHandler, list[i].szDescription });
            //cbCodecs.DataSource = codecs;
            //if (selected != 0) cbCodecs.SelectedValue = selected;
        }

        /// <summary>Изменение выбранного кодека</summary>
        void cbCodecs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCodecs.SelectedValue != null)
            {
                uint fccHandler = (uint)cbCodecs.SelectedValue;
                btnCodecOptions.Enabled = Avi.IsConfig(fccHandler);
            }
        }

        /// <summary>Кнопка настройки параметров кодека</summary>
        void btnCodecOptions_Click(object sender, EventArgs e)
        {
            uint fccHandler = (uint)cbCodecs.SelectedValue;
            Avi.ConfigCodec(fccHandler);
        }

        /// <summary>Сжатие видео</summary>
        void cbCompress_CheckedChanged(object sender, EventArgs e)
        {
            gbVideo.Enabled = cbCompress.Checked;
        }

        #endregion

        #region Управление экспортом

        /// <summary>Остановка экспорта</summary>
        void tsbStop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Остановить процесс экспорта?", "Экпорт видео",
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                stop = true;
            }
        }

        /// <summary>Пауза/возобновление экспорта</summary>
        void tsbPause_Click(object sender, EventArgs e)
        {
            pause = !pause;
            if (pause) tsbPause.Text = "Продолжить";
            else tsbPause.Text = "Пауза";
        }

        #endregion

        /// <summary>Выбор пути к файлу</summary>
        void btnSelectPath_Click(object sender, EventArgs e)
        {
            //if (fbdpath.ShowDialog() == DialogResult.OK) 
            //    tbExportPath.Text = fbdpath.SelectedPath;
            FolderBrowserDialogEx dialog = new FolderBrowserDialogEx();
            dialog.ShowFullPathInEditBox = true;
            if (dialog.ShowDialog() == DialogResult.OK)
                tbExportPath.Text = dialog.SelectedPath;
        }

        #region Процесс экспорта

        /// <summary>Старт экспорта</summary>
        void tsbStart_Click(object sender, EventArgs e)
        {
            if (process) return;
            //Проверка выбора длительности или вагонов
            if (cbSelectWagonOption.Checked && SettingContainer.SelectedWagons.Count == 0)
            {
                MessageBox.Show("Не указаны вагоны для экспорта");
                return;
            }
            if (!cbSelectWagonOption.Checked && (StartExport == StopExport))
            {
                MessageBox.Show("Не указан диапазон видео");
                return;
            }
            //Запуск потока открытия видео
            Thread workThread = new Thread(new ThreadStart(ExportThread));
            workThread.SetApartmentState(ApartmentState.STA);
            workThread.Priority = ThreadPriority.BelowNormal;
            workThread.Start();
        }
        
        /// <summary>Поток экспорта</summary>
        void ExportThread()
        {
            if (reader == null) return;

            //Сортировка видеопотоков
            IEnumerable<VideoStreamInfo> sortedEnum = reader.VideoIndex.StreamInfoList.OrderBy(f => f.Id);
            IList<VideoStreamInfo> sortedList = sortedEnum.ToList();

            bool exportWagons = SettingContainer.SelectedWagons.Count > 0;
            //Инициализация бибилотек Avi - если видео пишется без пережатия
            if (!cbCompress.Checked) Avi.AVIFileInit();
            
            for (int i = 0; i < dgvTelecameras.RowCount; i++)
            {
                if ((bool)dgvTelecameras.Rows[i].Cells[0].Value)
                {
                    if (!exportWagons)
                    {
                        sortedList = ExportVideoStream(sortedList, i);
                    }
                    else
                    {
                        ExportVideoStreamByWagon(sortedList, i);
                    }
                }
            }
            if (exportWagons)
            {
                ExportExportWagonResultInfo();
            }

            SetEndExportControlState();
        }

        /// <summary>Поток экспорта</summary>
        void ExportToFramesThread()
        {
            if (reader == null) return;

            //Сортировка видеопотоков
            IEnumerable<VideoStreamInfo> sortedEnum = reader.VideoIndex.StreamInfoList.OrderBy(f => f.Id);
            IList<VideoStreamInfo> sortedList = sortedEnum.ToList();

            bool exportWagons = SettingContainer.SelectedWagons.Count > 0;
            //Инициализация бибилотек Avi - если видео пишется без пережатия
            if (!cbCompress.Checked) Avi.AVIFileInit();

            for (int i = 0; i < dgvTelecameras.RowCount; i++)
            {
                if ((bool)dgvTelecameras.Rows[i].Cells[0].Value)
                {
                    if (!exportWagons)
                    {
                        sortedList = ExportVideoStream(sortedList, i);
                    }
                    else
                    {
                        ExportVideoStreamByWagon(sortedList, i);
                    }
                }
            }
            if (exportWagons)
            {
                ExportExportWagonResultInfo();
            }

            SetEndExportControlState();
        }

        void ExportExportWagonResultInfo()
        {
            string textFileName = NamingUtilits.GetExportTextFileName(
                                                      GetTextBoxTextThreadSafe(tbExportPath),
                                                      SettingContainer.VideoFileName
                                                      );
            CreateFileByName(textFileName);
            CreateCatalogIfNotExist(textFileName);
            using (WagonExporter wagonExporter = new WagonExporter(textFileName))
            {
                wagonExporter.WriteHeaderRow();
                foreach (WagonDataShort wagonInfo in SettingContainer.SelectedWagons)
                {
                    wagonExporter.WriteWagonDataRow(wagonInfo);
                }
            }
        }

        IList<VideoStreamInfo> ExportVideoStream(IList<VideoStreamInfo> sortedList, int i)
        {
            //Создание файла для видеопотока
            string filepath = GetTextBoxTextThreadSafe(tbExportPath) + "\\" + dgvTelecameras.Rows[i].Cells[3].Value.ToString();
            //Сохдание файла
            CreateFileByName(filepath);
            //Создание каталога если его нет
            CreateCatalogIfNotExist(filepath);
            //Размеры кадра
            int FrameWidth = sortedList[i].Width;
            int FrameHeight = sortedList[i].Height;
            //Расчет скорости кадров
            double fps = CalcFps(i);
            int step = (int)Math.Round(1.0d * 1000 / fps);

            //Класс записи в AVI со сжатием
            VideoExportToAvi ExportToAVI = null;

            //Указатель на интерфейса открытого AVI-файла
            IntPtr aviFile = IntPtr.Zero;
            //Указатель потока в Avi
            IntPtr newAviStream = IntPtr.Zero;
            //Индекс потока в Avi
            uint StreamIndex;
            //Позиция сэмпла
            int Pos = 0;

            //Сжатое видео
            if (cbCompress.Checked)
            {
                //Объект экспорта
                ExportToAVI = InitExportAviObject(filepath, FrameWidth, FrameHeight, fps, ExportToAVI);
            }
            //Не сжатое видео
            else
            {
                //Открытие файла
                StreamIndex = InitUncompressedVideoParams(filepath, FrameWidth, FrameHeight, fps, ref aviFile, ref newAviStream);
            }

            //Инициализация прогресс-бара
            SetProgressBarStartState(StartExport, StopExport);

            //Сканирование видеопотока
            for (int timestamp = StartExport; timestamp < StopExport; timestamp += step)
            {

                #region Pause/Stop
                if (CheckStartStopState() == ExportState.Stoped)
                {
                    break;
                }
                #endregion

                ExportState state = ExportFrame(ref sortedList, ref ExportToAVI, newAviStream, i, timestamp, ref Pos);
                if (state == ExportState.SkipCadr) continue;
                if (state == ExportState.Error)
                {
                    MessageBox.Show("Ошибка экспорта кадра", "Экспорт видео",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }

                //Прогрессбар
                int percent = (int)(100 * (1.0f * (timestamp - StartExport) / (StopExport - StartExport)));
                UpdateProgressGUIState(filepath, timestamp, percent);
            }
            CloseAVIStreams(ExportToAVI, aviFile, newAviStream);
            return sortedList;
        }

        IList<VideoStreamInfo> ExportVideoStreamByWagon(IList<VideoStreamInfo> sortedList, int streamIndex)
        {
            foreach (WagonDataShort wagon in SettingContainer.SelectedWagons)
            {
                //Создание файла для видеопотока
                string trainFileName = SettingContainer.VideoFileName;
               // string filepath = NamingUtilits.GetWagonFileName(wagon.SnSost, streamIndex, GetTextBoxTextThreadSafe(tbExportPath), trainFileName);
                string filepath = NamingUtilits.GetWagonFileName(wagon.Sn, streamIndex, GetTextBoxTextThreadSafe(tbExportPath), trainFileName);
                // Создать файл для экспорта видео
                CreateFileByName(filepath);
                //Создание каталога если его нет
                CreateCatalogIfNotExist(filepath);
                 //Размеры кадра
                int FrameWidth = sortedList[streamIndex].Width;
                int FrameHeight = sortedList[streamIndex].Height;
                //Расчет скорости кадров
                double fps = CalcFps(streamIndex);
                int step = (int)Math.Round(1.0d * 1000 / fps);

                //Класс записи в AVI со сжатием
                VideoExportToAvi ExportToAVI = null;

                //Указатель на интерфейса открытого AVI-файла
                IntPtr aviFile = IntPtr.Zero;
                //Указатель потока в Avi
                IntPtr newAviStream = IntPtr.Zero;
                //Индекс потока в Avi
                uint StreamIndex;
                //Позиция сэмпла
                int Pos = 0;

                //Сжатое видео
                if (cbCompress.Checked)
                {
                    //Объект экспорта
                    ExportToAVI = InitExportAviObject(filepath, FrameWidth, FrameHeight, fps, ExportToAVI);
                }
                //Не сжатое видео
                else
                {
                    //Открытие файла
                    StreamIndex = InitUncompressedVideoParams(filepath, FrameWidth, FrameHeight, fps, ref aviFile, ref newAviStream);
                    Thread.Sleep(100);
                    // Thread.Sleep(250);
                }
                int startExportTime = wagon.TimeSpanBegin;
                int endExportTime = wagon.TimeSpan;

                //Инициализация прогресс-бара
                SetProgressBarStartState(startExportTime, endExportTime);

                //Сканирование видеопотока
                for (int timestamp = startExportTime; timestamp < endExportTime; timestamp += step)
                {

                    #region Pause/Stop
                    if (CheckStartStopState() == ExportState.Stoped)
                    {
                        break;
                    }
                    #endregion

                    ExportState state = ExportFrame(ref sortedList, ref ExportToAVI, newAviStream, streamIndex, timestamp, ref Pos);
                    if (state == ExportState.SkipCadr) continue;
                    if (state == ExportState.Error)
                    {
                        MessageBox.Show("Ошибка экспорта кадра", "Экспорт видео",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    //Прогрессбар
                    int percent = (int)(100 * (1.0f * (timestamp - startExportTime) / (endExportTime - startExportTime)));
                    UpdateProgressGUIState(filepath, timestamp, percent);
                }
                CloseAVIStreams(ExportToAVI, aviFile, newAviStream);
            }
                return sortedList;
        }

        IList<VideoStreamInfo> ExportVideoStreamByWagonToFrames(IList<VideoStreamInfo> sortedList, int streamIndex)
        {
            foreach (WagonDataShort wagon in SettingContainer.SelectedWagons)
            {
                //Создание файла для видеопотока
                string trainFileName = SettingContainer.VideoFileName;
                string filepath = NamingUtilits.GetWagonFileName(wagon.SnSost, streamIndex, GetTextBoxTextThreadSafe(tbExportPath), trainFileName);
                // Создать файл для экспорта видео
                CreateFileByName(filepath);
                //Создание каталога если его нет
                CreateCatalogIfNotExist(filepath);
                //Размеры кадра
                int FrameWidth = sortedList[streamIndex].Width;
                int FrameHeight = sortedList[streamIndex].Height;
                //Расчет скорости кадров
                double fps = CalcFps(streamIndex);
                int step = (int)Math.Round(1.0d * 1000 / fps);

                VideoStorage videoStorrage = new VideoStorage();


                //Класс записи в AVI со сжатием
                VideoExportToAvi ExportToAVI = null;

                //Указатель на интерфейса открытого AVI-файла
                IntPtr aviFile = IntPtr.Zero;
                //Указатель потока в Avi
                IntPtr newAviStream = IntPtr.Zero;
                //Индекс потока в Avi
                uint StreamIndex;
                //Позиция сэмпла
                int Pos = 0;

                //Сжатое видео
                if (cbCompress.Checked)
                {
                    //Объект экспорта
                    ExportToAVI = InitExportAviObject(filepath, FrameWidth, FrameHeight, fps, ExportToAVI);
                }
                //Не сжатое видео
                else
                {
                    //Открытие файла
                    StreamIndex = InitUncompressedVideoParams(filepath, FrameWidth, FrameHeight, fps, ref aviFile, ref newAviStream);
                }
                int startExportTime = wagon.TimeSpanBegin;
                int endExportTime = wagon.TimeSpan;

                //Инициализация прогресс-бара
                SetProgressBarStartState(startExportTime, endExportTime);

                //Сканирование видеопотока
                for (int timestamp = startExportTime; timestamp < endExportTime; timestamp += step)
                {

                    #region Pause/Stop
                    if (CheckStartStopState() == ExportState.Stoped)
                    {
                        break;
                    }
                    #endregion

                    ExportState state = ExportFrame(ref sortedList, ref ExportToAVI, newAviStream, streamIndex, timestamp, ref Pos);
                    if (state == ExportState.SkipCadr) continue;
                    if (state == ExportState.Error)
                    {
                        MessageBox.Show("Ошибка экспорта кадра", "Экспорт видео",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    //Прогрессбар
                    int percent = (int)(100 * (1.0f * (timestamp - startExportTime) / (endExportTime - startExportTime)));
                    UpdateProgressGUIState(filepath, timestamp, percent);
                }
                CloseAVIStreams(ExportToAVI, aviFile, newAviStream);
            }
            return sortedList;
        }

        ExportState ExportFrame(ref IList<VideoStreamInfo> sortedList, ref VideoExportToAvi ExportToAVI, IntPtr newAviStream,  int streamIndex, int timestamp, ref int Pos)
        {
            #region Формирование кадра

            try
            {
                VideoStreamInfo vs = sortedList[streamIndex];
                if (vs == null) return ExportState.SkipCadr;
                //Получение кадра
                Bitmap bmp = GetBitmap(timestamp, vs);
                if (bmp == null) return ExportState.SkipCadr;
                //Поместить кадр в соответсвующий регион
                //Сжатое видео
                if (cbCompress.Checked) ((ImageRegion)ExportToAVI.Regions[0]).Image = bmp;
                //Не сжатое видео
                else
                {

                }
            }
            catch { };

            #endregion

            //Создание кадра
            try
            {
                if (cbCompress.Checked)
                {
                    ExportToAVI.MakeFrame();
                    ExportToAVI.SaveFrame();
                }
                else
                {
                    VideoFrame frame;
                    if (reader.ReadFrame(sortedList[streamIndex].Id, timestamp, out frame) == VideoStorageResult.Ok)
                    {
                        int writeBytes = 0;
                        int writeSamples = 0;
                        IntPtr unmanagedPointer = Marshal.AllocHGlobal(frame.FrameData.Length);
                        Marshal.Copy(frame.FrameData, 0, unmanagedPointer, frame.FrameData.Length);
                        uint res = Avi.AVIStreamWrite(newAviStream, Pos, 1, unmanagedPointer,
                                                      frame.FrameData.Length, Avi.AVIIF_KEYFRAME,
                                                      ref writeSamples, ref writeBytes);
                        //CheckAviFileResult(res);
                        Marshal.FreeHGlobal(unmanagedPointer);
                        Pos++;
                    }
                }
            }
            catch
            {
                return ExportState.Error;
            }
            return ExportState.InProcess;
        }
        
        void CloseAVIStreams(VideoExportToAvi ExportToAVI, IntPtr aviFile, IntPtr newAviStream)
        {
            if (cbCompress.Checked) ExportToAVI.Close();
            else
            {
                Avi.AVIStreamRelease(newAviStream);
                Avi.AVIFileRelease(aviFile);
            };
        }

        void UpdateProgressGUIState(string filepath, int timestamp, int percentProgress)
        {
            SetProgessBarValueThreadSafe(progressMain, timestamp, ValueType.Value);
           // int percent = (int)(100 * (1.0f * (timestamp - StartExport) / (StopExport - StartExport)));
            //SetGroupBoxCaptionThreadSafe(gbProgress, "Экспорт в файл " + Path.GetFileName(filepath) + ": " + percent.ToString() + "%");
            //gbProgress.Text = "Экспорт в файл " + Path.GetFileName(filepath) + ": " + percent.ToString() + "%";
            string s = "Экспорт в файл \"" + Path.GetFileName(filepath) + "\": " + percentProgress.ToString() + "%";
            SetFormCaptionThreadSafe(this, s);
        }

        ExportState CheckStartStopState()
        {
            //Стоп
            if (stop)
            {
                SetProgessBarValueThreadSafe(progressMain, 0, ValueType.Value);
                return ExportState.Stoped;
            }
            //Пауза
            if (pause)
            {
                while (pause)
                {
                    System.Threading.Thread.Sleep(1);
                    if (stop) break;
                }
            }
            return ExportState.InProcess;
        }

        void SetEndExportControlState()
        {
            process = false;//Состояние процесса экспорта

            SetProgessBarValueThreadSafe(progressMain, 0, ValueType.Minimum);
            SetProgessBarValueThreadSafe(progressMain, 100, ValueType.Maximum);
            SetProgessBarValueThreadSafe(progressMain, 0, ValueType.Value);

            ToolStripButtonEnabledThreadSafe(tsbStart, true);
            ToolStripButtonEnabledThreadSafe(tsbStop, false);
            ToolStripButtonEnabledThreadSafe(tsbPause, false);

            ControlEnabledThreadSafe(gbTK, true);
            ControlEnabledThreadSafe(dgvTelecameras, true);
            ControlEnabledThreadSafe(gbVideo, true);
            ControlEnabledThreadSafe(gbPath, true);

            SetFormCaptionThreadSafe(this, "Экспорт видео");

            MessageBox.Show("Процесс экспорта завершен");

            stop = false;
        }

        static uint InitUncompressedVideoParams(string filepath, int FrameWidth, int FrameHeight, double fps, ref IntPtr aviFile, ref IntPtr newAviStream)
        {
            uint StreamIndex;
            Avi.AVIFileOpen(ref aviFile, filepath, Avi.OF_CREATE, 0);
            Avi.AVISTREAMINFO streamInfo = new Avi.AVISTREAMINFO();
            streamInfo.fccType = Avi.StreamtypeVIDEO;
            streamInfo.fccHandler = mjpg;
            streamInfo.dwScale = 10000U;
            streamInfo.dwRate = (uint)(fps * 10000.0D);
            streamInfo.rcFrame = new Avi.RECT() { left = 0, top = 0, right = (uint)FrameWidth, bottom = (uint)FrameHeight };
            streamInfo.dwQuality = 10000;
            //Заголовок bitmap
            Avi.BITMAPINFOHEADER bmiHeader = new Avi.BITMAPINFOHEADER();
            bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(Avi.BITMAPINFOHEADER));
            bmiHeader.biWidth = FrameWidth;
            bmiHeader.biHeight = FrameHeight;
            bmiHeader.biPlanes = 1;
            bmiHeader.biXPelsPerMeter = 0;
            bmiHeader.biYPelsPerMeter = 0;
            bmiHeader.biCompression = mjpg;
            bmiHeader.biBitCount = 24;
            StreamIndex = Avi.AVIFileCreateStream(aviFile, out newAviStream, ref streamInfo);
            //Установить формат потока
            int bmi_size = Marshal.SizeOf(typeof(Avi.BITMAPINFOHEADER));
            IntPtr bmi_ptr = Marshal.AllocHGlobal(bmi_size);
            Marshal.StructureToPtr(bmiHeader, bmi_ptr, false);
            Avi.AVIStreamSetFormat(newAviStream, 0, bmi_ptr, bmi_size);
            Marshal.FreeHGlobal(bmi_ptr);
            return StreamIndex;
        }

        VideoExportToAvi InitExportAviObject(string filepath, int FrameWidth, int FrameHeight, double fps, VideoExportToAvi ExportToAVI)
        {
            ExportToAVI = new VideoExportToAvi(fps, new RectangleF(0.0F, 0.0F, FrameWidth, FrameHeight));
            //Параметры сжатия AVI
            Avi.AVICOMPRESSOPTIONS opts = new Avi.AVICOMPRESSOPTIONS();
            opts.fccType = vidc_type;
            opts.fccHandler = (uint)GetComboBoxSelectedValueThreadSafe(cbCodecs);
            opts.dwQuality = 10000;
            //Создание файла
            ExportToAVI.Open(filepath, Avi.OF_CREATE, opts);
            //Определение области рисования очередного кадра
            Rectangle rect = new Rectangle(0, 0, FrameWidth, FrameHeight);
            //Добавление региона
            ImageRegion reg = new ImageRegion(new Region(rect));
            reg.SizeMode = PictureBoxSizeMode.Normal;
            ExportToAVI.Regions.Add(reg);
            return ExportToAVI;
        }

        double CalcFps(int i)
        {
            double fps = (reader.VideoIndex.GetFramesCount(i) * 1000.0d) /
                         (reader.VideoIndex.RecordFinished -
                          reader.VideoIndex.RecordStarted).TotalMilliseconds;
            return fps;
        }

        void SetProgressBarStartState(int startExport, int endExport)
        {
            SetProgessBarValueThreadSafe(progressMain, startExport, ValueType.Minimum);
            SetProgessBarValueThreadSafe(progressMain, endExport, ValueType.Maximum);

            //Скрытие кнопок
            ToolStripButtonEnabledThreadSafe(tsbStop, true);
            ToolStripButtonEnabledThreadSafe(tsbPause, true);
            ToolStripButtonEnabledThreadSafe(tsbStart, false);

            ControlEnabledThreadSafe(gbTK, false);

            ControlEnabledThreadSafe(dgvTelecameras, false);
            ControlEnabledThreadSafe(gbVideo, false);
            ControlEnabledThreadSafe(gbPath, false);

            process = true;//Состояние процесса экспорта
        }

        static void CreateCatalogIfNotExist(string filepath)
        {
            string path = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        static void CreateFileByName(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                if (MessageBox.Show("Файл \"" + filepath + "\" уже существует, перезаписать?", "Экспорт видео",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    File.Delete(filepath);
            }
        }

        /// <summary>Получение готовой картинки кадра</summary>
        /// <param name="timestamp">Временная метка</param>
        /// <returns></returns>
        Bitmap GetBitmap(int timestamp, VideoStreamInfo vs)
        {
            VideoFrame frame = null;
            if (reader.ReadFrame(vs.Id, timestamp, out frame) == VideoStorageResult.Ok)
            {
                //Буфер декодированного кадра
                byte[] buff = new byte[vs.Width * vs.Height * 3];

                if (vs.ContentType == "image/jpeg")
                {
                    //Декодирование кадра
                    if (codec_type == CodecType.VCM)
                    {
                        //Декодирование кадра VCM
                        decoder.DecodeFrame(frame.FrameData, vs.Width, vs.Height, buff);
                    }
                    if (codec_type == CodecType.VCM)
                    {
                        //Декодирование кадра WPF
                        decoder.DecodeWPF(frame.FrameData, vs.Width, vs.Height, buff);
                        //Переворачивание кадра относительно горизонтельной оси
                        byte[] img = new byte[buff.Length];
                        AlfaPribor.IppInterop.IppiSize size;
                        size.width = vs.Width;
                        size.height = vs.Height;
                        AlfaPribor.IppInterop.IppFunctions.ippiMirror_8u_C3R(buff, vs.Width * 3, img, vs.Height * 3,
                                                                             size, AlfaPribor.IppInterop.IppiAxis.ippAxsHorizontal);
                        buff = img;
                    }
                    return GetBitmapImage(buff, vs.Width, vs.Height);
                }

                if (vs.ContentType == "image/raw8")
                {
                    byte[] f = ApplyPalette(buff, vs.Width, vs.Height);
                    return GetBitmapImage(f, vs.Width, vs.Height);
                }

                return null;

            }
            else return null;
        }

        /// <summary>Применение палитры</summary>
        /// <param name="buffer">Входной буфер термограммы 8 бит на пиксель</param>
        byte[] ApplyPalette(byte[] buffer, int width, int height)
        {
            int step = 3;//шаг в результирующем изображении
            byte[] drawbuf = new byte[width * height * step];
            //Замена всех каналов цветного изображения значениями из серого
            IppiSize size = new IppiSize();
            size.width = width;
            size.height = height;
            IppFunctions.ippiDup_8u_C1C3R(buffer, width, drawbuf, width * step, size);
            return drawbuf;
        }

        /// <summary>Получение Bitmap</summary>
        public Bitmap GetBitmapImage(byte[] bmp, int w, int h)
        {
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
            Bitmap bitmap = new Bitmap(ms);
            return bitmap;
        }

        /// <summary>Преобразование объекта в массив байт</summary>
        /// <param name="obj">Объект</param>
        /// <returns>Массив байт</returns>
        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        #endregion
        
        #region Thread Safe

        /// <summary>Безопасное получение выбранного значения комбо бокса</summary>
        /// <param name="cb">Комбо бокс</param>
        /// <returns>Выбранное значение</returns>
        uint GetComboBoxSelectedValueThreadSafe(ComboBox cb)
        {
            uint sel_value = 0;
            if (cb.InvokeRequired)
            {
                GetComboBoxSelectedValueCallback d = new GetComboBoxSelectedValueCallback(GetComboBoxSelectedValueThreadSafe);
                try
                {
                    sel_value = (uint)Invoke(d, new object[] { cb });
                }
                catch { };
            }
            else sel_value = (uint)cb.SelectedValue;
            return sel_value;
        }

        /// <summary>Безопасное получение текста Текст Бокса</summary>
        /// <param name="cb">Комбо бокс</param>
        /// <returns>Выбранное значение</returns>
        string GetTextBoxTextThreadSafe(TextBox tb)
        {
            string value = "";
            if (tb.InvokeRequired)
            {
                GetTextBoxTextCallback d = new GetTextBoxTextCallback(GetTextBoxTextThreadSafe);
                try
                {
                    value = (string)Invoke(d, new object[] { tb });
                }
                catch { };
            }
            else value = tb.Text;
            return value;
        }

        /// <summary>Безопасное получение значениея NumericUpDown</summary>
        /// <param name="nud">NumericUpDown</param>
        /// <returns>Выбранное значение</returns>
        decimal GetNumericUpDownValueThreadSafe(NumericUpDown nud)
        {
            decimal value = 0;
            if (nud.InvokeRequired)
            {
                GetNumericUpDownValueCallback d = new GetNumericUpDownValueCallback(GetNumericUpDownValueThreadSafe);
                try
                {
                    value = (decimal)Invoke(d, new object[] { nud });
                }
                catch { };
            }
            else value = nud.Value;
            return value;
        }

        /// <summary>Безопасное получение значениея CheckBox</summary>
        /// <param name="cb">CheckBox</param>
        /// <returns>Выбранное значение</returns>
        bool GetCheckBoxValueThreadSafe(CheckBox cb)
        {
            bool value = false;
            if (cb.InvokeRequired)
            {
                GetCheckBoxValueCallback d = new GetCheckBoxValueCallback(GetCheckBoxValueThreadSafe);
                try
                {
                    value = (bool)Invoke(d, new object[] { cb });
                }
                catch { };
            }
            else value = cb.Checked;
            return value;
        }

        /// <summary>Безопасная установка активности контрола</summary>
        /// <param name="b">Кнопка</param>
        /// <param name="value">Активность</param>
        void ControlEnabledThreadSafe(Control b, bool value)
        {
            if (b.InvokeRequired)
            {
                ControlEnabledCallback d = new ControlEnabledCallback(ControlEnabledThreadSafe);
                try
                {
                    Invoke(d, new object[] { b, value });
                }
                catch { };
            }
            else b.Enabled = value;
        }

        /// <summary>Безопасная установка активности контрола</summary>
        /// <param name="b">Кнопка</param>
        /// <param name="value">Активность</param>
        void ToolStripButtonEnabledThreadSafe(ToolStripButton b, bool value)
        {
            if (b.Owner.InvokeRequired)
            {
                ToolStripButtonEnabledCallback d = new ToolStripButtonEnabledCallback(ToolStripButtonEnabledThreadSafe);
                try
                {
                    Invoke(d, new object[] { b, value });
                }
                catch { };
            }
            else b.Enabled = value;
        }

        /// <summary>Безопасная установка активности контрола</summary>
        /// <param name="b">Кнопка</param>
        /// <param name="value">Активность</param>
        void SetProgessBarValueThreadSafe(ProgressBar pb, int value, ValueType v_type)
        {
            if (pb.InvokeRequired)
            {
                SetProgessBarValueCallback d = new SetProgessBarValueCallback(SetProgessBarValueThreadSafe);
                try
                {
                    Invoke(d, new object[] { pb, value, v_type });
                }
                catch { };
            }
            else
            {
                try
                {
                    if (v_type == ValueType.Value) pb.Value = value;
                    if (v_type == ValueType.Maximum) pb.Maximum = value;
                    if (v_type == ValueType.Minimum) pb.Minimum = value;
                }
                catch { };
            }
        }

        /// <summary>Безопасное закрытие окна</summary>
        /// <param name="form">Форма</param>
        void CloseFormThreadSafe(Form form)
        {
            if (form.InvokeRequired)
            {
                CloseFormCallback d = new CloseFormCallback(CloseFormThreadSafe);
                try
                {
                    Invoke(d, new object[] { form });
                }
                catch { };
            }
            else
            {
                form.Close();
            }
        }

        /// <summary>Безопасная установка заголовка окна</summary>
        /// <param name="form">Форма</param>
        /// <param name="caption">Заголовок</param>
        void SetFormCaptionThreadSafe(Form form, string caption)
        {
            if (form.InvokeRequired)
            {
                SetFormCaptionCallback d = new SetFormCaptionCallback(SetFormCaptionThreadSafe);
                try
                {
                    Invoke(d, new object[] { form, caption });
                }
                catch { };
            }
            else
            {
                form.Text = caption;
            }
        }

        void SetGroupBoxCaptionThreadSafe(GroupBox gb, string caption)
        {
            if (gb.InvokeRequired)
            {
                SetGroupBoxCaptionCallback d = new SetGroupBoxCaptionCallback(SetGroupBoxCaptionThreadSafe);
                try
                {
                    Invoke(d, new object[] { gb, caption });
                }
                catch { };
            }
            else
            {
                gb.Text = caption;
            }
        }
        #endregion

        /// <summary>Проверяет код результата выполнения операции с AVI-файлом</summary>
        /// <param name="result">Результат выполнения операции с AVI-файлом</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти для открытия файла</exception>
        /// <exception cref="System.IO.IOException">Ошибка на диске или ошибка чтения/записи в файл</exception>
        /// <exception cref="System.Exception">Неизвестная ошибка</exception>
        /// <exception cref="AlfaPribor.AviFile.BadAviFormatException">Неизвестный формат AVI-файла</exception>
        /// <exception cref="AlfaPribor.AviFile.CodecException">Компрессия/декомпрессия заданного формата данных не поддерживается</exception>
        /// <exception cref="AlfaPribor.AviFile.BufferTooSmallException">Недостаточный размер буфера для чтения данных</exception>
        void CheckAviFileResult(uint result)
        {
            switch (result)
            {
                case Avi.AVIERR_OK:
                    return;

                case Avi.AVIERR_BADFORMAT:
                    throw new BadAviFormatException();

                case Avi.AVIERR_UNSUPPORTED:
                    throw new CodecException("Compression/decompression for this data format is not supported!");

                case Avi.AVIERR_MEMORY:
                    throw new OutOfMemoryException();

                case Avi.AVIERR_FILEREAD:
                    throw new IOException("Can not read a file!");

                case Avi.AVIERR_FILEWRITE:
                    throw new IOException("Can not write a file!");

                case Avi.AVIERR_FILEOPEN:
                    throw new IOException("Can not open a file!");

                case Avi.AVIERR_READONLY:
                    throw new IOException("Can`t write into a file because file is read only!");

                case Avi.AVIERR_BUFFERTOOSMALL:
                    throw new BufferTooSmallException();

                case Avi.AVIERR_NOCOMPRESSOR:
                    throw new CodecException("Compressor not found!");

                case Avi.AVIERR_NODATA:
                    return;

                default:
                    throw new Exception("Unknown error!");
            }
        }

        void tsbExport_Click(object sender, EventArgs e)
        {
            FormConnectionString form = new FormConnectionString();
            form.Show();
        }

        void cbSelectWagonOption_CheckedChanged(object sender, EventArgs e)
        {
            gbSelectDb.Enabled = cbSelectWagonOption.Checked;
        }

        void ShowConnectionStringForm()
        {
            FormConnectionString form = new FormConnectionString();
            form.Show();
        }

        FormSelectWagons _selectWagonForm=null;

        void ShowSelectWagonForm()
        {
            _selectWagonForm = new FormSelectWagons();
            _selectWagonForm.FormClosed += new FormClosedEventHandler(_selectWagonForm_FormClosed);
            _selectWagonForm.ShowDialog();
        }
        
        void SetWagonExportState()
        {
            cbSelectWagonOption.CheckedChanged -= cbSelectWagonOption_CheckedChanged;
            if (SettingContainer.SelectedWagons.Count > 0)
            {
                cbSelectWagonOption.CheckState = CheckState.Checked;
            }
            else
            {
                cbSelectWagonOption.CheckState = CheckState.Unchecked;
            }
            cbSelectWagonOption.CheckedChanged += cbSelectWagonOption_CheckedChanged;
        }

        void _selectWagonForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetWagonExportState();
        }

        void SetDtatbaseConnectionTextBox_Click(object sender, EventArgs e)
        {
            ShowConnectionStringForm();
        }

        void btnSelectWagons_Click(object sender, EventArgs e)
        {
            ShowSelectWagonForm();
        }

        void InstallSystemTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SettingContainer.DatabaseProviderType = (ProviderType)cbInstallSystemType.SelectedIndex;
                SettingContainer.ResetProvider();
            }
            catch
            {
            }
        }

        void dgvTelecameras_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void InstalSystemTypeLabel_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
      

