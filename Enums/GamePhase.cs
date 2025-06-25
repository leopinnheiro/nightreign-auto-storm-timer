using System.ComponentModel;

namespace nightreign_auto_storm_timer.Enums;

public enum GamePhase
{
    [Description("Waiting")]
    Waiting,
    [Description("Storm One")]
    StormOne,
    [Description("Storm One Shrinking")]
    StormOneShrinking,
    [Description("Storm Two")]
    StormTwo,
    [Description("Storm Two Shrinking")]
    StormTwoShrinking,
    [Description("Boss Fight")]
    BossFight
}
