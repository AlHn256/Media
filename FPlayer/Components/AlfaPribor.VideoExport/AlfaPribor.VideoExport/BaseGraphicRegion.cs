using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AlfaPribor.VideoExport
{
    /// <summary>Описывает объект, способный отображать себя на графической поверхности </summary>
    public abstract class BaseGraphicRegion : IDisposable, ICloneable
    {
        #region Fields

        /// <summary>Границы области, занимаемой объектом</summary>
        private Region _Bounds;

        /// <summary>Номер слоя, к которому относится область</summary>
        private int _Layer;

        /// <summary>Идентификатор объекта</summary>
        private string _Id;

        /// <summary>Признак видимости графического объекта на канве рисования</summary>
        private bool _Visible;

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Задает границы объекта, номер слоя и идентификатор</summary>
        /// <param name="bounds">Границы объекта</param>
        /// <param name="layer">Номер слоя, к которому относится объект</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или идентификатор объекта
        /// </exception>
        public BaseGraphicRegion(Region bounds, int layer, string id)
            : this(bounds, layer, id, true) { }

        /// <summary>Конструктор класса. Задает границы объекта, номер слоя, идентификатор и видимость объекта</summary>
        /// <param name="bounds">Границы объекта</param>
        /// <param name="layer">Номер слоя, к которому относится объект</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="visible">Видимость объекта</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или идентификатор объекта
        /// </exception>
        public BaseGraphicRegion(Region bounds, int layer, string id, bool visible)
        {
            Bounds = bounds;
            _Layer = layer;
            Id = id;
            _Visible = visible;
        }

        /// <summary>Конструктор класса. Задает границы объекта и номер слоя</summary>
        /// <param name="bounds">Границы объекта</param>
        /// <param name="layer">Номер слоя, к которому относится объект</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или идентификатор объекта
        /// </exception>
        public BaseGraphicRegion(Region bounds, int layer)
            : this(bounds, layer, string.Empty) { }

        /// <summary>Конструктор класса. Задает границы объекта в соответствии с заданными</summary>
        /// <param name="bounds">Границы объекта</param>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом,
        /// или идентификатор объекта
        /// </exception>
        public BaseGraphicRegion(Region bounds) 
            : this(bounds, 0, string.Empty) { }

        /// <summary>Конструктор класса. Создает пустой объект с нулевыми границами</summary>
        public BaseGraphicRegion()
            : this(new Region(), 0, string.Empty) { }

        /// <summary>Деструктор объектов класса. Освобождает неуправляемые ресурсы, используемые объектом</summary>
        ~BaseGraphicRegion()
        {
            // Освобождаем только неуправляемые ресурсы объекта
            Dispose(false);
        }

        /// <summary>Отображает объект на указанной графической поверхности в заданной области</summary>
        /// <param name="surface">Поверхность для рисования</param>
        /// <param name="region">Область на поверхности surface, отведенная для рисования</param>
        /// <exception cref="System.ArgumentNullException">Должно генерироваться, если не задан один из параметров</exception>
        protected abstract void Draw(Graphics surface, Region region);

        /// <summary>Отображает объект на указанной графической поверхности</summary>
        /// <param name="surface">Поверхность для рисования</param>
        /// <exception cref="System.ArgumentNullException">Возникает в случае, если не задана поверхность для рисования</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public void Draw(Graphics surface)
        {
            CheckDisposed();
            using (Region bounds = _Bounds.Clone())
            {
                Draw(surface, bounds);
            }
            
        }

        #endregion

        #region Properties

        /// <summary>Границы области, занимаемой объектом</summary>
        /// <exception cref="System.ArgumentNullException">Не заданы границы области, занимаемой объектом</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public Region Bounds
        {
            get 
            {
                CheckDisposed();
                return _Bounds;
            }
            set 
            {
                CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("Bounds");
                }
                _Bounds = value; 
            }
        }

        /// <summary>Номер слоя, к конорому относится область</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        /// <remarks>Слой с наименьшим номером отрисовывается самым последним, следовательно, он перекрывает все остальные слои</remarks>
        public int Layer
        {
            get 
            {
                CheckDisposed();
                return _Layer; 
            }
            set 
            {
                CheckDisposed();
                _Layer = value; 
            }
        }

        /// <summary>Идентификатор объекта</summary>
        /// <exception cref="System.ArgumentNullException">Не задан идентификатор объекта</exception>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public string Id
        {
            get 
            {
                CheckDisposed();
                return _Id; 
            }
            set 
            {
                CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("Id");
                }
                _Id = value; 
            }
        }

        /// <summary>Признак видимости объекта на графической поверхности рисования</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        public bool Visible
        {
            get 
            {
                CheckDisposed();
                return _Visible;
            }
            set
            {
                CheckDisposed();
                _Visible = value;
            }
        }

        #endregion

        #region Члены IDisposable

        /// <summary>Признак освобождения ресурсов объекта</summary>
        private bool _Disposed = false;

        /// <summary>Освобождает управляемые и неуправляемые ресурсы объекта</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Освобождает используемые объектом ресурсы</summary>
        /// <param name="disposing">TRUE - освободить все используемые объектом ресурсы, FALSE - освободить только неуправляемые ресурсы</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
            {
                return;
            }
            if (_Bounds != null)
            {
                _Bounds.Dispose();
                _Bounds = null;
            }
            if (disposing)
            {
                _Id = null;
                _Layer = 0;
                _Visible = false;
            }
            _Disposed = true;
        }

        /// <summary>TRUE - ресурсы объекта освобождены, FALSE - в противном случае</summary>
        protected bool Disposed
        {
            get { return _Disposed; }
        }

        /// <summary>Проверяет факт освобождения ресурсов, занимаемых объектом</summary>
        /// <exception cref="System.ObjectDisposedException">Ресурсы объекта освобождены. Дальнейшее использование объекта невозможно</exception>
        protected void CheckDisposed()
        {
            if (_Disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
        }

        #endregion

        #region Члены ICloneable

        /// <summary>Копирует объект, создавая его точную независимую копию</summary>
        /// <returns>Независимая копия объекта</returns>
        public virtual object Clone()
        {
            BaseGraphicRegion result = (BaseGraphicRegion)this.MemberwiseClone();
            result._Bounds = _Bounds.Clone();
            result._Id = (string)_Id.Clone();
            return result;
        }

        #endregion
    }
}
