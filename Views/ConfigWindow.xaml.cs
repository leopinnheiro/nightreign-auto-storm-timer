using nightreign_auto_storm_timer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Views
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            DataContext = new ConfigViewModel();
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            };

            IsVisibleChanged += ConfigWindow_IsVisibleChanged;
        }
        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ConfigViewModel vm)
                return;

            vm.SaveConfig();
            Hide();
        }

        private void ConfigWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                DataContext = new ConfigViewModel();
            }
        }
    }
}
