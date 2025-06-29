using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace nightreign_auto_storm_timer.ViewModels;
public partial class MainViewModel : ObservableObject
{
    private readonly GamePhaseManager _manager;

    [ObservableProperty]
    private string currentPhaseLabel = EnumHelper.GetDescription(GamePhase.Waiting);

    [ObservableProperty]
    private TimeSpan? timeRemaining;

    public string TimeRemainingFormatted =>
        TimeRemaining?.ToString(@"mm\:ss") ?? "--:--";

    [ObservableProperty]
    private bool isCompactMode;

    [ObservableProperty]
    private bool isMonitoring;

    private bool IsTimerRunning => _manager.IsTimerRunning;

    [ObservableProperty]
    private bool isShowButtons = true;

    public string TimerIconPath => IsTimerRunning
        ? "/Assets/Icons/pause.png"
        : "/Assets/Icons/play.png";

    public string ViewModeIconPath => IsCompactMode
        ? "/Assets/Icons/maximize.png"
        : "/Assets/Icons/minimize.png";

    public string ShowButtonsIconPath => IsShowButtons
        ? "/Assets/Icons/chevron-left.png"
        : "/Assets/Icons/chevron-right.png";

    public ObservableCollection<PhaseItemViewModel> PhaseList { get; } = new();

    public MainViewModel()
    {
        _manager = AppManager.Instance.GamePhaseManager;
        _manager.OnPhaseChanged += OnPhaseChanged;
        _manager.OnTimerTick += OnTimerTick;


        IsCompactMode = AppConfigManager.Instance.CurrentConfig.StartupViewMode == ViewMode.Compact;
        ToggleMonitoring();

        AppConfigManager.Instance.RegisterForUpdates(OnConfigUpdated);
        SetupPhase();
    }

    private void OnConfigUpdated(AppConfig config)
    {
        SetupPhase();
    }

    public void SetupPhase()
    {
        PhaseList.Clear();
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayOne, GamePhase.StormOne));
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayOne, GamePhase.StormOneShrinking));
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayOne, GamePhase.StormTwo));
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayOne, GamePhase.StormTwoShrinking));
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayOne, GamePhase.BossFight));

        PhaseList.Add(new PhaseItemViewModel(GameDay.DayTwo, GamePhase.StormOne));
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayTwo, GamePhase.StormOneShrinking));
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayTwo, GamePhase.StormTwo));
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayTwo, GamePhase.StormTwoShrinking));
        PhaseList.Add(new PhaseItemViewModel(GameDay.DayTwo, GamePhase.BossFight));

        PhaseList.Add(new PhaseItemViewModel(GameDay.DayThree, GamePhase.BossFight));
    }

    private void OnPhaseChanged(GameDay day, GamePhase phase)
    {
        CurrentPhaseLabel = $"{EnumHelper.FormatDay(day)} - {EnumHelper.FormatPhase(phase)}";

        if (PhaseList.Any(x => x.Day == day && x.Phase == phase))
        {
            foreach (var item in PhaseList)
                item.IsActive = item.Day == day && item.Phase == phase;
        }
    }

    private void OnTimerTick(int secondsRemaining)
    {
        TimeRemaining = TimeSpan.FromSeconds(secondsRemaining);
        OnPropertyChanged(nameof(TimeRemainingFormatted));
        OnPropertyChanged(nameof(IsTimerRunning));
        OnPropertyChanged(nameof(TimerIconPath));
    }

    [RelayCommand]
    public void ToggleMonitoring()
    {
        if (IsMonitoring)
        {
            _manager.Stop();
            IsMonitoring = false;
        }
        else
        {
            _manager.Start();
            IsMonitoring = true;
        }
    }

    [RelayCommand]
    public void ToggleCompactMode()
    {
        IsCompactMode = !IsCompactMode;
        OnPropertyChanged(nameof(ViewModeIconPath));
    }

    [RelayCommand]
    public void ToggleHelpWindow()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.ToggleHelpView);
    }

    [RelayCommand]
    public void OpenSettingsWindow()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.OpenSettingsWindow);
    }

    [RelayCommand]
    public void CloseApp()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.CloseApp);
    }

    [RelayCommand]
    public void PauseOrResumeTimer()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.PauseOrResumeTimer);
        OnPropertyChanged(nameof(IsTimerRunning));
        OnPropertyChanged(nameof(TimerIconPath));
    }

    [RelayCommand]
    public void ResetTimer()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.ResetTimer);
    }

    [RelayCommand]
    public void NextPhase()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.NextPhase);
    }

    [RelayCommand]
    public void PreviousPhase()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.PreviousPhase);
    }

    [RelayCommand]
    public void AlternateCompactMode()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.ToggleCompactMode);
    }

    [RelayCommand]
    public void ToggleShowButtons()
    {
        IsShowButtons = !IsShowButtons;
        OnPropertyChanged(nameof(ShowButtonsIconPath));
    }

    [RelayCommand]
    public void DebugScreenshot()
    {
        AppManager.Instance.ShortcutManager.ExecuteShortcut(ShortcutAction.DEBUG);
    }
}