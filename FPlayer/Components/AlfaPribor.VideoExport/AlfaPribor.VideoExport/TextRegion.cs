using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AlfaPribor.VideoExport
{
    /// <summary>Описывает регион, представленный текстовой надписью, заданной свойством Text</summary>
    public class TextRegion : BaseGraphicRegion
    {
        #region Fields

        /// <summary>Графическая кисть для заливки фона текста</summary>
        private Brush _Background;

        /// <summary>Строка текста</summary>
        private string _Text;

        /// <summary>Шрифт текста</summary>
        private Font _Font;

        /// <summary>Графическая кисть для рисования текста</summary>
        private Brush _Brush;

        /// <summary>Формат расположения текста</summary>
        private StringFormat _Format;

        #endregion

        #region Methods

        /// <summary>Конструктор класса.
        /// Инициализирует свойства объекта класса занчениями по умолчанию
        /// </summary>
        public TextRegion()
        {
            SetDefault();
        }

        /// <summary>Конструктор класса.
        /// Инициализирует свойства объекта класса границами области, которой будет представлен объект, и
        /// текстом надписи
        /// </summary>
        /// <param name="bounds">Границы области, представленной текущим объектом</param>
        /// <param name="text">Текст надписи</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или строка текста
        /// </exception>
        public TextRegion(Region bounds, string text)
            : base(bounds)
        {
            SetDefault();
            Text = text;
        }

        /// <summary>Конструктор класса.
        /// Инициализирует свойства объекта класса границами области, которой будет представлен объект,
        /// номером слоя и текстом надписи
        /// </summary>
        /// <param name="bounds">Границы области, представленной текущим объектом</param>
        /// <param name="layer">Номер слоя, в котором расположен объект</param>
        /// <param name="text">Текст надписи</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или строка текста
        /// </exception>
        public TextRegion(Region bounds, int layer, string text)
            : base(bounds, layer)
        {
            SetDefault();
            Text = text;
        }

        /// <summary>Конструктор класса.
        /// Инициализирует свойства объекта класса границами области, которой будет представлен объект,
        /// номером слоя и текстом надписи
        /// </summary>
        /// <param name="bounds">Границы области, представленной текущим объектом</param>
        /// <param name="layer">Номер слоя, в котором расположен объект</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="text">Текст надписи</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// идентификатор объекта или строка текста
        /// </exception>
        public TextRegion(Region bounds, int layer, string id, string text)
            : base(bounds, layer, id)
        {
            SetDefault();
            Text = text;
        }

        /// <summary>Конструктор класса.
        /// Инициализирует свойства объекта класса границами области, которой будет представлен объект,
        /// текстом надписи, форматом текста, форматом изображения текста и графической кистью,
        /// которой будет отрисовываться текст
        /// </summary>
        /// <param name="bounds">Границы области, представленной текущим объектом</param>
        /// <param name="text">Текст надписи</param>
        /// <param name="font">Шрифт, которым будет отрисовываться текст</param>
        /// <param name="brush">Графическая кисть для отрисовки текста</param>
        /// <param name="format">Формат текста</param>
        /// <exception cref="System.ArgumentNullException">Одному или нескольким аргументам не присвоено значение</exception>
        public TextRegion(Region bounds, string text, Font font, Brush brush, StringFormat format)
            : base(bounds)
        {
            SetDefault();
            Text = text;
            Font = font;
            Brush = brush;
            Format = format;
        }

        /// <summary>Конструктор класса</summary>
        /// <param name="bounds">Границы области, представленной текущим объектом</param>
        /// <param name="layer">Номер слоя, в котором размещен текущий объект</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="text">Текст надписи</param>
        /// <param name="font">Шрифт, которым будет отрисовываться текст</param>
        /// <param name="brush">Графическая кисть для отрисовки текста</param>
        /// <param name="format">Формат расположения текста</param>
        /// <param name="background">Графическая кисть для фона</param>
        /// <exception cref="System.ArgumentNullException">Одному или нескольким аргументам не присвоено значение</exception>
        public TextRegion(Region bounds, int layer, string id, string text, Font font, Brush brush, StringFormat format, Brush background)
            : base(bounds, layer, id)
        {
            Text = text;
            Font = font;
            Brush = brush;
            Format = format;
            Background = background;
        }

        /// <summary>Конструктор класса. Задает границы объекта и номер слоя</summary>
        /// <param name="bounds">Границы объекта</param>
        /// <param name="layer">Номер слоя, к которому относится объект</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом</exception>
        public TextRegion(Region bounds, int layer)
            : base(bounds, layer) 
        { 
            SetDefault();
        }

        /// <summary>Конструктор класса. Задает границы объекта в соответствии с заданными</summary>
        /// <param name="bounds">Границы объекта</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом</exception>
        public TextRegion(Region bounds) 
            : base(bounds) 
        {
            SetDefault();
        }

        /// <summary>Присваивает свойствам объекта значения по умолчанию</summary>
        private void SetDefault()
        {
            _Background = null;
            _Text = string.Empty;
            _Font = new Font(FontFamily.GenericSansSerif, 8.0F);
            _Brush = new SolidBrush(Color.Black);
            _Format = new StringFormat();
        }

        /// <summary>Отображает объект на указанной графической поверхности</summary>
        /// <param name="surface">Поверхность для рисования</param>
        /// <param name="region">Область на поверхности surface, разрешенная для рисования</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект</exception>
        protected override void Draw(Graphics surface, Region region)
        {
            if (surface == null)
            {
                throw new ArgumentNullException("surface");
            }
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }
            if (region.IsEmpty(surface))
            {
                return;
            }
            RectangleF sourceRect = region.GetBounds(surface);
            if (!region.IsVisible(sourceRect))
            {
                return;
            }
            surface.Clip = region;
            if (_Background != null)
            {
                surface.FillRegion(_Background, region);
            }
            if (!string.IsNullOrEmpty(_Text))
            {
                surface.DrawString(_Text, _Font, _Brush, sourceRect, _Format);
            }
        }

        /// <summary>Освобождает используемые объектом ресурсы</summary>
        /// <param name="disposing">TRUE - освободить все используемые объектом ресурсы, FALSE - освободить только неуправляемые ресурсы</param>
        protected override void Dispose(bool disposing)
        {
            if (this.Disposed)
            {
                return;
            }
            if (_Background != null)
            {
                _Background.Dispose();
                _Background = null;
            }
            if (_Font != null)
            {
                _Font.Dispose();
                _Font = null;
            }
            if (_Brush != null)
            {
                _Brush.Dispose();
                _Brush = null;
            }
            if (_Format != null)
            {
                _Format.Dispose();
                _Format = null;
            }
            if (disposing)
            {
                _Text = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>Копирует объект, создавая его точную независимую копию</summary>
        /// <returns>Независимая копия объекта</returns>
        public override object Clone()
        {
            TextRegion result = (TextRegion)base.Clone();
            result._Background = (Brush)_Background.Clone();
            result._Brush = (Brush)_Brush.Clone();
            result._Font = (Font)_Font.Clone();
            result._Format = (StringFormat)_Format.Clone();
            result._Text = (string)_Text.Clone();
            return result;
        }
        #endregion

        #region Properties

        /// <summary>Графическая кисть для заливки фона текста</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public Brush Background
        {
            get 
            {
                CheckDisposed();
                return _Background; 
            }
            set 
            {
                CheckDisposed();
                _Background = value; 
            }
        }

        /// <summary>Строка текста</summary>
        /// <exception cref="System.ArgumentNullException">Не задана строка с текстом</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public string Text
        {
            get 
            {
                CheckDisposed();
                return _Text; 
            }
            set 
            {
                CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("Text");
                }
                _Text = value; 
            }
        }

        /// <summary>Шрифт текста</summary>
        /// <exception cref="System.ArgumentNullException">Не задано значение значение свойства</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public Font Font
        {
            get 
            {
                CheckDisposed();
                return _Font; 
            }
            set
            {
                CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("Font");
                }
                _Font = value;
            }
        }

        /// <summary>Графическая кисть для рисования текста</summary>
        /// <exception cref="System.ArgumentNullException">Не задано значение значение свойства</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public Brush Brush
        {
            get 
            {
                CheckDisposed();
                return _Brush; 
            }
            set
            {
                CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("Brush");
                }
                _Brush = value;
            }
        }

        /// <summary>Формат расположения текста</summary>
        /// <exception cref="System.ArgumentNullException">Не задано значение значение свойства</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public StringFormat Format
        {
            get 
            {
                CheckDisposed();
                return _Format; 
            }
            set
            {
                CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("Format");
                }
                _Format = value;
            }
        }

        #endregion
    }
}
