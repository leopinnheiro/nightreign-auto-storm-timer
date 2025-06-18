using nightreign_auto_storm_timer.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Views
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigViewModel GetDataContext()
        {
            return (ConfigViewModel)DataContext;
        }

        public ConfigWindow()
        {
            InitializeComponent();
            DataContext = new ConfigViewModel();
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            };
        }
        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ConfigViewModel vm)
                return;

            vm.SaveConfig();
            Hide();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ConfigViewModel vm)
                return;

            Hide();
        }

        public void OnGuideWindowMoved(double x, double y, string monitorName)
        {
            if (DataContext is not ConfigViewModel vm)
                return;

            vm.OnGuideWindowMoved(x, y, monitorName);
        }
    }
}
