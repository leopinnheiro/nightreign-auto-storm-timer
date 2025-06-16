namespace nightreign_auto_storm_timer.Models;

public class TextStateDetector
{
    public HashSet<string> Keywords { get; }
    public Action<string> OnDetected { get; set; }

    public TextStateDetector(IEnumerable<string> keywords, Action<string> onDetected)
    {
        Keywords = [.. keywords.Select(NormalizeText)];
        OnDetected = onDetected;
    }

    public void HandleOcr(string text)
    {
        if (IsMatch(text))
        {
            OnDetected?.Invoke(text);
        }
    }

    public bool IsMatch(string text)
    {
        return Keywords.Contains(NormalizeText(text));
    }

    private static string NormalizeText(string input) =>
        input.Trim().ToUpperInvariant();
}
