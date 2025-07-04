using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Managers;
using System.Collections.Concurrent;
using System.IO;

namespace nightreign_auto_storm_timer.Services;

public static class LogService
{
    private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.log");
    private static readonly BlockingCollection<string> _logQueue = new();
    private static readonly CancellationTokenSource _cancellationTokenSource = new();
    private static readonly long MaxFileSizeBytes = 1024 * 1024; // 1MB

    static LogService()
    {
        Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
    }

    private static void ProcessQueue()
    {
        try
        {
            foreach (var logEntry in _logQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
            {
                try
                {
                    EnsureLogFileSizeLimit();
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                    System.Diagnostics.Debug.WriteLine(logEntry);
                }
                catch
                {
                    // Silent IO error
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Silent canceled exception
        }
    }

    private static void EnsureLogFileSizeLimit()
    {
        var fileInfo = new FileInfo(_logFilePath);
        if (!fileInfo.Exists)
            return;

        if (fileInfo.Length <= MaxFileSizeBytes)
            return;

        try
        {
            var lines = File.ReadAllLines(_logFilePath).ToList();

            while (lines.Count > 0 && new FileInfo(_logFilePath).Length > MaxFileSizeBytes)
            {
                lines.RemoveAt(0);
                File.WriteAllLines(_logFilePath, lines);
            }
        }
        catch
        {
            // Silent errors to truncate log
        }
    }

    public static void Shutdown()
    {
        _logQueue.CompleteAdding();
        _cancellationTokenSource.Cancel();
    }

    private static void Log(string message, LogLevel level)
    {
        if (level == LogLevel.Debug && !AppConfigManager.Instance.CurrentConfig.Debug)
            return;

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string line = $"[{timestamp}] [{level}] {message}";
        _logQueue.Add(line);
    }

    public static void Info(string message) => Log(message, LogLevel.Info);
    public static void Warning(string message) => Log(message, LogLevel.Warning);
    public static void Error(string message) => Log(message, LogLevel.Error);
    public static void Debug(string message) => Log(message, LogLevel.Debug);

    public static void Exception(Exception ex, string? context = null)
    {
        string msg = context != null
            ? $"{context}: {ex.Message}\n{ex.StackTrace}"
            : $"{ex.Message}\n{ex.StackTrace}";

        Error(msg);
    }

    public static void LogDebugArtifacts(string screenshotPath, string croppedPath, string processedPath, string recognizePath, string recognizedText)
    {
        Debug("Debug files saved:");
        Debug($" - Screenshot: {screenshotPath}");
        Debug($" - Cropped:    {croppedPath}");
        Debug($" - Processed:  {processedPath}");
        Debug($" - OCR Result: {recognizePath}");
        Debug($"OCR Text: {recognizedText}");
    }
}
