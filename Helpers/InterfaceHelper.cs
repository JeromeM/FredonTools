using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace SasFredonWPF.Helpers
{
    internal class InterfaceHelper(MainWindow mainWindow)
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
            _mainWindow.ProgressBarConversion.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        public void ResetProgressBar(int max)
        {
            _dotCount = 0;
            _currentProgress = 0;

            _mainWindow.ProgressBarConversion.BeginAnimation(RangeBase.ValueProperty, null);
            _mainWindow.ProgressBarConversion.Value = 0;
            _mainWindow.ProgressBarConversion.Maximum = max;
            _mainWindow.ProgressBarText.Text = "";
        }

        public void UpdateUi()
        {
            _currentProgress++;
            AnimateProgressBar(_mainWindow.ProgressBarConversion.Value, _currentProgress);

            _dotCount = (_dotCount % 3) + 1;
            _mainWindow.ButtonConversion.Content = new string('●', _dotCount);
        }

    }
}
