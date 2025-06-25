using CommunityToolkit.Mvvm.ComponentModel;

namespace SasFredonWPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private OptionsViewModel _options = new();

        [ObservableProperty]
        private ExpenseViewModel _expense = new();
    }
}
