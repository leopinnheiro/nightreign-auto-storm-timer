using nightreign_auto_storm_timer.Models;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using Tesseract;

namespace nightreign_auto_storm_timer.Utils;

public class ScreenOcrProcessor : IDisposable
{
    private TesseractEngine? tesseractEngine;
    private CancellationTokenSource? _cts;
    private int _fps;
    private readonly List<TextStateDetector> _stateDetectors;

    public ScreenOcrProcessor(List<TextStateDetector> stateDetectors)
    {
        using var stream = AssetLoader.LoadAssetStream(Asset.AgmenaTrainedData);
        if (stream != null)
            tesseractEngine = TesseractResourceLoader.LoadEngineFromStream(stream, "agmena");

        _fps = AppConfig.Current.Fps;
        _stateDetectors = stateDetectors;

        AppConfig.ConfigChanged += (s, e) =>
        {
            _fps = AppConfig.Current.Fps;
        };
    }

    public void Start()
    {
        Debug.WriteLine("ScreenOcrProcessor STARTED");
        if (_cts != null) return;

        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using Bitmap fullScreenshot = CaptureScreen();
                    Rectangle dayArea = CreateRelativeRect(fullScreenshot);

                    using Bitmap cropped = CropImage(fullScreenshot, dayArea);
                    using Bitmap processed = ImageProcessor.ProcessImage(cropped);

                    string text = RunTesseract(processed);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var detector in _stateDetectors)
                        {
                            detector.HandleOcr(text);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                await Task.Delay(1000 / _fps, token);
            }
        }, token);
    }

    public void Stop()
    {
        Debug.WriteLine("ScreenOcrProcessor STOPPED");
        _cts?.Cancel();
        _cts = null;
    }

    public void Dispose()
    {
        Stop();
    }

    private Bitmap CaptureScreen()
    {
        var screen = System.Windows.Forms.Screen.AllScreens
            .FirstOrDefault(s => s.DeviceName == AppConfig.Current.SelectedMonitorDeviceName);

        if (screen == null)
            screen = System.Windows.Forms.Screen.PrimaryScreen;

        var bounds = screen.Bounds;
        Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
        using Graphics g = Graphics.FromImage(bmp);
        g.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);
        return bmp;
    }

    private Bitmap CropImage(Bitmap source, Rectangle area)
    {
        return source.Clone(area, source.PixelFormat);
    }

    private Rectangle CreateRelativeRect(Bitmap image)
    {
        int height = image.Height;
        int width = image.Width;

        int top = (int)(height * 0.5);
        int bottom = (int)(height * 0.65);
        int left = (int)(width * 0.35);
        int right = (int)(width * 0.65);

        int rectWidth = right - left;
        int rectHeight = bottom - top;

        return new Rectangle(left, top, rectWidth, rectHeight);
    }

    private string RunTesseract(Bitmap img)
    {
        var imageBytes = ImageProcessor.ImageToBytes(img);

        if (tesseractEngine != null)
        {
            using var pix = Pix.LoadFromMemory(imageBytes);
            using var page = tesseractEngine.Process(pix);
            return page.GetText().Trim();
        }

        return "";
    }
}
