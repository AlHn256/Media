using System;
using System.Collections.Generic;
using System.Text;
using AnalogNamespace;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Класс аналоговый тепловизор</summary>
    public class ThermalCamera : VideoCameraBase
    {

        #region Variables

        /// <summary>Устройство трансляции</summary>
        CapDevice device;

        /// <summary>Идентификатор телекамеры</summary>
        int index;
        /// <summary>Номер устройства видеозахвата</summary>
        int FDevice = 0;
        /// <summary>Канал устройства видеозахвата</summary>
        int FChannel = 0;
        /// <summary>Название телекамеры</summary>
        string FModel;
        /// <summary>Название видеоканала</summary>
        string FName;
        /// <summary>Тип телекамеры</summary>
        VideoCameraType FType;

        /// <summary>Ширина кадра</summary>
        const int ResX = 352;
        /// <summary>Высота кадра</summary>
        const int ResY = 288;

        #endregion

        /// <summary>Конструктор класса аналогового тепловизора</summary>
        /// <param name="settings"></param>
        public ThermalCamera(AnalogCameraSettings settings) : base(settings)
        {
            SetLocalParameters(settings);
        }

        /// <summary>Присвоение локальный параметров</summary>
        /// <param name="settings">Параметры телекамеры</param>
        void SetLocalParameters(AnalogCameraSettings settings)
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
            compat.Brightness = false;
            compat.Contrast = false;
            compat.Hue = false;
            compat.Saturation = false;
            compat.Gamma = false;
            compat.Sharpness = false;
            compat.Shutter = false;
            compat.Compression = false;
            compat.FPS = false;
            compat.Resolutions = "352x288";
            compat.format = VideoCameraFormatFrame.Bitmap;
            return compat;
        }

        /// <summary>Подключение к камере</summary>
        bool Connect()
        {
            AnalogCameraSettings settings = Settings as AnalogCameraSettings;
            if (settings == null) return false;
            SetLocalParameters(settings);
            try
            {
                device = new CapDevice(settings.Device, settings.Channel,
                                       settings.ResolutionX, settings.ResolutionY,
                                       settings.VideoStandard, settings.ColorSpace);
                device.NewFrameArrived += new CapDevice.DelegateEventNewFrame(NewFrameArrived);
                device.OnThreadStart += new EventHandler(device_OnThreadStart);
                // Подключение к камере
                device.Start();
            }
            catch { return false; }
            return true;
        }

        void device_OnThreadStart(object sender, EventArgs e)
        {
            if (device != null)
            {
                //Настройки тепловизора
                //device.SetBrightness(60);
                //device.SetContrast(50);
                device.SetDefaultBrightness();
                device.SetDefaultContrast();
                device.SetHue(50);
                device.SetSaturation(0);    //!! Насыщенность на 0 для черно белого сингала с тепловизора
            }
        }

        /// <summary>Отключение</summary>
        void Disconnect()
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

        void NewFrameArrived(byte[] frame)
        {
            try
            {
                RaiseOnCameraFrame(this, frame);
            }
            catch { };
        }

    }
}
