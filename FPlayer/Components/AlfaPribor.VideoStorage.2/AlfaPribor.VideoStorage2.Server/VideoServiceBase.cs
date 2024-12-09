using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Базовый класс сервисов хранилища</summary>
    class VideoServiceBase
    {
        /// <summary>Генерирует событие "Завершение работы сервиса"</summary>
        protected void RaiseCloseEvent()
        {
            if (Closing != null)
            {
                try
                {
                    Closing(this, EventArgs.Empty);
                }
                catch { }
            }
        }

        /// <summary>Событие "Завершение работы сервиса"</summary>
        public event EventHandler Closing;
    }
}
