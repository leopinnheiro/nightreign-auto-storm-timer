using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Utils;
using System.Diagnostics;

namespace nightreign_auto_storm_timer.ViewModels;

public partial class PhaseItemViewModel : ObservableObject
{
    [ObservableProperty]
    private GameDay day;

    [ObservableProperty]
    private GamePhase phase;

    [ObservableProperty]
    private bool isActive;

    public string Name => $"{EnumHelper.FormatDay(Day)} - {EnumHelper.FormatPhase(Phase)}";
    public string TimeFormatted
    {
        get
        {
            int seconds = AppConfigManager.Instance.CurrentConfig.GetDuration(Day, Phase);
            return seconds > 0
                ? TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss")
                : "";
        }
    }

    public PhaseItemViewModel(GameDay day, GamePhase phase)
    {
        Day = day;
        Phase = phase;
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(TimeFormatted));
    }
}
