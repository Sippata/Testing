using System.Windows;
using Testing.Model;

namespace Testing.ViewModel
{
    public class MainViewModel
    {
        public StudentInfo StudentInfo { get; } = new StudentInfo();
        public TestInfo TestInfo { get; } = new TestInfo();
        public Test Test { get; set; }
        public Question Question { get; set; }

        public void Cleanup()
        {
            
        }

        #region Commands

        public RelayCommand CloseWindow { get; } = new RelayCommand(window =>
        {
            if(window is Window win)
                win.Close();
        });
        
        
        
        #endregion
    }
}