using nightreign_auto_storm_timer.Controls;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Views
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : WindowBase
    {
        public HelpWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            };
        }
    }
}
