using CommunityToolkit.Mvvm.ComponentModel;

namespace nightreign_auto_storm_timer.Models
{
    public partial class Phase : ObservableObject
    {
        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private int timeInSeconds;

        [ObservableProperty]
        private bool isActive;

        public bool IsVisibleTime => TimeInSeconds > 0;

        public string TimeFormatted =>
            TimeInSeconds > 0 ? TimeSpan.FromSeconds(TimeInSeconds).ToString(@"mm\:ss") : "";
    }
}
