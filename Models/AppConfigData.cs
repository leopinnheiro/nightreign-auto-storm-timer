using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace nightreign_auto_storm_timer.Models;

public partial class AppConfigData : ObservableObject
{
    [ObservableProperty]
    private string selectedMonitorDeviceName = string.Empty;
    [ObservableProperty]
    private bool monitorIsHdr = false;

    [ObservableProperty]
    private bool debug = false;

    [ObservableProperty]
    private int fps = 20;

    [ObservableProperty]
    private int capturePositionX = 0;
    [ObservableProperty]
    private int capturePositionY = 0;
    [ObservableProperty]
    private int capturePositionWidth = 0;
    [ObservableProperty]
    private int capturePositionHeight = 0;

    [ObservableProperty]
    private int dayOneStormOne = 265;
    [ObservableProperty]
    private int dayOneStormOneShrinking = 174;
    [ObservableProperty]
    private int dayOneStormTwo = 206;
    [ObservableProperty]
    private int dayOneStormTwoShrinking = 174;

    [ObservableProperty]
    private int dayTwoStormOne = 265;
    [ObservableProperty]
    private int dayTwoStormOneShrinking = 177;
    [ObservableProperty]
    private int dayTwoStormTwo = 206;
    [ObservableProperty]
    private int dayTwoStormTwoShrinking = 177;

    public AppConfigData()
    {
        DefaultCaptureArea();   
    }

    private void DefaultCaptureArea()
    {
        if (CapturePositionX <= 0 || CapturePositionY <= 0 || CapturePositionWidth <= 0 || CapturePositionHeight <= 0)
        {
            Rectangle rect = ScreenUtil.SuggestCaptureArea();
            CapturePositionX = rect.X;
            CapturePositionY = rect.Y;
            CapturePositionWidth = rect.Width;
            CapturePositionHeight = rect.Height;
        }
    }

    public Screen GetScreen()
    {
        foreach (var screen in Screen.AllScreens)
        {
            if (screen.DeviceName == SelectedMonitorDeviceName)
                return screen;
        }

        return Screen.PrimaryScreen;
    }

    public Rectangle GetCaptureArea()
    {
        DefaultCaptureArea();
        return new Rectangle(CapturePositionX, CapturePositionY, CapturePositionWidth, CapturePositionHeight);
    }
}
