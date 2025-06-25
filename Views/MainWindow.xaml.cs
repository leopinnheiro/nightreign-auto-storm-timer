using nightreign_auto_storm_timer.Controls;
using nightreign_auto_storm_timer.Managers;
using nightreign_auto_storm_timer.Utils;
using nightreign_auto_storm_timer.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : WindowBase
{
    public MainWindow(MainViewModel vm)
    {
        AppPaths.EnsureDirectoriesExist();

        InitializeComponent();
        DataContext = vm;

        MouseLeftButtonDown += (s, e) =>
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        };

        UpdateModeUI();

        SourceInitialized += (_, _) =>
        {
            var config = AppConfigManager.Instance.CurrentConfig;
            if (config.RememberWindowPosition)
            {
                Left = config.WindowLeft;
                Top = config.WindowTop;
            }
        };
    }

    public void UpdateModeUI()
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

    private void ShowButtons_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;
        vm.ToggleShowButtons();

        if (vm.IsShowButtons)
        {
            ButtonsPanel.Visibility = Visibility.Visible;
        }
        else
        {
            ButtonsPanel.Visibility = Visibility.Collapsed;
        }
    }
}

