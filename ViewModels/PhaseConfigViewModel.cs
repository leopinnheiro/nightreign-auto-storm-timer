using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Models;

namespace nightreign_auto_storm_timer.ViewModels;

public partial class PhaseConfigViewModel : ObservableObject
{
    [ObservableProperty]
    private int stormOne;

    [ObservableProperty]
    private int stormOneShrinking;

    [ObservableProperty]
    private int stormTwo;

    [ObservableProperty]
    private int stormTwoShrinking;

    public PhaseConfigViewModel() { }

    public PhaseConfigViewModel(PhaseConfig config)
    {
        stormOne = config.StormOne;
        stormOneShrinking = config.StormOneShrinking;
        stormTwo = config.StormTwo;
        stormTwoShrinking = config.StormTwoShrinking;
    }

    public PhaseConfig ToModel()
    {
        return new PhaseConfig
        {
            StormOne = StormOne,
            StormOneShrinking = StormOneShrinking,
            StormTwo = StormTwo,
            StormTwoShrinking = StormTwoShrinking
        };
    }
}

