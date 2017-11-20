
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isSubfolders = false;
        TimeSpan t = new TimeSpan(0, 2, 3);
        public MainWindow()
        {
            InitializeComponent();

        }


        private void chooseFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FileImporter fm = new FileImporter();
                    fm.OnVideoLoaded += onVideoRecievedHandler;
                    fm.getAllMediaFromDirectory(folderDialog.SelectedPath, isSubfolders);
                    fm.getAllMediaFromDirectory(folderDialog.SelectedPath,true);
                }
            }
        }
        // event handler raised when data of enterred pathes is loaded on fileImporter.
        private void onVideoRecievedHandler(Object o,List<Video> arg)
        {
            System.Windows.MessageBox.Show("Done!!!");
            Random r = new Random();
            int x = r.Next(arg.Count);

            playVideoFromPosition(arg.ElementAt(x).Path, t); ;
        }

        private void selectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ("MKV|*.mkv");
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPathTextbox.Text = ofd.FileName;
            }
        }

        private void playVideoFromPosition(string path, TimeSpan t)
        {
            mediaPlayer.Source = new Uri(path);
            mediaPlayer.Position = t;
            mediaPlayer.Play();
        }

        #region Media player functions

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(Directory.Exists(folderPathTextbox.Text)))
                return;


        }



        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            var f = new FileImporter();
            f.createDB();

            //mediaPlayer.Source = new Uri(folderPathTextbox.Text);
            //mediaPlayer.Play();
        }

        // Play the media.
        void OnMouseDownPlayMedia(object sender, MouseButtonEventArgs args)
        {
            mediaPlayer.Play();
        }

        // Pause the media.
        void OnMouseDownPauseMedia(object sender, MouseButtonEventArgs args)
        {
            mediaPlayer.Pause();

        }

        // Stop the media.
        void OnMouseDownStopMedia(object sender, MouseButtonEventArgs args)
        {
            mediaPlayer.Stop();

        }

        #endregion Media player functions

    }
}
