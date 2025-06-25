using nightreign_auto_storm_timer.Models;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Utils;

public static class ShortcutFormatter
{
    public static string FormatShortcut(Shortcut shortcut)
    {
        var parts = new List<string>();

        if (shortcut.Modifiers.HasFlag(ModifierKeys.Control))
            parts.Add("Ctrl");
        if (shortcut.Modifiers.HasFlag(ModifierKeys.Shift))
            parts.Add("Shift");
        if (shortcut.Modifiers.HasFlag(ModifierKeys.Alt))
            parts.Add("Alt");
        if (shortcut.Modifiers.HasFlag(ModifierKeys.Windows))
            parts.Add("Win");

        parts.Add(NormalizeKeyName(shortcut.Key));

        return string.Join(" + ", parts);
    }

    private static string NormalizeKeyName(Key key)
    {
        string name = key.ToString();

        return name switch
        {
            // Números do teclado principal
            "D0" => "0",
            "D1" => "1",
            "D2" => "2",
            "D3" => "3",
            "D4" => "4",
            "D5" => "5",
            "D6" => "6",
            "D7" => "7",
            "D8" => "8",
            "D9" => "9",

            // Teclas comuns
            "Return" => "Enter",
            "Escape" => "Esc",
            "Back" => "Backspace",
            "Capital" => "Caps Lock",
            "Next" => "Page Down",
            "Prior" => "Page Up",
            "Snapshot" => "Print Screen",

            // Teclas especiais (OEM)
            "OemPlus" => "+",
            "OemMinus" => "-",
            "OemComma" => ",",
            "OemPeriod" => ".",
            "OemQuestion" => "/",
            "Oem1" => ";",
            "Oem3" => "`",
            "Oem5" => "\\",
            "Oem6" => "]",
            "OemOpenBrackets" => "[",

            _ => name
        };
    }

}

