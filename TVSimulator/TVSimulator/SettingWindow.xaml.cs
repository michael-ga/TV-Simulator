using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YoutubeImporter;

namespace TVSimulator
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private bool isSubfolders;
        private FileImporter fileImporter;
        private Search ytbSearcher ;


        public SettingWindow()
        {
            InitializeComponent();
            fileImporter = new FileImporter();
            ytbSearcher = new Search();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isSubfolders = !isSubfolders;
        }

        private void getFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileImporter.getAllMediaFromDirectory(folderDialog.SelectedPath, isSubfolders);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    
        // start async Task in order to sync Youtbe channel
        private void sync_btn_Click(object sender, RoutedEventArgs e)
        {
            switch(((System.Windows.Controls.Button)sender).Name)
            {
                case "sync_all_btn":
                    ytbSearcher.syncYoutubeChannels();
                    ytbSearcher.syncYoutubePlaylistChannels();
                    break;
                case "sync_reg_btn":
                    ytbSearcher.syncYoutubeChannels();
                    break;
                case "sync_plylst_btn":
                    ytbSearcher.syncYoutubePlaylistChannels();
                    break;

                default:
                    break;
            }
        }

        private void launch_youtube_browser_click(object sender, RoutedEventArgs e)
        {
            YoutubeImporter.MainWindow m = new YoutubeImporter.MainWindow();
            m.Show();
        }
    }
}
