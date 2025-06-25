using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Services;
using nightreign_auto_storm_timer.Utils;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Input;
using Shortcut = nightreign_auto_storm_timer.Models.Shortcut;

namespace nightreign_auto_storm_timer.Managers;

public class AppConfigManager
{
    public static AppConfigManager Instance { get; } = new();

    public AppConfigNew CurrentConfig { get; private set; } = new();

    private readonly List<Action<AppConfigNew>> _onConfigChanged = [];

    private readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    private AppConfigManager() { }

    public void Load()
    {
        if (File.Exists(ConfigPath))
        {
            try
            {
                var json = File.ReadAllText(ConfigPath);
                CurrentConfig = JsonSerializer.Deserialize<AppConfigNew>(json) ?? new AppConfigNew();
                LogService.Info("[Config] Loaded config file.");
            }
            catch (Exception ex)
            {
                LogService.Exception(ex, "[Config] Failed to load config file.");
                CurrentConfig = new AppConfigNew();
            }
        }

        ApplyDefaultsIfNeeded();
        NotifyListeners();
    }

    public void Save()
    {
        string? json = JsonSerializer.Serialize(CurrentConfig, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(ConfigPath, json);
        LogService.Info($"[Config] save config file in: {ConfigPath}");
    }

    public void Update(Action<AppConfigNew> updateAction)
    {
        updateAction(CurrentConfig);
        Save();
        NotifyListeners();
    }

    public void RegisterForUpdates(Action<AppConfigNew> callback)
    {
        _onConfigChanged.Add(callback);
    }

    private void NotifyListeners()
    {
        LogService.Debug("[Config] Notify Listeners.");
        foreach (var callback in _onConfigChanged)
        {
            callback(CurrentConfig);
        }
    }

    private void ApplyDefaultsIfNeeded()
    {
        LogService.Info("[Config] Apply defaults if needed.");
        var shortcuts = CurrentConfig.Shortcuts;

        void SetDefault(ShortcutAction action, Key key, ModifierKeys modifiers, bool enabled = true)
        {
            string name = ShortcutNames.GetName(action);
            if (!shortcuts.ContainsKey(name))
                shortcuts[name] = new ShortcutConfig(new Shortcut(key, modifiers), enabled);
        }

        SetDefault(ShortcutAction.CloseApp, Key.Q, ModifierKeys.Control);
        SetDefault(ShortcutAction.PauseOrResumeTimer, Key.F1, ModifierKeys.None);
        SetDefault(ShortcutAction.ResetTimer, Key.F2, ModifierKeys.None);
        SetDefault(ShortcutAction.PreviousPhase, Key.F3, ModifierKeys.None);
        SetDefault(ShortcutAction.NextPhase, Key.F4, ModifierKeys.None);
        SetDefault(ShortcutAction.ToggleCompactMode, Key.F5, ModifierKeys.None);
        SetDefault(ShortcutAction.ToggleHelpView, Key.F7, ModifierKeys.None);
        SetDefault(ShortcutAction.OpenSettingsWindow, Key.F8, ModifierKeys.None);
        SetDefault(ShortcutAction.DEBUG, Key.F9, ModifierKeys.None, false);

        CurrentConfig.WindowLeft = Math.Max(CurrentConfig.WindowLeft, 0);
        CurrentConfig.WindowTop = Math.Max(CurrentConfig.WindowTop, 0);
        CurrentConfig.CaptureConfig.FPS = Math.Max(CurrentConfig.CaptureConfig.FPS, 10);
    }

    public Rectangle GetCaptureArea()
    {
        var captureConfig = CurrentConfig.CaptureConfig;
        return new()
        {
            X = captureConfig.PositionX,
            Y = captureConfig.PositionY,
            Width = captureConfig.Width,
            Height = captureConfig.Height
        };
    }

    public Rectangle GetScreenArea()
    {
        Screen screen = GetScreen();
        return new()
        {
            X = screen.Bounds.Left,
            Y = screen.Bounds.Top,
            Width = screen.Bounds.Width,
            Height = screen.Bounds.Height
        };
    }

    public Screen GetScreen()
    {
        foreach (var screen in Screen.AllScreens)
        {
            if (screen.DeviceName == CurrentConfig.CaptureConfig.MonitorDeviceName)
                return screen;
        }

        return Screen.PrimaryScreen!;
    }

    public int GetScreenIndex()
    {
        for (int i = 0; i < Screen.AllScreens.Length; i++)
        {
            if (Screen.AllScreens[i].DeviceName == CurrentConfig.CaptureConfig.MonitorDeviceName)
                return i;
        }

        return 0;
    }
}

