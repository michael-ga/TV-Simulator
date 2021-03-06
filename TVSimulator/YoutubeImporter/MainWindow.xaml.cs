﻿using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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

        YouTubeChannel currChannelSelection = null;
        YoutubeVideo currVideoSelection = null;
        YoutubePlaylist currentPlaylistSelection = null;
        YoutubePlaylistChannel currentPlaylistChannelSelection = null;

        private List<YouTubeChannel> _channels;
        private List<YoutubePlaylist> _playlists;
        private List<YoutubePlaylistChannel> _playlistChannels;

        private int type = -1;
        private bool isChannel = false;

        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            _channels = new List<YouTubeChannel>();

            mListView.ItemsSource = Channels;
            mListView.SelectionChanged += selectedHandler;

            mListView_Copy.ItemsSource = myChannels;
            mListView_Copy.SelectionChanged += selectedHandler;
        } 
        #endregion

        #region Button Listeners

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Equals(""))
            {
                mListView.ItemsSource = null;
                return;
            }
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
            {
                isChannel = true;
                showMyChannelsBtn.IsEnabled = false;
                showPlaylist.IsEnabled = true;
                myChannels = db.getYoutubeChannelList();
            }
        }

        private void removeChannelBtn_Click(object sender, RoutedEventArgs e)
        {
           if (currChannelSelection != null && currChannelSelection.Path != null)
            {
                if(type == (int)SelectionType.channel)
                {
                    bool res = db.removeElementByIDFromCollection(Constants.YOUTUBE_CHANNEL_COLLECTION, currChannelSelection.Path);
                    if (res)
                        myChannels = db.getYoutubeChannelList();
                    else
                        return;
                }
            }
           if ( (type == (int)SelectionType.playlistChannel) && currentPlaylistChannelSelection != null)
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
            isChannel = true;
            showMyChannelsBtn.IsEnabled = false;
            showPlaylist.IsEnabled = true;
            myChannels = db.getYoutubeChannelList();
            if (Channels.Count >= 1)
                removeChannelBtn.Visibility = Visibility.Visible;
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
                    if (Playlists.Count >= 1)
                        removeChannelBtn.Visibility = Visibility.Visible;
            }

        }

        

        //private async void syncBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    sync_btn.Click -= syncBtn_Click;

        //    var progressIndicator = new Progress<MyTaskProgressReport>(ReportProgress);
        //    await searcher.syncAllAsyncReportProgress(500, progressIndicator);

        //    sync_btn.Click += syncBtn_Click;

        //}
        #endregion

        private void ReportProgress(MyTaskProgressReport progress)
        {
            progress_lbl.Content = progress.CurrentProgressMessage; 
        }

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
            isChannel = false;
            showPlaylist.IsEnabled = false;
            showMyChannelsBtn.IsEnabled = true;
            PlaylistChannels = db.getPlaylistChannels();
            removeChannelBtn.Visibility = Visibility.Visible;
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

        public List<YouTubeChannel> myChannels
        {
            get { return _channels; }
            set
            {
                type = (int)SelectionType.channel;
                _channels = value;
                mListView_Copy.ItemsSource = myChannels;
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
                mListView_Copy.ItemsSource = _playlistChannels;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
