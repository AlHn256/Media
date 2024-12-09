using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AlfaPribor.VideoExport
{
    /// <summary>Описывает регион, представленный графическим изображением, заданным свойством Image</summary>
    public class ImageRegion : BaseGraphicRegion
    {
        #region Fields

        /// <summary>Изображение, помещаемое в область с границами Bounds</summary>
        private Image _Image;

        /// <summary>Тип расположения изображения внутри региона</summary>
        private PictureBoxSizeMode _SizeMode;

        /// <summary>Графическая кисть для заливки фона изображения</summary>
        private Brush _Background;

        #endregion

        #region Methods

        /// <summary>Конструктор класса.
        /// Инициализирует свойства объекта класса значениями по умолчанию
        /// </summary>
        public ImageRegion()
        {
            SetDefault();
        }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса значением границ, изображением, которое будет помещено в регион, и
        /// типом размещения изображения внутри региона.
        /// </summary>
        /// <param name="bounds">Границы области кадра, представленной текущим объектом</param>
        /// <param name="image">Изображение, размещаемое внутри региона</param>
        /// <param name="mode">Способ размещения изображения внутри региона</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом</exception>
        public ImageRegion(Region bounds, Image image, PictureBoxSizeMode mode)
            : base(bounds)
        {
            Image = image;
            _SizeMode = mode;
            _Background = new SolidBrush(Color.Black);
        }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса значением границ, изображением, которое будет помещено в регион,
        /// типом размещения изображения внутри региона и номер слоя, в котором он расположен.
        /// </summary>
        /// <param name="bounds">Границы области кадра, представленной текущим объектом</param>
        /// <param name="layer">Номер слоя, в котором расположен объект</param>
        /// <param name="image">Изображение, размещаемое внутри региона</param>
        /// <param name="mode">Способ размещения изображения внутри региона</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом</exception>
        public ImageRegion(Region bounds, int layer, Image image, PictureBoxSizeMode mode)
            : base(bounds, layer)
        {
            Image = image;
            _SizeMode = mode;
            _Background = new SolidBrush(Color.Black);
        }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса значением границ, изображением, которое будет помещено в регион,
        /// типом размещения изображения внутри региона и номер слоя, в котором он расположен.
        /// </summary>
        /// <param name="bounds">Границы области кадра, представленной текущим объектом</param>
        /// <param name="layer">Номер слоя, в котором расположен объект</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="image">Изображение, размещаемое внутри региона</param>
        /// <param name="mode">Способ размещения изображения внутри региона</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или идентификатор объекта
        /// </exception>
        public ImageRegion(Region bounds, int layer, string id, Image image, PictureBoxSizeMode mode)
            : base(bounds, layer, id)
        {
            Image = image;
            _SizeMode = mode;
            _Background = new SolidBrush(Color.Black);
        }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса значением границ, изображением, которое будет помещено в регион,
        /// типом размещения изображения внутри региона и номер слоя, в котором он расположен.
        /// </summary>
        /// <param name="bounds">Границы области кадра, представленной текущим объектом</param>
        /// <param name="layer">Номер слоя, в котором расположен объект</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="image">Изображение, размещаемое внутри региона</param>
        /// <param name="mode">Способ размещения изображения внутри региона</param>
        /// <param name="background">Графическая кисть для заливки фона изображения</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или идентификатор объекта
        /// </exception>
        public ImageRegion(Region bounds, int layer, string id, Image image, PictureBoxSizeMode mode, Brush background)
            : base(bounds, layer, id)
        {
            Image = image;
            _SizeMode = mode;
            _Background = background;
        }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса значением границ, которые описывают регион, и номером слоя, в котором он расположен
        /// </summary>
        /// <param name="bounds">Границы области кадра, представленной текущим объектом</param>
        /// <param name="layer">Номер слоя, в котором расположен объект</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом</exception>
        public ImageRegion(Region bounds, int layer)
            : base(bounds, layer) 
        {
            SetDefault();
        }

        /// <summary>Конструктор класса. Задает границы объекта, номер слоя и идентификатор</summary>
        /// <param name="bounds">Границы объекта</param>
        /// <param name="layer">Номер слоя, к которому относится объект</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или идентификатор объекта
        /// </exception>
        public ImageRegion(Region bounds, int layer, string id)
            : base(bounds, layer, id)
        {
            SetDefault();
        }

        /// <summary>Конструктор класса. Задает границы объекта в соответствии с заданными</summary>
        /// <param name="bounds">Границы объекта</param>
        public ImageRegion(Region bounds)
            : base(bounds) 
        {
            SetDefault();
        }

        /// <summary>Присваивает свойствам объекта значения по умолчанию</summary>
        private void SetDefault()
        {
            _Image = null;
            _SizeMode = PictureBoxSizeMode.Normal;
            _Background = new SolidBrush(Color.Black);
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
            if (_SizeMode == PictureBoxSizeMode.AutoSize)
            {
                // Очищаем область для рисования
                surface.FillRegion(_Background, region);
                // Изменяем границы области в соответствии с размерами изображения
                sourceRect.Width = _Image.Width;
                sourceRect.Height = _Image.Height;
                region.Dispose();
                region = new Region(sourceRect);
            }
            surface.Clip = region;
            RectangleF imageRect = GetImageRect(surface, sourceRect);
            bool need_fill = true;
            if (_Image != null)
            {
                need_fill = (_Image.Height < sourceRect.Height) || (_Image.Width < sourceRect.Width);
            }
            if (need_fill)
            {
                using (Region backgroundRgn = region.Clone())
                {
                    backgroundRgn.Xor(imageRect);
                    if (!backgroundRgn.IsEmpty(surface))
                    {
                        surface.FillRegion(_Background, backgroundRgn);
                    }
                }
            }
            if (!imageRect.IsEmpty)
            {
                surface.PageUnit = GraphicsUnit.Pixel;
                surface.InterpolationMode = InterpolationMode.HighQualityBicubic;
                surface.DrawImage(_Image, imageRect);
            }
        }

        /// <summary>Вычисляем регион вывода изображения с учетом настроек по его размещению</summary>
        /// <param name="surface">Поверхность для рисования</param>
        /// <param name="sourceRect">Прямоугольная область, в которой нужно разместить изображение</param>
        /// <returns>Регион вывода изображения</returns>
        private RectangleF GetImageRect(Graphics surface, RectangleF sourceRect)
        {
            if (_Image == null)
            {
                return new RectangleF();
            }
            // выравниваем левый верхний угол изображенния и региона для его вывода
            RectangleF imgRect = new RectangleF(sourceRect.X, sourceRect.Y, _Image.Width, _Image.Height);
            Region imgRegion = null;
            try
            {
                switch (_SizeMode)
                {
                    case PictureBoxSizeMode.CenterImage:
                        // Центрируем область изображения относительно области вывода
                        imgRect.X += (sourceRect.Width - (float)_Image.Width) / 2.0F;
                        imgRect.Y += (sourceRect.Height - (float)_Image.Height) / 2.0F;
                        imgRegion = new Region(imgRect);
                        break;

                    case PictureBoxSizeMode.AutoSize:
                    case PictureBoxSizeMode.Normal:
                        imgRegion = new Region(imgRect);
                        break;

                    case PictureBoxSizeMode.StretchImage:
                        imgRegion = this.Bounds;
                        break;

                    case PictureBoxSizeMode.Zoom:
                        imgRegion = new Region(imgRect);
                        Matrix transformMatrix = new Matrix();
                        float k; // коэфф. масштабирования
                        if (sourceRect.Height < sourceRect.Width)
                        {
                            k = sourceRect.Height / (float)_Image.Height;
                        }
                        else
                        {
                            k = sourceRect.Width / (float)_Image.Width;
                        }
                        // масштабируем область изображения
                        transformMatrix.Scale(k, k);
                        imgRegion.Transform(transformMatrix);
                        // Центрируем область изображения относительно области вывода
                        imgRect = imgRegion.GetBounds(surface);
                        // При масштабировании могут поменяться координаты левого верхнего угла региона.
                        // Если регион сместился - передвигаем его на начальную позицию
                        imgRegion.Translate(
                            (sourceRect.X - imgRect.X),
                            (sourceRect.Y - imgRect.Y)
                        );
                        // ... и центрируем
                        imgRegion.Translate(
                            (sourceRect.Width - imgRect.Width) / 2.0F,
                            (sourceRect.Height - imgRect.Height) / 2.0F
                        );
                        break;

                    default:
                        imgRegion = new Region();
                        break;
                }
                return imgRegion.GetBounds(surface);
            }
            finally
            {
                if (imgRegion != null)
                {
                    imgRegion.Dispose();
                }
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
            if (_Image != null)
            {
                _Image.Dispose();
                _Image = null;
            }
            if (_Background != null)
            {
                _Background.Dispose();
                _Background = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>Копирует объект, создавая его точную независимую копию</summary>
        /// <returns>Независимая копия объекта</returns>
        public override object Clone()
        {
            ImageRegion result = (ImageRegion)base.Clone();
            if (_Image != null)
            {
                result._Image = (Image)_Image.Clone();
            }
            // при повторном клонировании возникает исключение ArgumentException
            result._Background = (Brush)_Background.Clone();
            return result;
        }
        #endregion

        #region Properties

        /// <summary>Изображение, помещаемое в область с границами Bounds</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public Image Image
        {
            get 
            {
                CheckDisposed();
                return _Image; 
            }
            set 
            {
                CheckDisposed();
                if (_Image != null)
                {
                    _Image.Dispose();
                }
                _Image = value; 
            }
        }

        /// <summary>Определяет, как располагается изображение внутри региона</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public PictureBoxSizeMode SizeMode
        {
            get 
            {
                CheckDisposed();
                return _SizeMode; 
            }
            set 
            {
                CheckDisposed();
                _SizeMode = value; 
            }
        }

        /// <summary>Графическая кисть для заливки фона изображения</summary>
        /// <exception cref="System.ArgumentNullException">Не задан фон изображения</exception>
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
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _Background = value; 
            }
        }

        #endregion
    }
}
