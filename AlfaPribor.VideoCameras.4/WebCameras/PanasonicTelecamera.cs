using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using WebCamCommon;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Класс "камера Panasonic" протокол HTTP
    /// В разработке, бета версия, для проверки работоспособности телекамеры</summary>
    public class PanasonicTelecamera : VideoCameraBase
    {

        #region Variables

        /// <summary>Объект связи с телекамерой</summary>
        PanasonicVideo panasonic;
        /// <summary>Идентификатор телекамеры</summary>
        int index;
        /// <summary>Сетевой адрес телекамеры</summary>
        string FHost = "";
        /// <summary>Номер входа видеосервера</summary>
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
        /// <summary>Состояние подключения</summary>
        VideoCameraStatus state = VideoCameraStatus.Unknown;

        #endregion

        /// <summary>Конструктор класса телекамеры</summary>
        /// <param name="settings">Параметры IP камеры</param>
        public PanasonicTelecamera(IpCameraSettings settings) : base(settings)
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
            compat.Brightness = true;
            compat.Contrast = true;
            compat.Hue = true;
            compat.Saturation = false;
            compat.Sharpness = true;
            compat.Shutter = true;
            compat.Compression = true;
            compat.Resolutions = "2048x1536;1920x1080;1600x1200;1280x1024;1280x960;1280x720;" + 
                                 "1024x768;800x500;720x576;704x576;704x288;640x480;640x288;480x360;352x288;320x240;";
            compat.FPS = true;
            compat.format = VideoCameraFormatFrame.Jpeg;
            return compat;
        }

        /*
        #region Video parameters

        /// <summary>Установка яркости в процентах</summary>
        /// <param name="value">Значение яркости в процентах</param>
        protected override void SetBrightness(int value)
        {
            // Поверка значения яркости
            if (value < 0 && value > 100) return;
            // Установка значения яркости на камере
            SendWebCommand(cBrightness, value.ToString());
            base.SetBrightness(value);
        }

        /// <summary>Установка контрастности в процентах</summary>
        /// <param name="value">Значение контрастности в процентах</param>
        protected override void SetContrast(int value)
        {
            // Поверка значения контрастности
            if (value < 0 && value > 100) return;
            // Установка значения контрастности на камере
            SendWebCommand(cContrast, value.ToString());
            base.SetContrast(value);
        }

        protected override void SetSaturation(int value)
        {
            // Поверка значения цветности
            if (value < 0 && value > 100) return;
            // Установка значения цветности на камере
            SendWebCommand(cColorLevel, value.ToString());

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
            SendWebCommand(cMaxExposureTime, value.ToString());

            base.SetShutter(value);
        }

        /// <summary>Установка коэффициента компресии</summary>
        /// <param name="value">Коэффициент компресии от 0 до 100</param>
        protected override void SetCompression(int value)
        {
            // Поверка значения коэффициента компресии
            if (value < 0 && value > 100) return;
            // Установка значения коэффициента уомпресии на камере
            SendWebCommand(cCompression, value.ToString());

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

        /// <summary>Установить врезку триггеров</summary>
        /// <param name="enable">Включить врезку</param>
        public void SetTriggerData(bool enable)
        {
            if (enable) SendWebCommand(cSetTriggers, "yes");
            else SendWebCommand(cSetTriggers, "no");
        }

        #endregion
        */

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
                panasonic = new PanasonicVideo(FHost, FUsername, FPassword, settings.Resolution, settings.FPS.ToString(), settings.Compression.ToString());
                panasonic.Camera = FChannel.ToString();
                panasonic.OnFrame += new EventHandler<PanasonicVideoFrameEventArgs>(OnFrame);
                panasonic.OnStatus += new EventHandler<PanasonicVideoStatusEventArgs>(OnStatus); 
                // Подключение к камере
                panasonic.Active = true;
            }
            catch(Exception) { return false; }
            return true;
        }

        public VideoCameraStatus State
        {
            get { return state; }
            protected set
            {
                if (state != value)
                {
                    state = value;
                    RaiseOnCameraStatus(this, state);
                }
            }
        }

        void OnStatus(object sender, PanasonicVideoStatusEventArgs e)
        {
            if (e.Status == PanasonicVideoStatus.Online)
            {
                State = VideoCameraStatus.Online;
            }
            else if (e.Status == PanasonicVideoStatus.Offline || 
                e.Status == PanasonicVideoStatus.AccessDenied || 
                e.Status == PanasonicVideoStatus.BadRequest || 
                e.Status == PanasonicVideoStatus.Error)
            {
                State = VideoCameraStatus.Offline;
            }
            else if (e.Status == PanasonicVideoStatus.Inactive)
            {
                State = VideoCameraStatus.Unknown;
            }
        }

        void OnFrame(object sender, PanasonicVideoFrameEventArgs e)
        {
            RaiseOnCameraFrame(this, e.Frame);
        }

        /// <summary>Отключение</summary>
        void Disconnect()
        {
            // Отключение от камеры
            panasonic.Active = false;
            panasonic.OnFrame -= OnFrame;
            panasonic.OnStatus -= OnStatus;
            panasonic.Dispose();
            panasonic = null;
        }

        /*
        #region WebCommands (Видеопараметры телекамеры)

        //Строки команд

        //Изменение яркости
        string cBrightness = "ImageSource.I0.Sensor.Brightness";
        //Изменение контрастрности
        string cContrast = "ImageSource.I0.Sensor.Contrast";
        //Изменение цветности
        string cColorLevel = "ImageSource.I0.Sensor.ColorLevel";
        //Изменение времени экспозиции
        string cMaxExposureTime = "ImageSource.I0.Sensor.MaxExposureTime";
        //Изменение четкости
        string cSharpness = "ImageSource.I0.Sensor.Sharpness";
        //Изменение сжатия
        string cCompression = "Image.I0.Appearance.Compression";
        //Изменение темпа передачи кадров
        string cFPS = "Image.I0.Stream.FPS";
        //Установка врезки триггеров
        string cSetTriggers = "Image.TriggerDataEnabled";

        /// <summary>Посылка изменения параметра телекамеры</summary>
        /// <param name="Param">Параметр</param>
        /// <param name="NewValue">Новое значения</param>
        void SendWebCommand(string Param, string NewValue)
        {
            if (FHost == "") return;
            string url;
            url = "http://" + FHost;
            url += "/axis-cgi/param.cgi?action=update&";
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
            catch { }
        }

        #endregion WebCommands
        */

    }

}
