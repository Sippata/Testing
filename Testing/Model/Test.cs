using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Testing.Model
{
    public class Test
    {
        public bool CanMove { get; set; }
        private DataSet DataSet { get; set; }
        private const int MaxAnswerCount = 6;
        private RandomIterator<DataRow> Rows { get; set; }
        public Question Question { get; } = new Question();
        public bool IsTestStopped { get; set; } = true;

        private TestInfo _testInfo;
        private void RefreshQuestion()
        {
            if(Rows.Current == default(DataRow))
                return;
            
            Question.QuestionText = Rows.Current["Question"].ToString();
            Question.SetPicture(Rows.Current["Picture"].ToString());
            
            Question.AnswerPairs.Clear();
            for (int i = 1; i <= MaxAnswerCount; i++)
            {
                string answer = Rows.Current[$"Option {i}"].ToString();
                bool isCorrect = Rows.Current[$"{i} Correct"].ToString() == "1";
                
                if(answer != "")
                    Question.AnswerPairs.Add(new KeyValuePair<string, bool>(answer, isCorrect));
            }
            Question.ShakeAnswers();
        }

        public bool IsAnswerCorrect(IEnumerable<string> answers)
        {
            return answers.Aggregate(true, (current, answer) => current & Question.IsCorrect(answer));
        }

        public void LoadTest(TestInfo testInfo)
        {
            _testInfo = testInfo;
            
            var db = new AccessDb(testInfo.DbFileInfo);
            DataSet = db.GetDataSet(testInfo.TableName);
            
            Rows = new RandomIterator<DataRow>(
                source: (IEnumerator<DataRow>)DataSet.Tables[0].Rows.GetEnumerator(),
                count: testInfo.QuestionCount);
            
            Rows.CurrentChanged += RefreshQuestion;
        }

        public void Start()
        {
            Rows.Reset();
            
            NewQuestionSet(_testInfo.QuestionCount);
            
            Rows.MoveNext();
            
            _testInfo.TestStartTime = DateTime.Now;

            CanMove = true;
            IsTestStopped = false;
        }

        public void Stop()
        {
            CanMove = false;
            IsTestStopped = true;
        }

        public bool NextQuestion()
        {
            return Rows.MoveNext();
        }

        public void NewQuestionSet(int count)
        {
            Rows.Refresh(count);
        }

        public void SkipQuestion()
        {
            Rows.Skip();
        }
    }
}