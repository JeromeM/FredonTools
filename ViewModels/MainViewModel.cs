using CommunityToolkit.Mvvm.ComponentModel;

namespace SasFredonWPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private OptionsViewModel options = new();

        [ObservableProperty]
        private ExpenseViewModel expense = new();
    }
}
