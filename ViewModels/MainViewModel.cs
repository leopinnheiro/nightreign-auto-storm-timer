using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace nightreign_auto_storm_timer.ViewModels
{
    public enum State
    {
        Waiting_DayOne,
        Timer,
        Waiting_DayTwo
    }

    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isCompactMode = true;

        public ObservableCollection<Phase> PhaseList { get; } = [];

        [ObservableProperty]
        private Phase? activePhase;
        
        [ObservableProperty]
        private int currentRemainingTime;

        private DispatcherTimer? _timer;

        public string ActivePhaseName => ActivePhase?.Name ?? "";

        public string CurrentRemainingTimeFormatted =>
            TimeSpan.FromSeconds(CurrentRemainingTime).ToString(@"mm\:ss");

        private Phase dayOnePhase;
        private Phase dayTwoPhase;

        [ObservableProperty]
        private State state = State.Waiting_DayOne;

        [ObservableProperty]
        private bool isUsingProcessor;

        private ScreenOcrProcessor _processor;

        public MainViewModel()
        {
            State = State.Waiting_DayOne;

            SetupPhase();

            CurrentRemainingTime = ActivePhase?.TimeInSeconds ?? 0;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            List<TextStateDetector> detectors =
            [
                new(["DIA I", "DAY I", "TAG I", "GIORNO I", "JOUR I"],
                    DayOneDetected
                    ),
                new(["DIA II", "DAY II", "TAG II", "GIORNO II", "JOUR II"],
                    DayTwoDetected
                    )
            ];

            _processor = new ScreenOcrProcessor(stateDetectors: detectors);

            IsUsingProcessor = true;

            AppConfig.ConfigChanged += (s, e) =>
            {
                SetupPhase();
            };
        }

        public void SetupPhase()
        {
            string? previousPhaseName = ActivePhase?.Name;

            PhaseList.Clear();

            dayOnePhase = new Phase { Name = "Day 1 Storm", TimeInSeconds = AppConfig.Current.DayOneStormOne };
            dayTwoPhase = new Phase { Name = "Day 2 Storm", TimeInSeconds = AppConfig.Current.DayTwoStormOne };

            PhaseList.Add(dayOnePhase);
            PhaseList.Add(new Phase { Name = "Day 1 Storm Shrinking", TimeInSeconds = AppConfig.Current.DayOneStormOneShrinking });
            PhaseList.Add(new Phase { Name = "Day 1 Storm 2", TimeInSeconds = AppConfig.Current.DayOneStormTwo });
            PhaseList.Add(new Phase { Name = "Day 1 Storm 2 Shrinking", TimeInSeconds = AppConfig.Current.DayOneStormTwoShrinking });
            PhaseList.Add(new Phase { Name = "Boss Fight", TimeInSeconds = 0, State = State.Waiting_DayTwo });
            PhaseList.Add(dayTwoPhase);
            PhaseList.Add(new Phase { Name = "Day 2 Storm Shrinking", TimeInSeconds = AppConfig.Current.DayTwoStormOneShrinking });
            PhaseList.Add(new Phase { Name = "Day 2 Storm 2", TimeInSeconds = AppConfig.Current.DayTwoStormTwo });
            PhaseList.Add(new Phase { Name = "Day 2 Storm 2 Shrinking", TimeInSeconds = AppConfig.Current.DayTwoStormTwoShrinking });
            PhaseList.Add(new Phase { Name = "Final Boss", TimeInSeconds = 0, State = State.Waiting_DayOne });

            if (!string.IsNullOrEmpty(previousPhaseName))
                ActivePhase = PhaseList.FirstOrDefault(p => p.Name == previousPhaseName) ?? PhaseList[0];
            else
                ActivePhase = PhaseList[0];
        }

        public void ResetTimer()
        {
            CurrentRemainingTime = ActivePhase?.TimeInSeconds ?? 0;
        }

        partial void OnActivePhaseChanged(Phase? oldValue, Phase? newValue)
        {
            if (oldValue != null)
                oldValue.IsActive = false;

            if (newValue != null)
            {
                newValue.IsActive = true;
                CurrentRemainingTime = newValue.TimeInSeconds;
                
                if (newValue.State != null)
                    State = (State)newValue.State;
            }

            OnPropertyChanged(nameof(ActivePhaseName));
        }

        partial void OnCurrentRemainingTimeChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(CurrentRemainingTimeFormatted));
        }

        partial void OnStateChanged(State value)
        {
            Debug.WriteLine($"Current State: {value.ToString()}");
        }

        partial void OnIsUsingProcessorChanged(bool value)
        {
            if (value)
                _processor.Start();
            else
                _processor.Stop();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (CurrentRemainingTime > 0)
            {
                CurrentRemainingTime--;
            }
            else
            {
                _timer?.Stop();
                MoveToNextPhase();
            }
        }

        private void MoveToNextPhase()
        {
            if (ActivePhase == null) return;

            int index = PhaseList.IndexOf(ActivePhase);
            int nextIndex = (index + 1) % PhaseList.Count;
            var nextPhase = PhaseList[nextIndex];

            ActivePhase = nextPhase;

            if (nextPhase.TimeInSeconds == 0)
            {
                _timer?.Stop();
            }
            else
            {
                _timer?.Start();
            }
        }

        public void ToggleTimer()
        {
            if (_timer == null) return;

            if (_timer.IsEnabled)
            {
                SoundManager.PlayStop();
                _timer.Stop();
            }
            else
            {
                SoundManager.PlayStart();
                if (ActivePhase != null && ActivePhase.TimeInSeconds == 0)
                {
                    MoveToNextPhase();
                }
                else
                {
                    _timer.Start();
                    State = State.Timer;
                }
            }
        }

        public void ToggleCompactMode()
        {
            IsCompactMode = !IsCompactMode;
        }

        public void GoToNextPhase()
        {
            if (ActivePhase == null || PhaseList.Count == 0) return;

            int index = PhaseList.IndexOf(ActivePhase);
            if (index < PhaseList.Count - 1)
            {
                ActivePhase = PhaseList[index + 1];
            }
        }

        public void GoToPreviousPhase()
        {
            if (ActivePhase == null || PhaseList.Count == 0) return;

            int index = PhaseList.IndexOf(ActivePhase);
            if (index > 0)
            {
                ActivePhase = PhaseList[index - 1];
            }
        }

        public void ToggleUsingProcessor()
        {
            IsUsingProcessor = !IsUsingProcessor;
        }

        public void SetDayOnePhase()
        {
            if (State == State.Waiting_DayOne)
            {
                ActivePhase = dayOnePhase;
                ToggleTimer();
            }
        }

        public void SetDayTwoPhase()
        {
            if (State == State.Waiting_DayTwo)
            {
                ActivePhase = dayTwoPhase;
                ToggleTimer();
            }
        }

        private void DayOneDetected(string text)
        {
            Debug.WriteLine("Day I Detected");
            SetDayOnePhase();
        }

        private void DayTwoDetected(string text)
        {
            Debug.WriteLine("Day II Detected");
            SetDayTwoPhase();
        }

        public void Close()
        {
            _processor?.Stop();
            _processor?.Dispose();
        }
    }
}
