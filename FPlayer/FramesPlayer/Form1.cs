using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//Внешние проекты
using AlfaPribor.VideoPlayer;
using AlfaPribor.VideoStorage;
using AlfaPribor.DirectXVideoRenderer;
using AlfaPribor.JpegCodec;
using AlfaPribor.IppInterop;

namespace FramesPlayer
{
    public partial class Form1 : Form
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
        /// <summary>Тип кодека</summary>
        CodecType codec_type;
        /// <summary>Массив типов телекамер</summary>
        //VideoStreamInfo[] StreamInfo;
        /// <summary>Состояние изменения размера панели</summary>
        bool ProcessResize;

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            CreateVideoPlayer();
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
            //obj_VideoPlayer.ShowLengthInGroupBox = true;
            //Создание кнопок
            List<VideoPlayerInterface.PlayerButtonInfo[]> buttons = new List<VideoPlayerInterface.PlayerButtonInfo[]>();

            //Кнопки воспроизведения
            VideoPlayerInterface.PlayerButtonInfo[] play_buttons = new VideoPlayerInterface.PlayerButtonInfo[3];
            play_buttons[0] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.PlayBack,  Properties.Resources.Play_back_active);
            play_buttons[1] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.Pause, Properties.Resources.Pause_active);
            play_buttons[2] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.Play, Properties.Resources.Play_active);

            //Кнопки перехода по кадрам
            VideoPlayerInterface.PlayerButtonInfo[] frames_buttons = new VideoPlayerInterface.PlayerButtonInfo[2];
            frames_buttons[0] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.PrevFrame, Properties.Resources.Cadr_prev_active);
            frames_buttons[1] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.NextFrame, Properties.Resources.Cadr_next_active);

            //Кнопки перехода в начало и конец записи
            VideoPlayerInterface.PlayerButtonInfo[] last_buttons = new VideoPlayerInterface.PlayerButtonInfo[2];
            last_buttons[0] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.FirstFrame, Properties.Resources.Beginning_active);
            last_buttons[1] = new VideoPlayerInterface.PlayerButtonInfo
                (VideoPlayerInterface.PlayerButtonType.LastFrame, Properties.Resources.End_active);

            //Добавление кнопок плееру
            buttons.Add(play_buttons);
            buttons.Add(frames_buttons);
            buttons.Add(last_buttons);

            obj_VideoPlayer.SetButtons(buttons);

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

        /// <summary>Событие получения кадра от плеера</summary>
        /// <param name="buf">Данные кадра</param>
        /// <param name="timestamp">Метка времени от начала видеопотока</param>
        void obj_VideoPlayer_EventNewFrame(int resX, int resY, int channel_id, byte[] frame, int timestamp)
        {
            if (GetCameraType(channel_id) == ImageType.Unknown) return;

            //Отображение кадра телевизионной камеры
            if (GetCameraType(channel_id) == ImageType.Jpeg)
                if (obj_DrawFrames_TK != null && !ProcessResize &&
                    obj_DrawFrames_TK.ChannelID == channel_id && !obj_DrawFrames_TK.InitState)
                    DrawJpegFrame(channel_id, frame, resX, resY);

            //Отображение тепловизионного кадра raw8
            if (GetCameraType(channel_id) == ImageType.Raw8)
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
                if (obj_DrawFrames_TK != null) obj_DrawFrames_TK.SetFPS(fps);
            }
            if (GetCameraType(stream_id) == ImageType.Raw8)
            {
                if (obj_DrawFrames_TV != null) obj_DrawFrames_TV.SetFPS(fps);
            }
        }

        /// <summary>Обработка открытия видео</summary>
        /// <param name="info">Информация о видео</param>
        void obj_VideoPlayer_EventOpenVideo(IList<VideoStreamInfo> info)
        {
            //Проверка объектов отрисовки кадров 
            //и в случае изменени я параметров отображаемого видео 
            //переинициализация в случае несовпавдения разрешения

            //Объект отрисовки кадров телекамеры
            if (obj_DrawFrames_TK != null)
                for (int i = 0; i < info.Count; i++)
                    //Найден объект отрисовки видеоканала
                    if (info[i].Id == obj_DrawFrames_TK.ChannelID)
                        if (info[i].Width != obj_DrawFrames_TK.FrameWidth ||
                            info[i].Height != obj_DrawFrames_TK.FrameHeight)
                        {
                            //Переинициализация объекта отрисовки
                            int id = obj_DrawFrames_TK.ChannelID;//Идентификатор отображаемой телекамеры
                            CreateDrawFramesTK(info[i].Width, info[i].Height, id, false, 0);
                            break;
                        }

            //Кнопка выбора телекамер
            tsmiChannels.Enabled = true;
        }

        /// <summary>Обработка закрытия видео</summary>
        void obj_VideoPlayer_EventCloseVideo()
        {
            //Проверка объектов отрисовки кадров 
            //и в случае изменения параметров отображаемого видео 
            //переинициализация
            obj_DrawFrames_TK.Dispose();
            obj_DrawFrames_TK = null;

            obj_DrawFrames_TV.Dispose();
            obj_DrawFrames_TV = null;

            //Кнопка выбора телекамер
            tsmiChannels.Enabled = false;
        }

        /// <summary>Отрисовка сжатого jpeg кадра</summary>
        /// <param name="channel_id">Идентификатор видеоканала</param>
        /// <param name="frame">Данные кадра</param>
        void DrawJpegFrame(int channel_id, byte[] frame, int resX, int resY)
        {
            byte[] out_data = new byte[resX * resY * 3];
            //Декомпрессия кадра
            if (codec_type == CodecType.VCM) obj_JpegDecoder.DecodeFrame(frame, resX, resY, out_data);
            if (codec_type == CodecType.WPF)
            {
                obj_JpegDecoder.DecodeWPF(frame, resX, resY, out_data);
                //Переворачивание кадра относительно горизонтельной оси
                byte[] img = new byte[out_data.Length];
                AlfaPribor.IppInterop.IppiSize size;
                size.width = resX;
                size.height = resY;
                AlfaPribor.IppInterop.IppFunctions.ippiMirror_8u_C3R(out_data, resX * 3, img, resX * 3,
                                                                     size, AlfaPribor.IppInterop.IppiAxis.ippAxsHorizontal);
                out_data = img;
            }
            //Отрисовка декодированного кадра
            if (obj_DrawFrames_TK != null) obj_DrawFrames_TK.NewFrame(out_data);
        }

        /// <summary>Инициализация объекта отрисовки кадров телекамер</summary>
        /// <param name="resX">Ширина кадра</param>
        /// <param name="resY">Высота кадра</param>
        /// <param name="id">Идентификатор телекамеры</param>
        /// <param name="AniFish">Исправлять искажения</param>
        /// <param name="Coeff">КОэффициент исправления искажений</param>
        void CreateDrawFramesTK(int resX, int resY, int id, bool AniFish, int Coeff)
        {
            if (obj_DrawFrames_TK == null) obj_DrawFrames_TK = new DrawTelecameraFrames();
            obj_DrawFrames_TK.Init(resX, resY, pictureBoxVideo, false, AniFish, Coeff);
            obj_DrawFrames_TK.ChannelID = id;
            obj_DrawFrames_TK.ShowGraphicElement(GraphicElement.DrawFPS, false);
        }

        #region Изменение размера панели

        void pictureBoxVideo_SizeChanged(object sender, EventArgs e)
        {
            ProcessResize = false;
        }

        void pictureBoxVideo_Resize(object sender, EventArgs e)
        {
            ProcessResize = true;
        }

        #endregion

        void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Диалог открытия файла
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = Application.ExecutablePath;
            openFile.Multiselect = false;
            if (openFile.ShowDialog() != DialogResult.OK) return;
            if (!System.IO.File.Exists(openFile.FileName)) return;

            if (obj_VideoStorage != null) obj_VideoStorage.Active = false;
            if (obj_VideoStorage == null) obj_VideoStorage = new VideoStorage();
            //Открытие видео
            VideoStorageSettings vs_settings = new VideoStorageSettings();
            vs_settings.Partitions.Add(new VideoPartitionSettings(0, true,
                                       System.IO.Path.GetDirectoryName(openFile.FileName), 0));
            //Интервал проверки хранилища циклическим буфером
            vs_settings.CircleBufferCheckInterval = 0;
            obj_VideoStorage.SetSettings(vs_settings);
            obj_VideoStorage.Active = true;
            //Присвоение плееру объекта хранилища
            obj_VideoPlayer.ObjVideoStorage = obj_VideoStorage;
            if (obj_VideoPlayer.Open(System.IO.Path.GetFileNameWithoutExtension(openFile.FileName),0))
            {
                obj_VideoPlayer.Delta = 100;
                obj_VideoPlayer.Position = 0;
                /*
                //Чтение инормации о потоках
                IVideoIndex index = obj_VideoPlayer.ObjVideoReader.VideoIndex;
                StreamInfo = new Camera[index.StreamInfoList.Count];
                for (int i = 0; i < StreamInfo.Length; i++)
                {

                    CameraTypes[i].id = index.StreamInfoList[i].Id;
                    CameraTypes[i].type = index.StreamInfoList[i].ContentType;
                }
                */ 
            }
        }

    }

}
