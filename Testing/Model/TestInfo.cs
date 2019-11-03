using System;
using System.IO;
using GalaSoft.MvvmLight;

namespace Testing.Model
{
    public class TestInfo : ObservableObject
    {
        public FileInfo DbFileInfo { get; set; }
        public string TableName => Path.GetFileNameWithoutExtension(DbFileInfo.Name);
        public TimeSpan TestTime { get; set; }
        public DateTime TestStartTime { get; set; }
        public DateTime TestEndTime { get; set; }
        private int _questionCount;

        public int QuestionCount
        {
            get => _questionCount;
            set => Set(nameof(QuestionCount), ref _questionCount, value);
        }

        public int CorrectAnswersCount
        {
            get => _correctAnswersCount;
            set => Set(nameof(CorrectAnswersCount), ref _correctAnswersCount, value);
        }

        private int _answerCount;
        private int _correctAnswersCount;

        public int AnswerCount
        {
            get => _answerCount;
            set => Set(nameof(AnswerCount), ref _answerCount, value);
        }
        public double Rate => Math.Round((double)CorrectAnswersCount / AnswerCount * 100);
        public bool IsExam { get; set; }
        public int Mark
        {
            get
            {
                if (Rate < 70)
                    return 2;
                if (Rate < 80)
                    return 3;
                return Rate < 90 ? 4 : 5;
            }
        }
    }
}