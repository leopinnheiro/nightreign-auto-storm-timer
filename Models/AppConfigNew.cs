using nightreign_auto_storm_timer.Enums;

namespace nightreign_auto_storm_timer.Models;

public class AppConfigNew
{
    public bool Debug { get; set; } = true;
    public bool RememberWindowPosition { get; set; }
    public double WindowLeft { get; set; }
    public double WindowTop { get; set; }
    public ViewMode StartupViewMode { get; set; } = ViewMode.Compact;
    public bool DisableFeedbackSounds { get; set; } = true;

    public CaptureConfig CaptureConfig { get; set; } = new();
    public PhaseConfig PhasesDayOne { get; set; }
    public PhaseConfig PhasesDayTwo { get; set; }
    public Dictionary<string, ShortcutConfig> Shortcuts { get; set; } = [];

    public AppConfigNew()
    {
        PhasesDayOne = new PhaseConfig
        {
            StormOne = 270,
            StormOneShrinking = 177,
            StormTwo = 206,
            StormTwoShrinking = 177
        };

        PhasesDayTwo = new PhaseConfig
        {
            StormOne = 270,
            StormOneShrinking = 177,
            StormTwo = 206,
            StormTwoShrinking = 177
        };
    }

    public int GetDuration(GameDay day, GamePhase phase) => (day, phase) switch
    {
        (GameDay.DayOne, GamePhase.StormOne) => PhasesDayOne.StormOne,
        (GameDay.DayOne, GamePhase.StormOneShrinking) => PhasesDayOne.StormOneShrinking,
        (GameDay.DayOne, GamePhase.StormTwo) => PhasesDayOne.StormTwo,
        (GameDay.DayOne, GamePhase.StormTwoShrinking) => PhasesDayOne.StormTwoShrinking,

        (GameDay.DayTwo, GamePhase.StormOne) => PhasesDayTwo.StormOne,
        (GameDay.DayTwo, GamePhase.StormOneShrinking) => PhasesDayTwo.StormOneShrinking,
        (GameDay.DayTwo, GamePhase.StormTwo) => PhasesDayTwo.StormTwo,
        (GameDay.DayTwo, GamePhase.StormTwoShrinking) => PhasesDayTwo.StormTwoShrinking,

        _ => 0
    };
}
