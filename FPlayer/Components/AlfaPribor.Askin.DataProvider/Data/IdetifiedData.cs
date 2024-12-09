using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.ASKIN.Data
{
    /// <summary>Базовый класс для всех идентифицированных данных</summary>
    public class IdetifiedData
    {
        /// <summary>Идентификатор данных</summary>
        private int _Id;

        /// <summary>Конструктор класса. Присваивает идентификатору значение по умолчанию</summary>
        public IdetifiedData()
        {
            _Id = 0;
        }

        /// <summary>Конструктор класса. Присваивает идентификатору заданное значение</summary>
        /// <param name="id">Значение идентификатора</param>
        public IdetifiedData(int id)
        {
            _Id = id;
        }

        /// <summary>Идентификатор данных</summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
    }
}
