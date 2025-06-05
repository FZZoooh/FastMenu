using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FastMenu
{
    public static class MenuCommands
    {
        public static RoutedCommand OpenNotepad = new RoutedCommand();
        public static RoutedCommand LockWorkstation = new RoutedCommand();
        public static RoutedCommand OpenDocuments = new RoutedCommand();
        public static RoutedCommand OpenArt = new RoutedCommand();
        // 后续可扩展更多命令
    }
}