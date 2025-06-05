using System;
using System.Windows;
using System.Runtime.InteropServices;

namespace FastMenu
{
    public partial class App : Application
    {
        // 核心DPI修复（兼容所有Windows版本）
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        protected override void OnStartup(StartupEventArgs e)
        {
            // 1. 修复DPI缩放问题
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware(); // 关键！解决125%缩放偏移问题
            }

            // 2. 初始化主窗口（保持隐藏）
            base.OnStartup(e);
            var mainWin = new MainWindow();
            mainWin.Show(); // 先显示一次确保窗口加载
            mainWin.Hide(); // 立刻隐藏
        }

        // 可选：崩溃捕获（调试用）
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"程序崩溃了:\n{e.Exception.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}