using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Статус камеры / устройства / диска</summary>
    public enum DevState : int
    {
        /// <summary>Не известно</summary>
        unknown = -1,
        /// <summary>Нет связи</summary>
        offline = 0,
        /// <summary>Есть связь</summary>
        online = 1,
        /// <summary>Нет видеосигнала (только для телекамер)</summary>
        novideo = 2,
        /// <summary>Выключено / не активно</summary>
        none = 3,
        /// <summary>Изменение настроек</summary>
        changeSettings = 4
    }

    /// <summary>Режим работы устройства</summary>
    public enum DevMode
    {
        /// <summary>Не известно</summary>
        unknown,
        /// <summary>Неактивен</summary>
        duty,
        /// <summary>Ожидание</summary>
        wait,
        /// <summary>Запись</summary>
        record,
        /// <summary>Пауза аппаратная (БС приостанавливает счет)</summary>
        pause_hardware,
        /// <summary>Пауза аппаратная (БС продолжает счет)</summary>
        pause_software,
        /// <summary>Локомотивы</summary>
        loco
    }

    /// <summary>Статус датчика вскрытия шкафа</summary>
    public enum TamperStat
    {
        /// <summary>Не известно</summary>
        unknown = 0,
        /// <summary>Шкаф закрыт</summary>
        secure,
        /// <summary>Шкаф открыт</summary>
        alarm
    }

    /// <summary>Информация о статусе диска</summary>
    public class DiskStatData
    {
        private string _Id = string.Empty;
        private DevState _Status = DevState.unknown;
        private int _Volume = 0;
        private int _FreeSpace = 0;

        /// <summary>Идентификатор диска (буква, путь)</summary>
        [XmlAttribute("id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Статус диска </summary>
        [XmlElement("status", typeof(DevState))]
        public DevState Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        
        /// <summary>Объем в мегабайтах</summary>
        [XmlElement("volume")]
        public int Volume
        {
            get { return _Volume; }
            set { _Volume = value; }
        }

        /// <summary>Свободно</summary>
        [XmlElement("free")]
        public int FreeSpace
        {
            get { return _FreeSpace; }
            set { _FreeSpace = value; }
        }

        /// <summary>Конструктор</summary>
        public DiskStatData()
        {
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор диска (путь, буква)</param>
        /// <param name="stat">Статус диска</param>
        /// <param name="volume">Объем диска</param>
        /// <param name="free_space">Свободно</param>
        public DiskStatData(string id, DevState stat, int volume, int free_space)
        {
            _Id = id;
            _Status = stat;
            _Volume = volume;
            _FreeSpace = free_space;
        }
    }

}
