using System;
using System.Collections.Generic;
using AlfaPribor.Collections;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Информаци о видеопотоке</summary>
    [Serializable]
    public class VideoStreamInfo : IEquatable<VideoStreamInfo>
    {

        #region Fields

        /// <summary>Идентификатор видеопотока / номер камеры</summary>
        protected int _Id;

        /// <summary>Тип содержимого видеопотока (например image/jpeg)</summary>
        protected string _ContentType = string.Empty;

        /// <summary>Разрешение кадров видеопотока в виде строки (например 640x480)</summary>
        protected string _Resolution = string.Empty;

        /// <summary>Ширина кадра в пикселах</summary>
        protected int _Width = 0;

        /// <summary>Высота кадра в пикселах</summary>
        protected int _Height = 0;

        /// <summary>Угол поворота изображения, содержащегося в кадрах</summary>
        /// <remarks>Используется для видеоданных, получаемых с тепловизора</remarks>
        protected int _Rotation = 0;

        #endregion

        #region Methods

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор потока / номер камеры</param>
        /// <param name="content_type">Описатель типа потока (все что после Content-Type:)</param>
        public VideoStreamInfo(int id, string content_type)
        {
            _Id = id;
            VideoContent content = new VideoContent(content_type);
            _ContentType = "image/" + content.Type.ToString();
            _Width = content.Width;
            _Height = content.Height;
            if (_Width != 0 || _Height != 0) { _Resolution = _Width.ToString() + "x" + _Height.ToString(); }
            _Rotation = content.Rotation;
        }

        /// <summary>Конструктор копирования</summary>
        /// <param name="other">Объект, данные которого копируются</param>
        public VideoStreamInfo(VideoStreamInfo other)
        {
            _Id = other._Id;
            _ContentType = other._ContentType;
            _Resolution = other._Resolution;
            _Width = other._Width;
            _Height = other._Height;
            _Rotation = other._Rotation;
        }

        /// <summary>Сравнивает текущий объект с заданным по идентификационному номеру</summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>Возвращает TRUE если условие сравнения выполняется, в противном случае возвращает FALSE</returns>
        public bool EqualId(VideoStreamInfo obj)
        {
            return _Id == obj._Id;
        }

        /// <summary>Возвращает строковое представление объекта</summary>
        /// <returns>Строка с перечислением значений свойств объекта</returns>
        public override string ToString()
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(_ContentType)) { result += _ContentType + ";"; }
            if (!string.IsNullOrEmpty(_Resolution)) { result += " resolution=" + _Resolution + ";"; }
            if (_Rotation != 0) { result += " rotation=" + _Rotation.ToString(); }
            if (result[result.Length-1] == ';') { result = result.Substring(0, result.Length - 1); }
            return result;
        }

        #endregion

        #region Properties

        /// <summary>Идентификатор видеопотока / номер камеры</summary>
        public int Id { get { return _Id; } }

        /// <summary>Тип содержимого (например image/jpeg)</summary>
        public string ContentType { get { return _ContentType; } }

        /// <summary>Разрешение в виде строки (например 640x480)</summary>
        public string Resolution { get { return _Resolution; } }

        /// <summary>Ширина изображения в пикселях</summary>
        public int Width { get { return _Width; } }

        /// <summary>Высота изображения в пикселях</summary>
        public int Height { get { return _Height; } }

        /// <summary>Угол поворота изображения</summary>
        public int Rotation { get { return _Rotation; } }

        #endregion

        #region Члены IEquatable<VideoStreamInfo>

        /// <summary>Определяет идентичность текущего объекта заданному</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты идентичны, FALSE - в противном случае</returns>
        public bool Equals(VideoStreamInfo other)
        {
            if (other == null) return false;
            return
                _Id == other._Id &&
                _ContentType == other._ContentType &&
                _Width == other._Width &&
                _Height == other._Height &&
                _Rotation == other._Rotation;
        }

        #endregion
    }
}
