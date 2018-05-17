using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            _channels = new List<YouTubeChannel>();

            mListView.ItemsSource = Channels;
            mListView.SelectionChanged += selectedHandler;
        } 
        #endregion

        #region Button Listeners

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Equals(""))
                return;
            try
            {
                if(isPlaylistMod.IsChecked == true)
                {
                    Playlists = await searcher.playlistSearch(SearchBox.Text, 30);
                    mListView.ItemsSource = Playlists;
                }
                else
                {
                    Channels = await searcher.channelSearch(SearchBox.Text, 30);
                    mListView.ItemsSource = Channels;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error: please check your connection");
            }
        }
        // adding only channel info and not loading video yet, 
        // adding also playlist channel if channel has playlists
        // playlist channel contain only playlist info and not loading videos
        private void addChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (type != (int)SelectionType.channel)
                return;
            var playlistChanel = searcher.getPlayListChannel(currChannelSelection);
            if (playlistChanel != null && playlistChanel.Playlist_list != null && playlistChanel.Playlist_list.Count > 0)
            {
                if(!db.insertPlaylistChannel(playlistChanel))
                    Debug.WriteLine("not added");
                else
                    Debug.WriteLine("Playlist Channel Added");

            }
            if (!(db.insertYoutubechannel(currChannelSelection)))
                Debug.WriteLine("channel not added");
            else
                Debug.WriteLine("channel Added");

        }

        private void removeChannelBtn_Click(object sender, RoutedEventArgs e)
        {
           
            if(type == (int)SelectionType.channel)
            {
                bool res = db.removeElementByIDFromCollection(Constants.YOUTUBE_CHANNEL_COLLECTION, currChannelSelection.Path);
                if (res)
                    Channels = db.getYoutubeChannelList();
                else
                    Debug.WriteLine("nothing happened");
            }
            // remove playlist channels related
            if (type == (int)SelectionType.playlistChannel)
            {
                bool res1 = db.removeElementByIDFromCollection(Constants.YOUTUBE_PLAYLIST_CHANNEL_COLLECTION, currentPlaylistChannelSelection.Path);
                if (res1)
                    PlaylistChannels = db.getPlaylistChannels();
                else
                    Debug.WriteLine("nothing happened");
            }
        }

        
        private  async void showVideosBtn_Click(object sender, RoutedEventArgs e)    //for testing
        {
            if(type == (int)SelectionType.playlist && currentPlaylistSelection != null)
            {
                if(currentPlaylistChannelSelection != null)
                {
                    var temp = db.getPlayListByPlaylistID(currentPlaylistChannelSelection.Path,currentPlaylistSelection.Path);
                    if (temp != null)
                        PlaylistVideo = temp.Videos;
                }
                else
                    PlaylistVideo = await searcher.GetVideosFromPlaylistAsync(currentPlaylistSelection.Path);
            }

            if (type == (int)SelectionType.channel && currChannelSelection != null)
            {
                if (currChannelSelection.VideoList != null)
                    Videos = currChannelSelection.VideoList;
                else
                    Videos = await searcher.GetVideosFromChannelAsync(currChannelSelection.Path);
            }
        }

        private void showMyChannelsBtn_Click(object sender, RoutedEventArgs e)
        {
            Channels = db.getYoutubeChannelList();
        }


        private void showPlaylistClick(Object sender, RoutedEventArgs e)
        {
            //if (type == (int)SelectionType.channel && currChannelSelection != null)
            //{
            //    if (currChannelSelection.p)
            //    var playlistChannel = db.getPlayListChannelByChannelID(currChannelSelection.Path);
            //    if(playlistChannel != null && playlistChannel.Playlist_list.Count>0)
            //    {
            //        var id = playlistChannel.Playlist_list[0].Path;
            //        Playlists = searcher.getPlayList_ListFromChannel(currChannelSelection.Path);
            //        mListView.ItemsSource = Playlists;
            //    }
            //}
            if( type == (int)SelectionType.playlistChannel )
            {
                if(currentPlaylistChannelSelection !=null && currentPlaylistChannelSelection.Playlist_list != null)
                    Playlists = currentPlaylistChannelSelection.Playlist_list;
            }

        }

        private void syncBtn_Click(object sender, RoutedEventArgs e)
        {
            var t = new Task(() =>  searcher.syncYoutubeChannels());
            //t.ContinueWith(a => 
            t.Start();
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
            playlistChannel,
            playlistVideo
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

        private List<YoutubePlaylistVideo> ytb_playlist_videos;
        public List<YoutubePlaylistVideo> PlaylistVideo
        {
            get { return ytb_playlist_videos; }
            set
            {
                type = (int)SelectionType.playlistVideo;
                ytb_playlist_videos = value;
                mListView.ItemsSource = PlaylistVideo;
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
                YoutubePlaylistChannel a = new YoutubePlaylistChannel(tmp.Path,"Playlist - "+tmp.Name,"","", tmp.PhotoURL);
                a.Playlist_list = new List<YoutubePlaylist>();
                a.Playlist_list.Add(new YoutubePlaylist(tmp.Path, tmp.Name, "", "", tmp.PhotoURL));
                a.Playlist_list[0].Videos = await searcher.GetVideosFromPlaylistAsync(tmp.Path);
                db.insertPlaylistChannel(a);
            }
        }

        
    }
}
