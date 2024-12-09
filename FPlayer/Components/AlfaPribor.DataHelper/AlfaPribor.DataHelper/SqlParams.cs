using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace AlfaPribor.DataHelper
{

    /// <summary>Класс связка строка выполнения - параметры запроса</summary>
    public class SqlParams
    {
        
        /// <summary>Конструктор класса</summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public SqlParams(string sql, List<DbParameter> parameters)
        {
            SQL = sql;
            Params = parameters;
        }

        /// <summary>Строка выполнения</summary>
        public string SQL {get; set;}

        /// <summary>Параметры в строке выполнения</summary>
        public List<DbParameter> Params { get; set; }
                     
    }
}
