using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
        private bool promoIsPlay = false;
        public event EventHandler Tick;

        public DateTime timeNow;
        public Database db;
        public ChannelsBuilder cb = new ChannelsBuilder();
        private List<Channel> chanList;
        private Channel currentChannel;
        public int curChannelNum = 1;
        private List<int> indBoard; // indexes in all boards to get results faster - roy
        private Media playNow;
        private Media playNext;

        public Media PlayNow { get => playNow; set => playNow = value; }
        public Media PlayNext { get => playNext; set => playNext = value; }

        #endregion fields

        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            fileImporter = new FileImporter();
            fileImporter.OnVideoLoaded += onVideoRecievedHandler;
            //chooseFolderBtn_Click(new object(), new RoutedEventArgs());
            chanList = db.getChannelList();
            if (chanList.Count() == 0 || chanList == null)
                cb.buildLocalChannels();
            indBoard = getIndexes();
        }

        private List<int> getIndexes()
        {
            List<int> a = new List<int>();
            int i = 0;
            int j = 0;

            DateTime thisDay = DateTime.Now;
            thisDay = thisDay.AddHours(-thisDay.Hour);
            thisDay = thisDay.AddMinutes(-thisDay.Minute);
            thisDay = thisDay.AddSeconds(-thisDay.Second);
            thisDay = thisDay.AddMilliseconds(-thisDay.Millisecond);

            while (i<chanList.Count())
            {
                var temp = chanList[i].BoardSchedule.ElementAt(j).Key;
                if (DateTime.Compare(temp, thisDay) < 0)     //if temp is earlier than finalhourDate
                    j++;
                else
                {
                    i++;
                    if (j == 0)
                        a.Add(0);
                    else
                        a.Add(j - 1);
                    j = 0;
                }
            }
            return a;
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
            mediaPlayer_MediaEnded(mediaPlayer, new RoutedEventArgs());
        }

        private void Channel_Up_Click(object sender, RoutedEventArgs e)
        {
            if(chanList == null)
            {
                System.Windows.MessageBox.Show("no channels");
                return;
            }

            curChannelNum = switchChannel(curChannelNum,1);         // second paramter 1 for increament
            int index = parseChanneltoIndex(curChannelNum);

            var c = chanList[index];
            playFromChannel(c);
        }

        private void Channel_Down_Click(object sender, RoutedEventArgs e)
        {
            if(chanList == null)
            {
                System.Windows.MessageBox.Show("no channels");
                return;
            }
            curChannelNum = switchChannel(curChannelNum, -1);    // second paramter -1 for decreament
            int index = parseChanneltoIndex(curChannelNum);
            var c = chanList[index];
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
        private void onVideoRecievedHandler(Object o, List<Media> arg) { }

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
            if (chanList == null || chanList.Count() < 1)
                chanList = db.getChannelList();

            var index = parseChanneltoIndex(curChannelNum);
            var c0 = chanList[index];
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
            if (curChannel == null)
            {
                System.Windows.MessageBox.Show("Error playing channel");
                return;
            }

            DateTime timeNow = DateTime.Now;
            int i = indBoard[parseChanneltoIndex(curChannelNum)];
            var temp = curChannel.BoardSchedule.ElementAt(i).Key;
        
            while (DateTime.Compare(temp, timeNow) < 0) //if temp is earlier than timeNow
            {
                i++;
                temp = curChannel.BoardSchedule.ElementAt(i).Key;
            }
            var sec = 0;
            var dateShow = new DateTime();
            if (i==0) // incase this time is earlier than the first show in the broad scedule
            {
                playNext = curChannel.BoardSchedule.ElementAt(i).Value;

                System.Windows.MessageBox.Show("the program will start tommorow acordiing to your views setting");
                return;
            }
            else
            {
                playNow = curChannel.BoardSchedule.ElementAt(i-1).Value;
                playNext = curChannel.BoardSchedule.ElementAt(i).Value;
                dateShow = curChannel.BoardSchedule.ElementAt(i - 1).Key;

                var diff = (timeNow.Subtract(dateShow)).TotalSeconds;
                sec = (int)diff;
                var t = new TimeSpan(0, 0, sec);
                playVideoFromPosition(playNow.Path, t);
            }

            TimeSpan dateShowTS = new TimeSpan(dateShow.Hour, dateShow.Minute, 0);

            changeLabels(curChannel, dateShowTS,sec);

        }
        private void changeLabels(Channel c,TimeSpan startTime,int timeLeft)
        {
            lblChannelNumber.Content = c.ChannelNumber;
            string strMediaName = c.Genre + " - " + playNow.Name;
            string broadCastNow = "Now: " + playNow.Name;
            string broadCastNext = "Next: " + playNext.Name;

            if(c.TypeOfMedia.Equals(Constants.TVSERIES))
            {
                TvSeries tvNow = playNow as TvSeries;
                TvSeries tvNext;
                strMediaName += " - Season " + tvNow.Season + " Episode " + tvNow.Episode;
                broadCastNow += " - Season " + tvNow.Season + " Episode " + tvNow.Episode;

                tvNext = playNext as TvSeries;
                broadCastNext += " - Season " + tvNext.Season + " Episode " + tvNext.Episode;
            }
            lblMediaName.Content = strMediaName;
            lblBroadcastNow.Content = broadCastNow;
            lblBroadcastNext.Content = broadCastNext;

            //change times labels 
            var et = startTime.Add(playNow.getDurationTimespan());
            lblStartTime.Content = timesToString(startTime.Hours, startTime.Minutes);
            lblEndTime.Content = timesToString(et.Hours, et.Minutes);

            mediaProgressBar.Maximum = (int)playNow.getDurationTimespan().TotalSeconds * 60;
            mediaProgressBar.Value = timeLeft * 60;
            
            //change Description
            if (c.TypeOfMedia.Equals(Constants.MOVIE))
            {
                Movie m = playNow as Movie;
                txtDescription.Text = m.Description + "  - IMDB Rating: " + m.ImdbRating;
            }
            if (c.TypeOfMedia.Equals(Constants.TVSERIES))
            {
                TvSeries t = playNow as TvSeries;
                txtDescription.Text = t.Name +  " - Season " + t.Season + " Episode " + t.Episode + " - " + t.Description + "  - IMDB Rating: " + t.ImdbRating;
            }
        }

        private String timesToString(int hours, int minutes)
        {
            var str = "";
            if (hours < 10)
                str += "0" + hours;
            else
                str += hours;

            str += ":";

            if (minutes < 10)
                str += "0" + minutes;
            else
                str += minutes;

            return str;   
        }

        private void tickevent(object sender,EventArgs e)
        {
            timeNow = DateTime.Now;
            lblClock.Content = timeNow.ToShortTimeString();
            mediaProgressBar.Value +=900;            
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
            if (channelNumber == chanList.Count())
                return chanList.Count() - 1;
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

        private void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!promoIsPlay)
            {
                string dir = Directory.GetCurrentDirectory();
                dir = dir.Substring(0, dir.IndexOf("TVSimulator")) + "TVSimulator\\TVSimulator\\Resources\\promo.mp4";
                mediaPlayer.Source = new Uri(dir);
                mediaPlayer.Play();
                promoIsPlay = true;

            }
            else
            {
                var index = parseChanneltoIndex(curChannelNum);
                var c0 = chanList[index];
                playFromChannel(c0);
                promoIsPlay = false;

            }

            /*var t = new Task(() =>
            {
                Thread.Sleep(7000);
                
            });
            t.ContinueWith( ()=>
            {
                var index = parseChanneltoIndex(curChannelNum);
                var c0 = chanList[index];
                playFromChannel(c0);
            }
            t.Start();*/
        }

        public int switchChannel(int channelNumber, int incOrDec)
        {
            var c = chanList;

            if(incOrDec == 1)   //increase channel
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
