using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OfficeOpenXml;
using SasFredonWPF.Models;
using SasFredonWPF.Services;
using SasFredonWPF.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SasFredonWPF.ViewModels
{
    public partial class ExpenseViewModel : ObservableObject
    {
        private readonly ExpenseDataService _dataService = new();

        [ObservableProperty]
        private ObservableCollection<ExpenseLineModel> _lines;
        
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;
        
        [ObservableProperty]
        private string _displayedMonth = string.Empty;
        
        [ObservableProperty]
        private string _selectedType = "Trajet Zone 1A"; // Valeur par défaut

        public ObservableCollection<string> ExpenseTypes { get; } =
        [
            "Trajet Zone 1A",
            "Trajet Zone 1B",
            "Trajet Zone 2",
            "Trajet Zone 3",
            "Trajet Zone 4",
            "Trajet Zone 5",
            "Repas Restaurant"
        ];

        public RelayCommand<object> ExportToExcelCommand { get; }

        public ExpenseViewModel()
        {
            Lines = [];
            LoadExpenses();

            ExportToExcelCommand = new RelayCommand<object>(ExecuteExportToExcel);
        }

        private void ExecuteExportToExcel(object? parameter)
        {
            var toPrint = parameter switch
            {
                bool b => b,
                string s when bool.TryParse(s, out bool parsed) => parsed,
                _ => false
            };
            ExportToExcel(toPrint);
        }

        [RelayCommand]
        private void LoadExpenses()
        {
            Lines.Clear();
            var daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);

            // Initialiser les lignes vides
            for (var day = 1; day <= daysInMonth; day++)
            {
                Lines.Add(new ExpenseLineModel { Day = day });
            }

            var monthExpenses = ExpenseDataService.GetMonthExpenses(SelectedDate.Year, SelectedDate.Month);

            foreach (var expense in monthExpenses)
            {
                var day = expense.Date.Day;
                var ligne = Lines.FirstOrDefault(l => l.Day == day);
                ligne?.ExpenseByDay.Add(expense);
            }

            DisplayedMonth = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SelectedDate.ToString("MMMM yyyy", new CultureInfo("fr-FR")));
        }

        [RelayCommand]
        private void NextMonth()
        {
            SelectedDate = SelectedDate.AddMonths(1);
            LoadExpenses();
        }

        [RelayCommand]
        private void PreviousMonth()
        {
            SelectedDate = SelectedDate.AddMonths(-1);
            LoadExpenses();
        }

        [RelayCommand]
        private void AddSelectedExpense()
        {
            Debug.WriteLine($"Ajout : {SelectedType} à la date {SelectedDate:yyyy-MM-dd}");
            if (!string.IsNullOrWhiteSpace(SelectedType))
            {
                AddExpense(SelectedDate, SelectedType);
            }
        }

        private void AddExpense(DateTime date, string type)
        {
            var expense = new ExpenseModel { Date = date, Type = type };
            ExpenseDataService.AddExpense(expense);
            LoadExpenses();
        }

        [RelayCommand]
        private void DeleteExpense(int id)
        {
            var result = MessageBox.Show(
                "Voulez-vous vraiment supprimer ce frais ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes) return;
            _dataService.DeleteExpense(id);
            LoadExpenses();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            LoadExpenses();
        }

        [RelayCommand]
        private void EditExpense(ExpenseModel model)
        {
            var window = new EditExpenseWindow(this, model)
            {
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() != true) return;
            _dataService.UpdateExpense(window.Expense);
            LoadExpenses();
        }

        private void ExportToExcel(bool toPrint = false)
        {
            // Nombre de jours dans le mois
            var daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);

            // Récupérer les données depuis la base
            var monthExpenses = ExpenseDataService.GetMonthExpenses(SelectedDate.Year, SelectedDate.Month);

            // Mapping des types de frais vers les colonnes
            var typeToColumnMap = new Dictionary<string, int>
            {
                { "Trajet Zone 1A", 2 },    // Colonne B
                { "Trajet Zone 1B", 3 },    // Colonne C
                { "Trajet Zone 2", 4 },     // Colonne D
                { "Trajet Zone 3", 5 },     // Colonne E
                { "Trajet Zone 4", 6 },     // Colonne F
                { "Trajet Zone 5", 7 },     // Colonne G
                { "Repas Restaurant", 8 }   // Colonne H
            };

            // Totaux
            var colIntToName = new Dictionary<int, string>
            {
                { 1, "A" },
                { 2, "B" },
                { 3, "C" },
                { 4, "D" },
                { 5, "E" },
                { 6, "F" },
                { 7, "G" },
                { 8, "H" }
            };

            // Configuration EPPlus (nécessaire pour les versions récentes)
            ExcelPackage.License.SetNonCommercialPersonal("Jerome Meyer");

            try
            {

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Frais du mois");

                // Config taille des colonnes
                worksheet.Columns[1, 7].Width = 10;
                worksheet.Column(8).Width = 20;

                // En-têtes
                worksheet.Cells["A1"].Value = "SAS FREDON";
                worksheet.Cells["A2"].Value = "2 Rue du Petit Barry";
                worksheet.Cells["A3"].Value = "87380 SAINT GERMAIN LES BELLES";
                worksheet.Cells["A5"].Value = "Salarié : Vincent Fredon";
                worksheet.Cells["A1:A5"].Style.Font.Bold = true;

                // Titre
                worksheet.Cells["A7"].Value = "Tableau mensuel pour les indemnités de trajets et repas";
                using (var range = worksheet.Cells["A7:H7"])
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 14;
                }

                // Mois en cours
                worksheet.Cells["A9"].Value = $"Mois de {DisplayedMonth}";
                using (var range = worksheet.Cells["A9:H9"])
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;
                    range.Style.Font.Bold = true;
                }

                // En-têtes du tableau
                worksheet.Cells["A11"].Value = "DATES";
                worksheet.Cells["B11:G11"].Value = "Trajet";
                worksheet.Cells["H11"].Value = "Repas au restau";
                worksheet.Cells["B12"].Value = "Zone 1A";
                worksheet.Cells["C12"].Value = "Zone 1B";
                worksheet.Cells["D12"].Value = "Zone 2";
                worksheet.Cells["E12"].Value = "Zone 3";
                worksheet.Cells["F12"].Value = "Zone 4";
                worksheet.Cells["G12"].Value = "Zone 5";
                worksheet.Cells["H12"].Value = "Si Zones 1A et 1B";
                using (var range = worksheet.Cells["A11:H12"])
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Bordures du haut
                for (var col = 1; col <= 8; col++) // Colonnes A à H
                {
                    using var range = worksheet.Cells[11, col, 12, col];
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }

                // Ajout des jours du mois
                for (var day = 1; day <= daysInMonth; day++)
                {
                    var currentRow = day + 12; // +12 = nombre de lignes avant le premier jour
                    var ws = worksheet.Cells[$"A{currentRow}"];
                    ws.Value = day;
                    ws.Style.Font.Bold = true;
                    ws.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    for (var col = 1; col <= 8; col++)
                    {
                        worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    // On rajoute un 1 dans la cellule s'il y a eu une dépense
                    var dayExpenses = monthExpenses.Where(e => e.Date.Day == day).ToList();

                    foreach (var expense in dayExpenses)
                    {
                        if (!typeToColumnMap.TryGetValue(expense.Type, out var columnIndex)) continue;
                        worksheet.Cells[currentRow, columnIndex].Value = 1;
                        worksheet.Cells[currentRow, columnIndex].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }
                }

                // Totaux
                const int totalRowIndex = 45;
                worksheet.Cells[$"A{totalRowIndex}"].Value = "TOTAL";
                worksheet.Cells[$"A{totalRowIndex}"].Style.Font.Bold = true;
                for (var col = 1; col <= 8; col++)
                {
                    worksheet.Cells[totalRowIndex, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[totalRowIndex, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    if (col < 2) continue;
                    var currentCol = colIntToName[col];
                    const int fromRow = 13;
                    var toRow = fromRow + daysInMonth;
                    worksheet.Cells[totalRowIndex, col].Formula = $"SUM({currentCol}{fromRow}:{currentCol}{toRow})";
                }

                if (toPrint)
                {
                    try
                    {
                        var tempFilePath = Path.Combine(Path.GetTempPath(), $"Frais_{DisplayedMonth.Replace(" ", "_")}_{Guid.NewGuid()}.xlsx");
                        FileInfo tempFile = new(tempFilePath);
                        package.SaveAs(tempFile);

                        Microsoft.Office.Interop.Excel.Application excelApp = new()
                        {
                            Visible = false,
                        };
                        var workbook = excelApp.Workbooks.Open(tempFile.FullName);
                        Microsoft.Office.Interop.Excel.Worksheet worksheetExcel = workbook.Sheets[1];

                        // Mise en page
                        worksheetExcel.PageSetup.FitToPagesWide = 1; // Ajuster à 1 page en largeur
                        worksheetExcel.PageSetup.FitToPagesTall = 1; // Ajuster à 1 page en hauteur
                        worksheetExcel.PageSetup.LeftMargin = excelApp.InchesToPoints(0.25); // Marge gauche réduite (0,25 pouce)
                        worksheetExcel.PageSetup.RightMargin = excelApp.InchesToPoints(0.25); // Marge droite réduite
                        worksheetExcel.PageSetup.TopMargin = excelApp.InchesToPoints(0.25); // Marge haute réduite
                        worksheetExcel.PageSetup.BottomMargin = excelApp.InchesToPoints(0.25); // Marge basse réduite

                        PrintDialog printDialog = new();
                        if (printDialog.ShowDialog() == true)
                        {
                            worksheetExcel.PrintOut(
                                From: Type.Missing,
                                To: Type.Missing,
                                Copies: 1,
                                Preview: false,
                                ActivePrinter: printDialog.PrintQueue.FullName,
                                PrintToFile: false,
                                Collate: true
                            );
                        }

                        workbook.Close(false);
                        excelApp.Quit();

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheetExcel);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                        if (File.Exists(tempFilePath))
                        {
                            File.Delete(tempFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erreur lors de l'impression : {ex.Message}");
                        MessageBox.Show($"Une erreur s'est produite lors de l'impression :\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    SaveFileDialog saveFileDialog = new()
                    {
                        Filter = "Fichiers Excel (*.xlsx)|*.xlsx",
                        FileName = $"Frais_{DisplayedMonth.Replace(" ", "_")}.xlsx",
                        DefaultExt = ".xlsx"
                    };

                    if (saveFileDialog.ShowDialog() != true) return;
                    // Sauvegarder le fichier
                    FileInfo fileInfo = new(saveFileDialog.FileName);
                    package.SaveAs(fileInfo);

                    MessageBox.Show($"Fichier {fileInfo.FullName} exporté avec succès");
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du fichier Excel :\n{ex.Message}",
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

}
