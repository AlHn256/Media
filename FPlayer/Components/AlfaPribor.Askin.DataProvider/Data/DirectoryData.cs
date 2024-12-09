using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.ASKIN.Data
{
    /// <summary>Статус каталога</summary>
    public enum DirStat
    {
        /// <summary>Статус не определен</summary>
        Unknown = 0,

        /// <summary>Архив событий</summary>
        EventsArchive,

        /// <summary>Архив взвешивания поездов</summary>
        TrainsArchive,
    }

    /// <summary>Класс описывает запись таблицы "Каталоги видеоархива" базы данных СБВ УВГ</summary>
    public class DirectoryData : IdetifiedData
    {

        #region Fields

        /// <summary>Путь к каталогу</summary>
        private string _Path;

        /// <summary>Статус каталога</summary>
        private bool _Active;

        /// <summary>Статус каталога</summary>
        private DirStat _Status;

        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства класса значениями по умолчанию</summary>
        public DirectoryData()
        {
            _Path = string.Empty;
            _Status = DirStat.Unknown;
            _Active = false;
        }

        /// <summary>Конструктор класса. Инициализирует свойства класса начальными значениями</summary>
        /// <param name="id">Идентификатор каталога</param>
        /// <param name="path">Путь к каталогу</param>
        /// <param name="status">Статус каталога</param>
        /// <param name="active">Статус каталога (активен/не активен)</param>
        public DirectoryData(int id, string path, DirStat status, bool active)
            : base(id)
        {
            _Path = path;
            _Status = status;
            _Active = active;
        }

        /// <summary>Конструктор класса. Инициализирует свойства класса начальными значениями</summary>
        /// <param name="path">Путь к каталогу</param>
        /// <param name="status">Статус каталога</param>
        /// <param name="active">Статус каталога (активен/не активен)</param>
        public DirectoryData(string path, DirStat status, bool active)
            : base()
        {
            _Path = path;
            _Status = status;
            _Active = active;
        }

        #endregion

        #region Properties

        /// <summary>Путь к каталогу</summary>
        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }

        /// <summary>Статус каталога (активен/не активен)</summary>
        public bool Active
        {
            get { return _Active; }
            set { _Active = value; }
        }

        /// <summary>Статус каталога</summary>
        public DirStat Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        #endregion

    }
}
