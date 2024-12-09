using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Shared
{
    
    /// <summary>Статусы поезда</summary>

    public enum TrainStatus
    {
        /// <summary>Неизвестен</summary>
      
        Unknown = -1,

        /// <summary>Записывается</summary>
      
        Recording = 0,

        /// <summary>Записан</summary>
      
        Recorded = 1,

        /// <summary>Заблокирован (не участвует в замыкании кольцевого буфера)</summary>
      
        Locked = 2,

        /// <summary>Перепаковывается</summary>
      
        Repacking = 3,

        /// <summary>Перепакован</summary>
      
        Repacked = 4
    }

    /// <summary>Направление движения поезда в створе ворот</summary>

    public enum TrainDirection
    {
        /// <summary>Неизвестно</summary>
      
        Unknown = -1,

        /// <summary>Прямое</summary>
      
        Positive = 0,

        /// <summary>Обратное</summary>
      
        Negative = 1
    }

    /// <summary>Статус датчика</summary>

    public enum SensorStat
    {
        /// <summary>Не известно</summary>
      
        Unknown = 0,

        /// <summary>Норма</summary>
      
        Secure,

        /// <summary>Тревога</summary>
      
        Alarm,

        /// <summary>Не используется</summary>
      
        None,

        /// <summary>Не исправен</summary>
      
        Fault
    }

    /// <summary>Режим интерполяции</summary>

    public enum InterpolationMode
    {
        /// <summary>Нет интерполяции</summary>
      
        None = 0,

        /// <summary>Билинейная интерполяция</summary>
      
        Bilinear = 1,

        /// <summary>Бикубическая интерполяция</summary>
      
        Bicubic = 2
    }

    /// <summary>Текущий статус сервера</summary>

    public enum ServerStat
    {
        /// <summary>Неактивен (не обрабатывает сигналы БИС/БС)</summary>
      
        Inactive,
        /// <summary>Ожидание</summary>
      
        Awaiting,
        /// <summary>Запись поезда</summary>
      
        Recording,
        /// <summary>Пауза в записи поезда</summary>
      
        Paused
    }

    /// <summary>Тип расположения датчиков счета (ДСК) и определения скорости (ДОС) относительно ворот</summary>

    public enum DirectionType
    {
        /// <summary>Прямое - ДСК, затем ДОС</summary>
      
        Forward = 0,

        /// <summary>Обратное - ДОС, затем ДСК</summary>
      
        Backward = 1
    }

    /// <summary>Тип блока согласования</summary>

    public enum BisType
    {
        /// <summary>Блок индикации и согласования (БИС 1-09)</summary>
      
        Bis = 0,

        /// <summary>Блок согласования (БС.32/БС.32М)</summary>
      
        Bs32 = 1
    }

    /// <summary>Тип источника данных</summary>

    public enum AskoContractor
    {
        /// <summary>Источник данных отсутствует</summary>
      
        Unknown = 0,

        /// <summary>Источником данных является АРМ ПКО</summary>
      
        ARMPKO,

        /// <summary>Источником данных является ЕАСАПР</summary>
      
        EASAPR,

        /// <summary>Источником данных является АСКМ</summary>
      
        ASKM,

        /// <summary>Источником данных является Белорусская Железная Дорога</summary>
      
        BelZD

    }

    /// <summary>Статус наличия коммерческой неисправности у вагона</summary>

    public enum WagNgbStatus
    {
        /// <summary>Неизвестно - возможно как наличие, так и отсутствие коммерческой неисправности</summary>
      
        Unknown = -1,

        /// <summary>Коммерческая неисправность отсутствует</summary>
      
        WithoutKN = 0,

        /// <summary>Коммерческая неисправность - вагон отмечен оператором</summary>
      
        WithOpMark = 1,

        /// <summary>Коммерческая неисправность - негабаритный вагон</summary>
      
        WithNgb = 2,

        /// <summary> Коммерческая неисправность - вагон помечен оператором или негабаритный вагон</summary>
      
        WithKN = 3,
    }

    /// <summary>Уровень полномочий пользователя (оператора)</summary>

    public enum UserLevel
    {
        /// <summary>Пользователь</summary>
      
        User = 0,
        /// <summary>Опытный пользователь</summary>
      
        PowerUser = 1,
        /// <summary>Администратор</summary>
      
        Administrator = -1
    }

    /// <summary>Модель телекамеры</summary>

    public enum CameraModel
    {
        /// <summary> Модель телекамеры неизвестна</summary>
      
        Unknown = 0,
        /// <summary>Axis</summary>
      
        Axis
    }

    /// <summary>Тип перезагрузки</summary>

    public enum RestartMode
    {
        /// <summary>Неизвестен (ничего не делать)</summary>
      
        Unknown = 0,
        /// <summary>Команда остановить (и перезагзузить сервис)</summary>
      
        Service = 1,
        /// <summary>Метод остановить, а  затем запустить контроллек АСКО</summary>
      
        Internal = 2
    }

}
