using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>
    /// Хранилище видеоданных ВидеоИнспектор-2008/2010 с инструментарием для разделенного использования разными потоками
    /// </summary>
    public class SyncAskoVideoStorage : AskoVideoStorage, ISyncVideoStorage
    {
        private object _SyncRoot = new object();

        #region ISyncVideoStorage members

        /// <summary>
        /// Объект синхронизации доступа к открытым членам класса из разных потоков
        /// </summary>
        public object SyncRoot
        {
            get { return _SyncRoot; }
        }

        #endregion
    }
}
