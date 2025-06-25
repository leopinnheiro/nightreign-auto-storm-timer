using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Services;
using nightreign_auto_storm_timer.Utils;
using nightreign_auto_storm_timer.ViewModels;
using nightreign_auto_storm_timer.Views;
using System.Windows;

namespace nightreign_auto_storm_timer.Managers;

public class AppManager
{
    public static AppManager Instance { get; } = new();

    public MainViewModel MainViewModel { get; private set; }
    public MainWindow MainWindow { get; private set; }

    public HelpViewModel HelpViewModel { get; private set; }
    public HelpWindow HelpWindow { get; private set; }

    public SettingsViewModel SettingsViewModel { get; private set; }

    public GuideWindow GuideWindow { get; private set; }

    public ShortcutManager ShortcutManager { get; }

    public GamePhaseManager GamePhaseManager { get; }
    private AppManager()
    {
        GamePhaseManager = new GamePhaseManager();
        ShortcutManager = new ShortcutManager();
    }

    public void Start()
    {
        MainViewModel = new MainViewModel();
        MainWindow = new MainWindow(MainViewModel);

        HelpViewModel = new HelpViewModel();
        HelpWindow = new HelpWindow
        {
            DataContext = HelpViewModel
        };

        SettingsViewModel = new SettingsViewModel();

        GuideWindow = new GuideWindow();

        RegisterShortcuts();
        MainWindow.Show();
    }

    private void RegisterShortcuts()
    {
        // Close App
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.CloseApp, () =>
        {
            // Save window position before close app
            var config = AppConfigManager.Instance.CurrentConfig;
            if (config.RememberWindowPosition)
            {
                config.WindowLeft = MainWindow.Left;
                config.WindowTop = MainWindow.Top;
            }

            CloseApp();
        });

        // Reset
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.PauseOrResumeTimer, () =>
        {
            GamePhaseManager.ToggleTimer();
        });

        // Reset
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.ResetTimer, () =>
        {
            GamePhaseManager.ResetTimer();
        });

        // Previous phase
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.PreviousPhase, () =>
        {
            GamePhaseManager.ForcePreviousPhase();
        });

        // Next phase
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.NextPhase, () =>
        {
            GamePhaseManager.ForceNextPhase();
        });

        // Toggle compact mode
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.ToggleCompactMode, () =>
        {
            MainViewModel.ToggleCompactMode();
            MainWindow.UpdateModeUI();
        });

        // Toggle help view
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.ToggleHelpView, () =>
        {
            if (HelpWindow.IsVisible)
                HelpWindow.Hide();
            else
            {
                HelpWindow.Left = MainWindow.Left + MainWindow.Width + 10;
                HelpWindow.Top = MainWindow.Top;

                HelpWindow.Show();
            }
        });

        // Toggle Config view
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.OpenSettingsWindow, () =>
        {
            using (new ShortcutSuspension())
            {
                SettingsViewModel.ConfigFields();
                var settingsWindow = new SettingsWindow(SettingsViewModel);
                settingsWindow.ShowDialog();
            }
        });

        // Debug
        ShortcutManager.RegisterNamedShortcut(ShortcutAction.DEBUG, () =>
        {
            DebugUtil.DebugScreen();
        });
    }

    private void CloseApp()
    {
        HelpWindow.Close();
        MainWindow.Close();
        AppConfigManager.Instance.Save();

        LogService.Shutdown();
        Application.Current.Shutdown();
    }
}

