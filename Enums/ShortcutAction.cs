using System.ComponentModel;

namespace nightreign_auto_storm_timer.Enums;
public enum ShortcutAction
{
    [Description("Close App")]
    CloseApp,
    [Description("Pause or Resume Timer")]
    PauseOrResumeTimer,
    [Description("Reset Timer")]
    ResetTimer,
    [Description("Go to previous phase")]
    PreviousPhase,
    [Description("Go to next phase")]
    NextPhase,
    [Description("Toggle compact mode")]
    ToggleCompactMode,
    [Description("Toggle help view")]
    ToggleHelpView,
    [Description("Open settings")]
    OpenSettingsWindow,
    [Description("Take DEBUG screenshot")]
    DEBUG,
    [Description("Reset status")]
    ResetStatus
}
