using System.Collections.ObjectModel;

namespace SasFredonWPF.Models
{
    public class ExpenseLineModel
    {
        public int Day { get; set; }
        public string ExpenseText => string.Join("\n", ExpenseByDay.Select(f => f.Type));
        public ObservableCollection<ExpenseModel> ExpenseByDay { get; set; } = [];
    }
}
