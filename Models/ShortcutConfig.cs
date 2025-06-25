namespace nightreign_auto_storm_timer.Models
{
    public class ShortcutConfig(Shortcut shortcut, bool enabled = true)
    {
        public bool Enabled { get; set; } = enabled;
        public Shortcut Shortcut { get; set; } = shortcut;
    }
}
