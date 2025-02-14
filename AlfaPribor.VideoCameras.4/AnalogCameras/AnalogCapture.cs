using System;
using System.Collections.Generic;
using System.Text;

namespace AnalogNamespace
{

    public enum CameraConnectionState
    {
        /// <summary>Неизвестно</summary>
        Unknown = 0,
        /// <summary>Активна</summary>
        Active = 1,
        /// <summary>Неактивна</summary>
        NonActive = 2
    }

    /// <summary>Общий класс для работы с аналоговым устройством видеозахвата</summary>
    public class AnalogCapture
    {

        #region Declaration

        #region Enums


        #endregion

        #region Variables

        /// <summary>Устройство трансляции</summary>
        CapDevice device;

        /// <summary>Идентификатор телекамеры</summary>
        private int index;
        /// <summary>Флаг трансляции кадра</summary>
        private bool TranslateVideo = false;
        /// <summary>Глобальный номер кадра</summary>
        private long FrameIndex;
        /// <summary>Флаг наличия видеосигнала</summary>
        private bool IsSignal = true;

        #endregion

        #region Events

        /// <summary>Состояние телекамеры</summary>
        /// <param name="sender">Объект телекамеры</param>
        /// <param name="state">Текущее состояние</param>
        public delegate void DelegateEventChangeState(object sender, int state);
        /// <summary>Состояние телекамеры</summary>
        public event DelegateEventChangeState EventChangeState;

        /// <summary>Событие нового кадра</summary>
        /// <param name="sender">Объект телекамеры</param>
        /// <param name="data">Jpeg данные кадра</param>
        /// <param name="index">Глобальный номер кадра</param>
        /// <param name="compression">Сжатие кадра</param>
        public delegate void DelegateEventNewFrame(object sender, byte[] frame, long index, bool compression);
        /// <summary>Событие нового кадра</summary>
        public event DelegateEventNewFrame EventNewFrame;

        /// <summary>Событие темпа воспроизведения</summary>
        /// <param name="sender">Объект телекамеры</param>
        /// <param name="FPS">Число кадров в секунду</param>
        public delegate void DelegateEventFPS(object sender, int FPS);
        /// <summary>Событие темпа воспроизведения</summary>
        public event DelegateEventFPS EventChangeFPS;

        /// <summary>Событие трансляции видео</summary>
        /// <param name="sender">Объект телекамеры</param>
        /// <param name="translate">Состояние трансляции</param>
        public delegate void DelegateEventOnVideo(object sender, bool translate);
        /// <summary>Событие трансляции видео</summary>
        public event DelegateEventOnVideo EventChangeOnVideo;

        /// <summary>Событие отсутствия/возобновления сигнала</summary>
        /// <param name="sender">Объект отправитель</param>
        /// <param name="IsSignal">Наличие сиглана</param>
        public delegate void DelegateEventNoSignal(object sender, bool IsSignal);
        /// <summary>Событие отсутствия/возобновления сигнала</summary>
        public event DelegateEventNoSignal EventNoSignal;

        #endregion

        #endregion Declaration

        #region Private

        /// <summary>Событие готовности буфера передачи кадров</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void device_OnNewBitmapReady(object sender, EventArgs e)
        {

        }

        private void device_NewFrameArrived(byte[] frame)
        {
            FrameIndex++;
            //bool s = Signal(frame);
            bool s = true;
            if (!s)
            {
                IsSignal = false;
                if (EventNoSignal != null) EventNoSignal(this, false);
            }
            if (s && !IsSignal)
            {
                IsSignal = true;
                if (EventNoSignal != null) EventNoSignal(this, false);
            }
            if (TranslateVideo)
                if (EventNewFrame != null) EventNewFrame(this, frame, FrameIndex, false);
        }

        private bool Signal(IntPtr frame)
        {
            int cnt = 0; //Счетчик "пустых" байт
            //Проверка связи по наличию периодичного синего (191, 1, 0)
            int[] arr = new int[3];
            System.Runtime.InteropServices.Marshal.Copy(frame, arr, 0, 3);
            if (arr[0] == 191 && arr[1] == 1 && arr[2] == 0)  cnt++;
            if (cnt == 20) return true;//Если длительное остутствие значащих пикселй
            return false;
        }

        #endregion

        #region Public

        /// <summary>Конструктор класса</summary>
        /// <param name="i">Индекс телекамеры</param>
        public AnalogCapture(int i)
        {
            index = i;
            FrameIndex = 0;
            IsSignal = true;
        }

        /// <summary>Установка параметров телекамеры</summary>
        /// <param name="dev_num">Номер устройства видеозахвата</param>
        /// <param name="input">Вход устройства видеозахвата</param>
        /// <param name="ResX">Ширина кадра</param>
        /// <param name="ResY">Высота кадра</param>
        public void SetParameters(int dev_num, int input, int ResX, int ResY)
        {
            device = new CapDevice(dev_num, input, ResX, ResY, );
            device.OnNewBitmapReady += new EventHandler(device_OnNewBitmapReady);
            device.NewFrameArrived += new CapDevice.DelegateEventNewFrame(device_NewFrameArrived);
        }

        /// <summary>Получение номера телекамеры (присвоенного при создании индекса))</summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>Подключение</summary>
        public void Connect()
        {
            if (device != null) device.Start();
        }

        /// <summary>Отключение</summary>
        public void Disconnect()
        {
            if (device != null) device.Stop();
        }

        /// <summary>Включение трансляции видео (выдачи кадра по событию)</summary>
        public void OnVideo()
        {
            TranslateVideo = true;
            if (EventChangeOnVideo != null) EventChangeOnVideo(this, TranslateVideo);
        }

        /// <summary>Выключение трансляции видео (выдачи кадра по событию)</summary>
        public void OffVideo()
        {
            TranslateVideo = false;
            if (EventChangeOnVideo != null) EventChangeOnVideo(this, TranslateVideo);
        }

        /// <summary>Установить яркость в процентах</summary>
        public void SetBrightness(int Value)
        {
            if (device != null) device.SetBrightness(Value);
        }

        /// <summary>Установить контраст в процентах</summary>
        public void SetContrast(int Value)
        {
            if (device != null) device.SetContrast(Value);
        }

        /// <summary>Установить насыщенность цвета в процентах</summary>
        public void SetColorLevel(int Value)
        {
            if (device != null) device.SetSaturation(Value);
        }

        /// <summary>Установить резкость в процентах</summary>
        public void SetSharpness(int Value)
        {
            if (device != null) device.SetSharpness(Value);
        }

        /// <summary>Установить цветность в процентах</summary>
        public void SetHue(int Value)
        {
            if (device != null) device.SetHue(Value);
        }

        /// <summary>Установить гамму в процентах</summary>
        public void SetGamma(int Value)
        {
            if (device != null) device.SetGamma(Value);
        }

        /// <summary>Установить вижеостандарт</summary>
        public void SetVideoStandard(AnalogVideoStandard Value)
        {
            if (device != null) device.SetVideoStandard(Value);
        }

        /// <summary>Установить цветовое пространство</summary>
        public void SetColorSpace(AnalogVideoStandard Value)
        {
            if (device != null) device.SetColorSpace(MediaSubType.YUY2);
        }

        #endregion

    }
}
