using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Models;
using System.Collections.ObjectModel;

namespace nightreign_auto_storm_timer.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public Action? RequestClose { get; set; }

    public ObservableCollection<ShortcutItemViewModel> FirstColumnShortcuts { get; } = [];
    public ObservableCollection<ShortcutItemViewModel> SecondColumnShortcuts { get; } = [];

    public PhaseConfigViewModel PhaseConfigDayOne { get; private set; } = new();
    public PhaseConfigViewModel PhaseConfigDayTwo { get; private set; } = new();

    public CaptureConfigViewModel Capture { get; private set; } = new();

    public ObservableCollection<ViewMode> ViewModes { get; } = [];

    [ObservableProperty]
    private bool isDebug;
    [ObservableProperty]
    private bool rememberWindowPosition;
    [ObservableProperty]
    private bool disableFeedbackSounds;
    [ObservableProperty]
    private ViewMode selectedViewMode;

    public SettingsViewModel()
    {
        AppConfigManager.Instance.RegisterForUpdates(OnConfigUpdated);

        // View Modes
        foreach (ViewMode viewMode in Enum.GetValues(typeof(ViewMode)))
        {
            ViewModes.Add(viewMode);
        }
    }

    [RelayCommand]
    private void Save()
    {
        SaveChanges();
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }

    private void OnConfigUpdated(AppConfig config)
    {
        ConfigFields();
    }

    public void ConfigFields()
    {
        var config = AppConfigManager.Instance.CurrentConfig;
        AppConfig(config);
        ShortcutsConfig(config.Shortcuts);
        CaptureConfig(config.CaptureConfig);
    }

    private void AppConfig(AppConfig config)
    {
        IsDebug = config.Debug;
        RememberWindowPosition = config.RememberWindowPosition;
        DisableFeedbackSounds = config.DisableFeedbackSounds;
        PhaseConfigDayOne = new PhaseConfigViewModel(config.PhasesDayOne);
        PhaseConfigDayTwo = new PhaseConfigViewModel(config.PhasesDayTwo);
        SelectedViewMode = config.StartupViewMode;
    }

    private void ShortcutsConfig(Dictionary<string, ShortcutConfig> shortcutsConfig)
    {
        FirstColumnShortcuts.Clear();
        SecondColumnShortcuts.Clear();
        List<ShortcutItemViewModel> shortcuts = [];

        foreach (ShortcutAction action in Enum.GetValues(typeof(ShortcutAction)))
        {
            if (shortcutsConfig.TryGetValue(action.ToString(), out var shortcutConfig))
            {
                shortcuts.Add(new ShortcutItemViewModel(action, shortcutConfig));
            }
        }

        for (int i = 0; i < shortcuts.Count; i++)
        {
            if (i % 2 == 0)
                FirstColumnShortcuts.Add(shortcuts[i]);
            else
                SecondColumnShortcuts.Add(shortcuts[i]);
        }
    }

    private void CaptureConfig(CaptureConfig config)
    {
        Capture = new CaptureConfigViewModel(config);
    }

    public void SaveChanges()
    {
        AppConfigManager.Instance.Update(config =>
        {
            SaveAppConfig(config);
            config.CaptureConfig = Capture.ToModel();
            SaveShortcutsConfig(config.Shortcuts);
        });
    }

    private void SaveAppConfig(AppConfig config)
    {
        config.Debug = IsDebug;
        config.RememberWindowPosition = RememberWindowPosition;
        config.DisableFeedbackSounds = DisableFeedbackSounds;
        config.StartupViewMode = SelectedViewMode;

        //Phases
        config.PhasesDayOne = PhaseConfigDayOne.ToModel();
        config.PhasesDayTwo = PhaseConfigDayTwo.ToModel();
    }

    private void SaveShortcutsConfig(Dictionary<string, ShortcutConfig> shortcutsConfig)
    {
        var shortcuts = FirstColumnShortcuts.Concat(SecondColumnShortcuts);
        foreach (var item in shortcuts)
        {
            if (!shortcutsConfig.ContainsKey(item.Action.ToString()))
                shortcutsConfig[item.Action.ToString()] = new ShortcutConfig(item.Shortcut, item.Enabled);

            item.UpdateConfig(shortcutsConfig[item.Action.ToString()]);
        }
    }

    [RelayCommand]
    private async Task CaptureShortcutAsync(ShortcutItemViewModel item)
    {
        item.IsCapturing = true;

        var shortcut = await AppManager.Instance.ShortcutManager.CaptureNextShortcutAsync();

        item.Shortcut = shortcut;
        item.IsCapturing = false;
    }

    [RelayCommand]
    private void Apply(string? parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter)) return;

        var parts = parameter.Split(':');
        if (parts.Length != 2) return;

        string type = parts[0];
        if (!double.TryParse(parts[1], System.Globalization.CultureInfo.InvariantCulture, out double value))
            return;

        switch (type)
        {
            case "add":
                PhaseConfigDayOne.StormOne += (int)value;
                PhaseConfigDayOne.StormOneShrinking += (int)value;
                PhaseConfigDayOne.StormTwo += (int)value;
                PhaseConfigDayOne.StormTwoShrinking += (int)value;

                PhaseConfigDayTwo.StormOne += (int)value;
                PhaseConfigDayTwo.StormOneShrinking += (int)value;
                PhaseConfigDayTwo.StormTwo += (int)value;
                PhaseConfigDayTwo.StormTwoShrinking += (int)value;
                break;

            case "mult":
                PhaseConfigDayOne.StormOne += (int)(PhaseConfigDayOne.StormOne * value);
                PhaseConfigDayOne.StormOneShrinking += (int)(PhaseConfigDayOne.StormOneShrinking * value);
                PhaseConfigDayOne.StormTwo += (int)(PhaseConfigDayOne.StormTwo * value);
                PhaseConfigDayOne.StormTwoShrinking += (int)(PhaseConfigDayOne.StormTwoShrinking * value);

                PhaseConfigDayTwo.StormOne += (int)(PhaseConfigDayTwo.StormOne * value);
                PhaseConfigDayTwo.StormOneShrinking += (int)(PhaseConfigDayTwo.StormOneShrinking * value);
                PhaseConfigDayTwo.StormTwo += (int)(PhaseConfigDayTwo.StormTwo * value);
                PhaseConfigDayTwo.StormTwoShrinking += (int)(PhaseConfigDayTwo.StormTwoShrinking * value);
                break;
        }
        NormalizeTimer();
    }

    private void NormalizeTimer()
    {
        PhaseConfigDayOne.StormOne = Math.Clamp(PhaseConfigDayOne.StormOne, 0, int.MaxValue);
        PhaseConfigDayOne.StormOneShrinking = Math.Clamp(PhaseConfigDayOne.StormOneShrinking, 0, int.MaxValue);
        PhaseConfigDayOne.StormTwo = Math.Clamp(PhaseConfigDayOne.StormTwo, 0, int.MaxValue);
        PhaseConfigDayOne.StormTwoShrinking = Math.Clamp(PhaseConfigDayOne.StormTwoShrinking, 0, int.MaxValue);

        PhaseConfigDayTwo.StormOne = Math.Clamp(PhaseConfigDayOne.StormOne, 0, int.MaxValue);
        PhaseConfigDayTwo.StormOneShrinking = Math.Clamp(PhaseConfigDayOne.StormOneShrinking, 0, int.MaxValue);
        PhaseConfigDayTwo.StormTwo = Math.Clamp(PhaseConfigDayOne.StormTwo, 0, int.MaxValue);
        PhaseConfigDayTwo.StormTwoShrinking = Math.Clamp(PhaseConfigDayOne.StormTwoShrinking, 0, int.MaxValue);
    }
}
