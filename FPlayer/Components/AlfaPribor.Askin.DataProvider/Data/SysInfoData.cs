using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKIN.Data
{

    /// <summary>Статус камеры / устройства / диска</summary>
    public enum DevState
    {
        /// <summary>Не известно</summary>
        unknown = 0,

        /// <summary>Есть связь</summary>
        online,

        /// <summary>Нет связи</summary>
        offline,

        /// <summary>
        /// Нет видеосигнала (только камеры)
        /// </summary>
        novideo,

        /// <summary>Выключено / не активно</summary>
        none
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

    /// <summary>Информация о статусе телекамеры</summary>
    public class CameraStatData
    {
        private int _Id = 0;
        private string _Name = string.Empty;
        private DevState _Status = DevState.unknown;
        private string _Resolution = string.Empty;

        /// <summary>Идентификатор камеры (номер)</summary>
        [XmlAttribute("id")]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Наименование камеры</summary>
        [XmlElement("name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>Статус камеры</summary>
        [XmlElement("status", typeof(DevState))]
        public DevState Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>Графическое разрешение кадров видеоизображения</summary>
        [XmlElement("resolution", typeof(string))]
        public string Resolution
        {
            get { return _Resolution; }
            set { _Resolution = value; }
        }

        /// <summary>Конструктор</summary>
        public CameraStatData()
        {
        }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор камеры (номер)</param>
        /// <param name="name">Наименование камеры</param>
        /// <param name="stat">Статус камеры</param>
        public CameraStatData(int id, string name, DevState stat)
        {
            _Id = id;
            _Name = name;
            _Status = stat;
        }

        /// <summary>Конструктор класса.
        /// Инициализирует объект класса начальными значениями идентификатора видеокамеры, наименованием видеокамеры,
        /// состоянием устройства и графическим разрешением кадров видеоизображения
        /// </summary>
        /// <param name="id">Идентификатор камеры (номер)</param>
        /// <param name="name">Наименование камеры</param>
        /// <param name="stat">Статус камеры</param>
        /// <param name="resolution">Графическое разрешение кадров видеоизображения</param>
        public CameraStatData(int id, string name, DevState stat, string resolution)
            : this(id, name, stat)
        {
            _Resolution = resolution;
        }
    }
}
