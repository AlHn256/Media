using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.Streams2
{
    /// <summary>Состояние заголовка потока</summary>
    public enum StreamHeaderState
    {
        /// <summary>Заголовок прочитан, имеет верную структуру, совместимый номер версии данных</summary>
        Ok = 0,

        /// <summary>Неизвестный тип заголовка</summary>
        Invalid,

        /// <summary>Несовместимый номер версии данных</summary>
        NotSupported
    }
}
