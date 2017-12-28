
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
        public bool isSubfolders = false, IsFullscreen = false;
        private FileImporter fileImporter;
        private bool infoPressed = true;
        public event EventHandler Tick;
        public DateTime timeNow;
        public Database db;
        public ChannelsBuilder cb = new ChannelsBuilder();
        public int curChannelNum = 2;
        #endregion fields


        public MainWindow()
        {
            InitializeComponent();
            fileImporter = new FileImporter();
            fileImporter.OnVideoLoaded += onVideoRecievedHandler;
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
            if (cb.LocalChannels == null)
            {
                System.Windows.MessageBox.Show("no channels");
                return;
            }
            curChannelNum++;
            if (cb.LocalChannels.Count > 0)
            {
                int num = curChannelNum % cb.LocalChannels.Count;
                editChannelNumber.Text = cb.LocalChannels.ElementAt(num).ChannelNumber.ToString();
                var c = cb.LocalChannels.ElementAt(num);
                playFromChannel(c);
            }
        }

        private void Channel_Down_Click(object sender, RoutedEventArgs e)
        {
            if (cb.LocalChannels == null)
            {
                System.Windows.MessageBox.Show("no channels");
                return;
            }
            curChannelNum--;
            if (curChannelNum < 0)
                curChannelNum = cb.LocalChannels.Count;

            //editChannelNumber.Text = cb.LocalChannels.ElementAt(curChannelNum % cb.LocalChannels.Count).ChannelNumber.ToString();
            var c = cb.LocalChannels.ElementAt(curChannelNum % cb.LocalChannels.Count);
            playFromChannel(c);

        }

        #endregion button listeners

        #region Media player functions

        private void playVideoFromPosition(string path, TimeSpan t)
        {
            mediaPlayer.Source = new Uri(path);
            mediaPlayer.Position = t;
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
            mediaPlayer.Volume = volumeSlider.Value / 60;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        #endregion Media player functions

        #region subMethods

        // event handler raised when data of enterred pathes is loaded on fileImporter.
        private void onVideoRecievedHandler(Object o, List<Media> arg)
        {
            cb.buildLocalChannels();
            Window_Loaded(this, new RoutedEventArgs());
            //if (arg.ElementAt(x) is Music)
            //    musicImage.Visibility = Visibility.Visible;
            //else
            //    musicImage.Visibility = Visibility.Hidden;
            //playVideoFromPosition(arg.ElementAt(x).Path, new TimeSpan(0,0,0)); 
            //Random r = new Random();
            //int x = r.Next(arg.Count);
            //folderPathTextbox.Text = arg.ElementAt(x).Name;     //check the name of media


        }

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
            if (point.Y > heigth - menuBar.Height - 100)
                menuBar.Visibility = Visibility.Visible;
            else
                menuBar.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (cb.LocalChannels == null || cb.LocalChannels.Count < 1)
                return;
            var c0 = cb.LocalChannels.ElementAt(curChannelNum);
            editChannelNumber.Text = cb.LocalChannels.ElementAt(curChannelNum).ChannelNumber.ToString();
            playFromChannel(c0);

            timeNow = DateTime.Now;
            lblClock.Content = timeNow.ToShortTimeString();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(15);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void playFromChannel(Channel curChannel)
        {
            if (curChannel == null || curChannel.Media == null || curChannel.DurationList.Count < 1)
            {
                System.Windows.MessageBox.Show("Error playing channel");
                return;
            }
            var durLength = curChannel.DurationList.Count();
            var totalDur = curChannel.DurationList.ElementAt(durLength - 1);
            var s = curChannel.Media.Count;
            var a = DateTime.Parse(Constants.START_CYCLE);
            var b = DateTime.Now;
            var diff = ((b.Subtract(a)).TotalMinutes) % totalDur;

            for (var i = 0; i < curChannel.DurationList.Count(); i++)
            {
                if (diff < curChannel.DurationList.ElementAt(i))
                {
                    TimeSpan t;
                    int min;
                    if (i == 0)
                    {
                        min = (int)diff;
                        t = new TimeSpan(0, min, 0);
                        playVideoFromPosition(curChannel.Media.ElementAt(i).Path, t);
                        changeLabels(curChannel, min, i);
                        return;
                    }
                    else
                    {
                        min = (int)diff - curChannel.DurationList.ElementAt(i - 1);
                        t = new TimeSpan(0, min, 0);
                        playVideoFromPosition(curChannel.Media.ElementAt(i).Path, t);
                        changeLabels(curChannel, min, i);
                        return;
                    }
                }
            }
        }

        private void changeLabels(Channel c, int time, int mediaNum)
        {
            lblChannelNumber.Content = c.ChannelNumber;
            lblMediaName.Content = c.Genre + " - " + c.Media.ElementAt(mediaNum).Name;
            lblBroadcastNow.Content = "Now: " + c.Media.ElementAt(mediaNum).Name;

            if (mediaNum < c.Media.Count() - 1)
                lblBroadcastNext.Content = "Next: " + c.Media.ElementAt(mediaNum + 1).Name;
            else
                lblBroadcastNext.Content = "Next: " + c.Media.ElementAt(0).Name;

            //change times labels
            var b = DateTime.Now;
            lblStartTime.Content = (b.AddMinutes(time * (-1))).ToShortTimeString();
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
            if (c.TypeOfMedia.Equals(Constants.TVSERIES))
            {
                TvSeries t = c.Media.ElementAt(mediaNum) as TvSeries;
                txtDescription.Text = t.Description + "  - IMDB Rating: " + t.ImdbRating;
            }

        }

        private void tickevent(object sender, EventArgs e)
        {
            timeNow = DateTime.Now;
            lblClock.Content = timeNow.ToShortTimeString();
            mediaProgressBar.Value += 15;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (editChannelNumber.Text == null || editChannelNumber.Text == "" || System.Text.RegularExpressions.Regex.IsMatch(editChannelNumber.Text, "[^0-9]"))
                return;
            else
            {
                int num = int.Parse(editChannelNumber.Text);
                if (cb.LocalChannels != null)
                {
                    for (var i = 0; i < cb.LocalChannels.Count; i++)    //this loop dont needed when the app will be ready
                    {
                        if (num == cb.LocalChannels.ElementAt(i).ChannelNumber)
                        {
                            curChannelNum = num;
                            var c = cb.LocalChannels.ElementAt(i);
                            playFromChannel(c);
                            return;
                        }
                    }
                }
            }
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
