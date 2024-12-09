using System;
using System.Collections.Generic;
using AlfaPribor.Collections;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Класс описывает содержимое видеопотока (или отделых видеокадров)</summary>
    [Serializable]
    [DataContract]
    public class VideoContent : IEquatable<VideoContent>
    {
        #region Fields

        /// <summary>Тип видеоданных</summary>
        private VideoContentType _Type;

        /// <summary>Ширина кадра в пикселах</summary>
        private int _Width;

        /// <summary>Высота кадра в пикселах</summary>
        private int _Height;

        /// <summary>Угол поворота изображения</summary>
        private int _Rotation;

        #endregion

        #region Methods

        /// <summary>Инициирует объект значениями, заданными в строке value</summary>
        /// <param name="value">Значения свойств объекта</param>
        public VideoContent(string value)
        {
            Reset();
            Parse(value);
        }

        /// <summary>Инициализирует объект значением типа содержимого видеоданных</summary>
        /// <param name="type">Тип содержимого видеоданных</param>
        public VideoContent(VideoContentType type)
        {
            _Type = type;
        }

        /// <summary>Инициализирует объект значением типа содержимого видеоданных и характеристиками видеокадра</summary>
        /// <param name="type">Тип содержимого видеоданных</param>
        /// <param name="width">Ширина видеокадра в пикселах</param>
        /// <param name="height">Высота видеокадра в пикселах</param>
        public VideoContent(VideoContentType type, int width, int height)
        {
            _Type = type;
            _Width = width;
            _Height = height;
        }

        /// <summary>Инициализирует объект значением типа содержимого видеоданных и характеристиками видеокадра</summary>
        /// <param name="type">Тип содержимого видеоданных</param>
        /// <param name="width">Ширина видеокадра в пикселах</param>
        /// <param name="height">Высота видеокадра в пикселах</param>
        /// <param name="rotation">Угол поворота изображения</param>
        public VideoContent(VideoContentType type, int width, int height, int rotation)
        {
            _Type = type;
            _Width = width;
            _Height = height;
            _Rotation = rotation;
        }

        /// <summary>Конструктор класса</summary>
        public VideoContent()
        {
            Reset();
        }

        /// <summary>Конструктор копирования</summary>
        /// <param name="other">Объект, свойства которого копируются</param>
        public VideoContent(VideoContent other)
        {
            _Type = other._Type;
            _Width = other._Width;
            _Height = other._Height;
            _Rotation = other._Rotation;
        }

        /// <summary>Считывает значения свойств из заданной строки</summary>
        /// <param name="properties">Имя параметра</param>
        public void Parse(string properties)
        {
            PropertiesDictionary props = new PropertiesDictionary(properties, ';');
            string value;
            if (props.TryGetValue("resolution", out value))
            {
                HandleResolution(value);
            }
            if (props.TryGetValue("rotation", out value))
            {
                HandleRotation(value);
            }
            HandleType(props);
         }

        /// <summary>Получить значение типа видеоданных</summary>
        /// <param name="properties">Коллекция строка со значениями свойств объекта</param>
        private void HandleType(PropertiesDictionary properties)
        {
            string value;
            if (properties.TryGetValue("image/jpeg", out value))
            {
                _Type = VideoContentType.jpeg;
            }
            else if (properties.TryGetValue("image/raw8", out value))
            {
                _Type = VideoContentType.raw8;
            }
            else if (properties.TryGetValue("image/raw16", out value))
            {
                _Type = VideoContentType.raw16;
            }
            else if (properties.TryGetValue("application/vnd.alpha.scan", out value))
            {
                _Type = VideoContentType.scan;
            }
        }

        /// <summary>Получить размер кадра видеоизображаения</summary>
        /// <param name="value">Строка с размером кадра видеоизображения</param>
        private void HandleResolution(string value)
        {
            _Width = 0;
            _Height = 0;
            int pos = value.IndexOf('x');
            if (pos >= 0)
            {
                try
                {
                    _Width = Int32.Parse(value.Substring(0, pos));
                    _Height = Int32.Parse(value.Substring(pos + 1));
                }
                catch { }
            }
        }

        /// <summary>Получить угол поворота видеоизображения</summary>
        /// <param name="value">Строка с углом поворота видеоизображения</param>
        private void HandleRotation(string value)
        {
            try
            {
                _Rotation = Int32.Parse(value);
            }
            catch
            {
                _Rotation = 0;
            }
        }

        /// <summary>Очистить значения свойств</summary>
        public void Reset()
        {
            _Type = VideoContentType.none;
            _Width = 0;
            _Height = 0;
            _Rotation = 0;
        }

        /// <summary>Возвращает строковое представленпие свойств объекта</summary>
        /// <returns></returns>
        public override string ToString()
        {
            PropertiesDictionary properties = new PropertiesDictionary(3);
            switch (_Type)
            {
                case VideoContentType.jpeg:
                    properties.Add("image/jpeg", "");
                    break;
                case VideoContentType.raw8:
                    properties.Add("image/raw8", "");
                    break;
                case VideoContentType.raw16:
                    properties.Add("image/raw16", "");
                    break;
                case VideoContentType.scan:
                    properties.Add("application/vnd.alpha.scan", "");
                    break;
            }
            if ((_Width != 0) || (_Height != 0))
            {
                properties.Add("resolution", _Width.ToString() + "x" + _Height.ToString());
            }
            if (_Rotation != 0)
            {
                properties.Add("rotation", _Rotation.ToString());
            }
            return properties.ToString();
        }

        #endregion

        #region Properties

        /// <summary>Тип видеоданных</summary>
        [DataMember]
        public VideoContentType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        /// <summary>Ширина кадра в пикселах</summary>
        [DataMember]
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        /// <summary>Высота кадра в пикселах</summary>
        [DataMember]
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        /// <summary>Угол поворота изображения</summary>
        [DataMember]
        public int Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }

        #endregion

        #region Члены IEquatable<VideoContent>

        /// <summary>
        /// Определяет идентичность текущего объекта заданному
        /// </summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты идентичны, FALSE - в противном случае</returns>
        public bool Equals(VideoContent other)
        {
            if (other == null)
            {
                return false;
            }
            return
                _Type == other._Type &&
                _Width == other._Width &&
                _Height == other._Height &&
                _Rotation == other._Rotation;
        }

        #endregion
    }
}
