using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Shared
{

    /// <summary>Идентификаторы датчиков подключенных к БИС/БС.32</summary>

    public enum BisSensorId
    {
        /// <summary>Неизвестен</summary>
        Unknown,
        /// <summary>НГБ1</summary>
      
        Ngb1,
        /// <summary>НГБ2</summary>
      
        Ngb2,
        /// <summary>НГБ3</summary>
      
        Ngb3,
        /// <summary>НГБ4</summary>
      
        Ngb4,
        /// <summary>НГБ5</summary>
      
        Ngb5,
        /// <summary>НГБ6summary>
      
        Ngb6,
        /// <summary>НГБ7</summary>
      
        Ngb7,
        /// <summary>НГБ8</summary>
      
        Ngb8,
        /// <summary>НГБ9</summary>
      
        Ngb9,
        /// <summary>НГБ10</summary>
      
        Ngb10,
        /// <summary>НГБ11</summary>
      
        Ngb11,
        /// <summary>НГБ12</summary>
      
        Ngb12,
        /// <summary>НГБ13</summary>
      
        Ngb13,
        /// <summary>Статус реле КН</summary>
      
        PowerShock,
        /// <summary>Вскрытие корпуса</summary>
      
        Tamper,
        /// <summary>Датчик начала состава</summary>
      
        DNS,
        /// <summary>Датчик счета вагонов</summary>
      
        DSW,
        /// <summary>Датчик счета колес</summary>
      
        DSK,
        /// <summary>Датчик определения скорости</summary>
      
        DOS,
        /// <summary>Датчик педали 1</summary>
      
        DP1,
        /// <summary>Датчик педали 2</summary>
      
        DP2,
        /// <summary>Датчик педали 3</summary>
      
        DP3,
        /// <summary>Датчик педали 4</summary>
      
        DP4,
        /// <summary>Датчик наличия поезда 1</summary>
      
        TP1,
        /// <summary>Датчик наличия поезда 2</summary>
      
        TP2
    }

    /// <summary>Тип и статус датчика (входа) БИС/БС</summary>

    public class BisSensor : IEquatable<BisSensor>
    {
        /// <summary>Конструктор</summary>        
        public BisSensor()
        {
            Id = BisSensorId.Unknown;
            Stat = SensorStat.Unknown;
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор датчика (входа)</param>
        /// <param name="stat">Статус датчика</param>
        public BisSensor(BisSensorId id, SensorStat stat)
        {
            Id = id;
            Stat = stat;
        }

        /// <summary>Идентификатор датчика (входа)</summary>
        
        public BisSensorId Id { get; set; }

        /// <summary>Статус датчика</summary>
        
        public SensorStat Stat { get; set; }

        #region Члены IEquatable<BisSensor>

        /// <summary>
        /// Предикат "Равно"
        /// </summary>
        /// <param name="other">Объект для сравнения</param>
        /// <returns></returns>
        public bool Equals(BisSensor other)
        {
            if (other == null) return false;
            return Id == other.Id;
        }

        #endregion

    }

    /// <summary>Вспомогательный класс для работы с масками датчиков БИС</summary>
    public static class BisSensorMasking
    {

        /// <summary>Маска датчиков негабаритов (ДНГ)</summary>
        public const uint IM_DNG = 0x000001FF;
        /// <summary>Маска датчиков негабаритов ПС (ДНГПС)</summary>
        public const uint IM_DNGPS = 0x00000600;
        /// <summary>Маска датчиков основных габаритов (ДНГОГ)</summary>
        public const uint IM_DNGOG = 0x00001800;
        /// <summary>Маска счетных датчиков</summary>
        public const uint IM_COUNTS = 0xF0000000;
        /// <summary>Маска датчика вскрытия шкафа</summary>
        public const uint IM_TAMPER = 0x08000000;
        /// <summary>Маска датчика срабатывания реле контроля напряжения</summary>
        public const uint IM_KN = 0x04000000;

        /// <summary>Индекс по идентификатору датчика</summary>
        /// <param name="sensor">Идентификатор датчика</param>
        /// <returns>-1 - нет индекса</returns>
        public static int IndexOf(BisSensorId sensor)
        {
            switch (sensor)
            {
                case BisSensorId.Ngb1: return 0;
                case BisSensorId.Ngb2: return 1;
                case BisSensorId.Ngb3: return 2;
                case BisSensorId.Ngb4: return 3;
                case BisSensorId.Ngb5: return 4;
                case BisSensorId.Ngb6: return 5;
                case BisSensorId.Ngb7: return 6;
                case BisSensorId.Ngb8: return 7;
                case BisSensorId.Ngb9: return 8;
                case BisSensorId.Ngb10: return 9;
                case BisSensorId.Ngb11: return 10;
                case BisSensorId.Ngb12: return 11;
                case BisSensorId.Ngb13: return 12;
                case BisSensorId.PowerShock: return 26;
                case BisSensorId.Tamper: return 27;
                case BisSensorId.DNS: return 28;
                case BisSensorId.DSW: return 29;
                case BisSensorId.DSK: return 30;
                case BisSensorId.DOS: return 31;
                case BisSensorId.DP1: return 28;
                case BisSensorId.DP2: return 29;
                case BisSensorId.DP3: return 30;
                case BisSensorId.DP4: return 31;
            }
            return -1;
        }

        /// <summary>Маска по идентификатору датчика</summary>
        /// <param name="sensor">Идентификатор датчика</param>
        /// <returns>0 - нет маски</returns>
        public static uint MaskOf(BisSensorId sensor)
        {
            int index = IndexOf(sensor);
            if (index != -1)
            {
                return (uint)(0x00000001 << index);
            }
            return 0;
        }
    }
}
