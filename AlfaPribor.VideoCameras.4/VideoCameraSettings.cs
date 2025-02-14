using System;
using System.Collections.Generic;
using System.Text;
using AnalogNamespace;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Базовые настройки</summary>
    public class VideoCameraSettings
    {

        int _Id;
        string _Name;
        VideoCameraType _Type;
        string _Model;

        /// <summary>Конструктор</summary>
        public VideoCameraSettings()
        {
            _Id = 0;
            _Name = string.Empty;
            _Type = VideoCameraType.Unknown;
            _Model = string.Empty;
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор камеры (номер канала)</param>
        /// <param name="name">Наименование камеры</param>
        /// <param name="type">Тип камеры</param>
        /// <param name="model">Модель</param>
        public VideoCameraSettings(int id, string name, VideoCameraType type, string model)
        {
            _Id = id;
            _Name = name;
            _Type = type;
            _Model = model;
        }

        #region Общие свойства

        /// <summary>Идентификатор камеры (номер канала) </summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Наименование камеры </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>Тип камеры</summary>
        public VideoCameraType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        /// <summary>Модель камеры (опционально)</summary>
        public string Model
        {
            get { return _Model; }
            set { _Model = value; }
        }

        #endregion

    }

    /// <summary>Настройки для аналоговой камеры</summary>
    public class AnalogCameraSettings : VideoCameraSettings
    {

        int _Channel;
        int _Device;
        int _ResolutionX;
        int _ResolutionY;
        AnalogVideoStandard _VideoStandard;
        System.Guid _ColorSpace;

        /// <summary>Конструктор</summary>
        public AnalogCameraSettings() : base()
        {
            _Channel = 0;
            _Device = 0;
            _VideoStandard = AnalogVideoStandard.PAL_D;
            _ColorSpace = MediaSubType.YUY2;
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор камеры</param>
        /// <param name="name">Наименование</param>
        /// <param name="type">Тип камеры</param>
        /// <param name="model">Модель</param>
        /// <param name="device">Номер видеоустройства</param>
        /// <param name="channel">Номер видеоканала</param>
        public AnalogCameraSettings(int id, string name, VideoCameraType type, string model, 
                                    int device, int channel) : base(id, name, type, model)
        {
            _Channel = channel;
            _Device = device;
            _VideoStandard = AnalogVideoStandard.PAL_D;
            _ColorSpace = MediaSubType.YUY2;
        }

        /// <summary>Номер видеоустройства</summary>
        public int Device
        {
            get { return _Device; }
            set { _Device = value; }
        }

        /// <summary>Номер видеоканала</summary>
        public int Channel
        {
            get { return _Channel; }
            set
            {
                if (value < 0) return;
                if (value > 3) return;
                _Channel = value;
            }
        }

        /// <summary>Ширина кадра</summary>
        public int ResolutionX
        {
            get { return _ResolutionX; }
            set { _ResolutionX = value; }
        }

        /// <summary>Высота кадра</summary>
        public int ResolutionY
        {
            get { return _ResolutionY; }
            set { _ResolutionY = value; }
        }

        /// <summary>Видеостандарт</summary>
        public AnalogVideoStandard VideoStandard
        {
            get { return _VideoStandard; }
            set { _VideoStandard = value; }
        }

        /// <summary>Цветовое пространство</summary>
        public System.Guid ColorSpace
        {
            get { return _ColorSpace; }
            set { _ColorSpace = value; }
        }

    }

    /// <summary>Настройки для IP камеры</summary>
    public class IpCameraSettings : VideoCameraSettings
    {
        string _Address;
        int _Channel = 1;
        string _UserName;
        string _Password;
        string _Resolution;
        int _FPS;
        int _Compression;
        int _Port;

        /// <summary>Конструктор класса по умолчанию</summary>
        public IpCameraSettings() : base()
        {
            _Address = string.Empty;
            _UserName = string.Empty;
            _Password = string.Empty;
            _Resolution = string.Empty;
            _FPS = 25;
            _Compression = 30;
        }

        /// <summary>Конструктор класса с параметрами</summary>
        /// <param name="id">Идентификатор телекамеры</param>
        /// <param name="name">Название видеоканала</param>
        /// <param name="type">Тип телекамеры</param>
        /// <param name="model">Название модели телекамеры</param>
        /// <param name="address">Сетевой адрес телекамеры</param>
        /// <param name="user_name">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        public IpCameraSettings(int id, string name, VideoCameraType type, string model, string address,
            string user_name, string password) : base(id, name, type, model)
        {
            _Address = address;
            _UserName = user_name;
            _Password = password;
            _Resolution = "640x480";
            _FPS = 25;
            _Compression = 50;
        }

        /// <summary>IP адрес или хост</summary>
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        /// <summary>Номер входа видеоканала (1..4)</summary>
        public int Channel
        {
            get { return _Channel; }
            set { if (value >= 1 && value <= 4) _Channel = value; }
        }

        /// <summary>Имя пользователя</summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// <summary>Пароль</summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        /// <summary>Разрешение</summary>
        public string Resolution
        {
            get { return _Resolution; }
            set { _Resolution = value; }
        }

        /// <summary>Темп передачи</summary>
        public int FPS
        {
            get { return _FPS; }
            set { _FPS = value; }
        }

        /// <summary>Компрессия кадра</summary>
        public int Compression
        {
            get { return _Compression; }
            set { _Compression = value; }
        }
        public int Port { 
            get { return _Port; } 
            set { _Port=value; } 
        }

    }

}
