using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Models;
using System.Collections.ObjectModel;

namespace nightreign_auto_storm_timer.ViewModels;

public partial class HelpViewModel : ObservableObject
{
    public ObservableCollection<HelpItem> HelpList { get; } = [];

    public HelpViewModel()
    {
        AppConfigManager.Instance.RegisterForUpdates(OnConfigUpdated);
        ConfigFields(AppConfigManager.Instance.CurrentConfig.Shortcuts);
    }

    private void OnConfigUpdated(AppConfigNew config)
    {
        ConfigFields(config.Shortcuts);
    }

    public void ConfigFields(Dictionary<string, ShortcutConfig> shortcutsConfig)
    {
        HelpList.Clear();
        foreach (ShortcutAction action in Enum.GetValues(typeof(ShortcutAction)))
        {
            if (shortcutsConfig.TryGetValue(action.ToString(), out var shortcutConfig))
            {
                if (shortcutConfig.Enabled)
                {
                    HelpList.Add(new HelpItem(action, shortcutConfig));
                }
            }
        }
    }
}
