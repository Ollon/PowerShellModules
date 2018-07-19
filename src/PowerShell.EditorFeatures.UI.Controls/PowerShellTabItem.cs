using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PowerShell.EditorFeatures.UI.Controls
{
    public class PowerShellTabItem : TabItem
    {
        static PowerShellTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PowerShellTabItem), new FrameworkPropertyMetadata(typeof(PowerShellTabItem)));
        }
    }
}
