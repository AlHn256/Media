using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FramesPlayer.ExportConfiguration
{
    public class DBOperation
    {

        public string ConnectionString { get; private set; }
        ISQLCommandExecuter sqlCommandExecuter;

        public DBOperation(string connectionString)
        {
            sqlCommandExecuter = new SQLCommandExecuter();
            ConnectionString = connectionString;
        }
        
        public List<string> GetDataBaseList()
        {
            List<string> databaseList = new List<string>();
            string sqlQuery = @"select name from sys.databases
                                where name != 'master'
                                and name!='tempdb' and name!='model' and name!='msdb'";
            databaseList = sqlCommandExecuter.ExecuteListResult<string>(ConnectionString, sqlQuery);
            return databaseList;                                        
        }
    }
}
