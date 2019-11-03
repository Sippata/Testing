using System;
using System.Collections.Generic;
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

        public List<Question> GetQuestions(string tableName)
        {
            var log = NLog.LogManager.GetCurrentClassLogger();
            
            List<Question> questions = new List<Question>();
            
            log.Debug($"Connectiong to {_builder.ConnectionString}");
            using (var connection = new OleDbConnection(_builder.ConnectionString))
            {
                log.Debug("Db connected");

                log.Debug("Opening db...");
                connection.Open();
                log.Debug("Db opened");
                
                string query = $"SELECT * FROM {tableName}";
                log.Debug($"Query = {query}");
                OleDbCommand command = new OleDbCommand(query, connection);
                OleDbDataReader reader = command.ExecuteReader();
                
                log.Debug("Reading from db...");
                while (reader.Read())
                {
                    Question question = new Question();

                    question.QuestionText = reader["Question"].ToString();
                    question.SetPicture(reader["Picture"].ToString());
                    for (int i = 1; i <= 6; i++)
                    {
                        string answer = reader[$"Option {i}"].ToString();
                        bool isCorrect = reader[$"{i} Correct"].ToString() == "1";
                
                        if(answer != "")
                        {
                            question.AnswerPairs.Add(new KeyValuePair<string, bool>(answer, isCorrect));
                        }
                    }
                    questions.Add(question);
                }
                log.Debug("Data was read...");
            }
            
            return questions;
        }
    }
}