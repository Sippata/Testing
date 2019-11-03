using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace Testing.Model
{
    public class Test : ObservableObject
    {
        public delegate void TimeOutHandler();

        public delegate void TestStartedHandler();
        public Task WriteReport { get; set; }
        public DispatcherTimer Timer { get; }

        public event TimeOutHandler TimeOut;
        public event TestStartedHandler TestStarted;
        public bool CanMove
        {
            get => _canMove;
            set
            {
                _canMove = value;
                RaisePropertyChanged(nameof(CanMove));
            }
        }

        private RandomIterator<Question> QuestionRndIter { get; set; }

        public Question CurrentQuestion
        {
            get => _currentQuestion;
            private set
            {
                _currentQuestion = value;
                RaisePropertyChanged(nameof(CurrentQuestion));
            }
        }

        public bool IsTestStopped
        {
            get => _isTestStopped;
            set
            {
                _isTestStopped = value;
                RaisePropertyChanged(nameof(IsTestStopped));
            }
        }

        private TimeSpan _time;

        public TimeSpan Time
        {
            get => _time;
            set => Set(nameof(Time), ref _time, value);
        }
        
        public List<Question> Questions { get; private set; }

        private readonly TestInfo _testInfo;
        private bool _canMove;
        private Question _currentQuestion = new Question();
        private bool _isTestStopped = true;


        public Test(TestInfo testInfo)
        {
            _testInfo = testInfo;
            
            Timer = new DispatcherTimer(){Interval = TimeSpan.FromSeconds(1)};
            Timer.Tick += TimerOnTick;
        }
        
        private void TimerOnTick(object sender, EventArgs e)
        {
            Time -= TimeSpan.FromSeconds(1);
            if (Time <= TimeSpan.FromSeconds(0))
            {
                TimeOut?.Invoke();
            }
        }
        
        public void LoadTest(List<Question> questions)
        {
            Questions = questions;
            QuestionRndIter = new RandomIterator<Question>(Questions);
        }

        public void Start()
        {
            WriteReport?.Wait();
            NewQuestionSet(_testInfo.QuestionCount);
            _testInfo.AnswerCount = QuestionRndIter.Sum(question => question.AnswerPairs.Count);
            NextQuestion();
            
            CanMove = true;
            IsTestStopped = false;
            
            _testInfo.TestStartTime = DateTime.Now;
            Time = _testInfo.TestTime;
            Timer.Start();

            _testInfo.CorrectAnswersCount = 0;
            
            TestStarted?.Invoke();
        }

        public void Stop()
        {
            _testInfo.TestEndTime = DateTime.Now;
            CanMove = false;
            IsTestStopped = true;
            Timer.Stop();
        }

        public bool NextQuestion()
        {
            if (!QuestionRndIter.MoveNext())
            {
                return false;
            }
            CurrentQuestion = QuestionRndIter.Current;
            CurrentQuestion?.ShakeAnswers();
            return true;
        }

        public void NewQuestionSet(int count)
        {
            QuestionRndIter.Refresh(count);
            QuestionRndIter.Reset();
        }

        public void SkipQuestion()
        {
            QuestionRndIter.Skip();
            CurrentQuestion = QuestionRndIter.Current;
        }
    }
}