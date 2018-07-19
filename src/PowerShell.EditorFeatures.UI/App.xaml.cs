using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PowerShell.EditorFeatures.Core.Host;



[assembly: ThemeInfo(ResourceDictionaryLocation.SourceAssembly, ResourceDictionaryLocation.None)]

namespace PowerShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IEditorFeaturesHostObject HostObject { get; set; }


        private void OnAppStartup(object sender, StartupEventArgs e)
        {



        }
    }
}
