using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Статус каталога</summary>
    public enum DirStat
    {
        /// <summary>Статус не определен</summary>
        Unknown = 0,
        /// <summary>Архив событий</summary>
        EventsArchive,
        /// <summary>Архив поездов</summary>
        TrainsArchive,
    }

    /// <summary>Класс описывает запись таблицы "Каталоги видеоархива" базы данных</summary>
    public class DirectoryData
    {

        #region Fields

        /// <summary>Идентификатор</summary>
        int _Id;
        /// <summary>Путь к каталогу</summary>
        string _Path;
        /// <summary>Статус каталога</summary>
        bool _Active;
        /// <summary>Статус каталога</summary>
        DirStat _Status;
        /// <summary>Емкость диска</summary>
        long _TotalSize;
        /// <summary>Доступно на диске</summary>
        long _TotalFreeSpace;


        #endregion

        #region Methods

        /// <summary>Конструктор класса. Инициализирует свойства класса начальными значениями</summary>
        /// <param name="id">Идентификатор каталога</param>
        /// <param name="path">Путь к каталогу</param>
        /// <param name="status">Статус каталога</param>
        /// <param name="active">Статус каталога (активен/не активен)</param>
        public DirectoryData(int id, string path, DirStat status, bool active)
        {
            _Id = id;
            _Path = path;
            _Status = status;
            _Active = active;
        }

        #endregion

        #region Properties

        /// <summary>Идентифиуатор каталога</summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

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

        /// <summary>Емкость диска</summary>
      
        public long TotalSize
        {
            get { return _TotalSize; }
            set { _TotalSize = value; }
        }

        /// <summary>Доступно на диске</summary>
        public long TotalFreeSpace
        {
            get { return _TotalFreeSpace; }
            set { _TotalFreeSpace = value; }
        }

        #endregion

    }
}
