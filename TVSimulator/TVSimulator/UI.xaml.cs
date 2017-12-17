
using MediaClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace TVSimulator
{
    public partial class MainWindow : Window
    {
        #region fields
        public bool isSubfolders = false, IsFullscreen=false;
        private FileImporter fileImporter;
        #endregion fields

        public MainWindow()
        {
            InitializeComponent();
            fileImporter = new FileImporter();
            fileImporter.OnVideoLoaded += onVideoRecievedHandler;
        }

        #region button listeners

        private void chooseFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileImporter.getAllMediaFromDirectory(folderDialog.SelectedPath, isSubfolders);
                }
            }
        }

        private void testBtn_Click(object sender, RoutedEventArgs e) {}

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isSubfolders = !isSubfolders;
        }

        private void Channel_Up_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Channel_Down_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion button listeners

        #region Media player functions

        private void playVideoFromPosition(string path, TimeSpan t)
        {
            mediaPlayer.Position = t;
            mediaPlayer.Source = new Uri(path);
            mediaPlayer.Play();
        }
       
        #endregion Media player functions

        #region subMethods

        // event handler raised when data of enterred pathes is loaded on fileImporter.
        private void onVideoRecievedHandler(Object o, List<Media> arg)
        {
            Random r = new Random();
            int x = r.Next(arg.Count);
            folderPathTextbox.Text = arg.ElementAt(x).Name;     //check the name of media

            if (arg.ElementAt(x) is Music)
                musicImage.Visibility = Visibility.Visible;
            else
                musicImage.Visibility = Visibility.Hidden;
            playVideoFromPosition(arg.ElementAt(x).Path, new TimeSpan(0,0,0)); 
        }

        #endregion helper methods

    }
}
