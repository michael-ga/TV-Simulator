
using MediaClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        private bool infoPressed = true;
        #endregion fields


        public MainWindow()
        {
            InitializeComponent();
            fileImporter = new FileImporter();
            mediaPlayer.Play();
            //fileImporter.OnVideoLoaded += onVideoRecievedHandler;
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = volumeSlider.Value/100;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        #endregion Media player functions

        #region subMethods

        // event handler raised when data of enterred pathes is loaded on fileImporter.
        /*private void onVideoRecievedHandler(Object o, List<Media> arg)
        {
            Random r = new Random();
            int x = r.Next(arg.Count);
            folderPathTextbox.Text = arg.ElementAt(x).Name;     //check the name of media

            if (arg.ElementAt(x) is Music)
                musicImage.Visibility = Visibility.Visible;
            else
                musicImage.Visibility = Visibility.Hidden;
            playVideoFromPosition(arg.ElementAt(x).Path, new TimeSpan(0,0,0)); 
        }*/


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        private void mainWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var point = GetMousePosition();

            var heigth = System.Windows.SystemParameters.PrimaryScreenHeight;
            if (point.Y > heigth - menuBar.Height)
                menuBar.Visibility = Visibility.Visible;
            else
                menuBar.Visibility = Visibility.Hidden;
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            if(infoPressed)
            {
                statsBox.Visibility = Visibility.Hidden;
                txtDescription.Visibility = Visibility.Visible;
            }
            else
            {
                statsBox.Visibility = Visibility.Visible;
                txtDescription.Visibility = Visibility.Hidden;
            }
            infoPressed = !infoPressed;
            
        }

        private void mainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                volumeSlider.Value += 1;
            else
                volumeSlider.Value -= 1;
        }

        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
        }

        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        #endregion helper methods

    }
}
