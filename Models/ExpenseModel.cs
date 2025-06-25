namespace SasFredonWPF.Models
{
    public class ExpenseModel
    {
        public int Id { get; init; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
