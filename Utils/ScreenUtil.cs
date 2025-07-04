using System.Drawing;
using System.Windows.Forms;

namespace nightreign_auto_storm_timer.Utils
{
    public static class ScreenUtil
    {
        public static Rectangle SuggestCaptureArea(Screen? screen = null)
        {
            screen ??= Screen.PrimaryScreen!;

            return Suggest(screen.Bounds.Width, screen.Bounds.Height);
        }

        public static Rectangle SuggestCaptureArea(Bitmap image)
        {
            return Suggest(image.Width, image.Height);
        }

        private static Rectangle Suggest(int baseWidth, int baseHeight)
        {
            int x = (int)(baseWidth * 0.36);
            int y = (int)(baseHeight * 0.45);

            int bottom = (int)(baseHeight * 0.65);
            int right = (int)(baseWidth * 0.65);

            int width = right - x;
            int height = bottom - y;

            return new Rectangle(x, y, width, height);
        }
    }
}
