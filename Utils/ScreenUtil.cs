using nightreign_auto_storm_timer.Models;
using System.Drawing;
using System.Windows.Forms;

namespace nightreign_auto_storm_timer.Utils
{
    public static class ScreenUtil
    {
        public static Rectangle SuggestCaptureArea(Screen? screen = null)
        {
            screen ??= Screen.PrimaryScreen!;

            int screenWidth = screen.Bounds.Width;
            int screenHeight = screen.Bounds.Height;

            int x = (int)(screenWidth * 0.35);
            int y = (int)(screenHeight * 0.45);

            int bottom = (int)(screenHeight * 0.65);
            int right = (int)(screenWidth * 0.65);

            int width = right - x;
            int height = bottom - y;

            return new Rectangle(x, y, width, height);
        }

        public static Bitmap CaptureScreen(Screen screen)
        {
            var bounds = screen.Bounds;
            Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
            using Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            return bmp;
        }

        public static Bitmap CaptureCurrentScreen()
        {
            var screen = AppConfig.Current.GetScreen();
            return CaptureScreen(screen);
        }
    }
}
