using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Shared
{
    /// <summary>Негабариты вагона</summary>

    public class NgbValue : IEquatable<NgbValue>
    {
        uint _Mask = 0;

        /// <summary>Нет негабаритов</summary>
        public static readonly uint None = 0x0000000;
        /// <summary>Левый нижний боковой</summary>
        public static readonly uint LeftBottom = 0x0000001;
        /// <summary>Левый первый</summary>
        public static readonly uint Left1 = 0x0000002;
        /// <summary>Левый второй</summary>
        public static readonly uint Left2 = 0x0000004;
        /// <summary>Левый третий</summary>
        public static readonly uint Left3 = 0x0000008;
        /// <summary>Верхний</summary>
        public static readonly uint Top = 0x0000010;
        /// <summary>Правый четвертый</summary>
        public static readonly uint Right4 = 0x0000020;
        /// <summary>Правый пятый</summary>
        public static readonly uint Right5 = 0x0000040;
        /// <summary>Правый шестой</summary>
        public static readonly uint Right6 = 0x0000080;
        /// <summary>Правый нижний боковой</summary>
        public static readonly uint RightBottom = 0x0000100;
        /// <summary>Левый подвижного состава</summary>
        public static readonly uint LeftPS = 0x0000200;
        /// <summary>Правый подвижного состава</summary>
        public static readonly uint RightPS = 0x0000400;
        /// <summary>Левый основной</summary>
        public static readonly uint LeftMain = 0x0000800;
        /// <summary>Правый основной</summary>
        public static readonly uint RightMain = 0x0001000;
        /// <summary>Все негабариты</summary>
        public static readonly uint All = 0x0001fff;

        /// <summary>Конструктор</summary>
        public NgbValue()
        {
        }

        /// <summary>Конструктор</summary>
        /// <param name="mask">Маска негабаритов</param>
        public NgbValue(uint mask)
        {
            Mask = mask;
        }

        /// <summary>Конструктор</summary>
        /// <param name="mask">Маска негабаритов</param>
        /// <param name="inverse">Инвертировать негабариты</param>
        public NgbValue(uint mask, bool inverse)
        {
            Mask = mask;
        }

        /// <summary>Маска негабаритов</summary>
        public uint Mask
        {
            get { return _Mask; }
            set
            {
                _Mask = value & NgbValue.All;
                // Проверка в соответствии с  алгоритмом работы эл. габ. ворот с контролем
                // основного габарита погрузки ...
                // Если есть негабариты большие чем левый основной, то убрать левый основной
                if (Contains(NgbValue.Left1 | NgbValue.Left2 | NgbValue.Left3))
                {
                    Set0InValue(NgbValue.LeftMain);
                }
                // Если есть негабариты большие чем правый основной, то убрать правый основной
                if (Contains(NgbValue.Right4 | NgbValue.Right5 | NgbValue.Right6))
                {
                    Set0InValue(NgbValue.RightMain);
                }
                // Если есть левый негабарит ПС, то убрать левый боковой
                if (Contains(NgbValue.LeftPS))
                {
                    Set0InValue(NgbValue.LeftBottom);
                }
                // Если есть правый негабарит ПС, то убрать правый боковой
                if (Contains(NgbValue.RightPS))
                {
                    Set0InValue(NgbValue.RightBottom);
                }
            }
        }

        /// <summary>Предикат "Собеджит какой либо негабарит из маски"</summary>
        /// <param name="mask">Маска негабаритов</param>
        /// <returns>true - содержит</returns>
        public bool Contains(uint mask)
        {
            return (_Mask & mask) != 0;
        }

        void Set0InValue(uint mask)
        {
            _Mask &= (uint)(~mask);
        }

        void Set1InValue(uint mask)
        {
            _Mask |= mask;
        }

        /// <summary>Количество негабаритов в маске</summary>
        public int Count
        {
            get
            {
                int result = 0;
                for (int i = 0; i < 32; i++)
                {
                    uint mask = (uint)(0x00000001 << i);
                    if ((_Mask & mask) != 0) result++;
                }
                return result;
            }
        }

        /// <summary>Инвертировать негабариты</summary>
        public void Invert()
        {
            if (_Mask == NgbValue.None) return;
            uint old_mask = _Mask;
            uint new_mask = 0;
            // инвертируем первые 9 бит
            for (int i = 0; i < 9; i++)
            {
                new_mask <<= 1;
                if ((old_mask & 0x0001) != 0) new_mask |= 0x0001;
                old_mask >>= 1;
            }
            // инверсия  10 и 11 битов
            if (Contains(NgbValue.LeftPS)) new_mask |= NgbValue.RightPS;
            if (Contains(NgbValue.RightPS)) new_mask |= NgbValue.LeftPS;
            // инверсия  12 и 13 битов
            if (Contains(NgbValue.LeftMain)) new_mask |= NgbValue.RightMain;
            if (Contains(NgbValue.RightMain)) new_mask |= NgbValue.LeftMain;
            _Mask = new_mask;
        }

        #region Члены IEquatable<NGBTValue>

        /// <summary>Предикат "Равно"</summary>
        /// <param name="other">Объект для сравнения</param>
        /// <returns>true - равно</returns>
        public bool Equals(NgbValue other)
        {
            if (other == null) return false;
            return Mask == other.Mask;
        }

        #endregion

        /// <summary>Строковое представление негабаритов</summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < 13; i++)
            {
                uint mask = (uint)(0x00000001 << i);
                if ((_Mask & mask) != 0)
                {
                    if (result != string.Empty) result += ", ";
                    if (i == 0)
                    {
                        result += "Левый нижний";
                    }
                    else if (i == 1)
                    {
                        result += "Левый верхний 1";
                    }
                    else if (i == 2)
                    {
                        result += "Левый верхний 2";
                    }
                    else if (i == 3)
                    {
                        result += "Левый верхний 3";
                    }
                    else if (i == 4)
                    {
                        result += "Вертикальный";
                    }
                    else if (i == 5)
                    {
                        result = "Правый верхний 4";
                    }
                    else if (i == 6)
                    {
                        result += "Правый верхний 5";
                    }
                    else if (i == 7)
                    {
                        result += "Правый верхний 6";
                    }
                    else if (i == 8)
                    {
                        result += "Правый нижний";
                    }
                    else if (i == 9)
                    {
                        result += "Левый подвижного состава";
                    }
                    else if (i == 10)
                    {
                        result += "Правый подвижного состава";
                    }
                    else if (i == 11)
                    {
                        result += "Левый основной";
                    }
                    else if (i == 12)
                    {
                        result += "Правый основной";
                    }
                }
            }
            return result;
        }
    
    }
}
