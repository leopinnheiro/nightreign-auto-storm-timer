using System.Drawing;

namespace nightreign_auto_storm_timer.Utils
{
    public static class ImageUtil
    {
        public static Bitmap CropImage(Bitmap source, Rectangle area)
        {
            return source.Clone(area, source.PixelFormat);
        }
    }
}
