using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Результат выполнения операции</summary>
    public enum VideoStorageResult
    {
        /// <summary>Успешно</summary>
        Ok = 0,

        /// <summary>Выполнено частично</summary>
        Partial,

        /// <summary>Не найдено</summary>
        NotFound,

        /// <summary>Заблокировано (например воспроизводится или перепаковывается)</summary>
        Blocked,

        /// <summary>Не могу выполнить операцию</summary>
        Fault 
    }

    /// <summary>Статус интерфейсов</summary>
    public enum VideoStorageIntStat
    {
        /// <summary>Готов</summary>
        Ok = 0,

        /// <summary>Не найдены видеоданные с указанным идентификатором</summary>
        NotFound,

        /// <summary>Не удалось открыть для чтения</summary>
        FailToOpen,

        /// <summary>Не удалось открыть для записи</summary>
        FailToCreate,

        /// <summary>Не удалось удалить видеозапись</summary>
        FailToDelete,

        /// <summary>Видеозапись с указанным идентификатором уже существует</summary>
        AlreadyExist,

        /// <summary>Хранилище неактивно</summary>
        StorageInactive,
        
        /// <summary>Ошибка ввода/вывода</summary>
        IOError,
    }

    /// <summary>Состояние раздела хранилища видеоданных</summary>
    public enum VideoPartitionState
    {
        /// <summary>Раздел используется для чтения/записи, ошибки доступа отсутствуют</summary>
        Ok = 0,

        /// <summary>Раздел активен, но недоступен (не привязан к физическому или виртуальному носителю данных)</summary>
        Fault,

        /// <summary>Раздел был отключен через настройки хранилища (недоступен для чтения/записи)</summary>
        Inactive
    }

    /// <summary>Тип содержимого видеоданных</summary>
    public enum VideoContentType
    {
        /// <summary>Нет изображения</summary>
        none = 0,

        /// <summary>Изображение в формате JPEG (JFIF)</summary>
        jpeg,

        /// <summary>Термограмма 8 бит/пиксель</summary>
        raw8,

        /// <summary>Термограмма 16 бит/пиксель</summary>
        raw16, 

        /// <summary>Скан лазерного сканера</summary>
        scan
    }

    /// <summary>Режим открытия видеозаписи</summary>
    public enum VideoRecordOpenMode
    {
        /// <summary>Открыть видеозапись только для чтения</summary>
        Read,

        /// <summary>Открыть видеозапись для изменения</summary>
        ReadWrite
    }

    /// <summary>Тип хранилища видеоданных (реализации под различные системы)</summary>
    public enum VideoStorageType
    {
        /// <summary>Реализация для ВидеоИнспектор-2008/2010</summary>
        ASKO,
        
        /// <summary>Первоначальная реализация</summary>
        Original
    }
}
