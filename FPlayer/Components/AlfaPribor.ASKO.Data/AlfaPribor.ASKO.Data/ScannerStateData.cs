﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Информация о статусе лазерного сканера</summary>
    public class ScannerStateData : IEquatable<ScannerStateData>, ICloneable
    {

        int _Id = 0;
        DevState _Status = DevState.unknown;

        /// <summary>Пустой конструктор</summary>
        public ScannerStateData() { }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор камеры (номер)</param>
        public ScannerStateData(int id) { _Id = id; }

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор камеры (номер)</param>
        /// <param name="name">Наименование камеры</param>
        /// <param name="stat">Статус камеры</param>
        public ScannerStateData(int id, DevState stat) { _Id = id; _Status = stat; }

        /// <summary>Идентификатор сканера (номер) 0 - левый, 1 - правый</summary>
        public int Id { get { return _Id; } set { _Id = value; } }

        /// <summary>Статус камеры</summary>
        public DevState Status { get { return _Status; } set { _Status = value; } }

        #region IEquatable<ScannerStateData> members

        /// <summary>Проверяет равенство текущего объекта заданному</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>TRUE - объекты равны, FALSE - в противном случае</returns>
        public bool Equals(ScannerStateData other)
        {
            if (other == null) return false;
            return _Id == other._Id && _Status == other._Status;
        }

        #endregion

        #region Члены ICloneable

        /// <summary>Создает полную копию объекта</summary>
        /// <returns>Копия объекта</returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

    }
}