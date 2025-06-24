namespace SasFredonWPF.Models
{
    public class ExpenseModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
