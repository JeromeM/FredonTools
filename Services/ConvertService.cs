using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using SasFredonWPF.Helpers;

namespace SasFredonWPF.Services
{
    class ConvertService(MainWindow mainWindow)
    {
        private readonly MainWindow _mainWindow = mainWindow;
        private readonly InterfaceHelper IH = new(mainWindow);

        public async Task Convert()
        {

            string sourceFolder = _mainWindow.TextBlockExcel.Text;
            string destinationFolder = _mainWindow.TextBlockPdf.Text;

            string[] xlsFiles = FileHelper.GetFiles(sourceFolder, "*.xls");

            if (xlsFiles.Length == 0)
            {
                _mainWindow.ProgressBar_Text.Text = "Aucun fichier PDF trouvé";
                return;
            }

            _mainWindow.Button_Conversion.IsEnabled = false;

            IH.ResetProgressBar(xlsFiles.Length);

            await Task.Run(() =>
            {

                var excelApp = new Excel.Application
                {
                    Visible = false,
                    ScreenUpdating = false,
                    DisplayAlerts = false
                };

                try
                {
                    foreach (string xlsFullPath in xlsFiles)
                    {
                        var workbook = excelApp.Workbooks.Open(xlsFullPath);

                        string xlsFilename = Path.GetFileName(xlsFullPath);
                        string pdfFilename = Path.ChangeExtension(xlsFilename, ".pdf");
                        string pdfFullPath = Path.Combine(destinationFolder, pdfFilename);

                        workbook.ExportAsFixedFormat2(Excel.XlFixedFormatType.xlTypePDF, pdfFullPath, Excel.XlFixedFormatQuality.xlQualityStandard, IncludeDocProperties: true);

                        workbook.Close(false);

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            IH.UpdateUI();
                            _mainWindow.ProgressBar_Text.Text = $"Traitement du fichier {xlsFilename}";
                        });

                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _mainWindow.ProgressBar_Text.Text = $"Erreur {ex.Message}";
                    });
                }
                finally
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                }
            });
        }
    }
}
