using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Utils;

namespace nightreign_auto_storm_timer.ViewModels;

public partial class ShortcutItemViewModel : ObservableObject
{
    public ShortcutAction Action { get; }
    public string DisplayName => EnumHelper.GetDescription(Action);

    [ObservableProperty]
    private bool enabled;

    [NotifyPropertyChangedFor(nameof(DisplayText))]
    [ObservableProperty]
    private Shortcut shortcut;

    [NotifyPropertyChangedFor(nameof(DisplayText))]
    [ObservableProperty]
    private bool isCapturing;

    public bool CanToggleEnabled => true;
    //public bool CanToggleEnabled => Action != ShortcutAction.CloseApp;

    public string DisplayText =>
        IsCapturing ? "Press the shortcut..." : Shortcut.ToFormattedString();

    public ShortcutItemViewModel(ShortcutAction action, ShortcutConfig config)
    {
        Action = action;
        Enabled = config.Enabled;
        Shortcut = config.Shortcut;
    }

    public void UpdateConfig(ShortcutConfig config)
    {
        config.Enabled = Enabled;
        config.Shortcut = Shortcut;
    }

    public string ToFormattedString()
    {
        return Shortcut.ToFormattedString();
    }
}
