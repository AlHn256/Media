using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Reflection;

using AlfaPribor.DataHelper;

using AlfaPribor.ASKO.DataBase;
using AlfaPribor.ASKO.Data;
using System.Text;

namespace AlfaPribor.ASKO.DataProvider
{

    /// <summary>Реализация провайдера данных АСКО для MS SQL server</summary>
    public partial class DataProvider
    {

        /// <summary>Версия базы данных</summary>
        const int db_version = 9;

        #region Fields

        /// <summary>Строка с параметрами подключения у базе данных</summary>
        protected string _ConnectionString;

        /// <summary>Имя конфигурационного параметра, содержащего номер текущей версии базы данных</summary>
        static string DbVersionParamName = "Config.Revision";

        #endregion

        #region Common Methods

        /// <summary>Конструктор класса. Инициирует объект класса строкой подключения к БД</summary>
        /// <param name="connection_str">Строка с параметрами подключения к базе данных</param>
        /// <exception cref="System.ArgumentException">Не задана строка подключения к БД</exception>
        public DataProvider(string connection_str)
        {
            if (string.IsNullOrEmpty(connection_str))
            {
                throw new ArgumentException();
            }
            _ConnectionString = connection_str;
        }

        /// <summary>Объединяет два условия запроса логической связкой AND</summary>
        /// <param name="str1">Первое условие запроса</param>
        /// <param name="str2">Второе условие запроса</param>
        /// <returns>Модифицированная строка запроса</returns>
        protected string AND(string str1, string str2)
        {
            string result = str1;
            if (!string.IsNullOrEmpty(str2))
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += "AND\n" + str2;
                }
                else
                {
                    result = str2;
                }
            }
            return result;
        }

        /// <summary>Объединяет два условия запроса логической связкой OR</summary>
        /// <param name="str1">Первое условие запроса</param>
        /// <param name="str2">Второе условие запроса</param>
        /// <returns>Модифицированная строка запроса</returns>
        protected string OR(string str1, string str2)
        {
            string result = str1;
            if (!string.IsNullOrEmpty(str2))
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += "OR\n" + str2;
                }
                else
                {
                    result = str2;
                }
            }
            return result;
        }

        #region Status

        /// <summary>Получить коммуникационный статус устройства</summary>
        /// <param name="dev_name">Имя устройства в таблице статуса устройств (Status)</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Не задано имя устройства</exception>
        /// <returns>Статус устройства</returns>
        DevState GetDevStat(string dev_name)
        {
            string state_name = GetStringStat(dev_name);
            if (string.IsNullOrEmpty(state_name)) return DevState.unknown;
            for (var status = DevState.unknown; status <= DevState.none; ++status)
            {
                if (status.ToString() == state_name) return status;
            }
            return DevState.unknown;
        }

        /// <summary>Получить строковое представление коммуникационный статус устройства</summary>
        /// <param name="dev_name">Имя устройства в таблице статуса устройств (Status)</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Не задано имя устройства</exception>
        /// <returns>Строковое представление статуса устройства или null, если запись о статусе устройства не найдена</returns>
        string GetStringStat(string dev_name)
        {
            if (string.IsNullOrEmpty(dev_name))
            {
                throw new ArgumentException("dev_name");
            }
            string result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[DevStat]" },
                    From = "[dbo].[status]",
                    Where = "[DevName] = @name"
                };
                object status_name = _DataHelper.ExecuteScalar(sql, new SqlParameter("name", dev_name));
                if (status_name != null)
                {
                    return (string)status_name;
                }
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Установить статус сервера</summary>
        /// <param name="dev_name">Имя устройства в таблице статуса устройств (Status)</param>
        /// <param name="status">Значение коммуникационного статуса</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Не задан статус или имя устройства</exception>
        void SetStat(string dev_name, string status)
        {
            if (string.IsNullOrEmpty(dev_name))
            {
                throw new ArgumentException("Invalid argument!", "dev_name");
            }
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("Invalid argument!", "status");
            }
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "COUNT([DevName])" },
                    From = "[dbo].[status]",
                    Where = "[DevName] = @name"
                };
                SqlParameter nameParam = new SqlParameter("name", dev_name);
                SqlParameter statParam = new SqlParameter("stat", status);
                bool is_exist = (int)_DataHelper.ExecuteScalar(sql, nameParam) > 0;
                nameParam = (SqlParameter)CopyParam(nameParam);
                if (is_exist)
                {
                    sql = (string)new SqlUpdateBuilder
                    {
                        Table = "[dbo].[status]",
                        Fields = new string[] { "[DevStat]" },
                        Values = new string[] { "@stat" },
                        Where = "[DevName] = @name"
                    };
                    _DataHelper.ExecuteNoneQuery(sql, nameParam, statParam);
                }
                else
                {
                    sql = (string)new SqlInsertBuilder
                    {
                        Table = "[dbo].[status]",
                        Fields = new string[] { "[DevName]", "[DevStat]" },
                        Values = new string[] { "@name", "@stat" }
                    };
                    _DataHelper.ExecuteNoneQuery(sql, nameParam, statParam);
                }
                _DataHelper.Commit();
            }
        }

        #endregion

        /// <summary>Копирует параметры для нового запроса к базе данных</summary>
        /// <param name="source">Список существующих параметров</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на исходный список параметров</exception>
        /// <returns>Список новых параметров, идентичных заданным</returns>
        protected List<DbParameter> CopyParams(List<DbParameter> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            List<DbParameter> dest = new List<DbParameter>(source.Count);
            foreach (var param in source)
            {
                dest.Add(new SqlParameter(param.ParameterName, param.Value));
            }
            return dest;
        }

        /// <summary>Копирует параметры для нового запроса к базе данных</summary>
        /// <param name="source">Список существующих параметров</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на исходный список параметров</exception>
        /// <returns>Список новых параметров, идентичных заданным</returns>
        protected DbParameter[] CopyParams(params DbParameter[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            DbParameter[] dest = new DbParameter[source.Length];
            for (int i = 0; i < dest.Length; ++i)
            {
                dest[i] = new SqlParameter(source[i].ParameterName, source[i].Value);
            }
            return dest;
        }

        /// <summary>Копирует параметр для нового запроса к базе данных</summary>
        /// <param name="source">Параметров sql-запроса</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на исходный параметр</exception>
        /// <returns>Новый параметр, идентичный заданному</returns>
        protected DbParameter CopyParam(DbParameter source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new SqlParameter(source.ParameterName, source.Value);
        }

        #region Check

        /// <summary>Проверяет данные события перед занесением в базу данных</summary>
        /// <param name="data">Данные события</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        void CheckEvent(EventData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.EventTime == DateTime.MinValue || data.MsgId < 1)
            {
                throw new ArgumentException("Illegal value of property!", "data");
            }
        }

        /// <summary>Проверяет данные поезда перед занесением в базу данных</summary>
        /// <param name="data">Данные поезда</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        void CheckTrain(TrainData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.BeginTime == DateTime.MinValue)
            {
                throw new ArgumentException("Illegal value of property!", "data");
            }
        }

        /// <summary>Проверяет данные оператора перед занесением в базу данных</summary>
        /// <param name="data">Данные оператора</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        void CheckOperator(OperatorData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrEmpty(data.Login) || string.IsNullOrEmpty(data.Password) || string.IsNullOrEmpty(data.OpName))
            {
                throw new ArgumentException("Illegal value of property!", "data");
            }
        }

        /// <summary>Проверяет данные каталога видеоархива перед занесением в базу данных</summary>
        /// <param name="data">Данные каталога видеоархива</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        void CheckDirectory(DirectoryData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Path == null || data.Status == DirStat.Unknown)
            {
                throw new ArgumentException("Illegal value of property!", "data");
            }
        }

        /// <summary>Проверяет данные записи журнала взвешивания вагонов перед занесением в базу данных</summary>
        /// <param name="data">Данные взвешивания вагона</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        void CheckWagon(WagonData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.TrainId < 1 || data.WagId < 1 || data.TimeSpanEnd < 0)
            {
                throw new ArgumentException("Некорректное значение данных!", "data");
            }
        }

        #endregion

        /// <summary>Преобразует целое число в статус каталога</summary>
        /// <param name="dir_stat">Целочисленное представление статуса каталога</param>
        /// <returns>Статус каталога</returns>
        DirStat GetDirStat(int dir_stat)
        {
            return (DirStat)dir_stat;
        }

        #endregion

        #region Properties

        /// <summary>Строка с параметрами подключения к базе данных</summary>
        public string ConnectionString
        {
            get { return _ConnectionString; }
        }

        /// <summary>Версия базы данных, с которой может корректно работать провайдер данных</summary>
        public static int DBversion
        {
            get { return db_version; }
        }

        #endregion

        #region DataProvider members

        #region Trains

        /// <summary>Получить данные поезда/отцепа с указанным идентификатором</summary>
        /// <param name="train_id">Идентификатор поезда/отцепа</param>
        /// <param name="first_loco">Подсчитывать число локомотивов только в начале состава</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Данные поезда, null - нет поезда с таким идентификатором</returns>
        public TrainData GetTrainData(int train_id, bool first_loco)
        {
            if (train_id < 1) throw new ArgumentOutOfRangeException("train_id");
            TrainData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                try
                {
                    SqlParameter train_id_param = new SqlParameter("train_id", train_id);
                    DataTable dataTable = _DataHelper.ExecuteCommand(SelectTrainSQLv6(1, first_loco, -1, -1, 0) + " AND [trains].[TrainId] = @train_id", 
                                                                     new SqlParameter("train_id", train_id));
                    IEnumerable<TrainData> TrainDataQuery = TrainData(dataTable);
                    List<TrainData> TrainDataList = TrainDataQuery.ToList();
                    if (TrainDataList.Count > 0) result = TrainDataList[0];
                }
                catch (Exception e)
                { }
                //Завершение транзакции
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить список всех поездов (дополнительный)</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainsList(out int total)
        {
            return GetTrainsList(DateTime.MinValue, DateTime.MinValue, "", "", "", "", 0, out total, false);
        }

        /// <summary>Получить список поездов соответствующих критериям (дополнительный)</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда (нестрогое соответствие)</param>
        /// <param name="train_index">Индекс поезда (нестрогое соответствие)</param>
        /// <param name="wag_inv">Инвентарный номер вагона, входящего в состав поеда</param>
        /// <param name="way">Путь прохождения</param>
        /// <param name="limit">Ограничени количества записей в выборке</param>
        /// <param name="total">Общее количество поездов, соответствующих критерию</param>
        /// <param name="first_loco">Подсчитывать число локомотивов (подвижные единицы с кодом 1) только в начале состава, иначе - все локомотивы</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainsList(DateTime begin, DateTime end, string train_num, string train_index, string wag_inv,
                                              string way, int limit, out int total, bool first_loco)
        {
            return GetTrainsList(begin, end, train_num, train_index, wag_inv, way, -1, 0, 0, limit, 0, 0, out total, first_loco);
        }

        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда (нестрогое соответствие)</param>
        /// <param name="train_index">Индекс поезда (нестрогое соответствие)</param>
        /// <param name="wag_inv">Инвентарный номер вагона, входящего в состав поеда</param>
        /// <param name="way">Путь прохождения</param>
        /// <param name="train_status">Статус поезда: 0 - не важно, 1 - записан, 2 - не записан</param>
        /// <param name="accepted">Признак обработки оператором: 0 - не важно, 1 - обработанные, 2 - не обработанные</param>
        /// <param name="direction">Направление движения состава: 0 - не важно, 1 - прямое, 2 - обратное</param>
        /// <param name="start">Стартовая запись выборки, по порядку (0 - игнорируется)</param>
        /// <param name="stop">Конечная запись выборки, по порядку (0 - игнорируется)</param>
        /// <param name="limit">Ограничени количества записей в выборке</param>
        /// <param name="total">Общее количество поездов, соответствующих критерию</param>
        /// <param name="first_loco">Подсчитывать число локомотивов (подвижные единицы с кодом 1) только в начале состава, иначе - все локомотивы</param>
        /// <param name="cargoMinHeight"></param>
        /// <param name="cargoMaxHeight"></param>
        /// <param name="isCargoNGBT">Превышение минимальной высоты груза. Значения - 1 не  задействован, 0, 1</param>
        /// <param name="isCargoMaxHeight">Правышение максимальной высоты груза. Значения - 1 не  задействован, 0, 1</param>
        /// <param name="MinWagons">Минимальное число вагонов</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainsList(DateTime begin, DateTime end, string train_num, string train_index, string wag_inv, 
                                              string way, int train_status, int accepted, int direction, int limit, int start, int stop, out int total, 
                                              bool first_loco, int cargoMinHeight = 0, int cargoMaxHeight = 0,int isCargoNGBT = -1, int isCargoMaxHeight = -1, 
                                              int MinWagons = 0, List<int> trains_id = null)
        {
            List<TrainData> result = null;
            total = 0;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                
                //Where
                string where_str = WhereTrainsSQL(ref _params, begin, end, train_num, train_index, wag_inv, way, train_status, accepted, direction, /*trains_id*/ trains_id,
                                                  cargoMinHeight, cargoMaxHeight, isCargoNGBT, isCargoMaxHeight);
                string sql = "WITH Ordered AS (";
                //Select
                sql += SelectTrainSQLv6(limit, first_loco, isCargoNGBT, isCargoMaxHeight, MinWagons);
                //Where
                if (where_str != null) sql += " AND " + where_str;
                sql += " ORDER BY [TimeBegin] DESC) ";
                //Выборка порции
                sql += " SELECT * FROM Ordered ";
                if (start > 0) sql += " WHERE RowNumber >= " + start.ToString();
                if (stop > 0) sql += " AND RowNumber <= " + stop.ToString();

                try
                {
                    DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params, 10000);
                    IEnumerable<TrainData> TrainDataQuery = TrainData(dataTable);
                    //Список результирующих записей
                    result = TrainDataQuery.Cast<TrainData>().ToList();

                    //Определение общего числа для постраничного запроса
                    if (limit != 0 || start != 0 || stop != 0)
                    {
                        // Запрашиваем общее количество записей, удовлетворяющих заданному критерию
                        sql = (string)new SqlSelectBuilder
                        {
                            Fields = new string[] { "Count([TrainId])" },
                            From = "[trains]"
                        };                        
                        total = (int)_DataHelper.ExecuteScalar(sql, CopyParams(_params));
                    }
                    else total = result.Count;

                }
                //Завершение транзации
                catch (Exception e)
                { }

                //Завершение транзации
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Запрос данных составов</summary>
        /// <param name="dataTable">Таблица запрошенных данных</param>
        IEnumerable<TrainData> TrainData(DataTable dataTable)
        {
            IEnumerable<TrainData> TrainDataQuery =
                    from trains in dataTable.AsEnumerable()
                    let train_id = trains.IsNull("TrainId") ? 0 : (int)trains["TrainId"]
                    let train_num = trains.IsNull("TrainNumber") ? String.Empty : (string)trains["TrainNumber"]
                    let train_index = trains.IsNull("TrainIndex") ? String.Empty : (string)trains["TrainIndex"]
                    let begin_time = trains.IsNull("TimeBegin") ? DateTime.MinValue : (DateTime)trains["TimeBegin"]
                    let end_time = trains.IsNull("TimeEnd") ? DateTime.MinValue : (DateTime)trains["TimeEnd"]
                    let direction = trains.IsNull("Direction") ? 0 : (int)trains["Direction"]
                    let dir_id = trains.IsNull("DirId") ? 0 : (int)trains["DirId"]
                    let dir_path = trains.IsNull("DirPath") ? String.Empty : (string)trains["DirPath"]
                    let status = trains.IsNull("Status") ? 0 : (int)trains["Status"]
                    let w_count = trains.IsNull("WagonsCount") ? 0 : (int)trains["WagonsCount"]
                    let l_count = trains.IsNull("LocoCount") ? 0 : (int)trains["LocoCount"]
                    let accept = trains.IsNull("Accepted") ? 0 : (int)trains["Accepted"]
                    let speed = trains.IsNull("SpeedEnd") ? 0 : (int)trains["SpeedEnd"]
                    let way = trains.IsNull("Way") ? String.Empty : (string)trains["Way"]
                    let way_id = trains.IsNull("WayId") ? 0 : (int)trains["WayId"]
                    let w_ngb_count = trains.IsNull("WagonsNgbCount") ? 0 : (int)trains["WagonsNgbCount"]
                    let invertNL = trains.IsNull("InvertInvNum") ? false : Convert.ToBoolean(trains["InvertInvNum"])
                    let has_video = System.IO.File.Exists(dir_path + "\\" + train_id.ToString() + ".frames") &&
                                    System.IO.File.Exists(dir_path + "\\" + train_id.ToString() + ".index")
                    let op_id = trains.IsNull("OpId") ? 0 : (int)trains["OpId"]
                    let op_name = trains.IsNull("OpName") ? String.Empty : (string)trains["OpName"]
                    let comment = trains.IsNull("Comment") ? String.Empty : (string)trains["Comment"]
                    let record_offset = trains.IsNull("RecordOffset") ? 0 : (int)trains["RecordOffset"]
                    let is_numbers = trains.IsNull("is_numbers") ? false : Convert.ToBoolean(trains["is_numbers"])
                    let is_models = trains.IsNull("is_models") ? false : Convert.ToBoolean(trains["is_models"])
                    let is_ocr = trains.IsNull("is_ocr") ? false : Convert.ToBoolean(trains["is_ocr"])
                    select new TrainData
                    {
                        Id = train_id,
                        TrainNum = train_num,
                        TrainIndex = train_index,
                        BeginTime = begin_time,
                        EndTime = end_time,
                        Direction = direction,
                        DirId = dir_id,
                        DirPath = dir_path,
                        Status = status,
                        WagonsCount = w_count,
                        LocoCount = l_count,
                        Accepted = accept, //(accept == 1),//Принят оператором
                        WagonsNgbCount = w_ngb_count,
                        InvertInvNum = invertNL,
                        Speed = speed,
                        Way = way,
                        WayId = way_id,
                        HasVideo = has_video,
                        OpId = op_id,
                        OpName = op_name,
                        Comment = comment,
                        RecordOffset = record_offset,
                        IsNumbers = is_numbers,
                        IsModels = is_models,
                        IsOcr = is_ocr
                    };
            return TrainDataQuery;
        }

        /* //Старые методы запросов поездов, наверное не понадобятся
        /// <summary>Формирование строки выборки составов</summary>
        /// <param name="limit">Ограничение выборки</param>
        /// <param name="first_loco">Учитывать локомотивы только в начале состава</param>
        /// <param name="isCargoNGBT">Выбрать вагоны с превышением минимальной высоты груза. -1 не выбирать, 0, 1</param>
        /// <param name="isCargoMaxHeight">Выбрать вагоны с правышением максимальной высоты груза. -1 не выбирать, 0, 1</param>
        /// <returns></returns>
        string SelectTrainSQL(int limit, bool first_loco, int isCargoNGBT, int isCargoMaxHeight, int MinWagons)
        {
            string sql = "SELECT ";
            if (limit > 0) sql += " TOP(" + limit.ToString() + ")";
            else sql += " TOP(1000000) \r\n";//TOP должен быть обязательно
            //Поля запроса
            sql += " ROW_NUMBER() OVER (ORDER BY [TimeBegin] DESC) AS RowNumber, \r\n" +
                   " [dbo].[v_trains].[TrainId], \r\n" +
                   " [TrainNumber], \r\n" +
                   " [TrainIndex], \r\n" +
                   " [Accepted], \r\n" +
                   " [Way], \r\n" +
                   " [WayId], \r\n" +
                   " [OpId], \r\n" +
                   " [OpName], \r\n" +
                   " [InvertInvNum], \r\n" +
                   " [TimeBegin], \r\n" +
                   " [TimeEnd], \r\n" +
                   " [Direction], \r\n" +
                   " [DirId], \r\n" +
                   " [DirPath], \r\n" +
                   " [Status], \r\n" +
                   " [Comment], \r\n" +
                   " [RecordOffset], \r\n" +
                   " COALESCE(SumTrainSpeed / nullif(WagonsCount, 0), 0) AS SpeedEnd, \r\n";
            //Число локомотивов и общее (с локомотивами) число вагонов
            if (first_loco) sql += " COALESCE((MinWagon - FirstWagon), (SELECT COUNT(WagonSn) FROM Wagons WHERE [dbo].[v_trains].[TrainId] = [dbo].[wagons].[TrainId] AND Loco = 1)) AS ";
            sql += " LocoCount, \r\n WagonsCount, \r\n ";
            //Число негабаритных вагонов
            if (first_loco)
                 sql += " (SELECT COUNT(WagonSn) FROM wagons WHERE [dbo].[v_trains].[TrainId] = [dbo].[wagons].[TrainId] AND Ngb <> 0 AND WagonSn > (MinWagon - 1)) AS WagonsNgbCount \r\n";
            else sql += " (SELECT COUNT(WagonSn) FROM wagons WHERE [dbo].[v_trains].[TrainId] = [dbo].[wagons].[TrainId] AND Ngb <> 0 AND Loco <> 1) AS WagonsNgbCount \r\n";
            //Грузы
            if (isCargoNGBT > -1) sql += ", CargoNGBT \r\n";
            if (isCargoMaxHeight > -1) sql += ", CargoMaxHeightNGBT \r\n";
            //From
            sql += " FROM [dbo].[v_trains] \r\n" +
                   " LEFT JOIN \r\n " +
                   " (SELECT [dbo].[wagons].[TrainId], \r\n" + 
                   " COUNT([dbo].[wagons].WagonSn) AS WagonsCount, \r\n";
            //Более корректная выборка числа вагонов и локомотивов
            if (first_loco)
                 sql += " (SELECT MIN(wagons.WagonSn) FROM wagons WHERE TrainId = [dbo].[wagons].[TrainId] AND Loco <> 1) AS MinWagon, \r\n" +  //Минимальный номер вагона (первый вагон)
                        " (SELECT MIN(wagons.WagonSn) FROM wagons WHERE [TrainId] = [dbo].[wagons].[TrainId]) AS FirstWagon, \r\n";             //Номер первой подвижной единице в составе
            else sql += " (SELECT COUNT(wagons.WagonSn) FROM wagons WHERE TrainId = [dbo].[wagons].[TrainId] AND Loco = 1) AS LocoCount, \r\n";
            //Информация о грузе
            if (isCargoNGBT > -1) sql += " (SELECT COUNT(wc.WagId) from wagons_cargo wc where wc.WagId IN(select w.WagonId from wagons w where w.TrainId = [dbo].[wagons].[TrainId] and w.Loco < 1) and wc.cargo_clear = 0 and wc.height_max > @cargoMinHeight and wc.cargo_type = 0) as CargoNGBT, \r\n";
            if (isCargoMaxHeight > -1) sql += " (SELECT COUNT(wc.WagId) from wagons_cargo wc where wc.WagId IN(select w.WagonId from wagons w where w.TrainId = [dbo].[wagons].[TrainId] and w.Loco < 1) and wc.cargo_exist = 1 and wc.height_max > @cargoMaxHeight ) as CargoMaxHeightNGBT, \r\n";
            //Сумма скоростей
            sql += " SUM([dbo].[wagons].[SpeedEnd]) AS SumTrainSpeed \r\n";
            //From
            sql += " FROM [dbo].[wagons], [dbo].[v_trains] \r\n" +
                   " WHERE [dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId]  \r\n" +
                   " GROUP BY [dbo].[wagons].[TrainId]) wag ON wag.TrainId = v_trains.TrainId \r\n";
            sql += " WHERE [dbo].[v_trains].[TrainId] > 0 ";
            //Минимальное число вагонов
            if (MinWagons > 0) sql += " AND WagonsCount >= " + MinWagons.ToString();
            return sql;
        }

        /// <summary>Экспериментальный SELECT</summary>
        /// <param name="limit"></param>
        /// <param name="first_loco"></param>
        /// <param name="isCargoNGBT"></param>
        /// <param name="isCargoMaxHeight"></param>
        /// <param name="MinWagons"></param>
        /// <returns></returns>
        string SelectTrainSQL2(int limit, bool first_loco, int isCargoNGBT, int isCargoMaxHeight, int MinWagons)
        {
            string sql = "SELECT ";
            if (limit > 0) sql += " TOP(" + limit.ToString() + ")";
            else sql += " TOP(1000000) \r\n";//TOP должен быть обязательно
            //Поля запроса
            sql += " ROW_NUMBER() OVER (ORDER BY [TimeBegin] DESC) AS RowNumber, \r\n" +
                   " [dbo].[v_trains].[TrainId], \r\n" +
                   " [TrainNumber], \r\n" +
                   " [TrainIndex], \r\n" +
                   " [Accepted], \r\n" +
                   " [Way], \r\n" +
                   " [WayId], \r\n" +
                   " [OpId], \r\n" +
                   " [OpName], \r\n" +
                   " [InvertInvNum], \r\n" +
                   " [TimeBegin], \r\n" +
                   " [TimeEnd], \r\n" +
                   " [Direction], \r\n" +
                   " [DirId], \r\n" +
                   " [DirPath], \r\n" +
                   " [Status], \r\n" +
                   " [Comment], \r\n" +
                   " [RecordOffset], \r\n" +
                   " COALESCE(SumTrainSpeed / nullif(WagonsCount, 0), 0) AS SpeedEnd, \r\n";
            //Число локомотивов и общее (с локомотивами) число вагонов
            if (first_loco) sql += " COALESCE((MinWagon - FirstWagon), (SELECT COUNT(*) FROM Wagons WHERE [dbo].[v_trains].[TrainId] = [dbo].[wagons].[TrainId] AND Loco = 1)) AS ";
            sql += " LocoCount, \r\n WagonsCount, \r\n ";
            //Число негабаритных вагонов
            if (first_loco)
                sql += " (SELECT COUNT(*) FROM wagons WHERE [dbo].[v_trains].[TrainId] = [dbo].[wagons].[TrainId] AND Ngb <> 0 AND WagonSn > (MinWagon - 1)) AS WagonsNgbCount \r\n";
            else sql += " (SELECT COUNT(*) FROM wagons WHERE [dbo].[v_trains].[TrainId] = [dbo].[wagons].[TrainId] AND Ngb <> 0 AND Loco <> 1) AS WagonsNgbCount \r\n";
            //Грузы
            if (isCargoNGBT > -1) sql += ", CargoNGBT \r\n";
            if (isCargoMaxHeight > -1) sql += ", CargoMaxHeightNGBT \r\n";
            //From
            sql += " FROM [dbo].[v_trains] \r\n" +
                   " LEFT JOIN \r\n " +
                   " (SELECT [dbo].[wagons].[TrainId], \r\n" +
                   " COUNT(*) AS WagonsCount, \r\n";
            //Более корректная выборка числа вагонов и локомотивов
            if (first_loco)
                sql += " (SELECT MIN(wagons.WagonSn) FROM wagons WHERE TrainId = [dbo].[wagons].[TrainId] AND Loco <> 1) AS MinWagon, \r\n" +  //Минимальный номер вагона (первый вагон)
                       " (SELECT MIN(wagons.WagonSn) FROM wagons WHERE [TrainId] = [dbo].[wagons].[TrainId]) AS FirstWagon, \r\n";             //Номер первой подвижной единице в составе
            else sql += " (SELECT COUNT(*) FROM wagons WHERE TrainId = [dbo].[wagons].[TrainId] AND Loco = 1) AS LocoCount, \r\n";
            //Информация о грузе
            if (isCargoNGBT > -1) sql += " (SELECT COUNT(wc.WagId) from wagons_cargo wc where wc.WagId IN(select w.WagonId from wagons w where w.TrainId = [dbo].[wagons].[TrainId] and w.Loco < 1) and wc.cargo_clear = 0 and wc.height_max > @cargoMinHeight and wc.cargo_type = 0) as CargoNGBT, \r\n";
            if (isCargoMaxHeight > -1) sql += " (SELECT COUNT(wc.WagId) from wagons_cargo wc where wc.WagId IN(select w.WagonId from wagons w where w.TrainId = [dbo].[wagons].[TrainId] and w.Loco < 1) and wc.cargo_exist = 1 and wc.height_max > @cargoMaxHeight ) as CargoMaxHeightNGBT, \r\n";
            //Сумма скоростей
            sql += " SUM([dbo].[wagons].[SpeedEnd]) AS SumTrainSpeed \r\n";
            //From
            sql += " FROM [dbo].[wagons], [dbo].[v_trains] \r\n" +
                   " WHERE [dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId]  \r\n" +
                   " GROUP BY [dbo].[wagons].[TrainId]) wag ON wag.TrainId = v_trains.TrainId \r\n";
            sql += " WHERE [dbo].[v_trains].[TrainId] > 0 ";
            //Минимальное число вагонов
            if (MinWagons > 0) sql += " AND WagonsCount >= " + MinWagons.ToString();
            return sql;
        }

        /// <summary>Экспериментальный SELECT - из запроса удалено представление v_trains - работает намного быстрее !!!</summary>
        /// <param name="limit"></param>
        /// <param name="first_loco"></param>
        /// <param name="isCargoNGBT"></param>
        /// <param name="isCargoMaxHeight"></param>
        /// <param name="MinWagons"></param>
        /// <returns></returns>
        string SelectTrainSQL4(int limit, bool first_loco, int isCargoNGBT, int isCargoMaxHeight, int MinWagons)
        {
            string sql = "SELECT ";
            if (limit > 0) sql += " TOP(" + limit.ToString() + ")";
            else sql += " TOP(1000000) \r\n";//TOP должен быть обязательно
            //Поля запроса
            sql += " ROW_NUMBER() OVER (ORDER BY [TimeBegin] DESC) AS RowNumber, \r\n" +
                   " [trains].[TrainId], \r\n" +
                   " [TrainNumber], \r\n" +
                   " [TrainIndex], \r\n" +
                   " [Accepted], \r\n" +
                   " [Way], \r\n" +
                   " [WayId], \r\n" +
                   " [trains].[OpId], \r\n" +
                   " [operators].[OpName], \r\n" +
                   " [InvertInvNum], \r\n" +
                   " [TimeBegin], \r\n" +
                   " [TimeEnd], \r\n" +
                   " [Direction], \r\n" +
                   " [trains].[DirId], \r\n" +
                   " [DirPath], \r\n" +
                   " [trains].[Status], \r\n" +
                   " [Comment], \r\n" +
                   " [RecordOffset], \r\n" +
                   " COALESCE(SumTrainSpeed / nullif(WagonsCount, 0), 0) AS SpeedEnd, \r\n";
            //Число локомотивов и общее (с локомотивами) число вагонов
            if (first_loco) sql += " COALESCE((MinWagon - FirstWagon), (SELECT COUNT(*) FROM Wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Loco = 1)) AS ";
            sql += " LocoCount, \r\n WagonsCount, \r\n ";
            //Число негабаритных вагонов
            if (first_loco)
                 sql += " (SELECT COUNT(*) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Ngb <> 0 AND WagonSn >= MinWagon) AS WagonsNgbCount \r\n";
            else sql += " (SELECT COUNT(*) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Ngb <> 0 AND Loco <> 1) AS WagonsNgbCount \r\n";
            //Грузы
            if (isCargoNGBT > -1) sql += ", CargoNGBT \r\n";
            if (isCargoMaxHeight > -1) sql += ", CargoMaxHeightNGBT \r\n";
            //From
            sql += " FROM [trains] \r\n" +
                   " LEFT OUTER JOIN operators ON trains.OpId = operators.OpId \r\n" + 
                   " LEFT OUTER JOIN directories ON trains.DirId = directories.DirId \r\n" + 
                   " LEFT JOIN \r\n " +
                   " (SELECT [dbo].[wagons].[TrainId], \r\n" +
                   " COUNT(*) AS WagonsCount, \r\n";
            //Более корректная выборка числа вагонов и локомотивов
            if (first_loco)
                 sql += " (SELECT MIN(wagons.WagonSn) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Loco <> 1) AS MinWagon, \r\n" +  //Минимальный номер вагона (первый вагон)
                        " (SELECT MIN(wagons.WagonSn) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId]) AS FirstWagon, \r\n";             //Номер первой подвижной единице в составе
            else sql += " (SELECT COUNT(*) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Loco = 1) AS LocoCount, \r\n";
            //Информация о грузе
            if (isCargoNGBT > -1) sql += " (SELECT COUNT(wc.WagId) from wagons_cargo wc where wc.WagId IN(select w.WagonId from wagons w where w.TrainId = [dbo].[wagons].[TrainId] and w.Loco < 1) and wc.cargo_clear = 0 and wc.height_max > @cargoMinHeight and wc.cargo_type = 0) as CargoNGBT, \r\n";
            if (isCargoMaxHeight > -1) sql += " (SELECT COUNT(wc.WagId) from wagons_cargo wc where wc.WagId IN(select w.WagonId from wagons w where w.TrainId = [dbo].[wagons].[TrainId] and w.Loco < 1) and wc.cargo_exist = 1 and wc.height_max > @cargoMaxHeight ) as CargoMaxHeightNGBT, \r\n";
            //Сумма скоростей
            sql += " SUM([wagons].[SpeedEnd]) AS SumTrainSpeed \r\n";
            //From
            sql += " FROM [wagons], [trains] \r\n" +
                   " WHERE [wagons].[TrainId] = [trains].[TrainId]  \r\n" +
                   " GROUP BY [wagons].[TrainId], [trains].[TrainId]) wag ON wag.TrainId = [trains].TrainId \r\n";
            sql += " WHERE [trains].[TrainId] > 0 ";
            //Минимальное число вагонов
            if (MinWagons > 0) sql += " AND WagonsCount >= " + MinWagons.ToString();
            return sql;
        }
        */

        /// <summary>SELECT составов версия 6</summary>
        /// <param name="limit"></param>
        /// <param name="first_loco"></param>
        /// <param name="isCargoNGBT"></param>
        /// <param name="isCargoMaxHeight"></param>
        /// <param name="MinWagons"></param>
        /// <returns></returns>
        string SelectTrainSQLv6(int limit, bool first_loco, int isCargoNGBT, int isCargoMaxHeight, int MinWagons)
        {
            string sql = "SELECT ";
            if (limit > 0) sql += " TOP(" + limit.ToString() + ")";
            else sql += " TOP(1000000) \r\n";//TOP должен быть обязательно
            //Поля запроса
            sql += " ROW_NUMBER() OVER (ORDER BY [TimeBegin] DESC) AS RowNumber, \r\n" +
                   " [trains].[TrainId], \r\n" +
                   " [TrainNumber], \r\n" +
                   " [TrainIndex], \r\n" +
                   " [Accepted], \r\n" +
                   " [Way], \r\n" +
                   " [WayId], \r\n" +
                   " [trains].[OpId], \r\n" +
                   " [operators].[OpName], \r\n" +
                   " [InvertInvNum], \r\n" +
                   " [TimeBegin], \r\n" +
                   " [TimeEnd], \r\n" +
                   " [Direction], \r\n" +
                   " [trains].[DirId], \r\n" +
                   " [DirPath], \r\n" +
                   " [trains].[Status], \r\n" +
                   " [Comment], \r\n" +
                   " [RecordOffset], \r\n" +
                   " COALESCE(SumTrainSpeed / nullif(WagonsCount, 0), 0) AS SpeedEnd, \r\n";
            //Число локомотивов и общее (с локомотивами) число вагонов
            if (first_loco) sql += " COALESCE((MinWagon - FirstWagon), (SELECT COUNT(*) FROM Wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Loco = 1)) AS ";
            sql += " LocoCount, \r\n WagonsCount, \r\n ";
            #region Число негабаритных вагонов
            string s1 = "WagonSn >= MinWagon";
            if (!first_loco) s1 = "Loco <> 1";
            //Обычные негабариты (старые)
            int ngb_version = GetIntConfigParam("Way.0.ExtendedNGB", 0);
            if (ngb_version == 0) sql += " (SELECT COUNT(*) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Ngb <> 0 AND " + s1 + ") AS WagonsNgbCount, \r\n";
            //Расширенные негабариты версии 1
            if (ngb_version == 1)
                sql += " (SELECT COUNT(*) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND \r\n" +
                       //Учет маски степеней негабаритности
                       " (((Ngb & 1) + (Ngb & 2) + (Ngb & 4) + (Ngb & 64) + (Ngb & 128) + (Ngb & 256) + (Ngb & 512) + (Ngb & 1024)+ (Ngb & 2048)) > 0) " +
                       " AND " + s1 + ") AS WagonsNgbCount, \r\n";
            //Расширенные негабариты версии 2
            if (ngb_version == 2)
                sql += " (SELECT COUNT(*) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND \r\n" +
                       //Учет маски степеней негабаритности
                       " (((Ngb & 1) + (Ngb & 2) + (Ngb & 4) + (Ngb & 8) + (Ngb & 16) + (Ngb & 32) + (Ngb & 64) + (Ngb & 128) + (Ngb & 256) + (Ngb & 512) + (Ngb & 1024)) > 0) " +
                       " AND " + s1 + ") AS WagonsNgbCount, \r\n";
            #endregion
            //Наличие натурного листа
            sql += " CAST((SELECT TOP(1) numbers.Sn FROM numbers WHERE[trains].[TrainId] = [numbers].[TrainId]) AS bit) AS is_numbers, \r\n";
            //Наличие моделей вагонов
            sql += " CAST((SELECT TOP(1) wagons_models.wagon_sn FROM wagons_models WHERE[trains].[TrainId] = [wagons_models].[train_id]) AS bit) AS is_models, \r\n";
            //Наличие распознанных номеров
            //!!!!!!! сделать поле id состава в таблице wagons_ocr чтобы упростить запрос
            sql += " CAST((SELECT TOP(1) id FROM wagons_ocr " + 
                   " WHERE wagons_ocr.wagon_id = (SELECT TOP(1) ([wagons].[WagonId]) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId])) as bit) as is_ocr";
            //Грузы
            if (isCargoNGBT > -1) sql += ", CargoNGBT \r\n";
            if (isCargoMaxHeight > -1) sql += ", CargoMaxHeightNGBT \r\n";
            //From
            sql += " FROM [trains] \r\n" +
                   " LEFT OUTER JOIN operators ON trains.OpId = operators.OpId \r\n" +
                   " LEFT OUTER JOIN directories ON trains.DirId = directories.DirId \r\n" +
                   " LEFT JOIN \r\n " +
                   " (SELECT [dbo].[trains].[TrainId], \r\n" +
                   " COUNT(*) AS WagonsCount, \r\n";
            //Более корректная выборка числа вагонов и локомотивов
            if (first_loco)
                 sql += " (SELECT MIN(wagons.WagonSn) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Loco <> 1) AS MinWagon, \r\n" + //Минимальный номер вагона (первый вагон)
                        " (SELECT MIN(wagons.WagonSn) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId]) AS FirstWagon, \r\n";              //Номер первой подвижной единице в составе
            else sql += " (SELECT COUNT(*) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Loco = 1) AS LocoCount, \r\n";
            //Информация о грузе
            if (isCargoNGBT > -1) sql += " (SELECT COUNT(wc.WagId) from wagons_cargo wc where wc.WagId IN(select w.WagonId from wagons w where w.TrainId = [dbo].[wagons].[TrainId] and w.Loco < 1) and wc.cargo_clear = 0 and wc.height_max > @cargoMinHeight and wc.cargo_type = 0) as CargoNGBT, \r\n";
            if (isCargoMaxHeight > -1) sql += " (SELECT COUNT(wc.WagId) from wagons_cargo wc where wc.WagId IN(select w.WagonId from wagons w where w.TrainId = [dbo].[wagons].[TrainId] and w.Loco < 1) and wc.cargo_exist = 1 and wc.height_max > @cargoMaxHeight ) as CargoMaxHeightNGBT, \r\n";
            //Сумма скоростей
            sql += " SUM([wagons].[SpeedEnd]) AS SumTrainSpeed \r\n";
            //From
            sql += " FROM [wagons], [trains] \r\n" +
                   " WHERE [wagons].[TrainId] = [trains].[TrainId]  \r\n" +
                   " GROUP BY [trains].[TrainId]) wag ON wag.TrainId = [trains].TrainId \r\n";
            sql += " WHERE [trains].[TrainId] > 0 ";
            //Минимальное число вагонов
            if (MinWagons > 0) sql += " AND WagonsCount >= " + MinWagons.ToString();
            return sql;
        }

        /// <summary>Формирование строки Where запроса составов</summary>
        /// <param name="_params">Формируемые параметры</param>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда (нестрогое соответствие)</param>
        /// <param name="train_index">Индекс поезда (нестрогое соответствие)</param>
        /// <param name="wag_inv">Инвентарный номер вагона, входящего в состав поеда</param>
        /// <param name="way">Путь прохождения</param>
        /// <param name="train_status">Статус поезда: 0 - не важно, 1 - записан, 2 - не записан</param>
        /// <param name="accepted">Признак обработки оператором: 0 - не важно, 1 - обработанные, 2 - не обработанные</param>
        /// <param name="direction">Направление движения состава: 0 - не важно, 1 - прямое, 2 - обратное</param>
        /// <param name="trains_id">Идентификаторы составов</param>
        /// <param name="cargoMinHeight"></param>
        /// <param name="cargoMaxHeight"></param>
        /// <param name="isCargoNGBT"></param>
        /// <param name="isCargoMaxHeight"></param>
        /// <returns></returns>
        string WhereTrainsSQL(ref List<DbParameter> _params, DateTime begin, DateTime end, string train_num, string train_index, string wag_inv,
                              string way, int train_status, int accepted, int direction, List<int> trains_id,
                              int cargoMinHeight = 0, int cargoMaxHeight = 0, int isCargoNGBT = -1, int isCargoMaxHeight = -1)
        {
            _params = new List<DbParameter>();
            string where_str = null;
            //Идентификаторы составов
            if (trains_id != null && trains_id.Count > 0)
            {
                where_str = AND(where_str, " ([trains].[TrainId] IN (");
                for (int i = 0; i < trains_id.Count; i++)
                {
                    if (i > 0) where_str += ",";
                    where_str += trains_id[i].ToString();
                }
                where_str += ")) ";
            }
            //Начало записи
            if (begin != DateTime.MinValue)
            {
                where_str = "([TimeBegin] >= @begin) \r\n";
                _params.Add(new SqlParameter("begin", begin));
            }
            //Конец записи
            if (end != DateTime.MinValue)
            {
                where_str = AND(where_str, " ([TimeBegin] <= @end) \r\n");
                _params.Add(new SqlParameter("end", end));
            }
            //Номер
            if (!string.IsNullOrEmpty(train_num))
            {
                where_str = AND(where_str, " ([TrainNumber] like \'%" + train_num + "%\')\r\n");
            }
            //Индекс
            if (!string.IsNullOrEmpty(train_index))
            {
                where_str = AND(where_str, " ([TrainIndex] like \'%" + train_index + "%\')\r\n");
            }
            //Инвентарный номер вагона
            if (!string.IsNullOrEmpty(wag_inv))
            {
                string findWagQuery = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[TrainId]" },
                    From = "[numbers]",
                    Where = "[numbers].[Inv] like \'%" + wag_inv + "%\'"
                };
                where_str = AND(where_str, "(([trains].[TrainId] IN (" + findWagQuery + "))\r\n");
                string findWagQuery2 = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[TrainId]" },
                    From = "[wagons]",
                    Where = "[wagons].[InvNum] like \'%" + wag_inv + "%\'"
                };
                where_str = OR(where_str, "([trains].[TrainId] IN (" + findWagQuery2 + ")))\r\n");
                //Запрос из трабицы натурных листов
                string ocrInvStr = @"([trains].[TrainId] IN ( 
                                        SELECT [TrainId] FROM [wagons] WHERE([wagons].[InvNum] IS NULL OR Len([wagons].[InvNum]) = 0 ) 
                                        AND [wagons].WagonId IN(SELECT ww.wagon_id FROM wagons_ocr ww where ww.[inv_num] like '%" + wag_inv + "%')))\r\n";
                where_str = OR(where_str, ocrInvStr);
            }
            //Путь
            if (!string.IsNullOrEmpty(way))
            {
                where_str = AND(where_str, "([trains].[Way] like \'%" + way + "%\')\r\n");
            }
            //Статус
            if (train_status > -1)
            {
                where_str = AND(where_str, "([trains].[Status] = @train_status)\r\n");
                _params.Add(new SqlParameter("train_status", train_status));
            }
            //Обработка
            if (accepted > 0)
            {
                where_str = AND(where_str, "([trains].[Accepted] = @train_accepted)\r\n");
                if (accepted == 1) _params.Add(new SqlParameter("train_accepted", 1));
                if (accepted == 2) _params.Add(new SqlParameter("train_accepted", 0));
            }
            //Направление
            if (direction > 0)
            {
                where_str = AND(where_str, "([trains].[Direction] = @train_direct)\r\n");
                _params.Add(new SqlParameter("train_direct", direction - 1));
            }
            //Груз
            if (isCargoNGBT > -1)
            {
                where_str = AND(where_str, "(CargoNGBT" + (isCargoNGBT > 0 ? ">" : " = ") + "0)");
                _params.Add(new SqlParameter("cargoMinHeight", cargoMinHeight));
            }
            if (isCargoMaxHeight > -1)
            {
                where_str = AND(where_str, "(CargoMaxHeightNGBT " + (isCargoMaxHeight > 0 ? ">" : "=") + " 0 )");
                _params.Add(new SqlParameter("cargoMaxHeight", cargoMaxHeight));
            }
            return where_str;
        }

        /// <summary>Получить число поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда (нестрогое соответствие)</param>
        /// <param name="train_index">Индекс поезда (нестрогое соответствие)</param>
        /// <param name="wag_inv">Инвентарный номер вагона, входящего в состав поеда</param>
        /// <param name="way">Путь прохождения</param>
        /// <param name="train_status">Статус поезда (-1 - любой статус)</param>
        /// <param name="accepted">Признак обработки оператором (0 - не важно, 1 - обработанные, 2 - не обработанные)</param>
        /// <param name="train_direct">Направление движения состава</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public int GetTrainsCount(DateTime begin, DateTime end, string train_num, string train_index, string wag_inv, 
                                  string way, int train_status, int accepted, int direction,
                                  int cargoMinHeight = 0, int cargoMaxHeight = 0, int isCargoNGBT = -1, int isCargoMaxHeight = -1)
        {
            int cnt = 0;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                //Where
                string where_str = WhereTrainsSQL(ref _params, begin, end, train_num, train_index, wag_inv, way, train_status, accepted, direction, null,
                                                  cargoMinHeight, cargoMaxHeight, isCargoNGBT, isCargoMaxHeight);
                //Sql
                string sql = "SELECT COUNT (*) as cnt FROM \r\n" + 
                             "(SELECT [trains].[TrainId] FROM [trains] \r\n" + 
                             "LEFT JOIN (SELECT [wagons].[TrainId] FROM [wagons], [trains] \r\n" + 
                             "WHERE [wagons].[TrainId] = [trains].[TrainId] \r\n" +
                             "GROUP BY [wagons].[TrainId]) wag ON wag.TrainId = [trains].TrainId \r\n";
                if (where_str != null) sql += " WHERE " + where_str;
                sql += ")  AS t";

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                if (dataTable != null && dataTable.Rows.Count > 0) cnt = (int)dataTable.Rows[0]["cnt"];

                _DataHelper.Commit();
            }
            return cnt;
        }

        /// <summary>Изменить данные о поезде в базе данных</summary>
        /// <param name="data">Измененные данные о поезде</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        public bool ModifyTrain(TrainData data)
        {
            try { CheckTrain(data); } catch { return false; }
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                List<string> fields = new List<string>() { "[TimeBegin]", "[TimeEnd]", "[Direction]", 
                                                           "[TrainNumber]", "[TrainIndex]", "[Status]",
                                                           "[DirId]", "[Way]", "[InvertInvNum]", "[Comment]", "[RecordOffset]" };
                List<string> values = new List<string>() { "@begin", "@end", "@direction", 
                                                           "@trainnum", "@trainindex", "@status",
                                                           "@dir_id", "@way", "@invertNL", "@comment", "@recordoffset" };
                if (data.OpId != 0)
                {
                    fields.Add("[OpId]");
                    values.Add("@opid");
                    _params.Add(new SqlParameter("opid", (object)data.OpId));
                }
                string sql = (string)new SqlUpdateBuilder
                {
                    Table = "[dbo].[trains]",
                    Fields = fields.ToArray(),
                    Values = values.ToArray(),
                    Where = "[TrainId] = @train_id"
                };
                _params.Add(new SqlParameter("train_id", data.Id));
                _params.Add(new SqlParameter("begin", data.BeginTime));
                _params.Add(new SqlParameter("end", data.EndTime != DateTime.MinValue ? (object)data.EndTime : (object)DBNull.Value));
                _params.Add(new SqlParameter("direction", data.Direction));
                _params.Add(new SqlParameter("trainnum", data.TrainNum != null ? (object)data.TrainNum : (object)DBNull.Value));
                _params.Add(new SqlParameter("trainindex", data.TrainIndex != null ? (object)data.TrainIndex : (object)DBNull.Value));
                _params.Add(new SqlParameter("status", data.Status));
                _params.Add(new SqlParameter("dir_id", data.DirId > 0 ? (object)data.DirId : (object)DBNull.Value));
                _params.Add(new SqlParameter("way", data.Way != null ? (object)data.Way : (object)DBNull.Value));
                _params.Add(new SqlParameter("invertNL", (object)Convert.ToInt32(data.InvertInvNum)));
                _params.Add(new SqlParameter("comment", data.Comment != null ? (object)data.Comment : (object)DBNull.Value));
                _params.Add(new SqlParameter("recordoffset", data.RecordOffset));

                int result = _DataHelper.ExecuteNoneQuery(sql, _params);
                if (result == 0) return false;
                _DataHelper.Commit();
            }
            return true;
        }
        
        /// <summary>Установить идентификатор оператора, приявшего состав</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <param name="operator_id">Идентификатор оператора</param>
        /// <returns>Результат операции</returns>
        public bool AcceptTrain(int train_id, int operator_id)
        {
            if (train_id < 1)
            {
                return false;
                throw new ArgumentOutOfRangeException("train_id");
            }
            if (operator_id < 1)
            {
                return false;
                throw new ArgumentOutOfRangeException("operator_id");
            }
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    string sql = "UPDATE trains SET Accepted = " + operator_id.ToString() + " WHERE TrainId = " + train_id;
                    List<DbParameter> _params = new List<DbParameter>();
                    //_params.Add(new SqlParameter("train_id", train_id));
                    //_params.Add(new SqlParameter("Accepted", operator_id));
                    if (_DataHelper.ExecuteNoneQuery(sql, _params) == 0) return false;
                    _DataHelper.Commit();
                }
                catch { return false; }
            }
            return true;
        }

        /// <summary>Добавить запись о взешивании поезда в базу данных</summary>
        /// <param name="data">Данные поезда</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        /// <remarks>В случае успешного выполнения ссылка data будет указывать на новый объект,
        /// который будет содержать идентификатор добавленного поезда в базе данных</remarks>
        public bool AddTrain(ref TrainData data)
        {
            CheckTrain(data);
            TrainData result = data;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    List<DbParameter> _params = new List<DbParameter>();

                    //Дата и время начала прохожденя состава
                    SqlParameter param_date = new SqlParameter("TimeBegin", SqlDbType.DateTime2);
                    param_date.Value = data.BeginTime;
                    _params.Add(param_date);

                    //Направление
                    _params.Add(new SqlParameter("Direction", data.Direction));

                    //Идентификатор каталога видео
                    if (data.DirId < 1) _params.Add(new SqlParameter("DirId", null));
                    else _params.Add(new SqlParameter("DirId", data.DirId));

                    //Идентификатор состава
                    SqlParameter paramTrainId = new SqlParameter
                    {
                        ParameterName = "TrainId",
                        Value = data.Id,
                        Direction = ParameterDirection.Output
                    };
                    _params.Add(paramTrainId);

                    //Путь
                    _params.Add(new SqlParameter("WayId", data.WayId));

                    //Выполнение процедуры
                    _DataHelper.ExecuteStoredProcedure("[dbo].[add_train]", _params);
                    result = new TrainData
                    {
                        BeginTime = data.BeginTime,
                        Direction = data.Direction,
                        DirId = data.DirId,
                        DirPath = data.DirPath,
                        EndTime = data.EndTime,
                        Id = (int)paramTrainId.Value,
                        WagonsCount = data.WagonsCount,
                        WayId = data.WayId
                    };
                    _DataHelper.Commit();
                }
                catch { return false; }
            }
            data = result;
            return true;
        }

        /// <summary>Удалить все данные поезда (включая вагоны и отладочные данные)</summary>
        /// <param name="train_id">Идентификатор поезда в системе АСКО СВ</param>
        /// <exception cref="System.Data.SqlClient.SqlException">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение входного аргумента</exception>
        public bool DeleteTrain(int train_id)
        {
            if (train_id < 1)
            {
                return false;
                throw new ArgumentOutOfRangeException("id");
            }
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    string sql = (string)new SqlDeleteBuilder { Table = "[dbo].[trains]", Where = "[TrainId] = @id" };
                    if (_DataHelper.ExecuteNoneQuery(sql, new SqlParameter("id", train_id)) == 0) return false;
                    _DataHelper.Commit();
                }
                catch { return false; }
            }
            return true;
        }

        /// <summary>Обновление всех "записываемых" поездов до "записанных"</summary>
        /// <returns>Результат операции</returns>
        public bool UpdateRecordingTrainsToRecorded()
        {
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    List<DbParameter> _params = new List<DbParameter>();
                    List<string> fields = new List<string>() { "[status]" };
                    List<string> values = new List<string>() { "@st" };
                    string sql = (string)new SqlUpdateBuilder
                    {
                        Table = "[dbo].[trains]",
                        Fields = fields.ToArray(),
                        Values = values.ToArray(),
                        Where = "[status] = 0"
                    };
                    _params.Add(new SqlParameter("st", 1));
                    int result = _DataHelper.ExecuteNoneQuery(sql, _params);
                    if (result == 0) return false;
                    _DataHelper.Commit();
                }
                catch { return false; }
            }
            return true;
        }

        /// <summary>Получение статуса инверсии натурного листа</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <param name="invert_num">Инверсия натурного листа</param>
        /// <returns>Результат запроса данных поезда</returns>
        bool GetTrainInvertNumbers(int train_id, ref bool invert_num)
        {
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = "SELECT InvertInvNum FROM trains WHERE TrainId = @train_id";
                SqlParameter train_id_param = new SqlParameter("train_id", train_id);
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, train_id_param);
                _DataHelper.Commit();
                if (dataTable != null && dataTable.Rows.Count == 1)
                {
                    invert_num = (int)dataTable.Rows[0]["InvertInvNum"] == 1;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Wagons

        /// <summary>Добавить вагон</summary>
        /// <param name="data">Данные вагона</param>
        /// <exception cref="System.ArgumentNullException">Не задан объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойства объекта данных</exception>
        /// <exception cref="System.Exception">Не задан провайдер данных или ошибка обращения к базе данных</exception>
        /// <returns>Серийный номер вагона</returns>
        public int AddWagon(ref WagonData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            WagonData result = new WagonData(data);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("TrainId", data.TrainId));
                _params.Add(new SqlParameter("Sn", data.Sn));
                _params.Add(new SqlParameter("SnSost", data.SnSost));
                _params.Add(new SqlParameter("Loco", data.Loco));
                _params.Add(new SqlParameter("InvNum", data.InvNumber));
                _params.Add(new SqlParameter("TimeSpanBegin", data.TimeSpanBegin));
                _params.Add(new SqlParameter("TimeSpanEnd", data.TimeSpanEnd));
                _params.Add(new SqlParameter("TimeSpanBeginBS", data.TimeSpanBeginBS));
                _params.Add(new SqlParameter("TimeSpanEndBS", data.TimeSpanEndBS));
                _params.Add(new SqlParameter("SpeedBegin", data.SpeedBegin));
                _params.Add(new SqlParameter("SpeedEnd", data.SpeedEnd));
                _params.Add(new SqlParameter("DirectionBegin", data.DirectionBegin));
                _params.Add(new SqlParameter("DirectionEnd", data.DirectionEnd));
                _params.Add(new SqlParameter("Ngb", data.Ngb));
                _params.Add(new SqlParameter("Mark", data.Mark));
                _params.Add(new SqlParameter("Comment", data.Comment));
                _params.Add(new SqlParameter("TimeChanged", data.TimeChanged));
                //Выходной параметр - идентификатор добавленного вагона
                SqlParameter paramWagId = new SqlParameter
                {
                    ParameterName = "WagId",
                    Value = data.WagId,
                    Direction = ParameterDirection.Output
                };
                _params.Add(paramWagId);
                //Выполнение хранимой процедуры
                DataTable t = _DataHelper.ExecuteStoredProcedure("[dbo].[add_wagon]", _params);

                result = new WagonData
                {
                    WagId = (int)paramWagId.Value,
                    TrainId = data.TrainId,
                    Sn = data.Sn,
                    SnSost = data.SnSost,
                    Loco = data.Loco,
                    InvNumber = data.InvNumber,
                    TimeSpanBegin = data.TimeSpanBegin,
                    TimeSpanEnd = data.TimeSpanEnd,
                    TimeSpanBeginBS = data.TimeSpanBeginBS,
                    TimeSpanEndBS = data.TimeSpanEndBS,
                    SpeedBegin = data.SpeedBegin,
                    SpeedEnd = data.SpeedEnd,
                    DirectionBegin = data.DirectionBegin,
                    DirectionEnd = data.DirectionEnd,
                    Ngb = data.Ngb,
                    Mark = data.Mark,
                    Comment = data.Comment,
                    TimeChanged = data.TimeChanged
                };
                _DataHelper.Commit();
            }
            data = result;
            return result.WagId;
        }

        public bool InvertNL(int wag_id)
        {
            bool result = false;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[InvertInvNum]"},
                    From = "[dbo].[trains], [dbo].[wagons]",
                    Where = "[dbo].[wagons].[TrainId] = [dbo].[trains].[TrainId] AND [dbo].[wagons].[WagonId] = @wagid"
                };
                SqlParameter sn_param = new SqlParameter("wagid", wag_id);
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, sn_param);

                result = dataTable.Rows[0].IsNull("InvertInvNum") ? false : Convert.ToBoolean(dataTable.Rows[0]["InvertInvNum"]);
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Поля выборки из таблицы "wagons"</summary>
        string[] WagonFields = new string[] {   "[v_wagons].[WagonId]", "[v_wagons].[WagonSn]", "[v_wagons].[SnSost]",
                                                "[v_wagons].[Loco]", "[v_wagons].[TrainId]", "[v_wagons].[InvNum]", 
                                                "[v_wagons].[TimeSpanBegin]", "[v_wagons].[TimeSpanEnd]", 
                                                "[v_wagons].[TimeSpanBeginBS]", "[v_wagons].[TimeSpanEndBS]", 
                                                "[v_wagons].[SpeedBegin]", "[v_wagons].[SpeedEnd]", 
                                                "[v_wagons].[DirectionBegin]", "[v_wagons].[DirectionEnd]", 
                                                "[v_wagons].[Ngb]", "[v_wagons].[Mark]", "[v_wagons].[Comment]", 
                                                "[v_wagons].[Banned]", "[v_wagons].[TimeChanged]", 
                                                //распознанные данные
                                                "[v_wagons].[ocr_inv_num]",
                                                "[v_wagons].[ocr_type]", "[v_wagons].[ocr_accuracy]",
                                                "[v_wagons].[ocr_check]", "[v_wagons].[ocr_train]",
                                                //натурный лист
                                                "[numbers].[Inv]" };

        /// <summary>Получение объектов вагонов из результатов выборки вагонов</summary>
        /// <param name="dataTable">Выбранная таблица данных</param>
        IEnumerable<WagonData> WagonQuerySelect(DataTable dataTable)
        {
            IEnumerable<WagonData> WagonDataQuery =
                    
                    #region Wagon main data

                    from wagons in dataTable.AsEnumerable()
                    let wag_id = (int)wagons["WagonId"]
                    let sn = (int)wagons["WagonSn"]
                    let sn_sost = wagons.IsNull("SnSost") ? 0 : (int)wagons["SnSost"]
                    let loco = wagons.IsNull("Loco") ? 0 : (int)wagons["Loco"]
                    let train_id = (int)wagons["TrainId"]
                    let inv_num = wagons.IsNull("InvNum") ? (wagons.IsNull("ocr_inv_num") ? string.Empty : (string)wagons["ocr_inv_num"]) : (string)wagons["InvNum"]
                    let inv_num_nl = wagons.IsNull("Inv") ? string.Empty : (string)wagons["Inv"]

                    let time_span_begin = wagons.IsNull("TimeSpanBegin") ? 0 : (int)wagons["TimeSpanBegin"]
                    let time_span_end = wagons.IsNull("TimeSpanEnd") ? 0 : (int)wagons["TimeSpanEnd"]
                    let time_span_begin_bs = wagons.IsNull("TimeSpanBeginBS") ? 0 : (int)wagons["TimeSpanBeginBS"]
                    let time_span_end_bs = wagons.IsNull("TimeSpanEndBS") ? 0 : (int)wagons["TimeSpanEndBS"]
                    let speed_begin = wagons.IsNull("SpeedBegin") ? 0 : (int)wagons["SpeedBegin"]
                    let speed_end = wagons.IsNull("SpeedEnd") ? 0 : (int)wagons["SpeedEnd"]
                    let direction_begin = wagons.IsNull("DirectionBegin") ? 0 : (int)wagons["DirectionBegin"]
                    let direction_end = wagons.IsNull("DirectionEnd") ? 0 : (int)wagons["DirectionEnd"]
                    let ngb = wagons.IsNull("Ngb") ? 0 : (int)wagons["Ngb"]
                    let mark = wagons.IsNull("Mark") ? 0 : (int)wagons["Mark"]
                    let banned = wagons.IsNull("Banned") ? 0 : (int)wagons["Banned"]
                    let comment = wagons.IsNull("Comment") ? string.Empty : (string)wagons["Comment"]
                    let time_changed = wagons.IsNull("TimeChanged") ? DateTime.MinValue : (DateTime)wagons["TimeChanged"]

                    #endregion

                    #region Askin Data

                    let ocr_inv_num = wagons.IsNull("ocr_inv_num") ? string.Empty : (string)wagons["ocr_inv_num"]
                    let ocr_type = wagons.IsNull("ocr_type") ? 0 : (int)wagons["ocr_type"]
                    let ocr_acc = wagons.IsNull("ocr_accuracy") ? 0 : (int)wagons["ocr_accuracy"]
                    let trainAskinId = wagons.IsNull("ocr_train") ? -1 : (int)wagons["ocr_train"]
                    let ocr_userCheck = wagons.IsNull("ocr_check") ? 0 : (int)wagons["ocr_check"]

                    #endregion

                    select new WagonData
                    {
                        Sn = sn,
                        SnSost = sn_sost,
                        Loco = loco,
                        TrainId = train_id,
                        WagId = wag_id,
                        InvNumber = inv_num,
                        InvNumByNL = inv_num_nl,
                        TimeSpanBegin = time_span_begin,
                        TimeSpanEnd = time_span_end,
                        TimeSpanBeginBS = time_span_begin_bs,
                        TimeSpanEndBS = time_span_end_bs,
                        SpeedBegin = speed_begin,
                        SpeedEnd = speed_end,
                        DirectionBegin = direction_begin,
                        DirectionEnd = direction_end,
                        Ngb = ngb,
                        Mark = mark == 1,
                        Banned = banned == 1,
                        TimeChanged = time_changed,
                        Comment = comment,
                        //ASKIN
                        Ocr_InvNumber = ocr_inv_num,
                        Ocr_InvType = ocr_type,
                        Ocr_Accuracy = ocr_acc,
                        Ocr_TrainId = trainAskinId,
                        Ocr_UserCheck = ocr_userCheck
                    };
            return WagonDataQuery;
        }

        IEnumerable<NLTableData> NLTableDataDataSelect(DataTable dataTable)
        {
            IEnumerable<NLTableData> natListDataQuery =
                  from nlTableField in dataTable.AsEnumerable()

                  let trainID = (int)nlTableField["TrainId"]
                  let sn = (int)nlTableField["Sn"]
                  let inv = (string)nlTableField["Inv"]

                  select new NLTableData {

                      TrainID = trainID,
                      WagonSN = sn,
                      InvNum = inv
                  };
            return natListDataQuery;
        }

        /// <summary>Получить запись журнала вагонов по идентификатору вагона</summary>
        /// <param name="wagon_id">Идентификаторов записи</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Запись журнала вагонов, null - нет вагона с таким серийным номером</returns>
        public WagonData GetWagonData(int wagon_id)
        {
            if (wagon_id < 1) throw new ArgumentOutOfRangeException("id");
            WagonData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                int locoCount = GetTrainLocoCount(_DataHelper, wagon_id);
                string orderInv = "[numbers].[Sn]" + (locoCount == 0 ? string.Empty : " + " + locoCount.ToString() + " ");;
                if (InvertNL(wagon_id))
                {
                    orderInv = @"(SELECT COUNT(*) FROM [dbo].[v_wagons] WHERE [v_wagons].[TrainId] = 
								 (SELECT [v_wagons].[TrainId] FROM [dbo].[v_wagons] WHERE [v_wagons].[WagonId] = @wagid))
								  - [numbers].[Sn] + 1 " + (locoCount == 0 ? string.Empty : " - " + locoCount.ToString()+ " ");
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = WagonFields,
                    From = "[dbo].[v_wagons]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" +
                           "[v_wagons].[TrainId] = [numbers].[TrainId] AND [v_wagons].[WagonSn] = " + orderInv + ")",
                    Where = "[v_wagons].[WagonId] = @wagid"
                };
                //Параметр
                SqlParameter sn_param = new SqlParameter("wagid", wagon_id);
                //Запрос
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, sn_param);
                IEnumerable<WagonData> WagonDataQuery = WagonQuerySelect(dataTable);
                List<WagonData> WagonData = WagonDataQuery.ToList();
                if (WagonData.Count > 0) result = WagonData[0];
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить запись журнала вагонов идентификатору состава и номеру пересечения</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <param name="vagon_sn">Порядковый номер в составе</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Запись журнала вагонов, null - нет вагона с таким серийным номером</returns>
        public WagonData GetWagonData(int train_id, int vagon_sn)
        {
            if (vagon_sn < 1) throw new ArgumentOutOfRangeException("sn");
            WagonData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string orderInv = "[numbers].[Sn]";
                if (GetTrainData(train_id, false).InvertInvNum)
                {
                    orderInv = "(SELECT COUNT(*) FROM [dbo].[v_wagons] WHERE [v_wagons].[TrainId] = @trainid) - [numbers].[Sn] + 1";
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = WagonFields,
                    From = "[dbo].[v_wagons]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" +
                           "[v_wagons].[TrainId] = [numbers].[TrainId] AND [v_wagons].[WagonSn] = " + orderInv + ")",
                    Where = "[v_wagons].[TrainId] = @trainid AND [v_wagons].[WagonSn] = @sn"
                };
                //Параметры
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("trainid", train_id));
                _params.Add(new SqlParameter("sn", vagon_sn));
                //Запрос
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<WagonData> WagonDataQuery = WagonQuerySelect(dataTable);
                List<WagonData> WagonData = WagonDataQuery.ToList();
                if (WagonData.Count > 0) result = WagonData[0];
                _DataHelper.Commit();
            }
            return result;
        }
        
        /// <summary>Получить множество записей из журнала вагонов</summary>
        /// <param name="train_id">Идентификатор поезда/отцепа в СБВ УВГ (0 - любой идентификатор)</param>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="inv">Инвентарный номер вагона</param>
        /// <param name="inv_type">Тип распознанного номера</param>
        /// <param name="start">Номер первой записи</param>
        /// <param name="count">Максимальное количество записей</param>
        /// <param name="total">Количество записей соответствующих критериям</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Список записей журнала взвешивания вагонов</returns>
        public IList<WagonData> GetWagonList(int train_id, DateTime begin, DateTime end,
                                             string inv, int start, int count, out int total, SortOrder order)
        {
            if (train_id < 0) throw new ArgumentOutOfRangeException("train_id");

            List<WagonData> result = new List<WagonData>(count);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                int locoCount = GetTrainLocoCountByTrainID(_DataHelper, train_id);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = null;
                if (train_id > 0)
                {
                    where_str = AND(where_str, "([v_wagons].[TrainId] = @train_id)\n");
                    _params.Add(new SqlParameter("train_id", train_id));
                }
                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] >= @time_begin)\n");
                    _params.Add(new SqlParameter("time_begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeEnd] <= @time_end)\n");
                    _params.Add(new SqlParameter("time_end", end));
                }
                if (!string.IsNullOrEmpty(inv))
                {
                    where_str = AND(where_str, "([InvNum] = @inv)\n");
                    _params.Add(new SqlParameter("inv", inv));
                }
                string orderInv = "[numbers].[Sn]";
                var train = GetTrainData(train_id, false);
                //Если состав не найден ничего не делать
                if (train == null)
                {
                    total = 0;
                    return result;
                }
                if (train.InvertInvNum)
                {
                    orderInv = "(SELECT COUNT(*) FROM [dbo].[v_wagons] WHERE [v_wagons].[TrainId] = @train_id) - [numbers].[Sn] + 1";
                }

                string sql = (string)new SqlSelectBuilder
                {
                    Top = start + count - 1 < 0 ? 0 : start + count - 1,
                    Fields = WagonFields,
                    From = "[dbo].[v_wagons]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" +
                           "[v_wagons].[TrainId] = [numbers].[TrainId] AND [v_wagons].[WagonSn] = " + orderInv + ")",
                    Where = where_str,
                    OrderBy = order == SortOrder.Ascending ? "[v_wagons].[TimeSpanEnd]" : "[v_wagons].[TimeSpanEnd] DESC"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<WagonData> WagonDataQuery = WagonQuerySelect(dataTable);

                // Формируем список результирующих записей
                int index = 1;
                foreach (var wagon_data in WagonDataQuery)
                {
                    // Пропускаем первые start-1 записей если start 
                    if (start > 0 && (index++ < start)) continue;
                    result.Add(wagon_data);
                    if (count > 0 && (result.Count >= count)) break;
                }
                // Запрашиваем общее количество записей, удовлетворяющих заданному критерию
                sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "Count([WagonSn])" },
                    From = "[dbo].[v_wagons]",
                    Where = where_str
                };
                total = (int)_DataHelper.ExecuteScalar(sql, CopyParams(_params));
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Запрос вагонов выбранного поезда</summary>
        /// <param name="train_id">Идентификатор поезда</param>
        /// <returns>Списко вагонов</returns>
        public IList<WagonData> GetWagonList(int train_id)
        {
            int total;
            return GetWagonList(train_id, DateTime.MinValue, DateTime.MinValue,
                                "", 0, 0, out total, SortOrder.Ascending);
        }

        /// <summary>Получить список вагонов по инвентарному номеру </summary>
        /// <param name="invNumber"></param>
        /// <returns></returns>
        public IList<WagonData> GetWagonsByInvNumber(string invNumber)
        {
            List<WagonData> result = new List<WagonData>();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    List<DbParameter> _params = new List<DbParameter>();
                    //Просто медленный запрос  outer join работает очень медленно пришлось разбить на более мелкие и объединить результаты
                    /*Запрос к таблиц вагонов*/
                    string queryText = @"SELECT vw.*, '' AS Inv FROM v_wagons vw WHERE vw.InvNum = @InvNum OR vw.ocr_inv_num = @ocr_inv_num";
                    DataTable dataTable = _DataHelper.ExecuteCommand(queryText, new List<DbParameter> { new SqlParameter("@InvNum", invNumber), new SqlParameter("@ocr_inv_num", invNumber) });
                    IEnumerable<WagonData> WagonDataQuery = WagonQuerySelect(dataTable);
                    if (WagonDataQuery != null) result.AddRange(WagonDataQuery);
                    /*Запрос к таблице инвентарных номеров*/
                    string selectNNText = "SELECT n.* FROM numbers n WHERE n.Inv = @Inv";
                    DataTable dataTableNN = _DataHelper.ExecuteCommand(selectNNText, new List<DbParameter> { new SqlParameter("@Inv", invNumber) });
                    IEnumerable<NLTableData> natListInfo = NLTableDataDataSelect(dataTableNN);
                    Dictionary<int, DateTime> tableTrainIDTrainTime = new Dictionary<int, DateTime>();
                    if (natListInfo != null && natListInfo.Count()>0)
                    {
                        List<NLTableData> nlTable = new List<NLTableData>();
                        nlTable.AddRange(natListInfo);
                        Dictionary<int, List<int>> wagonDictionary = new Dictionary<int, List<int>>();

                        List<int> trainIDList = new List<int>();
                        for (int i = 0; i < nlTable.Count; i++)
                        {
                            var data = nlTable[i];
                            if (!wagonDictionary.ContainsKey(data.TrainID))
                            {
                                wagonDictionary.Add(data.TrainID, new List<int>());
                                trainIDList.Add(data.TrainID);
                            }
                            wagonDictionary[data.TrainID].Add(data.WagonSN);
                        }
                        //Запросить составы в базе данных
                        int totalTrainsCont = 0;
                        //Возможно 0 нужно заменить на -1 Проверить под отладчиком
                        IEnumerable<TrainData> trains = GetTrainsList(DateTime.MinValue, DateTime.MinValue, string.Empty, string.Empty, string.Empty,
                                                                      string.Empty, /*train_status*/-1, 0, 0, -1, 0, 0, out totalTrainsCont,
                                                                      true, 0, 0, -1, -1, 0, trainIDList);
                        foreach (var train in trains)
                        {
                            try { tableTrainIDTrainTime.Add(train.Id, train.BeginTime); } catch { }
                            var list = wagonDictionary[train.Id];
                            int locoCount = train.LocoCount;
                            for (int t = 0; t < list.Count; t++)
                                wagonDictionary[train.Id][t] = wagonDictionary[train.Id][t] + locoCount;
                        }
                        //Запросить данные вагонов
                        //Посмотреть запрос и внедрить
                        string wagonQuery = GetWagonByTrainIDAndWagonSNV5(wagonDictionary);
                        DataTable wagonData = _DataHelper.ExecuteCommand(wagonQuery);
                        IEnumerable<WagonData> wagons = WagonQuerySelect(wagonData);
                        if (wagons != null)
                        {
                            //Проверить на дубликаты, можно в базе при добавлении условий
                            //Добавляем с проверкой
                            foreach (var wagon in wagons)
                            {
                                if (result.Count == 0 || result.Count(x => x.WagId == wagon.WagId) == 0)
                                {
                                    try
                                    {
                                        if (tableTrainIDTrainTime.ContainsKey(wagon.TrainId))
                                            wagon.Time = tableTrainIDTrainTime[wagon.TrainId];
                                    }
                                    catch { }
                                    wagon.InvNumber = invNumber;
                                    result.Add(wagon);
                                }
                            }
                        }
                    }
                }
                finally { _DataHelper.Commit(); }
            }
            return result;
        }

        /// <summary>Получить актуальный вагон по инвентарному номеру  </summary>
        /// <param name="invNumber"></param>
        /// <returns></returns>
        public WagonData GetLastWagonByInvNumber(string invNumber)
        {
            WagonData result = new WagonData();
            IList<WagonData> collection = GetWagonsByInvNumber(invNumber);
            if (collection.Count > 0)
            {
                result = collection[0];
                for (int i = 1; i < collection.Count; i++)
                {
                    if (result.Time < collection[i].Time)
                        result = collection[i];
                }
            }
            return result;
        }

        string GetTrainListByIdV5(List<int> trainIDList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"WITH Ordered AS(SELECT  TOP(1000000) ROW_NUMBER() OVER(ORDER BY[TimeBegin] DESC) AS RowNumber,
                                 [trains].[TrainId],
                                 [TrainNumber],
                                 [TrainIndex],
                                 [Accepted],
                                 [Way],
                                 [WayId],
                                 [trains].[OpId],
                                 [operators].[OpName],
                                 [InvertInvNum],
                                 [TimeBegin],
                                 [TimeEnd],
                                 [Direction],
                                 [trains].[DirId],
                                 [DirPath],
                                 [trains].[Status],
                                 [Comment],
                                 [RecordOffset],
                                 COALESCE(SumTrainSpeed / nullif(WagonsCount, 0), 0) AS SpeedEnd,
                                 COALESCE((MinWagon - FirstWagon), (SELECT COUNT(*) FROM Wagons WHERE[trains].[TrainId] = [wagons].[TrainId] AND Loco = 1)) AS LocoCount,
                                 WagonsCount,
                                 (SELECT COUNT(*) FROM wagons WHERE[trains].[TrainId] = [wagons].[TrainId] AND Ngb <> 0 AND WagonSn >= MinWagon) AS WagonsNgbCount
                                 FROM[trains]
                                 LEFT OUTER JOIN operators ON trains.OpId = operators.OpId
                                 LEFT OUTER JOIN directories ON trains.DirId = directories.DirId
                                 LEFT JOIN 
                                    (SELECT[dbo].[trains].[TrainId],
                                    COUNT(*) AS WagonsCount,
                                    (SELECT MIN(wagons.WagonSn) FROM wagons WHERE [trains].[TrainId] = [wagons].[TrainId] AND Loco <> 1) AS MinWagon,
                                    (SELECT MIN(wagons.WagonSn) FROM wagons WHERE[trains].[TrainId] = [wagons].[TrainId]) AS FirstWagon,
                                    SUM([wagons].[SpeedEnd]) AS SumTrainSpeed
                                    FROM[wagons], [trains] WHERE[wagons].[TrainId] = [trains].[TrainId]
                                    GROUP BY[trains].[TrainId]) wag ON wag.TrainId = [trains].TrainId
                                WHERE [trains].[TrainId] IN( ");
            for (int i = 0; i < trainIDList.Count; i++)
            {
                sb.Append(trainIDList[i]);
                sb.Append(i < (trainIDList.Count - 1) ? ", " : " ");
            }
            sb.Append(@") ORDER BY[TimeBegin] DESC)  SELECT* FROM Ordered");
            return sb.ToString();
        }

        string GetWagonByTrainIDAndWagonSNV5(Dictionary<int, List<int>> trainWagoDict)
        {
            SqlSelectBuilder builder = new SqlSelectBuilder();
            builder.Fields = WagonFields;
            //builder.Fields = new string[] { "[v_wagons].*", "[numbers].[Inv]" };
            builder.From = @"[dbo].[v_wagons] LEFT OUTER JOIN[dbo].[numbers] ON([v_wagons].[TrainId] = [numbers].[TrainId] AND[v_wagons].[WagonSn] = [numbers].[Sn])";
            StringBuilder sb = new StringBuilder();
            sb.Append( "(");
            int counter = 0;
            foreach (var pair in trainWagoDict)
            {
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    if (counter > 0) sb.Append(" OR ");
                    sb.AppendLine(string.Format("[v_wagons].[TrainId] = {0} and [v_wagons].WagonSn = {1}", pair.Key, pair.Value[i]));
                    counter++;
                }
            }
            sb.Append(")");
            builder.Where = sb.ToString();
            builder.OrderBy = " [v_wagons].[TimeSpanEnd] Desc";
            return builder.ToString();
        }
        
        /// <summary>Изменить запись в таблице вагонов (wagons)</summary>
        /// <param name="data">Данные взвешивания вагона</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта или не задан контекст транзакции</exception>
        /// <exception cref="System.ObjectDisposedException">Транзакция завершена и не может быть использована</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public bool ModifyWagon(WagonData data)
        {
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                if (context == null) throw new ArgumentException("context");
                CheckWagon(data);
                List<DbParameter> _params = new List<DbParameter>(0);
                string sql = (string)new SqlUpdateBuilder
                {
                    Fields = new string[] { "[WagonSn]", "[SnSost]", "[Loco]", "[InvNum]", 
                                            "[TimeSpanBegin]", "[TimeSpanEnd]", 
                                            "[TimeSpanBeginBS]", "[TimeSpanEndBS]", 
                                            "[SpeedBegin]", "[SpeedEnd]", 
                                            "[DirectionBegin]", "[DirectionEnd]",
                                            "[Ngb]", "[Comment]", "[Mark]", "[TimeChanged]" },
                    Values = new string[] { "@Sn", "@SnSost", "@Loco", "@InvNum", 
                                            "@TimeSpanBegin", "@TimeSpanEnd", 
                                            "@TimeSpanBeginBS", "@TimeSpanEndBS", 
                                            "@SpeedBegin", "@SpeedEnd",
                                            "@DirectionBegin", "@DirectionEnd",
                                            "@Ngb", "@Comment", "@Mark", "@TimeChanged" },
                    Table = "[dbo].[wagons]",
                    Where = "[WagonId] = @WagId"
                };
                DateTime changed = DateTime.Now;
                _params.Add(new SqlParameter("WagId", data.WagId));                     //Идентификатор вагона в базе
                _params.Add(new SqlParameter("Sn", data.Sn));                           //Номер пересечения
                _params.Add(new SqlParameter("SnSost", data.SnSost));                   //Номер в составе
                _params.Add(new SqlParameter("Loco", data.Loco));                       //Тип вагона
                _params.Add(new SqlParameter("InvNum", data.InvNumber));                //Инвентарный номер
                _params.Add(new SqlParameter("TimeSpanBegin", data.TimeSpanBegin));     //Метка времени начало вагона
                _params.Add(new SqlParameter("TimeSpanEnd", data.TimeSpanEnd));         //Метка времени конца вагона
                _params.Add(new SqlParameter("TimeSpanBeginBS", data.TimeSpanBeginBS)); //Метка времени начало вагона
                _params.Add(new SqlParameter("TimeSpanEndBS", data.TimeSpanEndBS));     //Метка времени конца вагона
                _params.Add(new SqlParameter("SpeedBegin", data.SpeedBegin));           //Скорость в начале
                _params.Add(new SqlParameter("SpeedEnd", data.SpeedEnd));               //Скорость в конце
                _params.Add(new SqlParameter("DirectionBegin", data.DirectionBegin));   //Направление в начале
                _params.Add(new SqlParameter("DirectionEnd", data.DirectionEnd));       //Направление в конце
                _params.Add(new SqlParameter("Ngb", data.Ngb));                         //Маска негабарито
                _params.Add(new SqlParameter("Mark", data.Mark));                       //Признак маркирования вагона
                _params.Add(new SqlParameter("Comment", data.Comment));                 //Комментарий
                _params.Add(new SqlParameter("TimeChanged", changed));                  //Время изменения
                int result = context.ExecuteNoneQuery(sql, _params);
                if (result != 1) return false;
                else return true;
            }
        }

        /// <summary>Маркировать вагон</summary>
        /// <param name="data">Данные взвешивания вагона</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта или не задан контекст транзакции</exception>
        /// <exception cref="System.ObjectDisposedException">Транзакция завершена и не может быть использована</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public bool MarkWagon(WagonData data)
        {
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                if (context == null) throw new ArgumentException("context");
                CheckWagon(data);
                List<DbParameter> _params = new List<DbParameter>(13);
                string sql = (string)new SqlUpdateBuilder
                {
                    Fields = new string[] { "[Mark]" },
                    Values = new string[] { "@Mark" },
                    Table = "[dbo].[wagons]",
                    Where = "[WagonId] = @WagId"
                };
                DateTime changed = DateTime.Now;
                _params.Add(new SqlParameter("WagId", data.WagId));//Идентификатор вагона в базе
                _params.Add(new SqlParameter("Mark", data.Mark));
                int result = context.ExecuteNoneQuery(sql, _params);
                if (result != 1) return false;
                else return true;
            }
        }

        /// <summary>Удаление вагона</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <param name="wagon_sn">Порядковый номер вагона</param>
        /// <returns></returns>
        public bool DeleteWagon(int train_id, int wagon_id)
        {
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                if (context == null) throw new ArgumentException("context");
                List<DbParameter> _params = new List<DbParameter>(2);
                string sql = (string)new SqlDeleteBuilder
                {
                    Table = "[dbo].[wagons]",
                    Where = "[TrainId] = @TrainId AND [WagonId] = @WagonId"
                };
                DateTime changed = DateTime.Now;
                _params.Add(new SqlParameter("TrainId", train_id));//Идентификатор вагона в базе
                _params.Add(new SqlParameter("WagonId", wagon_id));
                int result = context.ExecuteNoneQuery(sql, _params);
                if (result > 0) return true;
                else return false;
            }
        }

        /// <summary>Пересчет порядковых номеров вагонов после указанного вагона</summary>
        /// <param name="wagon">Данные вагона</param>
        /// <param name="insert">true - Вставка вагона (прибавить последующие порядковые номера)
        /// false - удаление вагона (убавить последующие порядковые номера)</param>
        /// <returns>Результат операции</returns>
        public void UpdateWagonsSn(WagonData wagon, bool insert)
        {
            //Вагоны состава
            IList<WagonData> wagons = GetWagonList(wagon.TrainId);
            //Пересчет порядковых номеров вагонов в базе данных
            for (int i = 0; i < wagons.Count; i++)
            {
                //Для вагонов с порядковым номером равным или большим текущего
                if (wagons[i].Sn >= wagon.Sn && wagons[i].WagId != wagon.WagId)
                {
                    if (insert)
                    {
                        //Увеличить порядковый номер
                        wagons[i].Sn++;
                        wagons[i].SnSost++;
                    }
                    else
                    {
                        //Уменьшить порядковый номер
                        wagons[i].Sn--;
                        wagons[i].SnSost--;
                    }
                    ModifyWagon(wagons[i]);
                }
            }
        }
        
        #endregion

        #region Nature lists

        /// <summary>Занести в базу данных данные инвентарных номеров вагонов для заданного поезда, изменить номер и индекс поезда</summary>
        /// <param name="train_id">Идентификатор поезда в системе</param>
        /// <param name="data">Данные поезда с инвентарными номерами вагонов</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Индекс поезда вне допустимого диапазона значений</exception>
        /// <exception cref="System.ArgumentNullException">Не задан объект данных</exception>
        /// <exception cref="System.ArgumentException">Записи о поезде с заданным идентификатором не обнаружено в базе данных</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public void BindNatList(int train_id, NatListData data)
        {
            if (train_id < 1) throw new ArgumentOutOfRangeException("train_id");
            if (data == null) throw new ArgumentNullException("data");
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                // Получаем данные о поезде
                TrainData train = GetTrainData(train_id, false);
                // Правим номер и индекс поезда
                train.TrainIndex = data.TrainIndex;
                train.TrainNum = data.TrainNum;
                ModifyTrain(train);
                // Удалить старые номера натурного листа
                string table_name = "[dbo].[numbers]";
                string query = (string)new SqlDeleteBuilder() { Table = table_name, Where = "[TrainId] = @TrainId" };
                _DataHelper.ExecuteNoneQuery(query, new SqlParameter("TrainId", train_id));
                // Добавить новые номера натурного листа
                if (data.WagInvNums != null)
                {
                    for (int i = 1; i <= data.WagInvNums.Count; ++i)
                    {
                        query = (string)new SqlInsertBuilder()
                        {
                            Table = table_name,
                            Fields = new string[] { "[TrainId]", "[Sn]", "[Inv]" },
                            Values = new string[] { "@TrainId", "@Sn", "@Inv" }
                        };
                        string next_inv = data.WagInvNums[i - 1];
                        _DataHelper.ExecuteNoneQuery(query,
                            new SqlParameter("TrainId", train_id),
                            new SqlParameter("Sn", i),
                            new SqlParameter("Inv", string.IsNullOrEmpty(next_inv) ? (object)DBNull.Value : next_inv)
                        );
                    }
                }
                _DataHelper.Commit();
            }
        }

        /// <summary>Занести в базу данных данные моделей вагонов для заданного поезда</summary>
        /// <param name="train_id">Идентификатор поезда в системе</param>
        /// <param name="data">Данные поезда с моделями вагонов</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Идентификатор поезда вне допустимого диапазона значений</exception>
        /// <exception cref="System.ArgumentNullException">Не задан объект данных</exception>
        /// <exception cref="System.ArgumentException">Записи о поезде с заданным идентификатором не обнаружено в базе данных</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public void BindNatModels(int train_id, NatListModels data)
        {
            if (train_id < 1) throw new ArgumentOutOfRangeException("train_id");
            if (data == null) throw new ArgumentNullException("data");
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                /*
                // Получаем данные о поезде
                TrainData train = GetTrainData(train_id, false);
                // Правим номер и индекс поезда
                train.TrainIndex = data.TrainIndex;
                train.TrainNum = data.TrainNum;
                ModifyTrain(train);
                // Удалить старые номера натурного листа
                string table_name = "[dbo].[numbers]";
                string query = (string)new SqlDeleteBuilder() { Table = table_name, Where = "[TrainId] = @TrainId" };
                _DataHelper.ExecuteNoneQuery(query, new SqlParameter("TrainId", train_id));
                // Добавить новые номера натурного листа
                if (data.WagInvNums != null)
                {
                    for (int i = 1; i <= data.WagInvNums.Count; ++i)
                    {
                        query = (string)new SqlInsertBuilder()
                        {
                            Table = table_name,
                            Fields = new string[] { "[TrainId]", "[Sn]", "[Inv]" },
                            Values = new string[] { "@TrainId", "@Sn", "@Inv" }
                        };
                        string next_inv = data.WagInvNums[i - 1];
                        _DataHelper.ExecuteNoneQuery(query, new SqlParameter("TrainId", train_id),
                                                            new SqlParameter("Sn", i),
                                                            new SqlParameter("Inv", string.IsNullOrEmpty(next_inv) ? (object)DBNull.Value : next_inv));
                    }
                }
                */
                // Добавить модели вагонов и коды габаритов подвижного состава
                if (data != null)
                {
                    // Удалить старые данные моделей
                    string table_name = "[dbo].[wagons_models]";
                    string query = (string)new SqlDeleteBuilder() { Table = table_name, Where = "[train_id] = @TrainId" };
                    _DataHelper.ExecuteNoneQuery(query, new SqlParameter("TrainId", train_id));
                    //Добавить вагоны
                    for (int sn = 1; sn <= data.WagInvNums.Count; sn++)
                    {
                        query = (string)new SqlInsertBuilder()
                        {
                            Table = table_name,
                            Fields = new string[] { "[train_id]", "[wagon_sn]", "[model]", "[ngb_code]" },
                            Values = new string[] { "@TrainId", "@Sn", "@Model", "@Code" }
                        };
                        string model = data.WagModels[sn - 1];
                        int code = data.WagGabarit[sn - 1];
                        _DataHelper.ExecuteNoneQuery(query, new SqlParameter("TrainId", train_id),
                                                            new SqlParameter("Sn", sn),
                                                            new SqlParameter("Model", string.IsNullOrEmpty(model) ? (object)DBNull.Value : model),
                                                            new SqlParameter("Code", code));
                    }
                }
                _DataHelper.Commit();
            }
        }

        /// <summary>Изменяет порядок инвентарных номеров вагонов заданного поезда на обратный</summary>
        /// <param name="train_id">Идентификатор поезда в системе АСКОПВ</param>
        /// <returns>Данные поезда с инвентарными номерами вагонов</returns>
        public NatListData ReverseWagNumbers(int train_id)
        {
            if (train_id < 1) throw new ArgumentOutOfRangeException();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                _DataHelper.ExecuteStoredProcedure("[dbo].[reverse_numbers]", new SqlParameter("TrainId", train_id));
                _DataHelper.Commit();
            }
            return GetNatList(train_id);
        }

        /// <summary>Прочитать из базы данные инвентарных номеров вагонов для заданного поезда</summary>
        /// <param name="train_id">Идентификатор поезда в системе АСКОПВ</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Индекс поезда вне допустимого диапазона значений</exception>
        /// <exception cref="System.ArgumentException">Записи о поезде с заданным идентификатором не обнаружено в базе данных</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        /// <returns>Данные поезда с инвентарными номерами вагонов</returns>
        protected NatListData GetNatList(int train_id)
        {
            if (train_id < 1) throw new ArgumentOutOfRangeException();
            NatListData result = null;
            TrainData train = GetTrainData(train_id, false);
            result = new NatListData(train.TrainNum, train.TrainIndex, train.BeginTime);
            result.WagInvNums = GetWagonList(train_id).Select(data => data.InvNumber).ToList<string>();
            return result;
        }
               
        #endregion

        #region Events

        public int GetEventListCount()
        {
            return 0;
        }

        /// <summary>Получить записи из журнала событий системы</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="msg_list">Список категорий сообщений разделенный запятыми (пустая строка - все категории)</param>
        /// <param name="start">Номер первой записи</param>
        /// <param name="count">Маскимальное количество записей</param>
        /// <param name="total">Количество записей соответствующих критериям</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Список записей из журнала событий</returns>
        public IList<EventData> GetEventList(DateTime begin, DateTime end,
                                             string msg_list, int start, int count, out int total, string operatorName)
        {
            if (start < 1) throw new ArgumentOutOfRangeException("start");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            List<EventData> result = new List<EventData>();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = null;
                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([EvDateTime] >= @begin_time)\n");
                    _params.Add(new SqlParameter("begin_time", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([EvDateTime] <= @end_time)\n");
                    _params.Add(new SqlParameter("end_time", end));
                }
                if (!string.IsNullOrEmpty(msg_list))
                {
                    where_str = AND(where_str, "([MsgId] IN (" + msg_list + "))\n");
                }
                if (!string.IsNullOrEmpty(operatorName))
                {
                    where_str = AND(where_str, "OpId IN(select o.OpId from operators o where OpName = '"+ operatorName + "')\n");
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Top = start + count - 1,
                    Fields = new string[] { "[Id]", "[EvDateTime]", "[MsgText]", "[EvSource]", "[EvData]", "[OpId]", "[OpName]", "[MsgId]", "[HasVideo]" },
                    From = "[dbo].[v_eventlog]",
                    Where = where_str,
                    OrderBy = string.IsNullOrEmpty(operatorName) ? "[EvDateTime] DESC, [Id]" : "[EvDateTime], [Id]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<EventData> EventDataQuery =
                    from v_eventlog in dataTable.AsEnumerable()
                    let source = v_eventlog.IsNull("EvSource") ? string.Empty : (string)v_eventlog["EvSource"]
                    let data = v_eventlog.IsNull("EvData") ? string.Empty : (string)v_eventlog["EvData"]
                    let text = v_eventlog.IsNull("MsgText") ? string.Empty : (string)v_eventlog["MsgText"]
                    let opId = v_eventlog.IsNull("OpId") ? 0 : (int)v_eventlog["OpId"]
                    let name = v_eventlog.IsNull("OpName") ? string.Empty : (string)v_eventlog["OpName"]
                    let msgid = v_eventlog.IsNull("MsgId") ? 0 : (int)v_eventlog["MsgId"]
                    let has = v_eventlog.IsNull("HasVideo") ? 0 : (int)v_eventlog["HasVideo"]
                    
                    select new EventData
                    {
                        Sn = (int)v_eventlog["Id"],
                        EventTime = (DateTime)v_eventlog["EvDateTime"],
                        Text = text,
                        Source = source,
                        Data = data,
                        MsgId = msgid,
                        OpId = opId,
                        OpName = name,
                        HasVideo = (has == 1)
                    };
                // Формируем список результирующих записей
                int index = 1;
                foreach (var event_data in EventDataQuery)
                {
                    // Пропускаем первые start-1 записей
                    if (index++ < start) continue;
                    result.Add(event_data);
                    if (count > 0 && result.Count >= count) break;
                }
                // Запрашиваем общее количество записей, удовлетворяющих заданному критерию
                sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "Count([Id])" },
                    From = "[dbo].[v_eventlog]",
                    Where = where_str
                };
                total = (int)_DataHelper.ExecuteScalar(sql, CopyParams(_params));
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить записи из журнала событий системы</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="msg_id_list">Список идентификаторов сообщений разделенный запятыми (пустая строка - все сообщения)</param>
        /// <returns>Список записей из журнала событий</returns>
        public IList<EventData> GetEventList(DateTime begin, DateTime end, string msg_list, string operatorName=null)
        {
            int total;
            return GetEventList(begin, end, msg_list, 1, 0, out total, operatorName);
        }

        /// <summary>Добавить запись в журнал событий</summary>
        /// <param name="data">Данные события</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        /// <remarks>В случае успешного выполнения ссылка data будет указывать на новый объект,
        /// который будет содержать серийный номер добавленного события в базе данных</remarks>
        public void AddEvent(ref EventData data)
        {
            CheckEvent(data);
            EventData result = data;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();

                SqlParameter param_date = new SqlParameter("EvDateTime", SqlDbType.DateTime2);
                param_date.Value = data.EventTime;
                _params.Add(param_date);
                _params.Add(new SqlParameter("MsgId", data.MsgId));
                _params.Add(new SqlParameter("EvData", string.IsNullOrEmpty(data.Data) ? null : data.Data));
                _params.Add(new SqlParameter("EvSource", string.IsNullOrEmpty(data.Source) ? null : data.Source));
                if (data.OpId <= 0) _params.Add(new SqlParameter("OpId", null));
                else _params.Add(new SqlParameter("OpId", data.OpId));
                if (data.HasVideo) _params.Add(new SqlParameter("HasVideo", 1));
                _params.Add(new SqlParameter("WayId", data.WayId));

                SqlParameter paramSn = new SqlParameter
                {
                    ParameterName = "Sn",
                    Value = data.Sn,
                    Direction = ParameterDirection.Output
                };
                _params.Add(paramSn);
                _DataHelper.ExecuteStoredProcedure("[dbo].[add_eventlog_item]", _params);
                result = new EventData
                {
                    EventTime = data.EventTime,
                    MsgId = data.MsgId,
                    Source = data.Source,
                    Data = data.Data,
                    OpId = data.OpId,
                    HasVideo = data.HasVideo,
                    Sn = (int)paramSn.Value,
                    Text = data.Text,
                };
                _DataHelper.Commit();
            }
            data = result;
        }

        /// <summary>Удалить записи из журнала событий</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        public void DeleteEvents(DateTime begin, DateTime end)
        {
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = null;
                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([EvDateTime] >= @begin)\n");
                    _params.Add(new SqlParameter("begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([EvDateTime] <= @end)\n");
                    _params.Add(new SqlParameter("end", end));
                }
                string sql = (string)new SqlDeleteBuilder
                {
                    Table = "[dbo].[eventlog]",
                    Where = where_str
                };
                _DataHelper.ExecuteNoneQuery(sql, _params);
                _DataHelper.Commit();
            }
        }

        /// <summary>Удалить записи из журнала событий по идентификатору</summary>
        public int DeleteEvent(int id)
        {
            int res = 0;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                //_DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                res = _DataHelper.ExecuteNoneQuery("DELETE FROM eventlog WHERE id = " + id.ToString(), null);
                //_DataHelper.Commit();

                //Удалять по 1000
                res = _DataHelper.ExecuteNoneQuery("DELETE FROM eventlog WHERE id = " + id.ToString(), null);
            }
            return res;
        }
        
        /// <summary>Удалить записи из журнала событий по идентификатору</summary>
        public int DeleteEvents(int start, int stop)
        {
            int res = 0;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                //Удалять указанный диапазон
                res = _DataHelper.ExecuteNoneQuery("DELETE FROM eventlog WHERE id >= " + start.ToString() + " AND id <= " + stop.ToString(), null);
            }
            return res;
        }

        /// <summary>Возвращает список сообщений о событиях</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список сообщений</returns>
        public IList<MessageData> GetMessageList()
        {
            List<MessageData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[MsgId]", "[MsgText]" },
                    From = "[dbo].[messages]",
                    OrderBy = "[MsgId]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql);
                IEnumerable<MessageData> MsgDataQuery =
                    from msg in dataTable.AsEnumerable()
                    select new MessageData((int)msg["MsgId"], (string)msg["MsgText"]);
                result = MsgDataQuery.ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        public EventData GetLastProgrammStartEvent()
        {
            EventData result = new EventData { Sn=-1};
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();

                string sql = @"SELECT 
                               ev.Id, ev.WayId, ev.EvDateTime, ev.MsgId, ev.EvSource, ev.EvData, ev.OpId, ev.HasVideo, mmm.MsgText, op.OpName
                               FROM eventlog ev
                               LEFT OUTER JOIN operators op on ev.OpId = op.OpId 
                               JOIN [messages] mmm on mmm.MsgId = ev.MsgId
                               WHERE ev.Id = (SELECT MAX(ee.Id) FROM eventlog ee WHERE ee.MsgId = 1)";
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<EventData> EventDataQuery =
                    from v_eventlog in dataTable.AsEnumerable()
                    let source = v_eventlog.IsNull("EvSource") ? string.Empty : (string)v_eventlog["EvSource"]
                    let data = v_eventlog.IsNull("EvData") ? string.Empty : (string)v_eventlog["EvData"]
                    let text = v_eventlog.IsNull("MsgText") ? string.Empty : (string)v_eventlog["MsgText"]
                    let opId = v_eventlog.IsNull("OpId") ? 0 : (int)v_eventlog["OpId"]
                    let name = v_eventlog.IsNull("OpName") ? string.Empty : (string)v_eventlog["OpName"]
                    let msgid = v_eventlog.IsNull("MsgId") ? 0 : (int)v_eventlog["MsgId"]
                    let has = v_eventlog.IsNull("HasVideo") ? 0 : (int)v_eventlog["HasVideo"]
                    
                    select new EventData
                    {
                        Sn = (int)v_eventlog["Id"],
                        EventTime = (DateTime)v_eventlog["EvDateTime"],
                        Text = text,
                        Source = source,
                        Data = data,
                        MsgId = msgid,
                        OpId = opId,
                        OpName = name,
                        HasVideo = (has == 1)
                    };
                _DataHelper.Commit();
                result = EventDataQuery.FirstOrDefault();
            }
            if (result == null)
            {
                result = new EventData{ Sn = -1 };
            }
            return result;
        }

        public IList<EventData> GetLastMessage(EventData programmStartEvent)
        {
            List<EventData> result = new List<EventData>();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                SqlParameter parametr = new SqlParameter("@ID",SqlDbType.Int);
                parametr.Value = programmStartEvent.Sn;
               _params.Add(parametr);

               string sql = @"SELECT 
                              ev.Id, ev.WayId, ev.EvDateTime, ev.MsgId, ev.EvSource, ev.EvData, ev.OpId, ev.HasVideo,mmm.MsgText,op.OpName
                              FROM 
                              eventlog ev
                              LEFT OUTER JOIN operators op on ev.OpId=op.OpId 
                              JOIN [messages] mmm on mmm.MsgId = ev.MsgId
                              WHERE ev.Id >@ID"; 
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<EventData> EventDataQuery =
                    from v_eventlog in dataTable.AsEnumerable()
                    let source = v_eventlog.IsNull("EvSource") ? string.Empty : (string)v_eventlog["EvSource"]
                    let data = v_eventlog.IsNull("EvData") ? string.Empty : (string)v_eventlog["EvData"]
                    let text = v_eventlog.IsNull("MsgText") ? string.Empty : (string)v_eventlog["MsgText"]
                    let opId = v_eventlog.IsNull("OpId") ? 0 : (int)v_eventlog["OpId"]
                    let name = v_eventlog.IsNull("OpName") ? string.Empty : (string)v_eventlog["OpName"]
                    let msgid = v_eventlog.IsNull("MsgId") ? 0 : (int)v_eventlog["MsgId"]
                    let has = v_eventlog.IsNull("HasVideo") ? 0 : (int)v_eventlog["HasVideo"]

                    select new EventData
                    {
                        Sn = (int)v_eventlog["Id"],
                        EventTime = (DateTime)v_eventlog["EvDateTime"],
                        Text = text,
                        Source = source,
                        Data = data,
                        MsgId = msgid,
                        OpId = opId,
                        OpName = name,
                        HasVideo = (has == 1)
                    };
                result.AddRange(EventDataQuery);
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получение списка минимального и максимального идентификаторов событий для удаления</summary>
        /// <param name="Start">Начальная дата</param>
        /// <param name="Stop">Конечная дата</param>
        public IList<int> GetEventsForDelete(DateTime Start, DateTime Stop)
        {
            List<int> arr = new List<int>();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("begin", Start));
                _params.Add(new SqlParameter("end", Stop));
                string sql = @"SELECT MIN(id) AS min_id, MAX(id) AS max_id FROM eventlog WHERE EvDateTime > @begin AND EvDateTime < @end";
                DataTable t = _DataHelper.ExecuteCommand(sql, _params);
                if (t != null && t.Rows.Count == 1 &&
                    t.Rows[0]["min_id"] != DBNull.Value &&
                    t.Rows[0]["max_id"] != DBNull.Value)
                {
                    arr.Add((int)t.Rows[0]["min_id"]);
                    arr.Add((int)t.Rows[0]["max_id"]);
                }
                _DataHelper.Commit();
            }
            return arr;
        }

        #endregion

        #region Operators

        /// <summary>Получить список операторов</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>список операторов системы</returns>
        public IList<OperatorData> GetOperatorsList()
        {
            List<OperatorData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[OpId]", "[OpLogin]", "[OpPassword]", 
                                            "[OpName]", "[Status]", "[Permissions]",
                                            "[UsbKey]", "[UsbType]" },
                    From = "[dbo].[operators]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql);
                IEnumerable<OperatorData> OpDataQuery =
                    from opr in dataTable.AsEnumerable()
                    select new OperatorData((int)opr["OpId"], (string)opr["OpLogin"], (string)opr["OpPassword"],
                                            (string)opr["OpName"], (bool)opr["Status"], (int)opr["Permissions"], 
                                            (int)opr["UsbKey"], GetUsbType(opr));
                result = OpDataQuery.ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить тип ключа из объекта строки таблицы</summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        int GetUsbType(DataRow opr)
        {
            int usb_type = 0;
            if (opr["UsbType"] != DBNull.Value) usb_type = (int)opr["UsbType"];
            return usb_type;
        }

        /// <summary>Получить данные оператора</summary>
        /// <param name="id">Идентификатор оператора</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Данные оператора системы</returns>
        public OperatorData GetOperatorData(int id)
        {
            OperatorData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[OpId]", "[OpLogin]", "[OpPassword]", 
                                            "[OpName]", "[Status]", "[Permissions]",
                                            "[UsbKey]", "[UsbType]" },
                    From = "[dbo].[operators]",
                    Where = "[OpId] = " + id.ToString()
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql);
                IEnumerable<OperatorData> OpDataQuery =
                    from opr in dataTable.AsEnumerable()
                    select new OperatorData((int)opr["OpId"], (string)opr["OpLogin"], (string)opr["OpPassword"],
                                            (string)opr["OpName"], (bool)opr["Status"], (int)opr["Permissions"], 
                                            (int)opr["UsbKey"], GetUsbType(opr));
                if (OpDataQuery.Count() > 0) result = OpDataQuery.ElementAt(0);
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Добавить нового оператора в базу данных</summary>
        /// <param name="data">Данные оператора</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта данных</exception>
        /// <remarks>В случае успешного выполнения ссылка data будет указывать на новый объект,
        /// который будет содержать идентификатор добавленного оператора в базе данных</remarks>
        public void AddOperator(ref OperatorData data)
        {
            CheckOperator(data);
            OperatorData result = data;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                List<DbParameter> _params = new List<DbParameter>();
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                _params.Add(new SqlParameter("OpLogin", data.Login));
                _params.Add(new SqlParameter("OpPassword", data.Password));
                _params.Add(new SqlParameter("OpName", data.OpName));
                _params.Add(new SqlParameter("Permissions", data.Permissions));
                _params.Add(new SqlParameter("UsbKey", data.UsbKey));
                _params.Add(new SqlParameter("UsbType", data.UsbType));
                SqlParameter paramOpId = new SqlParameter
                {
                    ParameterName = "OpId",
                    Value = data.Id,
                    Direction = ParameterDirection.Output
                };
                _params.Add(paramOpId);
                _DataHelper.ExecuteStoredProcedure("[dbo].[add_operator]", _params);
                result = new OperatorData((int)paramOpId.Value, data.Login, data.Password,
                                          data.OpName, data.Status, data.Permissions, data.UsbKey, data.UsbType);
                _DataHelper.Commit();
            }
            data = result;
        }

        /// <summary>Удалить оператора из базы данных</summary>
        /// <param name="id">Идентификатор оператора</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение входного параметра вне допустимого диапазона значений</exception>
        public void DeleteOperator(int id)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException("id");
            }
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlDeleteBuilder
                {
                    Table = "[dbo].[operators]",
                    Where = "[OpId] = @id"
                };
                int result = _DataHelper.ExecuteNoneQuery(sql, new SqlParameter("id", id));
                if (result == 0)
                {
                    throw new Exception("Record with given 'Id' not found!");
                }
                _DataHelper.Commit();
            }
        }

        /// <summary>Изменить данные об операторе в базе данных</summary>
        /// <param name="data">Измененные данные оператора</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта данных</exception>
        public void ModifyOperator(OperatorData data)
        {
            CheckOperator(data);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                List<DbParameter> _params = new List<DbParameter>(3);
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlUpdateBuilder
                {
                    Table = "[dbo].[operators]",
                    Fields = new string[] { "[OpLogin]", "[OpPassword]", "[OpName]", "[Status]", "[Permissions]", "[UsbKey]", "[UsbType]" },
                    Values = new string[] { "@login", "@password", "@name", "@status", "@level", "@usbkey", "@usbtype" },
                    Where = "[OpId] = @id"
                };
                _params.Add(new SqlParameter("login", data.Login));
                _params.Add(new SqlParameter("password", data.Password));
                _params.Add(new SqlParameter("name", data.OpName));
                _params.Add(new SqlParameter("status", data.Status));
                _params.Add(new SqlParameter("level", data.Permissions));
                _params.Add(new SqlParameter("usbkey", data.UsbKey));
                _params.Add(new SqlParameter("usbtype", data.UsbType));
                _params.Add(new SqlParameter("id", data.Id));
                int result = _DataHelper.ExecuteNoneQuery(sql, _params);
                if (result == 0)
                {
                    throw new Exception("Record with given 'Id' not found!");
                }
                _DataHelper.Commit();
            }
        }

        #endregion

        #region Directories

        /// <summary>Получить список каталогов видеоархива</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список каталогов видеоархива</returns>
        public IList<DirectoryData> GetDirectoriesList()
        {
            List<DirectoryData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[DirId]", "[DirPath]", "[DirStat]" },
                    From = "[dbo].[directories]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql);
                IEnumerable<DirectoryData> DirDataQuery =
                    from directory in dataTable.AsEnumerable()
                    select new DirectoryData((int)directory["DirId"], (string)directory["DirPath"],
                                             GetDirStat((int)directory["DirStat"]),
                                             (int)directory["DirStat"] > 0);
                result = DirDataQuery.ToList();
                foreach (DirectoryData dir in result)
                {
                    DriveInfo drive = null;
                    try
                    {
                        drive = DriveInfo.GetDrives().First(d => d.Name == Path.GetPathRoot(dir.Path));
                        dir.TotalSize = drive.TotalSize;
                        dir.TotalFreeSpace = drive.TotalFreeSpace;
                    }
                    catch { };
                }
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Добавить каталог видеоархива в базу данных</summary>
        /// <param name="data">Данные каталога видеоархива</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойства заданного объекта</exception>
        /// <remarks>В случае успешного выполнения ссылка data будет указывать на новый объект,
        /// который будет содержать идентификатор добавленного каталога в базе данных</remarks>
        public bool AddDirectory(ref DirectoryData data)
        {
            CheckDirectory(data);
            DirectoryData result = data;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                List<DbParameter> _params = new List<DbParameter>();
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                _params.Add(new SqlParameter("DirPath", data.Path));
                _params.Add(new SqlParameter("DirStat", (int)data.Status));
                SqlParameter paramDirId = new SqlParameter
                {
                    ParameterName = "DirId",
                    Value = data.Id,
                    Direction = ParameterDirection.Output
                };
                _params.Add(paramDirId);
                _DataHelper.ExecuteNoneQueryStoredProcedure("[dbo].[add_directory]", _params);
                if ((paramDirId.Value as int? ?? default(int)) == 0) return false;
                //Возвращает информацию о каталоге с идентификатором
                result = new DirectoryData((int)paramDirId.Value, data.Path, data.Status, data.Active);
                _DataHelper.Commit();
            }
            data = result;
            return true;
        }

        /// <summary>Удалить каталог видеоархива из базы данных</summary>
        /// <param name="id">Идентификатор каталога видеоархива в базе данных</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение входного параметра вне допустимого диапазона значений</exception>
        public bool DeleteDirectory(int id)
        {
            if (id < 1) return false;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlDeleteBuilder
                {
                    Table = "[dbo].[directories]",
                    Where = "[DirId] = @id"
                };
                int result = _DataHelper.ExecuteNoneQuery(sql, new SqlParameter("id", id));
                if (result == 0) return false;
                _DataHelper.Commit();
                return true;
            }
        }

        /// <summary>Изменить данные о каталоге видеоархива в базе данных</summary>
        /// <param name="data">Измененные данные каталога видеоархива</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойства заданного объекта</exception>
        public bool ModifyDirectory(DirectoryData data)
        {
            CheckDirectory(data);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                List<DbParameter> _params = new List<DbParameter>(3);
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlUpdateBuilder
                {
                    Table = "[dbo].[directories]",
                    Fields = new string[] { "[DirPath]", "[DirStat]" },
                    Values = new string[] { "@path", "@stat" },
                    Where = "[DirId] = @id"
                };
                _params.Add(new SqlParameter("path", data.Path));
                _params.Add(new SqlParameter("stat", (int)data.Status));
                _params.Add(new SqlParameter("id", data.Id));
                int result = _DataHelper.ExecuteNoneQuery(sql, _params);
                if (result == 0) return false;
                _DataHelper.Commit();
                return true;
            }
        }

        #endregion

        #region Marked frames

        /// <summary>Получить данные о маркированных кадрах видеозаписи поезда</summary>
        /// <param name="train_id">Идентификатор поезда (0 - любой поезд)</param>
        /// <param name="cam_id">Идентификатор видеокамеры (1 .. 4; 0 - любая видеокамера)</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение аргумента вне допустимого диапазона значений</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        /// <returns>Список маркированных кадров</returns>
        public IList<MarkedFrame> GetFrameMarkList(int train_id, int cam_id)
        {
            if (train_id < 0) throw new ArgumentOutOfRangeException("train_id");
            if (cam_id < 0 || cam_id > 4) throw new ArgumentOutOfRangeException("cam_id");
            List<MarkedFrame> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = null;
                if (train_id > 0)
                {
                    _params.Add(new SqlParameter("TrainId", train_id));
                    where_str = AND(where_str, "([TrainId] = @TrainId)");
                }
                if (cam_id > 0)
                {
                    _params.Add(new SqlParameter("CameraId", cam_id));
                    where_str = AND(where_str, "([CameraId] = @CameraId)");
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[TrainId]", "[CameraId]", "[ParamValue] AS CamName", "[TimeSpan]" },
                    From = "[dbo].[marked_frames] JOIN [dbo].[config] ON [ParamName] = 'Camera.' + STR([CameraId], 1, 0) + '.Name' ",
                    Where = where_str,
                    OrderBy = "[TimeSpan]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<MarkedFrame> fmDataQuery =
                    from fm in dataTable.AsEnumerable()
                    select new MarkedFrame((int)fm[0], (int)fm[1], (string)fm[2], (int)fm[3] );
                result = fmDataQuery.ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Добавляет маркер на кадр видеозаписи поезда</summary>
        /// <param name="data">Данные маркера видеокадра</param>
        /// <exception cref="System.ArgumentException">Недопустимое значение аргумента</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public void AddFrameMark(MarkedFrame data)
        {
            CheckFrameMarkData(data);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>(3);
                _params.Add(new SqlParameter("TrainId", data.TrainId));
                _params.Add(new SqlParameter("CameraId", data.CameraId));
                _params.Add(new SqlParameter("TimeSpan", data.TimeSpan));
                string query = (string)new SqlInsertBuilder()
                {
                    Table = "[dbo].[marked_frames]",
                    Fields = new string[] { "[TrainId]", "[CameraId]", "[TimeSpan]" },
                    Values = new string[] { "@TrainId", "@CameraId", "@TimeSpan" },
                };
                _DataHelper.ExecuteNoneQuery(query, _params);
                _DataHelper.Commit();
            }
        }

        /// <summary>Удаляет маркер на кадр видеозаписи поезда</summary>
        /// <param name="data">Данные маркера видеокадра</param>
        /// <exception cref="System.ArgumentException">Недопустимое значение аргумента</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public void DeleteFrameMark(MarkedFrame data)
        {
            CheckFrameMarkData(data);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>(3);
                _params.Add(new SqlParameter("TrainId", data.TrainId));
                _params.Add(new SqlParameter("CameraId", data.CameraId));
                _params.Add(new SqlParameter("TimeSpan", data.TimeSpan));
                string query = (string)new SqlDeleteBuilder()
                {
                    Table = "[dbo].[marked_frames]",
                    Where = "([TrainId] = @TrainId) AND ([CameraId] = @CameraId) AND ([TimeSpan] = @TimeSpan)"
                };
                int count = _DataHelper.ExecuteNoneQuery(query, _params);
                if (count == 0)
                {
                    throw new ArgumentException("Record not found!");
                }
                _DataHelper.Commit();
            }
        }

        /// <summary>Удаляет все маркеры видеокадров записи для заданного поезда</summary>
        /// <param name="train_id">Идентификатор поезда в базе данных АСКОПВ</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение аргумента</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public void DeleteFrameMark(int train_id)
        {
            if (train_id < 1) throw new ArgumentOutOfRangeException("train_id");
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(IsolationLevel.ReadCommitted);
                string query = (string)new SqlDeleteBuilder()
                {
                    Table = "[dbo].[marked_frames]",
                    Where = "([TrainId] = @TrainId)"
                };
                int count = _DataHelper.ExecuteNoneQuery(query, new SqlParameter("TrainId", train_id));
                if (count == 0)
                {
                    throw new ArgumentException("Records not found!");
                }
                _DataHelper.Commit();
            }
        }
        
        /// <summary>Проверить данные маркировки вагона на корректность перед занесением в базу данных</summary>
        /// <param name="data">Данные маркировки вагона</param>
        /// <exception cref="System.ArgumentException">Данные содержат ошибку</exception>
        protected virtual void CheckFrameMarkData(MarkedFrame data)
        {
            if (data == null) throw new ArgumentNullException();
            if (data.TrainId < 1) throw new ArgumentException("'TrainId' должен быть больше нуля!");
            if (data.CameraId < 1 || data.CameraId > 4) throw new ArgumentException("'CameraId' должна быть в диапазоне: 1..4!");
            if (data.TimeSpan < 0) throw new ArgumentException("'TimeSpan' должен быть больше или равен нуля!");
        }

        #endregion

        #region Status

        /// <summary>Получить статус устройства из таблицы "status"</summary>
        /// <param name="device">Заданное имя устройства</param>
        /// <returns>Статус устройства</returns>
        public string GetStat(string device)
        {
            return GetStringStat(device);
        }
        
        /// <summary>Установить статус перезагрузки службы</summary>
        /// <param name="restart">Статус перезагрузки</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetRestart()
        {
            SetStat("Server.Restart", true.ToString());
        }

        /// <summary>Установить статус записи АРМ</summary>
        /// <param name="stat">Статус БИС</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetArmMode(DevMode mode)
        {
            SetStat("Arm.Mode", mode.ToString());
        }

        /// <summary>Получить статус АРМ</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Статус АРМ</returns>
        public DevMode GetArmMode()
        {
            string mode = GetStringStat("Arm.Mode");
            for (int i = 0; i < Enum.GetNames(typeof(DevMode)).Length; i++)
                if (mode == Enum.GetNames(typeof(DevMode)).GetValue(i).ToString())
                    return (DevMode)Enum.GetValues(typeof(DevMode)).GetValue(i);
            return DevMode.unknown;
        }

        #region БС

        /// <summary>Получить коммуникационный статус БС</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Статус БИС</returns>
        public DevState GetBisStat()
        {
            //Проверка - включен ли БС
            bool enabled = GetBoolConfigParam("BS.Active", false);
            if (enabled == false) return DevState.none;
            //Вернуть текущий статус
            return GetDevStat("BS");
        }

        /// <summary>Установить коммуникационный статус БИС</summary>
        /// <param name="stat">Статус БИС</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetBisStat(DevState stat)
        {
            SetStat("BS", stat.ToString());
        }

        /// <summary>Установить статус датчика вскрытия БС</summary>
        /// <param name="stat">Статус БИС</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetBisTamper(TamperStat stat)
        {
            SetStat("Tamper", stat.ToString());
        }

        /// <summary>Установить коммуникационный статус БИС</summary>
        /// <param name="stat">Статус БИС</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetBisMode(DevMode mode)
        {
            SetStat("BS.Mode", mode.ToString());
        }

        /// <summary>Получить коммуникационный статус БИС</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Статус БИС</returns>
        public DevMode GetBisMode()
        {
            bool enabled = GetBoolConfigParam("BS.Active", false);
            if (enabled == false) return DevMode.unknown;
            string mode = GetStringStat("BS.Mode");
            for (int i = 0; i < Enum.GetNames(typeof(DevMode)).Length; i++)
                if (mode == Enum.GetNames(typeof(DevMode)).GetValue(i).ToString())
                    return (DevMode)Enum.GetValues(typeof(DevMode)).GetValue(i);
            return DevMode.unknown;
        }

        #endregion

        #region ASU

        /// <summary>Получить количество запросов от АСУ СТ</summary>
        /// <returns></returns>
        public int GetASURequestCount()
        {
            int result = 0;
            try
            {
                result = Convert.ToInt32(GetStringStat("ASU.RequestCount"));
            }
            catch { }
            return result;
        }

        /// <summary>Установить количество запросов от АСУ СТ</summary>
        /// <param name="count"></param>
        public void SetASURequestCount(int count)
        {
            SetStat("ASU.RequestCount", count.ToString());
        }

        /// <summary>Получить количество ответов на запросы АСУ СТ</summary>
        /// <returns></returns>
        public int GetASUReplyCount()
        {
            int result = 0;
            try
            {
                result = Convert.ToInt32(GetStringStat("ASU.ReplyCount"));
            }
            catch { }
            return result;
        }

        /// <summary>Установить количество ответов на запросы АСУ СТ</summary>
        /// <param name="count"></param>
        public void SetASUReplyCount(int count)
        {
            SetStat("ASU.ReplyCount", count.ToString());
        }

        /// <summary>Получить дату сброса счетчиков запросов/ответов АСУ СТ</summary>
        /// <returns></returns>
        public DateTime GetASUResetDate()
        {
            DateTime result = DateTime.MinValue;
            try
            {
                string value = GetStringStat("ASU.ResetDate");
                if (string.IsNullOrEmpty(value) == false)
                {
                    result = Convert.ToDateTime(value);
                }
            }
            catch { }
            return result;
        }

        /// <summary>Установить дату сброса счетчиков запросов/ответов АСУ СТ</summary>
        /// <param name="value"></param>
        public void SetASUResetDate(DateTime value)
        {
            SetStat("ASU.ResetDate", value.ToString());
        }

        #endregion

        #region Датчик вскрытия

        /// <summary>Получить статус датчика вскрытия шкафа</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Статус датчика вскрытия шкафа</returns>
        /// <remarks>Предполагается, что датчик вскрытия шкафа в таблице status обозначется как 'Tamper'</remarks>
        public TamperStat GetTamperStat()
        {
            string status_name = GetStringStat("Tamper");
            for (var status = TamperStat.unknown; status <= TamperStat.alarm; ++status)
                if (status.ToString() == status_name) return status;
            return TamperStat.unknown;
        }

        /// <summary>Установить статус датчика вскрытия шкафа</summary>
        /// <param name="stat">Статус датчика вскрытия шкафа</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetTamperStat(int point, TamperStat stat)
        {
            SetStat("Tamper", stat.ToString());
        }

        #endregion

        #region Телекамера

        /// <summary>Получить коммуникационный статус камеры</summary>
        /// <param name="camera">Номер камеры (1...5)</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Номер камеры вне допустимого диапазона значений</exception>
        /// <returns>Статус камеры</returns>
        public DevState GetCameraStat(int camera)
        {
            if ((camera < 1) || (camera > 6)) throw new ArgumentOutOfRangeException("camera");
            int cam_id = camera - 1;
            //Получение активности канала
            bool enabled = GetBoolConfigParam("Camera." + cam_id.ToString() + ".Active", false);
            if (enabled == false) return DevState.none;
            return GetDevStat("Camera." + cam_id.ToString());
        }

        /// <summary>Получить коммуникационный статус камеры</summary>
        /// <param name="connection">Подключение</param>
        /// <param name="point">Номер пункта считывания (0..4)</param>
        /// <param name="cam_id">Номер камеры (0..5)</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Номер камеры вне допустимого диапазона значений</exception>
        /// <returns>Статус камеры</returns>
        DevState GetCameraStat(SqlDataHelper connection, int cam_id)
        {
            //Получение активности канала
            bool enabled = GetBoolConfigParam(connection, "Channel." + cam_id.ToString() + ".Active", false);
            if (enabled == false) return DevState.none;
            string stat = GetStringStat("Camera." + cam_id.ToString());
            if (stat == DevState.offline.ToString()) return DevState.offline;
            if (stat == DevState.online.ToString()) return DevState.online;
            return DevState.unknown;
        }

        /// <summary>Установить коммуникационный статус камеры</summary>
        /// <param name="camera">Номер камеры (1...6)</param>
        /// <param name="stat">Статус камеры</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetCameraStat(int camera, DevState stat, int minCamersCount, int maxCamersCount)
        {
            if ((camera < minCamersCount) || (camera > maxCamersCount)) throw new ArgumentOutOfRangeException("camera");
            int cam_id = camera - 1;
            SetStat("Camera." + cam_id.ToString(), stat.ToString());
        }

        #endregion

        #endregion
                
        #region Parameters

        /// <summary>Получить значение параметра конфигурации</summary>
        /// <param name="param_name">Имя параметра</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Не задано имя конфигурационного параметра</exception>
        /// <returns>Значение параметра или null, если параметр не существует</returns>
        public string GetConfigParam(string param_name)
        {
            if (string.IsNullOrEmpty(param_name))
            {
                throw new ArgumentException("Invalid argument!", "param_name");
            }
            string result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[ParamValue]" },
                    From = "[dbo].[config]",
                    Where = "[ParamName] = @name"
                };
                object obj = _DataHelper.ExecuteScalar(sql, new SqlParameter("name", param_name));
                if (obj != null)
                {
                    result = obj.ToString();
                }
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Установить значение параметра конфигурации</summary>
        /// <param name="param_name">Имя параметра</param>
        /// <param name="param_value">Значение параметра</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Не задано имя конфигурационного параметра</exception>
        public void SetConfigParam(string param_name, string param_value)
        {
            if (string.IsNullOrEmpty(param_name))
            {
                throw new ArgumentException("Invalid argument!", "param_name");
            }
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "COUNT([ParamName])" },
                    From = "[dbo].[config]",
                    Where = "[ParamName] = @name"
                };
                SqlParameter nameParam = new SqlParameter("name", param_name);
                SqlParameter valueParam = new SqlParameter("value", param_value);
                bool is_exist = (int)_DataHelper.ExecuteScalar(sql, nameParam) > 0;
                nameParam = (SqlParameter)CopyParam(nameParam);
                if (is_exist)
                {
                    sql = (string)new SqlUpdateBuilder
                    {
                        Table = "[dbo].[config]",
                        Fields = new string[] { "[ParamValue]" },
                        Values = new string[] { "@value" },
                        Where = "[ParamName] = @name"
                    };
                    _DataHelper.ExecuteNoneQuery(sql, nameParam, valueParam);
                }
                else
                {
                    sql = (string)new SqlInsertBuilder
                    {
                        Table = "[dbo].[config]",
                        Fields = new string[] { "[ParamName]", "[ParamValue]" },
                        Values = new string[] { "@name", "@value" }
                    };
                    _DataHelper.ExecuteNoneQuery(sql, nameParam, valueParam);
                }
                _DataHelper.Commit();
            }
        }

        /// <summary>Получить значение параметра конфигурации (bool)</summary>
        /// <param name="param_name">Имя параметра</param>
        /// <param name="default_value">Значение по умолчанию</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Не задано имя конфигурационного параметра</exception>
        /// <exception cref="System.FormatException">Значение параметра конфигурации не относится к типу Boolean</exception>
        /// <returns>Значение параметра конфигурации</returns>
        public bool GetBoolConfigParam(string param_name, bool default_value)
        {
            string result = GetConfigParam(param_name);
            return string.IsNullOrEmpty(result) ? default_value : Boolean.Parse(result);
        }

        /// <summary>Получить значение параметра конфигурации (int)</summary>
        /// <param name="param_name">Имя параметра</param>
        /// <param name="default_value">Значение по умолчанию</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Не задано имя конфигурационного параметра</exception>
        /// <exception cref="System.FormatException">Значение параметра конфигурации не относится к типу Int32</exception>
        /// <returns>Значение параметра конфигурации</returns>
        public int GetIntConfigParam(string param_name, int default_value)
        {
            string result = GetConfigParam(param_name);
            return string.IsNullOrEmpty(result) ? default_value : Int32.Parse(result);
        }

        /// <summary>Получить значение параметра конфигурации (string)</summary>
        /// <param name="param_name">Имя параметра</param>
        /// <param name="default_value">Значение по умолчанию</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Не задано имя конфигурационного параметра</exception>
        /// <returns>Значение параметра конфигурации</returns>
        public string GetStringConfigParam(string param_name, string default_value)
        {
            string result = GetConfigParam(param_name);
            return string.IsNullOrEmpty(result) ? default_value : result;
        }

        /// <summary>Удаляет все параметры конфигурации</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void ClearParams()
        {
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlDeleteBuilder
                {
                    Table = "[dbo].[config]",
                    Where = "[ParamName] <> @param_value"
                };
                _DataHelper.ExecuteNoneQuery(sql, new SqlParameter("param_value", DataProvider.DbVersionParamName));
                _DataHelper.Commit();
            }
        }

        string GetConfigParam(SqlDataHelper connection, string param_name)
        {
            if (connection == null)
            {
                throw new ArgumentException("Invalid argument!", "connection");
            }
            if (string.IsNullOrEmpty(param_name))
            {
                throw new ArgumentException("Invalid argument!", "param_name");
            }
            string result = null;
            string sql = (string)new SqlSelectBuilder
            {
                Fields = new string[] { "[ParamValue]" },
                From = "[dbo].[config]",
                Where = "[ParamName] = @name"
            };
            object obj = connection.ExecuteScalar(sql, new SqlParameter("name", param_name));
            if (obj != null)
            {
                result = obj.ToString();
            }
            return result;
        }

        bool GetBoolConfigParam(SqlDataHelper connection, string param_name, bool default_value)
        {
            string result = GetConfigParam(connection, param_name);
            return string.IsNullOrEmpty(result) ? default_value : Boolean.Parse(result);
        }

        int GetIntConfigParam(SqlDataHelper connection, string param_name, int default_value)
        {
            string result = GetConfigParam(connection, param_name);
            return string.IsNullOrEmpty(result) ? default_value : Int32.Parse(result);
        }

        string GetStringConfigParam(SqlDataHelper connection, string param_name, string default_value)
        {
            string result = GetConfigParam(connection, param_name);
            return string.IsNullOrEmpty(result) ? default_value : result;
        }

        string GetCameraResolutionString(SqlDataHelper connection, int cam_id)
        {
            int width = GetIntConfigParam(connection, "Telecamera." + cam_id.ToString() + ".ResolutionX", 640);
            int height = GetIntConfigParam(connection, "Telecamera." + cam_id.ToString() + ".ResolutionY", 480);
            return width.ToString() + "x" + height.ToString();
        }

        /// <summary>Получает список параметров конфигурании и их значений</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список параметров конфигурации</returns>
        public Dictionary<string, string> GetConfigParamList()
        {
            Dictionary<string, string> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[ParamName]", "[ParamValue]" },
                    From = "[dbo].[config]",
                    OrderBy = "[ParamName]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql);
                result = new Dictionary<string, string>(dataTable.Rows.Count);
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add((string)row[0], (string)row[1]);
                }
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Установить статус изменения параметров</summary>
        /// <param name="change">Статус изменения параметров</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetChangeParams(bool change)
        {
            SetStat("Server.ChangeParams", change.ToString());
        }

        /// <summary>Получить статус изменения параметров</summary>
        public bool GetChangeParams()
        {
            string val = GetStat("Server.ChangeParams");
            try { return bool.Parse(val); }
            catch { return false; }
        }

        #endregion

        #region Telemetry

        /// <summary>Добавить данные о состоянии датчиков счета в базу данных</summary>
        /// <param name="data">Состояние датчиков счета</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Свойство значение свойства объекта data</exception>
        /// <exception cref="System.Data.SqlClient.SqlException">Не удалось выполнить запрос</exception>
        public void AddBsSensorsData(BsSensorsData data)
        {
            if (data == null) throw new ArgumentNullException();
            if (data.TrainId < 1) throw new ArgumentOutOfRangeException("Illegal value of property!", "TrainId");

            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                //Параметры
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("trainid", data.TrainId));
                _params.Add(new SqlParameter("timestamp", data.TimeSpan));
                _params.Add(new SqlParameter("state", data.Sensor.StateMask));
                //Команда
                string sql = (string)new SqlInsertBuilder
                {
                    Table = "[dbo].[bs_input_data]",
                    Fields = new string[] { "[TrainId]", "[TimeSpan]", "[StateMask]" },
                    Values = new string[] { "@trainid", "@timestamp", "@state" }
                };
                _DataHelper.ExecuteNoneQuery(sql, _params);
                _DataHelper.Commit();
            }

        }

        /// <summary>Добавить отладочные данные переменных блока счета в базу данных</summary>
        /// <param name="data">Данные переменных блока счета</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Свойство FreeSpaceLimit имеет недопустимое значение</exception>
        /// <exception cref="System.Data.SqlClient.SqlException">Не удалось выполнить запрос</exception>
        public void AddBsVariablesData(BsVariablesData data)
        {

            if (data == null) throw new ArgumentNullException();
            if (data.TrainId < 1) throw new ArgumentOutOfRangeException("Illegal value of property!", "TrainId");

            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                //Параметры
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("trainid", data.TrainId));
                _params.Add(new SqlParameter("timestamp", data.TimeSpan));
                _params.Add(new SqlParameter("sensor1", data.Sensor1));
                _params.Add(new SqlParameter("sensor2", data.Sensor2));
                _params.Add(new SqlParameter("wagon", data.WagNum));
                _params.Add(new SqlParameter("direction", (int)data.Direction));
                //Команда
                string sql = (string)new SqlInsertBuilder
                {
                    Table = "[dbo].[bs_output_data]",
                    Fields = new string[] { "[TrainId]", "[TimeSpan]", "[Sensor1]", "[Sensor2]", "[WagonNum]", "[Direction]" },
                    Values = new string[] { "@trainid", "@timestamp", "@sensor1", "@sensor2", "@wagon", "@direction" }
                };
                _DataHelper.ExecuteNoneQuery(sql, _params);
                _DataHelper.Commit();
            }

        }

        /// <summary>Получить данные датчиков для указанного состава</summary>
        /// <param name="train_id">Идентификатор поезда</param>
        public IList<BsSensorsData> GetSensorsData(int train_id)
        {
            if (train_id < 0) throw new ArgumentOutOfRangeException("train_id");

            List<BsSensorsData> result = new List<BsSensorsData>();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[TimeSpan] < 20000000";
                if (train_id > 0)
                {
                    where_str = AND(where_str, "([TrainId] = @train_id)\n");
                    _params.Add(new SqlParameter("train_id", train_id));
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[TrainId]", "[TimeSpan]", "[StateMask]" },
                    From = "[dbo].[bs_input_data]",
                    Where = where_str,
                    OrderBy = "[bs_input_data].[TimeSpan] ASC"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<BsSensorsData> SensorsDataQuery =
                    from sensors in dataTable.AsEnumerable()
                    let train = sensors.IsNull("TrainId") ? 0 : (int)sensors["TrainId"]
                    let span = sensors.IsNull("TimeSpan") ? 0 : (int)sensors["TimeSpan"]
                    let mask = sensors.IsNull("StateMask") ? 0 : (int)sensors["StateMask"]
                    select new BsSensorsData(train, span, mask);

                // Формируем список результирующих записей
                foreach (var sensor_data in SensorsDataQuery)
                {
                    result.Add(sensor_data);
                }
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить данные переменных для указанного состава</summary>
        /// <param name="train_id">Идентификатор поезда</param>
        public IList<BsVariablesData> GetVariablesData(int train_id)
        {
            if (train_id < 0) throw new ArgumentOutOfRangeException("train_id");

            List<BsVariablesData> result = new List<BsVariablesData>();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[TimeSpan] < 20000000";
                if (train_id > 0)
                {
                    where_str = AND(where_str, "([TrainId] = @train_id)\n");
                    _params.Add(new SqlParameter("train_id", train_id));
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[TrainId]", "[TimeSpan]", "[Sensor1]", "[Sensor2]", "[WagonNum]", "[Direction]" },
                    From = "[dbo].[bs_output_data]",
                    Where = where_str,
                    OrderBy = "[bs_output_data].[TimeSpan] ASC"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<BsVariablesData> VariablesDataQuery =
                    from sensors in dataTable.AsEnumerable()
                    let train = sensors.IsNull("TrainId") ? 0 : (int)sensors["TrainId"]
                    let span = sensors.IsNull("TimeSpan") ? 0 : (int)sensors["TimeSpan"]
                    let sensor1 = sensors.IsNull("Sensor1") ? 0 : (int)sensors["Sensor1"]
                    let sensor2 = sensors.IsNull("Sensor2") ? 0 : (int)sensors["Sensor2"]
                    let wagon_num = sensors.IsNull("WagonNum") ? 0 : (int)sensors["WagonNum"]
                    let direction = sensors.IsNull("Direction") ? 0 : (int)sensors["Direction"]
                    select new BsVariablesData(train, span, sensor1, sensor2, wagon_num, direction);

                // Формируем список результирующих записей
                foreach (var sensor_data in VariablesDataQuery)
                {
                    result.Add(sensor_data);
                }
                _DataHelper.Commit();
            }
            return result;
        }

        #endregion

        #region Cargo (объединить с запросом вагонов)

        /// <summary>Поля выборки из таблицы "wagons", "wagons_cargo" и "wagons_ocr" - представление "v_wagons_data"</summary>
        string[] WagonFieldsCargo = new string[] {  //wagons
                                                    "[v_wagons_data].[WagonId]",           "[v_wagons_data].[WagonSn]", 
                                                    "[v_wagons_data].[SnSost]",            "[v_wagons_data].[Loco]",              
                                                    "[v_wagons_data].[TrainId]",           "[v_wagons_data].[InvNum]",            
                                                    "[v_wagons_data].[TimeSpanBegin]",     "[v_wagons_data].[TimeSpanEnd]",
                                                    "[v_wagons_data].[TimeSpanBeginBS]",   "[v_wagons_data].[TimeSpanEndBS]",
                                                    "[v_wagons_data].[SpeedBegin]",        "[v_wagons_data].[SpeedEnd]",
                                                    "[v_wagons_data].[DirectionBegin]",    "[v_wagons_data].[DirectionEnd]",
                                                    "[v_wagons_data].[Ngb]",               "[v_wagons_data].[Mark]",
                                                    "[v_wagons_data].[Comment]",           "[v_wagons_data].[TimeChanged]",
                                                    "[v_wagons_data].[Banned]",                   
                                                    //cargo_result
                                                    "[v_wagons_data].[shift_x]",           "[v_wagons_data].[shift_y]",           
                                                    "[v_wagons_data].[shift_z]",           "[v_wagons_data].[volume]",               
                                                    "[v_wagons_data].[width]",             "[v_wagons_data].[height]",            
                                                    "[v_wagons_data].[length]",            "[v_wagons_data].[width_max]",    
                                                    "[v_wagons_data].[height_max]",        "[v_wagons_data].[cargo_exist]",       
                                                    "[v_wagons_data].[cargo_clear]",       "[v_wagons_data].[cargo_type]",
                                                    "[v_wagons_data].[wagon_type]",        "[v_wagons_data].[wagon_model]",
                                                    "[v_wagons_data].[floor_level]",   
                                                    //cargo_compare_result /*Поля для хранения результатов смещения груза*/
                                                    "[v_wagons_data].server_id",
                                                    "[v_wagons_data].remote_train_id",  "[v_wagons_data].remote_train_number",  "[v_wagons_data].remote_train_index",
                                                    "[v_wagons_data].remote_wagon_sn",  "[v_wagons_data].remote_wagon_id",
                                                    "[v_wagons_data].remote_wagon_date",
                                                    "[v_wagons_data].diff_shift_x",     "[v_wagons_data].diff_shift_y",         "[v_wagons_data].diff_shift_z",
                                                    "[v_wagons_data].diff_volume",      "[v_wagons_data].diff_width",           "[v_wagons_data].diff_height",
                                                    "[v_wagons_data].diff_length",      "[v_wagons_data].diff_width_max",       "[v_wagons_data].diff_height_max",
                                                    "[v_wagons_data].diff_cargo_exist",
                                                    "[v_wagons_data].diff_cargo_clear",
                                                    "[v_wagons_data].is_relation_model",
                                                    "[v_wagons_data].diff_speed",
                                                    "[v_wagons_data].valid", 
                                                    "[v_wagons_data].compare_date",
                                                    "[v_wagons_data].operator_id",
                                                    //ocr
                                                    "[v_wagons_data].[ocr_inv_num]",
                                                    "[v_wagons_data].[ocr_type]",          "[v_wagons_data].[ocr_accuracy]",
                                                    "[v_wagons_data].[ocr_check]",         "[v_wagons_data].[ocr_train]", 
                                                    //ngb
                                                    "[wagons_ngb].[ngb_zonal]", "[wagons_ngb].[ngb_main]",  "[wagons_ngb].[ngb_soft]",
                                                    "[wagons_ngb].[ngb_grade1]", "[wagons_ngb].[ngb_grade2]", "[wagons_ngb].[ngb_grade3]",
                                                    "[wagons_ngb].[ngb_grade4]", "[wagons_ngb].[ngb_grade5]", "[wagons_ngb].[ngb_grade6]", "[wagons_ngb].[ngb_gradeEx]",
                                                    "[wagons_ngb].[ngb_static_t]", "[wagons_ngb].[ngb_static_tpr]", "[wagons_ngb].[ngb_static_1t]", "[wagons_ngb].[ngb_static_tc]",
                                                    "[wagons_ngb].[ngb_static_1vm]", "[wagons_ngb].[ngb_static_0vm]", "[wagons_ngb].[ngb_static_02vm]", "[wagons_ngb].[ngb_static_03vm]",
                                                    "[wagons_ngb].[ngb_build]",
                                                    //inv nubers
                                                    "[numbers].[Inv]",
                                                    "[wagons_models].[model]", "[wagons_models].[ngb_code]" };

        /// <summary>Коректировка номера вагона  </summary>
        /// <param name="wagons"></param>
        private void SetWagonSnShow(ref List<WagonData> wagons)
        {
            if (wagons == null || wagons != null && wagons.Count() < 1)
                return;
            int locoCount = 0;
            bool handled = false;
            foreach (WagonData wagon in wagons)
            {
                if (wagon.Loco < 1)
                    handled = true;
                if (!handled)
                {
                    wagon.SnShow = wagon.SnSost;
                    locoCount++;
                }
                else
                {
                    wagon.SnShow = wagon.SnSost - locoCount;
                }

            }
        }
        private void SetSnShow(ref WagonData wagon, int locoCount)
        {
            if (wagon.Loco > 0 && wagon.SnSost <= locoCount)
                wagon.SnShow = wagon.SnSost;
            else
                wagon.SnShow = wagon.SnSost - locoCount;
        }
        /// <summary>Получение объектов вагонов из результатов выборки вагонов</summary>
        /// <param name="dataTable">Выбранная таблица данных</param>
        IEnumerable<WagonData> WagonAndCargoQuerySelect(DataTable dataTable)
        {
            try
            {
                IEnumerable<WagonData> WagonDataQuery = from wagons in dataTable.AsEnumerable()

                                                        #region Wagon main data

                                                        let wag_id = (int)wagons["WagonId"]
                                                        let sn = (int)wagons["WagonSn"]
                                                        let sn_sost = wagons.IsNull("SnSost") ? 0 : (int)wagons["SnSost"]
                                                        let loco = wagons.IsNull("Loco") ? 0 : (int)wagons["Loco"]
                                                        let train_id = (int)wagons["TrainId"]
                                                        let inv_num = wagons.IsNull("InvNum") ? (wagons.IsNull("ocr_inv_num") ? string.Empty : (string)wagons["ocr_inv_num"]) : (string)wagons["InvNum"]
                                                        let inv_num_nl = wagons.IsNull("Inv") ? string.Empty : (string)wagons["Inv"]
                                                        let time_span_begin = wagons.IsNull("TimeSpanBegin") ? 0 : (int)wagons["TimeSpanBegin"]
                                                        let time_span_end = wagons.IsNull("TimeSpanEnd") ? 0 : (int)wagons["TimeSpanEnd"]
                                                        let time_span_begin_bs = wagons.IsNull("TimeSpanBeginBS") ? 0 : (int)wagons["TimeSpanBeginBS"]
                                                        let time_span_end_bs = wagons.IsNull("TimeSpanEndBS") ? 0 : (int)wagons["TimeSpanEndBS"]
                                                        let speed_begin = wagons.IsNull("SpeedBegin") ? 0 : (int)wagons["SpeedBegin"]
                                                        let speed_end = wagons.IsNull("SpeedEnd") ? 0 : (int)wagons["SpeedEnd"]
                                                        let direction_begin = wagons.IsNull("DirectionBegin") ? 0 : (int)wagons["DirectionBegin"]
                                                        let direction_end = wagons.IsNull("DirectionEnd") ? 0 : (int)wagons["DirectionEnd"]
                                                        let ngb = wagons.IsNull("Ngb") ? 0 : (int)wagons["Ngb"]
                                                        let mark = wagons.IsNull("Mark") ? 0 : (int)wagons["Mark"]
                                                        let banned = wagons.IsNull("Banned") ? 0 : (int)wagons["Banned"]
                                                        let comment = wagons.IsNull("Comment") ? string.Empty : (string)wagons["Comment"]
                                                        let time_changed = wagons.IsNull("TimeChanged") ? DateTime.MinValue : (DateTime)wagons["TimeChanged"]

                                                        #endregion

                                                        #region Askin Data

                                                        let ocr_inv_num = wagons.IsNull("ocr_inv_num") ? string.Empty : (string)wagons["ocr_inv_num"]
                                                        let ocr_type = wagons.IsNull("ocr_type") ? 0 : (int)wagons["ocr_type"]
                                                        let ocr_acc = wagons.IsNull("ocr_accuracy") ? 0 : (int)wagons["ocr_accuracy"]
                                                        let trainAskinId = wagons.IsNull("ocr_train") ? -1 : (int)wagons["ocr_train"]
                                                        let ocr_userCheck = wagons.IsNull("ocr_check") ? 0 : (int)wagons["ocr_check"]

                                                        #endregion

                                                        #region Cargo

                                                        let shift_x = wagons.IsNull("shift_x") ? 0 : (int)wagons["shift_x"]
                                                        let shift_y = wagons.IsNull("shift_y") ? 0 : (int)wagons["shift_y"]
                                                        let shift_z = wagons.IsNull("shift_z") ? 0 : (int)wagons["shift_z"]
                                                        let length = wagons.IsNull("length") ? 0 : (int)wagons["length"]
                                                        let height = wagons.IsNull("height") ? 0 : (int)wagons["height"]
                                                        let width = wagons.IsNull("width") ? 0 : (int)wagons["width"]
                                                        let volume = wagons.IsNull("volume") ? 0 : (int)wagons["volume"]
                                                        let width_max = wagons.IsNull("width_max") ? 0 : (int)wagons["width_max"]
                                                        let height_max = wagons.IsNull("height_max") ? 0 : (int)wagons["height_max"]
                                                        let cargo_exist = wagons.IsNull("cargo_exist") ? false : (bool)wagons["cargo_exist"]
                                                        let cargo_clear = wagons.IsNull("cargo_clear") ? false : (bool)wagons["cargo_clear"]
                                                        let cargo_type = wagons.IsNull("cargo_type") ? 1 : (int)wagons["cargo_type"]
                                                        let wagon_type = wagons.IsNull("wagon_type") ? 0 : (int)wagons["wagon_type"]
                                                        let wagon_model = wagons.IsNull("wagon_model") ? string.Empty : (string)wagons["wagon_model"]
                                                        let floor_level = wagons.IsNull("floor_level") ? 0 : (int)wagons["floor_level"]

                                                        #endregion

                                                        #region Cargo Compare Result

                                                        let server_id = wagons.IsNull("server_id") ? -1 : (int)wagons["server_id"]
                                                        let remote_train_id = wagons.IsNull("remote_train_id") ? 0 : (int)wagons["remote_train_id"]
                                                        let remote_train_number = wagons.IsNull("remote_train_number") ? string.Empty : (string)wagons["remote_train_number"]
                                                        let remote_train_index = wagons.IsNull("remote_train_index") ? string.Empty : (string)wagons["remote_train_index"]
                                                        let remote_wagon_sn = wagons.IsNull("remote_wagon_sn") ? 0 :(int)wagons["remote_wagon_sn"]
                                                        let remote_wagon_id = wagons.IsNull("remote_wagon_id") ? 0 : (int)wagons["remote_wagon_id"]
                                                        let remote_wagon_date = wagons.IsNull("remote_wagon_date") ? DateTime.MinValue :(DateTime)wagons["remote_wagon_date"]

                                                        let diff_shift_x = wagons.IsNull("diff_shift_x") ? 0 : (int)wagons["diff_shift_x"]
                                                        let diff_shift_y = wagons.IsNull("diff_shift_y") ? 0 : (int)wagons["diff_shift_y"]
                                                        let diff_shift_z = wagons.IsNull("diff_shift_z") ? 0 : (int)wagons["diff_shift_z"]

                                                        let diff_volume = wagons.IsNull("diff_volume") ? 0 : (int)wagons["diff_volume"]
                                                        let diff_width = wagons.IsNull("diff_width") ? 1 : (int)wagons["diff_width"]
                                                        let diff_height = wagons.IsNull("diff_height") ? 0 :(int)wagons["diff_height"]
                                                        let diff_length = wagons.IsNull("diff_length") ? 0 :(int)wagons["diff_length"]

                                                        let diff_width_max = wagons.IsNull("diff_width_max") ? 0 : (int)wagons["diff_width_max"]
                                                        let diff_height_max = wagons.IsNull("diff_height_max") ? 0 : (int)wagons["diff_height_max"]
                                                        let diff_cargo_exist = wagons.IsNull("diff_cargo_exist") ? false : (int)wagons["diff_cargo_exist"] > 0
                                                        let diff_cargo_clear = wagons.IsNull("diff_cargo_clear") ? false : (int)wagons["diff_cargo_clear"] > 0
                                                        let is_relation_model = wagons.IsNull("is_relation_model") ? false : (int)wagons["is_relation_model"] > 0
                                                        let diff_speed = wagons.IsNull("diff_speed") ? 0.0f : (float)((double)wagons["diff_speed"])
                                                        let valid = wagons.IsNull("valid") ? -1 : (int)wagons["valid"]
                                                        let compare_date = wagons.IsNull("compare_date") ? DateTime.MinValue : (DateTime)wagons["compare_date"]
                                                        let operator_id = wagons.IsNull("operator_id") ? 0 : (int)wagons["operator_id"]

                                                        #endregion

                                                        #region Расширенные негабариты версии 2

                                                        let ngb_zonal = wagons.IsNull("ngb_zonal") ? 0 : (int)wagons["ngb_zonal"]
                                                        let ngb_main = wagons.IsNull("ngb_main") ? 0 : (int)wagons["ngb_main"]
                                                        let ngb_soft = wagons.IsNull("ngb_soft") ? 0 : (int)wagons["ngb_soft"]
                                                        let ngb_grade1 = wagons.IsNull("ngb_grade1") ? 0 : (int)wagons["ngb_grade1"]
                                                        let ngb_grade2 = wagons.IsNull("ngb_grade2") ? 0 : (int)wagons["ngb_grade2"]
                                                        let ngb_grade3 = wagons.IsNull("ngb_grade3") ? 0 : (int)wagons["ngb_grade3"]
                                                        let ngb_grade4 = wagons.IsNull("ngb_grade4") ? 0 : (int)wagons["ngb_grade4"]
                                                        let ngb_grade5 = wagons.IsNull("ngb_grade5") ? 0 : (int)wagons["ngb_grade5"]
                                                        let ngb_grade6 = wagons.IsNull("ngb_grade6") ? 0 : (int)wagons["ngb_grade6"]
                                                        let ngb_gradeEx = wagons.IsNull("ngb_gradeEx") ? 0 : (int)wagons["ngb_gradeEx"]
                                                        let ngb_static_t = wagons.IsNull("ngb_static_t") ? 0 : (int)wagons["ngb_static_t"]
                                                        let ngb_static_tpr = wagons.IsNull("ngb_static_tpr") ? 0 : (int)wagons["ngb_static_tpr"]
                                                        let ngb_static_1t = wagons.IsNull("ngb_static_1t") ? 0 : (int)wagons["ngb_static_1t"]
                                                        let ngb_static_tc = wagons.IsNull("ngb_static_tc") ? 0 : (int)wagons["ngb_static_tc"]
                                                        let ngb_static_1vm = wagons.IsNull("ngb_static_1vm") ? 0 : (int)wagons["ngb_static_1vm"]
                                                        let ngb_static_0vm = wagons.IsNull("ngb_static_0vm") ? 0 : (int)wagons["ngb_static_0vm"]
                                                        let ngb_static_02vm = wagons.IsNull("ngb_static_02vm") ? 0 : (int)wagons["ngb_static_02vm"]
                                                        let ngb_static_03vm = wagons.IsNull("ngb_static_03vm") ? 0 : (int)wagons["ngb_static_03vm"]
                                                        let ngb_build = wagons.IsNull("ngb_build") ? 0 : (int)wagons["ngb_build"]

                                                        #endregion

                                                        #region Таблица моделей вагонов и кодов габарита подвижного состава
                                                        let ngb_code = wagons.IsNull("ngb_code") ? 0 : (int)wagons["ngb_code"]
                                                        let model = wagons.IsNull("model") ? string.Empty : (string)wagons["model"]
                                                        #endregion

                                                        select new WagonData
                                                        {
                                                            //Main data
                                                            Sn = sn,
                                                            SnSost = sn_sost,
                                                            Loco = loco,
                                                            TrainId = train_id,
                                                            WagId = wag_id,
                                                            InvNumber = string.IsNullOrEmpty(inv_num) ? ocr_inv_num : inv_num,
                                                            InvNumByNL = inv_num_nl,
                                                            TimeSpanBegin = time_span_begin,
                                                            TimeSpanEnd = time_span_end,
                                                            TimeSpanBeginBS = time_span_begin_bs,
                                                            TimeSpanEndBS = time_span_end_bs,
                                                            SpeedBegin = speed_begin,
                                                            SpeedEnd = speed_end,
                                                            DirectionBegin = direction_begin,
                                                            DirectionEnd = direction_end,
                                                            Ngb = ngb,
                                                            Mark = mark == 1,
                                                            Banned = banned == 1,
                                                            TimeChanged = time_changed,
                                                            Comment = comment,
                                                            //ASKIN
                                                            Ocr_InvNumber = ocr_inv_num,
                                                            Ocr_Accuracy = ocr_acc,
                                                            Ocr_InvType = ocr_type,
                                                            Ocr_TrainId = trainAskinId,
                                                            Ocr_UserCheck = ocr_userCheck,
                                                            //Cargo
                                                            ShiftX = shift_x,
                                                            ShiftY = shift_y,
                                                            ShiftZ = shift_z,
                                                            CargoLength = length,
                                                            CargoHeight = height,
                                                            CargoWidth = width,
                                                            CargoVolume = volume,
                                                            CargoWidthMax = width_max,
                                                            CargoHeightMax = height_max,
                                                            CargoExist = cargo_exist,
                                                            CargoClear = cargo_clear,
                                                            CargoType = cargo_type,
                                                            WagonType = wagon_type,
                                                            WagonModel = string.IsNullOrEmpty(wagon_model) ? model : wagon_model,
                                                            FloorLevel = floor_level,
                                                            //Compare Result
                                                            RemoteServerId = server_id,
                                                            RemoteTrainId = remote_train_id,
                                                            RemoteTrainNumber = remote_train_number,
                                                            RemoteTrainIndex = remote_train_index,
                                                            RemoteWagonSn = remote_wagon_sn,
                                                            RemoteWagonId = remote_wagon_id,
                                                            RemoteWagonDate = remote_wagon_date,
                                                            DiffShiftX = diff_shift_x,
                                                            DiffShiftY = diff_shift_y,
                                                            DiffShiftZ = diff_shift_z,
                                                            DiffVolume = diff_volume,
                                                            DiffWidth = diff_width,
                                                            DiffHeght = diff_height,
                                                            DiffLength = diff_length,
                                                            DiffWidthMax = diff_width_max,
                                                            DiffHeightMax = diff_height_max,
                                                            DiffCargoExists = diff_cargo_exist,
                                                            DiffCargoClear = diff_cargo_clear,
                                                            IsRelationModel = is_relation_model,
                                                            DiffSpeed = diff_speed,
                                                            RemoteValid = valid,
                                                            CompareDate = compare_date,
                                                            OperatorID = operator_id,
                                                            //Ngb
                                                            NgbZonal = ngb_zonal,
                                                            NgbBase = ngb_main,
                                                            NgbSoft = ngb_soft,
                                                            NgbGrade1 = ngb_grade1,
                                                            NgbGrade2 = ngb_grade2,
                                                            NgbGrade3 = ngb_grade3,
                                                            NgbGrade4 = ngb_grade4,
                                                            NgbGrade5 = ngb_grade5,
                                                            NgbGrade6 = ngb_grade6,
                                                            NgbGradeEx = ngb_gradeEx,
                                                            NgbStatic_T = ngb_static_t,
                                                            NgbStatic_Tpr = ngb_static_tpr,
                                                            NgbStatic_1T = ngb_static_1t,
                                                            NgbStatic_Tc = ngb_static_tc,
                                                            NgbStatic_1VM = ngb_static_1vm,
                                                            NgbStatic_0VM = ngb_static_0vm,
                                                            NgbStatic_02VM = ngb_static_02vm,
                                                            NgbStatic_03VM = ngb_static_03vm,
                                                            NgbBuild = ngb_build,
                                                            //Модели вагонов и код габарита ПС
                                                            NgbCode = ngb_code,
                                                            Model = model
                                                        };
                return WagonDataQuery;
            }
            catch(Exception ex) { string debug = ex.Message; }
            return null;
        }
        
        /// <summary>Получить множество записей из журнала вагонов</summary>
        /// <param name="train_id">Идентификатор поезда/отцепа в СБВ УВГ (0 - любой идентификатор)</param>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="inv">Инвентарный номер вагона</param>
        /// <param name="start">Номер первой записи</param>
        /// <param name="count">Максимальное количество записей</param>
        /// <param name="total">Количество записей соответствующих критериям</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Список записей журнала взвешивания вагонов</returns>
        IList<WagonData> GetWagonAndCargoList(int train_id, DateTime begin, DateTime end,
                                              string inv, int start, int count, 
                                              out int total, SortOrder order)
        {
            if (train_id < 0) throw new ArgumentOutOfRangeException("train_id");

            List<WagonData> result = new List<WagonData>(count);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = null;
                if (train_id > 0)
                {
                    where_str = AND(where_str, "([v_wagons_data].[TrainId] = @train_id)\n");
                    _params.Add(new SqlParameter("train_id", train_id));
                }
                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] >= @time_begin)\n");
                    _params.Add(new SqlParameter("time_begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeEnd] <= @time_end)\n");
                    _params.Add(new SqlParameter("time_end", end));
                }
                if (!string.IsNullOrEmpty(inv))
                {
                    where_str = AND(where_str, "([InvNum] = @inv)\n");
                    _params.Add(new SqlParameter("inv", inv));
                }
                string orderInv = "[numbers].[Sn]";

                //Получение признака инверсии номеров натурного листа для состава
                bool invert_num = false;
                bool res = GetTrainInvertNumbers(train_id, ref invert_num);
                
                //Если состав не найден ничего не делать
                if (!res)
                {
                    total = 0;
                    return result;
                }

                if (invert_num) orderInv = "(SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = @train_id) - [numbers].[Sn] + 1";

                string sql = (string)new SqlSelectBuilder
                {
                    Top = start + count - 1 < 0 ? 0 : start + count - 1,
                    Fields = WagonFieldsCargo,
                    From = "[dbo].[v_wagons_data]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" + "[v_wagons_data].[TrainId] = [numbers].[TrainId] AND [v_wagons_data].[WagonSn] = " + orderInv + ") \r\n" +
                           " LEFT OUTER JOIN [dbo].[wagons_ngb] ON (" + "[v_wagons_data].[WagonId] = [wagons_ngb].[wagon_id]) ",
                    Where = where_str,
                   // OrderBy = order == SortOrder.Ascending ? "[v_wagons_data].[TimeSpanEnd]" : "[v_wagons_data].[TimeSpanEnd] DESC"
                    OrderBy = order == SortOrder.Ascending ? "[v_wagons_data].[WagonSn]" : "[v_wagons_data].[WagonSn] DESC"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<WagonData> WagonDataQuery = WagonAndCargoQuerySelect(dataTable);
               

                // Формируем список результирующих записей
                int index = 1;
                foreach (var wagon_data in WagonDataQuery)
                {
                    // Пропускаем первые start-1 записей если start
                    if (start > 0 && (index++ < start)) continue;
                    result.Add(wagon_data);
                    if (count > 0 && (result.Count >= count)) break;
                }

                SetWagonSnShow(ref result);

                // Запрашиваем общее количество записей, удовлетворяющих заданному критерию
                sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "Count([WagonSn])" },
                    From = "[dbo].[v_wagons_data]",
                    Where = where_str
                };
                total = (int)_DataHelper.ExecuteScalar(sql, CopyParams(_params));

                _DataHelper.Commit();
            }
            return result;
        }
        
        /// <summary>Получить множество записей из журнала вагонов новая версия - ускоренная</summary>
        /// <param name="train_id">Идентификатор поезда/отцепа в СБВ УВГ (0 - любой идентификатор)</param>
        /// <param name="order">Порядок сортировки</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Список записей журнала взвешивания вагонов</returns>
        IList<WagonData> GetWagonAndCargoListRenew(int train_id)
        {
            if (train_id < 0) throw new ArgumentOutOfRangeException("train_id");
            List<WagonData> result = new List<WagonData>(0);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                //Получение числа локомотивов
                int locoCount = GetTrainLocoCountByTrainID(_DataHelper, train_id);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = null;
                if (train_id > 0)
                {
                    where_str = AND(where_str, "([v_wagons_data].[TrainId] = @train_id)\n");
                    _params.Add(new SqlParameter("train_id", train_id));
                }
                string orderInv = "[numbers].[Sn]" + (locoCount == 0 ? string.Empty : string.Format(" + {0} ", locoCount));
                 
                //Получение признака инверсии номеров натурного листа для состава
                bool invert_num = false;
                bool res = GetTrainInvertNumbers(train_id, ref invert_num);

                string orderModel = (locoCount == 0 ? string.Empty : string.Format(" - {0} ", locoCount)) + "= [wagons_models].[wagon_sn]";
                //Если состав не найден ничего не делать
                if (!res) return result;
                if (invert_num)
                {
                    orderInv = "(SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = @train_id) - [numbers].[Sn] + 1 "/*+  (locoCount==0 ? string.Empty : string.Format(" + {0} ", locoCount))*/;
                    orderModel = "  = (SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = @train_id) - [wagons_models].[wagon_sn] + 1 ";
                }
                /*Проверить модель не - locoCount, а + locoCount*/
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = WagonFieldsCargo,
                    From = "[dbo].[v_wagons_data] \r\n" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" + "[v_wagons_data].[TrainId] = [numbers].[TrainId] AND [v_wagons_data].[WagonSn] = " + orderInv + ") \r\n" +
                           " LEFT OUTER JOIN [dbo].[wagons_ngb] ON (" + "[v_wagons_data].[WagonId] = [wagons_ngb].[wagon_id]) \r\n" +
                           " LEFT OUTER JOIN [dbo].[wagons_models] ON (" + "[v_wagons_data].[TrainId] = [wagons_models].[train_id] AND [v_wagons_data].[WagonSn]"+ orderModel + ") \r\n",
                    Where = where_str,
                    OrderBy = "[v_wagons_data].[WagonSn]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<WagonData> WagonDataQuery = WagonAndCargoQuerySelect(dataTable);
                //SetWagonSnShow(ref WagonDataQuery);
                // Формируем список результирующих записей
                foreach (var wagon_data in WagonDataQuery) result.Add(wagon_data);

                SetWagonSnShow(ref result);
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Запрос вагонов выбранного поезда</summary>
        /// <param name="train_id">Идентификатор поезда</param>
        /// <returns>Списко вагонов</returns>
        public IList<WagonData> GetWagonAndCargoList(int train_id)
        {
            return GetWagonAndCargoListRenew(train_id);
        }

        public int GetTrainLocoCount(SqlDataHelper helper, int wagonID)
        {
            int result = 0;
            try
            {
                string commandText = string.Format("select count(WagonId)from wagons ww  where ww.TrainId IN(select TrainId from wagons w where w.WagonId = {0}) and ww.Loco=1", wagonID);
               object res =  helper.ExecuteScalar(commandText, new DbParameter[] { });
               result = Convert.ToInt32(res);
            }
            catch(Exception ex) {
                result = 0;
            }
            return result;
        }

        public int GetTrainLocoCount(SqlDataHelper helper, int trainID, int wagonSn)
        {
            int result = 0;
            try
            {
                string commandText = string.Format("select count(WagonId)from wagons ww  where ww.TrainId IN(select TrainId from wagons w where w.TrainId = {0} and w.WagonSn = {1}) and ww.Loco=1", trainID, wagonSn);
                object res = helper.ExecuteScalar(commandText, 
                                                                new DbParameter[] { }
                                                  );
                result = Convert.ToInt32(res);
            }
            catch { result = 0; }
            return result;
        }
        
        public int GetTrainLocoCountByTrainID(SqlDataHelper helper, int trainID)
        {
            int result = 0;
            try
            {
                string commandText = string.Format("select count(w.WagonId) from wagons w where TrainId={0} and w.Loco=1", trainID);
                object res = helper.ExecuteScalar(commandText,
                                                                new DbParameter[] { }
                                                  );
                result = Convert.ToInt32(res);
            }
            catch { result = 0; }
            return result;
        }

        /// <summary>Получить запись журнала вагонов по идентификатору вагона</summary>
        /// <param name="wagon_id">Идентификаторов записи</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Запись журнала вагонов, null - нет вагона с таким серийным номером</returns>
        public WagonData GetWagonAndCargoData(int wagon_id)
        {
            if (wagon_id < 1) throw new ArgumentOutOfRangeException("id");
            WagonData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                int locoCount = GetTrainLocoCount(_DataHelper, wagon_id);

                string orderInv =  "[numbers].[Sn]" + (locoCount == 0 ? string.Empty : " + " + locoCount.ToString());
                string orderModel = (locoCount == 0 ? string.Empty : string.Format(" - {0} ", locoCount)) + "= [wagons_models].[wagon_sn]";
                //Если инверсия натурного листа
                //Проверить инвертированные натурные листы
                if (InvertNL(wagon_id))
                {
                    orderInv = @"(SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = " + 
								"(SELECT [v_wagons_data].[TrainId] FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[WagonId] = @wagid)) " + 
								" - [numbers].[Sn] + 1 "/* + (locoCount == 0 ? string.Empty : " + " + locoCount.ToString())*/;

                    orderModel = "  = (SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] IN( select [v_wagons_data].TrainId from [dbo].[v_wagons_data] WHERE [v_wagons_data].[WagonId] = @wagid )) - [wagons_models].[wagon_sn] + 1 ";
                }
                //Если нет инверсии
                else
                {
                    //Определение числа локомотивов
                    //TrainData train = GetTrainData(int train_id, true);
                    //orderInv = "[numbers].[Sn]";
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = WagonFieldsCargo,
                    From = "[dbo].[v_wagons_data]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" + "[v_wagons_data].[TrainId] = [numbers].[TrainId] AND [v_wagons_data].[WagonSn] = " + orderInv + ") \r\n" +
                           " LEFT OUTER JOIN [dbo].[wagons_ngb] ON (" + "[v_wagons_data].[WagonId] = [wagons_ngb].[wagon_id]) "+
                           " LEFT OUTER JOIN [dbo].[wagons_models] ON (" + "[v_wagons_data].[TrainId] = [wagons_models].[train_id] AND [v_wagons_data].[WagonSn] " + orderModel +" ) \r\n",
                    Where = "[v_wagons_data].[WagonId] = @wagid"
                };
                //Параметр
                SqlParameter sn_param = new SqlParameter("wagid", wagon_id);
               
                //Запрос
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, sn_param);
                IEnumerable<WagonData> WagonDataQuery = WagonAndCargoQuerySelect(dataTable);
                List<WagonData> WagonData = WagonDataQuery.ToList();
                if (WagonData.Count > 0)
                {
                    result = WagonData[0];
                    SetSnShow(ref result, locoCount);
                }
                _DataHelper.Commit();
            }
            return result;
        }

        ////Старая версия нужно проверить. Она не правильная, но работает
        ///// <summary>Получить запись журнала вагонов идентификатору состава и номеру пересечения</summary>
        ///// <param name="train_id">Идентификатор состава</param>
        ///// <param name="wagon_sn">Номер пересчения в составе</param>
        ///// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        ///// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        ///// <returns>Запись журнала вагонов, null - нет вагона с таким серийным номером</returns>
        //public WagonData GetWagonAndCargoData(int train_id, int wagon_sn)
        //{
        //    if (wagon_sn < 1) throw new ArgumentOutOfRangeException("sn");
        //    WagonData result = null;
        //    using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
        //    {
        //        _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        //        //string orderInv = "[numbers].[Sn]";
        //        //int locoCount = GetTrainLocoCount(_DataHelper, train_id, wagon_sn);
        //        int locoCount = 0;

        //        string orderInv = "[numbers].[Sn]" + (locoCount == 0 ? string.Empty : " + " + locoCount.ToString());

        //        if (GetTrainData(train_id, false).InvertInvNum)
        //        {
        //            orderInv = "(SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = @trainid) - [numbers].[Sn] + 1 "  /*+(locoCount == 0 ? string.Empty : " - " + locoCount.ToString())*/; 
        //        }
        //        string sql = (string)new SqlSelectBuilder
        //        {
        //            Fields = WagonFieldsCargo,
        //            From = "[dbo].[v_wagons_data]" +
        //                   " LEFT OUTER JOIN [dbo].[numbers] ON (" + "[v_wagons_data].[TrainId] = [numbers].[TrainId] AND [v_wagons_data].[WagonSn] = " + orderInv + ") \r\n" +
        //                   " LEFT OUTER JOIN [dbo].[wagons_ngb] ON (" + "[v_wagons_data].[WagonId] = [wagons_ngb].[wagon_id]) " + 
        //                   " LEFT OUTER JOIN [dbo].[wagons_models] ON (" + "[v_wagons_data].[TrainId] = [wagons_models].[train_id] AND [v_wagons_data].[WagonSn]" + (locoCount == 0 ? string.Empty : string.Format(" - {0} ", locoCount)) + " = [wagons_models].[wagon_sn]) \r\n",
        //            Where = "[v_wagons_data].[TrainId] = @trainid AND [v_wagons_data].[WagonSn] = @sn"
        //        };
        //        //Параметры
        //        List<DbParameter> _params = new List<DbParameter>();
        //        _params.Add(new SqlParameter("trainid", train_id));
        //        _params.Add(new SqlParameter("sn", wagon_sn));
        //        //Запрос
        //        DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
        //        IEnumerable<WagonData> WagonDataQuery = WagonAndCargoQuerySelect(dataTable);
        //        List<WagonData> WagonData = WagonDataQuery.ToList();
        //        if (WagonData.Count > 0) result = WagonData[0];
        //        _DataHelper.Commit();
        //    }
        //    return result;
        //}


        //Новая версия нужно проверить. Она должна работать правильно по сути, но нуждается в проверке
        /// <summary>Получить запись журнала вагонов идентификатору состава и номеру пересечения</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <param name="wagon_sn">Номер пересчения в составе</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Запись журнала вагонов, null - нет вагона с таким серийным номером</returns>
        public WagonData GetWagonAndCargoData(int train_id, int wagon_sn)
        {
            if (wagon_sn < 1) throw new ArgumentOutOfRangeException("sn");
            WagonData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                //string orderInv = "[numbers].[Sn]";
                int locoCount = GetTrainLocoCount(_DataHelper, train_id, wagon_sn);
                //int locoCount = 0;

                string orderInv = "[numbers].[Sn]" + (locoCount == 0 ? string.Empty : " + " + locoCount.ToString());
                string orderModel = (locoCount == 0 ? string.Empty : string.Format(" - {0} ", locoCount)) + "= [wagons_models].[wagon_sn]";
                bool invertNL = GetTrainData(train_id, false).InvertInvNum;
                if (invertNL)
                {
                    orderInv = "(SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = @trainid) - [numbers].[Sn] + 1 "  /*+(locoCount == 0 ? string.Empty : " - " + locoCount.ToString())*/;
                    orderModel = "  = (SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = @trainid) - [wagons_models].[wagon_sn] + 1 ";
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = WagonFieldsCargo,
                    From = "[dbo].[v_wagons_data]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" + "[v_wagons_data].[TrainId] = [numbers].[TrainId] AND [v_wagons_data].[WagonSn] = " + orderInv + ") \r\n" +
                           " LEFT OUTER JOIN [dbo].[wagons_ngb] ON (" + "[v_wagons_data].[WagonId] = [wagons_ngb].[wagon_id]) " +
                           " LEFT OUTER JOIN [dbo].[wagons_models] ON (" + "[v_wagons_data].[TrainId] = [wagons_models].[train_id] AND [v_wagons_data].[WagonSn]" + orderModel + ") \r\n",
                    Where = "[v_wagons_data].[TrainId] = @trainid AND [v_wagons_data].[WagonSn] = @sn"
                };
                //Параметры
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("trainid", train_id));
                _params.Add(new SqlParameter("sn", wagon_sn));
                //Запрос
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<WagonData> WagonDataQuery = WagonAndCargoQuerySelect(dataTable);
                List<WagonData> WagonData = WagonDataQuery.ToList();
                if (WagonData.Count > 0)
                {
                    result = WagonData[0];
                    SetSnShow(ref result, locoCount);
                }
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Запрос инвернатрного номер для вагона (с учетом локомотивов)</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <param name="wagon_sn">Порядковый номер вагона в составе с учетом локомотива</param>
        /// <returns>Инвернарного номер в натурном листе</returns>
        public string GetWagonNL(int train_id, int wagon_sn, int loco_cnt)
        {
            if (wagon_sn < 1) throw new ArgumentOutOfRangeException("sn");
            string number = "";
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string orderInv = "[numbers].[Sn] + " + loco_cnt.ToString();
                string orderModel = (loco_cnt == 0 ? string.Empty : string.Format(" - {0} ", loco_cnt)) + "= [wagons_models].[wagon_sn]";

                if (GetTrainData(train_id, false).InvertInvNum)
                {
                    orderInv = "(SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = @trainid) - [numbers].[Sn] + 1 +" + loco_cnt.ToString()+ " ";
                    orderModel = "  = (SELECT COUNT(*) FROM [dbo].[v_wagons_data] WHERE [v_wagons_data].[TrainId] = @train_id) - [wagons_models].[wagon_sn] + 1 ";
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = WagonFieldsCargo,
                    From = "[dbo].[v_wagons_data]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" + "[v_wagons_data].[TrainId] = [numbers].[TrainId] AND [v_wagons_data].[WagonSn] = " + orderInv + ") \r\n" +
                           " LEFT OUTER JOIN [dbo].[wagons_ngb] ON (" + "[v_wagons_data].[WagonId] = [wagons_ngb].[wagon_id]) " +
                           " LEFT OUTER JOIN [dbo].[wagons_models] ON (" + "[v_wagons_data].[TrainId] = [wagons_models].[train_id] AND [v_wagons_data].[WagonSn] "+ orderModel +") \r\n",
                    Where = "[v_wagons_data].[TrainId] = @trainid AND [v_wagons_data].[WagonSn] = @sn"
                };
                //Параметры
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("trainid", train_id));
                _params.Add(new SqlParameter("sn", wagon_sn));
                //Запрос
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<WagonData> WagonDataQuery = WagonAndCargoQuerySelect(dataTable);
                List<WagonData> WagonData = WagonDataQuery.ToList();
                if (WagonData.Count > 0) number = WagonData[0].InvNumByNL;
                _DataHelper.Commit();
            }
            return number;
        }

        /// <summary>Вставка или обновление информации о грузе вагона</summary>
        /// <param name="wagon_id">Идентификатор вагона</param>
        /// <param name="cr">Данные о грузе</param>
        /// <param name="result">Результат операции</param>
        /// <returns>Результат операции</returns>
        public bool InsertOrUpdateCargo(int wagon_id, CargoResultData data, WagonData wagon)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            /*ALTER PROCEDURE [dbo].[add_update_cargo]
	        @WagId int, @shift_x int, @shift_y int, @shift_z int, @volume int,
            @width int, @height int, @length int, @width_max int, @height_max int,
            @cargo_exist bit, @cargo_clear bit, @cargo_type int, @wagon_type int,
            @wagon_model nvarchar(max), @floor_level int
             */
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("WagId", wagon_id));
                _params.Add(new SqlParameter("shift_x", data.ShiftX));
                _params.Add(new SqlParameter("shift_y", data.ShiftY));
                _params.Add(new SqlParameter("shift_z", data.ShiftZ));
                _params.Add(new SqlParameter("volume", data.CargoVolume));
                _params.Add(new SqlParameter("width", data.CargoWidth));
                _params.Add(new SqlParameter("height", data.CargoHeight));
                _params.Add(new SqlParameter("length", data.CargoLength));
                _params.Add(new SqlParameter("width_max", data.CargoWidthMax));
                _params.Add(new SqlParameter("height_max", data.CargoHeightMax));
                _params.Add(new SqlParameter("cargo_exist", data.CargoExist));
                _params.Add(new SqlParameter("cargo_clear", data.CargoClear));
                _params.Add(new SqlParameter("cargo_type", wagon.CargoType));

                _params.Add(new SqlParameter("wagon_type", wagon.WagonType));
                //Обработать null
                if (string.IsNullOrEmpty(wagon.WagonModel))
                    _params.Add(new SqlParameter("wagon_model", DBNull.Value));
                else
                    _params.Add(new SqlParameter("wagon_model", wagon.WagonModel));
                _params.Add(new SqlParameter("floor_level", wagon.FloorLevel));

                //Выполнение хранимой процедуры
                _DataHelper.ExecuteStoredProcedure("[dbo].[add_update_cargo]", _params);
                _DataHelper.Commit();
            }
            return true;
        }
        
        /// <summary>Вставка или обновление информации о грузе вагона</summary>
        /// <param name="wagon_id">Идентификатор вагона</param>
        /// <param name="cr">Данные о грузе</param>
        /// <param name="result">Результат операции</param>
        /// <returns>Результат операции</returns>
        public bool InsertOrUpdateCargoCompareResult(int wagon_id, CargoRemoteCompareResultData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            /*ALTER PROCEDURE [dbo].[add_update_cargo]
	        @WagId int, @shift_x int, @shift_y int, @shift_z int, @volume int,
            @width int, @height int, @length int, @width_max int, @height_max int,
            @cargo_exist bit, @cargo_clear bit, @cargo_type int, @wagon_type int,
            @wagon_model nvarchar(max), @floor_level int
             */
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();


                _params.Add(new SqlParameter("WagonID", wagon_id));
                _params.Add(new SqlParameter("ServerID", data.RemoteServerId));
                _params.Add(new SqlParameter("RemotTrainID", data.RemoteTrainId));
                _params.Add(new SqlParameter("RemoteTrainNumber", data.RemoteTrainNumber));
                _params.Add(new SqlParameter("RemoteTrainIndex", data.RemoteTrainIndex));
                _params.Add(new SqlParameter("RemoteWagonSn", data.RemoteWagonSn));
                _params.Add(new SqlParameter("RemoteWagonID", data.RemoteWagonId));
                _params.Add(new SqlParameter("RemoteWagonDateBegin", data.RemoteWagonDate));

                _params.Add(new SqlParameter("diff_shift_x", data.DiffShiftX));
                _params.Add(new SqlParameter("diff_shift_y", data.DiffShiftY));
                _params.Add(new SqlParameter("diff_shift_z", data.DiffShiftZ));

                _params.Add(new SqlParameter("diff_volume", data.DiffVolume));
                _params.Add(new SqlParameter("diff_width", data.DiffWidth));
                _params.Add(new SqlParameter("diff_height", data.DiffHeght));
                _params.Add(new SqlParameter("diff_length", data.DiffLength));
                _params.Add(new SqlParameter("diff_width_max", data.DiffWidthMax));
                _params.Add(new SqlParameter("diff_height_max", data.DiffHeightMax));
                _params.Add(new SqlParameter("diff_cargo_exist", Convert.ToInt32(data.DiffCargoExists)));
                _params.Add(new SqlParameter("diff_cargo_clear", Convert.ToInt32(data.DiffCargoClear)));
                _params.Add(new SqlParameter("IsRelationModel", Convert.ToInt32(data.IsRelationModel)));
                _params.Add(new SqlParameter("diff_speed", data.DiffSpeed));
                _params.Add(new SqlParameter("Valid", data.RemoteValid));
                _params.Add(new SqlParameter("CompareDate", data.CompareDate));
                if(data.OperatorID > 0)
                    _params.Add(new SqlParameter("OperatorID", data.OperatorID));
                else
                    _params.Add(new SqlParameter("OperatorID", DBNull.Value));
                //Выполнение хранимой процедуры
                _DataHelper.ExecuteStoredProcedure("[dbo].[add_or_update_wagons_compare]", _params);
                _DataHelper.Commit();
            }
            return true;
        }
        
        /// <summary>Получеение данных о грузе в вагоне  </summary>
        /// <param name="wagonID">Идентификатор вагона</param>
        /// <returns></returns>
        public CargoResultData GetCargoResultByWagonID(int wagonID) 
        {
            CargoResultData cr = new CargoResultData();
            try 
            {
                string query = @"SELECT TOP 1 * FROM dbo.wagons_cargo WHERE WagId = @WagId";
                using (SqlConnection conn = new SqlConnection(_ConnectionString)) 
                {
                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(query, conn)) 
                    {
                        comm.Parameters.AddWithValue("WagID", wagonID);
                        using (SqlDataReader reader = comm.ExecuteReader()) 
                            while (reader.Read()) cr = GetItemFromReader<CargoResultData>(reader);
                    }
                }
            }
            catch { }
            return cr;
        }

        /// <summary>Удаление вагона и записи в таблице грузов. Если не прописано каскадное удалени в БД</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <param name="wagon_sn">Порядковый номер вагона</param>
        /// <returns></returns>
        public bool DeleteWagonAndCargo(int train_id, int wagon_id)
        {
            bool result = false;
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {   
                if (context == null) throw new ArgumentException("context");

                  //Открыть транзакцию, так как пакетное изменение
                context.BeginTransaction();
                
                List<DbParameter> cargoParams = new List<DbParameter>(1);
                cargoParams.Add(new SqlParameter("WagonId",wagon_id));
                string cargoSql = (string)new SqlDeleteBuilder
                {
                    Table = "[dbo].[cargo_result]",
                    Where = "[WagId] = @WagonId"
                };
                result = context.ExecuteNoneQuery(cargoSql,cargoParams)>-1;//Так как вагон без груза вполне может быть
                List<DbParameter> _params = new List<DbParameter>(2);
                string sql = (string)new SqlDeleteBuilder
                {
                    Table = "[dbo].[wagons]",
                    Where = "[TrainId] = @TrainId AND [WagonId] = @WagonId"
                };
                DateTime changed = DateTime.Now;
                _params.Add(new SqlParameter("TrainId", train_id));//Идентификатор вагона в базе
                _params.Add(new SqlParameter("WagonId", wagon_id));
                result = result && context.ExecuteNoneQuery(sql, _params) > 0;
                context.Commit();
            }
            return result;
        }

        /// <summary>Изменить запись в таблице вагонов (wagons) и грузов (cargo_result)</summary>
        /// <param name="data">Данные взвешивания вагона</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта или не задан контекст транзакции</exception>
        /// <exception cref="System.ObjectDisposedException">Транзакция завершена и не может быть использована</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public bool ModifyWagonAndCargo(WagonData data)
        {
            bool result = false;
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                if (context == null) throw new ArgumentException("context");
                CheckWagon(data);

                context.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);//Он и так по умолчанию. Просто для порядка.

                #region Update Wagon

                //Обновление вагона
                List<DbParameter> w_params = new List<DbParameter>(0);
                string sql = (string)new SqlUpdateBuilder
                {
                    Fields = new string[] { "[WagonSn]", "[SnSost]", "[Loco]", "[InvNum]", 
                                            "[TimeSpanBegin]", "[TimeSpanEnd]", 
                                            "[TimeSpanBeginBS]", "[TimeSpanEndBS]", 
                                            "[SpeedBegin]", "[SpeedEnd]", 
                                            "[DirectionBegin]", "[DirectionEnd]",
                                            "[Ngb]", "[Comment]", "[Mark]", "[TimeChanged]" },
                    Values = new string[] { "@Sn", "@SnSost", "@Loco", "@InvNum", 
                                            "@TimeSpanBegin", "@TimeSpanEnd", 
                                            "@TimeSpanBeginBS", "@TimeSpanEndBS", 
                                            "@SpeedBegin", "@SpeedEnd",
                                            "@DirectionBegin", "@DirectionEnd",
                                            "@Ngb", "@Comment", "@Mark", "@TimeChanged" },
                    Table = "[dbo].[wagons]",
                    Where = "[WagonId] = @WagId"
                };
                DateTime changed = DateTime.Now;
                w_params.Add(new SqlParameter("WagId", data.WagId));                     //Идентификатор вагона в базе
                w_params.Add(new SqlParameter("Sn", data.Sn));                           //Номер пересечения
                w_params.Add(new SqlParameter("SnSost", data.SnSost));                   //Номер в составе
                w_params.Add(new SqlParameter("Loco", data.Loco));                       //Тип вагона
                w_params.Add(new SqlParameter("InvNum", data.InvNumber));                //Инвентарный номер
                w_params.Add(new SqlParameter("TimeSpanBegin", data.TimeSpanBegin));     //Метка времени начало вагона
                w_params.Add(new SqlParameter("TimeSpanEnd", data.TimeSpanEnd));         //Метка времени конца вагона
                w_params.Add(new SqlParameter("TimeSpanBeginBS", data.TimeSpanBeginBS)); //Метка времени начало вагона
                w_params.Add(new SqlParameter("TimeSpanEndBS", data.TimeSpanEndBS));     //Метка времени конца вагона
                w_params.Add(new SqlParameter("SpeedBegin", data.SpeedBegin));           //Скорость в начале
                w_params.Add(new SqlParameter("SpeedEnd", data.SpeedEnd));               //Скорость в конце
                w_params.Add(new SqlParameter("DirectionBegin", data.DirectionBegin));   //Направление в начале
                w_params.Add(new SqlParameter("DirectionEnd", data.DirectionEnd));       //Направление в конце
                w_params.Add(new SqlParameter("Ngb", data.Ngb));                         //Маска негабарито
                w_params.Add(new SqlParameter("Mark", data.Mark));                       //Признак маркирования вагона
                w_params.Add(new SqlParameter("Comment", data.Comment));                 //Комментарий
                w_params.Add(new SqlParameter("TimeChanged", changed));                  //Время изменения
                //Выполнение команды
                 result = context.ExecuteNoneQuery(sql, w_params)>0;
                #endregion

                #region Update Cargo
                //Обновление данных о грузе
                List<DbParameter> c_params = new List<DbParameter>();
                c_params.Add(new SqlParameter("WagId", data.WagId));
                c_params.Add(new SqlParameter("shift_x", data.ShiftX));
                c_params.Add(new SqlParameter("shift_y", data.ShiftY));
                c_params.Add(new SqlParameter("shift_z", data.ShiftZ));
                c_params.Add(new SqlParameter("volume", data.CargoVolume));
                c_params.Add(new SqlParameter("width", data.CargoWidth));
                c_params.Add(new SqlParameter("height", data.CargoHeight));
                c_params.Add(new SqlParameter("length", data.CargoLength));
                c_params.Add(new SqlParameter("width_max", data.CargoWidthMax));
                c_params.Add(new SqlParameter("height_max", data.CargoHeightMax));
                c_params.Add(new SqlParameter("cargo_exist", SqlDbType.Bit){Value = data.CargoWidth == 0 ? DBNull.Value : (object)data.CargoExist});
                c_params.Add(new SqlParameter("cargo_clear", SqlDbType.Bit){Value = data.CargoWidth == 0 ? DBNull.Value : (object)data.CargoClear});
                c_params.Add(new SqlParameter("cargo_type", data.CargoType));
                c_params.Add(new SqlParameter("wagon_type", data.WagonType));
                //Обработать null
                if (string.IsNullOrEmpty(data.WagonModel))
                    c_params.Add(new SqlParameter("wagon_model", DBNull.Value));
                else
                    c_params.Add(new SqlParameter("wagon_model", data.WagonModel));
                c_params.Add(new SqlParameter("floor_level", data.FloorLevel));
                //Выполнение хранимой процедуры
                //context.ExecuteStoredProcedure("[dbo].[add_update_cargo]", c_params);
                int res = context.ExecuteNoneQueryStoredProcedure("[dbo].[add_update_cargo]", c_params);
                #endregion

                //Завершить транзакцию
                context.Commit();
            }
            //Опционально обновить UserCheck в отдельной транзакции. На случай, если нет интеграции с АСКИН.
            UpdateUserCheck(data.WagId, data.Ocr_UserCheck);
            return result;
        }

        public bool UpdateUserCheck(int wagonID, int userCheck)
        {
            try
            {
                using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
                {
                    try
                    {
                        string updateOcrUserCheck = "UPDATE wagons_ocr SET [check] = @check WHERE wagon_id = @wagID";
                        List<DbParameter> ocrParams = new List<DbParameter>
                        {
                            new SqlParameter("check", userCheck),
                            new SqlParameter("wagID", wagonID)
                        };
                        context.ExecuteNoneQuery(updateOcrUserCheck, ocrParams);
                        //Завершить транзакцию
                        context.Commit();

                    }
                    catch { return false; }
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>Изменить значение свойства вагона</summary>
        /// <param name="WagId">Идентификатор вагона</param>
        /// <param name="Property">Свойство</param>
        /// <param name="Value">Значение</param>
        public bool ModifyWagonData(int WagId, string Property, object Value)
        {
            if (Value == null) throw new ArgumentNullException("data");
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                context.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                int rows = 0;
                string table = "";  //Таблица
                string field = "";  //Поле параметра
                string wag_id = ""; //Поле идентификатора вагона
                if (Property == "InvNumber") { table = "wagons"; field = "InvNum"; wag_id = "WagonId"; }
                if (Property == "Comment") { table = "wagons"; field = "Comment"; wag_id = "WagonId"; }
                if (Property == "WagonType") { table = "wagons_cargo"; field = "wagon_type"; wag_id = "WagId"; }
                if (Property == "WagonModel") { table = "wagons_cargo"; field = "wagon_model"; wag_id = "WagId"; }
                if (Property == "CargoExist") { table = "wagons_cargo"; field = "cargo_exist"; wag_id = "WagId"; }
                if (Property == "CargoType") { table = "wagons_cargo"; field = "cargo_type"; wag_id = "WagId"; }
                if (Property == "CargoClear") { table = "wagons_cargo"; field = "cargo_clear"; wag_id = "WagId"; }
                if (Property == "FloorLevel") { table = "wagons_cargo"; field = "floor_level"; wag_id = "WagId"; }
                SqlParameter param = new SqlParameter("val", Value);
                //Обновление таблицы вагоны
                if (table == "wagons")
                {
                    string sql = (string)new SqlUpdateBuilder
                    {
                        Table = "[dbo].[" + table + "]",
                        Fields = new string[] { "[" + field + "]" },
                        Values = new string[] { "@val" },
                        Where = "[" + wag_id + "] = " + WagId.ToString()
                    };
                    rows = context.ExecuteNoneQuery(sql, param);
                }
                //Обновление таблицы грузы
                if (table == "wagons_cargo")
                {
                    //Проверка наличия записи о вагоне
                    string sql = (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[WagId]" },
                        From = "[dbo].[wagons_cargo]",
                        Where = "[WagId] = " + WagId.ToString()
                    };
                    DataTable dataTable = context.ExecuteCommand(sql);
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        //Обновление записи
                        sql = (string)new SqlUpdateBuilder
                        {
                            Table = "[dbo].[" + table + "]",
                            Fields = new string[] { "[" + field + "]" },
                            Values = new string[] { "@val" },
                            Where = "[" + wag_id + "] = " + WagId.ToString()
                        };
                        rows = context.ExecuteNoneQuery(sql, param);
                    }
                    else
                    {
                        //Добавление записи
                        sql = (string)new SqlInsertBuilder
                        {
                            Table = "[dbo].[" + table + "]",
                            Fields = new string[] { "WagId", "[" + field + "]" },
                            Values = new string[] { WagId.ToString(), "@val" },
                        };
                        rows = context.ExecuteNoneQuery(sql, param);
                    }

                }
                context.Commit();
                if (rows == 1) return true;
            }
            return false;
        }

        string[] AddToArray(string[] array, string value)
        {
            List<string> result = new List<string>(array.Length+1);
            result.AddRange(array);
            return result.ToArray();
        }
        
        /// <summary>Обновить данные группового редактирования типа вагона и типа груза. </summary>
        /// <param name="wagon">Объект с заполненными полями, тип вагона, тип груза, уровень пола и модель вагона</param>
        /// <returns></returns>
        public bool UpdateCargoMainDataGroup(WagonData wagon)
        {
            bool result = true;
            SqlUpdateBuilder builder = new SqlUpdateBuilder
            {
                Table = "[dbo].[wagons_cargo]",
                Fields = new string[] { "wagon_type", "@cargo_type" },
                Values = new string[] { "@wagon_type", "@cargo_type" },
                Where = "WagId IN(select w.WagonId from wagons w where w.TrainId=@TrainId)"
            };
             List<DbParameter> parametrs = new List<DbParameter>();
            parametrs.Add(new SqlParameter("@wagon_type",wagon.WagonType));
            parametrs.Add(new SqlParameter("@cargo_type",wagon.CargoType));
            if (!string.IsNullOrEmpty(wagon.WagonModel))
            {
                builder.Fields = AddToArray(builder.Fields, "wagon_model");
                builder.Values = AddToArray(builder.Fields, "@wagonModel");
                parametrs.Add(new SqlParameter("@wagonModel", wagon.WagonModel));
            }
            if(wagon.FloorLevel>0){
                 builder.Fields = AddToArray(builder.Fields, "floor_level");
                builder.Values = AddToArray(builder.Fields, "@floor_level");
                parametrs.Add(new SqlParameter("@floor_level", wagon.FloorLevel));
            }
            //Сохраннение
            using (SqlDataHelper context = new SqlDataHelper(DataBaseUtils.ConnectionString))
            {
                string query = builder.ToString();
                context.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                result = context.ExecuteNoneQuery(query, parametrs)>0;
                context.Commit();
            }
            return result;
       }

        #endregion

        #region Расширенные негабариты (объединить с запросами вагонов)

        /// <summary>Сохранить расширенные негабариты версии 1</summary>
        /// <param name="wagon_id">Идентификтаор вагона</param>
        /// <param name="wagon">Данные вагона</param>
        /// <returns>Результат операции</returns>
        public bool AddNgbData(int wagon_id, WagonData wagon)
        {
            if (wagon == null) throw new ArgumentNullException("data");

            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("WagId",       wagon_id));
                _params.Add(new SqlParameter("ngb_zonal",   wagon.NgbZonal));
                _params.Add(new SqlParameter("ngb_main",    wagon.NgbBase));
                _params.Add(new SqlParameter("ngb_soft",    wagon.NgbSoft));
                _params.Add(new SqlParameter("ngb_1t",      wagon.NgbStatic_1T));
                _params.Add(new SqlParameter("ngb_tpr",     wagon.NgbStatic_Tpr));
                _params.Add(new SqlParameter("ngb_build",   wagon.NgbBuild));
                _params.Add(new SqlParameter("ngb_og1",     wagon.NgbGrade1));
                _params.Add(new SqlParameter("ngb_og2",     wagon.NgbGrade2));
                _params.Add(new SqlParameter("ngb_og3",     wagon.NgbGrade3));
                _params.Add(new SqlParameter("ngb_og4",     wagon.NgbGrade4));
                _params.Add(new SqlParameter("ngb_og5",     wagon.NgbGrade5));
                _params.Add(new SqlParameter("ngb_og6",     wagon.NgbGrade6));
                //Выполнение хранимой процедуры
                _DataHelper.ExecuteStoredProcedure("[dbo].[add_update_ngb]", _params);
                _DataHelper.Commit();
            }
            return true;
        }

        /// <summary>Сохранить расширенные негабариты версии 2</summary>
        /// <param name="wagon_id">Идентификтаор вагона</param>
        /// <param name="wagon">Данные вагона</param>
        /// <returns>Результат операции</returns>
        public bool AddNgbDataEx(int wagon_id, WagonData wagon)
        {
            if (wagon == null) throw new ArgumentNullException("data");

            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("WagId", wagon_id));
                _params.Add(new SqlParameter("ngb_zonal", wagon.NgbZonal));
                _params.Add(new SqlParameter("ngb_main", wagon.NgbBase));
                _params.Add(new SqlParameter("ngb_soft", wagon.NgbSoft));
                _params.Add(new SqlParameter("ngb_grade1", wagon.NgbGrade1));
                _params.Add(new SqlParameter("ngb_grade2", wagon.NgbGrade2));
                _params.Add(new SqlParameter("ngb_grade3", wagon.NgbGrade3));
                _params.Add(new SqlParameter("ngb_grade4", wagon.NgbGrade4));
                _params.Add(new SqlParameter("ngb_grade5", wagon.NgbGrade5));
                _params.Add(new SqlParameter("ngb_grade6", wagon.NgbGrade6));
                _params.Add(new SqlParameter("ngb_gradeEx", wagon.NgbGrade6));
                _params.Add(new SqlParameter("ngb_static_t", wagon.NgbStatic_T));
                _params.Add(new SqlParameter("ngb_static_tpr", wagon.NgbStatic_Tpr));
                _params.Add(new SqlParameter("ngb_static_1t", wagon.NgbStatic_1T));
                _params.Add(new SqlParameter("ngb_static_tc", wagon.NgbStatic_Tc));
                _params.Add(new SqlParameter("ngb_static_1vm", wagon.NgbStatic_1VM));
                _params.Add(new SqlParameter("ngb_static_0vm", wagon.NgbStatic_0VM));
                _params.Add(new SqlParameter("ngb_static_02vm", wagon.NgbStatic_02VM));
                _params.Add(new SqlParameter("ngb_static_03vm", wagon.NgbStatic_03VM));
                _params.Add(new SqlParameter("ngb_build", wagon.NgbBuild));
                //Выполнение хранимой процедуры
                _DataHelper.ExecuteStoredProcedure("[dbo].[add_update_ngb]", _params);
                _DataHelper.Commit();
            }
            return true;
        }
        
        #endregion

        #region Recognition

        public bool InsertWagonRecognitionData(int sn, string inv_num, int number_type, int accuracy, int user_check, int wagonAskoID, int ocrTrainID )
        {
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                if (context == null) throw new ArgumentException("context");
                List<DbParameter> _params = new List<DbParameter>();
                //string sql = (string)new SqlInsertBuilder
                //{
                
                //    Fields = new string[] { 
                //                             "[ocr_train]",
                //                             "[wagon_id]",
                //                             "[sn]",
                //                             "[inv_num]",
                //                             "[type]",
                //                             "[accuracy]",
                //                             "[check]"

                //    },
                //    Values = new string[] { 
                //                            "@ocr_train",
                //                            "@wagon_id",
                //                            "@sn",
                //                            "@inv_num",
                //                            "@type",
                //                            "@accuracy",
                //                            "@check" 
                //    },
                //    Table = "[dbo].[wagons_ocr]"
                //};
                string sql = "add_update_ocr";
                DateTime changed = DateTime.Now;
                _params.Add(new SqlParameter("ocr_train", ocrTrainID));//Идентификатор вагона в базе
                _params.Add(new SqlParameter("wagon_id", wagonAskoID));
                _params.Add(new SqlParameter("sn", sn));
                _params.Add(new SqlParameter("inv_num", inv_num));
                _params.Add(new SqlParameter("type", number_type));
                _params.Add(new SqlParameter("accuracy", accuracy));
                _params.Add(new SqlParameter("check", user_check));

                //int result = context.ExecuteNoneQuery(sql, _params);
                int result = context.ExecuteNoneQueryStoredProcedure(sql, _params);
                if (result != 1) return false;
                else return true;
            }
        }

        #endregion

        #region Common
        
        /// <summary>Получение текущего идентификатора таблицы (последней вставленной строки)</summary>
        /// <param name="table">Имя таблицы</param>
        /// <returns>Максимальное значение</returns>
        public int GetCurrentIdent(string table)
        {
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = "SELECT IDENT_CURRENT('" + table + "')";
                DataTable t = _DataHelper.ExecuteCommand(sql);
                _DataHelper.Commit();
                if (t.Rows.Count == 0 || t.Columns.Count == 0) return 0;
                return System.Convert.ToInt32(t.Rows[0][0]);
            }
        }

        #endregion

        #endregion

        #region DataBase

        /// <summary>Проверяет подключение к базе данных СБВ УВГ</summary>
        /// <returns>true, если подключение возможно, false в противном случае</returns>
        public bool CheckConnect()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>Проверяет совместивость провайдера данных с базой данных СБВ УВГ</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>true, если база данных совместима с текущей версией провайдера, false в противном случае</returns>
        public bool CheckDBversion()
        {
            string version = GetConfigParam(DataProvider.DbVersionParamName);
            if (!string.IsNullOrEmpty(version))
            {
                return Int32.Parse(version) == DataProvider.DBversion;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Получить номер версии из базы</summary>
        /// <returns>Версия базы данных, 0 - не определена</returns>
        public int GetDBversion()
        {
            string version = "";
            try { version = GetConfigParam(DataProvider.DbVersionParamName); } catch { return 0; };
            if (!string.IsNullOrEmpty(version))
            {
                try { return Int32.Parse(version); }
                catch { return 0; };
            }
            else return 0;
        }

        /// <summary>Получить номер старой версии АСКО 2010</summary>
        public string GetDBversionOld()
        {
            string version = "";
            try { version = GetConfigParam("System.Version"); } catch { return ""; };
            if (!string.IsNullOrEmpty(version)) return version;
            else return "";
        }
        
        #endregion

        #region Reflection

        protected T GetItemFromReader<T>(SqlDataReader reader) where T : new() 
        {
            T obj = new T();
            Type tType = typeof(T);
            foreach (PropertyInfo property in tType.GetProperties()) 
            {
                try 
                {
                    var value = reader[property.Name];
                    property.SetValue(obj, Convert.ChangeType(value, property.PropertyType), null);
                }
                catch { }
            }
            return obj;
        }

        #endregion

    }
}
