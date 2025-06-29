using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Services;

namespace nightreign_auto_storm_timer.Managers;

public class GamePhaseManager
{
    public GameDay CurrentDay { get; private set; } = GameDay.None;
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Waiting;

    private readonly OcrMonitorService _ocrService;
    private readonly CustomTimer _timer;
    private readonly CustomTimer _timerToWaiting;

    public event Action<GameDay, GamePhase>? OnPhaseChanged;
    public event Action<int>? OnTimerTick;

    private int _timeLeft = 0;
    public bool IsTimerRunning { get; private set; } = false;

    private AppConfig _config = AppConfigManager.Instance.CurrentConfig;

    public GamePhaseManager()
    {
        _ocrService = new OcrMonitorService(OnOcrDetected);
        _timer = new CustomTimer(OnTimerFinished);
        _timer.OnTick += seconds =>
        {
            _timeLeft = seconds;
            OnTimerTick?.Invoke(seconds);
        };

        _timerToWaiting = new CustomTimer(OnTimerToWaitingFinished);
    }

    public void Start()
    {
        if (CurrentPhase == GamePhase.Waiting || CurrentPhase == GamePhase.BossFight)
            _ocrService.Start();
    }

    public void Stop()
    {
        _ocrService.Stop();
        _timer.Stop();
    }

    private void OnOcrDetected(GameDay detectedDay)
    {
        if (CurrentDay != detectedDay)
        {
            CurrentDay = detectedDay;

            if (CurrentDay == GameDay.DayThree)
                SetPhase(GamePhase.BossFight);
            else
                SetPhase(GamePhase.StormOne);
        }
    }

    private void SetPhase(GamePhase newPhase)
    {
        CurrentPhase = newPhase;
        OnPhaseChanged?.Invoke(CurrentDay, CurrentPhase);

        _ocrService.Stop();

        if (CurrentPhase == GamePhase.Waiting || CurrentPhase == GamePhase.BossFight)
        {
            IsTimerRunning = false;
            _timeLeft = 0;
            _timer.Stop();

            _ocrService.Start();
            OnTimerTick?.Invoke(_timeLeft);
        }
        else
        {
            int durationSeconds = _config.GetDuration(CurrentDay, CurrentPhase);
            if (durationSeconds > 0)
            {
                _timeLeft = durationSeconds;
                _timer.Start(durationSeconds);
                IsTimerRunning = true;
            }

            if (CurrentPhase == GamePhase.StormTwoShrinking)
                _ocrService.Start();
        }
        LogService.Debug($"Set new phase {CurrentPhase} - {CurrentDay} : seconds left {_timeLeft}");

        if (CurrentDay == GameDay.DayThree)
        {
            _timerToWaiting.Start(30);
        }
    }

    private void OnTimerFinished()
    {
        var (nextDay, nextPhase) = GetNextPhaseInfo();
        SetPhase(nextPhase);
    }

    public (GameDay day, GamePhase phase) GetNextPhaseInfo() => (CurrentDay, CurrentPhase) switch
    {
        (GameDay.None, GamePhase.Waiting) => (GameDay.DayOne, GamePhase.StormOne),

        (GameDay.DayOne, GamePhase.StormOne) => (GameDay.DayOne, GamePhase.StormOneShrinking),
        (GameDay.DayOne, GamePhase.StormOneShrinking) => (GameDay.DayOne, GamePhase.StormTwo),
        (GameDay.DayOne, GamePhase.StormTwo) => (GameDay.DayOne, GamePhase.StormTwoShrinking),
        (GameDay.DayOne, GamePhase.StormTwoShrinking) => (GameDay.DayOne, GamePhase.BossFight),
        (GameDay.DayOne, GamePhase.BossFight) => (GameDay.DayTwo, GamePhase.StormOne),

        (GameDay.DayTwo, GamePhase.StormOne) => (GameDay.DayTwo, GamePhase.StormOneShrinking),
        (GameDay.DayTwo, GamePhase.StormOneShrinking) => (GameDay.DayTwo, GamePhase.StormTwo),
        (GameDay.DayTwo, GamePhase.StormTwo) => (GameDay.DayTwo, GamePhase.StormTwoShrinking),
        (GameDay.DayTwo, GamePhase.StormTwoShrinking) => (GameDay.DayTwo, GamePhase.BossFight),
        (GameDay.DayTwo, GamePhase.BossFight) => (GameDay.DayThree, GamePhase.BossFight),

        _ => (CurrentDay, GamePhase.BossFight)
    };

    public (GameDay day, GamePhase phase) GetPreviousPhaseInfo() => (CurrentDay, CurrentPhase) switch
    {
        (GameDay.DayThree, GamePhase.BossFight) => (GameDay.DayTwo, GamePhase.BossFight),

        (GameDay.DayTwo, GamePhase.BossFight) => (GameDay.DayTwo, GamePhase.StormTwoShrinking),
        (GameDay.DayTwo, GamePhase.StormTwoShrinking) => (GameDay.DayTwo, GamePhase.StormTwo),
        (GameDay.DayTwo, GamePhase.StormTwo) => (GameDay.DayTwo, GamePhase.StormOneShrinking),
        (GameDay.DayTwo, GamePhase.StormOneShrinking) => (GameDay.DayTwo, GamePhase.StormOne),
        (GameDay.DayTwo, GamePhase.StormOne) => (GameDay.DayOne, GamePhase.BossFight),

        (GameDay.DayOne, GamePhase.BossFight) => (GameDay.DayOne, GamePhase.StormTwoShrinking),
        (GameDay.DayOne, GamePhase.StormTwoShrinking) => (GameDay.DayOne, GamePhase.StormTwo),
        (GameDay.DayOne, GamePhase.StormTwo) => (GameDay.DayOne, GamePhase.StormOneShrinking),
        (GameDay.DayOne, GamePhase.StormOneShrinking) => (GameDay.DayOne, GamePhase.StormOne),

        (GameDay.DayOne, GamePhase.StormOne) => (GameDay.None, GamePhase.Waiting),

        _ => (CurrentDay, CurrentPhase)
    };

    public void ForceNextPhase()
    {
        var (nextDay, nextPhase) = GetNextPhaseInfo();
        CurrentDay = nextDay;
        SetPhase(nextPhase);
    }

    public void ForcePreviousPhase()
    {
        var (previousDay, previousPhase) = GetPreviousPhaseInfo();
        CurrentDay = previousDay;
        SetPhase(previousPhase);
    }

    public void ResetTimer()
    {
        if (CurrentPhase != GamePhase.BossFight && CurrentPhase != GamePhase.Waiting)
        {
            int duration = _config.GetDuration(CurrentDay, CurrentPhase);
            _timer.Start(duration);
        }
    }

    public void PauseTimer()
    {
        _timer.Stop();
        IsTimerRunning = false;
    }

    public void ResumeTimer()
    {
        if (_timeLeft > 0)
        {
            _timer.Start(_timeLeft);
            IsTimerRunning = true;
        }
    }

    public void ToggleTimer()
    {
        if (IsTimerRunning)
        {
            PauseTimer();
        }
        else
        {
            ResumeTimer();
        }
    }

    private void OnTimerToWaitingFinished()
    {
        CurrentDay = GameDay.None;
        SetPhase(GamePhase.Waiting);
    }
}