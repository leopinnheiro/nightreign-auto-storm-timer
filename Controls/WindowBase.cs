using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace nightreign_auto_storm_timer.Controls;

public class WindowBase : Window
{
    private bool _isClosing;

    public WindowBase()
    {
        Opacity = 0;
        Loaded += WindowBase_Loaded;
        Closing += WindowBase_Closing;
    }

    private void WindowBase_Loaded(object? sender, RoutedEventArgs e)
    {
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        BeginAnimation(OpacityProperty, fadeIn);
    }

    private void WindowBase_Closing(object? sender, CancelEventArgs e)
    {
        if (_isClosing)
            return;

        e.Cancel = true;
        _isClosing = true;

        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };

        fadeOut.Completed += (_, _) =>
        {
            Closing -= WindowBase_Closing;
            Close();
        };

        BeginAnimation(OpacityProperty, fadeOut);
    }
}
