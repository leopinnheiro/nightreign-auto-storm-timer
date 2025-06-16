using nightreign_auto_storm_timer.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace nightreign_auto_storm_timer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AppConfig.Load();

    }
}

