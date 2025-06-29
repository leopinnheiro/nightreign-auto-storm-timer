using nightreign_auto_storm_timer.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace nightreign_auto_storm_timer.Models;

public class CaptureConfig
{
    public string MonitorDeviceName { get; set; }
    public int FPS { get; set; } = 30;
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public CaptureConfig()
    {
        var screen = Screen.PrimaryScreen!;
        MonitorDeviceName = screen.DeviceName;

        Rectangle rect = ScreenUtil.SuggestCaptureArea(screen);
        PositionX = rect.X;
        PositionY = rect.Y;
        Width = rect.Width;
        Height = rect.Height;
    }
}
