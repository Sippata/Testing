using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Testing.Model;

namespace Testing.ViewModel
{
    public class StudentInfoVM : ViewModelBase
    {
        public string Firstname
        {
            get => _studentInfo.Firstname;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Поле не может быть пустым.");
                _studentInfo.Firstname = value;
                RaisePropertyChanged(nameof(Firstname));
            }
        }

        public string Lastname
        {
            get => _studentInfo.Lastname;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Поле не может быть пустым.");
                _studentInfo.Lastname = value;
                RaisePropertyChanged(nameof(Lastname));
            }
        }

        public string Group
        {
            get => _studentInfo.Group;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Поле не может быть пустым.");
                _studentInfo.Group = value;
                RaisePropertyChanged(nameof(Group));
            }
        }

        public string RecordBookNum
        {
            get => _studentInfo.RecordBookNum;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Поле не может быть пустым.");
                _studentInfo.RecordBookNum = value;
                RaisePropertyChanged(nameof(RecordBookNum));
            }
        }

        public bool IsExam => _testInfo.IsExam;

        private StudentInfo _studentInfo { get; }
        private TestInfo _testInfo { get; }
        private Test _test { get; }
        public StudentInfoVM(StudentInfo studentInfo, TestInfo testInfo, Test test)
        {
            _test = test;
            _studentInfo = studentInfo;
            _testInfo = testInfo;
            SendInfo = new RelayCommand(SendInfoMethod, () =>
            {
                return !(string.IsNullOrWhiteSpace(Firstname) ||
                        string.IsNullOrWhiteSpace(Lastname) ||
                        string.IsNullOrWhiteSpace(Group) ||
                        !(IsExam ^ string.IsNullOrWhiteSpace(RecordBookNum)));
            }, true);
        }

        public ICommand SendInfo { get; }

        private void SendInfoMethod()
        {
            _test.Start();
            foreach (Window window in Application.Current.Windows)
            {
                if(window is StudentInfoWindow studentInfoWindow)
                    studentInfoWindow.Close();
            }
        }
    }
}