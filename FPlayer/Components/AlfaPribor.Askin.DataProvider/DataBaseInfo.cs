using System;
using System.Collections.Generic;
using System.Text;

/// <summary>Информация о базе данных</summary>
namespace AlfaPribor.ASKIN.DataBase
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
            Directories.DirType = new DataBaseFieldInfo(Directories.TableName, "DirType", "int", "Тип каталога", true);
            Directories.DirStat = new DataBaseFieldInfo(Directories.TableName, "DirStat", "int", "Статус каталога", true);

            //Таблицы событий
            Eventlog.Sn = new DataBaseFieldInfo(Eventlog.TableName, "Sn", "int", "Серийный номер события", false);
            Eventlog.EvTime = new DataBaseFieldInfo(Eventlog.TableName, "EvTime", "datetime", "Дата и время события", false);
            Eventlog.MsgId = new DataBaseFieldInfo(Eventlog.TableName, "MsgId", "int", "Идентификатор сообщения", false);
            Eventlog.EvSource = new DataBaseFieldInfo(Eventlog.TableName, "EvSource", "nvarchar(250)", "Источник события / наименование устройства", true);
            Eventlog.EvData = new DataBaseFieldInfo(Eventlog.TableName, "EvData", "nvarchar(MAX)", "Дополнительные данные события", true);

            //Таблица название сообщений
            Messages.MsgId = new DataBaseFieldInfo(Messages.TableName, "MsgId", "int", "Идентификатор типа события", false);
            Messages.MsgText = new DataBaseFieldInfo(Messages.TableName, "MsgText", "nvarchar(255)", "Наименование события", false);
            Messages.Category = new DataBaseFieldInfo(Messages.TableName, "Category", "int", "Категория события", false);

            //Таблица "Операторы"
            Operators.OpId = new DataBaseFieldInfo(Operators.TableName, "OpId", "int", "Идентификатор записи", false);
            Operators.OpLogin = new DataBaseFieldInfo(Operators.TableName, "OpLogin", "nvarchar(32)", "Колонка логин", false);
            Operators.OpPassword = new DataBaseFieldInfo(Operators.TableName, "OpPassword", "nvarchar(32)", "Пароль", false);
            Operators.FullName = new DataBaseFieldInfo(Operators.TableName, "FullName", "nvarchar(50)", "Описание пользователя", false);
            Operators.OpLevel = new DataBaseFieldInfo(Operators.TableName, "OpLevel", "int", "Полномочия пользователя", false);

            //Таблица статусов
            Status.Name = new DataBaseFieldInfo(Status.TableName, "Name", "nvarchar(50)", "Тип статуса", true);
            Status.DeviceStatus = new DataBaseFieldInfo(Status.TableName, "DeviceStatus", "nvarchar(50)", "Состояние", true);

            //Таблица информации о составе
            Trains.TrainId = new DataBaseFieldInfo(Trains.TableName, "TrainId", "int", "Идентификатор поезда", false);
            Trains.TimeBegin = new DataBaseFieldInfo(Trains.TableName, "TimeBegin", "datetime", "Дата и время начала записи", false);
            Trains.TimeEnd = new DataBaseFieldInfo(Trains.TableName, "TimeEnd", "datetime", "Дата и время окончания записи", true);
            Trains.Speed = new DataBaseFieldInfo(Trains.TableName, "Speed", "int", "Скорость", true);
            Trains.Direction = new DataBaseFieldInfo(Trains.TableName, "Direction", "int", "Направление", true);
            Trains.DirId = new DataBaseFieldInfo(Trains.TableName, "DirId", "int", "Идентификатор раздела видеохранилища", true);

            //Таблица информации о вагонах
            Wagons.Sn = new DataBaseFieldInfo(Wagons.TableName, "Sn", "int", "Идентификатор записи вагона в базе", true);
            Wagons.TrainId = new DataBaseFieldInfo(Wagons.TableName, "TrainId", "int", "Идентификатор состава для вагона", true);
            Wagons.WagonSn = new DataBaseFieldInfo(Wagons.TableName, "WagonSn", "int", "Порядковый номер вагона в составе", true);
            Wagons.InvNum = new DataBaseFieldInfo(Wagons.TableName, "InvNum", "nvarchar(10)", "Инвентарный номер вагона", true);
            Wagons.InvType = new DataBaseFieldInfo(Wagons.TableName, "InvType", "int", "Тип идентификации инвентарного номера", true);
            Wagons.RecordTime = new DataBaseFieldInfo(Wagons.TableName, "WeightTime", "datetime", "Дата и время записи вагона", true);
            Wagons.Comment = new DataBaseFieldInfo(Wagons.TableName, "Comment", "nvarchar(MAX)", "Комментарий", true);
            Wagons.TimeSpanBegin = new DataBaseFieldInfo(Wagons.TableName, "TimeSpanBegin", "int", "Метка времени начала вагона", true);
            Wagons.TimeSpan = new DataBaseFieldInfo(Wagons.TableName, "TimeSpan", "int", "Метка времени окончания вагона", true);
            Wagons.TimeChanged = new DataBaseFieldInfo(Wagons.TableName, "Changed", "datetime", "Дата и время последнего изменения", true);

            //Поезда натурных листов по прибытию
            TrainsArrival.Id = new DataBaseFieldInfo(TrainsArrival.TableName, "id", "int", "Идентификатор записи натурного листа состава по прибытию", true);
            TrainsArrival.CodeDirection = new DataBaseFieldInfo(TrainsArrival.TableName, "code_direction", "nvarchar(50)", "Код направления", true);
            TrainsArrival.CodeStation = new DataBaseFieldInfo(TrainsArrival.TableName, "code_station", "nvarchar(50)", "Код станции", true);
            TrainsArrival.Number = new DataBaseFieldInfo(TrainsArrival.TableName, "number", "nvarchar(4)", "Номер поезда", true);
            TrainsArrival.Index = new DataBaseFieldInfo(TrainsArrival.TableName, "number", "nvarchar(11)", "Индекс поезда", true);
            TrainsArrival.Feature = new DataBaseFieldInfo(TrainsArrival.TableName, "feature", "int", "Признак списывания (с головы, с хвоста)", true);
            TrainsArrival.Date = new DataBaseFieldInfo(Trains.TableName, "date", "datetime", "Ожидаемая дата и время прибытия", true);

            //Инвентарные номера вагонов по прибытию
            NumbersArrival.Id = new DataBaseFieldInfo(NumbersArrival.TableName, "id", "int", "Идентификатор записи вагона натурного листа по прибытию", true);
            NumbersArrival.TrainId = new DataBaseFieldInfo(NumbersArrival.TableName, "train_id", "int", "Идентификатор поезда натурного листа по прибытию", true);
            NumbersArrival.Sn = new DataBaseFieldInfo(NumbersArrival.TableName, "sn", "int", "Порядковый номер вагона в натурном листе по прибытию", true);
            NumbersArrival.Number = new DataBaseFieldInfo(NumbersArrival.TableName, "number", "nvarchar(8)", "Инвентарный номер вагона в натурном листе по прибытию", true);

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
            /// <summary>Тип каталога</summary>
            public static DataBaseFieldInfo DirType;
            /// <summary>Статус каталога</summary>
            public static DataBaseFieldInfo DirStat;

        }

        /// <summary>Таблица событий</summary>
        public struct Eventlog
        {

            /// <summary>Название таблицы</summary>
            public const string TableName = "eventlog";

            /// <summary>Серийный номер события</summary>
            public static DataBaseFieldInfo Sn;
            /// <summary>Дата и время события</summary>
            public static DataBaseFieldInfo EvTime;
            /// <summary>Идентификатор сообщения</summary>
            public static DataBaseFieldInfo MsgId;
            /// <summary>Источник события / наименование устройства</summary>
            public static DataBaseFieldInfo EvSource;
            /// <summary>Дополнительные данные события</summary>
            public static DataBaseFieldInfo EvData;

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
            /// <summary>Категория события</summary>
            public static DataBaseFieldInfo Category;

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
            public static DataBaseFieldInfo FullName;
            /// <summary>Полномочия пользователя</summary>
            public static DataBaseFieldInfo OpLevel;

        }

        /// <summary>Таблица статуса устройств</summary>
        public struct Status
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "status";

            /// <summary>Тип статуса</summary>
            public static DataBaseFieldInfo Name;
            /// <summary>Состояние</summary>
            public static DataBaseFieldInfo DeviceStatus;
        }

        /// <summary>Таблица информации о составах</summary>
        public struct Trains
        {

            /// <summary>Название таблицы</summary>
            public const string TableName = "trains";

            /// <summary>Идентификатор поезда</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>Дата и время начала записи</summary>
            public static DataBaseFieldInfo TimeBegin;
            /// <summary>Дата и время окончания записи</summary>
            public static DataBaseFieldInfo TimeEnd;
            /// <summary>Скорость</summary>
            public static DataBaseFieldInfo Speed;
            /// <summary>Направление</summary>
            public static DataBaseFieldInfo Direction;
            /// <summary>Идентификатор раздела видеохранилища</summary>
            public static DataBaseFieldInfo DirId;

        }

        /// <summary>Таблица вагонов состава</summary>
        public struct Wagons
        {

            /// <summary>Название таблицы</summary>
            public const string TableName = "wagons";

            /// <summary>Идентификатор записи вагона в базе</summary>
            public static DataBaseFieldInfo Sn;
            /// <summary>Идентификатор состава для вагона</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>Порядковый номер вагона в составе</summary>
            public static DataBaseFieldInfo WagonSn;
            /// <summary>Инвентарный номер вагона</summary>
            public static DataBaseFieldInfo InvNum;
            /// <summary>Тип инвентарного номера вагона (распознан, введен вручную...)</summary>
            public static DataBaseFieldInfo InvType;
            /// <summary>Дата и время записи вагона</summary>
            public static DataBaseFieldInfo RecordTime;
            /// <summary>Комментарий</summary>
            public static DataBaseFieldInfo Comment;
            /// <summary>Метка времени в миллисекундах окончания вагона (смещение от начала видеозаписи)</summary>
            public static DataBaseFieldInfo TimeSpan;
            /// <summary>Метка времени в миллисекундах начала вагона (смещение от начала видеозаписи)</summary>
            public static DataBaseFieldInfo TimeSpanBegin;
            /// <summary>Дата и время последнего изменения</summary>
            public static DataBaseFieldInfo TimeChanged;

        }

        /// <summary>Таблица поездов по прибытию</summary>
        public struct TrainsArrival
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "trains_arrival";

            /// <summary>Идентификатор записи</summary>
            public static DataBaseFieldInfo Id;
            /// <summary>Код направления</summary>
            public static DataBaseFieldInfo CodeDirection;
            /// <summary>Код станции</summary>
            public static DataBaseFieldInfo CodeStation;
            /// <summary>Номер состава</summary>
            public static DataBaseFieldInfo Number;
            /// <summary>Индекс состава</summary>
            public static DataBaseFieldInfo Index;
            /// <summary>Признак направления списывания</summary>
            public static DataBaseFieldInfo Feature;
            /// <summary>Ожидаемая дата прибытия</summary>
            public static DataBaseFieldInfo Date;
        }

        /// <summary>Таблица вагонов по прибытию</summary>
        public struct NumbersArrival
        {
            /// <summary>Название таблицы</summary>
            public const string TableName = "numbers_arrival";

            /// <summary>Идентификатор записи</summary>
            public static DataBaseFieldInfo Id;
            /// <summary>Идентификатор натурного листа поезда по прибытию</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>Порядковый номер вагона</summary>
            public static DataBaseFieldInfo Sn;
            /// <summary>Инвентарный номер</summary>
            public static DataBaseFieldInfo Number;
        }

        #endregion

    }

}