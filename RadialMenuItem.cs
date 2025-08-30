using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FastMenu
{
    /// <summary>
    /// 环形菜单项
    /// </summary>
    public class RadialMenuItem
    {
        /// <summary>
        /// 菜单项图标（文本或图像资源）
        /// </summary>
        public object Icon { get; set; }
        
        /// <summary>
        /// 菜单项命令
        /// </summary>
        public ICommand Command { get; set; }
        
        /// <summary>
        /// 菜单项工具提示
        /// </summary>
        public string ToolTip { get; set; }
    }
}