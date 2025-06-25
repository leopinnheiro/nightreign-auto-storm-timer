using nightreign_auto_storm_timer.Enums;
using System.ComponentModel;

namespace nightreign_auto_storm_timer.Models;

public class GameState : INotifyPropertyChanged
{
    private GameDay _currentDay = GameDay.None;
    private GamePhase _currentPhase = GamePhase.Waiting;
    private TimeSpan? _phaseDuration;
    private TimeSpan? _timeRemaining;

    public event PropertyChangedEventHandler? PropertyChanged;

    public GameDay CurrentDay
    {
        get => _currentDay;
        set
        {
            if (_currentDay != value)
            {
                _currentDay = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PhaseStatus));
            }
        }
    }

    public GamePhase CurrentPhase
    {
        get => _currentPhase;
        set
        {
            if (_currentPhase != value)
            {
                _currentPhase = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PhaseStatus));
                OnPropertyChanged(nameof(IsBossFight));
                OnPropertyChanged(nameof(IsWaiting));
            }
        }
    }

    public TimeSpan? PhaseDuration
    {
        get => _phaseDuration;
        set
        {
            if (_phaseDuration != value)
            {
                _phaseDuration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTimerRunning));
            }
        }
    }

    public TimeSpan? TimeRemaining
    {
        get => _timeRemaining;
        set
        {
            if (_timeRemaining != value)
            {
                _timeRemaining = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTimerRunning));
                OnPropertyChanged(nameof(TimeRemainingFormatted));
            }
        }
    }

    // True se timer está ativo e com duração válida
    public bool IsTimerRunning => PhaseDuration.HasValue && TimeRemaining.HasValue;

    // True se a fase atual é Boss Fight
    public bool IsBossFight => CurrentPhase == GamePhase.BossFight;

    // True se ainda está aguardando OCR detectar fase
    public bool IsWaiting => CurrentPhase == GamePhase.Waiting;

    // Status para exibir na UI (ex: "DayOne - StormOne" ou "Waiting...")
    public string PhaseStatus
    {
        get
        {
            if (IsWaiting) return "Waiting...";
            if (IsBossFight) return "Boss Fight";
            return $"{CurrentDay} - {CurrentPhase}";
        }
    }

    // Formata o tempo restante em mm:ss, ou 00:00 se nulo
    public string TimeRemainingFormatted =>
        TimeRemaining?.ToString(@"mm\:ss") ?? "00:00";

    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}

