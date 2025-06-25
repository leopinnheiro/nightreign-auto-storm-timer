using nightreign_auto_storm_timer.Enums;
using System.ComponentModel;

namespace nightreign_auto_storm_timer.Utils;

public static class EnumHelper
{
    public static string GetDescription(Enum value)
    {
        var attr = value.GetType()
                        .GetField(value.ToString())?
                        .GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .FirstOrDefault() as DescriptionAttribute;

        return attr?.Description ?? value.ToString();
    }

    public static string FormatDay(GameDay day) => GetDescription(day);

    public static string FormatPhase(GamePhase phase) => GetDescription(phase);
}


