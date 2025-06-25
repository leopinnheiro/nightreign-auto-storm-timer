using nightreign_auto_storm_timer.Enums;

namespace nightreign_auto_storm_timer.Models;

public class PhaseConfig
{
    public int StormOne { get; set; }
    public int StormOneShrinking { get; set; }
    public int StormTwo { get; set; }
    public int StormTwoShrinking { get; set; }

    public int GetDuration(GamePhase phase)
    {
        return phase switch
        {
            GamePhase.StormOne => StormOne,
            GamePhase.StormOneShrinking => StormOneShrinking,
            GamePhase.StormTwo => StormTwo,
            GamePhase.StormTwoShrinking => StormTwoShrinking,
            _ => 0
        };
    }
}