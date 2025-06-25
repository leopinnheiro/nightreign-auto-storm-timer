using nightreign_auto_storm_timer.Controls;
using nightreign_auto_storm_timer.ViewModels;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : WindowBase
    {
        public SettingsWindow(SettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            };

            vm.RequestClose = Close;
        }
    }
}
