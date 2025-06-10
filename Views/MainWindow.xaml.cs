using Gma.System.MouseKeyHook;
using nightreign_auto_storm_timer.ViewModels;
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

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _globalHook = Hook.GlobalEvents();
        _globalHook.KeyDown += GlobalHook_KeyDown;
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        if (_globalHook is not null)
        {
            _globalHook.KeyDown -= GlobalHook_KeyDown;
            _globalHook.Dispose();
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
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
        if (e.KeyCode == System.Windows.Forms.Keys.F6)
        {
            if (_helpWindow == null)
            {
                _helpWindow = new HelpWindow();
                _helpWindow.Owner = this;

                // Exibir à direita da janela principal
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
}

