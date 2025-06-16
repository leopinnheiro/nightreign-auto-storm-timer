using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Models;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Monitor = nightreign_auto_storm_timer.Models.Monitor;

namespace nightreign_auto_storm_timer.ViewModels;

public partial class ConfigViewModel : ObservableObject
{
    public ObservableCollection<Monitor> Monitors { get; } = [];
    public ObservableCollection<int> FpsList { get; } = [10, 20, 30, 40, 50, 60];

    [ObservableProperty]
    private int selectedFps;

    [ObservableProperty]
    private Monitor? selectedMonitor;

    public ConfigViewModel()
    {
        // FPS
        int savedFps = AppConfig.Current.Fps;
        if (!FpsList.Contains(savedFps))
        {
            FpsList.Insert(0, savedFps);
        }
        SelectedFps = savedFps;

        // Monitor
        foreach (var screen in Screen.AllScreens)
        {
            Monitor monitor = new(screen);
            Monitors.Add(monitor);

            if (monitor.DeviceName == AppConfig.Current.SelectedMonitorDeviceName)
                SelectedMonitor = monitor;
        }

        if (SelectedMonitor == null)
            SelectedMonitor = Monitors.FirstOrDefault(m => m.IsPrimary);
    }

    public void SaveConfig()
    {
        AppConfig.Current.Fps = SelectedFps;
        AppConfig.Current.SelectedMonitorDeviceName = SelectedMonitor.DeviceName;
        AppConfig.Save();
    }
}
