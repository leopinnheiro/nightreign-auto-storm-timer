using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Utils;
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

        GlobalExceptionHandler.Initialize();

        AppConfigManager.Instance.Load();
        AppManager.Instance.Start();
    }
}

