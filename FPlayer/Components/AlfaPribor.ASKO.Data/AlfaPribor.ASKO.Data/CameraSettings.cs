using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Параметры совместимости (что можно настраивать в камере)</summary>
    public struct CameraCompat
    {
        /// <summary>Яркость</summary>
        public bool Brightness;
        /// <summary>Контраст</summary>
        public bool Contrast;
        /// <summary>Оттенок</summary>
        public bool Hue;
        /// <summary>Насыщенность</summary>
        public bool Saturation;
        /// <summary>Гамма</summary>
        public bool Gamma;
        /// <summary>Резкость</summary>
        public bool Sharpness;
        /// <summary>Время экспозиции в мкс</summary>
        public bool Shutter;
        /// <summary>Степень сжатия</summary>
        public bool Compression;
        /// <summary>Список возможных разрешений (например: 1280x960;640x480;320x240)</summary>
        public string Resolutions;
        /// <summary>Темп передачи</summary>
        public bool FPS;
    }

    /// <summary>Настройки камеры</summary>
    public class CameraSettings : IEquatable<CameraSettings>
    {
        int _Id = 0;
        string _Name = string.Empty;
        string _IP = string.Empty;
        string _Login = string.Empty;
        string _Password = string.Empty;
        int _FPS = 0;
        bool _UseCorrection = false;
        int _CorrectRatio = 0;
        int _FieldOfView = 0;
        bool _Active = false;
        int _Protocol = 0;
        int _Compression = 30;
        int _Brightness = 50;
        int _Contrast = 50;
        int _Saturation = 50;
        int _Sharpness = 50;
        int _Gamma = 50;
        int _Hue = 50;
        int _Shutter = 4000;
        int _ResX = 640;
        int _ResY = 480;
        CameraCompat _compat;
        
        /// <summary>Конструктор класса. Инициализирует объект класса идентификатором камеры</summary>
        /// <param name="id">Идентификатор камеры (номер)</param>
        public CameraSettings(int id) {
            _Id = id;
        }

        /// <summary>Параметры совместимости (что можно настраивать в камере)</summary>
        public CameraCompat Compat
        {
            get { return _compat; }
            set { _compat = value; }
        }

        /// <summary>Идентификатор телекамеры</summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Название телекамеры</summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>Сетевой адрес видеокамеры</summary>
        public string IP
        {
            get { return _IP; }
            set { _IP = value; }
        }

        /// <summary>Имя пользователя для подключения к камере</summary>
        public string Login
        {
            get { return _Login; }
            set { _Login = value; }
        }

        /// <summary>Пароль для подключения к камере</summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        /// <summary>Скорость передачи кадров (кадров/сек)</summary>
        public int FPS
        {
            get { return _FPS; }
            set { _FPS = value; }
        }

        /// <summary>Выдержка (микросекунды)</summary>
        public int Shutter
        {
            get { return _Shutter; }
            set { _Shutter = value; }
        }

        /// <summary>Гамма</summary>
        public int Gamma
        {
            get { return _Gamma; }
            set { _Gamma = value; }
        }

        /// <summary>Оттенок</summary>
        public int Hue 
        {
            get { return _Hue; }
            set { _Hue = value; }
        }

        /// <summary>Признак использования коррекции изображений, получаемых с камеры</summary>
        public bool UseCorrection
        {
            get { return _UseCorrection; }
            set { _UseCorrection = value; }
        }

        /// <summary>Коэффициент коррекции искажений "рыбий глаз"(0..100)</summary>
        public int CorrectRatio
        {
            get { return _CorrectRatio; }
            set { _CorrectRatio = value; }
        }

        /// <summary>Поле зрения камеры (метры)</summary>
        public int FieldOfView
        {
            get { return _FieldOfView; }
            set { _FieldOfView = value; }
        }

        /// <summary>Разрешает взаимодействие программы с видеокамерой</summary>
        public bool Active
        {
            get { return _Active; }
            set { _Active = value; }
        }

        
        /// <summary>Версия протокола</summary>
        public int ProtocolVersion
        {
            get { return _Protocol; }
            set { _Protocol = value; }
        }

        /// <summary>Степень сжатия видеокадров (0..100)</summary>
        public int Compression
        {
            get { return _Compression; }
            set { _Compression = value; }
        }

        /// <summary>Величина яркости изображения с телекамеры (0..100)</summary>
        public int Brightness
        {
            get { return _Brightness; }
            set { _Brightness = value; }
        }

        /// <summary>Величина контрастности изображения с телекамеры (0..100)</summary>
        public int Contrast
        {
            get { return _Contrast; }
            set { _Contrast = value; }
        }

        /// <summary>Насыщенность цвета изображения с телекамеры (0..100)</summary>
        public int Saturation
        {
            get { return _Saturation; }
            set { _Saturation = value; }
        }

        /// <summary>Резкость изображения<summary>
        public int Sharpness
        {
            get { return _Sharpness; }
            set { _Sharpness = value; }
        }

        /// <summary>Получить горизонтальное разрешение</summary>
        /// <returns></returns>
        public int ResolutionX
        {
            get { return _ResX; }
            set { _ResX = value; }
        }

        /// <summary>Получить вертикальное разрешение</summary>
        /// <returns></returns>
        public int ResolutionY
        {
            get { return _ResY; }
            set { _ResY = value; }
        }

        #region IEquatable<CameraSettings> members

        /// <summary>Проверяет равенство текущего объекта заданному</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты равны, FALSE - в противном случае</returns>
        public bool Equals(CameraSettings other)
        {
            bool result = base.Equals(other);
            if (result)
            {
                result =
                    _Active == other._Active &&
                    _CorrectRatio == other._CorrectRatio &&
                    _Shutter == other._Shutter &&
                    _FieldOfView == other._FieldOfView &&
                    _FPS == other._FPS &&
                    _Gamma == other._Gamma &&
                    _IP == other._IP &&
                    _Login == other._Login &&
                    _Password == other._Password &&
                    _UseCorrection == other._UseCorrection &&
                    _Protocol == other._Protocol &&
                    _Compression == other._Compression &&
                    _Brightness == other._Brightness &&
                    _Contrast == other._Contrast &&
                    _Saturation == other._Saturation &&
                    _Sharpness == other._Sharpness;
            }
            return result;
        }

        #endregion

    }
}
