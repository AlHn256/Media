using System;
using System.Collections.Generic;
using System.Text;
using AxisNamespace;
using System.Net;
using WebCamCommon;

namespace AlfaPribor.VideoCameras4
{
    
    /// <summary>Класс "камера Beward BD-серии"</summary>
    public class BewardHttpCamera: VideoCameraBase
    {

        #region Variables

        /// <summary>Объект связи с телекамерой</summary>
        BewardVideo beward;
        /// <summary>Идентификатор телекамеры</summary>
        int index;
        /// <summary>Сетевой адрес телекамеры</summary>
        string FHost = "";
        /// <summary>Сетевой порт телекамеры</summary>
        int FPort = 8008;
        /// <summary>Номер канала</summary>
        int FChannel = 1;
        /// <summary>Имя пользователя телекамеры</summary>
        string FUsername = "";
        /// <summary>Пароль телекамеры</summary>
        string FPassword = "";
        /// <summary>Название телекамеры</summary>
        string FModel;
        /// <summary>Название видеоканала</summary>
        string FName;
        /// <summary>Тип телекамеры</summary>
        VideoCameraType FType;
        /// <summary>Статус телекамеры</summary>
        VideoCameraStatus _State = VideoCameraStatus.Unknown;

        #endregion

        /// <summary>Конструктор класса телекамеры Beward</summary>
        /// <param name="settings">Параметры IP камеры</param>
        public BewardHttpCamera(IpCameraSettings settings) : base(settings)
        {
            SetLocalParameters(settings);
        }

        /// <summary>Присвоение локальный параметров</summary>
        /// <param name="settings">Параметры телекамеры</param>
        void SetLocalParameters(IpCameraSettings settings)
        {
            // Инициализация приватных членов
            //ip
            index = settings.Id;
            FHost = settings.Address;
            FChannel = settings.Channel;
            FUsername = settings.UserName;
            FPassword = settings.Password;
            //common
            FModel = settings.Model;
            FName = settings.Name;
            FType = settings.Type;
        }

        /// <summary>Установка активности телекамеры</summary>
        /// <param name="value">Активность телекамеры</param>
        protected override void SetActive(bool value)
        {
            if (Active == value) return;
            if (value)
            {
                if (!Connect())
                {
                    //Если подключение не сработало
                    base.SetActive(false);
                    return;
                }
            }
            else
            {
                Disconnect();
            }
            base.SetActive(value);
        }

        /// <summary>Установка параметров телекамеры</summary>
        /// <param name="value">Структура параметров</param>
        protected override void SetSettings(VideoCameraSettings value)
        {
            if (value is IpCameraSettings)
            {
                bool need_restart = Active;
                if (need_restart) Active = false;
                base.SetSettings(value);
                if (need_restart) Active = true;
            }
        }

        /// <summary>Запрос параметров совместимости телекамеры</summary>
        /// <returns>Структура параметров совместимости</returns>
        protected override VideoCameraCompat GetCompat()
        {
            VideoCameraCompat compat = new VideoCameraCompat();
            compat.Brightness = false;
            compat.Contrast = false;
            compat.Hue = true;
            compat.Saturation = false;
            compat.Sharpness = true;
            compat.Shutter = true;
            compat.Compression = true;
            compat.Resolutions = "1280x960;1600x1200";
            compat.FPS = true;
            compat.format = VideoCameraFormatFrame.Jpeg;
            return compat;
        }

        #region Video parameters

        protected override void SetSaturation(int value)
        {
            // Поверка значения цветности
            if (value < 0 && value > 100) return;
            // Установка значения цветности на камере
            SendWebCommand(cSaturation, value.ToString());

            base.SetSaturation(value);
        }

        /// <summary>Установка четкости в процентах</summary>
        /// <param name="value">Значение четкости в процентах</param>
        protected override void SetSharpness(int value)
        {
            // Поверка значения резкости
            if (value < 0 && value > 100) return;
            // Установка значения резкости на камере
            SendWebCommand(cSharpness, value.ToString());

            base.SetSharpness(value);
        }

        /// <summary>Установка времени выдержки в микросекундах</summary>
        /// <param name="value">Значение времени выдержки в микросекундах</param>
        protected override void SetShutter(int value)
        {
            // Поверка значения выдержки
            if (value < 0 && value > 1000000) return;
            // Установка значения выдержки на камере
            SendWebCommand(cExposureTime, value.ToString());

            base.SetShutter(value);
        }

