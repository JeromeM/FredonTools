using SasFredonWPF.Models;
using SasFredonWPF.ViewModels;
using System.Windows;

namespace SasFredonWPF.Views
{
    
    public partial class EditExpenseWindow
    {
        public ExpenseModel Expense { get; }

        public EditExpenseWindow(ExpenseViewModel viewModel, ExpenseModel expense)
        {
            InitializeComponent();
            
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
            if (DataContext is not ExpenseEditViewModel vm || string.IsNullOrWhiteSpace(vm.SelectedType))
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
