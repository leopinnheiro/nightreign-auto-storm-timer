using nightreign_auto_storm_timer.Utils;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Models;

public record Shortcut(Key Key, ModifierKeys Modifiers)
{
    public override string ToString()
    {
        return Modifiers != ModifierKeys.None
            ? $"{Modifiers} + {Key}"
            : Key.ToString();
    }

    public string ToFormattedString()
    {
        return ShortcutFormatter.FormatShortcut(this);
    }
}
