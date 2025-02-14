using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.VideoCameras4
{

    #region События компонента "камера"

    /// <summary>Делегат "Изменение коммуникационного статуса камеры"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="status">Текущий статус</param>
    public delegate void EvCameraStatus(object sender, VideoCameraStatus status);

    /// <summary>Делегат "Получен кадр"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="frame">Бинарные данные кадра</param>
    public delegate void EvCameraFrame(object sender, byte[] frame);

    /// <summary>Делегат "Получен кадр"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="frame">Бинарные данные кадра</param>
    /// <param name="resolution">Разрешение кадра</param>
    public delegate void EvCameraFrameEx(object sender, byte[] frame, string resolution);

    #endregion

    /// <summary>Базовый класс "телекамера"</summary>
    public class VideoCameraBase : IVideoCamera, IDisposable
    {

        #region Variables

        bool active;
        VideoCameraStatus _Status;
        VideoCameraSettings settings;
        VideoParams videoParams;

        #endregion

        #region Events

        /// <summary>Извещение об изменении статуса камеры</summary>
        public event EvCameraStatus OnCameraStatus;
        /// <summary>Извещение о получении очередного кадра</summary>
        public event EvCameraFrame OnCameraFrame;
        /// <summary>Извещение расширенное о получении очередного кадра</summary>
        //public event EvCameraFrameEx OnCameraFrameEx;

        #endregion

        /// <summary>Конструктор класса</summary>
        /// <param name="settings">Параметры телекамеры</param>
        public VideoCameraBase(VideoCameraSettings settings)
        {
            active = false;
            _Status = VideoCameraStatus.Unknown;
            this.settings = settings;
            videoParams = new VideoParams();
            videoParams.Brightness = 50;
            videoParams.Contrast = 50;
            videoParams.Hue = 50;
            videoParams.Saturation = 50;
            videoParams.Gamma = 50;
            videoParams.Sharpness = 0;
            videoParams.Shutter = 250;
            videoParams.Compression = 30;
        }

        #region Члены IVideoCamera

        /// <summary>Идентификатор камеры (номер видеопотока)</summary>
        public int Id
        {
            get 
            {
                if (settings == null) return 0;
                return settings.Id; 
            }
        }

         /// <summary>Активация камеры</summary>
        public bool Active
        {
            get { return active; }
            set
            {
                SetActive(value);
                if (value) _Status = VideoCameraStatus.Online;
                else _Status = VideoCameraStatus.Offline;
            }
        }

        /// <summary>Текущий статус камеры</summary>
        public VideoCameraStatus Status
        {
            get { return _Status; }
        }

        /// <summary>Настройки камеры</summary>
        public VideoCameraSettings Settings
        {
            get { return settings; }
            set { SetSettings(value); }
        }

        /// <summary>Параметры совместимости</summary>
        public VideoCameraCompat Compat
        {
            get { return GetCompat(); }
        }

        /// <summary>Яркость</summary>
        public int Brightness
        {
            get { return videoParams.Brightness; }
            set { SetBrightness(value); }
        }

        /// <summary>Контраст</summary>
        public int Contrast
        {
            get { return videoParams.Contrast; }
            set { SetContrast(value); }
        }

        /// <summary>Оттенок</summary>
        public int Hue
        {
            get { return videoParams.Hue; }
            set { SetHue(value); }
        }

        /// <summary>Насыщенность</summary>
        public int Saturation
        {
            get { return videoParams.Saturation; }
            set { SetSaturation(value); }
        }

        /// <summary>Гамма</summary>
        public int Gamma
        {
            get { return videoParams.Gamma; }
            set { SetGamma(value); }
        }

        /// <summary>Резкость</summary>
        public int Sharpness
        {
            get { return videoParams.Sharpness; }
            set { SetSharpness(value); }
        }

        /// <summary>Время затвора</summary>
        public int Shutter
        {
            get { return videoParams.Shutter; }
            set { SetShutter(value); }
        }

        /// <summary>Коэффициент компресии</summary>
        public int Compression
        {
            get { return videoParams.Compression; }
            set { SetCompression(value); }
        }

        #endregion

        /// <summary>Активация</summary>
        /// <param name="value">Активность телекамера</param>
        protected virtual void SetActive(bool value)
        {
            active = value;
        }

        /// <summary>Установить параметры телекамеры</summary>
        /// <param name="value">Структура параметров</param>
        protected virtual void SetSettings(VideoCameraSettings value)
        {
            settings = value;
        }

        /// <summary>Получение параметров совместимости</summary>
        protected virtual VideoCameraCompat GetCompat()
        {
            VideoCameraCompat compat = new VideoCameraCompat();
            compat.Brightness = false;
            compat.Contrast = false;
            compat.Hue = false;
            compat.Saturation = false;
            compat.Gamma = false;
            compat.Sharpness = false;
            compat.Shutter = false;
            compat.Compression = false;
            compat.Resolutions = "320x240";
            compat.format = VideoCameraFormatFrame.Bitmap;
            return compat;
        }

        /// <summary>Установка яркости телекамеры</summary>
        /// <param name="value">Значение яркости телекамеры</param>
        protected virtual void SetBrightness(int value)
        {
            videoParams.Brightness = value;
        }

        /// <summary>Установка контрастности телекамеры</summary>
        /// <param name="value">Значение контрастности телекамеры</param>
        protected virtual void SetContrast(int value)
        {
            videoParams.Contrast = value;
        }

        /// <summary>Установка оттенка телекамеры</summary>
        /// <param name="value">Значение оттенка телекамеры</param>
        protected virtual void SetHue(int value)
        {
            videoParams.Hue = value;
        }

        /// <summary>Установка насыщенности телекамеры</summary>
        /// <param name="value">Значение насыщенности телекамеры</param>
        protected virtual void SetSaturation(int value)
        {
            videoParams.Saturation = value;
        }

        /// <summary>Установка гаммы</summary>
        /// <param name="value">Значение гаммы</param>
        protected virtual void SetGamma(int value)
        {
            videoParams.Gamma = value;
        }

        /// <summary>Установка четкости телекамеры</summary>
        /// <param name="value">Значение четкости телекамеры</param>
        protected virtual void SetSharpness(int value)
        {
            videoParams.Sharpness = value;
        }

        /// <summary>Установка времени выдержки телекамеры</summary>
        /// <param name="value">Значение выдержки телекамеры</param>
        protected virtual void SetShutter(int value)
        {
            videoParams.Shutter = value;
        }

        /// <summary>Установка компрессии телекамеры</summary>
        /// <param name="value">Значение компрессии телекамеры</param>
        protected virtual void SetCompression(int value)
        {
            videoParams.Compression = value;
        }

        /// <summary>Установка скорости передачи кадров</summary>
        /// <param name="value">Значение скорости передачи кадров</param>
        protected virtual void SetFPS(int value)
        {
            videoParams.FPS = value;
        }

        /// <summary>Генерация события - "Статус телекамеры"</summary>
        /// <param name="sender">Объект телекамеры</param>
        /// <param name="status">Текущий статус</param>
        protected void RaiseOnCameraStatus(object sender, VideoCameraStatus status)
        {
            try
            {
                if (OnCameraStatus != null) OnCameraStatus(sender, status);
            }
            catch (Exception) { }
        }

        /// <summary>Генерация события текущего кадра</summary>
        /// <param name="sender">Объект телекамеры</param>
        /// <param name="frame">Текущий кадр</param>
        protected void RaiseOnCameraFrame(object sender, byte[] frame)
        {
            try
            {
                if (OnCameraFrame != null) OnCameraFrame(sender, frame);
            }
            catch (Exception ex) { }
        }

        #region Члены IDisposable

        public void Dispose()
        {
            Active = false;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
