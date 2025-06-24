using CommunityToolkit.Mvvm.ComponentModel;
using SasFredonWPF.Models;
using System.Collections.ObjectModel;

namespace SasFredonWPF.ViewModels
{
    public partial class ExpenseEditViewModel : ObservableObject
    {
        public ObservableCollection<string> ExpenseTypes { get; }

        [ObservableProperty]
        private string selectedType;

        [ObservableProperty]
        private DateTime selectedDate;

        public ExpenseEditViewModel(List<string> types, ExpenseModel model)
        {
            ExpenseTypes = new ObservableCollection<string>(types);
            SelectedType = model.Type;
            SelectedDate = model.Date;
        }
    }
}
