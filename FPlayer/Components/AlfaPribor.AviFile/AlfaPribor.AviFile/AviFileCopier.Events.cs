using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace AlfaPribor.AviFile
{
    /// <summary>Содержит дополнительные сведения события "Копирование экземпляра данных"</summary>
    public class CopySampleEventArgs : EventArgs
    {
        /// <summary>Номер копируемого экземпляра данных</summary>
        private int _Number;

        /// <summary>Правило копирования</summary>
        private CopyRule _Rule;

        /// <summary>Конструктор объектов класса</summary>
        /// <param name="rule">Правило копирования</param>
        /// <param name="number">Номер копируемого экземпляра данных</param>
        public CopySampleEventArgs(CopyRule rule, int number)
        {
            _Rule = rule;
            _Number = number;
        }

        /// <summary>Правило копирования</summary>
        public CopyRule Rule
        {
            get { return _Rule; }
        }

        /// <summary>Номер копируемого экземпляра данных</summary>
        public int Number
        {
            get { return _Number; }
        }
    }

    /// <summary>Описывает сведения о возникшем исключении в процессе копирования данных</summary>
    public class CopyExceptionEventArgs : CancelEventArgs
    {
        /// <summary>Правило копирования данных</summary>
        private CopyRule _Rule;

        /// <summary>Ссылка на возникшее исключение</summary>
        private System.Exception _InnerException;

        /// <summary>Конструктор объектов класса</summary>
        /// <param name="rule">Правило копирования данных</param>
        /// <param name="except">Ссылка на возникшее исключение</param>
        public CopyExceptionEventArgs(CopyRule rule, Exception except)
        {
            _Rule = rule;
            _InnerException = except;
        }

        /// <summary>Правило копирования данных, вызвавшее исключение</summary>
        public CopyRule Rule
        {
            get { return _Rule; }
        }

        /// <summary>Ссылка на возникшее исключение</summary>
        public System.Exception InnerException
        {
            get { return _InnerException; }
        }
    }

    /// <summary>Содержит дополнительные сведения события "Чтение экземпляра данных"</summary>
    public class ReadSampleEventArgs : CopySampleEventArgs
    {
        /// <summary>Дополнительная информация</summary>
        private object _Info;

        /// <summary>Блок данных</summary>
        private byte[] _Data;

        /// <summary>Конструктор объектов класса</summary>
        /// <param name="rule">Правило копирования</param>
        /// <param name="number">Номер копируемого экземпляра данных</param>
        public ReadSampleEventArgs(CopyRule rule, int number)
            : this(rule, number, null, null) { }

        /// <summary>Конструктор объектов класса</summary>
        /// <param name="rule">Правило копирования</param>
        /// <param name="number">Номер копируемого экземпляра данных</param>
        /// <param name="info">Дополнительная информация о прочитанных данных</param>
        /// <param name="data">Блок данных</param>
        public ReadSampleEventArgs(CopyRule rule, int number, object info, byte[] data)
            : base(rule, number)
        {
            _Info = info;
            _Data = data;
        }

        /// <summary>Дополнительная информация о прочитанных данных</summary>
        public object Info
        {
            get { return _Info; }
            set { _Info = value; }
        }

        /// <summary>Блок данных</summary>
        public byte[] Data
        {
            get { return _Data; }
            set { _Data = value; }
        }
    }
}
