using SasFredonWPF.Models;
using SasFredonWPF.ViewModels;
using System.Windows;

namespace SasFredonWPF.Views
{
    
    public partial class EditExpenseWindow : Window
    {
        public ExpenseModel Expense { get; private set; }

        private ExpenseViewModel _viewModel;

        public EditExpenseWindow(ExpenseViewModel viewModel, ExpenseModel expense)
        {
            InitializeComponent();

            _viewModel = viewModel;
            Expense = new ExpenseModel
            {
                Id = expense.Id,
                Date = expense.Date,
                Type = expense.Type
            };

            DataContext = new ExpenseEditViewModel(viewModel.ExpenseTypes.ToList(), Expense);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ExpenseEditViewModel;
            if (vm is null || string.IsNullOrWhiteSpace(vm.SelectedType))
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            Expense.Type = vm.SelectedType;
            Expense.Date = vm.SelectedDate;
            DialogResult = true;
        }
    }
}
