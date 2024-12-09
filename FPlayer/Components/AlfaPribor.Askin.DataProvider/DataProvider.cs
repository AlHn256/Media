using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Linq;

using AlfaPribor.ASKIN.Data;
using AlfaPribor.DataHelper;

using AlfaPribor.ASKIN.DataBase;

namespace AlfaPribor.ASKIN.DataProvider
{

    /// <summary>Реализация провайдера данных АСКИН В для MS SQL server</summary>
    public partial class DataProvider
    {

        const int db_version = 6;

        #region Fields

        /// <summary>Строка с параметрами подключения у базе данных</summary>
        protected string _ConnectionString;

        /// <summary>Имя конфигурационного параметра, содержащего номер текущей версии базы данных</summary>
        private static string DbVersionParamName = "Config.Revision";

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
            if (data.TrainId < 1 || data.WagId < 1 || data.TimeSpan < 0)
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
            for (var status = DirStat.Unknown; status <= DirStat.EventsArchive; ++status)
            {
                if ((int)status == dir_stat)
                {
                    return status;
                }
            }
            return DirStat.Unknown;
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

        /*
        string GetTrainsSelectString(string where_str)
        {
            string sql = (string)new SqlSelectBuilder
            {
                Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                "(SumTrainSpeed/WagonsCount) AS Speed"},
                From = "[dbo].[v_trains], (" +
                (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                    "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                    "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                    "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                    From = "[dbo].[wagons],[dbo].[v_trains]",
                    Where = where_str,
                } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                OrderBy = "[TimeBegin] DESC"
            };
            return sql;
        }
        */

        /// <summary>Проверка или создание представления для отоборажения скленных вагонов</summary>
        private bool CheckOrCreateTrainView()
        {
            long viewID = 0;
            //Проверка создания представления
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                string checkQueryText = "select object_id('wTrainRefTrain')";
                if (!long.TryParse(_DataHelper.ExecuteScalar(checkQueryText, new List<DbParameter>()).ToString(), out viewID))
                {
                    viewID = 0;
                }
            }
            //Создание представления если оно отсуствует
            int result = -1;
            if (viewID < 1)
            {
                using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
                {
                    string createViewQueryText = @"create view wTrainRefTrain
                                                as
                                                select
				                                                w.WagId,
				                                                w.Num,
				                                                w.TrainId,
				                                                w.Direction,
				                                                w.RefId,
				                                                wr.WagId as ReferedWagonId,
				                                                tr.TrainId as RealTrainId
		                                                from
			                                                wagons w
		                                                left outer join wagons wr ON w.RefId = wr.WagId
		                                                /*left outer*/ join trains tr on wr.TrainId = tr.TrainId";
                    result = _DataHelper.ExecuteNoneQuery(createViewQueryText, new List<DbParameter>());
                }
            }
            return result > -1 || viewID > 0;
        }

        /// <summary>Преобразовать данные таблицы выборки поездов</summary>
        /// <param name="dataTable">Таблица выборки поездов</param>
        /// <returns></returns>
        IEnumerable<TrainData> GetTrainDataQuery(DataTable dataTable)
        {
            IEnumerable<TrainData> TrainDataQuery =
                from v_trains in dataTable.AsEnumerable()
                let point = v_trains.IsNull("RecId") ? 0 : (int)v_trains["RecId"]
                let id = v_trains.IsNull("TrainId") ? 0 : (int)v_trains["TrainId"]
                let train_num = v_trains.IsNull("TrainNum") ? String.Empty : (string)v_trains["TrainNum"]
                let train_index = v_trains.IsNull("TrainIndex") ? String.Empty : (string)v_trains["TrainIndex"]
                let begin_time = v_trains.IsNull("TimeBegin") ? DateTime.MinValue : (DateTime)v_trains["TimeBegin"]
                let end_time = v_trains.IsNull("TimeEnd") ? DateTime.MinValue : (DateTime)v_trains["TimeEnd"]
                let direction = v_trains.IsNull("Direction") ? 0 : (int)v_trains["Direction"]
                let dir_id = v_trains.IsNull("DirId") ? 0 : (int)v_trains["DirId"]
                let dir_path = v_trains.IsNull("DirPath") ? String.Empty : (string)v_trains["DirPath"]
                let status = v_trains.IsNull("Status") ? 0 : (int)v_trains["Status"]
                let w_count = v_trains.IsNull("WagonsCount") ? 0 : (int)v_trains["WagonsCount"]
                let l_count = v_trains.IsNull("LocoCount") ? 0 : (int)v_trains["LocoCount"]
                let speed = v_trains.IsNull("Speed") ? 0 : (int)v_trains["Speed"]
                let virt = v_trains.IsNull("Virtual") ? false : (bool)v_trains["Virtual"]

                select new TrainData
                {
                    RecId = point,
                    Id = id,
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
                    Speed = speed,
                    Virtual = virt
                };

            return TrainDataQuery;
        }

        /// <summary>Преобразовать данные таблицы выборки поездов</summary>
        /// <param name="dataTable">Таблица выборки поездов</param>
        /// <returns></returns>
        IEnumerable<TrainData> WIGetTrainDataQuery(DataTable dataTable)
        {
            IEnumerable<TrainData> TrainDataQuery =
                from v_trains in dataTable.AsEnumerable()
                let point = v_trains.IsNull("RecId") ? 0 : (int)v_trains["RecId"]
                let id = v_trains.IsNull("TrainId") ? 0 : (int)v_trains["TrainId"]
                let train_num = v_trains.IsNull("TrainNum") ? String.Empty : (string)v_trains["TrainNum"]
                let train_index = v_trains.IsNull("TrainIndex") ? String.Empty : (string)v_trains["TrainIndex"]
                let begin_time = v_trains.IsNull("TimeBegin") ? DateTime.MinValue : (DateTime)v_trains["TimeBegin"]
                let end_time = v_trains.IsNull("TimeEnd") ? DateTime.MinValue : (DateTime)v_trains["TimeEnd"]
                let direction = v_trains.IsNull("Direction") ? 0 : (int)v_trains["Direction"]
                let dir_id = v_trains.IsNull("DirId") ? 0 : (int)v_trains["DirId"]
                let dir_path = v_trains.IsNull("DirPath") ? String.Empty : (string)v_trains["DirPath"]
                let status = v_trains.IsNull("Status") ? 0 : (int)v_trains["Status"]
                let w_count = v_trains.IsNull("WagonsCount") ? 0 : (int)v_trains["WagonsCount"]
                let l_count = v_trains.IsNull("LocoCount") ? 0 : (int)v_trains["LocoCount"]
                let speed = v_trains.IsNull("Speed") ? 0 : (int)v_trains["Speed"]
                let virt = v_trains.IsNull("Virtual") ? false : (bool)v_trains["Virtual"]
                let virtualTrainID = v_trains.IsNull("referedTrainIdList") ? string.Empty : (string)v_trains["referedTrainIdList"]

                select new TrainData
                {
                    RecId = point,
                    Id = id,
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
                    Speed = speed,
                    Virtual = virt,
                    VirtualTrainIDList = virtualTrainID
                };

            return TrainDataQuery;
        }


