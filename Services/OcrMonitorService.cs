using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Utils;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace nightreign_auto_storm_timer.Services;
public class OcrMonitorService
{
    private readonly Action<GameDay> _onDayDetected;
    private CancellationTokenSource? _cts;

    private static readonly Dictionary<GameDay, List<string>> DayKeywords = new()
    {
        { GameDay.DayOne,   new() { "DIA I", "DAY I", "GIORNO I", "TAG I", "JOUR I", "DÍA I", "DZIEŃ I" } },
        { GameDay.DayTwo,   new() { "DIA II", "DAY II", "GIORNO II", "TAG II", "JOUR II", "DÍA II", "DZIEŃ II" } },
        { GameDay.DayThree, new() { "DIA III", "DAY III", "GIORNO III", "TAG III", "JOUR III", "DÍA III", "DZIEŃ III" } }
    };

    public OcrMonitorService(Action<GameDay> onDayDetected)
    {
        _onDayDetected = onDayDetected;
    }

    public void Start()
    {
        LogService.Debug($"[OcrMonitorService] Start");
        if (_cts != null) return;
        _cts = new CancellationTokenSource();
        Task.Run(() => RunOcrLoop(_cts.Token));
    }

    public void Stop()
    {
        LogService.Debug($"[OcrMonitorService] Stop");
        _cts?.Cancel();
        _cts = null;
    }

    private async Task RunOcrLoop(CancellationToken token)
    {
        int fps = AppConfigManager.Instance.CurrentConfig.CaptureConfig.FPS;
        int delay = fps > 0 ? 1000 / fps : 500;

        while (!token.IsCancellationRequested)
        {
            try
            {
                string text = PerformOcr();
                GameDay? detectedDay = ParseDayFromText(text);

                if (detectedDay.HasValue)
                {
                    _onDayDetected.Invoke(detectedDay.Value);
                }
            }
            catch (Exception ex)
            {
                LogService.Exception(ex, "[OcrMonitorService -> RunOcrLoop]");
            }

            await Task.Delay(delay, token);
        }
    }

    private string PerformOcr()
    {
        OcrUtil.Initialize();
        return OcrUtil.CaptureAndRecognize();
    }

    private GameDay? ParseDayFromText(string text)
    {
        foreach (var kvp in DayKeywords)
        {
            foreach (var keyword in kvp.Value)
            {
                var keywordFormatted = NormalizeText(keyword);
                var textFormatted = NormalizeText(text);
                LogService.Debug($"[OcrMonitorService] -> [ParseDayFromText] -> keyword: {keywordFormatted} | text: {textFormatted}");

                if (string.Equals(textFormatted, keywordFormatted, StringComparison.OrdinalIgnoreCase))
                    return kvp.Key;
            }
        }
        
        return null;
    }

    private string NormalizeText(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return Regex.Replace(sb.ToString(), @"\s+", "");
    }
}