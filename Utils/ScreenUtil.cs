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

            int x = (int)(screenWidth * 0.4);
            int y = (int)(screenHeight * 0.5);

            int bottom = (int)(screenHeight * 0.62);
            int right = (int)(screenWidth * 0.6);

            int width = right - x;
            int height = bottom - y;

            return new Rectangle(x, y, width, height);
        }
    }
}
