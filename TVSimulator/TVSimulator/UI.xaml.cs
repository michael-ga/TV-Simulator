
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
        #region fileds
        public bool isSubfolders = false, IsFullscreen=false;
        TimeSpan t = new TimeSpan(0, 2, 3);
        private Size _previousVideoContainerSize = new Size();
        private localFileImporter fileImporter;
        #endregion fileds

        public MainWindow()
        {
            //this.WindowState = WindowState.Maximized;

            InitializeComponent();
            fileImporter = new localFileImporter();
            fileImporter.OnVideoLoaded += onVideoRecievedHandler;

        }

        #region mouse listeners
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
        #endregion mouse listeners


        #region button listeners
        private void chooseFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileImporter.getLocalFilesToDB(folderDialog.SelectedPath, isSubfolders);
                }
            }
        }

        //TODO: DELETE THIS if we dont need this - ROY

        /*private void selectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ("MKV|*.mkv");
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPathTextbox.Text = ofd.FileName;
                play(ofd.FileName);
            }
        }*/



        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            //mediaPlayer.Source = new Uri(folderPathTextbox.Text);
            //mediaPlayer.Play();
            //FullscreenToggle();
            
        }

        private void testBtn_Click(object sender, RoutedEventArgs e)
        {
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
                btnExit.Visibility = Visibility.Visible;
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isSubfolders = !isSubfolders;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Channel_Up_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Channel_Down_Click(object sender, RoutedEventArgs e)
        {

        }



        #endregion Media player functions

        #region helper methods

        // event handler raised when data of enterred pathes is loaded on fileImporter.
        private void onVideoRecievedHandler(Object o, List<Media> arg)
        {
            System.Windows.MessageBox.Show("Done!!!");
            Random r = new Random();
            int x = r.Next(arg.Count);
            folderPathTextbox.Text = arg.ElementAt(x).Name;     //check the name of media

            if (arg.ElementAt(x).GetType().ToString() == "MediaClasses.Music")
                musicImage.Visibility = Visibility.Visible;
            else
                musicImage.Visibility = Visibility.Hidden;
            playVideoFromPosition(arg.ElementAt(x).Path, t); 
            //TODO: CANCEL THE RANDOM PLAY - ROY
        }
        #endregion helper methods

    }
}
