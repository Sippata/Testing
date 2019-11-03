using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Testing.Model
{
    public class Question
    {
        public string QuestionText { get; set; }
        public BitmapImage Picture { get; set; }
        public List<KeyValuePair<string, bool>> AnswerPairs { get; }

        public Question()
        {
            Picture = new BitmapImage();
            QuestionText = "";
            AnswerPairs = new List<KeyValuePair<string, bool>>();
        }
        
        public void SetPicture(string imgName)
        {
            var dirInfo = ConfigurationManager.AppSettings;
            var imgPath = Path.GetFullPath(Path.Combine(dirInfo["imgDir"], imgName));
            Picture = File.Exists(imgPath) ? BitmapFromUri(new Uri(imgPath)) : new BitmapImage();
        }
        
        private BitmapImage BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        public bool IsCorrect(string answer, bool isChecked)
        {
            foreach (var pair in AnswerPairs)
            {
                if (pair.Key == answer)
                {
                    return pair.Value == isChecked;
                }
            }

            return false;
        }

        public void ShakeAnswers()
        {
            var random = new Random();
            var n = AnswerPairs.Count;
            for (int i = n - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                
                var temp = AnswerPairs[j];
                AnswerPairs[j] = AnswerPairs[i];
                AnswerPairs[i] = temp;
            }
        }
    }
}