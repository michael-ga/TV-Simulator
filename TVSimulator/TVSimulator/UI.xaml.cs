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
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;


namespace TVSimulator
{
    public partial class MainWindow : Window
    {
        #region fields
        public bool isSubfolders = true, IsFullscreen=true;
        private bool isLocal = true;
        private FileImporter fileImporter;
        private bool infoPressed = true;
        private bool promoIsPlay = false;
        public event EventHandler Tick;
        DispatcherTimer timer;

        public DateTime timeNow;
        public Database db;
        public ChannelsBuilder cb;
        private List<Channel> chanList;
        public Channel currentChannel;
        public int curChannelNum = 1;
        private List<int> indBoard; // indexes in all boards to get results faster - roy
        private Media playNow;
        private Media playNext;
        private double lastVolume;
        private double lastSliderValue;
        private bool isMuted = false;

        public Media PlayNow { get => playNow; set => playNow = value; }
        public Media PlayNext { get => playNext; set => playNext = value; }
        public Channel CurrentChannel { get => currentChannel; }

        private SettingWindow settings;
        #endregion fields

        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            cb = new ChannelsBuilder();
        }

        public MainWindow(Database _db)
        {
            InitializeComponent();
            cb = new ChannelsBuilder();
            db = _db;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initMainWindow();
            if (chanList == null || chanList.Count() < 1)
                chanList = db.getChannelList();

            if (chanList == null || chanList.Count() < 1)
            {
                System.Windows.MessageBox.Show("error: loading channel list has been failed");
                return;
            }
            var index = parseChanneltoIndex(curChannelNum);
            var c0 = chanList[index];
            playFromChannel(c0);

            timeNow = DateTime.Now;
            lblClock.Content = timeNow.ToShortTimeString();


            timer.Interval = TimeSpan.FromSeconds(15);
            timer.Tick += tickevent;
            timer.Start();
        }



        public void forceRebuildChannels()
        {
            cb.rebuildAllChannels();
        }

        private void removeEmtpyScheduleChannels()
        {
            for (int i =0; i< chanList.Count(); i++)
            {
                if (chanList[i].BoardSchedule == null)
                {
                    chanList.RemoveAt(i);
                    for (int j=i; j < chanList.Count(); j++)    // fix channel numbers
                    {
                        chanList[j].ChannelNumber--; 
                    }
                }
            }
        }

        public int getIndexes(int indexOfChannel , DateTime thisDay)
        {
            int j = 0;
            if(chanList[indexOfChannel].BoardSchedule != null)
            {

                var temp = chanList[indexOfChannel].BoardSchedule.ElementAt(j).Key;
                while (DateTime.Compare(temp, thisDay) < 0)     //if temp is earlier than thisDay
                {
                    j++;
                    try
                    {
                        if (chanList[indexOfChannel].BoardSchedule != null && chanList[indexOfChannel].BoardSchedule.ElementAt(j).Key != null)
                            temp = chanList[indexOfChannel].BoardSchedule.ElementAt(j).Key;
                    }
                    catch (Exception)
                    {
                        break;
                       // need to change to nullable dictionary..
                    }
                }
            }
            return j;
        }

