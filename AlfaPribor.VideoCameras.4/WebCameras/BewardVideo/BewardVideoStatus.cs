using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Статус клиента Beward видео</summary>
    public enum BewardVideoStatus
    {
        /// <summary>Неактивен</summary>
        Inactive,
        /// <summary>Подключаюсь</summary>
        Connecting,
        /// <summary>Нет связи</summary>
        Offline,
        /// <summary>На связи</summary>
        Online,
        /// <summary>Не прошла авторизация (401)</summary>
        AccessDenied,
        /// <summary>Неверный запрос (400)</summary>
        BadRequest,
        /// <summary>Ошибка (не 200, 400 или 401)</summary>
        Error
    }

}
