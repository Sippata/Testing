using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Testing.Model;

namespace Testing.ViewModel
{
    public class ResultVM
    {
        public string Fullname => _studentInfo.FullName;
        public string Group => _studentInfo.Group;
        public int Mark => _testInfo.Mark;

        private Test _test { get; }
        private TestInfo _testInfo { get; }
        private StudentInfo _studentInfo { get; }

        public ResultVM(Test test, TestInfo testInfo, StudentInfo studentInfo)
        {
            _test = test;
            _testInfo = testInfo;
            _studentInfo = studentInfo;
            
            Continue = new RelayCommand(ContinueMethod);
            Exit = new RelayCommand(ExitMethod);
        }
        
        public void ContinueMethod()
        {
            var wnd = new StudentInfoWindow();
            wnd.Show();
            foreach (Window window in Application.Current.Windows)
            {
                if(window is ResultWindow resultWindow)
                    resultWindow.Close();
            }
        }

        public void ExitMethod()
        {
            _test.WriteReport?.Wait();
            Application.Current.Shutdown();
        }
        
        public ICommand Continue { get;  }
        public ICommand Exit { get; }
    }
}