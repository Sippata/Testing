using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Testing.Model
{
    public class AccessDb
    {
        private readonly OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder();
        public AccessDb(FileInfo dbFileInfo)
        {
            _builder.ConnectionString = $"Data Source={dbFileInfo.FullName}";
            _builder.Provider = ConfigurationManager.ConnectionStrings["AccessDb"].ProviderName;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>InvalidOperationException
        /// OleDbException</returns>
        public DataSet GetDataSet(string tableName)
        {
            DataSet ds = new DataSet();

            using (var connection = new OleDbConnection(_builder.ConnectionString))
            {
                connection.Open();

                string query = $"SELECT * FROM {tableName}";
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                adapter.Fill(ds);
                connection.Close();
            }
            
            return ds;
        }
    }
}