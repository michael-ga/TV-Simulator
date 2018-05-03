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

        // startup implemention is in main window
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            MainWindow mw = new MainWindow();
            if(mw.isChannelsExist())
                mw.Show();
            else
            {
                initWindow iw = new initWindow();
                iw.Show();
                mw.Close();
            }
            //}
        }
    }
}
