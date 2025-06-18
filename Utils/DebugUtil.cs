using nightreign_auto_storm_timer.Models;
using System.Drawing;
using System.IO;
using Path = System.IO.Path;

namespace nightreign_auto_storm_timer.Utils
{
    public static class DebugUtil
    {
        private static readonly string _DebugFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DEBUG");

        static DebugUtil()
        {
            Directory.CreateDirectory(_DebugFolder);
        }

        public static void SaveOcrProcessImages(Bitmap screenshot, Bitmap cropped, Bitmap processed)
        {
            if (!AppConfig.Current.Debug)
                return;

            screenshot.Save(Path.Combine(_DebugFolder, "ocr_screenshot.png"));
            cropped.Save(Path.Combine(_DebugFolder, "ocr_cropped.png"));
            processed.Save(Path.Combine(_DebugFolder, "ocr_processed.png"));
        }

        public static void SaveOcrProcessText(string text = "")
        {
            if (!AppConfig.Current.Debug)
                return;

            File.WriteAllText(Path.Combine(_DebugFolder, "ocr_text.txt"), text);
        }

        public static void SaveScreen()
        {
            if (!AppConfig.Current.Debug)
                return;

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");

            using Bitmap screenshot = ScreenUtil.CaptureCurrentScreen();
            Rectangle area = AppConfig.Current.GetCaptureArea();

            using Bitmap cropped = ImageUtil.CropImage(screenshot, area);
            using Bitmap processed = ImageProcessor.ProcessImage(cropped);

            screenshot.Save(Path.Combine(_DebugFolder, $"DEBUG_screenshot_{timestamp}.png"));
            cropped.Save(Path.Combine(_DebugFolder, $"DEBUG_cropped_{timestamp}.png"));
            processed.Save(Path.Combine(_DebugFolder, $"DEBUG_processed_{timestamp}.png"));

            var processor = new ScreenOcrProcessor(null);
            
            string text = processor.RunTesseract(processed);
            File.WriteAllText(Path.Combine(_DebugFolder, $"DEBUG_text_{timestamp}.txt"), text);

            using (Graphics g = Graphics.FromImage(screenshot))
            {
                using (Pen redPen = new Pen(Color.Purple, 3))
                {
                    Rectangle rect = AppConfig.Current.GetCaptureArea();
                    g.DrawRectangle(redPen, rect);
                }
            }
            screenshot.Save(Path.Combine(_DebugFolder, $"DEBUG_screenshot_area_{timestamp}.png"));
        }
    }
}
