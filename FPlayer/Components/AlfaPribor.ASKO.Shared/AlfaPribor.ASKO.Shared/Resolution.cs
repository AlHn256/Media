using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace AlfaPribor.ASKO.Shared
{
    /// <summary>
    /// Класс разбора строки графического разрешения изображения
    /// </summary>
    public class Resolution : ICloneable
    {
        /// <summary>
        /// Ширина графического изображения (точек)
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота графического изображения (точек)
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Конструктор объектов класса
        /// </summary>
        /// <param name="arg">Строка с графическим разрешением изображения</param>
        /// <exception cref="System.ArgumentException">Не задана строка с графическим расширением</exception>
        /// <exception cref="System.FormatException">Неверный формат строки</exception>
        /// <exception cref="System.OverflowException">Слишком большое значение ширины или высоты изображения</exception>
        public Resolution(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentException();
            }
            string[] parts = arg.Split(new Char[] { 'x', 'X', 'х', 'Х' });
            if (parts.Length != 2)
            {
                throw new FormatException();
            }
            try
            {
                Width = int.Parse(parts[0]);
                Height = int.Parse(parts[1]);
            }
            catch (ArgumentNullException)
            {
                throw new FormatException();
            }
        }
        /// <summary>
        /// Конструктор объектов класса
        /// </summary>
        public Resolution()
        {
            Width = 0;
            Height = 0;
        }
        /// <summary>
        /// Преобразует объект в строку
        /// </summary>
        /// <returns>Строковое представление графического разрешения</returns>
        public override string ToString()
        {
            return Width.ToString() + "x" + Height.ToString();
        }

        #region Члены ICloneable

        /// <summary>
        /// Создает полную копию объекта
        /// </summary>
        /// <returns>Копия текущего объекта</returns>
        public object Clone()
        {
            Resolution result = (Resolution)MemberwiseClone();
            return result;
        }

        #endregion
    }
}