        /// <summary>Установка коэффициента компресии</summary>
        /// <param name="value">Коэффициент компресии от 0 до 100</param>
        protected override void SetCompression(int value)
        {
            // Поверка значения коэффициента компресии
            if (value < 0 && value > 100) return;
            // Установка значения коэффициента уомпресии на камере
            SendWebCommand(cQuality, value.ToString());

            base.SetCompression(value);
        }

        /// <summary>Установить темп передачи кадров</summary>
        /// <param name="value">Темп передачи кадров</param>
        protected override void SetFPS(int value)
        {
            // Поверка значения коэффициента компресии
            if (value < 0 && value > 100) return;
            // Установка значения коэффициента уомпресии на камере
            SendWebCommand(cFPS, value.ToString());

            base.SetCompression(value);
        }

        #endregion

        /// <summary>Подключение</summary>
        bool Connect()
        {
            IpCameraSettings settings = Settings as IpCameraSettings;
            if (settings == null) return false;
            SetLocalParameters(settings);
            if (FUsername == "") return false;
            if (FPassword == "") return false;
            if (FHost == "") return false;
            try
            {
                beward = new BewardVideo(FHost, FPort, FUsername, FPassword, settings.Resolution, settings.FPS.ToString());
                beward.Channel = FChannel;
                beward.OnFrame += new EventHandler<BewardVideoFrameEventArgs>(beward_OnFrame);
                beward.OnStatus += new EventHandler<BewardVideoStatusEventArgs>(beward_OnStatus); 
                // Подключение к камере
                beward.Active = true;
            }
            catch (Exception e) { return false; }
            return true;
        }

        public VideoCameraStatus State
        {
            get { return _State; }
            protected set
            {
                if (_State != value)
                {
                    _State = value;
                    RaiseOnCameraStatus(this, _State);
                }
            }
        }

        void beward_OnStatus(object sender, BewardVideoStatusEventArgs e)
        {
            if (e.Status == BewardVideoStatus.Online)
            {
                State = VideoCameraStatus.Online;
            }
            else if (e.Status == BewardVideoStatus.Offline ||
                e.Status == BewardVideoStatus.AccessDenied ||
                e.Status == BewardVideoStatus.BadRequest ||
                e.Status == BewardVideoStatus.Error)
            {
                State = VideoCameraStatus.Offline;
            }
            else if (e.Status == BewardVideoStatus.Inactive)
            {
                State = VideoCameraStatus.Unknown;
            }
        }

        void beward_OnFrame(object sender, BewardVideoFrameEventArgs e)
        {
            RaiseOnCameraFrame(this, e.Frame);
        }

        /// <summary>Отключение</summary>
        void Disconnect()
        {
            // Отключение от камеры
            beward.Active = false;
            beward.OnFrame -= beward_OnFrame;
            beward.OnStatus -= beward_OnStatus;
            beward.Dispose();
            beward = null;
        }

        #region WebCommands (Видеопараметры телекамеры)

        //Строки команд

        //Изменение насыщенности
        string cSaturation = "ImageControls.Saturation";
        //Изменение режима экспозиции
        string cExposureMode = "ImageControls.ExposureMode";
        //Изменение времени экспозиции
        string cExposureTime = "ImageControls.ExposureTime";
        //Изменение четкости
        string cSharpness = "ImageControls.Sharpness";
        //Изменение гаммы
        string cGamma = "ImageControls.Gamma";
        //Изменение сжатия
        string cQuality = "Stream.Quality";
        //Изменение темпа передачи кадров
        string cFPS = "Stream.FrameRateScaling";

        /// <summary>Посылка изменения параметра телекамеры</summary>
        /// <param name="Param">Параметр</param>
        /// <param name="NewValue">Новое значения</param>
        void SendWebCommand(string Param, string NewValue)
        {
            if (FHost == "") return;
            string url;
            url = "http://" + FHost + "/cgi-bin/param_if.cgi?NumActions=1&Action_0=";
            url += Param + "=" + NewValue;
            WebRequest request = WebRequest.Create(url);
            string user = FUsername;
            string password = FPassword;
            request.Credentials = new NetworkCredential(user, password);
            AsyncCallback callback = new AsyncCallback(AsyncCallback);
            request.BeginGetResponse(callback, request);
        }

        void AsyncCallback(IAsyncResult ar)
        {
            try
            {
                WebRequest request = (WebRequest)ar.AsyncState;
                request.EndGetResponse(ar);
            }
            catch {  }
        }

        #endregion WebCommands

    }

}
