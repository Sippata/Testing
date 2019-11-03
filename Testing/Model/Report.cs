using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Testing.Model
{
    public class Report
    {
        private readonly StudentInfo _studentInfo;
        private readonly TestInfo _testInfo;
        
        public Report(StudentInfo studentInfo, TestInfo testInfo)
        {
            _studentInfo = studentInfo;
            _testInfo = testInfo;
        }

        private bool IsDirExist()
        {
            return Directory.Exists(ConfigurationManager.AppSettings["reportDir"]);
        }

        private void CreateDir()
        {
            string reportDir = ConfigurationManager.AppSettings["reportDir"];
            var reportFullDir = reportDir.Length == 0
                ? AppDomain.CurrentDomain.BaseDirectory
                : Path.GetFullPath(reportDir);

            Directory.CreateDirectory(reportFullDir);
        }

        private List<object[]> GetTitle()
        {
            if (_testInfo.IsExam)
                return new List<object[]>()
                {
                    new object[]
                    {
                        "№ п/п",
                        "Группа",
                        "Фамилия",
                        "Имя",
                        "Номер зачетной книжки",
                        "Оценка",
                        "Рейтинг(%)",
                        "Кол-во правильных ответов",
                        "Кол-во неправильных ответов",
                        "Начало тестирования",
                        "Время тестирования"
                    },
                };
            else
            {
                return new List<object[]>()
                {
                    new object[]
                    {
                        "№ п/п",
                        "Группа",
                        "Фамилия",
                        "Имя",
                        "Оценка",
                        "Рейтинг(%)",
                        "Кол-во правильных ответов",
                        "Кол-во неправильных ответов",
                        "Начало тестирования",
                        "Время тестирования"
                    },
                };
            }
        }

        private List<object[]> GetData(int index)
        {
            var timeSpan = _testInfo.TestEndTime - _testInfo.TestStartTime;
            
            if(_testInfo.IsExam)
                return new List<object[]>()
                {
                    new []
                    {
                        index as object,
                        _studentInfo.Group,
                        _studentInfo.Firstname,
                        _studentInfo.Lastname,
                        _studentInfo.RecordBookNum,
                        _testInfo.Mark,
                        _testInfo.Rate,
                        _testInfo.CorrectAnswersCount,
                        _testInfo.AnswerCount - _testInfo.CorrectAnswersCount,
                        _testInfo.TestStartTime.ToString("t"),
                        $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}",
                    },
                } ;
            else
            {
                return new List<object[]>()
                {
                    new []
                    {
                        index as object,
                        _studentInfo.Group,
                        _studentInfo.Firstname,
                        _studentInfo.Lastname,
                        _testInfo.Mark,
                        _testInfo.Rate,
                        _testInfo.CorrectAnswersCount,
                        _testInfo.AnswerCount - _testInfo.CorrectAnswersCount,
                        _testInfo.TestStartTime.ToString("t"),
                        $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}",
                    },
                } ;
            }
        }

        private ExcelWorksheet GetWorksheet(ExcelPackage excel)
        {
            ExcelWorksheet worksheet;
            if(excel.Workbook.Worksheets.Count == 0)
            {
                worksheet = excel.Workbook.Worksheets.Add("Results");
                var headerRow = GetTitle();
                worksheet.Cells[1, 1, headerRow[0].Length, 1].LoadFromArrays(headerRow);
            }
            else
            {
                worksheet = excel.Workbook.Worksheets.First();
            }

            return worksheet;
        }

        private FileInfo GetFileInfo()
        {
            var testName = Path.GetFileNameWithoutExtension(_testInfo.DbFileInfo.Name);
            var extension = ConfigurationManager.AppSettings["e"];
            var today = DateTime.Today.ToString("d");
            var fileName = _testInfo.IsExam 
                ? $"Exam_{testName}_{today}{extension}" 
                : $"{testName}_{today}{extension}";
            var path = Path.GetFullPath(
                Path.Combine(ConfigurationManager.AppSettings["reportDir"], fileName));
            
            return new FileInfo(path);
        }

        public void Write()
        {
            var log = NLog.LogManager.GetCurrentClassLogger();
            
            if(!IsDirExist())
                CreateDir();
            
            using (var excel = new ExcelPackage(GetFileInfo(),
                ConfigurationManager.AppSettings["p"]))
            {
                ExcelWorksheet worksheet = GetWorksheet(excel);
                
                int index = 1;
                while (worksheet.GetValue(index, 1) != null)
                {
                    index += 1;
                }

                var insertRow = GetData(index - 1);
                worksheet.Cells[index, 1, index, insertRow[0].Length].LoadFromArrays(insertRow);

                excel.Encryption.Algorithm = EncryptionAlgorithm.AES256;
                excel.Encryption.Password = ConfigurationManager.AppSettings["p"];

                excel.Save();
            }
        }

        public Task WriteAsync()
        {
            return Task.Factory.StartNew(Write);
        }
    }
}