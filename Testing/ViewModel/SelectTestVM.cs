using System;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Testing.Model;

namespace Testing.ViewModel
{
    public class SelectTestVM : ViewModelBase
    {
        public string DbFileName
        {
            get => Path.GetFileNameWithoutExtension(TestInfo.DbFileInfo?.Name);
            set
            {
                var path = Path.Combine(ConfigurationManager.AppSettings["dbDir"], $"{value}.mdb");
                var dbFileInfo = new FileInfo(path);
                if (dbFileInfo.Exists == false)
                {
                    throw new ArgumentException($"Файл '{dbFileInfo.Name}' не существует.");
                }

                TestInfo.DbFileInfo = dbFileInfo;
                RaisePropertyChanged(nameof(DbFileName));
            }
        }

        public int QuestionCount
        {
            get => TestInfo.QuestionCount;
            set
            {
                if(value <= 0)
                    throw new ArgumentException("Кол-во вопросов должно быть положительным числом.");
                
                TestInfo.QuestionCount = value;
                RaisePropertyChanged(nameof(QuestionCount));
            }
        }

        public int TestTime
        {
            get => TestInfo.TestTime.Minutes;
            set
            {
                if(value <= 0)
                    throw new ArgumentException("Время теста должно быть положительным числом.");
                
                TestInfo.TestTime = TimeSpan.FromMinutes(value);
                RaisePropertyChanged(nameof(TestTime));
            }
        }

        public bool IsExam
        {
            get => TestInfo.IsExam;
            set
            {
                if(value == IsExam)
                    return;

                TestInfo.IsExam = value;
                RaisePropertyChanged(nameof(IsExam));
            }
        }

        private TestInfo TestInfo { get; }
        private Test _test { get; }

        public SelectTestVM(TestInfo testInfo, Test test)
        {
            TestInfo = testInfo;
            _test = test;
            SendInfo = new RelayCommand(SendInfoMethod, 
                () => !string.IsNullOrWhiteSpace(DbFileName) && QuestionCount > 0 && TestTime > 0,
                true);
        }

        public ICommand SendInfo { get; }

        private void SendInfoMethod()
        {
            try
            {
                var db = new AccessDb(TestInfo.DbFileInfo);
                var questions = db.GetQuestions(TestInfo.TableName);
                _test.LoadTest(questions);
            }
            catch (OleDbException)
            {
                var mes = $"Ошибка при подключении к '{TestInfo.DbFileInfo.Name}'. " +
                          $"Проверьте корректность файла и повторите снова.";
                MessageBox.Show(mes, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (InvalidOperationException)
            {
                var mes = $"Ошибка при загрузке данных из '{TestInfo.DbFileInfo.Name}'. " +
                          $"Проверьте корректность имени файла и таблицы.";
                MessageBox.Show(mes, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            StudentInfoWindow wnd = new StudentInfoWindow();
            wnd.Show();
            foreach (Window window in Application.Current.Windows)
            {
                if(window is SelectTestWindow selectTestWindow)
                    selectTestWindow.Close();
            }
        }
    }
}