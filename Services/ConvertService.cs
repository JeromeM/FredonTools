using System.IO;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

using SasFredonWPF.Helpers;

namespace SasFredonWPF.Services
{
    internal class ConvertService(MainWindow mainWindow)
    {
        private readonly MainWindow _mainWindow = mainWindow;
        private readonly InterfaceHelper _ih = new(mainWindow);

        public async Task Convert()
        {

            var sourceFolder = _mainWindow.TextBlockExcel.Text;
            var destinationFolder = _mainWindow.TextBlockPdf.Text;

            var xlsFiles = FileHelper.GetFiles(sourceFolder, "*.xls");

            if (xlsFiles.Length == 0)
            {
                _mainWindow.ProgressBarText.Text = "Aucun fichier PDF trouvé";
                return;
            }

            _mainWindow.ButtonConversion.IsEnabled = false;

            _ih.ResetProgressBar(xlsFiles.Length);

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
                    foreach (var xlsFullPath in xlsFiles)
                    {
                        var workbook = excelApp.Workbooks.Open(xlsFullPath);

                        var xlsFilename = Path.GetFileName(xlsFullPath);
                        var pdfFilename = Path.ChangeExtension(xlsFilename, ".pdf");
                        var pdfFullPath = Path.Combine(destinationFolder, pdfFilename);

                        workbook.ExportAsFixedFormat2(Excel.XlFixedFormatType.xlTypePDF, pdfFullPath, Excel.XlFixedFormatQuality.xlQualityStandard, IncludeDocProperties: true);

                        workbook.Close(false);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _ih.UpdateUi();
                            _mainWindow.ProgressBarText.Text = $"Traitement du fichier {xlsFilename}";
                        });

                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _mainWindow.ProgressBarText.Text = $"Erreur {ex.Message}";
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
