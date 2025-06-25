using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Utils;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using Monitor = nightreign_auto_storm_timer.Models.Monitor;

namespace nightreign_auto_storm_timer.ViewModels;

public partial class CaptureConfigViewModel : ObservableObject
{
    [ObservableProperty]
    private int fps;
    [ObservableProperty]
    private int positionX;
    [ObservableProperty]
    private int positionY;
    [ObservableProperty]
    private int width;
    [ObservableProperty]
    private int height;

    [ObservableProperty]
    private Monitor? selectedMonitor;
    [ObservableProperty]
    private bool isShowGuide = false;

    public ObservableCollection<Monitor> Monitors { get; } = [];
    public ObservableCollection<int> FpsList { get; } = [10, 20, 30, 40, 50, 60];

    public CaptureConfigViewModel() {}

    public CaptureConfigViewModel(CaptureConfig config)
    {
        PositionX = config.PositionX;
        PositionY = config.PositionY;
        Width = config.Width;
        Height = config.Height;

        // FPS
        int savedFps = config.FPS;
        if (!FpsList.Contains(savedFps))
        {
            FpsList.Insert(0, savedFps);
        }
        Fps = savedFps;

        // Monitor
        Monitors.Clear();
        foreach (var screen in Screen.AllScreens)
        {
            Monitor monitor = new(screen);
            Monitors.Add(monitor);

            if (monitor.DeviceName == config.MonitorDeviceName)
                SelectedMonitor = monitor;
        }
        SelectedMonitor ??= Monitors.FirstOrDefault(m => m.IsPrimary);

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof (PositionX)
                or nameof(PositionY)
                or nameof(Width)
                or nameof(Height))
            {
                OnCapturePositionChanged();
            }
        };
    }

    partial void OnSelectedMonitorChanged(Monitor? value)
    {
        if (value != null && !IsShowGuide)
        {
            Rectangle rect = ScreenUtil.SuggestCaptureArea(value.Screen);
            PositionX = rect.X;
            PositionY = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }
    }

    partial void OnIsShowGuideChanged(bool value)
    {
        if (value)
        {
            OnCapturePositionChanged();
            AppManager.Instance.GuideWindow.Show();
        }
        else
            AppManager.Instance.GuideWindow.Hide();
    }

    private void OnCapturePositionChanged()
    {
        AppManager.Instance.GuideWindow.Left = PositionX;
        AppManager.Instance.GuideWindow.Top = PositionY;
        AppManager.Instance.GuideWindow.Width = Width;
        AppManager.Instance.GuideWindow.Height = Height;

        AppManager.Instance.GuideWindow.Topmost = false;
        AppManager.Instance.GuideWindow.Topmost = true;
    }

    public void OnGuideWindowMoved(double x, double y, string monitorName)
    {
        PositionX = (int)x;
        PositionY = (int)y;
        SelectedMonitor = Monitors.FirstOrDefault(m => m.DeviceName == monitorName);
    }
    public void OnGuideWindowResized(double width, double height)
    {
        Width = (int)width;
        Height = (int)height;
    }
    
    public CaptureConfig ToModel()
    {
        return new CaptureConfig
        {
            MonitorDeviceName = SelectedMonitor?.DeviceName ?? string.Empty,
            FPS = Fps,
            PositionX = PositionX,
            PositionY = PositionY,
            Width = Width,
            Height = Height,
        };
    }
}
