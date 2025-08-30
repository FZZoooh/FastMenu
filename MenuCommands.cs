using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FastMenu
{
    /// <summary>
    /// 菜单命令集合，包含应用程序中使用的所有路由命令
    /// </summary>
    public static class MenuCommands
    {
        /// <summary>
        /// 打开记事本命令
        /// </summary>
        public static RoutedCommand OpenNotepad = new RoutedCommand();
        
        /// <summary>
        /// 锁定工作站命令
        /// </summary>
        public static RoutedCommand LockWorkstation = new RoutedCommand();
        
        /// <summary>
        /// 打开文档命令
        /// </summary>
        public static RoutedCommand OpenDocuments = new RoutedCommand();
        
        /// <summary>
        /// 打开艺术图片命令
        /// </summary>
        public static RoutedCommand OpenArt = new RoutedCommand();
        
        /// <summary>
        /// 打开画图工具命令
        /// </summary>
        public static RoutedCommand OpenMspaint = new RoutedCommand();
    }
}