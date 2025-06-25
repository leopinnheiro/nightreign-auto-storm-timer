using nightreign_auto_storm_timer.Managers;
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
        private CaptureConfigViewModel? CaptureConfigViewModel;

        public GuideWindow()
        {
            InitializeComponent();

            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            };

            Loaded += (s, e) =>
            {
                CaptureConfigViewModel = AppManager.Instance.SettingsViewModel.Capture;
            };

            LocationChanged += (_, _) =>
            {
                var screen = Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle);
                CaptureConfigViewModel?.OnGuideWindowMoved(Left, Top, screen.DeviceName);
            };

            SizeChanged += (_, _) =>
            {
                CaptureConfigViewModel?.OnGuideWindowResized(ActualWidth, ActualHeight);
            };
        }

        private void Resize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragResize(WindowResizeEdge.BottomRight);
            }
        }

        private void DragResize(WindowResizeEdge direction)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            if (hwnd == IntPtr.Zero) return;

            const int WM_SYSCOMMAND = 0x112;
            const int SC_SIZE = 0xF000;

            int resizeDirection = direction switch
            {
                WindowResizeEdge.Left => 1,
                WindowResizeEdge.Right => 2,
                WindowResizeEdge.Top => 3,
                WindowResizeEdge.TopLeft => 4,
                WindowResizeEdge.TopRight => 5,
                WindowResizeEdge.Bottom => 6,
                WindowResizeEdge.BottomLeft => 7,
                WindowResizeEdge.BottomRight => 8,
                _ => 0
            };

            if (resizeDirection == 0) return;

            NativeMethods.SendMessage(hwnd, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + resizeDirection), IntPtr.Zero);
        }

        private enum WindowResizeEdge
        {
            Left, Right, Top, TopLeft, TopRight, Bottom, BottomLeft, BottomRight
        }

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        }

    }
}
