using HelperClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TVSimulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //TODO:: check if database doesn't contain any collection
            if ( !File.Exists(@"C:\TVSimulatorDB\MyData.db"))
            {
                initWindow iw = new initWindow();
                iw.Show();
            }
            else
            {
                MainWindow mw = new MainWindow();
                mw.Show();
            }
        }
    }
}
