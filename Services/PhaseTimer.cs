using System.Timers;
using Timer = System.Timers.Timer;

namespace nightreign_auto_storm_timer.Services;

public class PhaseTimer
{
    private readonly Timer _timer;
    private int _remainingSeconds;
    private Action? _onTimerFinished;

    public event Action<int>? OnTick;

    public PhaseTimer(Action onTimerFinished)
    {
        _onTimerFinished = onTimerFinished;
        _timer = new Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
    }

    public void Start(int durationSeconds)
    {
        _remainingSeconds = durationSeconds;
        OnTick?.Invoke(_remainingSeconds);
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _remainingSeconds--;
        OnTick?.Invoke(_remainingSeconds);

        if (_remainingSeconds <= 0)
        {
            _timer.Stop();
            _onTimerFinished?.Invoke();
        }
    }
}
