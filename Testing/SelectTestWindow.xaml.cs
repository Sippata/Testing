using System.Windows;
using System.Windows.Controls;
using Testing.ViewModel;

namespace Testing
{
    public partial class SelectTestWindow : Window
    {
        private int _noOfErrorsOnScreen;

        public SelectTestWindow()
        {
            InitializeComponent();
            QuestionCountTextBox.Text = "";
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

            SendButton.IsEnabled = _noOfErrorsOnScreen <= 0;
        }
    }
}