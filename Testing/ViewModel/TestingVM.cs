using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Testing.Model;

namespace Testing.ViewModel
{
    public class TestingVM : ViewModelBase
    {
        public string QuestionText => _test.CurrentQuestion.QuestionText;
        public List<CheckBox> Answers { get; set; } = new List<CheckBox>();
        public BitmapImage Image => _test.CurrentQuestion.Picture;
        public int CorrectAnswerCount
        {
            get => _testInfo.CorrectAnswersCount;
            set => _testInfo.CorrectAnswersCount = value;
        }

        public int QuestionCount => _testInfo.QuestionCount;

        public string Student => $"{_studentInfo.FullName} {_studentInfo.Group}";

        public string TimeText => $"{_test.Time.Minutes:d2}:{_test.Time.Seconds:d2}";

        public int PendingQuestionCount
        {
            get => _pendingQuestionCount;
            set
            {
                _pendingQuestionCount = value;
                RaisePropertyChanged(nameof(PendingQuestionCount));
            }
        }

        public int CorrectQuestionCount
        {
            get => _correctQuestionCount;
            set
            {
                _correctQuestionCount = value;
                RaisePropertyChanged(nameof(CorrectQuestionCount));
            }
        }

        public int WrongQuestionCount
        {
            get => _wrongQuestionCount;
            set
            {
                _wrongQuestionCount = value;
                RaisePropertyChanged(nameof(WrongQuestionCount));
            }
        }

        private StudentInfo _studentInfo;
        private TestInfo _testInfo;
        private Test _test;
        private int _pendingQuestionCount;
        private int _correctQuestionCount;
        private int _wrongQuestionCount;
        private byte _checkedItemCount;

        public TestingVM(StudentInfo studentInfo, TestInfo testInfo,  Test test)
        {
            _testInfo = testInfo;
            _studentInfo = studentInfo;
            _test = test;

            _test.PropertyChanged += TestOnPropertyChanged;
            _test.Timer.Tick += (sender, args) => RaisePropertyChanged(nameof(TimeText));
            _test.TimeOut += StopTest;
            _test.TestStarted += TestOnTestStarted;
            _studentInfo.PropertyChanged += StudentInfoOnPropertyChanged;
            _testInfo.PropertyChanged += TestInfoOnPropertyChanged;

            OpenSettings = new RelayCommand(() =>
            {
                var wnd = new SettingWindow();
                wnd.Show();
            }, () => _test.IsTestStopped);
            
            OpenTest = new RelayCommand(() =>
            {
                var wnd = new SelectTestWindow();
                wnd.Show();
            }, () => _test.IsTestStopped);
            
            SkipQuestion = new RelayCommand(_test.SkipQuestion, () => _test.CanMove);
            
            SendAnswer = new RelayCommand(SendAnswerMethod, () => _test.CanMove && _checkedItemCount > 0);

            Exit = new RelayCommand(() => Application.Current.Shutdown(), () => _test.IsTestStopped);
        }

        private void TestOnTestStarted()
        {
            CorrectQuestionCount = 0;
            WrongQuestionCount = 0;
        }

        private void SendAnswerMethod()
        {
            CheckAnswer();
            var isLast = !_test.NextQuestion();
            if(isLast)
            {
                StopTest();
            }
            else
            {
                PendingQuestionCount -= 1;
            }
        }

        private void StopTest()
        {
            _test.Stop();
            _test.WriteReport = new Report(_studentInfo, _testInfo).WriteAsync();
            new ResultWindow().Show();
            PendingQuestionCount = _testInfo.QuestionCount;
        }
        
        private void CheckAnswer()
        {
            bool hasMistake = false;
            foreach (var answer in Answers)
            {
                var text = answer.Content as TextBlock;
                bool isChecked = answer.IsChecked ?? false;
                bool isCorrect = _test.CurrentQuestion.IsCorrect(text?.Text, isChecked);
                if (isCorrect)
                    CorrectAnswerCount += 1;
                else
                    hasMistake = true;
            }

            if (hasMistake)
                WrongQuestionCount += 1;
            else
                CorrectQuestionCount += 1;
        }
        
        public ICommand SendAnswer { get; }
        public ICommand SkipQuestion { get; }
        public ICommand OpenTest { get; }
        public ICommand OpenSettings { get; }
        public ICommand Exit { get; }

        private void TestOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_test.CurrentQuestion):
                {
                    _checkedItemCount = 0;
                    RaisePropertyChanged(nameof(QuestionText));
                    RaisePropertyChanged(nameof(Image));
                    Answers = new List<CheckBox>(4);
                    foreach (var pair in _test.CurrentQuestion.AnswerPairs)
                    {
                        var cb = new CheckBox {Content = new TextBlock()
                        {
                            Text = pair.Key,
                            TextWrapping = TextWrapping.Wrap,
                        }, FontSize = 16, IsChecked = pair.Value};
                        cb.Checked += (o, args) => _checkedItemCount += 1;
                        cb.Unchecked += (o, args) => _checkedItemCount -= 1;
                        Answers.Add(cb);
                    }
                    RaisePropertyChanged(nameof(Answers));
                    break;
                }
            }
        }
        
        private void StudentInfoOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_studentInfo.Group):
                case nameof(_studentInfo.FullName):
                {
                    RaisePropertyChanged(nameof(Student));
                    break;
                }
            }
        }
        
        private void TestInfoOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_testInfo.AnswerCount):
                {
                    RaisePropertyChanged(nameof(QuestionCount));
                    break;
                }
                case nameof(_testInfo.QuestionCount):
                {
                    RaisePropertyChanged(nameof(PendingQuestionCount));
                    PendingQuestionCount = _testInfo.QuestionCount;
                    break;
                }
                case nameof(_testInfo.CorrectAnswersCount):
                {
                    RaisePropertyChanged(nameof(CorrectAnswerCount));
                    break;
                }
            }
        }
    }
}