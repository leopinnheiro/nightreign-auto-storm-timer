using CommunityToolkit.Mvvm.ComponentModel;

namespace nightreign_auto_storm_timer.Models;

public partial class AppConfigData : ObservableObject
{
    [ObservableProperty]
    private string selectedMonitorDeviceName = string.Empty;

    [ObservableProperty]
    private int fps = 20;
}
