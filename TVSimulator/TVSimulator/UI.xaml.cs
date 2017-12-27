
using MediaClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace TVSimulator
{
    public partial class MainWindow : Window
    {
        #region fields
        public bool isSubfolders = false, IsFullscreen=false;
        private FileImporter fileImporter;
        private bool infoPressed = true;
        public event EventHandler Tick;
        public DateTime timeNow;
        public Database db;
        public ChannelsBuilder cb = new ChannelsBuilder();
        #endregion fields


        public MainWindow()
        {
            InitializeComponent();
            fileImporter = new FileImporter();
            //fileImporter.OnVideoLoaded += onVideoRecievedHandler;
            cb.buildLocalChannels();
            
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

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            if (infoPressed)
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

        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
        }

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

        private void mainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                volumeSlider.Value += 1;
            else
                volumeSlider.Value -= 1;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = volumeSlider.Value/60;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        #endregion Media player functions

        #region subMethods

        // event handler raised when data of enterred pathes is loaded on fileImporter.
       /* private void onVideoRecievedHandler(Object o, List<Media> arg)
        {
            db.insertByType(arg);
            Random r = new Random();
            int x = r.Next(arg.Count);
            folderPathTextbox.Text = arg.ElementAt(x).Name;     //check the name of media

            /*if (arg.ElementAt(x) is Music)
                musicImage.Visibility = Visibility.Visible;
            else
                musicImage.Visibility = Visibility.Hidden;
            playVideoFromPosition(arg.ElementAt(x).Path, new TimeSpan(0,0,0)); 
        }*/

        //for get the mouse point
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            playFromChannel();
            
            //mediaPlayer.Play();
            timeNow = DateTime.Now;
            lblClock.Content = timeNow.ToShortTimeString();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(15);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void playFromChannel()
        {
            var channel1 = cb.LocalChannels.ElementAt(1);
            var durLength = channel1.DurationList.Count();
            var totalDur = channel1.DurationList.ElementAt(durLength - 1);

            var a = DateTime.Parse(Constants.START_CYCLE);
            var b = DateTime.Now;
            var diff = ((b.Subtract(a)).TotalMinutes) % totalDur;

            for(var i=0;i<channel1.DurationList.Count();i++)
            {
                if(diff < channel1.DurationList.ElementAt(i))
                {
                    TimeSpan t;
                    int min;
                    if (i == 0)
                    {
                        min = (int)diff;
                        t = new TimeSpan(0, min, 0);
                        playVideoFromPosition(channel1.Media.ElementAt(i).Path, t);
                        changeLabels(channel1, min, i);
                        return;
                    }
                    else
                    {
                        min = (int)diff - channel1.DurationList.ElementAt(i - 1);
                        t = new TimeSpan(0,min, 0);
                        playVideoFromPosition(channel1.Media.ElementAt(i).Path, t);
                        changeLabels(channel1,min,i);
                        return;
                    }
                }
            }
        }

        private void changeLabels(Channel c,int time,int mediaNum)
        {
            lblChannelNumber.Content = c.ChannelNumber;
            lblMediaName.Content = c.Genre + " - " +c.Media.ElementAt(mediaNum).Name;
            lblBroadcastNow.Content = "Now: " + c.Media.ElementAt(mediaNum).Name;
          
            if (mediaNum < c.Media.Count()-1)
                lblBroadcastNext.Content = "Next: " + c.Media.ElementAt(mediaNum + 1).Name;
            else
                lblBroadcastNext.Content = "Next: " + c.Media.ElementAt(0).Name;

            //change times labels
            var b = DateTime.Now;
            lblStartTime.Content = (b.AddMinutes(time*(-1))).ToShortTimeString();
            int x;
            if (mediaNum != 0)
            {
                x = c.DurationList.ElementAt(mediaNum) - c.DurationList.ElementAt(mediaNum - 1) - time;
                mediaProgressBar.Maximum = (c.DurationList.ElementAt(mediaNum) - c.DurationList.ElementAt(mediaNum - 1)) * 60;
            }
            else
            {
                mediaProgressBar.Maximum = c.DurationList.ElementAt(mediaNum) * 60;
                x = c.DurationList.ElementAt(mediaNum) - time;
            }
            mediaProgressBar.Value = time * 60;
        
            lblEndTime.Content = b.AddMinutes(x).ToShortTimeString();

            //change Description
            if (c.TypeOfMedia.Equals(Constants.MOVIE))
            {
                Movie m = c.Media.ElementAt(mediaNum) as Movie;
                txtDescription.Text = m.Description + "  - IMDB Rating: " + m.ImdbRating;
            }
            if(c.TypeOfMedia.Equals(Constants.TVSERIES))
            {
                TvSeries t = c.Media.ElementAt(mediaNum) as TvSeries;
                txtDescription.Text = t.Description + "  - IMDB Rating: " + t.ImdbRating;
            }
            
        }

        private void tickevent(object sender,EventArgs e)
        {
            timeNow = DateTime.Now;
            lblClock.Content = timeNow.ToShortTimeString();
            mediaProgressBar.Value +=15;            
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
