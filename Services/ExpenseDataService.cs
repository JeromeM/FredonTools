using Microsoft.Data.Sqlite;
using SasFredonWPF.Models;

namespace SasFredonWPF.Services
{
    public class ExpenseDataService
    {
        private const string ConnectionString = "Data Source=frais.db";

        public ExpenseDataService()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = """
                              CREATE TABLE IF NOT EXISTS Expense (
                                          Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                          Date TEXT NOT NULL,
                                          Type TEXT NOT NULL
                                      )
                              """;

            cmd.ExecuteNonQuery();
        }

        public static List<ExpenseModel> GetMonthExpenses(int year, int month)
        {
            var expense = new List<ExpenseModel>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Date, Type FROM Expense WHERE strftime('%Y', Date) = $year AND strftime('%m', Date) = $month";
            cmd.Parameters.AddWithValue("$year", year.ToString());
            cmd.Parameters.AddWithValue("$month", month.ToString("D2"));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                expense.Add(new ExpenseModel
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    Type = reader.GetString(2)
                });
            }

            return expense;
        }

        public static void AddExpense(ExpenseModel expense)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Expense (Date, Type) VALUES ($date, $type)";
            cmd.Parameters.AddWithValue("$date", expense.Date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$type", expense.Type);
            cmd.ExecuteNonQuery();
        }

        public void DeleteExpense(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Expense WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateExpense(ExpenseModel expense)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Expense SET Date = $date, Type = $type WHERE Id = $id";
            cmd.Parameters.AddWithValue("$date", expense.Date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$type", expense.Type);
            cmd.Parameters.AddWithValue("$id", expense.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
