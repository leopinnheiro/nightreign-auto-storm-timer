using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Utils;

namespace nightreign_auto_storm_timer.Models
{
    public class HelpItem
    {
        public ShortcutAction Action { get; }
        public string DisplayName => EnumHelper.GetDescription(Action);

        private Shortcut Shortcut;
        public string DisplayCommand => Shortcut.ToFormattedString();

        public HelpItem(ShortcutAction action, ShortcutConfig config)
        {
            Action = action;
            Shortcut = config.Shortcut;
        }
    }

}
