using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

/* Глобальные определения приложения */
namespace AlfaPribor.ASKO.Shared
{

    /// <summary>Типы событий</summary>

    public enum EventsTypes
    {

        /// <summary>Неопределено</summary>
      
        Unknown = 0,

        #region Приложение

        /// <summary>Запуск программы</summary>
        StartServer = 1,
        /// <summary>Завершение программы</summary>
        CloseServer = 2,
        /// <summary>Клиентское приложение подсоединено</summary>
        ClientConnected = 3,
        /// <summary>Клиентское приложение отсоединено</summary>
        ClientDisconnected = 4,

        #endregion
        
        #region Операторы

        /// <summary>Регистрация оператора</summary>
        OperatorReg = 5,
        /// <summary>Сброс регистрации оператора</summary>
        OperatorUnreg = 6,
        /// <summary>Подтверждение оператором</summary>
        OperatorConfirm = 7,
        /// <summary>Добавлен оператор</summary>
        OperatorAdd = 8,
        /// <summary>Оператор изменен</summary>
        OperatorChanged = 9,
        /// <summary>Удален оператор</summary>
        OperatorDeleted = 10,

        #endregion        

        #region Оборудование

        /// <summary>Вскрытие шкафа</summary>
        OpenSensor = 11,
        /// <summary>Закрытие шкафа</summary>
        CloseSensor = 12,
        /// <summary>Потеря связи</summary>
        Disconnect = 13,
        /// <summary>Возобновление связи</summary>
        Connect = 14,
        /// <summary>Изменение режима устройства</summary>
        ChangeDeviceMode = 15,
        /// <summary>Неисправность</summary>
        Failure = 16,
        /// <summary>Возврат к норме</summary>
        Norm = 17,
        /// <summary>Успешная операция</summary>
        Success = 18,

        #endregion

        #region Режимы АРМ

        /// <summary>Изменение состояния записи АРМ</summary>
        ChangeRecMode = 19,
        /// <summary>Изменение состояния просмотра архива</summary>
        ChangeArchiveMode = 20,

        #endregion

        #region Конфигурация

        /// <summary>Изменение конфигурации</summary>
        ChangeConfig = 21,

        #endregion

        #region События состава

        /// <summary>Начало состава</summary>
        BeginTrain = 22,
        /// <summary>Конец состава</summary>
        EndTrain = 23,
        /// <summary>Начало вагона</summary>
        BeginVagon = 24,
        /// <summary>Конец вагона</summary>
        EndVagon = 25,
        /// <summary>Начало локомотива</summary>
        BeginLoco = 26,
        /// <summary>Конец локомотива</summary>
        EndLoco = 27,

        #endregion

        #region Редактирование

        /// <summary>Изменение параметров поезда</summary>
        ChangeTrain = 28,
        /// <summary>Удаление поезда</summary>
        DeleteTrain = 29,
        /// <summary>Добавление вагона</summary>
        AddVagon = 30,
        /// <summary>Удаление вагона</summary>
        DeleteWagon = 31,
        /// <summary>Изменение данных вагона</summary>
        ChangeWagon = 32,
        /// <summary>Добавление локомотива</summary>
        AddLoco = 33,
        /// <summary>Удаление локомотива</summary>
        DeleteLoco = 34,
        /// <summary>Редактирование инвентарного номера</summary>
        ChangeInv = 35,

        #endregion

        #region Интеграция

        /// <summary>Принят запрос</summary>
        QueryRecieved = 36,
        /// <summary>Сообщение отправлено</summary>
        MessageSend = 37,
        /// <summary>Ошибка приема/передачи данных</summary>
        SendError = 38,

        #endregion

        #region Регистратор сигналов

        /// <summary>Постановка на ожидание записи регистратора сигналов</summary>
        BsRegStart = 39,
        /// <summary>Остановка записи регистратора сигналов</summary>
        BsRegStop = 40,
        /// <summary>Файл с регистратора сигналов закачан</summary>
        BsRegSaveFile = 41,

        #endregion

        #region Дополнительные

        /// <summary>Комментарий</summary>
        Comment = 42,
        /// <summary>База данных</summary>
        DataBase = 43

