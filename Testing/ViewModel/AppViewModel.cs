using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Testing.Model;

namespace Testing.ViewModel
{
    public partial class AppViewModel : INotifyPropertyChanged
    {
        private static Action _reload;
        private static Action _increaseCorrectAnswer;
        private static Action _increaseWrongAnswer;
        private static Action _clear;
        private static readonly bool[] IsTestInfo = new bool[3];
        private static readonly bool[] IsStudentInfo = new bool[4];
        private static Predicate<object> _isExam; 
        
        private static DispatcherTimer _timer;

        private static bool IsTestLoaded { get; set; }

        public AppViewModel()
        {
            _isExam = o => IsExam; 
            
            _clear = () =>
            {
                CorrectAnswersCount = 0;
                WrongAnswerCount = 0;
            };
            _increaseWrongAnswer = () =>
            {
                WrongAnswerCount += 1; 
            };
            _increaseCorrectAnswer = () =>
            {
                CorrectAnswersCount += 1;
            };
            _reload = () =>
            {
                OnPropertyChanged(nameof(Answers));
                OnPropertyChanged(nameof(QuestionText));
                OnPropertyChanged(nameof(Picture));
                OnPropertyChanged(nameof(RemainingQuestionCount));
            };
            
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timer.Tick += (sender, args) =>
            {
                var timeSpan = TestInfo.TestTime - (DateTime.Now - TestInfo.TestStartTime);
                TimeText = $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}";

                if (timeSpan.Minutes == 0 && timeSpan.Seconds <= 0)
                    StopTest();
            };
        }

        private static StudentInfo StudentInfo { get; } = new StudentInfo();
        private static TestInfo TestInfo { get; } = new TestInfo();
        private static Test Test { get; } = new Test();

        #region TestInfo

        public string DbFileName
        {
            get => TestInfo.DbFileInfo.Name;
            set
            {
                if(IsFileValid(value))
                {
                    var path = Path.Combine(
                        ConfigurationManager.AppSettings["dbDir"], value);
                    TestInfo.DbFileInfo = new FileInfo(path);
                    IsTestInfo[0] = true;
                }
                else
                {
                    IsTestInfo[0] = false;
                }
            }
        }
        
        public int QuestionsCount
        {
            get => TestInfo.QuestionCount;
            set
            {
                if (IsIntValid(value, nameof(QuestionsCount)))
                {
                    TestInfo.QuestionCount = value;
                    OnPropertyChanged(nameof(QuestionsCount));
                    
                    IsTestInfo[1] = true;
                }
                else
                {
                    IsTestInfo[1] = false;
                }
            }
        }


        public double TestTime
        {
            set
            {
                if (IsDoubleValid(value, nameof(TestTime)))
                {
                    var temp = value;
                    TestInfo.TestTime = TimeSpan.FromMinutes(temp);
                    IsTestInfo[2] = true;
                }
                else
                {
                    IsTestInfo[2] = false;
                }
            }
        }

        public bool IsExam
        {
            get => TestInfo.IsExam;
            set => TestInfo.IsExam = value;
        }

        public int Mark => TestInfo.Mark;

        #endregion // TestInfo

        #region StudentInfo

        public string FullName => $"{Firstname} {Lastname}";
        public string Firstname
        {
            get => StudentInfo.Firstname;
            set
            {
                if (IsStrValid(value, nameof(Firstname)))
                {
                    StudentInfo.Firstname = value;
                    IsStudentInfo[0] = true;
                }
                else
                {
                    IsStudentInfo[0] = false;
                }
            }
        }

        public string Lastname
        {
            get => StudentInfo.Lastname;
            set
            {
                if (IsStrValid(value, nameof(Lastname)))
                {
                    StudentInfo.Lastname = value;
                    IsStudentInfo[1] = true;
                }
                else
                {
                    IsStudentInfo[1] = false;
                }
            }
        }

        public string Group
        {
            get => StudentInfo.Grope;
            set
            {
                if (IsStrValid(value, nameof(Group)))
                {
                    StudentInfo.Grope = value;
                    IsStudentInfo[2] = true;
                }
                else
                {
                    IsStudentInfo[2] = false;
                }
            }
        }

        public string RecordBookNum
        {
            get => StudentInfo.RecordBookNum;
            set
            {
                if (IsStrValid(value, nameof(RecordBookNum)))
                {
                    StudentInfo.RecordBookNum = value;
                    IsStudentInfo[3] = true;
                }
                else
                {
                    IsStudentInfo[3] = false;
                }
            }
        }

        #endregion // StudentInfo

        #region Test

        public string QuestionText => Test.Question.QuestionText;

        public BitmapImage Picture => Test.Question.Picture;
        public List<CheckBox> Answers
        {
            get
            {
                var list = new List<CheckBox>();
                foreach (var pair in Test.Question.AnswerPairs)
                {
                    list.Add(new CheckBox{Content = pair.Key});
                }
                return list;
            }
        }

