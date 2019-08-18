namespace Testing.ViewModel
{
    public class ViewModelLocator
    {
        public static AppViewModel ViewModel { get; } = new AppViewModel();
        
        private static MainViewModel _main;
        public static MainViewModel Main => _main ?? (_main = new MainViewModel());
        
        public static void Cleanup()
        {
            _main.Cleanup();
        }
        
    }
}