        /// <summary>Получить данные поезда/отцепа с указанным идентификатором</summary>
        /// <param name="train_id">Идентификатор поезда/отцепа</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Данные поезда, null - нет поезда с таким идентификатором</returns>
        public TrainData GetTrainData(int train_id)
        {
            if (train_id < 1)
            {
                throw new ArgumentOutOfRangeException("train_id");
            }
            TrainData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons]",
                        Where = "[TrainId] = @train_id",
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin] DESC"
                };
                SqlParameter train_id_param = new SqlParameter("train_id", train_id);
                //Выборка
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, train_id_param);
                //Получение данных из выборки
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);

                List<TrainData> TrainDataList = TrainDataQuery.ToList();
                if (TrainDataList.Count > 0) result = TrainDataList[0];
                _DataHelper.Commit();
            }
            return result;
        }


        /// <summary>Получить список поездов соответствующих критериям Веб интерфейса</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда</param>
        /// <param name="train_index">Индекс поезда</param>
        /// <param name="wag_inv">Инвентарный номер вагона по ТГНЛ, входящий в состав поеда</param>
        /// <param name="train_direct">Направление движения состава</param>
        /// <param name="train_status">Статус отправки</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> WIGetTrainList(DateTime begin, DateTime end, string train_num, string train_index,
                                             string wag_inv, int train_status, int train_direct, int trainID)
        {
            List<TrainData> result = new List<TrainData>();
            try
            {
                CheckOrCreateTrainView();
                using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
                {
                    _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    List<DbParameter> _params = new List<DbParameter>();
                    string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                    if (begin != DateTime.MinValue)
                    {
                        where_str = AND(where_str, "([TimeBegin] >= @begin)\n");
                        _params.Add(new SqlParameter("begin", begin));
                    }
                    if (end != DateTime.MinValue)
                    {
                        where_str = AND(where_str, "([TimeBegin] <= @end)\n");
                        _params.Add(new SqlParameter("end", end));
                    }
                    if (!string.IsNullOrEmpty(train_num))
                    {
                        where_str = AND(where_str, "([TrainNum] like \'%" + train_num + "%\')\n");
                        //_params.Add(new SqlParameter("train_num", train_num));
                    }
                    if (!string.IsNullOrEmpty(train_index))
                    {
                        where_str = AND(where_str, "([TrainIndex] like \'%" + train_index + "%\')\n");
                        //_params.Add(new SqlParameter("train_index", train_index));
                    }
                    if (!string.IsNullOrEmpty(wag_inv))
                    {
                        string findWagQuery = (string)new SqlSelectBuilder
                        {
                            Fields = new string[] { "[TrainId]" },
                            From = "[dbo].v_wagons vw",
                            Where = "vw.Num like \'%" + wag_inv + "%\'"
                        };
                        where_str = AND(where_str, "([dbo].[v_trains].[TrainId] IN (" + findWagQuery + "))\n");
                        //_params.Add(new SqlParameter("wag_inv", wag_inv));
                    }
                    if (train_status > -1)
                    {
                        where_str = AND(where_str, "([Status] = @train_status)\n");
                        _params.Add(new SqlParameter("train_status", train_status));
                    }
                    if (train_direct > -1)
                    {
                        where_str = AND(where_str, "([dbo].[v_trains].[Direction] = @train_direct)\n");
                        _params.Add(new SqlParameter("train_direct", train_direct));
                    }
                    if (trainID > -1)
                    {
                        where_str = AND(where_str, "([dbo].[v_trains].[TrainId] = @trainID)\n");
                        _params.Add(new SqlParameter("trainID", trainID));

                    }

                    string sql = (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed",
                                "(SELECT STUFF((select ','+ Str(wTrainRefTrain.RealTrainId,6,1) from wTrainRefTrain where wTrainRefTrain.TrainId = v_trains.TrainId  Group By wTrainRefTrain.RealTrainId FOR XML PATH('')),1,1,''))as referedTrainIdList"
                        },
                        From = "[dbo].[v_trains], (" +
                        (string)new SqlSelectBuilder
                        {
                            Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                            From = "[dbo].[wagons],[dbo].[v_trains]",
                            Where = where_str,
                        } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                        Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                        OrderBy = "[TimeBegin] DESC"
                    };

                    DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                    IEnumerable<TrainData> TrainDataQuery = WIGetTrainDataQuery(dataTable);

                    // Формируем список результирующих записей
                    result = TrainDataQuery.Cast<TrainData>().ToList();
                    _DataHelper.Commit();
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="start">Номер первой записи (>0)</param>
        /// <param name="count">Максимальное количество записей (>0)</param>
        /// <param name="total">Количество поездов соответствующих критериям</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainList(DateTime begin, DateTime end, int start, int count, out int total)
        {
            if (start < 1) throw new ArgumentOutOfRangeException("start");
            if (count < 1) throw new ArgumentOutOfRangeException("count");

            List<TrainData> result = new List<TrainData>(count);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                string where_str2 = null;
                if (begin != DateTime.MinValue)
                {
                    where_str2 = "([TimeBegin] >= @begin)\n";
                    _params.Add(new SqlParameter("begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str2 = AND(where_str, "([TimeBegin] <= @end)\n");
                    _params.Add(new SqlParameter("end", end));
                }

                if (!string.IsNullOrEmpty(where_str2))
                    where_str = AND(where_str, where_str2);

                string sql = (string)new SqlSelectBuilder
                {
                    Top = start + count - 1,
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons],[dbo].[v_trains]",
                        Where = where_str,
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin] DESC"
                };

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);

                // Формируем список результирующих записей
                int index = 1;
                foreach (var train_data in TrainDataQuery)
                {
                    // Пропускаем первые start-1 записей
                    if (index++ < start) continue;
                    result.Add(train_data);
                    if (result.Count >= count) break;
                }
                // Запрашиваем общее количество записей, удовлетворяющих заданному критерию
                sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "Count([TrainId])" },
                    From = "[dbo].[trains]",
                    Where = where_str2
                };
                total = (int)_DataHelper.ExecuteScalar(sql, CopyParams(_params));
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainList(DateTime begin, DateTime end)
        {
            List<TrainData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] >= @begin)\n");
                    _params.Add(new SqlParameter("begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] <= @end)\n");
                    _params.Add(new SqlParameter("end", end));
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons],[dbo].[v_trains]",
                        Where = where_str,
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin]"
                };

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);
                // Формируем список результирующих записей
                result = TrainDataQuery.Cast<TrainData>().ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда</param>
        /// <param name="train_index">Индекс поезда</param>
        /// <param name="wag_inv">Инвентарный номер вагона по ТГНЛ, входящий в состав поеда</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainList(DateTime begin, DateTime end, string train_num, string train_index, string wag_inv)
        {
            List<TrainData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] >= @begin)\n");
                    _params.Add(new SqlParameter("begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] <= @end)\n");
                    _params.Add(new SqlParameter("end", end));
                }
                if (!string.IsNullOrEmpty(train_num))
                {
                    where_str = AND(where_str, "([TrainNum] like \'%" + train_num + "%\')\n");
                    //_params.Add(new SqlParameter("train_num", train_num));
                }
                if (!string.IsNullOrEmpty(train_index))
                {
                    where_str = AND(where_str, "([TrainIndex] like \'%" + train_index + "%\')\n");
                    //_params.Add(new SqlParameter("train_index", train_index));
                }
                if (!string.IsNullOrEmpty(wag_inv))
                {
                    string findWagQuery = (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[TrainId]" },
                        From = "[dbo].[numbers]",
                        Where = "[numbers].[Inv] like \'%" + wag_inv + "%\'"
                    };
                    where_str = AND(where_str, "([dbo].[v_trains].[TrainId] IN (" + findWagQuery + "))\n");
                    //_params.Add(new SqlParameter("wag_inv", wag_inv));
                }

                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons],[dbo].[v_trains]",
                        Where = where_str,
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin] DESC"
                };

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);

                // Формируем список результирующих записей
                result = TrainDataQuery.Cast<TrainData>().ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда</param>
        /// <param name="train_index">Индекс поезда</param>
        /// <param name="wag_inv">Инвентарный номер вагона по ТГНЛ, входящий в состав поеда</param>
        /// <param name="train_direct">Направление движения состава</param>
        /// <param name="train_status">Статус отправки</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainList(DateTime begin, DateTime end, string train_num, string train_index,
                                             string wag_inv, int train_status, int train_direct)
        {
            List<TrainData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str,  "([TimeBegin] >= @begin)\n");
                    _params.Add(new SqlParameter("begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] <= @end)\n");
                    _params.Add(new SqlParameter("end", end));
                }
                if (!string.IsNullOrEmpty(train_num))
                {
                    where_str = AND(where_str, "([TrainNum] like \'%" + train_num + "%\')\n");
                    //_params.Add(new SqlParameter("train_num", train_num));
                }
                if (!string.IsNullOrEmpty(train_index))
                {
                    where_str = AND(where_str, "([TrainIndex] like \'%" + train_index + "%\')\n");
                    //_params.Add(new SqlParameter("train_index", train_index));
                }
                if (!string.IsNullOrEmpty(wag_inv))
                {
                    string findWagQuery = (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[TrainId]" },
                        From = "[dbo].v_wagons vw",
                        Where = "vw.Num like \'%" + wag_inv + "%\'"
                    };
                    where_str = AND(where_str, "([dbo].[v_trains].[TrainId] IN (" + findWagQuery + "))\n");
                    //_params.Add(new SqlParameter("wag_inv", wag_inv));
                }
                if (train_status > -1)
                {
                    where_str = AND(where_str, "([Status] = @train_status)\n");
                    _params.Add(new SqlParameter("train_status", train_status));
                }
                if (train_direct > -1)
                {
                    where_str = AND(where_str, "([dbo].[v_trains].[Direction] = @train_direct)\n");
                    _params.Add(new SqlParameter("train_direct", train_direct));
                }

                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons],[dbo].[v_trains]",
                        Where = where_str,
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin] DESC"
                };

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);

                // Формируем список результирующих записей
                result = TrainDataQuery.Cast<TrainData>().ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда</param>
        /// <param name="train_index">Индекс поезда</param>
        /// <param name="wag_inv">Инвентарный номер вагона по ТГНЛ, входящий в состав поеда</param>
        /// <param name="train_direct">Направление движения состава</param>
        /// <param name="train_status">Статус отправки</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainList(DateTime begin, DateTime end, string train_num, string train_index,
                                             string wag_inv, int train_status, int train_direct, int trainID)
        {
            List<TrainData> result = new List<TrainData>();
            try
            {
                using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
                {
                    _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    List<DbParameter> _params = new List<DbParameter>();
                    string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                    if (begin != DateTime.MinValue)
                    {
                        where_str = AND(where_str, "([TimeBegin] >= @begin)\n");
                        _params.Add(new SqlParameter("begin", begin));
                    }
                    if (end != DateTime.MinValue)
                    {
                        where_str = AND(where_str, "([TimeBegin] <= @end)\n");
                        _params.Add(new SqlParameter("end", end));
                    }
                    if (!string.IsNullOrEmpty(train_num))
                    {
                        where_str = AND(where_str, "([TrainNum] like \'%" + train_num + "%\')\n");
                        //_params.Add(new SqlParameter("train_num", train_num));
                    }
                    if (!string.IsNullOrEmpty(train_index))
                    {
                        where_str = AND(where_str, "([TrainIndex] like \'%" + train_index + "%\')\n");
                        //_params.Add(new SqlParameter("train_index", train_index));
                    }
                    if (!string.IsNullOrEmpty(wag_inv))
                    {
                        string findWagQuery = (string)new SqlSelectBuilder
                        {
                            Fields = new string[] { "[TrainId]" },
                            From = "[dbo].v_wagons vw",
                            Where = "vw.Num like \'%" + wag_inv + "%\'"
                        };
                        where_str = AND(where_str, "([dbo].[v_trains].[TrainId] IN (" + findWagQuery + "))\n");
                        //_params.Add(new SqlParameter("wag_inv", wag_inv));
                    }
                    if (train_status > -1)
                    {
                        where_str = AND(where_str, "([Status] = @train_status)\n");
                        _params.Add(new SqlParameter("train_status", train_status));
                    }
                    if (train_direct > -1)
                    {
                        where_str = AND(where_str, "([dbo].[v_trains].[Direction] = @train_direct)\n");
                        _params.Add(new SqlParameter("train_direct", train_direct));
                    }
                    if (trainID > -1)
                    {
                        where_str = AND(where_str, "([dbo].[v_trains].[TrainId] = @trainID)\n");
                        _params.Add(new SqlParameter("trainID", trainID));

                    }

                    string sql = (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed"},
                        From = "[dbo].[v_trains], (" +
                        (string)new SqlSelectBuilder
                        {
                            Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                            From = "[dbo].[wagons],[dbo].[v_trains]",
                            Where = where_str,
                        } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                        Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                        OrderBy = "[TimeBegin] DESC"
                    };

                    DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                    IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);

                    // Формируем список результирующих записей
                    result = TrainDataQuery.Cast<TrainData>().ToList();
                    _DataHelper.Commit();
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="train_num">Номер поезда</param>
        /// <param name="train_index">Индекс поезда</param>
        /// <param name="wag_inv">Инвентарный номер вагона по ТГНЛ, входящий в состав поеда</param>
        /// <param name="train_direct">Направление движения состава</param>
        /// <param name="train_status">Статус отправки</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainList(DateTime begin, DateTime end, string train_num, string train_index,
                                             string wag_inv, int train_status, int train_direct, int limit, out int total)
        {

            List<TrainData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                string where_str2 = null;
                if (begin != DateTime.MinValue)
                {
                    where_str2 = "([TimeBegin] >= @begin)\n";
                    _params.Add(new SqlParameter("begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str2 = AND(where_str2, "([TimeBegin] <= @end)\n");
                    _params.Add(new SqlParameter("end", end));
                }
                if (!string.IsNullOrEmpty(train_num))
                {
                    where_str2 = AND(where_str2, "([TrainNum] like \'%" + train_num + "%\')\n");
                    //_params.Add(new SqlParameter("train_num", train_num));
                }
                if (!string.IsNullOrEmpty(train_index))
                {
                    where_str2 = AND(where_str2, "([TrainIndex] like \'%" + train_index + "%\')\n");
                    //_params.Add(new SqlParameter("train_index", train_index));
                }
                if (!string.IsNullOrEmpty(wag_inv))
                {
                    string findWagQuery = (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[TrainId]" },
                        From = "[dbo].[numbers]",
                        Where = "[numbers].[Inv] like \'%" + wag_inv + "%\'"
                    };
                    where_str2 = AND(where_str2, "([dbo].[v_trains].[TrainId] IN (" + findWagQuery + "))\n");
                    //_params.Add(new SqlParameter("wag_inv", wag_inv));
                }
                if (train_status > -1)
                {
                    where_str2 = AND(where_str2, "([Status] = @train_status)\n");
                    _params.Add(new SqlParameter("train_status", train_status));
                }
                if (train_direct > -1)
                {
                    where_str2 = AND(where_str2, "([dbo].[v_trains].[Direction] = @train_direct)\n");
                    _params.Add(new SqlParameter("train_direct", train_direct));
                }

                if (!string.IsNullOrEmpty(where_str2))
                    where_str = AND(where_str, where_str2);

                string sql = (string)new SqlSelectBuilder
                {
                    Top = limit,
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons],[dbo].[v_trains]",
                        Where = where_str,
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin] DESC"
                };

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);

                // Формируем список результирующих записей
                result = TrainDataQuery.Cast<TrainData>().ToList();

                // Запрашиваем общее количество записей, удовлетворяющих заданному критерию
                sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "Count([TrainId])" },
                    From = "[dbo].[trains]",
                    Where = where_str2
                };
                total = (int)_DataHelper.ExecuteScalar(sql, CopyParams(_params));

                //Завершение транзации
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="wag_inv">Распознанный инвентарный номер вагона</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainListByRec(DateTime begin, DateTime end, string rec_inv)
        {
            List<TrainData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                string where_str2 = null;

                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] >= @begin)\n");
                    _params.Add(new SqlParameter("begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] <= @end)\n");
                    _params.Add(new SqlParameter("end", end));
                }
                
                if (!string.IsNullOrEmpty(rec_inv))
                {
                    string findWagQuery = (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[TrainId]" },
                        From = "[dbo].[wagons]",
                        Where = "[wagons].[Num] like \'%" + rec_inv + "%\'"
                    };
                    where_str2 = AND(where_str2, "([dbo].[v_trains].[TrainId] IN (" + findWagQuery + "))\n");
                    //_params.Add(new SqlParameter("wag_inv", wag_inv));
                }

                //if (!string.IsNullOrEmpty(rec_inv))
                //{
                //    where_str = AND(where_str, "([Num] like \'%" + rec_inv + "%\'");
                //    //_params.Add(new SqlParameter("wag_inv", wag_inv));
                //}


                if (!string.IsNullOrEmpty(where_str2))
                    where_str = AND(where_str, where_str2);

                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
                                            "[DirId]", "[DirPath]", "[Status]", "WagonsCount", "LocoCount", 
                                            "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons],[dbo].[v_trains]",
                        Where = where_str,
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin] DESC"
                };

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);
                // Формируем список результирующих записей
                result = TrainDataQuery.Cast<TrainData>().ToList();
                _DataHelper.Commit();
            }
            return result;
        }
        
        /// <summary>Получить список поездов соответствующих критериям</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="rec_inv">Распознанные инвентарные номера вагонов</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Список поездов</returns>
        public IList<TrainData> GetTrainListByRec(DateTime begin, DateTime end, string[] rec_inv)
        {
            List<TrainData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "[dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId] \n";
                string where_str2 = null;

                if (begin != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] >= @begin)\n");
                    _params.Add(new SqlParameter("begin", begin));
                }
                if (end != DateTime.MinValue)
                {
                    where_str = AND(where_str, "([TimeBegin] <= @end)\n");
                    _params.Add(new SqlParameter("end", end));
                }

                if (rec_inv.Length > 0)
                {
                    string w_str = "";
                    for (int i = 0; i < rec_inv.Length; i++)
                    {
                        if (w_str != "") w_str += " OR ";
                        //w_str += "[wagons].[Num] like \'%" + rec_inv[i] + "%\'";
                        w_str += "[wagons].[Num] = \'" + rec_inv[i] + "\'";
                    }
                    string findWagQuery = (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[TrainId]" },
                        From = "[dbo].[wagons]",
                        Where = w_str
                    };
                    where_str2 = AND(where_str2, "([dbo].[v_trains].[TrainId] IN (" + findWagQuery + "))\n");
                }
                if (!string.IsNullOrEmpty(where_str2))
                    where_str = AND(where_str, where_str2);

                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
                                            "[DirId]", "[DirPath]", "[Status]", "WagonsCount", "LocoCount", 
                                            "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons],[dbo].[v_trains]",
                        Where = where_str,
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin] DESC"
                };

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);
                // Формируем список результирующих записей
                result = TrainDataQuery.Cast<TrainData>().ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>(!!! Переделать !!!) Получение списка неотправленных в АСУ СТ поездов
        /// по критерию "Status = 0" и заполнено поле время окончания состава</summary>
        /// <returns>Список неотправленных в АСУ СТ поездов</returns>
        public IList<TrainData> GetNewTrains()
        {
            List<TrainData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string where_str = "([Status] = 0) AND ([TimeEnd] IS NOT NULL) AND ([dbo].[wagons].[TrainId] = [dbo].[v_trains].[TrainId])\n";
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[RecId]", "[dbo].[v_trains].[TrainId]", "[TrainNum]", "[TrainIndex]",
		                                    "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
		                                    "[DirId]", "[DirPath]", "[Status]", "WagonsCount","LocoCount", 
		                                    "(SumTrainSpeed/WagonsCount) AS Speed"},
                    From = "[dbo].[v_trains], (" +
                    (string)new SqlSelectBuilder
                    {
                        Fields = new string[] { "[dbo].[wagons].[TrainId]",
		                                        "COUNT([dbo].[wagons].Sn) AS WagonsCount",
		                                        "COUNT(CASE WHEN [dbo].[wagons].[Loco] = 1  THEN 1 END)  AS LocoCount",
		                                        "SUM([dbo].[wagons].[Speed]) AS SumTrainSpeed"},
                        From = "[dbo].[wagons],[dbo].[v_trains]",
                        Where = where_str,
                    } + " GROUP BY [dbo].[wagons].[TrainId]) wag",
                    Where = "[dbo].[v_trains].[TrainId] = wag.TrainId",
                    OrderBy = "[TimeBegin] DESC"
                };

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<TrainData> TrainDataQuery = GetTrainDataQuery(dataTable);
                // Формируем список результирующих записей
                result = TrainDataQuery.Cast<TrainData>().ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Изменить данные о взвешивании поезда в базе данных</summary>
        /// <param name="data">Измененные данные о взвешивании поезда</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        public void ModifyTrain(TrainData data)
        {
            CheckTrain(data);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                string sql = (string)new SqlUpdateBuilder
                {
                    Table = "[dbo].[trains]",
                    Fields = new string[] { "[TimeBegin]", "[TimeEnd]", "[Direction]", "[Virtual]",
                                            "[TrainNum]", "[TrainIndex]", "[Status]", "[DirId]" },
                    Values = new string[] { "@begin", "@end", "@direction", "@virtual", 
                                            "@trainnum", "@trainindex", "@status", "@dir_id" },
                    Where = "[TrainId] = @train_id"
                };
                _params.Add(new SqlParameter("train_id", data.Id));
                _params.Add(new SqlParameter("begin", data.BeginTime));
                _params.Add(new SqlParameter("end", data.EndTime != DateTime.MinValue ? (object)data.EndTime : (object)DBNull.Value));
                _params.Add(new SqlParameter("direction", data.Direction));
                _params.Add(new SqlParameter("virtual", data.Virtual));
                _params.Add(new SqlParameter("trainnum", data.TrainNum != null ? (object)data.TrainNum : (object)DBNull.Value));
                _params.Add(new SqlParameter("trainindex", data.TrainIndex != null ? (object)data.TrainIndex : (object)DBNull.Value));
                _params.Add(new SqlParameter("status", data.Status));
                _params.Add(new SqlParameter("dir_id", data.DirId > 0 ? (object)data.DirId : (object)DBNull.Value));
                int result = _DataHelper.ExecuteNoneQuery(sql, _params);
                if (result == 0)
                {
                    throw new Exception("Record with given 'Id' not found!");
                }
                _DataHelper.Commit();
            }
        }

        /// <summary>Добавить запись о взешивании поезда в базу данных</summary>
        /// <param name="data">Данные поезда</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта</exception>
        /// <remarks>В случае успешного выполнения ссылка data будет указывать на новый объект,
        /// который будет содержать идентификатор добавленного поезда в базе данных</remarks>
        public void AddTrain(ref TrainData data)
        {
            CheckTrain(data);
            TrainData result = data;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("RecId", data.RecId));

                //_params.Add(new SqlParameter("TimeBegin", data.BeginTime));
                SqlParameter param_date = new SqlParameter("TimeBegin", SqlDbType.DateTime2);
                param_date.Value = data.BeginTime;
                _params.Add(param_date);

                //if (data.EndTime > DateTime.MinValue)
                //{
                //    SqlParameter param_date_end = new SqlParameter("TimeEnd", SqlDbType.DateTime2);
                //    param_date_end.Value = data.EndTime;
                //    _params.Add(param_date);
                //}

                _params.Add(new SqlParameter("Direction", data.Direction));
                if (data.DirId < 1) _params.Add(new SqlParameter("DirId", null));
                else _params.Add(new SqlParameter("DirId", data.DirId));

                //if (data.Virtual) _params.Add(new SqlParameter("Virtual", true));

                SqlParameter paramTrainId = new SqlParameter
                {
                    ParameterName = "TrainId",
                    Value = data.Id,
                    Direction = ParameterDirection.Output
                };
                _params.Add(paramTrainId);

                _DataHelper.ExecuteStoredProcedure("[dbo].[add_train]", _params);
                result = new TrainData
                {
                    RecId = data.RecId,
                    BeginTime = data.BeginTime,
                    Direction = data.Direction,
                    DirId = data.DirId,
                    DirPath = data.DirPath,
                    EndTime = data.EndTime,
                    Id = (int)paramTrainId.Value,
                    WagonsCount = data.WagonsCount,
                };
                _DataHelper.Commit();
            }
            data = result;
        }

        /// <summary>Удалить все данные поезда (включая вагоны и отладочные данные)</summary>
        /// <param name="train_id">Идентификатор поезда в системе АСКО СВ</param>
        /// <exception cref="System.Data.SqlClient.SqlException">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение входного аргумента</exception>
        public void DeleteTrain(int train_id)
        {
            if (train_id < 1)
            {
                throw new ArgumentOutOfRangeException("id");
            }
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlDeleteBuilder
                {
                    Table = "[dbo].[trains]",
                    Where = "[TrainId] = @id"
                };
                int result = _DataHelper.ExecuteNoneQuery(sql, new SqlParameter("id", train_id));
                if (result == 0)
                {
                    throw new Exception("Record with given 'Id' not found!");
                }
                _DataHelper.Commit();
            }
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
                _params.Add(new SqlParameter("Num", data.InvNumber));
                _params.Add(new SqlParameter("NumType", data.InvType));
                _params.Add(new SqlParameter("Accuracy", data.Accuracy));
                _params.Add(new SqlParameter("TimeSpan", data.TimeSpan));
                _params.Add(new SqlParameter("TimeSpanBegin", data.TimeSpanBegin));
                _params.Add(new SqlParameter("Speed", data.Speed));
                _params.Add(new SqlParameter("Direction", data.Direction));
                _params.Add(new SqlParameter("BestChannel", data.BestChannel));
                _params.Add(new SqlParameter("TotalFrames", data.TotalFrames));
                _params.Add(new SqlParameter("MissedFrames", data.MissedFrames));
                _params.Add(new SqlParameter("RecognizedFrames", data.RecognFrames));
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
                    InvType = data.InvType,
                    Accuracy = data.Accuracy,
                    TimeSpan = data.TimeSpan,
                    TimeSpanBegin = data.TimeSpanBegin,
                    Speed = data.Speed,
                    Direction = data.Direction,
                    BestChannel = data.BestChannel,
                    TotalFrames = data.TotalFrames,
                    MissedFrames = data.MissedFrames,
                    RecognFrames = data.RecognFrames
                };
                _DataHelper.Commit();
            }
            data = result;
            return result.WagId;
        }

        /// <summary>Выбрать данные из выборки вагонов</summary>
        /// <param name="dataTable">Таблица выборки данных вагонов</param>
        /// <returns></returns>
        IEnumerable<WagonData> GetWagonDataQuery(DataTable dataTable)
        {
            IEnumerable<WagonData> WagonDataQuery =
                from wagons in dataTable.AsEnumerable()
                let loco = wagons.IsNull("Loco") ? 0 : (int)wagons["Loco"]
                let inv_num = wagons.IsNull("Num") ? string.Empty : (string)wagons["Num"]
                let inv_type = wagons.IsNull("NumType") ? 0 : (int)wagons["NumType"]
                let accuracy = wagons.IsNull("Accuracy") ? 0 : (int)wagons["Accuracy"]
                let time_span = wagons.IsNull("TimeSpan") ? 0 : (int)wagons["TimeSpan"]
                let time_span_begin = wagons.IsNull("TimeSpanBegin") ? 0 : (int)wagons["TimeSpanBegin"]
                let speed = wagons.IsNull("Speed") ? 0 : (int)wagons["Speed"]
                let direction = wagons.IsNull("Direction") ? 0 : (int)wagons["Direction"]
                let best_channel = wagons.IsNull("BestChannel") ? 0 : (int)wagons["BestChannel"]
                let total_frames = wagons.IsNull("TotalFrames") ? string.Empty : (string)wagons["TotalFrames"]
                let missed_frames = wagons.IsNull("MissedFrames") ? string.Empty : (string)wagons["MissedFrames"]
                let recogn_frames = wagons.IsNull("RecognizedFrames") ? string.Empty : (string)wagons["RecognizedFrames"]
                let inv_num_nl = wagons.IsNull("Inv") ? string.Empty : (string)wagons["Inv"]
                let snsost = wagons.IsNull("SnSost") ? 0 : (int)wagons["SnSost"]
                let ref_id = wagons.IsNull("RefId") ? 0 : (int)wagons["RefId"]
                let ref_train = wagons.IsNull("RefTrain") ? 0 : (int)wagons["RefTrain"]
                let ref_point = wagons.IsNull("RefPoint") ? 0 : (int)wagons["RefPoint"]
                
                select new WagonData
                {
                    Sn = (int)wagons["Sn"],
                    SnSost = snsost,
                    Loco = loco,
                    TrainId = (int)wagons["TrainId"],
                    WagId = (int)wagons["WagId"],
                    InvNumber = inv_num,
                    InvNumByNL = inv_num_nl,
                    InvType = inv_type,
                    Accuracy = accuracy,
                    TimeSpanBegin = time_span_begin,
                    TimeSpan = time_span,
                    Speed = speed,
                    Direction = direction,
                    BestChannel = best_channel,
                    TotalFrames = total_frames,
                    MissedFrames = missed_frames,
                    RecognFrames = recogn_frames,
                    RefId = ref_id,
                    RefTrain = ref_train,
                    RefPoint = ref_point
                };

            return WagonDataQuery;
        }

        /// <summary>Получить запись журнала вагонов</summary>
        /// <param name="vagon_id">Идентификаторов записи</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Запись журнала вагонов, null - нет вагона с таким серийным номером</returns>
        public WagonData GetWagonData(int vagon_id)
        {
            if (vagon_id < 1) throw new ArgumentOutOfRangeException("sn");
            WagonData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { 
                        "[wagons].[WagId]", "[wagons].[TrainId]", "[wagons].[Sn]", "[wagons].[SnSost]",
                        "[wagons].[Loco]", "[wagons].[Num]", "[wagons].[NumType]", "[wagons].[Accuracy]", 
                        "[wagons].[TimeSpan]", "[wagons].[TimeSpanBegin]", "[wagons].[Speed]", "[wagons].[Direction]", "[wagons].[BestChannel]", 
                        "[wagons].[TotalFrames]", "[wagons].[MissedFrames]", "[wagons].[RecognizedFrames]", "[wagons].[RefId]", 
                        "[wagons].[RefTrain]", "[wagons].[RefPoint]", "[numbers].[Inv]"
                    },
                    From = "[dbo].[wagons]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" +
                           "[wagons].[TrainId] = [numbers].[TrainId] AND [wagons].[Sn] = [numbers].[Sn])",
                    Where = "[wagons].[WagId] = @wagid"
                };

                SqlParameter sn_param = new SqlParameter("wagid", vagon_id);
                //Выборка вагонов
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, sn_param);
                //Выбрать данные из выборки вагонов
                IEnumerable<WagonData> WagonDataQuery = GetWagonDataQuery(dataTable);
                //Преобразовать в List
                List<WagonData> WagonData = WagonDataQuery.ToList();
                if (WagonData.Count > 0) result = WagonData[0];
                _DataHelper.Commit();
            }
            return result;
        }
        
        /// <summary>Получить запись журнала вагонов без учета реверсивного движения (по номеру пересечения))</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <param name="vagon_sn">Порядковый номер в составе</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Запись журнала вагонов, null - нет вагона с таким серийным номером</returns>
        public WagonData GetWagonDataBySn(int train_id, int vagon_sn)
        {
            if (vagon_sn < 1) throw new ArgumentOutOfRangeException("sn");
            WagonData result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { 
                        "[wagons].[WagId]", "[wagons].[Loco]", "[wagons].[TrainId]", "[wagons].[Sn]", "[wagons].[SnSost]",
                        "[wagons].[Num]", "[wagons].[NumType]", "[wagons].[Accuracy]", 
                        "[wagons].[TimeSpan]", "[wagons].[TimeSpanBegin]", "[wagons].[Speed]", "[wagons].[Direction]", "[wagons].[BestChannel]",
                        "[wagons].[TotalFrames]", "[wagons].[MissedFrames]", "[wagons].[RecognizedFrames]", "[wagons].[RefId]", 
                        "[wagons].[RefTrain]", "[wagons].[RefPoint]", "[numbers].[Inv]"
                    },
                    From = "[dbo].[wagons]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" +
                           "[wagons].[TrainId] = [numbers].[TrainId] AND [wagons].[Sn] = [numbers].[Sn])",
                    Where = "[wagons].[TrainId] = @trainid AND [wagons].[Sn] = @sn",
                };
                //Параметры запроса
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("trainid", train_id));
                _params.Add(new SqlParameter("sn", vagon_sn));

                //Выполнение запроса
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                //Выбрать данные из выборки вагонов
                IEnumerable<WagonData> WagonDataQuery = GetWagonDataQuery(dataTable);
                //Преобразовать в List
                List<WagonData> WagonData = WagonDataQuery.ToList();

                if (WagonData.Count > 0) result = WagonData[0];
                
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить запись журнала вагонов с учетом реверсивного движения</summary>
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
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { 
                        "[wagons].[WagId]", "[wagons].[Loco]", "[wagons].[TrainId]", "[wagons].[Sn]", "[wagons].[SnSost]",
                        "[wagons].[Num]", "[wagons].[NumType]", "[wagons].[Accuracy]", 
                        "[wagons].[TimeSpan]", "[wagons].[TimeSpanBegin]", "[wagons].[Speed]", "[wagons].[Direction]", "[wagons].[BestChannel]",
                        "[wagons].[TotalFrames]", "[wagons].[MissedFrames]", "[wagons].[RecognizedFrames]", "[wagons].[RefId]", 
                        "[wagons].[RefTrain]", "[wagons].[RefPoint]", "[numbers].[Inv]"
                    },
                    From = "[dbo].[wagons]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" +
                           "[wagons].[TrainId] = [numbers].[TrainId] AND [wagons].[SnSost] = [numbers].[Sn])",
                    Where = "[wagons].[TrainId] = @trainid AND [wagons].[SnSost] = @sn",
                    OrderBy = "[SnSost] ASC, [NumType] ASC, [Accuracy] DESC"
                };
                //Параметры запроса
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("trainid", train_id));
                _params.Add(new SqlParameter("sn", vagon_sn));

                //Выполнение запроса
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);

                IEnumerable<WagonData> WagonDataQuery = GetWagonDataQuery(dataTable);

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
        /// <param name="accuracy">Вероятность распознанного номера</param>
        /// <param name="start">Номер первой записи</param>
        /// <param name="count">Максимальное количество записей</param>
        /// <param name="total">Количество записей соответствующих критериям</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Список записей журнала взвешивания вагонов</returns>
        public IList<WagonData> GetWagonList(int train_id, DateTime begin, DateTime end,
                                             string inv, int inv_type, int accuracy,
                                             int start, int count, out int total, SortOrder order)
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

                if (inv_type > 0)
                {
                    where_str = AND(where_str, "([NumType] = @inv_type)\n");
                    _params.Add(new SqlParameter("inv_type", inv_type));
                }
                if (accuracy > 0)
                {
                    where_str = AND(where_str, "([Accuracy] = @accuracy)\n");
                    _params.Add(new SqlParameter("accuracy", accuracy));
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Top = start + count - 1 < 0 ? 0 : start + count - 1,
                    Fields = new string[] 
                    { 
                        "[v_wagons].[WagId]", "[v_wagons].[TrainId]", "[v_wagons].[Sn]", "[v_wagons].[SnSost]", "[v_wagons].[Loco]",
                        "[v_wagons].[Num]", "[v_wagons].[NumType]", "[v_wagons].[Accuracy]", 
                        "[v_wagons].[TimeSpan]", "[v_wagons].[TimeSpanBegin]", "[v_wagons].[Speed]", 
                        "[v_wagons].[Direction]", "[v_wagons].[BestChannel]", "[v_wagons].[TotalFrames]", 
                        "[v_wagons].[MissedFrames]", "[v_wagons].[RecognizedFrames]", "[v_wagons].[RefId]", 
                        "[v_wagons].[RefTrain]", "[v_wagons].[RefPoint]", "[numbers].[Inv]" 
                    },
                    From = "[dbo].[v_wagons]" +
                           " LEFT OUTER JOIN [dbo].[numbers] ON (" +
                           "[v_wagons].[TrainId] = [numbers].[TrainId] AND [v_wagons].[Sn] = [numbers].[Sn])",
                    Where = where_str,
                    OrderBy = order == SortOrder.Ascending ? "[v_wagons].[WagId]" : "[v_wagons].[WagId] DESC"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                //Выбрать данные из выборки вагонов
                IEnumerable<WagonData> WagonDataQuery = GetWagonDataQuery(dataTable);
                // Формируем список результирующих записей
                int index = 1;
                foreach (var wagon_data in WagonDataQuery)
                {
                    // Пропускаем первые start-1 записей если start 
                    if (start > 0)
                        if (index++ < start) continue;

                    result.Add(wagon_data);

                    if (count > 0)
                        if (result.Count >= count) break;
                }
                // Запрашиваем общее количество записей, удовлетворяющих заданному критерию
                sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "Count([Sn])" },
                    From = "[dbo].[v_wagons]",
                    Where = where_str
                };
                total = (int)_DataHelper.ExecuteScalar(sql, CopyParams(_params));
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить группу вагонов (для склейки)</summary>
        /// <param name="start_train_id"></param>
        /// <param name="stop_train_id"></param>
        /// <returns></returns>
        public IList<WagonData> GetWagonList(int start_train_id, int stop_train_id)
        {
            if (start_train_id < 0 || stop_train_id < 0) throw new ArgumentOutOfRangeException("train_id");

            List<WagonData> result = new List<WagonData>();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();
                /*
                string where_str = null;
                if (start_train_id > 0)
                {
                    where_str = AND(where_str, "([v_wagons].[TrainId] >= @start_train_id)\n");
                    _params.Add(new SqlParameter("start_train_id", start_train_id));
                }
                if (stop_train_id > 0)
                {
                    where_str = AND(where_str, "([v_wagons].[TrainId] <= @stop_train_id)\n");
                    _params.Add(new SqlParameter("stop_train_id", stop_train_id));
                }
                */ 
                string sql = " SELECT * FROM v_wagons " +
                             " LEFT OUTER JOIN [dbo].[numbers] ON ([v_wagons].[TrainId] = [numbers].[TrainId] AND [v_wagons].[Sn] = [numbers].[Sn])" +
                             " WHERE [v_wagons].[TrainId] >= " + start_train_id.ToString() + " AND [v_wagons].[TrainId] <= " + stop_train_id.ToString() + 
                             " AND (RefId IS NULL OR RefId = 0) " + 
                             " ORDER BY WagId ASC";
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                //Выбрать данные из выборки вагонов
                IEnumerable<WagonData> WagonDataQuery = GetWagonDataQuery(dataTable);
                // Формируем список результирующих записей
                foreach (var wagon_data in WagonDataQuery) result.Add(wagon_data);
                _DataHelper.Commit();
            }
            return result;
        }
        
        /// <summary>Запрос вагонов выбранного периода</summary>
        /// <param name="begin">Начало периода</param>
        /// <param name="end">Окончание периода</param>
        /// <param name="sort">Порядок сортировки</param>
        /// <returns>Списко вагонов</returns>
        public IList<WagonData> GetWagonList(DateTime begin, DateTime end, SortOrder sort)
        {
            int total;
            return GetWagonList(0, begin, end, "", 0, 0, 0, 0, out total, sort);
        }

        /// <summary>Запрос вагонов выбранного поезда</summary>
        /// <param name="train_id">Идентификатор поезда</param>
        /// <returns>Списко вагонов</returns>
        public IList<WagonData> GetWagonList(int train_id)
        {
            int total;
            return GetWagonList(train_id, DateTime.MinValue, DateTime.MinValue,
                                "", 0, 0, 0, 0, out total, SortOrder.Ascending);
        }

        /// <summary>Запрос списка вагонов с учетом реверсивного движения (для отправки в АСУ СТ)</summary>
        /// <param name="train_id">Идентификатор состава</param>
        /// <returns></returns>
        public IList<WagonData> GetWagonListUnique(int train_id)
        {
            if (train_id < 0) throw new ArgumentOutOfRangeException("train_id");
            List<WagonData> result = new List<WagonData>(0);
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();

                //Выборка вагонов с сортировкой по номеру вагона в составе, типу распознанного номера и вероятности распознавания
                string sql = " SELECT * FROM v_wagons \r\n" + 
                             " LEFT OUTER JOIN [dbo].[numbers] ON ([v_wagons].[TrainId] = [numbers].[TrainId] AND [v_wagons].[Sn] = [numbers].[Sn]) \r\n" +
                             " WHERE [v_wagons].[TrainId] = " + train_id.ToString() + "\r\n" +
                             //" ORDER BY SnSost, NumType ASC, Accuracy DESC";
                             " ORDER BY SnSost ASC, Accuracy DESC, NumType ASC";
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                //Выбрать данные из выборки вагонов
                IEnumerable<WagonData> WagonDataQuery = GetWagonDataQuery(dataTable);
                // Формируем список результирующих записей
                int sn = 0;//Порядковый номер вагона в составе
                foreach (var wagon_data in WagonDataQuery)
                {
                    if (sn < wagon_data.SnSost)
                    {
                        result.Add(wagon_data);
                        sn = wagon_data.SnSost;
                    }
                }
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Изменить запись в таблице вагонов (wagons)</summary>
        /// <param name="data">Данные взвешивания вагона</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойств объекта или не задан контекст транзакции</exception>
        /// <exception cref="System.ObjectDisposedException">Транзакция завершена и не может быть использована</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public void ModifyWagon(WagonData data)
        {
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                if (context == null)
                {
                    throw new ArgumentException("context");
                }
                CheckWagon(data);
                List<DbParameter> _params = new List<DbParameter>(16);
                string sql = (string)new SqlUpdateBuilder
                {
                    Fields = new string[] 
                    { 
                        "[Sn]", "[SnSost]", "[Loco]", "[Num]", "[NumType]", "[Accuracy]", "[TimeSpan]", 
                        "[TimeSpanBegin]", "[Speed]", "[Direction]", "[BestChannel]", 
                        "[TotalFrames]", "[MissedFrames]", "[RecognizedFrames]", "[RefId]", "[RefTrain]", "[RefPoint]"
                    },
                    Values = new string[] 
                    {
                        "@Sn", "@SnSost","@Loco", "@Num", "@NumType", "@Accuracy", "@TimeSpan", 
                        "@TimeSpanBegin", "@Speed", "@Direction", "@BestChannel", 
                        "@TotalFrames", "@MissedFrames", "@RecognizedFrames", "@RefId", "@RefTrain", "@RefPoint"
                    },
                    Table = "[dbo].[wagons]",
                    Where = "[WagId] = @WagId"
                };
                DateTime changed = DateTime.Now;
                _params.Add(new SqlParameter("WagId", data.WagId));//Идентификатор вагона в базе
                _params.Add(new SqlParameter("Sn", data.Sn));
                _params.Add(new SqlParameter("SnSost", data.SnSost));
                _params.Add(new SqlParameter("Loco", data.Loco));
                _params.Add(new SqlParameter("Num", data.InvNumber));
                _params.Add(new SqlParameter("NumType", data.InvType));
                _params.Add(new SqlParameter("Accuracy", data.Accuracy));
                _params.Add(new SqlParameter("TimeSpan", data.TimeSpan));
                _params.Add(new SqlParameter("TimeSpanBegin", data.TimeSpanBegin));
                _params.Add(new SqlParameter("Speed", data.Speed));
                _params.Add(new SqlParameter("Direction", data.Direction));
                _params.Add(new SqlParameter("BestChannel", data.BestChannel));
                _params.Add(new SqlParameter("TotalFrames", data.TotalFrames));
                _params.Add(new SqlParameter("MissedFrames", data.MissedFrames));
                _params.Add(new SqlParameter("RecognizedFrames", data.RecognFrames));
                _params.Add(new SqlParameter("RefId", data.RefId));
                _params.Add(new SqlParameter("RefTrain", data.RefTrain));
                _params.Add(new SqlParameter("RefPoint", data.RefPoint));

                int result = context.ExecuteNoneQuery(sql, _params);
                if (result != 1)
                {
                    throw new Exception("Запись с указанным 'WagId' отсутствует!");
                }
            }
        }

        #endregion

        #region NatureLists

        /// <summary>Изменяет данные поезда и вагонов в БД в соответствии с данными ТГНЛ</summary>
        /// <param name="train">Данные поезда из ТГНЛ</param>
        /// <param name="wagons">Данные вагонов из ТГНЛ</param>
        /// <exception cref="System.ArgumentException">Ошибка в данных поезда или вагонов</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        public void BindToNL(TrainData train, IList<WagonData> wagons)
        {
            CheckTrain(train);
            if (wagons == null) { throw new ArgumentNullException("wagons"); }
            foreach (var data in wagons) { CheckWagon(data); }
            var wagonsQuery = from wagon in wagons.AsEnumerable()
                              where wagon.TrainId == train.Id
                              select wagon;
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    context.BeginTransaction();
                    BindTrain(context, train);

                    // Удаляем предыдущую информацию об инвентарных номерах вагонов
                    string sql = (string)new SqlDeleteBuilder
                    {
                        Table = "[dbo].[numbers]",
                        Where = "[trainId] = @TrainId"
                    };
                    context.ExecuteNoneQuery(sql, new SqlParameter("TrainId", train.Id));
                    foreach (var wagon in wagonsQuery)
                    {
                        BindWagon(context, wagon);
                    }
                    context.Commit();
                }
                catch
                {
                    context.Rollback();
                    throw;
                }
            }
        }

        /// <summary>Проверка наличия у поезда натурного листа</summary>
        /// <param name="train_id">Идентификатор поезда</param>
        /// <returns></returns>
        bool CheckNL(int train_id)
        {
            return true;
        }

        /// <summary>Изменение данных поезда по уточненным данным ТГНЛ</summary>
        /// <param name="context">Контекст транзакции в базе данных</param>
        /// <param name="data">Уточненные данные поезда</param>
        /// <exception cref="System.NullReferenceException">Не задан объект данных</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        void BindTrain(SqlDataHelper context, TrainData data)
        {
            List<DbParameter> _params = new List<DbParameter>(3);
            string sql = (string)new SqlUpdateBuilder
            {
                Fields = new string[] { "[TrainNum]", "[TrainIndex]" },
                Values = new string[] { "@TrainNum", "@TrainIndex" },
                Table = "[dbo].[trains]",
                Where = "[TrainId] = @TrainId"
            };
            _params.Add(new SqlParameter("TrainId", data.Id));
            _params.Add(new SqlParameter("TrainNum", data.TrainNum));
            _params.Add(new SqlParameter("TrainIndex", data.TrainIndex));

            int result = context.ExecuteNoneQuery(sql, _params);
            if (result != 1)
            {
                throw new Exception("Запись с указанным 'TrainId' отсутствует!");
            }
        }

        /// <summary>Изменение данных вагона по уточненным данным ТГНЛ</summary>
        /// <param name="context">Контекст транзакции в базе данных</param>
        /// <param name="data">Уточненные данные вагона</param>
        /// <exception cref="System.NullReferenceException">Не задан объект данных</exception>
        /// <exception cref="System.Exception">Не удалось выполнить запрос к базе данных</exception>
        void BindWagon(SqlDataHelper context, WagonData data)
        {
            // Добавляем данные об инвентарном номере вагона в таблицу натурного листа
            List<DbParameter> _params = new List<DbParameter>(3);
            string sql = (string)new SqlInsertBuilder
            {
                Fields = new string[] { "[TrainId]", "[Sn]", "[Inv]" },
                Values = new string[] { "@TrainId", "@Sn", "@Inv" },
                Table = "[dbo].[numbers]"
            };
            _params.Add(new SqlParameter("TrainId", data.TrainId));
            _params.Add(new SqlParameter("Sn", data.Sn));
            _params.Add(new SqlParameter("Inv", data.InvNumByNL));

            context.ExecuteNoneQuery(sql, _params);

            // Изменяем информацию о типе подвижной единицы
            /*
            _params.Clear();
            sql = (string)new SqlUpdateBuilder
            {
                Fields = new string[] { "[Loco]" },
                Values = new string[] { "@Loco" },
                Table = "[dbo].[wagons]",
                Where = "[WagId] = @WagonId"
            };
            _params.Add(new SqlParameter("WagonId", data.WagId));
            _params.Add(new SqlParameter("Loco", data.Loco));

            int result = context.ExecuteNoneQuery(sql, _params);
            if (result != 1)
            {
                throw new Exception("Запись с указанным 'WagId' отсутствует!");
            }
            */
        }

        /// <summary>Удаление старых поездов по прибытию</summary>
        public void ClearTrainsArrival(DateTime date, string index)
        {
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    context.BeginTransaction();
                    //Формирование строки удаление старых поездов по прибытию
                    List<DbParameter> _params = new List<DbParameter>(1);
                    _params.Add(new SqlParameter("@date", date));
                    _params.Add(new SqlParameter("@index", index));
                    string sql = (string)new SqlDeleteBuilder
                    {
                        Where = "[date] < @date OR [index] = @index",
                        Table = "[dbo].[trains_arrival]",
                    };
                    context.ExecuteNoneQuery(sql, _params);
                    context.Commit();
                }
                catch
                {
                    context.Rollback();
                    throw new Exception("Ошибка удаления поездов по прибытию");
                }
            }
        }

        /// <summary>Сохранение поезда по прибытию</summary>
        public void SaveTrainArrival(TrainData data, string code_direction, string code_station, int feature)
        {
            List<DbParameter> _params = new List<DbParameter>(6);
            //Формирование строки вставки поезда по прибытию
            string sql = (string)new SqlInsertBuilder
            {
                Fields = new string[] { "[code_direction]", "[code_station]", "[number]", "[index]", "[feature]", "[date]" },
                Values = new string[] { "@direction", "@station", "@number", "@index", "@feature", "@date" },
                Table = "[dbo].[trains_arrival]",
            };
            _params.Add(new SqlParameter("direction", code_direction));
            _params.Add(new SqlParameter("station", code_station));
            _params.Add(new SqlParameter("number", data.TrainNum));
            _params.Add(new SqlParameter("index", data.TrainIndex));
            _params.Add(new SqlParameter("feature", feature));
            _params.Add(new SqlParameter("date", data.BeginTime));

            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    context.BeginTransaction();
                    int result = context.ExecuteNoneQuery(sql, _params);
                    context.Commit();
                    //Сохранение идентификатора поезда
                    data.Id = DataBaseUtils.GetCurrentIdent("trains_arrival");
                    if (result != 1) throw new Exception("Ошибка сохранения поезда по прибытию");
                }
                catch
                {
                    context.Rollback();
                    throw new Exception("Ошибка сохранения поезда по прибытию");
                }
            }
        }

        /// <summary>Сохранить преднатурный лист</summary>
        /// <param name="wagons">Список вагонов</param>
        public void SaveWagonsArrival(IList<WagonData> wagons)
        {
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                try
                {
                    context.BeginTransaction();
                    //Цикл по вагонам
                    foreach (var wagon in wagons)
                    {
                        List<DbParameter> _params = new List<DbParameter>(3);
                        string sql = (string)new SqlInsertBuilder
                        {
                            Fields = new string[] { "[train_id]", "[sn]", "[number]" },
                            Values = new string[] { "@train_id", "@sn", "@number" },
                            Table = "[dbo].[numbers_arrival]"
                        };
                        _params.Add(new SqlParameter("train_id", wagon.TrainId));
                        _params.Add(new SqlParameter("sn", wagon.Sn));
                        _params.Add(new SqlParameter("number", wagon.InvNumByNL));
                        context.ExecuteNoneQuery(sql, _params);
                    }
                    context.Commit();
                }
                catch
                {
                    context.Rollback();
                    throw new Exception("Ошибка сохранения вагонов по прибытию");
                }
            }
        }

        /// <summary>Получение списка поездов по прибытию (с вагонами) (список преднатурных листов)</summary>
        public IList<ArrivalTrain> GetTrainsArrival()
        {
            List<ArrivalTrain> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                List<DbParameter> _params = new List<DbParameter>();

                /*                
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[id]", "[code_direction]", "[code_station]", "[number]", "[index]",
		                                    "[feature]", "[date]", 
                                            (string)new SqlSelectBuilder
                                            {
                                                Fields = new string[] { "COUNT([numbers_arrival].[id]) AS wagons_count" },
                                                From = "[dbo].[numbers_arrival]",
                                                Where = "[dbo].[numbers_arrival].[train_id] = [dbo].[trains_arrival].[id]",
                                            } },
                    From = "[dbo].[trains_arrival]",
                    OrderBy = "[date]"
                };
                */ 

                string sql = "SELECT id, code_direction, code_station, number, [index], feature, [date], " +
                             " (SELECT COUNT(numbers_arrival.id) FROM numbers_arrival " +
                             " WHERE numbers_arrival.train_id = trains_arrival.id) AS wagons_count " +
                             " FROM trains_arrival ORDER BY date";

                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<ArrivalTrain> query =
                    from trains in dataTable.AsEnumerable()
                    let id = trains.IsNull("id") ? 0 : (int)trains["id"]
                    let code_direction = trains.IsNull("code_direction") ? String.Empty : (string)trains["code_direction"]
                    let code_station = trains.IsNull("code_station") ? String.Empty : (string)trains["code_station"]
                    let num = trains.IsNull("number") ? String.Empty : (string)trains["number"]
                    let index = trains.IsNull("index") ? String.Empty : (string)trains["index"]
                    let feature = trains.IsNull("feature") ? 0 : (int)trains["feature"]
                    let date = trains.IsNull("date") ? DateTime.MinValue : (DateTime)trains["date"]
                    let w_count = trains.IsNull("wagons_count") ? 0 : (int)trains["wagons_count"]

                    select new ArrivalTrain
                    {
                        Id = id,
                        TrainNum = num,
                        TrainIndex = index,
                        Date = date,
                        CodeDirection = code_direction,
                        CodeStation = code_station,
                        Feature = feature,
                        WagonsCount = w_count,
                    };

                // Формируем список результирующих записей
                result = query.Cast<ArrivalTrain>().ToList();
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получение списка вагонов поезда по прибытию</summary>
        /// <returns></returns>
        public IList<WagonData> GetWagonsArrival(int train_id)
        {
            if (train_id < 1) throw new ArgumentOutOfRangeException("id");
            List<WagonData> result = new List<WagonData>();
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[numbers_arrival].[id]", "[numbers_arrival].[train_id]", 
                                            "[numbers_arrival].[sn]", "[numbers_arrival].[number]" },
                    From = "[dbo].[numbers_arrival]",
                    Where = "[numbers_arrival].[train_id] = @trainid"
                };
                //Параметры запроса
                List<DbParameter> _params = new List<DbParameter>();
                _params.Add(new SqlParameter("trainid", train_id));

                //Выполнение запроса
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<WagonData> query =
                    from wagons in dataTable.AsEnumerable()
                    let id = wagons.IsNull("id") ? 0 : (int)wagons["id"]
                    let sn = wagons.IsNull("sn") ? 0 : (int)wagons["sn"]
                    let train = wagons.IsNull("train_id") ? 0 : (int)wagons["train_id"]
                    let inv_num_nl = wagons.IsNull("number") ? string.Empty : (string)wagons["number"]

                    select new WagonData
                    {
                        WagId = id,
                        Sn = sn,
                        TrainId = train,
                        InvNumByNL = inv_num_nl,
                    };

                foreach (var wagon_data in query) result.Add(wagon_data);
                _DataHelper.Commit();
            }
            return result;
        }

        #endregion

        #region Events

        /// <summary>Получить записи из журнала событий системы</summary>
        /// <param name="begin">Начальная дата и время, MinValue - любая</param>
        /// <param name="end">Конечная дата и время, MinValue - любая</param>
        /// <param name="cat_list">Список категорий сообщений разделенный запятыми (пустая строка - все категории)</param>
        /// <param name="start">Номер первой записи</param>
        /// <param name="count">Маскимальное количество записей</param>
        /// <param name="total">Количество записей соответствующих критериям</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое значение входного параметра</exception>
        /// <returns>Список записей из журнала событий</returns>
        public IList<EventData> GetEventList(DateTime begin, DateTime end,
                                             string cat_list, int start, int count, out int total)
        {
            if (start < 1)
            {
                throw new ArgumentOutOfRangeException("start");
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            List<EventData> result = new List<EventData>(count);
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
                if (!string.IsNullOrEmpty(cat_list))
                {
                    where_str = AND(where_str, "([MsgiD] IN (" + cat_list + "))\n");
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Top = start + count - 1,
                    Fields = new string[] {
                        "[Sn]" ,"[EvDateTime]", "[MsgText]", "[RecogId]", "[EvSource]", "[EvData]", "[OpName]", "[MsgId]"
                    },
                    From = "[dbo].[v_eventlog]",
                    Where = where_str,
                    OrderBy = "[EvDateTime] DESC, [Sn]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<EventData> EventDataQuery =
                    from v_eventlog in dataTable.AsEnumerable()
                    let recog = v_eventlog.IsNull("RecogId") ? 0 : (int)v_eventlog["RecogId"]
                    let source = v_eventlog.IsNull("EvSource") ? string.Empty : (string)v_eventlog["EvSource"]
                    let data = v_eventlog.IsNull("EvData") ? string.Empty : (string)v_eventlog["EvData"]
                    let text = v_eventlog.IsNull("MsgText") ? string.Empty : (string)v_eventlog["MsgText"]
                    let name = v_eventlog.IsNull("OpName") ? string.Empty : (string)v_eventlog["OpName"]
                    let msgid = v_eventlog.IsNull("MsgId") ? 0 : (int)v_eventlog["MsgId"]

                    select new EventData
                    {
                        Sn = (int)v_eventlog["Sn"],
                        EventTime = (DateTime)v_eventlog["EvDateTime"],
                        RecogId = recog,
                        Text = text,
                        Source = source,
                        Data = data,
                        MsgId = msgid
                    };
                // Формируем список результирующих записей
                int index = 1;
                foreach (var event_data in EventDataQuery)
                {
                    // Пропускаем первые start-1 записей
                    if (index++ < start)
                    {
                        continue;
                    }

                    result.Add(event_data);
                    if (result.Count >= count)
                    {
                        break;
                    }
                }
                // Запрашиваем общее количество записей, удовлетворяющих заданному критерию
                sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "Count([Sn])" },
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
        public IList<EventData> GetEventList(DateTime begin, DateTime end, string msg_id_list)
        {
            string[] msg_ids = null;
            if (!string.IsNullOrEmpty(msg_id_list))
            {
                msg_ids = msg_id_list.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (msg_ids.Length == 0)
                {
                    throw new ArgumentException("Invalid list of 'MsgId'!", "msg_id_list");
                }
            }
            List<EventData> result = null;
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
                if (msg_ids != null)
                {
                    where_str = AND(where_str, "([MsgiD] IN (" + msg_id_list + "))\n");
                }
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] {
                        "[Sn]" ,"[EvDateTime]", "[MsgText]", "[RecogId]", "[EvSource]", "[EvData]", "[OpName]", "[MsgId]"
                    },
                    From = "[dbo].[v_eventlog]",
                    Where = where_str,
                    OrderBy = "[EvDateTime] DESC, [Sn]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql, _params);
                IEnumerable<EventData> EventDataQuery =
                    from v_eventlog in dataTable.AsEnumerable()
                    let recog = v_eventlog.IsNull("RecogId") ? 0 : (int)v_eventlog["RecogId"]
                    let source = v_eventlog.IsNull("EvSource") ? string.Empty : (string)v_eventlog["EvSource"]
                    let data = v_eventlog.IsNull("EvData") ? string.Empty : (string)v_eventlog["EvData"]
                    let text = v_eventlog.IsNull("MsgText") ? string.Empty : (string)v_eventlog["MsgText"]
                    let name = v_eventlog.IsNull("OpName") ? string.Empty : (string)v_eventlog["OpName"]
                    select new EventData
                    {
                        Sn = (int)v_eventlog["Sn"],
                        EventTime = (DateTime)v_eventlog["EvDateTime"],
                        Text = text,
                        RecogId = recog,
                        Source = source,
                        Data = data,
                        OpName = name,
                        MsgId = (int)v_eventlog["MsgId"],
                    };
                // Формируем список результирующих записей
                result = EventDataQuery.Cast<EventData>().ToList();
                _DataHelper.Commit();
            }
            return result;
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
                _params.Add(new SqlParameter("RecogId", data.RecogId));
                _params.Add(new SqlParameter("EvData", string.IsNullOrEmpty(data.Data) ? null : data.Data));
                _params.Add(new SqlParameter("EvSource", string.IsNullOrEmpty(data.Source) ? null : data.Source));
                if (data.OpId <= 0) _params.Add(new SqlParameter("OpId", null));
                else _params.Add(new SqlParameter("OpId", data.OpId));
                if (data.HasVideo) _params.Add(new SqlParameter("HasVideo", 1));

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
                    RecogId = data.RecogId,
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

        #endregion

        #region Operators

        /// <summary>Получить список операторов АРМ СБВ УВГ</summary>
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
                                            "[OpName]", "[Status]", "[Permissions]" },
                    From = "[dbo].[operators]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql);
                IEnumerable<OperatorData> OpDataQuery =
                    from opr in dataTable.AsEnumerable()
                    select new OperatorData((int)opr["OpId"], (string)opr["OpLogin"], (string)opr["OpPassword"],
                                            (string)opr["OpName"], (int)opr["Status"], (int)opr["Permissions"]);
                result = OpDataQuery.ToList();
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
                SqlParameter paramOpId = new SqlParameter
                {
                    ParameterName = "OpId",
                    Value = data.Id,
                    Direction = ParameterDirection.Output
                };
                _params.Add(paramOpId);
                _DataHelper.ExecuteStoredProcedure("[dbo].[add_operator]", _params);
                result = new OperatorData((int)paramOpId.Value, data.Login, data.Password,
                                          data.OpName, data.Status, data.Permissions);
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
                    Fields = new string[] { "[OpLogin]", "[OpPassword]", "[OpName]", "[Status]", "[Permissions]" },
                    Values = new string[] { "@login", "@password", "@name", "@status", "@level" },
                    Where = "[OpId] = @id"
                };
                _params.Add(new SqlParameter("login", data.Login));
                _params.Add(new SqlParameter("password", data.Password));
                _params.Add(new SqlParameter("name", data.OpName));
                _params.Add(new SqlParameter("status", data.Status));
                _params.Add(new SqlParameter("level", data.Permissions));
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
                                             (int)directory["DirStat"] == 1);
                result = DirDataQuery.ToList();
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
        public void AddDirectory(ref DirectoryData data)
        {
            CheckDirectory(data);
            DirectoryData result = data;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                List<DbParameter> _params = new List<DbParameter>();
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                _params.Add(new SqlParameter("DirPath", data.Path));
                int dir_stat = data.Active ? 1 : 0;
                _params.Add(new SqlParameter("DirStat", dir_stat));
                SqlParameter paramDirId = new SqlParameter
                {
                    ParameterName = "DirId",
                    Value = data.Id,
                    Direction = ParameterDirection.Output
                };
                _params.Add(paramDirId);
                _DataHelper.ExecuteStoredProcedure("[dbo].[add_directory]", _params);
                result = new DirectoryData((int)paramDirId.Value, data.Path, data.Status, data.Active);
                _DataHelper.Commit();
            }
            data = result;
        }

        /// <summary>Удалить каталог видеоархива из базы данных</summary>
        /// <param name="id">Идентификатор каталога видеоархива в базе данных</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение входного параметра вне допустимого диапазона значений</exception>
        public void DeleteDirectory(int id)
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
                    Table = "[dbo].[directories]",
                    Where = "[DirId] = @id"
                };
                int result = _DataHelper.ExecuteNoneQuery(sql, new SqlParameter("id", id));
                if (result == 0)
                {
                    throw new Exception("Record with given 'Id' not found!");
                }
                _DataHelper.Commit();
            }
        }

        /// <summary>Изменить данные о каталоге видеоархива в базе данных</summary>
        /// <param name="data">Измененные данные каталога видеоархива</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на объект данных</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение свойства заданного объекта</exception>
        public void ModifyDirectory(DirectoryData data)
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
                int dir_stat = data.Active ? 1 : 0;
                _params.Add(new SqlParameter("path", data.Path));
                _params.Add(new SqlParameter("stat", dir_stat));
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

        #region Devices

        /// <summary>Получить список камер (наименование и статус устройства)</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.FormatException">Неверный формат номера видеокамеры</exception>
        /// <exception cref="System.OverflowException">Номер видеокамеры превышает допустимое значение</exception>
        /// <exception cref="System.IndexOutOfRangeException">Неверный формат параметра, обозначающего имя видеокамеры</exception>
        /// <returns>Список камер</returns>
        /// <remarks>Предполагается, что параметр, описывающий название камеры (таблица config), имеет формат
        /// 'Telecamera.%n%.Name', где %n% - целочисленный идентификатор камеры. Имя камеры в таблице status должно
        /// иметь формат 'Telecamera.%n%'
        /// </remarks>
        public IList<CameraStatData> GetCameraList(int point)
        {
            List<CameraStatData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                int count = GetIntConfigParam(_DataHelper, "Video.TKCount", 5);
                result = new List<CameraStatData>();
                for (int i = 0; i < count; i++)
                {
                    result.Add(new CameraStatData(i + 1,
                               GetStringConfigParam(_DataHelper, "Telecamera." + i.ToString() + ".Name", ""),
                               GetCameraStat(_DataHelper, point, i),
                               GetCameraResolutionString(_DataHelper, i)));

                }
                _DataHelper.Commit();
            }
            return result;
        }

        /// <summary>Получить список дисков (наименование и статус дисков)</summary>
        /// <returns>Список дисков</returns>
        public IList<DiskStatData> GetDiskList()
        {
            List<DiskStatData> result = null;
            using (SqlDataHelper _DataHelper = new SqlDataHelper(_ConnectionString))
            {
                _DataHelper.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                string sql = (string)new SqlSelectBuilder
                {
                    Fields = new string[] { "[DirPath]", "[DirStat]" },
                    From = "[dbo].[directories]",
                    OrderBy = "[DirPath]"
                };
                DataTable dataTable = _DataHelper.ExecuteCommand(sql);
                _DataHelper.Commit();

                result = new List<DiskStatData>(dataTable.Rows.Count);
                List<string> proccessed_drives = new List<string>(dataTable.Rows.Count);
                foreach (var tableRow in dataTable.AsEnumerable())
                {
                    try
                    {
                        string drive_name = Directory.GetDirectoryRoot((string)tableRow[0]);
                        if (proccessed_drives.IndexOf(drive_name) >= 0)
                        {
                            // Этот диск уже обработан...
                            continue;
                        }
                        DriveInfo drive = new DriveInfo(drive_name);
                        DiskStatData disk = new DiskStatData();
                        disk.Id = drive.Name;
                        if ((int)tableRow[1] == 0)
                        {
                            disk.Status = DevState.none;
                        }
                        else
                        {
                            disk.Status = drive.IsReady ? DevState.online : DevState.offline;
                            proccessed_drives.Add(drive_name);
                        }
                        if (disk.Status == DevState.online)
                        {
                            disk.Volume = (int)(drive.TotalSize / (1024L * 1024L));
                            disk.FreeSpace = (int)(drive.TotalFreeSpace / (1024L * 1024L));
                        }
                        result.Add(disk);
                    }
                    catch { }
                }
            }
            return result;
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

        #region БС

        /// <summary>Получить коммуникационный статус БС</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Статус БИС</returns>
        public DevState GetBisStat(int point)
        {
            //Проверка - включен ли БС
            bool enabled = GetBoolConfigParam("Recog." + point.ToString() + ".BS.Active", false);
            if (enabled == false) return DevState.none;
            //Вернуть текущий статус
            return GetDevStat("Recog." + point.ToString() + ".BS");
        }

        /// <summary>Установить коммуникационный статус БИС</summary>
        /// <param name="stat">Статус БИС</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetBisStat(int point, DevState stat)
        {
            SetStat("Recog." + point.ToString() + ".BS", stat.ToString());
        }

        /// <summary>Установить статус датчика вскрытия БС</summary>
        /// <param name="stat">Статус БИС</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetBisTamper(int point, TamperStat stat)
        {
            SetStat("Recog." + point.ToString() + ".Tamper", stat.ToString());
        }

        #endregion

        #region ASU

        /// <summary>
        /// Получить количество запросов от АСУ СТ
        /// </summary>
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
        public TamperStat GetTamperStat(int point)
        {
            string status_name = GetStringStat("Recog." + point.ToString() + ".Tamper");
            for (var status = TamperStat.unknown; status <= TamperStat.alarm; ++status)
                if (status.ToString() == status_name) return status;
            return TamperStat.unknown;
        }

        /// <summary>Установить статус датчика вскрытия шкафа</summary>
        /// <param name="stat">Статус датчика вскрытия шкафа</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetTamperStat(int point, TamperStat stat)
        {
            SetStat("Recog." + point.ToString() + ".Tamper", stat.ToString());
        }

        #endregion

        #region Телекамера

        /// <summary>Получить коммуникационный статус камеры</summary>
        /// <param name="camera">Номер камеры (1...5)</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Номер камеры вне допустимого диапазона значений</exception>
        /// <returns>Статус камеры</returns>
        public DevState GetCameraStat(int point, int camera)
        {
            if ((camera < 1) || (camera > 5)) throw new ArgumentOutOfRangeException("camera");
            int cam_id = camera - 1;
            //Получение активности канала
            bool enabled = GetBoolConfigParam("Recog." + point.ToString() + ".Channel." + cam_id.ToString() + ".Active", false);
            if (enabled == false) return DevState.none;
            return GetDevStat("Recog." + point.ToString() + ".Camera." + cam_id.ToString());
        }

        /// <summary>Получить коммуникационный статус камеры</summary>
        /// <param name="connection">Подключение</param>
        /// <param name="point">Номер пункта считывания (0..4)</param>
        /// <param name="cam_id">Номер камеры (0..5)</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Номер камеры вне допустимого диапазона значений</exception>
        /// <returns>Статус камеры</returns>
        DevState GetCameraStat(SqlDataHelper connection, int point, int cam_id)
        {
            //Получение активности канала
            bool enabled = GetBoolConfigParam(connection, "Recog." + point.ToString() + ".Channel." + cam_id.ToString() + ".Active", false);
            if (enabled == false) return DevState.none;
            string stat = GetStringStat("Recog." + point.ToString() + ".Camera." + cam_id.ToString());
            if (stat == DevState.offline.ToString()) return DevState.offline;
            if (stat == DevState.online.ToString()) return DevState.online;
            return DevState.unknown;
        }

        /// <summary>Установить коммуникационный статус камеры</summary>
        /// <param name="camera">Номер камеры (1...5)</param>
        /// <param name="stat">Статус камеры</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetCameraStat(int point, int camera, DevState stat)
        {
            if ((camera < 1) || (camera > 5)) throw new ArgumentOutOfRangeException("camera");
            int cam_id = camera - 1;
            SetStat("Recog." + point.ToString() + ".Camera." + cam_id.ToString(), stat.ToString());
        }

        #endregion

        #endregion

        #region Modes

        #region Редактирование параметров

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

        #region Коммуникационный статус БС32

        /// <summary>Установить коммуникационный статус БИС</summary>
        /// <param name="stat">Статус БИС</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetBisMode(int point, DevMode mode)
        {
            SetStat("Recog." + point.ToString() + ".BS.Mode", mode.ToString());
        }

        /// <summary>Получить коммуникационный статус БИС</summary>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        /// <returns>Статус БИС</returns>
        public DevMode GetBisMode(int point)
        {
            bool enabled = GetBoolConfigParam("Recog." + point.ToString() + ".BS.Active", false);
            if (enabled == false) return DevMode.unknown;
            string mode = GetStringStat("Recog." + point.ToString() + ".BS.Mode");
            for (int i = 0; i < Enum.GetNames(typeof(DevMode)).Length; i++)
                if (mode == Enum.GetNames(typeof(DevMode)).GetValue(i).ToString())
                    return (DevMode)Enum.GetValues(typeof(DevMode)).GetValue(i);
            return DevMode.unknown;
        }

        /// <summary>Установить статус перезагрузки службы</summary>
        /// <param name="restart">Статус перезагрузки</param>
        /// <exception cref="System.Exception">Не удалось выполнить запрос</exception>
        public void SetRestart()
        {
            SetStat("Server.Restart", true.ToString());
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

        #endregion

        #endregion

        #region UpdateNumber

        /// <summary>
        /// Добавление события в журнал. БФл создан с целью возможности работы с другим кодом в рамках одной транзакции.
        /// </summary>
        /// <param name="data">Объект событие</param>
        /// <param name="context">Контекст БД</param>
        public void AddEvent(ref EventData data, SqlDataHelper context)
        {
            EventData result = data;
            List<DbParameter> _params = new List<DbParameter>();

            SqlParameter param_date = new SqlParameter("EvDateTime", SqlDbType.DateTime2);
            param_date.Value = data.EventTime;
            _params.Add(param_date);

            _params.Add(new SqlParameter("MsgId", data.MsgId));
            _params.Add(new SqlParameter("RecogId", data.RecogId));
            _params.Add(new SqlParameter("EvData", string.IsNullOrEmpty(data.Data) ? null : data.Data));
            _params.Add(new SqlParameter("EvSource", string.IsNullOrEmpty(data.Source) ? null : data.Source));
            if (data.OpId <= 0) _params.Add(new SqlParameter("OpId", null));
            else _params.Add(new SqlParameter("OpId", data.OpId));
            if (data.HasVideo) _params.Add(new SqlParameter("HasVideo", 1));

            SqlParameter paramSn = new SqlParameter
            {
                ParameterName = "Sn",
                Value = data.Sn,
                Direction = ParameterDirection.Output
            };

            _params.Add(paramSn);
            context.ExecuteStoredProcedure("[dbo].[add_eventlog_item]", _params);
            result = new EventData
            {
                EventTime = data.EventTime,
                MsgId = data.MsgId,
                RecogId = data.RecogId,
                Source = data.Source,
                Data = data.Data,
                OpId = data.OpId,
                HasVideo = data.HasVideo,
                Sn = (int)paramSn.Value,
                Text = data.Text,
            };
            data = result;
        }

        /// <summary>
        /// Обновить номер вагона
        /// </summary>
        /// <param name="wagonID">Идентификатор вагонов</param>
        /// <param name="invNumber">Инвентарый номер</param>
        /// <param name="userName">Имя пользователя, изменившего данный номер</param>
        public void UpdateWagonInvNumber(int wagonID, string invNumber, string userName)
        {
            using (SqlDataHelper context = new SqlDataHelper(_ConnectionString))
            {
                if (context == null) throw new ArgumentException("context");
                context.BeginTransaction();
                string getOldSnCommand = "Select Num, TrainId, SnSost from dbo.wagons where WagId=@WagID";
                List<DbParameter> getWagonParams = new List<DbParameter>
                {
                        new SqlParameter("WagID",wagonID)
                };

                string oldNumber=string.Empty;
                string trainID = string.Empty;
                string snSost = string.Empty;
                try
                {
                    DataTable table = context.ExecuteCommand(getOldSnCommand, getWagonParams);
                    if (table.Rows.Count > 0)
                    {
                        oldNumber = table.Rows[0]["Num"].ToString();
                        trainID = table.Rows[0]["TrainId"].ToString();
                        snSost = table.Rows[0]["SnSost"].ToString();
                    }
                }
                catch { }
               

                string sqlCommand = "Update dbo.wagons set Accuracy=@Accuracy,Num=@Num where WagId=@WagID";

                List<DbParameter> parametrs = new List<DbParameter>{
                    new SqlParameter("Accuracy",101),
                    new SqlParameter("Num",invNumber),
                    new SqlParameter("WagID",wagonID)
                };

                int result = context.ExecuteNoneQuery(sqlCommand, parametrs);
                if (result != 1)
                {
                    throw new Exception("Запись с указанным 'WagId' отсутствует!");
                }
                //Текст сообщения
                string messageText = String.Format(@"Изменение инвентарного номера вагона №{0} в составе с идентификатором {1} 
                                                    с {2} на {3} оператором - {4}.",
                                                                                   snSost,
                                                                                   trainID,
                                                                                   oldNumber, 
                                                                                   invNumber, 
                                                                                   userName
                                                   );
                EventData data = new EventData
                {
                    Data = messageText,
                    EventTime = DateTime.Now,
                    MsgId = 36/*Смена номера*/,
                    Source = "Пользователь ASKIN",
                    Text = messageText,
                    OpName = userName
                };

                AddEvent(ref data, context);

                context.Commit();
            }
        }

        #endregion

    }
}
