using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Testing.ViewModel
{
    class StringToFileInfo : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NLog.LogManager.GetCurrentClassLogger().Debug("Converter");
            NLog.LogManager.GetCurrentClassLogger().Debug(value is TextBox);
            string fileName = value as string;
            NLog.LogManager.GetCurrentClassLogger().Debug(fileName);
            if (fileName == null) return new FileInfo("");
            
            var fileInfo = new FileInfo(Path.Combine(
                ConfigurationManager.AppSettings["dbDir"], fileName));
            NLog.LogManager.GetCurrentClassLogger().Debug(fileInfo.FullName);
            return fileInfo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    class StringToTimeSpan : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (double.TryParse(value as string, out var num))
            {
                return TimeSpan.FromMinutes(num);
            }

            return TimeSpan.FromMinutes(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    class StringToCheckBox : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is KeyValuePair<string, bool> pair)
                return new CheckBox() {Content = pair.Key, };
            return new CheckBox() {Content = "Error"};
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CheckBox checkBox)
            {
                NLog.LogManager.GetCurrentClassLogger().Debug("ConvertBack");
                var isChecked = checkBox.IsChecked ?? false;
                return new KeyValuePair<string, bool>(checkBox.Content.ToString(), isChecked);
            }
            return "error";
        }
    }
    
    class StringsToCheckBoxes : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<KeyValuePair<string, bool>> list)
            {
                var resList = new List<CheckBox>();
                foreach (var pair in list)
                {
                    var checkBox = new CheckBox() {Content = pair.Key};
                    resList.Add(checkBox);
                }

                return resList;
            }

            return new List<CheckBox>()
            {
                new CheckBox() {Content = "Error"},
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<CheckBox> list)
            {
                var resList = new ObservableCollection<KeyValuePair<string, bool>>();
                foreach (var checkBox in list)
                {
                    var isChecked = checkBox.IsChecked ?? false;
                    var pair = new KeyValuePair<string, bool>(checkBox.Content.ToString(), isChecked);
                    resList.Add(pair);
                }

                return resList;
            }
            return new ObservableCollection<KeyValuePair<string, bool>>()
            {
                new KeyValuePair<string, bool>("error", false),
            }; 
        }
    }
}