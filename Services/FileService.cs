using System.IO.Compression;
using System.IO;

using SasFredonWPF.Helpers;

namespace SasFredonWPF.Services
{
    class FileService (MainWindow mainWindow)
    {
        // Reference to MainWindow to access controls
        private readonly MainWindow _mainWindow = mainWindow;
        private readonly InterfaceHelper IH = new(mainWindow);

        public async Task CompressFiles()
        {
            string zipFilename = $"Factures_{DateHelper.CurrentMonth}_{DateHelper.CurrentYear}.zip";

            string pdfPath = _mainWindow.TextBlockPdf.Text;
            string fullZipPath = Path.Combine(pdfPath, zipFilename);

            if (File.Exists(fullZipPath))
                File.Delete(fullZipPath);

            string[] pdfFiles = FileHelper.GetFiles(pdfPath, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                _mainWindow.ProgressBar_Text.Text = "Aucun fichier PDF trouvé";
                return;
            }

            IH.ResetProgressBar(pdfFiles.Length);

            await Task.Run(() =>
            {
                using var archive = ZipFile.Open(fullZipPath, ZipArchiveMode.Create);
                foreach (string pdfFilePath in pdfFiles)
                {
                    string pdfFileName = Path.GetFileName(pdfFilePath);
                    archive.CreateEntryFromFile(pdfFilePath, pdfFileName, CompressionLevel.Optimal);

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        IH.UpdateUI();
                        _mainWindow.ProgressBar_Text.Text = $"Compression du fichier {pdfFileName}";
                    });
                }
            });
        }

        public async Task DeletePDF()
        {
            string[] pdfFiles = FileHelper.GetFiles(_mainWindow.TextBlockPdf.Text, "*.pdf");
            await Task.Run(() => Array.ForEach(pdfFiles, File.Delete));
        }

        public async Task ArchiveXLS()
        {
            string xlsPath = _mainWindow.TextBlockExcel.Text;
            string archiveDirectory = Path.Combine(xlsPath, $"Archive_{DateHelper.CurrentMonth}_{DateHelper.CurrentYear}");

            if (!Directory.Exists(archiveDirectory))
                Directory.CreateDirectory(archiveDirectory);

            string[] xlsFiles = FileHelper.GetFiles(xlsPath, "*.xls");

            await Task.Run(() =>
            {
                foreach (string xlsFile in xlsFiles)
                {
                    string destination = Path.Combine(archiveDirectory, Path.GetFileName(xlsFile));

                    if (File.Exists(destination))
                        File.Delete(destination);

                    File.Move(xlsFile, destination);
                }
            });
        }

    }
}
