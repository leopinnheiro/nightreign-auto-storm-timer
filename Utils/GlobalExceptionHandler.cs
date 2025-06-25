using nightreign_auto_storm_timer.Services;
using System.Windows;

namespace nightreign_auto_storm_timer.Utils;

public static class GlobalExceptionHandler
{
    public static void Initialize()
    {
        Application.Current.DispatcherUnhandledException += (s, args) =>
        {
            LogService.Exception(args.Exception);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                LogService.Exception(ex);
        };

        TaskScheduler.UnobservedTaskException += (s, args) =>
        {
            LogService.Exception(args.Exception);
            args.SetObserved();
        };
    }
}

