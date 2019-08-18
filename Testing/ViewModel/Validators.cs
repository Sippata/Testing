using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Testing.ViewModel
{
    public class BlankValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string str)
                if(str == string.Empty)
                    return new ValidationResult(false, "Строка не может быить пустой");
            return new ValidationResult(true, null);
        }
    }

    public class DbFileValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string fileName)
            {
                var path = Path.Combine(ConfigurationManager.AppSettings["dbDir"], fileName);
                if (!File.Exists(path))
                {
                    return new ValidationResult(false, "Файл не существует");
                }
            }
            
            return new ValidationResult(true, null);
        }
    }

    public class IntValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(int.TryParse(value.ToString(), out var result))
                return new ValidationResult(true, null);
            return new ValidationResult(false, "Can't convert to int");
        }
    }
    
    public class DoubleValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(double.TryParse(value.ToString(), out var result))
                return new ValidationResult(true, null);
            return new ValidationResult(false, "Can't convert to double");
        }
    }
}