using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

using SasFredonWPF.Helpers;
using SasFredonWPF.Services;
using SasFredonWPF.ViewModels;

namespace SasFredonWPF
{
    public partial class MainWindow : Window
    {

        private readonly InterfaceHelper IH;
        private readonly ConvertService C;
        private readonly FileService S;

        // Main Functions
        public MainWindow()
        {
            InitializeComponent();

            IH = new InterfaceHelper(this);
            C = new ConvertService(this);
            S = new FileService(this);

            DataContext = new MainViewModel();

            TextBlockExcel.Text = Properties.Settings.Default.TextExcelPath;
            TextBlockPdf.Text = Properties.Settings.Default.TextPdfPath;

            if (DataContext is OptionsViewModel vm)
            {
                vm.CompressZipChecked = Properties.Settings.Default.CompressZip;
                vm.DeletePDFChecked = Properties.Settings.Default.DeletePDF;
                vm.ArchiveXLSChecked = Properties.Settings.Default.ArchiveXLS;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.TextExcelPath = TextBlockExcel.Text;
            Properties.Settings.Default.TextPdfPath = TextBlockPdf.Text;

            if (DataContext is OptionsViewModel vm)
            {
                Properties.Settings.Default.CompressZip = vm.CompressZipChecked;
                Properties.Settings.Default.DeletePDF = vm.DeletePDFChecked;
                Properties.Settings.Default.ArchiveXLS = vm.ArchiveXLSChecked;
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
            await C.Convert();

            if (DataContext is OptionsViewModel vm)
            {
                // Compression
                if (vm.CompressZipChecked)
                    await S.CompressFiles();

                // Delete PDF
                if (vm.DeletePDFChecked)
                    await S.DeletePDF();

                // Delete XLS
                if (vm.ArchiveXLSChecked)
                    await S.ArchiveXLS();
            }

            Button_Conversion.IsEnabled = true;
            Button_Conversion.Content = "Lancer la conversion";
            ProgressBar_Text.Text = "TERMINÉ";
        }

        ////////////Frais km
        private void ListView_Frais_Loaded(object sender, RoutedEventArgs e)
        {
            var totalWidth = ListView_Frais.ActualWidth;
            double compensation = SystemParameters.VerticalScrollBarWidth + 10;

            // Somme des largeurs des deux premières colonnes
            double usedWidth = 70 + 470;

            // Calcul de la largeur restante
            double remaining = totalWidth - usedWidth - compensation;

            if (remaining > 0)
            {
                var gridView = (GridView)ListView_Frais.View;
                gridView.Columns[2].Width = remaining;
            }
        }

    }

}