        private DateTime getToday()
        {
            return DateTime.Now.Date;
        }
        #region button listeners

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            triggerinfoPressed();
        }

        private void Power_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();  //close all windows
        }

        private void btnBoard_Click(object sender, RoutedEventArgs e)
        {
            boardWindow mw = new boardWindow(this);
            mw.Show();
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (!isMuted)
            {
                lastVolume = mediaPlayer.Volume;
                lastSliderValue = volumeSlider.Value;
                volumeSlider.Value = 0;
                mediaPlayer.Volume = 0;
                youtubePlayer.Volume = 0;
            }
            else
            {
                volumeSlider.Value = lastSliderValue;
                mediaPlayer.Volume = lastVolume;
            }
            isMuted = !isMuted;
        }

        private void Channel_Up_Click(object sender, RoutedEventArgs e)
        {
            if(chanList == null|| chanList.Count() == 0)
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
            if(chanList == null || chanList.Count() == 0)
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
                triggerFullScreen(true);    //send true to recognize escape
            }
            if (e.Key == Key.F11)
            {
                triggerFullScreen(false);   //send true to recognize F11
            }
            IsFullscreen = !IsFullscreen;

        }

        #endregion button listeners

        #region Media player functions

        private void playVideoFromPosition(string path, TimeSpan t)
        {
            errorMessage(false, "");
            mediaPlayer.Source = new Uri(path);
            if(!File.Exists(path))
            {
                errorMessage(true,"This file do not exists");
                return;
            }
            else
            {
                mediaPlayer.Position = t;
                mediaPlayer.Play();
            }
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
            // set youtube volume
            if (youtubePlayer != null)
            {
                youtubePlayer.Volume = (int)volumeSlider.Value;
            }
            if (volumeSlider.Value == 0)
                redX.Visibility = Visibility.Visible;
            else
                redX.Visibility = Visibility.Hidden;

        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        #endregion Media player functions
        #region Youtube media player functions
        private void playYoutubeChannel(Channel curChannel)
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
                temp = curChannel.BoardSchedule.ElementAt(i).Key;       //TODO: need to check if boardschedule is over or not
            }
            var sec = 0;
            var dateShow = new DateTime();
            if (i == 0) // incase this time is earlier than the first show in the broad scedule
            {
                playNext = curChannel.BoardSchedule.ElementAt(i).Value;

                System.Windows.MessageBox.Show("the program will start tommorow acordiing to your views setting");
                return;
            }
            else
            {
                playNow = curChannel.BoardSchedule.ElementAt(i - 1).Value;
                playNext = curChannel.BoardSchedule.ElementAt(i).Value;
                dateShow = curChannel.BoardSchedule.ElementAt(i - 1).Key;

                var diff = (timeNow.Subtract(dateShow)).TotalSeconds;
                sec = (int)diff;
                var t = new TimeSpan(0, 0, sec);

                youtubePlayer.VideoId = playNow.Path;
                youtubePlayer.StartSec = sec.ToString();
            }

            TimeSpan dateShowTS = new TimeSpan(dateShow.Hour, dateShow.Minute, 0);

            changeLabels(curChannel, dateShowTS, sec);

        }

        #endregion
        #region subMethods

        private void switchMediaControl(Constants.playerSwitch mode)
        {
            if (mode == Constants.playerSwitch.Youtube)
            {
                imageMessage.Visibility = Visibility.Hidden;
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

        

        private void playFromChannel(Channel curChannel)
        {
            currentChannel = curChannel;
            try
            {
                if (curChannel.TypeOfMedia.Equals(Constants.MOVIE) || curChannel.TypeOfMedia.Equals(Constants.TVSERIES))// local media..
                {
                    switchMediaControl(Constants.playerSwitch.Local);
                    playLocalChannel(curChannel);

                }
                else  // youtube media
                {
                    switchMediaControl(Constants.playerSwitch.Youtube);
                    playYoutubeChannel(curChannel);
                }
            }
            catch (Exception)
            {
                // handle here load error screen
                errorMessage(true, "handle here load error screen");
            }
        }

        private void playLocalChannel(Channel curChannel)
        {
            if (curChannel == null)
            {
                errorMessage(true,"Error playing channel");
                return;
            }

            DateTime timeNow = DateTime.Now;
            if(curChannel.BoardSchedule == null)
            {
                errorMessage(true, "Error playing channel");
                return;
            }
            DateTime lastDayInBoard = curChannel.BoardSchedule.Keys.Last();
            if(DateTime.Compare(lastDayInBoard, timeNow) < 0)
            {
                if(curChannel.TypeOfMedia.Equals(Constants.MOVIE))
                    curChannel.bs(timeNow);
                if (curChannel.TypeOfMedia.Equals(Constants.TVSERIES))
                    curChannel.bTVs(timeNow);
     
            }

            int i = indBoard[parseChanneltoIndex(curChannelNum)];
            var temp = curChannel.BoardSchedule.ElementAt(i).Key;
        
            while (DateTime.Compare(temp, timeNow) < 0) //if temp is earlier than timeNow
            {
                i++;
                temp = curChannel.BoardSchedule.ElementAt(i).Key;       //TODO: need to check if boardschedule is over or not
            }
            var sec = 0;
            var dateShow = new DateTime();
            if (i==0) // incase this time is earlier than the first show in the broad scedule
            {
                playNext = curChannel.BoardSchedule.ElementAt(i).Value;
                errorMessage(true, "The program will start tommorow acording to your views setting");
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

            if (c.TypeOfMedia.Equals(Constants.TVSERIES))
            {
                TvSeries tvNow = playNow as TvSeries;
                TvSeries tvNext;
                strMediaName += " - Season " + tvNow.Season + " Episode " + tvNow.Episode;
                broadCastNow += " - Season " + tvNow.Season + " Episode " + tvNow.Episode;

                tvNext = playNext as TvSeries;
                broadCastNext += " - Season " + tvNext.Season + " Episode " + tvNext.Episode;
            }   
            else if (!c.TypeOfMedia.Equals(Constants.MOVIE) && c.YoutubeChannelName != "")
            {
                strMediaName = c.YoutubeChannelName;
                var uri = new Uri("pack://application:,,,/Resources/youtubeChannelIcon.png");
                var bitmap = new BitmapImage(uri);
                chanIcon.Source = bitmap;

                var uri1 = new Uri("pack://application:,,,/Resources/youtubeChannel.png");
                var bitmap1 = new BitmapImage(uri1);
                channelImage.Source = bitmap1;
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
                Media m;
                if (c.Genre != "General")
                {
                    m = (Movie)playNow;
                    txtDescription.Text = ((Movie)m).Description + "  - IMDB Rating: " + ((Movie)m).ImdbRating;
                }
                else
                    txtDescription.Text = "No description for this movie";

                var uri = new Uri("pack://application:,,,/Resources/movieIcon.PNG");
                var bitmap = new BitmapImage(uri);
                chanIcon.Source = bitmap;

                var uri1 = new Uri("pack://application:,,,/Resources/movieChannel.png");
                var bitmap1 = new BitmapImage(uri1);
                channelImage.Source = bitmap1;
            }
            if (c.TypeOfMedia.Equals(Constants.TVSERIES))
            {
                TvSeries t = playNow as TvSeries;
                txtDescription.Text = t.Name +  " - Season " + t.Season + " Episode " + t.Episode + " - " + t.Description + "  - IMDB Rating: " + t.ImdbRating;

                var uri = new Uri("pack://application:,,,/Resources/tvSeriesIcon.png");
                var bitmap = new BitmapImage(uri);
                chanIcon.Source = bitmap;

                var uri1 = new Uri("pack://application:,,,/Resources/tvSeriesChannel.PNG");
                var bitmap1 = new BitmapImage(uri1);
                channelImage.Source = bitmap1;
            }
            if(c.TypeOfMedia.Equals("YouTubePlaylistChannel"))
            {
                var uri = new Uri("pack://application:,,,/Resources/ypIcon.png");
                var bitmap = new BitmapImage(uri);
                chanIcon.Source = bitmap;

                var uri1 = new Uri("pack://application:,,,/Resources/ypChannel.png");
                var bitmap1 = new BitmapImage(uri1);
                channelImage.Source = bitmap1;

                txtDescription.Text = "youtube channel";
            }
            //if(c.MChannelType == Channel.channelType.youtube_channel)
            //{
            //    YouTubeChannel ytc = new YouTubeChannel();
            //    txtDescription.Text = ytc.Description;
            //}

            editChannelNumber.Text = "" + c.ChannelNumber;
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
            if (mediaProgressBar.Value == mediaProgressBar.Maximum && currentChannel.MChannelType != Channel.channelType.local)
            {
                Debug.WriteLine("tick event invoke videoEnded");
                mediaProgressBar.Value = 0;
                youtubePlayer_videoEnded(youtubePlayer,new EventArgs());
            }

        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                var num = Int32.Parse(editChannelNumber.Text);
                int index = parseChanneltoIndex(num);
                var c = chanList[index];
                playFromChannel(c);
                curChannelNum = c.ChannelNumber;
            }
            catch(Exception ex)
            {
                return;
            }
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

        }

        private void youtubePlayer_videoEnded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(()=>
            {   // reload the channel to play next video
                Debug.WriteLine("invoking next youtube video");
                Channel_Up_Click(new Object(), new RoutedEventArgs());
                Channel_Down_Click(new Object(), new RoutedEventArgs());
                //playFromChannel(currentChannel);
            });
        }

        private void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!promoIsPlay)
            {
                promoIsPlay = true;
                string dir = Directory.GetCurrentDirectory();
                Thread.Sleep(2000);
                dir = dir.Substring(0, dir.IndexOf("TVSimulator")) + "TVSimulator\\TVSimulator\\Resources\\promo.mp4";
                mediaPlayer.Source = new Uri(dir);

                imageMessage.Visibility = Visibility.Hidden;
                if (!File.Exists(dir))
                {
                    imageMessage.Visibility = Visibility.Visible;
                    return;
                }
                else
                    mediaPlayer.Play();

            }
            else
            {
                promoIsPlay = false;
                var index = parseChanneltoIndex(curChannelNum);
                var c0 = chanList[index];
                playFromChannel(c0);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (settings == null)
            {
                settings = new SettingWindow(this);
                settings.Closing += Settings_Closing;
                settings.Show();
            }
            else
                settings.WindowState=WindowState.Normal;
        }

        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings = null;
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

        public void initMainWindow()
        {

            timer = new DispatcherTimer();
            // reload channels 
            //fileImporter = new FileImporter();
            //fileImporter.OnVideoLoaded += onVideoRecievedHandler;
            //chooseFolderBtn_Click(new object(), new RoutedEventArgs());

            //cb.rebuildAllChannels();

            chanList = db.getChannelList().Distinct().ToList();
            removeEmtpyScheduleChannels();
            //if (chanList.Count() == 0 || chanList == null)
            //    cb.buildLocalChannels();
            indBoard = new List<int>();
            for (var i = 0; i < chanList.Count(); i++)
                indBoard.Add(getIndexes(i, getToday()));
        }

        private void full_screen_btn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            IsFullscreen = true;
        }

        public bool isChannelsExist()
        {
            if ( File.Exists(Constants.DB_FILE_PATH))
            {
                return (db.getChannelList().Count() > 0);
            }
            return false;
        }

        private void triggerFullScreen(bool isEscape)
        {
            if (IsFullscreen && isEscape)
            {
                fullScreenlbl.Visibility = Visibility.Visible;
                this.WindowState = WindowState.Normal;
                //this.WindowStyle = WindowStyle.ToolWindow;
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += hideMesFS;
                timer.Start();
            }
            if (!IsFullscreen && !isEscape)
            {
                fullScreenlbl.Visibility = Visibility.Hidden;
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;

            }
        }

        private void hideMesFS(object sender, EventArgs e)
        {
            fullScreenlbl.Visibility = Visibility.Hidden;
            timer.Stop();
        }

        private void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var value = (sender as System.Windows.Controls.ComboBox).SelectedItem as string;    // get value
            if (value == null)
                return;
            int index = value.IndexOf('-');
            int num = Int32.Parse(value.Substring(0, index));     // parse value to int
            int index1 = parseChanneltoIndex(num);
            var c = chanList[index1];
            playFromChannel(c);
        }

        private void triggerinfoPressed()
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

        private void errorMessage(bool toShow,string msg)
        {
            if (toShow)
            {
                imageMessage.Visibility = Visibility.Visible;
                lblError.Visibility = Visibility.Visible;
                lblError.Content = msg;
            }
            else
            {
                imageMessage.Visibility = Visibility.Hidden;
                lblError.Visibility = Visibility.Hidden;
            }
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion helper methods
    }
}
