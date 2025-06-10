using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace nightreign_auto_storm_timer.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isCompactMode;

        public ObservableCollection<Phase> PhaseList { get; } = [];

        [ObservableProperty]
        private Phase? activePhase;
        
        [ObservableProperty]
        private int currentRemainingTime;

        private DispatcherTimer? _timer;

        public string ActivePhaseName => ActivePhase?.Name ?? "";

        public string CurrentRemainingTimeFormatted =>
            TimeSpan.FromSeconds(CurrentRemainingTime).ToString(@"mm\:ss");

        public MainViewModel()
        {
            PhaseList.Add(new Phase { Name = "Day 1 Storm", TimeInSeconds = 270 });
            PhaseList.Add(new Phase { Name = "Day 1 Storm Shrinking", TimeInSeconds = 180 });
            PhaseList.Add(new Phase { Name = "Day 1 Storm 2", TimeInSeconds = 210 });
            PhaseList.Add(new Phase { Name = "Day 1 Storm 2 Shrinking", TimeInSeconds = 180 });
            PhaseList.Add(new Phase { Name = "Boss Fight", TimeInSeconds = 0 });
            PhaseList.Add(new Phase { Name = "Day 2 Storm", TimeInSeconds = 270 });
            PhaseList.Add(new Phase { Name = "Day 2 Storm Shrinking", TimeInSeconds = 180 });
            PhaseList.Add(new Phase { Name = "Day 2 Storm 2", TimeInSeconds = 210 });
            PhaseList.Add(new Phase { Name = "Day 2 Storm 2 Shrinking", TimeInSeconds = 180 });
            PhaseList.Add(new Phase { Name = "Final Boss", TimeInSeconds = 0 });

            MockPhasesOnlyInDebug();

            ActivePhase = PhaseList[0];

            CurrentRemainingTime = ActivePhase?.TimeInSeconds ?? 0;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
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
            }

            OnPropertyChanged(nameof(ActivePhaseName));
        }

        partial void OnCurrentRemainingTimeChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(CurrentRemainingTimeFormatted));
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

        [Conditional("DEBUG")]
        public void MockPhasesOnlyInDebug()
        {
            PhaseList.Clear();
            PhaseList.Add(new Phase { Name = "Day 1 Storm", TimeInSeconds = 5 });
            PhaseList.Add(new Phase { Name = "Day 1 Storm Shrinking", TimeInSeconds = 5 });
            PhaseList.Add(new Phase { Name = "Day 1 Storm 2", TimeInSeconds = 5 });
            PhaseList.Add(new Phase { Name = "Day 1 Storm 2 Shrinking", TimeInSeconds = 5 });
            PhaseList.Add(new Phase { Name = "Boss Fight", TimeInSeconds = 0 });
            PhaseList.Add(new Phase { Name = "Day 2 Storm", TimeInSeconds = 5 });
            PhaseList.Add(new Phase { Name = "Day 2 Storm Shrinking", TimeInSeconds = 5 });
            PhaseList.Add(new Phase { Name = "Day 2 Storm 2", TimeInSeconds = 5 });
            PhaseList.Add(new Phase { Name = "Day 2 Storm 2 Shrinking", TimeInSeconds = 5 });
            PhaseList.Add(new Phase { Name = "Final Boss", TimeInSeconds = 0 });
        }
    }
}
