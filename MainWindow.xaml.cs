using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

using SasFredonWPF.Helpers;
using SasFredonWPF.Services;
using SasFredonWPF.ViewModels;
using System.Diagnostics;

namespace SasFredonWPF
{
    public partial class MainWindow : Window
    {

        //private readonly InterfaceHelper _ih;
        private readonly ConvertService _c;
        private readonly FileService _s;

        // Main Functions
        public MainWindow()
        {
            InitializeComponent();

            //_ih = new InterfaceHelper(this);
            _c = new ConvertService(this);
            _s = new FileService(this);

            DataContext = new MainViewModel();

            TextBlockExcel.Text = Properties.Settings.Default.TextExcelPath;
            TextBlockPdf.Text = Properties.Settings.Default.TextPdfPath;

            if (DataContext is MainViewModel vm)
            {
                vm.Options.CompressZipChecked = Properties.Settings.Default.CompressZip;
                vm.Options.DeletePdfChecked = Properties.Settings.Default.DeletePDF;
                vm.Options.ArchiveXlsChecked = Properties.Settings.Default.ArchiveXLS;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.TextExcelPath = TextBlockExcel.Text;
            Properties.Settings.Default.TextPdfPath = TextBlockPdf.Text;

            if (DataContext is MainViewModel vm)
            {
                Properties.Settings.Default.CompressZip = vm.Options.CompressZipChecked;
                Properties.Settings.Default.DeletePDF = vm.Options.DeletePdfChecked;
                Properties.Settings.Default.ArchiveXLS = vm.Options.ArchiveXlsChecked;
            }

            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }

        ////////////////////////////////////////////////////////////////////////////////
        
        // Interface Functions
       
        // Facturation
        private void ButtonLoadXls_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderXls = new();
            if (openFolderXls.ShowDialog() == true)
                TextBlockExcel.Text = openFolderXls.FolderName;
        }

        private void ButtonLoadPdf_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderPdf = new();
            if (openFolderPdf.ShowDialog() == true)
                TextBlockPdf.Text = openFolderPdf.FolderName;
        }

        private async void Button_Conversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ButtonConversion.IsEnabled = false;
                ButtonConversion.Content = "Lancement...";
            
                await _c.Convert();

                if (DataContext is MainViewModel vm)
                {
                    // Compression
                    if (vm.Options.CompressZipChecked)
                        await _s.CompressFiles();

                    // Delete PDF
                    if (vm.Options.DeletePdfChecked)
                        await _s.DeletePdf();

                    // Delete XLS
                    if (vm.Options.ArchiveXlsChecked)
                        await _s.ArchiveXls();
                }

                ButtonConversion.IsEnabled = true;
                ButtonConversion.Content = "Lancer la conversion";
                ProgressBarText.Text = "TERMINÉ";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur dans la conversion: {ex.Message}");
            }
        }

        ////////////Frais km
        private void ListView_Frais_Loaded(object sender, RoutedEventArgs e)
        {
            // Somme des largeurs des deux premières colonnes
            const double usedWidth = 70 + 470;
            
            var totalWidth = ListViewFrais.ActualWidth;
            var compensation = SystemParameters.VerticalScrollBarWidth + 10;

            // Calcul de la largeur restante
            var remaining = totalWidth - usedWidth - compensation;

            if (!(remaining > 0)) return;
            var gridView = (GridView)ListViewFrais.View;
            gridView.Columns[2].Width = remaining;
        }

    }

}