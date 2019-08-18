using System;
using System.IO;

namespace Testing.Model
{
    public class TestInfo
    {
        public FileInfo DbFileInfo { get; set; }
        public string TableName => Path.GetFileNameWithoutExtension(DbFileInfo.Name);
        public TimeSpan TestTime { get; set; }
        public DateTime TestStartTime { get; set; }
        public int QuestionCount { get; set; }
        public int CorrectAnswersCount { get; set; } = 0;
        public double Rate => (double)CorrectAnswersCount / QuestionCount * 100;
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