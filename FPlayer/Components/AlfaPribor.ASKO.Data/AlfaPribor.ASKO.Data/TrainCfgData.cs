using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Конфигурация поезда для БС (количество локомотивов в голове состава и осей)</summary>
    /// <remarks>Для ручной постановки на ожидание</remarks>
    public class TrainCfgData : IEquatable<TrainCfgData>, ICloneable
    {

        int _LocoCount = 1;
        int _Axiles = 4;

        /// <summary>Конструктор</summary>
        public TrainCfgData()
        {
        }

        /// <summary>Конструктор</summary>
        /// <param name="loco_count">Количество локомотивов (0...N)</param>
        /// <param name="axiles">Количество осей (0...M)</param>
        public TrainCfgData(int loco_count, int axiles)
        {
            LocoCount = loco_count;
            Axiles = axiles;
        }

        /// <summary>Количество локомотивов/секций</summary>
        public int LocoCount
        {
            get { return _LocoCount; }
            set
            {
                if (value < 0) value = 0;
                _LocoCount = value;
            }
        }

        /// <summary>Количество осей (в локомотиве/секции)</summary>
        public int Axiles
        {
            get { return _Axiles; }
            set
            {
                if (value < 0) value = 0;
                _Axiles = value;
            }
        }

        #region Члены IEquatable<TrainCfgData>

        /// <summary>
        /// Предикат "Равно"
        /// </summary>
        /// <param name="other">Объект для сравнения</param>
        /// <returns></returns>
        public bool Equals(TrainCfgData other)
        {
            if (other == null) return false;
            return _LocoCount == other._LocoCount &&
                _Axiles == other._Axiles;
        }

        #endregion

        #region Члены ICloneable

        /// <summary>
        /// Клонировать
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new TrainCfgData(_LocoCount, _Axiles);
        }

        #endregion
    }

}
