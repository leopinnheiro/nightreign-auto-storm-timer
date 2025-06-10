using nightreign_auto_storm_timer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Views
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            DataContext = new HelpViewModel();
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            };
        }
    }
}
