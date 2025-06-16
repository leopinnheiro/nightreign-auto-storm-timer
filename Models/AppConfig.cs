using System.IO;
using System.Text.Json;

namespace nightreign_auto_storm_timer.Models;

public static class AppConfig
{
    private static readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    public static AppConfigData Current { get; private set; } = new();

    public static event EventHandler? ConfigChanged;

    static AppConfig()
    {
        Current.PropertyChanged += Current_PropertyChanged;
    }

    private static void Current_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ConfigChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void Load()
    {
        if (File.Exists(_path))
        {
            try
            {
                var json = File.ReadAllText(_path);
                var data = JsonSerializer.Deserialize<AppConfigData>(json);
                if (data != null)
                {
                    // Remove event do antigo (só por segurança)
                    Current.PropertyChanged -= Current_PropertyChanged;

                    Current = data;

                    // Assina evento no novo objeto
                    Current.PropertyChanged += Current_PropertyChanged;
                }
            }
            catch
            {
                Current = new AppConfigData();
                Current.PropertyChanged += Current_PropertyChanged;
            }
        }
    }

    public static void Save()
    {
        var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_path, json);
    }
}