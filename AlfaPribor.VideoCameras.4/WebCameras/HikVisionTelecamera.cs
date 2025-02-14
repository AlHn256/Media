using System;
using System.Collections.Generic;
using System.Text;
using AxisNamespace;
using System.Net;
using WebCamCommon;

namespace AlfaPribor.VideoCameras4
{

    public class HikVisionTelecamera : VideoCameraBase
    {

        #region Variables

        /// <summary>Объект связи с телекамерой</summary>
        HikVisionVideo hikvision;
        /// <summary>Идентификатор телекамеры</summary>
        int index;
        /// <summary>Сетевой адрес телекамеры</summary>
        string FHost = "";
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

        /// <summary>Конструктор класса телекамеры HikVision</summary>
        /// <param name="settings">Параметры IP камеры</param>
        public HikVisionTelecamera(IpCameraSettings settings) : base(settings)
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
            compat.Resolutions = "1920x1080;1280x720;704x576;352x288";
            compat.FPS = true;
            compat.format = VideoCameraFormatFrame.Jpeg;
            return compat;
        }

        #region Video parameters


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
                hikvision = new HikVisionVideo(FHost, FUsername, FPassword,
                                               settings.Resolution, settings.FPS.ToString(), settings.Compression.ToString());
                hikvision.Channel = FChannel;
                hikvision.OnFrame += new EventHandler<HikVisionVideoFrameEventArgs>(hikvision_OnFrame);
                hikvision.OnStatus += new EventHandler<HikVisionVideoStatusEventArgs>(hikvision_OnStatus);
                // Подключение к камере
                hikvision.Active = true;
            }
            catch (Exception e) { return false; }
            return true;
        }
        
        /// <summary>Отключение</summary>
        void Disconnect()
        {
            // Отключение от камеры
            hikvision.Active = false;
            hikvision.OnFrame -= hikvision_OnFrame;
            hikvision.OnStatus -= hikvision_OnStatus;
            hikvision.Dispose();
            hikvision = null;
        }
               
        /// <summary>Статус подключения</summary>
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
        
        /// <summary>Изменение статуса подключения камеры</summary>
        void hikvision_OnStatus(object sender, HikVisionVideoStatusEventArgs e)
        {
            if (e.Status == HikVisionVideoStatus.Online)
            {
                State = VideoCameraStatus.Online;
            }
            else if (e.Status == HikVisionVideoStatus.Offline ||
                e.Status == HikVisionVideoStatus.AccessDenied ||
                e.Status == HikVisionVideoStatus.BadRequest ||
                e.Status == HikVisionVideoStatus.Error)
            {
                State = VideoCameraStatus.Offline;
            }
            else if (e.Status == HikVisionVideoStatus.Inactive)
            {
                State = VideoCameraStatus.Unknown;
            }
        }

        /// <summary>Событие Получение кадра</summary>
        void hikvision_OnFrame(object sender, HikVisionVideoFrameEventArgs e)
        {
            RaiseOnCameraFrame(this, e.Frame);
        }
                                    
    }

}
