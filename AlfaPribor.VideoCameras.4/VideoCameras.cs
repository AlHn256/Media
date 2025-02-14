using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Класс "Камеры". Версия 3</summary>
    public class VideoCameras : IDisposable
    {

        /// <summary>Флаг "активен"</summary>
        bool active;
        /// <summary>Список камер</summary>
        List<VideoCameraBase> _Cameras;
        /// <summary>Объект синхронизации</summary>
        object _SyncRoot = new object();

        /// <summary>Извещение об изменении статуса камеры</summary>
        public event EvCameraStatus OnCameraStatus;
        /// <summary>Извещение о получении очередного кадра</summary>
        public event EvCameraFrame OnCameraFrame;
        /// <summary>Извещение расширенное о получении очередного кадра (с указанием разрешения кадра)</summary>
        public event EvCameraFrameEx OnCameraFrameEx;

        /// <summary>Конструктор класса</summary>
        public VideoCameras()
        {
            active = false;
            _Cameras = new List<VideoCameraBase>();
        }

        /// <summary>Добавить телекамеру</summary>
        /// <param name="camera">Класс "Камера"</param>
        public void Add(VideoCameraBase camera)
        {
            if (GetCamera(camera.Id) != null) return;
            camera.OnCameraStatus += new EvCameraStatus(camera_OnCameraStatus);
            camera.OnCameraFrame += new EvCameraFrame(camera_OnCameraFrame);
            _Cameras.Add(camera);
            if (active == true) camera.Active = true;
        }

        /// <summary>Удалить телекамеру из коллекции</summary>
        /// <param name="camera">Класс "Камера"</param>
        public void Remove(VideoCameraBase camera)
        {
            if (GetCamera(camera.Id) == null) return;
            camera.Active = false;
            camera.OnCameraStatus -= camera_OnCameraStatus;
            camera.OnCameraFrame -= camera_OnCameraFrame;
            _Cameras.Remove(camera);
        }

         /// <summary>Очистить список камер</summary>
        public void Clear()
        {
            Active = false;
            foreach (VideoCameraBase camera in _Cameras)
            {
                camera.OnCameraStatus -= new EvCameraStatus(camera_OnCameraStatus);
                camera.OnCameraFrame -= new EvCameraFrame(camera_OnCameraFrame);
                camera.Dispose();
            }
            _Cameras.Clear();
        }

        /// <summary>Число телекамер в списке</summary>
        public int Count { get { return _Cameras.Count; } }

        /// <summary>Объект синхронизации доступа из разных потоков</summary>
        public object SyncRoot { get { return _SyncRoot; } }

        /// <summary>Получить интерфейс для управления камерой с указанным идентификатором</summary>
        /// <param name="id">Идентификатор телекамеры</param>
        /// <returns>Интерфейс телекамеры</returns>
        public IVideoCamera GetCamera(int id)
        {
            foreach (VideoCameraBase camera in _Cameras) { if (camera.Id == id) return camera; }
            return null;
        }

        /// <summary>Получить идентификатор телекамеры по номеру в коллекции</summary>
        /// <param name="sn">Номер в коллекции (c 0)</param>
        /// <returns>Иденификатор телекамеры</returns>
        public int GetCameraId(int sn)
        {
            if (sn >= _Cameras.Count) return -1;
            else return _Cameras[sn].Id;
        }

        /// <summary>Активность телекамеры</summary>
        public bool Active
        {
            get { return active; }
            set
            {
                if (active == value) return;
                foreach (VideoCameraBase camera in _Cameras) camera.Active = value;
                active = value;
            }
        }

        /// <summary>Обработчик события о приходе очередного кадра</summary>
        /// <param name="sender">Объект телекамеры</param>
        /// <param name="frame">Данные кадра</param>
        void camera_OnCameraFrame(object sender, byte[] frame)
        {
            //Событие кадра
            if (OnCameraFrame != null) OnCameraFrame(sender, frame);
            //Расширенное событие кадра
            if (OnCameraFrameEx != null)
            {
                //Определение разрешения
                string res = "640x480";
                if (((VideoCameraBase)sender).Settings.Type == VideoCameraType.IP)
                    res = ((IpCameraSettings)((VideoCameraBase)sender).Settings).Resolution;
                else 
                    if (((VideoCameraBase)sender).Settings.Type == VideoCameraType.Analog)
                        res = ((AnalogCameraSettings)((VideoCameraBase)sender).Settings).ResolutionX + "x" +
                              ((AnalogCameraSettings)((VideoCameraBase)sender).Settings).ResolutionY;
                OnCameraFrameEx(sender, frame, res);
            }
        }

        /// <summary>Обработчик события об изменении статуса</summary>
        /// <param name="sender">Объект телекамеры</param>
        /// <param name="status">Статус подключения</param>
        void camera_OnCameraStatus(object sender, VideoCameraStatus status)
        {
            if (OnCameraStatus != null) OnCameraStatus(sender, status);
        }

        #region Члены IDisposable

        public void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
