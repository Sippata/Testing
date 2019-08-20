using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Testing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StudentInfoWindow
    {
        private int _noOfErrorsOnScreen;

        public StudentInfoWindow()
        {
            InitializeComponent();
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        }
        
        private void OnError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;

            SendButton.IsEnabled = _noOfErrorsOnScreen <= 0;
        }
    }
}