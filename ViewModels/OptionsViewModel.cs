using CommunityToolkit.Mvvm.ComponentModel;

namespace SasFredonWPF.ViewModels
{
    public partial class OptionsViewModel : ObservableObject
    {
        public DateTime SelectedDate { get; set; } = DateTime.Now;

        [ObservableProperty]
        private bool _deletePdfChecked;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DeletePdfEnabled))]
        private bool _compressZipChecked;

        [ObservableProperty]
        private bool _archiveXlsChecked;

        public bool DeletePdfEnabled => CompressZipChecked;

        partial void OnCompressZipCheckedChanged(bool value)
        {
            if (!value && DeletePdfChecked)
            {
                DeletePdfChecked = false;
            }
        }
    }
}
