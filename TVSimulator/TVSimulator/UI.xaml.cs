
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
        public bool isSubfolders = false, IsFullscreen=false;
        TimeSpan t = new TimeSpan(0, 2, 3);
        private Size _previousVideoContainerSize = new Size();

        public MainWindow()
        {
            //this.WindowState = WindowState.Maximized;

            InitializeComponent();
           
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


        #region button listeners
        private void chooseFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FileImporter fm = new FileImporter();
                    fm.OnVideoLoaded += onVideoRecievedHandler;
                    fm.getAllMediaFromDirectory(folderDialog.SelectedPath, isSubfolders);
                   
                }
            }
        }

        private void selectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ("MKV|*.mkv");
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPathTextbox.Text = ofd.FileName;
                play(ofd.FileName);
            }
        }



        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {


        }



        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Source = new Uri(folderPathTextbox.Text);
            mediaPlayer.Play();
            FullscreenToggle();

        }


        #endregion button listeners




        #region Media player functions

        private void FullscreenToggle()
        {
            this.IsFullscreen = !this.IsFullscreen;
            if (this.IsFullscreen)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                mediaPlayer.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
                mediaPlayer.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            }
            else
            {
                mediaPlayer.Width = _previousVideoContainerSize.Width;
                mediaPlayer.Height = _previousVideoContainerSize.Height;
            }
        }



        private void playVideoFromPosition(string path, TimeSpan t)
        {
            mediaPlayer.Position = t;
            play(path);
        }

        private void play(string path)
        {
            mediaPlayer.Source = new Uri(path);
            mediaPlayer.Play();
        }

        #endregion Media player functions



        // event handler raised when data of enterred pathes is loaded on fileImporter.
        private void onVideoRecievedHandler(Object o, List<Video> arg)
        {
            System.Windows.MessageBox.Show("Done!!!");
            Random r = new Random();
            int x = r.Next(arg.Count);

            playVideoFromPosition(arg.ElementAt(x).Path, t); ;
        }

    }
}
