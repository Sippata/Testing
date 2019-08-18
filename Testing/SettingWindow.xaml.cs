using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Testing.Model;

namespace Testing
{
    public partial class SettingWindow : Window
    {
        private readonly Config _config = new Config();
        public SettingWindow()
        {
            InitializeComponent();
            DbTextBox.Text = Path.GetFullPath(_config.Get("dbDir"));
            ImgTextBox.Text = Path.GetFullPath(_config.Get("imgDir"));
            ReportTextBox.Text = Path.GetFullPath(_config.Get("reportDir"));
            ExTextBox.Text = _config.Get("e");
            PassTextBox.Text = _config.Get("p");
            
            ChangeVisibility(false);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dirDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop
            };
            var res = dirDialog.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                if (sender is Button btn)
                {
                    var field = btn.Name;
                    var path = dirDialog.SelectedPath;
                    if(path == null)
                        return;
                    
                    switch (field)
                    {
                        case "Db":
                            _config.Set("dbDir" ,path);
                            DbTextBox.Text = path;
                            break;
                        case "Img":
                            _config.Set("imgDir" ,path);
                            DbTextBox.Text = path;
                            break;
                        case "Report":
                            _config.Set("reportDir" ,path);
                            DbTextBox.Text = path;
                            break;
                    }
                }
            }
        }

        private void ChangeVisibility(bool flag)
        {
            if (flag)
            {
                MainStackPanel.Visibility = Visibility.Visible;
                InnerStackPanel.Visibility = Visibility.Visible;
                PasswordField.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainStackPanel.Visibility = Visibility.Collapsed;
                InnerStackPanel.Visibility = Visibility.Collapsed;
                PasswordField.Visibility = Visibility.Visible;
            }
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            _config.Set("p", textBox?.Text);
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            _config.Save();
            Close();
        }
        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PasswordField_OnTextInput(object sender, EventArgs e)
        {
            if (PasswordBox.Password == _config.Get("p"))
                ChangeVisibility(true);
        }

        private void ExTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _config.Set("e", ExTextBox.Text);
        }
    }
    
    public class Config
    {
        private static readonly Configuration _config = 
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public string Get(string param)
        {
            return _config.AppSettings.Settings[param].Value;
        }

        public void Set(string param, string value)
        {
            _config.AppSettings.Settings[param].Value = value;
        }

        public Action Save => _config.Save;
    }
}