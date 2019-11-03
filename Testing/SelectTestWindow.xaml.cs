using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Testing.ViewModel;

namespace Testing
{
    public partial class SelectTestWindow : Window
    {
        private int _noOfErrorsOnScreen;

        public SelectTestWindow()
        {
            InitializeComponent();
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        }

        private void SelectTestWindow_OnError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                _noOfErrorsOnScreen++;
            }
            else
            {
                _noOfErrorsOnScreen--;
            }
            
            if(SendButton != null)
                SendButton.IsEnabled = _noOfErrorsOnScreen <= 0;
        }
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}