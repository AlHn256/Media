using System;
using System.Collections.Generic;
using System.Text;
using AnalogNamespace;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Класс аналоговая телекамера</summary>
    public class AnalogTelecamera : VideoCameraBase
    {

        #region Variables

        /// <summary>Устройство трансляции</summary>
        CapDevice device;

        /// <summary>Идентификатор телекамеры</summary>
        private int index;
        /// <summary>Номер устройства видеозахвата</summary>
        private int FDevice = 0;
        /// <summary>Канал устройства видеозахвата</summary>
        private int FChannel = 0;
        /// <summary>Название телекамеры</summary>
        private string FModel;
        /// <summary>Название видеоканала</summary>
        private string FName;
        /// <summary>Тип телекамеры</summary>
        private VideoCameraType FType;

        #endregion

        /// <summary>Конструктор класса аналоговой телекамеры</summary>
        /// <param name="settings"></param>
        public AnalogTelecamera(AnalogCameraSettings settings) : base(settings)
        {
            SetLocalParameters(settings);
        }

        /// <summary>Присвоение локальный параметров</summary>
        /// <param name="settings">Параметры телекамеры</param>
        private void SetLocalParameters(AnalogCameraSettings settings)
        {
            // Инициализация приватных членов
            index = settings.Id;
            //analog
            FDevice = settings.Device;
            FChannel = settings.Channel;
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
            if (value is AnalogCameraSettings)
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
            compat.Saturation = true;
            compat.Gamma = false;
            compat.Sharpness = true;
            compat.Shutter = false;
            compat.Compression = false;
            compat.FPS = false;
            compat.Resolutions = "768x576;720x576;720x288;720x480;720x240;" +
                                 "640x576;640x480;640x288;640x240;" +
                                 "352x288;352x240;320x240;";
            compat.format = VideoCameraFormatFrame.Bitmap;
            return compat;
        }

        /// <summary>Установка видеостандарта</summary>
        /// <param name="VideoStandard">Видеостандарт</param>
        public void SetVideoStandard(AnalogVideoStandard VideoStandard)
        {
            if (device != null) device.SetVideoStandard(VideoStandard);
        }

        /// <summary>Установка цветового пространства</summary>
        /// <param name="subtype">Цветовое пространство</param>
        public void SetColorSpace(System.Guid subtype)
        {
            if (device != null) device.SetColorSpace(subtype);
        }

        /// <summary>Наименование видеоканала</summary>
        public string Name
        {
            get { return FName; }
            set { FName = value; }
        }

        #region Video parameters

        /// <summary>Установка яркости в процентах</summary>
        /// <param name="value">Значение яркости в процентах</param>
        protected override void SetBrightness(int value)
        {
            // Поверка значения яркости
            if (value < 0 && value > 100) return;
            // Установка значения яркости на камере
            if (device != null) device.SetBrightness(value);
            base.SetBrightness(value);
        }

        /// <summary>Установка контрастности в процентах</summary>
        /// <param name="value">Значение контрастности в процентах</param>
        protected override void SetContrast(int value)
        {
            // Поверка значения контрастности
            if (value < 0 && value > 100) return;
            // Установка значения контрастности на камере
            if (device != null) device.SetContrast(value);
            base.SetContrast(value);
        }

        /// <summary>Установка оттенка в процентах</summary>
        /// <param name="value">Значение оттенка в процентах</param>
        protected override void SetHue(int value)
        {
            // Поверка значения цветности
            if (value < 0 && value > 100) return;
            // Установка значения цветности на камере
            if (device != null) device.SetHue(value);
            base.SetHue(value);
        }

        /// <summary>Установка насыщенности в процентах</summary>
        /// <param name="value">Значение насыщенности в процентах</param>
        protected override void SetSaturation(int value)
        {
            // Поверка значения цветности
            if (value < 0 && value > 100) return;
            // Установка значения цветности на камере
            if (device != null) device.SetSaturation(value);
            base.SetSaturation(value);
        }

        /// <summary>Установка четкости в процентах</summary>
        /// <param name="value">Значение четкости в процентах</param>
        protected override void SetSharpness(int value)
        {
            // Поверка значения резкости
            if (value < 0 && value > 100) return;
            // Установка значения резкости на камере
            if (device != null) device.SetSharpness(value);
            base.SetSharpness(value);
        }

        /*
        /// <summary>Установка гаммы в процентах</summary>
        /// <param name="value">Значение гаммы в процентах</param>
        protected override void SetGamma(int value)
        {
            // Поверка значения резкости
            if (value < 0 && value > 100) return;
            // Установка значения резкости на камере
            if (device != null) device.SetGamma(value);
            base.SetSharpness(value);
        }
        */ 

        #endregion

        /// <summary>Подключение к камере</summary>
        private bool Connect()
        {
            AnalogCameraSettings settings = Settings as AnalogCameraSettings;
            if (settings == null) return false;
            SetLocalParameters(settings);
            try
            {
                //Создание объекта видеозахвата
                device = new CapDevice(settings.Device, settings.Channel, 
                                       settings.ResolutionX, settings.ResolutionY,
                                       settings.VideoStandard, settings.ColorSpace);
                //Подписка на событие получения кадра
                device.NewFrameArrived += new CapDevice.DelegateEventNewFrame(NewFrameArrived);
                // Подключение к камере
                device.Start();
            }
            catch { return false; }
            return true;
        }

        /// <summary>Отключение</summary>
        private void Disconnect()
        {
            device.Stop();
            device.NewFrameArrived -= NewFrameArrived;
            device.Dispose();
            device = null;
        }

        public static int GetCaptureDevicesCount()
        {
            return CapDevice.DeviceMonikes.Length;
        }

        public static string GetCaptureDeviceName(int i)
        {
            return CapDevice.DeviceMonikes[i].Name;
        }

        private void NewFrameArrived(byte[] frame)
        {
            try
            {
                RaiseOnCameraFrame(this, frame);
            }
            catch { };
        }

    }
}
