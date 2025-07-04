using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Services;
using System.Drawing;
using Tesseract;

namespace nightreign_auto_storm_timer.Utils;

public static class OcrUtil
{
    private static TesseractEngine? _engine;
    private static TesseractEngine? _engineDebug;

    public static void Initialize()
    {
        if (_engine != null && _engineDebug != null) return;

        using var stream = AssetLoader.LoadAssetStream(Asset.AgmenaTrainedData);
        if (stream != null)
        {
            _engine = TesseractResourceLoader.LoadEngineFromStream(stream, "agmena");
            _engineDebug = TesseractResourceLoader.LoadEngineFromStream(stream, "agmena");
        }
    }

    public static string CaptureAndRecognize()
    {
        Rectangle area = AppConfigManager.Instance.GetCaptureArea();
        using var cropped = Capture(area);
        using var processed = ProcessImage(cropped);

        return Recognize(processed);
    }

    public static Bitmap Capture(Rectangle area)
    {
        using var capturer = new ScreenCapturerSharpDX();
        var image = capturer.Capture(area);
        //Bitmap image = (Bitmap)Bitmap.FromFile("");
        //area = ScreenUtil.SuggestCaptureArea(image);
        //image = ImageUtil.CropImage(image, area);
        return image;
    }

    public static Bitmap ProcessImage(Bitmap image)
    {
        var processed = ImageProcessor.ProcessImage(image);
        return processed;
    }

    public static string Recognize(Bitmap image, bool debug = false)
    {
        if (_engine == null || _engineDebug == null) return "";

        var imageBytes = ImageProcessor.ImageToBytes(image);

        using var pix = Pix.LoadFromMemory(imageBytes);

        try
        {
            Page page = debug ? _engineDebug.Process(pix) : _engine.Process(pix);
            using (page)
            {
                string text = page.GetText().Trim().ToUpper();
                LogService.Debug($"OCR Text - {text}");
                return text;
            }
        }
        catch (InvalidOperationException)
        {
            return "InvalidOperationException";
        }

        
    }

    public static string RecognizeDebug(Bitmap image)
    {
        return Recognize(image, true);
    }
}

