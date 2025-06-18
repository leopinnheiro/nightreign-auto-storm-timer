using Gma.System.MouseKeyHook;
using nightreign_auto_storm_timer.Utils;
using nightreign_auto_storm_timer.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private IKeyboardMouseEvents? _globalHook;
    private HelpWindow? _helpWindow;
    private ConfigWindow? _configWindow;

    public MainWindow()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            File.WriteAllText("crash.log", e.ExceptionObject.ToString());
        };

        AppPaths.EnsureDirectoriesExist();

        InitializeComponent();
        DataContext = new MainViewModel();
        MouseLeftButtonDown += (s, e) =>
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        };

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;

        UpdateModeUI();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _globalHook = Hook.GlobalEvents();
        _globalHook.KeyDown += GlobalHook_KeyDown;
    }

    private void GlobalHook_KeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        // Close App
        if (e.KeyCode == System.Windows.Forms.Keys.Q &&
            System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control))
        {
            Dispatcher.Invoke(() => Application.Current.Shutdown());
        }

        // Start, Next, Pause, Resume
        if (e.KeyCode == System.Windows.Forms.Keys.F1)
        {
            vm.ToggleTimer();
        }
        // Reset
        else if (e.KeyCode == System.Windows.Forms.Keys.F2)
        {
            vm.ResetTimer();
        }
        // Previous phase
        else if (e.KeyCode == System.Windows.Forms.Keys.F3 || e.KeyCode == System.Windows.Forms.Keys.Subtract)
        {
            vm.GoToPreviousPhase();
        }
        // Next phase
        else if (e.KeyCode == System.Windows.Forms.Keys.F4 || e.KeyCode == System.Windows.Forms.Keys.Add)
        {
            vm.GoToNextPhase();
        }
        // Toggle compact mode
        else if (e.KeyCode == System.Windows.Forms.Keys.F5)
        {
            vm.ToggleCompactMode();
            Dispatcher.Invoke(UpdateModeUI);
        }
        // Start/ Stop processor
        else if (e.KeyCode == System.Windows.Forms.Keys.F6)
        {
            vm.ToggleUsingProcessor();
        }
        // Toggle help view
        else if (e.KeyCode == System.Windows.Forms.Keys.F7)
        {
            if (_helpWindow == null)
            {
                _helpWindow = new HelpWindow();
                _helpWindow.Owner = this;

                var main = this;
                _helpWindow.Left = main.Left + main.Width + 10;
                _helpWindow.Top = main.Top;

                _helpWindow.Show();
            }
            else if (_helpWindow.IsVisible)
            {
                _helpWindow.Hide();
            }
            else
            {
                _helpWindow.Show();
            }
        }
        // Toggle Config view
        else if (e.KeyCode == System.Windows.Forms.Keys.F8)
        {
            if (_configWindow == null)
            {
                _configWindow = new ConfigWindow();
                _configWindow.Owner = this;
                _configWindow.Show();
            }
            else if (_configWindow.IsVisible)
            {
                _configWindow.Hide();
            }
            else
            {
                _configWindow.Show();
            }
        }
        else if (e.KeyCode == System.Windows.Forms.Keys.F9)
        {
            Task.Run(DebugUtil.SaveScreen);
        }
    }

    private void UpdateModeUI()
    {
        if (DataContext is not MainViewModel vm) return;

        if (vm.IsCompactMode)
        {
            PhasePanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            PhasePanel.Visibility = Visibility.Visible;
        }
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        if (_globalHook is not null)
        {
            _globalHook.KeyDown -= GlobalHook_KeyDown;
            _globalHook.Dispose();
        }

        if (DataContext is MainViewModel vm)
            vm.Close();
    }
}

