using System.IO.Compression;
using System.IO;
using System.Windows;
using SasFredonWPF.Helpers;

namespace SasFredonWPF.Services
{
    internal class FileService (MainWindow mainWindow)
    {
        // Reference to MainWindow to access controls
        private readonly MainWindow _mainWindow = mainWindow;
        private readonly InterfaceHelper _ih = new(mainWindow);

        public async Task CompressFiles()
        {
            var zipFilename = $"Factures_{DateHelper.CurrentMonth}_{DateHelper.CurrentYear}.zip";

            var pdfPath = _mainWindow.TextBlockPdf.Text;
            var fullZipPath = Path.Combine(pdfPath, zipFilename);

            if (File.Exists(fullZipPath))
                File.Delete(fullZipPath);

            var pdfFiles = FileHelper.GetFiles(pdfPath, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                _mainWindow.ProgressBarText.Text = "Aucun fichier PDF trouvé";
                return;
            }

            _ih.ResetProgressBar(pdfFiles.Length);

            await Task.Run(() =>
            {
                using var archive = ZipFile.Open(fullZipPath, ZipArchiveMode.Create);
                foreach (var pdfFilePath in pdfFiles)
                {
                    var pdfFileName = Path.GetFileName(pdfFilePath);
                    archive.CreateEntryFromFile(pdfFilePath, pdfFileName, CompressionLevel.Optimal);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _ih.UpdateUi();
                        _mainWindow.ProgressBarText.Text = $"Compression du fichier {pdfFileName}";
                    });
                }
            });
        }

        public async Task DeletePdf()
        {
            var pdfFiles = FileHelper.GetFiles(_mainWindow.TextBlockPdf.Text, "*.pdf");
            await Task.Run(() => Array.ForEach(pdfFiles, File.Delete));
        }

        public async Task ArchiveXls()
        {
            var xlsPath = _mainWindow.TextBlockExcel.Text;
            var archiveDirectory = Path.Combine(xlsPath, $"Archive_{DateHelper.CurrentMonth}_{DateHelper.CurrentYear}");

            if (!Directory.Exists(archiveDirectory))
                Directory.CreateDirectory(archiveDirectory);

            var xlsFiles = FileHelper.GetFiles(xlsPath, "*.xls");

            await Task.Run(() =>
            {
                foreach (var xlsFile in xlsFiles)
                {
                    var destination = Path.Combine(archiveDirectory, Path.GetFileName(xlsFile));

                    if (File.Exists(destination))
                        File.Delete(destination);

                    File.Move(xlsFile, destination);
                }
            });
        }

    }
}
