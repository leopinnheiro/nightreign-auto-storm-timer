using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Services;
using System.Drawing.Imaging;
using System.IO;

namespace nightreign_auto_storm_timer.Utils;

public static class DebugUtil
{
    private static string _debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DEBUG");

    static DebugUtil()
    {
        EnsureDebugDirectoryExists();
    }

    private static void EnsureDebugDirectoryExists()
    {
        Directory.CreateDirectory(_debugPath);
    }

    public static void DebugScreen()
    {
        var screenArea = AppConfigManager.Instance.GetScreenArea();
        var area = AppConfigManager.Instance.GetCaptureArea();

        OcrUtil.Initialize();
        using var screenshot = OcrUtil.Capture(screenArea);
        using var cropped = OcrUtil.Capture(area);
        using var processed = OcrUtil.ProcessImage(cropped);
        string recognizedText = OcrUtil.Recognize(processed);

        string filePrefix = DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss_");

        string debugScreenshotPath = Path.Combine(_debugPath, $"{filePrefix}screenshot.png");
        string debugCroppedPath = Path.Combine(_debugPath, $"{filePrefix}cropped.png");
        string debugProcessedPath = Path.Combine(_debugPath, $"{filePrefix}processed.png");
        string debugRecognizePath = Path.Combine(_debugPath, $"{filePrefix}recognize.txt");

        screenshot.Save(debugScreenshotPath, ImageFormat.Png);
        cropped.Save(debugCroppedPath, ImageFormat.Png);
        processed.Save(debugProcessedPath, ImageFormat.Png);
        File.WriteAllText(debugRecognizePath, recognizedText);

        LogService.LogDebugArtifacts(
            debugScreenshotPath,
            debugCroppedPath,
            debugProcessedPath,
            debugRecognizePath,
            recognizedText
        );
    }
}
