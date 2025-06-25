using CommunityToolkit.Mvvm.ComponentModel;
using nightreign_auto_storm_timer.Models;
using System.Collections.ObjectModel;

namespace nightreign_auto_storm_timer.ViewModels;

public partial class HelpViewModel : ObservableObject
{
    public ObservableCollection<HelpItem> HelpList { get; } = [];

    public HelpViewModel()
    {
        HelpList.Add(new HelpItem { Command = "Ctrl + Q", Description = "Close the application" });
        HelpList.Add(new HelpItem { Command = "F1", Description = "Start, pause, and advance the phase" });
        HelpList.Add(new HelpItem { Command = "F2", Description = "Reset the remaining time" });
        HelpList.Add(new HelpItem { Command = "F3", Description = "Go to the previous phase" });
        HelpList.Add(new HelpItem { Command = "F4", Description = "Go to the next phase" });
        HelpList.Add(new HelpItem { Command = "F5", Description = "Toggle compact mode" });
        HelpList.Add(new HelpItem { Command = "F6", Description = "Toggle automatic mode" });
        HelpList.Add(new HelpItem { Command = "F7", Description = "Show/hide the help window" });
        HelpList.Add(new HelpItem { Command = "F8", Description = "Show/hide the configuration window" });
        HelpList.Add(new HelpItem { Command = "F9", Description = "If DEBUG is enabled, take a DEBUG screenshot of the screen." });
    }
}
