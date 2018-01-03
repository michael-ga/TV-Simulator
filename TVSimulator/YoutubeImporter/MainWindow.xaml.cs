using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace YoutubeImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window    {

        #region Fields
        Search searcher = new Search();
        Database db;

        YouTubeChannel currSelection = null;
        YoutubeVideo currVideoSelection = null;
        private List<YouTubeChannel> _channels;
        public List<YouTubeChannel> Channels
        {
            get { return _channels;}
            set
            {
                _channels = value;
                mListView.ItemsSource = Channels;
            }
            
        }

        private List<YoutubeVideo> videos;
        public List<YoutubeVideo> Videos
        {
            get { return videos; }
            set
            {
                videos = value;
                mListView.ItemsSource = Videos;
            }

        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            _channels = new List<YouTubeChannel>();
            
            mListView.ItemsSource = Channels;
            mListView.SelectionChanged += selectedHandler;
        }


        #region Button Listeners
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Equals(""))
                return;
            Channels = await searcher.channelSearch(SearchBox.Text, 30);
            mListView.ItemsSource = Channels;
        }

        private void addChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currSelection == null || mListView.ItemsSource != Channels)
                return;

            if (!(db.insertYoutubechannel(currSelection)))
                Debug.WriteLine("not added");
            else
                Debug.WriteLine("Added");

        }

        private void removeChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currSelection == null ||  mListView.ItemsSource != Channels)
                return;

            var res = db.removeElementByIDFromCollection(Constants.YOUTUBE_CHANNEL_COLLECTION, currSelection.Path);
            if (res)
                Channels = db.getYoutubeChannelList();
            else
                Debug.WriteLine("nothing happened");
        }

        private async void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mListView.ItemsSource == Channels)
            {
                var list = await searcher.GetVideosFromChannelAsync(currSelection.Path);
                if (list[0] == null)
                    return;

                //YoutubeEmbeddedPlayer yte = new YoutubeEmbeddedPlayer(list);
                //yte.Show();
            }
            if (mListView.ItemsSource == Videos)
            {
                string a = await searcher.durationReq(currVideoSelection.Path);
                //YoutubeEmbeddedPlayer yte = new YoutubeEmbeddedPlayer(currVideoSelection.Path);
                //yte.Show();
            }
        }
        private async void showVideosBtn_Click(object sender, RoutedEventArgs e)
        {
            Videos = await searcher.GetVideosFromChannelAsync(currSelection.Path);
        }
        private void showMyChannelsBtn_Click(object sender, RoutedEventArgs e)
        {
            Channels = db.getYoutubeChannelList();
        }
        #endregion


        #region Helpers and Listeners
        public void selectedHandler(Object sender, SelectionChangedEventArgs e)
        {
            if(mListView.ItemsSource == Channels)
                currSelection = ((YouTubeChannel)((ListView)e.Source).SelectedItem);
            if(mListView.ItemsSource == Videos)
                currVideoSelection = ((YoutubeVideo)((ListView)e.Source).SelectedItem);
        }



        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                Search_Click(sender, new RoutedEventArgs());
            }
        } 
        #endregion


    }
}
