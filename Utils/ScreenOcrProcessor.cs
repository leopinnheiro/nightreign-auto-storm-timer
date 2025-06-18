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
    private readonly List<TextStateDetector> _stateDetectors = [];

    public ScreenOcrProcessor(List<TextStateDetector>? stateDetectors)
    {
        using var stream = AssetLoader.LoadAssetStream(Asset.AgmenaTrainedData);
        if (stream != null)
            tesseractEngine = TesseractResourceLoader.LoadEngineFromStream(stream, "agmena");

        _fps = AppConfig.Current.Fps;
        if (stateDetectors != null)
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
                    using Bitmap fullScreenshot = ScreenUtil.CaptureCurrentScreen();
                    Rectangle dayArea = AppConfig.Current.GetCaptureArea();

                    using Bitmap cropped = ImageUtil.CropImage(fullScreenshot, dayArea);
                    using Bitmap processed = ImageProcessor.ProcessImage(cropped);

                    string text = RunTesseract(processed);

                    DebugUtil.SaveOcrProcessImages(fullScreenshot, cropped, processed);
                    DebugUtil.SaveOcrProcessText(text);

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

    public string RunTesseract(Bitmap img)
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
