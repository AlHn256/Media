using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.ASKO.DataBase
{

    /// <summary>Класс информации о поле базы данных</summary>
    public class DataBaseFieldInfo
    {

        string name = "";
        string type = "";
        string desc = "";
        string table_name = "";
        bool allow_null = false;

        /// <summary>Конструктор класса информации о поле базы данных</summary>
        /// <param name="_tablename">Название таблицы</param>
        /// <param name="_name">Имя поля</param>
        /// <param name="_type">Тип</param>
        /// <param name="_description">Описание поля</param>
        /// <param name="_allow_null">Разрешить null</param>
        public DataBaseFieldInfo(string _tablename, string _name, string _type, string _description, bool _allow_null)
        {
            table_name = _tablename;
            name = _name;
            type = _type;
            desc = _description;
            allow_null = _allow_null;
        }

        /// <summary>Имя колонки</summary>
        public string ColName
        {
            get { return name; }
        }

        /// <summary>Название</summary>
        public string FullName
        {
            get { return table_name + "." + name; }
        }

        /// <summary>Название таблицы</summary>
        public string TableName
        {
            get { return table_name; }
        }

        /// <summary>Тип поля</summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>Разрешить null</summary>
        public bool AllowNull
        {
            get { return allow_null; }
        }

        /// <summary>Описание</summary>
        public string Description
        {
            get { return desc; }
        }

    }

    public static class DB
    {

        static DB()
        {
            DBInit();
        }
        
        /// <summary>Инициализация полей данных базы данных</summary>
        public static void DBInit()
        {

            //Таблица "параметры"
            Config.ParamName = new DataBaseFieldInfo(Config.TableName, "ParamName", "nvarchar(50)", "Наименование параметра", false);
            Config.ParamValue = new DataBaseFieldInfo(Config.TableName, "ParamValue", "nvarchar(MAX)", "Значение параметра", true);

            //Таблица "каталоги"
            Directories.DirId = new DataBaseFieldInfo(Directories.TableName, "DirId", "int", "Идентификатор каталога видеоархива", false);
            Directories.DirPath = new DataBaseFieldInfo(Directories.TableName, "DirPath", "nvarchar(512)", "Путь к каталогу хранения", true);
            Directories.DirStat = new DataBaseFieldInfo(Directories.TableName, "DirStat", "int", "Стутас каталога", true);
            Directories.FreeSpaceLimit = new DataBaseFieldInfo(Directories.TableName, "DirStat", "int", "Лимит свободного простанства на диске", true);

            //Таблицы событий
            Eventlog.Id = new DataBaseFieldInfo(Eventlog.TableName, "Sn", "int", "Серийный номер события", false);
            Eventlog.EvDateTime = new DataBaseFieldInfo(Eventlog.TableName, "EvDateTime", "datetime", "Дата и время события", false);
            Eventlog.MsgId = new DataBaseFieldInfo(Eventlog.TableName, "MsgId", "int", "Идентификатор сообщения", false);
            Eventlog.EvSource = new DataBaseFieldInfo(Eventlog.TableName, "EvSource", "nvarchar(MAX)", "Источник события / наименование устройства", true);
            Eventlog.EvData = new DataBaseFieldInfo(Eventlog.TableName, "EvData", "nvarchar(MAX)", "Дополнительные данные события", true);
            Eventlog.OpId = new DataBaseFieldInfo(Eventlog.TableName, "OpId", "int", "Идентификатор оператора", true);
            Eventlog.HasVideo = new DataBaseFieldInfo(Eventlog.TableName, "OpId", "int", "Наличие видео", true);

            //Таблица название сообщений
            MarkedFrames.TrainId = new DataBaseFieldInfo(MarkedFrames.TableName, "TrainId", "int", "Идентификатор поезда", false);
            MarkedFrames.TimeSpan = new DataBaseFieldInfo(MarkedFrames.TableName, "TimeSpan", "int", "Метка времени", false);
            MarkedFrames.CameraId = new DataBaseFieldInfo(MarkedFrames.TableName, "CameraId", "int", "Идентификатор (номер) телекамеры", false);

            //Таблица название сообщений
            Messages.MsgId = new DataBaseFieldInfo(Messages.TableName, "MsgId", "int", "Идентификатор типа события", false);
            Messages.MsgText = new DataBaseFieldInfo(Messages.TableName, "MsgText", "nvarchar(255)", "Наименование события", false);

            //Натурные листы
            Numbers.Sn = new DataBaseFieldInfo(Numbers.TableName, "Sn", "int", "Порядковый номер", false);
            Numbers.TrainId = new DataBaseFieldInfo(Numbers.TableName, "TrainId", "int", "Идентификатор состава", false);
            Numbers.Inv = new DataBaseFieldInfo(Numbers.TableName, "Inv", "nvarchar(10)", "Инвентарный номер", true);

            //Таблица "Операторы"
            Operators.OpId = new DataBaseFieldInfo(Operators.TableName, "OpId", "int", "Идентификатор оператора", false);
            Operators.OpLogin = new DataBaseFieldInfo(Operators.TableName, "OpLogin", "nvarchar(32)", "Логин", false);
            Operators.OpPassword = new DataBaseFieldInfo(Operators.TableName, "OpPassword", "nvarchar(32)", "Пароль", false);
            Operators.OpName = new DataBaseFieldInfo(Operators.TableName, "FullName", "nvarchar(100)", "Описание пользователя (имя)", false);
            Operators.Permissions = new DataBaseFieldInfo(Operators.TableName, "Permissions", "int", "Полномочия пользователя", false);
            Operators.Status = new DataBaseFieldInfo(Operators.TableName, "Status", "bit", "Статус пользователя", false);
            Operators.UsbKey = new DataBaseFieldInfo(Operators.TableName, "UsbKey", "int", "Код usb-ключа", false);

            //Таблица статусов
            Status.DevName = new DataBaseFieldInfo(Status.TableName, "DevName", "nvarchar(50)", "Имя устройства (параметра)", true);
            Status.DevStat = new DataBaseFieldInfo(Status.TableName, "DevStat", "nvarchar(255)", "Состояние", true);

            //Таблица информации о составе
            Trains.TrainId = new DataBaseFieldInfo(Trains.TableName, "TrainId", "int", "Идентификатор поезда", false);
            Trains.WayId = new DataBaseFieldInfo(Trains.TableName, "WayId", "int", "Идентификатор пути", false);
            Trains.TimeBegin = new DataBaseFieldInfo(Trains.TableName, "TimeBegin", "datetime", "Дата и время начала записи", false);
            Trains.TimeEnd = new DataBaseFieldInfo(Trains.TableName, "TimeEnd", "datetime", "Дата и время окончания записи", true);
            Trains.TimeIndex = new DataBaseFieldInfo(Trains.TableName, "TimeIndex", "nvarchar(16)", "Индекс поезда", true);
            Trains.TimeNumber = new DataBaseFieldInfo(Trains.TableName, "TimeNumber", "nvarchar(4)", "Номер поезда", true);
            Trains.Way = new DataBaseFieldInfo(Trains.TableName, "Way", "nvarchar(4)", "Путь", true);
            Trains.Direction = new DataBaseFieldInfo(Trains.TableName, "Direction", "int", "Направление", true);
            Trains.Status = new DataBaseFieldInfo(Trains.TableName, "Status", "int", "Статус", true);
            Trains.DirId = new DataBaseFieldInfo(Trains.TableName, "DirId", "int", "Идентификатор раздела видеохранилища", true);
            Trains.OpId = new DataBaseFieldInfo(Trains.TableName, "OpId", "int", "Идентификатор оператора, производившего запись", true);
            Trains.Accepted = new DataBaseFieldInfo(Trains.TableName, "Accepted", "int", "Признак обработки состава", true);
            Trains.InvertInvNum = new DataBaseFieldInfo(Trains.TableName, "InvertInvNum", "int", "Инверсия инвентарных номеров в натурном листе", true);

            //Таблица информации о вагонах
            Wagons.WagonId = new DataBaseFieldInfo(Wagons.TableName, "WagonId", "int", "Идентификатор вагона в базе", false);
            Wagons.TrainId = new DataBaseFieldInfo(Wagons.TableName, "TrainId", "int", "Идентификатор состава для вагона", false);
            Wagons.WagonSn = new DataBaseFieldInfo(Wagons.TableName, "WagonSn", "int", "Номер пересечения подвижной единицы в составе", false);
            Wagons.SnSost = new DataBaseFieldInfo(Wagons.TableName, "SnSost", "int", "Порядковый номер подвижной единицы в составе", false);
            Wagons.TimeSpanBegin = new DataBaseFieldInfo(Wagons.TableName, "TimeSpanBegin", "int", "Метка времени начала вагона", true);
            Wagons.TimeSpanEnd = new DataBaseFieldInfo(Wagons.TableName, "TimeSpanEnd", "int", "Метка времени окончания вагона", true);
            Wagons.TimeSpanBeginBS = new DataBaseFieldInfo(Wagons.TableName, "TimeSpanBeginBS", "int", "Метка времени в миллисекундах начала вагона (смещение от начала видеозаписи) по меткам времени от БС", true);
            Wagons.TimeSpanEndBS = new DataBaseFieldInfo(Wagons.TableName, "TimeSpanEndBS", "int", "Метка времени в миллисекундах окончания вагона (смещение от начала видеозаписи) по меткам времени от БС", true);
            Wagons.SpeedBegin = new DataBaseFieldInfo(Wagons.TableName, "SpeedBegin", "int", "Скорость вагона на первой тележке", true);
            Wagons.SpeedEnd = new DataBaseFieldInfo(Wagons.TableName, "SpeedEnd", "int", "Скорость вагона на последней тележке", true);
            Wagons.DirectionBegin = new DataBaseFieldInfo(Wagons.TableName, "DirectionBegin", "int", "Направление вагона на первой тележке", true);
            Wagons.DirectionEnd = new DataBaseFieldInfo(Wagons.TableName, "DirectionEnd", "int", "Направление вагона на последней тележке", true);
            Wagons.NGB = new DataBaseFieldInfo(Wagons.TableName, "NGB", "int", "Маска негабаритностей", true);
            Wagons.InvNum = new DataBaseFieldInfo(Wagons.TableName, "InvNum", "nvarchar(10)", "Инвентарный номер вагона", true);
            Wagons.Mark = new DataBaseFieldInfo(Wagons.TableName, "Mark", "int", "Признак маркирования", true);
            Wagons.Banned = new DataBaseFieldInfo(Wagons.TableName, "Banned", "int", "Запрет ставить в состав", true);
            Wagons.Comment = new DataBaseFieldInfo(Wagons.TableName, "Comment", "nvarchar(MAX)", "Комментарий", true);
            Wagons.TimeChanged = new DataBaseFieldInfo(Wagons.TableName, "Changed", "datetime", "Дата и время последнего изменения", true);

            //Таблица данных по распознаанию
            WagonsOCR.Id = new DataBaseFieldInfo(WagonsOCR.TableName, "id", "int", "Идентификатор записи в таблице", false);
            WagonsOCR.OcrTrain = new DataBaseFieldInfo(WagonsOCR.TableName, "ocr_train", "int", "Идентификатор поезда в системе АСКИН", true);
            WagonsOCR.WagonId = new DataBaseFieldInfo(WagonsOCR.TableName, "wagon_id", "int", "Идентификатор  вагона в таблице wagons", true);
            WagonsOCR.Sn = new DataBaseFieldInfo(WagonsOCR.TableName, "sn", "int", "Порядковый номер пересечения", true);
            WagonsOCR.InvNum = new DataBaseFieldInfo(WagonsOCR.TableName, "inv_num", "nvarchar(8)", "Распознанный инвентарный номер", true);
            WagonsOCR.Type = new DataBaseFieldInfo(WagonsOCR.TableName, "type", "int", "Тип распознанного инвентарного номера", true);
            WagonsOCR.Accuracy = new DataBaseFieldInfo(WagonsOCR.TableName, "accuracy", "int", "Достоверность распознавания номера", true);
            WagonsOCR.Сheck = new DataBaseFieldInfo(WagonsOCR.TableName, "check", "int", "Проверка оператором", true);

            //Таблица данных по грузам
            WagonsCargo.Id = new DataBaseFieldInfo(WagonsCargo.TableName, "id", "int", "Идентификатор записи в таблице", false);
            WagonsCargo.WagId = new DataBaseFieldInfo(WagonsCargo.TableName, "WagId", "int", "Идентификатор вагона", true);
            WagonsCargo.shift_x = new DataBaseFieldInfo(WagonsCargo.TableName, "shift_x", "int", "Смещение по оси Х (относительно центра вагона", true);
            WagonsCargo.shift_y = new DataBaseFieldInfo(WagonsCargo.TableName, "shift_y", "int", "Смещение по оси Y (относительно центра вагона", true);
            WagonsCargo.shift_z = new DataBaseFieldInfo(WagonsCargo.TableName, "shift_z", "int", "Смещение по оси Z (относительно пола вагона)", true);
            WagonsCargo.volume = new DataBaseFieldInfo(WagonsCargo.TableName, "volume", "int", "Объём груза", true);
            WagonsCargo.width = new DataBaseFieldInfo(WagonsCargo.TableName, "width", "int", "Средняя ширина груза", true);
            WagonsCargo.height = new DataBaseFieldInfo(WagonsCargo.TableName, "height", "int", "Средняя высота груза", true);
            WagonsCargo.length = new DataBaseFieldInfo(WagonsCargo.TableName, "length", "int", "Длина груза (вагона без бортов и сцепок)", true);
            WagonsCargo.width_max = new DataBaseFieldInfo(WagonsCargo.TableName, "width_max", "int", "Максимальная ширина груза", true);
            WagonsCargo.height_max = new DataBaseFieldInfo(WagonsCargo.TableName, "height_max", "int", "Максимальная высота груза", true);
            WagonsCargo.cargo_exist = new DataBaseFieldInfo(WagonsCargo.TableName, "cargo_exist", "int", "Наличие груза", true);
            WagonsCargo.cargo_clear = new DataBaseFieldInfo(WagonsCargo.TableName, "cargo_clear", "int", "Флаг очистки вагона для пустых вагонов", true);
            WagonsCargo.cargo_type = new DataBaseFieldInfo(WagonsCargo.TableName, "cargo_type", "int", "Тип груза", true);
            WagonsCargo.wagon_type = new DataBaseFieldInfo(WagonsCargo.TableName, "wagon_type", "int", "Тип вагона", true);
            WagonsCargo.wagon_model = new DataBaseFieldInfo(WagonsCargo.TableName, "wagon_model", "nvarchar(MAX)", "Модель вагона", true);
            WagonsCargo.floor_level = new DataBaseFieldInfo(WagonsCargo.TableName, "floor_level", "int", "Уровень пола вагона для полувагонов", true);

        }

        #region Структуры таблиц данных

        /// <summary>Таблица настроек</summary>
        public struct Config
        {

            /// <summary>Название таблицы</summary>
            public const string TableName = "config";

            /// <summary>Название параметра</summary>
            public static DataBaseFieldInfo ParamName;
            /// <summary>Значение параметра</summary>
            public static DataBaseFieldInfo ParamValue;

        }

        /// <summary>Таблица каталогов файлового хранилища</summary>
        public struct Directories
        {

            /// <summary>Название таблицы</summary>
            public const string TableName = "directories";

            /// <summary>Идентификатор каталога видеоархива</summary>
            public static DataBaseFieldInfo DirId;
            /// <summary>Путь к каталогу видеоархива</summary>
            public static DataBaseFieldInfo DirPath;
            /// <summary>Стутус каталога</summary>
            public static DataBaseFieldInfo DirStat;
            /// <summary>Лимит свободного места</summary>
            public static DataBaseFieldInfo FreeSpaceLimit;

        }

        /// <summary>Таблица событий</summary>
        public struct Eventlog
        {

            /// <summary>Название таблицы</summary>
            public const string TableName = "eventlog";

            /// <summary>Серийный номер события</summary>
            public static DataBaseFieldInfo Id;
            /// <summary>Дата и время события</summary>
            public static DataBaseFieldInfo EvDateTime;
            /// <summary>Идентификатор сообщения</summary>
            public static DataBaseFieldInfo MsgId;
            /// <summary>Источник события / наименование устройства</summary>
            public static DataBaseFieldInfo EvSource;
            /// <summary>Дополнительные данные события</summary>
            public static DataBaseFieldInfo EvData;
            /// <summary>Идентификатор оператора при котором было зарегитрировано событие</summary>
            public static DataBaseFieldInfo OpId;
            /// <summary>Наличие виде (для тревоги)</summary>
            public static DataBaseFieldInfo HasVideo;

        }

        /// <summary>Таблица названий событий</summary>
        public struct Messages
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "messages";

            /// <summary>Идентификатор типа события</summary>
            public static DataBaseFieldInfo MsgId;
            /// <summary>Наименование события</summary>
            public static DataBaseFieldInfo MsgText;

        }

        /// <summary>Таблица маркированных кадров</summary>
        public struct MarkedFrames
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "marked_frames";

            /// <summary>Идентификатор поезда</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>Идентификатор телекамеры</summary>
            public static DataBaseFieldInfo CameraId;
            /// <summary>Метка времени</summary>
            public static DataBaseFieldInfo TimeSpan;
        }

        /// <summary>Таблица натурных листов</summary>
        public struct Numbers
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "numbers";

            /// <summary>Идентификатор поезда</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>Порядковый номер вагона</summary>
            public static DataBaseFieldInfo Sn;
            /// <summary>Инвентарный номер</summary>
            public static DataBaseFieldInfo Inv;
        }

        /// <summary>Таблица информации о пользователях</summary>
        public struct Operators
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "operators";

            /// <summary>Идентификатор записи</summary>
            public static DataBaseFieldInfo OpId;
            /// <summary>Колонка "Логин"</summary>
            public static DataBaseFieldInfo OpLogin;
            /// <summary>Пароль</summary>
            public static DataBaseFieldInfo OpPassword;
            /// <summary>Описание пользователя</summary>
            public static DataBaseFieldInfo OpName;
            /// <summary>Статус пользователя</summary>
            public static DataBaseFieldInfo Status;
            /// <summary>Полномочия пользователя</summary>
            public static DataBaseFieldInfo Permissions;
            /// <summary>Идентфикатор ключа оператора</summary>
            public static DataBaseFieldInfo UsbKey;

        }

        /// <summary>Таблица статусов</summary>
        public struct Status
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "status";

            /// <summary>Тип статуса</summary>
            public static DataBaseFieldInfo DevName;
            /// <summary>Состояние</summary>
            public static DataBaseFieldInfo DevStat;
        }
        
        /// <summary>Таблица информации о составах</summary>
        public struct Trains
        {

            /// <summary>Название таблицы</summary>
            public const string TableName = "trains";

            /// <summary>Идентификатор пути</summary>
            public static DataBaseFieldInfo WayId;
            /// <summary>Идентификатор поезда</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>Дата и время начала записи</summary>
            public static DataBaseFieldInfo TimeBegin;
            /// <summary>Дата и время окончания записи</summary>
            public static DataBaseFieldInfo TimeEnd;
            /// <summary>Индекс состава</summary>
            public static DataBaseFieldInfo TimeIndex;
            /// <summary>Номер состава</summary>
            public static DataBaseFieldInfo TimeNumber;
            /// <summary>Путь</summary>
            public static DataBaseFieldInfo Way;
            /// <summary>Направление</summary>
            public static DataBaseFieldInfo Direction;
            /// <summary>Статус</summary>
            public static DataBaseFieldInfo Status;
            /// <summary>Идентификатор раздела видеохранилища</summary>
            public static DataBaseFieldInfo DirId;
            /// <summary>Идентификатор оператора производившего запись</summary>
            public static DataBaseFieldInfo OpId;
            /// <summary>Признак подтверждения принятия оператором</summary>
            public static DataBaseFieldInfo Accepted;
            /// <summary>Признак инверсии натурного листа с "головы" на "хвост"</summary>
            public static DataBaseFieldInfo InvertInvNum;

        }

        /// <summary>Таблица вагонов состава</summary>
        public struct Wagons
        {

            /// <summary>Название таблицы</summary>
            public const string TableName = "wagons";

            /// <summary>Идентификатор записи вагона в базе</summary>
            public static DataBaseFieldInfo WagonId;
            /// <summary>Идентификатор состава для вагона</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>Номер пересечения подвижной единицы в составе</summary>
            public static DataBaseFieldInfo WagonSn;
            /// <summary>Порядковый номер подвижной единицы в составе</summary>
            public static DataBaseFieldInfo SnSost;
            /// <summary>Признак типа вагона</summary>
            public static DataBaseFieldInfo Loco;
            /// <summary>Метка времени в миллисекундах начала вагона (смещение от начала видеозаписи)</summary>
            public static DataBaseFieldInfo TimeSpanBegin;
            /// <summary>Метка времени в миллисекундах окончания вагона (смещение от начала видеозаписи)</summary>
            public static DataBaseFieldInfo TimeSpanEnd;
            /// <summary>Метка времени в миллисекундах начала вагона (смещение от начала видеозаписи) по меткам времени от БС</summary>
            public static DataBaseFieldInfo TimeSpanBeginBS;
            /// <summary>Метка времени в миллисекундах окончания вагона (смещение от начала видеозаписи) по меткам времени от БС</summary>
            public static DataBaseFieldInfo TimeSpanEndBS;
            /// <summary>Скорость вагона на первой тележке</summary>
            public static DataBaseFieldInfo SpeedBegin;
            /// <summary>Скорость вагона на последней тележке</summary>
            public static DataBaseFieldInfo SpeedEnd;
            /// <summary>Направление вагона на первой тележке</summary>
            public static DataBaseFieldInfo DirectionBegin;
            /// <summary>Направление  вагона на последней тележке</summary>
            public static DataBaseFieldInfo DirectionEnd;
            /// <summary>Маска негабаритов</summary>
            public static DataBaseFieldInfo NGB;
            /// <summary>Инвентарный номер вагона введенный оператором</summary>
            public static DataBaseFieldInfo InvNum;
            /// <summary>Признак маркирования</summary>
            public static DataBaseFieldInfo Mark;
            /// <summary>Запрет ставить в состав</summary>
            public static DataBaseFieldInfo Banned;
            /// <summary>Комментарий</summary>
            public static DataBaseFieldInfo Comment;
            /// <summary>Дата и время последнего изменения</summary>
            public static DataBaseFieldInfo TimeChanged;

        }

        /// <summary>Таблица распознанных данных по вагону</summary>
        public struct WagonsOCR
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "wagons_ocr";

            /// <summary>Идентификатор записи в таблице</summary>
            public static DataBaseFieldInfo Id;
            /// <summary>Идентификатор поезда в системе АСКИН</summary>
            public static DataBaseFieldInfo OcrTrain;
            /// <summary>Идентификатор  вагона в таблице wagons</summary>
            public static DataBaseFieldInfo WagonId;
            /// <summary>Порядковый номер пересечения</summary>
            public static DataBaseFieldInfo Sn;
            /// <summary>Распознанный инвентарный номер</summary>
            public static DataBaseFieldInfo InvNum;
            /// <summary>Тип распознанного инвентарного номера</summary>
            public static DataBaseFieldInfo Type;
            /// <summary>Достоверность распознавания номера</summary>
            public static DataBaseFieldInfo Accuracy;
            /// <summary>Проверка оператором</summary>
            public static DataBaseFieldInfo Сheck;
        }

        /// <summary>Таблица грузов вагонов</summary>
        public struct WagonsCargo
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "wagons_cargo";

            /// <summary>Идентификатор записи в таблице</summary>
            public static DataBaseFieldInfo Id;
            /// <summary>Идентификатор вагона</summary>
            public static DataBaseFieldInfo WagId;
            /// <summary>Смещение по оси Х (относительно центра вагона</summary>
            public static DataBaseFieldInfo shift_x;
            /// <summary>Смещение по оси Y (относительно центра вагона</summary>
            public static DataBaseFieldInfo shift_y;
            /// <summary>Смещение по оси Z (относительно пола вагона)</summary>
            public static DataBaseFieldInfo shift_z;
            /// <summary>Объём груза</summary>
            public static DataBaseFieldInfo volume;
            /// <summary>Средняя ширина груза</summary>
            public static DataBaseFieldInfo width;
            /// <summary>Средняя высота груза</summary>
            public static DataBaseFieldInfo height;
            /// <summary>Длина груза (вагона без бортов и сцепок)</summary>
            public static DataBaseFieldInfo length;
            /// <summary>Максимальная ширина груза</summary>
            public static DataBaseFieldInfo width_max;
            /// <summary>Максимальная высота груза</summary>
            public static DataBaseFieldInfo height_max;
            /// <summary>Наличие груза</summary>
            public static DataBaseFieldInfo cargo_exist;
            /// <summary>Флаг очистки вагона для пустых вагонов</summary>
            public static DataBaseFieldInfo cargo_clear;
            /// <summary>Тип груза</summary>
            public static DataBaseFieldInfo cargo_type;
            /// <summary>Тип вагона</summary>
            public static DataBaseFieldInfo wagon_type;
            /// <summary>Модель вагона</summary>
            public static DataBaseFieldInfo wagon_model;
            /// <summary>Уровень пола вагона для полувагонов</summary>
            public static DataBaseFieldInfo floor_level;
        }

        #endregion

    }
}
