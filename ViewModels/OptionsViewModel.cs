using CommunityToolkit.Mvvm.ComponentModel;

namespace SasFredonWPF.ViewModels
{
    public partial class OptionsViewModel : ObservableObject
    {
        public DateTime SelectedDate { get; set; } = DateTime.Now;

        [ObservableProperty]
        private bool deletePDFChecked;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DeletePDFEnabled))]
        private bool compressZipChecked;

        [ObservableProperty]
        private bool archiveXLSChecked;

        public bool DeletePDFEnabled => CompressZipChecked;

        partial void OnCompressZipCheckedChanged(bool value)
        {
            if (!value && DeletePDFChecked)
            {
                DeletePDFChecked = false;
            }
        }
    }
}
