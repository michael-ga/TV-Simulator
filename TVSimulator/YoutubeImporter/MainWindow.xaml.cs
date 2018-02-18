using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YoutubeImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window    {


        #region Fields
        
        Search searcher = new Search();
        Database db;

        YouTubeChannel currChannelSelection = null;
        YoutubeVideo currVideoSelection = null;
        YoutubePlaylist currentPlaylistSelection = null;
        YoutubePlaylistChannel currentPlaylistChannelSelection = null;


        private List<YouTubeChannel> _channels;
        private List<YoutubePlaylist> _playlists;
        private List<YoutubePlaylistChannel> _playlistChannels;

        private int type;
        
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
            try
            {
                //Channels = await searcher.channelSearch(SearchBox.Text, 30);
                Playlists = await searcher.playlistSearch(SearchBox.Text, 30);
                //mListView.ItemsSource = Channels;
                mListView.ItemsSource = Playlists;
            }
            catch (Exception)
            {
                MessageBox.Show("Error: please check your connection");
            }
        }

        private async void addChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currChannelSelection == null || mListView.ItemsSource != Channels)
                return;
            var playlistChanel = searcher.getPlayListChannel(currChannelSelection);
            if (playlistChanel.Playlist_list.Count > 0)
                db.insertPlaylistChannel(playlistChanel);
            if (!(db.insertYoutubechannel(currChannelSelection)))
                Debug.WriteLine("not added");
            else
                Debug.WriteLine("Added");

        }

        private void removeChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currChannelSelection == null ||  mListView.ItemsSource != Channels)
                return;

            bool res = db.removeElementByIDFromCollection(Constants.YOUTUBE_CHANNEL_COLLECTION, currChannelSelection.Path);
            if (res)
                Channels = db.getYoutubeChannelList();
            else
                Debug.WriteLine("nothing happened");
            // remove playlist channels related
            if (currentPlaylistChannelSelection == null)
                return;
            res = db.removeElementByIDFromCollection(Constants.YOUTUBE_PLAYLIST_CHANNEL_COLLECTION, currChannelSelection.Path);
            if (res)
                Channels = db.getYoutubeChannelList();
            else
                Debug.WriteLine("nothing happened");
        }

        
        private  async void showVideosBtn_Click(object sender, RoutedEventArgs e)    //for testing
        {
            if(type == (int)SelectionType.playlist && currentPlaylistSelection != null)
                Videos = await searcher.GetVideosFromPlaylistAsync(currentPlaylistSelection.Path);

            if (type == (int)SelectionType.channel && currChannelSelection != null)
                Videos = await searcher.GetVideosFromChannelAsync(currChannelSelection.Path);
        }

        private void showMyChannelsBtn_Click(object sender, RoutedEventArgs e)
        {
            Channels = db.getYoutubeChannelList();
        }


        private void showPlaylistClick(Object sender, RoutedEventArgs e)
        {
            if (type == (int)SelectionType.channel && currChannelSelection != null)
            {
                var playlistChannel = db.getPlayListChannelByChannelID(currChannelSelection.Path);
                if(playlistChannel != null && playlistChannel.Playlist_list.Count>0)
                {
                    var id = playlistChannel.Playlist_list[0].Path;
                    Playlists = searcher.getPlayList_ListFromChannel(currChannelSelection.Path);
                    mListView.ItemsSource = Playlists;
                }
            }
            if( type == (int)SelectionType.playlistChannel )
            {
                Playlists = currentPlaylistChannelSelection.Playlist_list;
            }

        }
        #endregion


        #region Helpers and Listeners
        public void selectedHandler(Object sender, SelectionChangedEventArgs e)
        {
            if ( type == (int)SelectionType.channel)
                currChannelSelection = ((YouTubeChannel)((ListView)e.Source).SelectedItem);
            if ( type == (int)SelectionType.videos)
                currVideoSelection = ((YoutubeVideo)((ListView)e.Source).SelectedItem);
            if ( type == (int)SelectionType.playlist)
                currentPlaylistSelection = ((YoutubePlaylist)((ListView)e.Source).SelectedItem);
            if (type == (int)SelectionType.playlistChannel)
                currentPlaylistChannelSelection = ((YoutubePlaylistChannel)((ListView)e.Source).SelectedItem);
        }



        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                Search_Click(sender, new RoutedEventArgs());
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlaylistChannels = db.getPlaylistChannels();
        }


        #region List Properties definition

        private enum SelectionType
        {
            channel,
            videos,
            playlist,
            playlistChannel
        }


        public List<YouTubeChannel> Channels
        {
            get { return _channels; }
            set
            {
                type = (int)SelectionType.channel;
                _channels = value;
                mListView.ItemsSource = Channels;
            }

        }

        public List<YoutubePlaylist> Playlists
        {
            get { return _playlists; }
            set
            {
                type = (int)SelectionType.playlist;
                _playlists = value;
                mListView.ItemsSource = Playlists;
            }

        }

        private List<YoutubeVideo> videos;
        public List<YoutubeVideo> Videos
        {
            get { return videos; }
            set
            {
                type = (int)SelectionType.videos;
                videos = value;
                mListView.ItemsSource = Videos;
            }

        }

        public List<YoutubePlaylistChannel> PlaylistChannels
        {
            get { return _playlistChannels; }
            set
            {
                type = (int)SelectionType.playlistChannel;
                _playlistChannels = value;
                mListView.ItemsSource = _playlistChannels;
            }
        }
        #endregion
        // add single playlist as a channel!
        private async void addPlaylistChanneleBtn_Click(object sender, RoutedEventArgs e)
        {
            if(type == (int)SelectionType.playlist)
            {
                var tmp = currentPlaylistSelection;
                YoutubePlaylistChannel a = new YoutubePlaylistChannel(tmp.Path,tmp.Name,"","",tmp.PhotoURL);
                a.Playlist_list = new List<YoutubePlaylist>();
                a.Playlist_list.Add(new YoutubePlaylist(tmp.Path, tmp.Name, "", "", tmp.PhotoURL));
                a.Playlist_list[0].Videos = await searcher.GetVideosFromPlaylistAsync(tmp.Path);
                db.insertPlaylistChannel(a);
            }
        }
    }
}
