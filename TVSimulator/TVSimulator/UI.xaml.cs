using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
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
        public bool isSubfolders = true, IsFullscreen=false;
        private bool isLocal = true;
        private FileImporter fileImporter;
        private bool infoPressed = true;
        public event EventHandler Tick;

        public DateTime timeNow;
        public Database db;
        public ChannelsBuilder cb = new ChannelsBuilder();
        private Channel currentChannel;
        public int curChannelNum = 1;
        #endregion fields

        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            fileImporter = new FileImporter();
            fileImporter.OnVideoLoaded += onVideoRecievedHandler;
            //chooseFolderBtn_Click(new object(), new RoutedEventArgs());
            cb.buildLocalChannels();
        }

        #region button listeners

       


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

        

        private void Channel_Up_Click(object sender, RoutedEventArgs e)
        {
            if(cb.LocalChannels == null)
            {
                System.Windows.MessageBox.Show("no channels");
                return;
            }

            curChannelNum = switchChannel(curChannelNum,1);         // second paramter 1 for increament
            int index = parseChanneltoIndex(curChannelNum);

            var c = cb.LocalChannels.ElementAt(index);
            playFromChannel(c);
        }

        private void Channel_Down_Click(object sender, RoutedEventArgs e)
        {
            if(cb.LocalChannels == null)
            {
                System.Windows.MessageBox.Show("no channels");
                return;
            }
            curChannelNum = switchChannel(curChannelNum, -1);    // second paramter -1 for decreament
            int index = parseChanneltoIndex(curChannelNum);
            var c = cb.LocalChannels.ElementAt(index);
            playFromChannel(c);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.ToolWindow;
            }
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
            mediaPlayer.Volume = volumeSlider.Value/60;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        #endregion Media player functions
        #region Youtube media player functions
        private async void playYoutubeChannel(Channel curChannel)
        {

            try
            {
                var searcher = new YoutubeImporter.Search();
                currentChannel.YoutubeVideoList = await searcher.GetVideosFromChannelAsync(curChannel.YoutubeChannelID);
                YtbTxtVideoId.Text = currentChannel.YoutubeVideoList[currentChannel.YoutubeVideoIndex].Path;
                lblMediaName.Content = currentChannel.YoutubeVideoList[currentChannel.YoutubeVideoIndex].Name;
                lblBroadcastNow.Content = currentChannel.YoutubeVideoList[currentChannel.YoutubeVideoIndex].Name;
                lblStartTime.Content = "";
                lblEndTime.Content = "";
                lblChannelNumber.Content = curChannel.ChannelNumber;
                if (currentChannel.YoutubeVideoList[currentChannel.YoutubeVideoIndex+1] != null)
                     lblBroadcastNext.Content = currentChannel.YoutubeVideoList[currentChannel.YoutubeVideoIndex+1].Name;
                lblBroadcastNow.Content = currentChannel.YoutubeVideoList[currentChannel.YoutubeVideoIndex].Name;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Connection Error: please check your internter connction");
            }
        }

        
        #endregion

        #region subMethods

        private void switchMediaControl(Constants.playerSwitch mode)
        {
            if (mode == Constants.playerSwitch.Youtube)
            {
                youtubePlayer.Visibility = Visibility.Visible;
                mediaPlayer.Stop();
                mediaPlayer.Visibility = Visibility.Hidden;
            }
            else
            {
                mediaPlayer.Visibility = Visibility.Visible;
                mediaPlayer.Play();
                if(youtubePlayer != null)
                {
                    youtubePlayer.Visibility = Visibility.Hidden;
                    youtubePlayer.Stop();
                }
            }

            
        }


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
            if (point.Y > heigth - menuBar.Height -100)
                menuBar.Visibility = Visibility.Visible;
            else
                menuBar.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (cb.LocalChannels == null || cb.LocalChannels.Count < 1)
                return;

            var index = parseChanneltoIndex(curChannelNum);
            var c0 = cb.LocalChannels.ElementAt(index);
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
            currentChannel = curChannel;
            if (curChannel.TypeOfMedia.Equals(Constants.YOUTUBE_CHANNEL))
            {
                switchMediaControl(Constants.playerSwitch.Youtube);
                playYoutubeChannel(curChannel);
            }
            else
            {
                switchMediaControl(Constants.playerSwitch.Local);
                playLocalChannel(curChannel);
            }
        }

        private void playLocalChannel(Channel curChannel)
        {
            
            if (curChannel == null || curChannel.DurationList.Count < 1)
            {
                System.Windows.MessageBox.Show("Error playing channel");
                return;
            }
            var durLength = curChannel.DurationList.Count();
            var totalDur = curChannel.DurationList.ElementAt(durLength - 1);

            var a = DateTime.Parse(Constants.START_CYCLE);
            var b = DateTime.Now;
            var diff = ((b.Subtract(a)).TotalSeconds) % totalDur;

            for (var i = 0; i < curChannel.DurationList.Count(); i++)
            {
                if (diff < curChannel.DurationList.ElementAt(i))
                {
                    TimeSpan t;
                    int sec,min;
                    if (i == 0)
                    {
                        sec = (int)diff;
                        t = new TimeSpan(0, 0, sec);
                        playVideoFromPosition(curChannel.Media.ElementAt(i).Path, t);
                        min = (int)t.TotalMinutes;
                        changeLabels(curChannel, min, i);
                        return;
                    }
                    else
                    {
                        sec = (int)diff - (int)curChannel.DurationList.ElementAt(i - 1);
                        t = new TimeSpan(0, 0, sec);
                        playVideoFromPosition(curChannel.Media.ElementAt(i).Path, t);
                        min = (int)t.TotalMinutes;
                        changeLabels(curChannel, min, i);
                        return;
                    }
                }
            }
        }

        private void changeLabels(Channel c,int time,int mediaNum)
        {
            
            lblChannelNumber.Content = c.ChannelNumber;
            string strMediaName = c.Genre + " - " +c.Media.ElementAt(mediaNum).Name;
            string broadCastNow = "Now: " + c.Media.ElementAt(mediaNum).Name;
            string broadCastNext = "";

            if (mediaNum < c.Media.Count() - 1)
                broadCastNext = "Next: " + c.Media.ElementAt(mediaNum + 1).Name;
            else
                broadCastNext = "Next: " + c.Media.ElementAt(0).Name;

            if (c.TypeOfMedia.Equals(Constants.TVSERIES))
            {
                TvSeries tvNow = c.Media.ElementAt(mediaNum) as TvSeries;
                TvSeries tvNext;
                strMediaName += " - Season " + tvNow.Season + " Episode " + tvNow.Episode;
                broadCastNow += " - Season " + tvNow.Season + " Episode " + tvNow.Episode;
                if (mediaNum < c.Media.Count() - 1)
                    tvNext = c.Media.ElementAt(mediaNum + 1) as TvSeries;
                else
                    tvNext = c.Media.ElementAt(0) as TvSeries;

                broadCastNext += " - Season " + tvNext.Season + " Episode " + tvNext.Episode;
            }
            lblMediaName.Content = strMediaName;
            lblBroadcastNow.Content = broadCastNow;
            lblBroadcastNext.Content = broadCastNext;
            

            //change times labels
            var b = DateTime.Now;
            lblStartTime.Content = (b.AddMinutes(time*(-1))).ToShortTimeString();
            double x;
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

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        public int parseChanneltoIndex(int channelNumber)
        {
            if (channelNumber == cb.LocalChannels.Count())
                return cb.LocalChannels.Count() - 1;
            else if (channelNumber == 1)
                return 0;
            else
                return channelNumber - 1;
        }
        private void playNextYoutubeVideo()    
        {
            Dispatcher.Invoke(new Action(() => youtubePlayer.AutoPlay = true));
           currentChannel.YoutubeVideoIndex++;
           Dispatcher.Invoke(new Action(() => YtbTxtVideoId.Text = currentChannel.YoutubeVideoList[currentChannel.YoutubeVideoIndex].Path));    // play the next video from the list
        }

        private void youtubePlayer_videoEnded(object sender, EventArgs e)
        {
            playNextYoutubeVideo();
        }

        public int switchChannel(int channelNumber, int incOrDec)
        {
            var c = cb.LocalChannels;

            if(incOrDec == 1)
            {
                if (channelNumber == c.Count())
                    return 1;
                else
                    return channelNumber + 1;
            }
            else
            {
                if (channelNumber == 1)
                    return c.Count();
                else
                    return channelNumber - 1;
            }
        }


        #endregion helper methods
    }
}
