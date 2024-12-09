using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace FramesPlayer.ExportConfiguration
{
    
    public interface ISQLCommandExecuter
    {
        bool TestConnection(string connectionString);
        /// <summary>Выполнить запрос и получить на выходе список объектов стандартных типов (int, string, double)</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="queryText"></param>
        /// <returns></returns>
        List<T> ExecuteListResult<T>(string connectionString, string queryText);
    }

    public class SQLCommandExecuter : ISQLCommandExecuter
    {

        #region Члены ISQLCommandExecuter

        public bool TestConnection(string connectionString)
        {
            bool result = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) { conn.Open(); }
                return true;
            }
            catch { result = false; }
            return result;
        }

    
        public List<T> ExecuteListResult<T>(string connectionString, string queryText)
        {
            List<T> result = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(queryText, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                result.Add((T)reader[0]);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return result;
        }
        
        public SQLCommandExecuter() { }

        #endregion
    }

}
