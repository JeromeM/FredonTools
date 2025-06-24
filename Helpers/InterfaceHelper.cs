using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SasFredonWPF.Helpers
{
    class InterfaceHelper(MainWindow mainWindow)
    {

        private readonly MainWindow _mainWindow = mainWindow;

        private int _dotCount;
        private int _currentProgress;

        private void AnimateProgressBar(double from, double to)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            _mainWindow.ProgressBar_Conversion.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        public void ResetProgressBar(int max)
        {
            _dotCount = 0;
            _currentProgress = 0;

            _mainWindow.ProgressBar_Conversion.BeginAnimation(ProgressBar.ValueProperty, null);
            _mainWindow.ProgressBar_Conversion.Value = 0;
            _mainWindow.ProgressBar_Conversion.Maximum = max;
            _mainWindow.ProgressBar_Text.Text = "";
        }

        public void UpdateUI()
        {
            _currentProgress++;
            AnimateProgressBar(_mainWindow.ProgressBar_Conversion.Value, _currentProgress);

            _dotCount = (_dotCount % 3) + 1;
            _mainWindow.Button_Conversion.Content = new string('●', _dotCount);
        }

    }
}
