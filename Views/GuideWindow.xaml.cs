using nightreign_auto_storm_timer.ViewModels;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Views
{
    /// <summary>
    /// Interaction logic for GuideWindow.xaml
    /// </summary>
    public partial class GuideWindow : Window
    {
        private readonly ConfigViewModel _ownerViewModel;
        public GuideWindow(ConfigViewModel ownerViewModel)
        {
            InitializeComponent();
            _ownerViewModel = ownerViewModel;

            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            };
            LocationChanged += (_, _) =>
            {
                var screen = Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle);
                _ownerViewModel.OnGuideWindowMoved(Left, Top, screen.DeviceName);
            };
        }
    }
}
