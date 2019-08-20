using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using Testing.Model;

namespace Testing.ViewModel
{
    public partial class AppViewModel : IDataErrorInfo
    {
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();
        private const string BlankError = "Строка не может быть пустой";
        private const string FileExistError = "Не существет такого файла";
        private const string FileOpenError = "Невозможно открыть файл";
        private const string MsAccessError = "Файл не является базой данных MSAccess";
        private const string PositiveNumError = "Число должно быть положительным";
        

        public void AddError(string propertyName, string error)
        {
            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName] = error;
            }
        }

        // Removes the specified error from the errors collection if it is present. 
        public void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
            }
        }
        
        private bool IsStrValid(string str, string propName)
        {
            bool isValid;
            
            if (string.IsNullOrEmpty(str))
            {
                AddError(propName, BlankError);
                isValid = false;
            }
            else
            {
                isValid = true;
                RemoveError(propName);
            }

            return isValid;
        }

        private bool IsFileValid(string fileName)
        {
            var path = Path.GetFullPath(Path.Combine(
                ConfigurationManager.AppSettings["dbDir"], fileName));
            if (!File.Exists(path))
            {
                AddError(nameof(DbFileName), FileExistError);
                return false;
            }
            else
            {
                RemoveError(nameof(DbFileName));
            }

            if (!AccessDb.CanOpen(path, out string mes))
            {
                switch (mes)
                {
                    case "NotDb":
                        AddError(nameof(DbFileName), FileOpenError);
                        return false;
                    case "OpenFault":
                        AddError(nameof(DbFileName), MsAccessError);
                        return false;
                    default:
                        RemoveError(nameof(DbFileName));
                        RemoveError(nameof(DbFileName));
                        break;
                }
            }

            return true;
        }

        private bool IsIntValid(int value, string propName)
        {
            if (value < 1)
            {
                AddError(propName, PositiveNumError);
                return false;
            }
            
            RemoveError(propName);
            return true;
        }
        
        private bool IsDoubleValid(double value, string propName)
        {
            if (value <= 0)
            {
                AddError(propName, PositiveNumError);
                return false;
            }
            
            RemoveError(propName);
            return true;
        }

        #region IDataErrorInfo Members

        public string this[string columnName] => _errors.ContainsKey(columnName) ? _errors[columnName] : null;
        public string Error { get; }
        

        #endregion
    }
}