        #endregion
            
    }

    /// <summary>Состояния сервера</summary>

    public enum eRecMode
    {
        /// <summary>Неизвестно</summary>
      
        Unknown = -1,
        /// <summary>Наблюдение</summary>
      
        Duty = 0,
        /// <summary>Ожидание</summary>
      
        Wait = 1,
        /// <summary>Запись состава</summary>
      
        Go = 2,
        /// <summary>Аппаратная пауза записи</summary>
      
        PauseHardware = 3,
        /// <summary>Программная пауза записи</summary>
      
        PauseSoftware = 4
    }

    /// <summary>Тип работы БС контроллер/опросчик</summary>

    public enum BSMode
    {
        /// <summary>Опросчик</summary>
      
        Controller = 0,
        /// <summary>Контроллер</summary>
      
        Poller = 1
    }
    
    /// <summary>Тип датчиков БС</summary>
    public enum BSSensors
    {
        /// <summary>ИК-излучатели</summary>
        InfraRed = 0,
        /// <summary>Индукционные педали</summary>
        InductionSensors = 1
    }
    
    /// <summary>Направление движения</summary>

    public enum Direction
    {
      
        Unknown = -1,
      
        Forward = 0,
      
        Backward = 1
    }

    /// <summary>Дискретность таймера ожидания окончания состава</summary>
    public enum TimerResolution
    {
        Off = 0,
        Second = 1,
        Minute = 2
    }

    /// <summary>Дейтствие по завершение ожидания</summary>
    public enum StopVagonAction
    {
        /// <summary>Пауза записи</summary>
        PauseRecord = 0,
        /// <summary>Снижние скорости записи</summary>
        DropFPS = 1
    }
    
    /// <summary>Тип вагона</summary>
    public enum VagonType
    {
        /// <summary>Обычная подвижная единица</summary>
        Vagon = 0,
        /// <summary>Локомотив</summary>
        Loco = 1,
        /// <summary>Рефрижератор</summary>
        Refrigerator = 2,
        /// <summary>Сдвоенная платформа</summary>
        DoublePlatform = 3
    }

    /// <summary>Тип телекамры</summary>
    public enum CameraTypes
    {
        /// <summary>Axis 221</summary>
        Axis_221 = 0,
        /// <summary>Axis p1346</summary>
        Axis_p1346 = 1,
        /// <summary>Видеосервер Axis</summary>
        Axis_Server = 2,
        /// <summary>Телекамера Basler</summary>
        Basler = 3,
        /// <summary>Телекамера Demo (воспроизводит файл)</summary>
        Demo = 10
    }

    public enum WagonTypes
    {
        Unknown = 0,
        BoxCar = 1,
        HalfWagon = 2,
        Hopper = 3,
        Platform = 4,
        Cistern = 5,
        Bunker = 6,
        Dumkar = 7
    };

    /// <param name="value">Тип груза</param>
    public enum CargoTypes
    {
        Empty = 0,
        Unknown = 1,
        Bulk = 2,
        Unitized = 3,
        Fuel = 4
    }

    /// <summary>Что отображать по умолчанию. Модель(Негабариты)/Груз</summary>
    public enum SickLiveView
    {
        ModelNGBT = 0,
        Cargo = 1
    }

    /// <summary>Тип негабаритов: 0 - обычные, 1 - расширенные</summary>
    public enum OversizeType
    {
        /// <summary>Неизвестно</summary>
        Unknown = -1,
        /// <summary>Обычные</summary>
        Standard = 0,
        /// <summary>Расширенные</summary>
        Extended = 1,
        /// <summary>Расширенные 2 (впоследствии заменит "Расширенные" и "Обычные")
        /// Негабариты версии 3</summary>
        Extended2 = 2
    }

    /// <summary>Версия протокола АРМ ПКО</summary>
    public enum ArmPKOVersion
    {
        /// <summary>Неизвестно</summary>
        Unknown = -1,
        /// <summary>Версия 1</summary>
        v1 = 0,
        /// <summary>Версия 2, 2018 г.</summary>
        v2 = 1
    }

}
