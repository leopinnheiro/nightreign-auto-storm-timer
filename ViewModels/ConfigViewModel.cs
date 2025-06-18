using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Utils;
using nightreign_auto_storm_timer.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
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

    [ObservableProperty]
    private bool monitorIsHdr;
    [ObservableProperty]
    private bool isShowGuide = false;
    [ObservableProperty]
    private bool isDebug;

    [ObservableProperty]
    private int dayOneStormOne;
    [ObservableProperty]
    private int dayOneStormOneShrinking;
    [ObservableProperty]
    private int dayOneStormTwo;
    [ObservableProperty]
    private int dayOneStormTwoShrinking;
    [ObservableProperty]
    private int dayTwoStormOne;
    [ObservableProperty]
    private int dayTwoStormOneShrinking;
    [ObservableProperty]
    private int dayTwoStormTwo;
    [ObservableProperty]
    private int dayTwoStormTwoShrinking;

    [ObservableProperty]
    private int capturePositionX;
    [ObservableProperty]
    private int capturePositionY;
    [ObservableProperty]
    private int capturePositionWidth;
    [ObservableProperty]
    private int capturePositionHeight;

    private GuideWindow _guideWindow;
    public ConfigViewModel()
    {
        _guideWindow = new(this);
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

        DayOneStormOne = AppConfig.Current.DayOneStormOne;
        DayOneStormOneShrinking = AppConfig.Current.DayOneStormOneShrinking;
        DayOneStormTwo = AppConfig.Current.DayOneStormTwo;
        DayOneStormTwoShrinking = AppConfig.Current.DayOneStormTwoShrinking;

        DayTwoStormOne = AppConfig.Current.DayTwoStormOne;
        DayTwoStormOneShrinking = AppConfig.Current.DayTwoStormOneShrinking;
        DayTwoStormTwo = AppConfig.Current.DayTwoStormTwo;
        DayTwoStormTwoShrinking = AppConfig.Current.DayTwoStormTwoShrinking;

        MonitorIsHdr = AppConfig.Current.MonitorIsHdr;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(CapturePositionX)
                or nameof(CapturePositionY)
                or nameof(CapturePositionWidth)
                or nameof(CapturePositionHeight))
            {
                OnCapturePositionChanged();
            }
        };

        CapturePositionX = AppConfig.Current.CapturePositionX;
        CapturePositionY = AppConfig.Current.CapturePositionY;
        CapturePositionWidth = AppConfig.Current.CapturePositionWidth;
        CapturePositionHeight = AppConfig.Current.CapturePositionHeight;

        IsDebug = AppConfig.Current.Debug;
    }

    partial void OnSelectedMonitorChanged(Monitor? value)
    {
        if (value != null && !IsShowGuide)
        {
            Rectangle rect = ScreenUtil.SuggestCaptureArea(value.Screen);
            CapturePositionX = rect.X;
            CapturePositionY = rect.Y;
            CapturePositionWidth = rect.Width;
            CapturePositionHeight = rect.Height;
        }
    }

    public void SaveConfig()
    {
        AppConfig.Current.Fps = SelectedFps;
        AppConfig.Current.DayOneStormOne = DayOneStormOne;
        AppConfig.Current.DayOneStormOneShrinking = DayOneStormOneShrinking;
        AppConfig.Current.DayOneStormTwo = DayOneStormTwo;
        AppConfig.Current.DayOneStormTwoShrinking = DayOneStormTwoShrinking;

        AppConfig.Current.DayTwoStormOne = DayTwoStormOne;
        AppConfig.Current.DayTwoStormOneShrinking = DayTwoStormOneShrinking;
        AppConfig.Current.DayTwoStormTwo = DayTwoStormTwo;
        AppConfig.Current.DayTwoStormTwoShrinking = DayTwoStormTwoShrinking;

        AppConfig.Current.MonitorIsHdr = MonitorIsHdr;
        AppConfig.Current.Debug = IsDebug;
        AppConfig.Save();
    }

    partial void OnIsShowGuideChanged(bool value)
    {
        if (value)
        {
            OnCapturePositionChanged();
            _guideWindow.Show();
        }
        else
            _guideWindow.Hide();
    }

    private void OnCapturePositionChanged()
    {
        _guideWindow.Left = CapturePositionX;
        _guideWindow.Top = CapturePositionY;
        _guideWindow.Width = CapturePositionWidth;
        _guideWindow.Height = CapturePositionHeight;

        _guideWindow.Topmost = false;
        _guideWindow.Topmost = true;
    }

    public void OnGuideWindowMoved(double x, double y, string monitorName)
    {
        CapturePositionX = (int)x;
        CapturePositionY = (int)y;
        SelectedMonitor = Monitors.FirstOrDefault(m => m.DeviceName == monitorName);
        Debug.WriteLine($"{CapturePositionX} - {CapturePositionY} - {monitorName}");
    }
}
