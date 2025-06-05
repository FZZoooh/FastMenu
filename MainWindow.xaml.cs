using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Media3D;
using System.Windows;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

namespace FastMenu
{
    public partial class MainWindow : Window
    {
        const int HOTKEY_ID = 9000;
        const uint MOD_ALT = 0x0001;
        const uint VK_R = 0x52;

        [DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")] static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private List<RadialMenuItem> menuItems = new List<RadialMenuItem>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            this.MouseMove += GlobalMouseTracker;
            this.MouseLeave += WindowHider;

            // 初始化菜单项
            menuItems = new List<RadialMenuItem>
            {
                new RadialMenuItem {
                    Icon = "📝",
                    Command = MenuCommands.OpenNotepad,
                    ToolTip = "打开记事本"
                },
                new RadialMenuItem {
                    Icon = "🔒",
                    Command = MenuCommands.LockWorkstation,
                    ToolTip = "锁定电脑"
                },
                new RadialMenuItem
                {
                    Icon = "📄",
                    Command = MenuCommands.OpenDocuments,
                    ToolTip = "打开文档"
                },
                new RadialMenuItem
                {
                    Icon = "🎨",
                    Command = MenuCommands.OpenArt,
                    ToolTip = "打开艺术"
                }
            };

            Loaded += (s, e) => GenerateRadialButtons();

            SizeChanged += (s, e) => GenerateRadialButtons();

        }

        private void WindowHider(object sender, MouseEventArgs e)
        {
            this.Hide();
        }

        private void GlobalMouseTracker(object sender, MouseEventArgs e)
        {
            // 获取鼠标在屏幕坐标（非窗口坐标）
            Point screenMousePos = PointToScreen(e.GetPosition(this));

            // 转换为窗口内的逻辑坐标
            Point windowMousePos = this.PointFromScreen(screenMousePos);

            // 更新所有按钮
            foreach (Button btn in MainCanvas.Children.OfType<Button>())
            {
                UpdateButtonScale(btn, windowMousePos);
            }
        }


        private void UpdateButtonScale(Button button, Point mousePos)
        {
            // 计算按钮中心（考虑实际渲染位置）
            double btnCenterX = Canvas.GetLeft(button) + button.ActualWidth / 2;
            double btnCenterY = Canvas.GetTop(button) + button.ActualHeight / 2;

            // 距离计算（欧几里得距离）
            double deltaX = mousePos.X - btnCenterX;
            double deltaY = mousePos.Y - btnCenterY;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // 非线性缩放曲线
            double maxDist = 150; // 影响半径
            double scaleFactor = 1 + 1 * Math.Exp(-distance * 3 / maxDist); // 指数衰减

            // 应用变换（确保从中心缩放）
            button.RenderTransform = new ScaleTransform(scaleFactor, scaleFactor);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            var hwnd = helper.Handle;
            HwndSource source = HwndSource.FromHwnd(hwnd);
            source.AddHook(HwndHook);

            RegisterHotKey(hwnd, HOTKEY_ID, MOD_ALT, VK_R); // Alt+R
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            UnregisterHotKey(hwnd, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                ShowAtMouse();
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void ShowAtMouse()
        {
            // 获取当前窗口的DPI缩放因子
            PresentationSource source = PresentationSource.FromVisual(this);
            double dpiX = 1.0, dpiY = 1.0;

            if (source?.CompositionTarget != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }

            // 获取鼠标位置（物理像素）
            var screenPos = System.Windows.Forms.Cursor.Position;

            // 转换为逻辑像素（考虑DPI缩放）
            double logicalX = screenPos.X / dpiX;
            double logicalY = screenPos.Y / dpiY;

            // 使用窗口的实际尺寸计算位置
            Left = logicalX - ActualWidth / 2;
            Top = logicalY - ActualHeight / 2;

            Show();
            Activate();
        }

        private void GenerateRadialButtons()
        {
            MainCanvas.Children.Clear();
            double radius = Math.Min(ActualWidth, ActualHeight) * 0.3;

            for (int i = 0; i < menuItems.Count; i++)
            {
                var position = CalculateButtonPosition(i, menuItems.Count, radius);
                var button = CreateRadialButton(menuItems[i]);
                Canvas.SetLeft(button, position.X - button.Width / 2);
                Canvas.SetTop(button, position.Y - button.Height / 2);
                MainCanvas.Children.Add(button);
            }
        }

        private Point CalculateButtonPosition(int index, int total, double radius)
        {
            double angle = 2 * Math.PI * index / total;
            double centerX = ActualWidth / 2;
            double centerY = ActualHeight / 2;
            return new Point(
                centerX + radius * Math.Cos(angle),
                centerY + radius * Math.Sin(angle)
            );
        }

        private Button CreateRadialButton(RadialMenuItem item)
        {
            var button = new Button
            {
                Content = item.Icon,
                Style = (Style)Application.Current.Resources["RadialButtonStyle"],
                Tag = item
            };
            button.Click += (s, e) => ExecuteCommand(item.Command);
            return button;
        }

        private void ExecuteCommand(ICommand command)
        {
            if (command == MenuCommands.OpenNotepad)
            {
                Process.Start("notepad.exe");
            }
            else if (command == MenuCommands.LockWorkstation)
            {
                LockComputer();
            }
            else if (command == MenuCommands.OpenDocuments)
            {
                Process.Start("explorer.exe", "D:\\Documents");
            }
            else if (command == MenuCommands.OpenArt)
            {
                Process.Start("explorer.exe", "D:\\Documents\\Images\\Arts");
            }
            else
            {
                command.Execute(null);
            }
            this.Hide();  // 执行后隐藏窗口
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern void LockWorkStation();

        private void LockComputer()
        {
            LockWorkStation();
        }
    }
}
