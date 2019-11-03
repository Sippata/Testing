using GalaSoft.MvvmLight;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace Testing.Model
{
    public class StudentInfo : ObservableObject
    {
        private string _firstname;

        public string Firstname
        {
            get => _firstname;
            set
            {
                Set(nameof(Firstname), ref _firstname, value);
                RaisePropertyChanged(nameof(FullName));
            }
        }

        private string _lastname;

        public string Lastname
        {
            get => _lastname;
            set
            {
                Set(nameof(Lastname), ref _lastname, value);
                RaisePropertyChanged(nameof(FullName));
            }
        }

        private string _group;

        public string Group
        {
            get => _group;
            set => Set(nameof(Group), ref _group, value);
        }

        private string _recordBookNum;

        public string RecordBookNum
        {
            get => _recordBookNum;
            set => Set(nameof(RecordBookNum), ref _recordBookNum, value);
        }
        public string FullName => $"{Firstname} {Lastname}";
    }
}