        public double CorrectAnswersCount
        {
            get => TestInfo.CorrectAnswersCount;
            set
            {
                TestInfo.CorrectAnswersCount = (int) value;
                OnPropertyChanged(nameof(CorrectAnswersCount));
            }
        }
        
        private double _wrongAnswerCount;

        public double WrongAnswerCount
        {
            get => _wrongAnswerCount;
            set
            {
                _wrongAnswerCount = value;
                OnPropertyChanged(nameof(WrongAnswerCount));
            }
        }
        
        private string _timeText = "00:00";
        public string TimeText
        {
            get => _timeText;
            set
            {
                _timeText = value;
                OnPropertyChanged(nameof(TimeText));
            }
        }

        public int RemainingQuestionCount => (int) (TestInfo.QuestionCount - CorrectAnswersCount - WrongAnswerCount);

        #endregion // Test

        #region Members

        private static void StopTest()
        {
            _timer.Stop();
            Test.Stop();
            
            var report = new Report(StudentInfo, TestInfo);
            report.Write(DateTime.Now);
            
            SystemSounds.Beep.Play();
            
            ResultWindow resultWindow = new ResultWindow();
            foreach (var window in Application.Current.Windows)
            {
                if (window is TestingWindow testingWindow)
                {
                    resultWindow.Owner = testingWindow;
                }
            }
            resultWindow.Show();

            _clear.Invoke();
        }

        private static bool CheckAnswer(ListBox listBox)
        {
            foreach (var item in listBox.Items)
            {
                var log = NLog.LogManager.GetCurrentClassLogger();
                log.Debug(item.ToString());
                if (item is CheckBox checkBox)
                {
                    var isCheckedTrue = checkBox.IsChecked == true;
                    var isCorrect = Test.Question.IsCorrect(checkBox.Content.ToString());
                    log.Debug($"Check:{isCheckedTrue}, Corr:{isCorrect}");
                    if (isCheckedTrue ^ isCorrect)
                        return false;
                }
            }

            return true;
        }
        
        #endregion // Members

        #region Commands
        public static RelayCommand CloseWindow { get; } = new RelayCommand(window =>
        {
            if (window is Window win)
                win.Close();
        });

        public static RelayCommand SendStudentInfo { get; } = new RelayCommand(window =>
        {
            Test.Start();
            _timer.Start();
            _reload.Invoke();
            CloseWindow.Execute(window);
        }, o =>
        {
            bool temp = IsStudentInfo[0] && IsStudentInfo[1] && IsStudentInfo[2];
            bool temp2 = !(IsStudentInfo[3] ^ _isExam(null));
            return temp && temp2;
        });

        public static RelayCommand SendTestInfo { get; } = new RelayCommand(window =>
        {
            Test.LoadTest(TestInfo);
            IsTestLoaded = true;
            var studentInfoWindow = new StudentInfoWindow();
            if (window is Window wnd)
                studentInfoWindow.Owner = wnd.Owner;
            studentInfoWindow.Show();
            CloseWindow.Execute(window);
        }, o => IsTestInfo.All(b => b));

        public static RelayCommand OpenTest { get; } = new RelayCommand(window =>
        {
            SelectTestWindow selectTestWindow = new SelectTestWindow();
            if (window is Window wnd)
                selectTestWindow.Owner = wnd;
            selectTestWindow.Show();
        }, o => !IsTestLoaded);

        public RelayCommand SendAnswer { get; } = new RelayCommand(listBox =>
        {
            if (listBox is ListBox list)
                if (CheckAnswer(list))
                    _increaseCorrectAnswer();
                else
                    _increaseWrongAnswer();
                
            
            if(Test.NextQuestion())
                _reload.Invoke();
            else
            {
                StopTest();
            }
        }, o => Test.CanMove);

        public static RelayCommand SkipQuestion { get; } = new RelayCommand(o =>
        {
            Test.SkipQuestion();
            _reload.Invoke();
        }, o => Test.CanMove);
        
        public static RelayCommand Continue { get; } = new RelayCommand(window =>
        {
            StudentInfoWindow studentInfoWindow = new StudentInfoWindow();
            studentInfoWindow.Show();
            CloseWindow.Execute(window);
        });
        
        public static RelayCommand Exit { get; } = new RelayCommand(window =>
        {
            if (window is Window wnd)
            {
                wnd.Owner?.Close();
                wnd.Close();
            }
        });
        
        public static RelayCommand Setting { get; } = new RelayCommand( window =>
        {
            var settingWindow = new SettingWindow();
            if (window is Window wnd)
            {
                settingWindow.Owner = wnd;
            }
            settingWindow.Show();
        });
        
    #endregion // Commands

        #region INotifyProprertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        
        #endregion

    }
}