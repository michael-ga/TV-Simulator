using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using HelperClasses;
using MediaClasses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using static YoutubeImporter.MainWindow;

namespace YoutubeImporter
{
    
    /// <summary>
    ///  this class handles api calls and data extracting from Youtube Data Api
    /// </summary>
    public class Search
    {
        #region Fields and Ctor
        private YouTubeService myService;
        private Database db;
        private int channelsToSync;
        private int chanelvideoCount,playlistVideoCount;
        private int channelAmount,playlistAmount;

       
        public Search()
        {
            myService = getService();
            db = new Database();
        }

        private void progressChangeEvent(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void fetchData(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region OAuth
        // authentication
        private YouTubeService getService()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCq2vcpfZaE-pyS5fSALAjNvqVw_rfkCio",
                ApplicationName = this.GetType().ToString()
            });
            return youtubeService;
        }
        #endregion

        #region Channel Queries 
        // this function returns list of channels result from query.
        public async Task<List<YouTubeChannel>> channelSearch(string searchValue, int maxResults = 50)
        {
            var searchListRequest = myService.Search.List("snippet");
            searchListRequest.Q = searchValue; // searchListRequest.Q -> search query takes every word with OR operator a|b 
            searchListRequest.Type = "channel"; // optional values are "channel","video","playlist"
            searchListRequest.MaxResults = maxResults;

            var searchListResponse = await searchListRequest.ExecuteAsync();    // Call the search.list method to retrieve results matching the specified query term.
            List<YouTubeChannel> channels = new List<YouTubeChannel>();
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#channel":
                        YouTubeChannel temp = new YouTubeChannel(searchResult.Snippet.ChannelId, searchResult.Snippet.ChannelTitle, "", "", searchResult.Snippet.Thumbnails.Default__.Url);
                        channels.Add(temp);
                        break;
                }
            }
            return channels;
        }


        //  get list of all  videos from channel
        public async Task<List<YoutubeVideo>> GetVideosFromChannelAsync(string ytChannelId)
        {
            List<SearchResult> res = new List<SearchResult>();
            List<SearchResult> res1 = new List<SearchResult>();
            List<YoutubeVideo> videoList = new List<YoutubeVideo>();
            string dur;
            string nextpagetoken = " ";

            while (nextpagetoken != null)
            {
                var searchListRequest = myService.Search.List("snippet");
                searchListRequest.MaxResults = 30;
                searchListRequest.ChannelId = ytChannelId;
                searchListRequest.PageToken = nextpagetoken;
                searchListRequest.Order = Google.Apis.YouTube.v3.SearchResource.ListRequest.OrderEnum.Date;// otions are date rating relevance  title videoCount viewCount 
                searchListRequest.Type = "video";

                var searchListResponse = searchListRequest.Execute();   // Call the search.list method to retrieve results matching the specified query term.
                res.AddRange(searchListResponse.Items);     // Process  the video responses 
                nextpagetoken = searchListResponse.NextPageToken;
            }
            channelAmount = res.Count;
            foreach (var item in res)
            {
                dur = await durationReq(item.Id.VideoId.ToString());
                dur = extractDuration(dur);
                YoutubeVideo temp = new YoutubeVideo(item.Id.VideoId.ToString(), item.Snippet.Title, dur, "", ytChannelId, extractThumbnail(item.Snippet),item.Snippet.Description);
                videoList.Add(temp);
                chanelvideoCount++;
                System.Diagnostics.Debug.WriteLine(videoList.Count);
            }
            return videoList;
        }

        #endregion Channel Queries 

        #region Playlist Queries
        // this function returns list of playlists result from query.
        public async Task<List<YoutubePlaylist>> playlistSearch(string searchValue, int maxResults = 50)
        {
            var searchListRequest = myService.Search.List("snippet");
            searchListRequest.Q = searchValue; // searchListRequest.Q -> search query takes every word with OR operator a|b 
            searchListRequest.Type = "playlist"; // optional values are "channel","video","playlist"
            searchListRequest.MaxResults = maxResults;
            searchListRequest.Order = Google.Apis.YouTube.v3.SearchResource.ListRequest.OrderEnum.Relevance;// otions are date rating relevance  title videoCount viewCount 
            var searchListResponse = await searchListRequest.ExecuteAsync();    // Call the search.list method to retrieve results matching the specified query term.
            List<YoutubePlaylist> playlists = new List<YoutubePlaylist>();
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#playlist":
                        YoutubePlaylist temp = new YoutubePlaylist(searchResult.Id.PlaylistId, searchResult.Snippet.Title, "", "", searchResult.Snippet.Thumbnails.Default__.Url);
                        playlists.Add(temp);
                        break;
                }
            }
            return playlists;
        } 


       

        //  get list of all  videos from playlist
        public async Task<List<YoutubePlaylistVideo>> GetVideosFromPlaylistAsync(string playlistId)
        {
            List<PlaylistItem> res = new List<PlaylistItem>();
            List<SearchResult> res1 = new List<SearchResult>();
            //List<YoutubeVideo> videoList = new List<YoutubeVideo>();
            List<YoutubePlaylistVideo> videoList = new List<YoutubePlaylistVideo>();
            int episodeIndex =0 ;
            string episodeVal;

            string dur;
            string nextpagetoken = " ";

            while (nextpagetoken != null)
            {
                var playlistItemsListRequest = myService.PlaylistItems.List("snippet");
                playlistItemsListRequest.MaxResults = 30;
                playlistItemsListRequest.PlaylistId = playlistId;
                playlistItemsListRequest.PageToken = nextpagetoken;

                var searchListResponse = playlistItemsListRequest.Execute();   // Call the search.list method to retrieve results matching the specified query term.
                res.AddRange(searchListResponse.Items);     // Process  the video responses 
                nextpagetoken = searchListResponse.NextPageToken;
            }
            playlistAmount = res.Count;
            foreach (var item in res)
            {
                dur = await durationReq(item.Snippet.ResourceId.VideoId);
                dur = extractDuration(dur);
                if (dur != "0")
                {
                    if (episodeIndex < 10)
                        episodeVal = "0" + episodeIndex.ToString();
                    else
                        episodeVal = episodeIndex.ToString();
                    episodeIndex++;
                    YoutubePlaylistVideo temp;
                    var thumbnail = item.Snippet.Thumbnails;
                    if (thumbnail != null && thumbnail.Default__.Url != null)
                        temp = new YoutubePlaylistVideo(item.Snippet.ResourceId.VideoId, item.Snippet.Title, dur, "", playlistId, item.Snippet.Thumbnails.Default__.Url, episodeVal);
                    else
                        temp = new YoutubePlaylistVideo(item.Snippet.ResourceId.VideoId, item.Snippet.Title, dur, "", playlistId,"", episodeVal);

                    videoList.Add(temp);
                    playlistVideoCount++;
                    System.Diagnostics.Debug.WriteLine(videoList.Count);
                }
            }
            return videoList;
        }



        // TODO:: EDGE CASE check when there is no playlists in channel. 
        public YoutubePlaylistChannel getPlayListChannel(YouTubeChannel ytChannel)
        {
            string nextpagetoken = " ";
            List<SearchResult> res = new List<SearchResult>();
            List<YoutubePlaylist> playlists = new List<YoutubePlaylist>();
            try
            {
                YoutubePlaylistChannel newChannel = new YoutubePlaylistChannel(ytChannel.Path, "Playlist - " + ytChannel.Name, "", "", ytChannel.PhotoURL);

                while (nextpagetoken != null)
                {
                    var searchListRequest = myService.Search.List("snippet");
                    searchListRequest.MaxResults = 30;
                    searchListRequest.ChannelId = ytChannel.Path;
                    searchListRequest.PageToken = nextpagetoken;
                    searchListRequest.Order = Google.Apis.YouTube.v3.SearchResource.ListRequest.OrderEnum.Date;// otions are date rating relevance  title videoCount viewCount 
                    searchListRequest.Type = "playlist";

                    var searchListResponse = searchListRequest.Execute();   // Call the search.list method to retrieve results matching the specified query term.
                    res.AddRange(searchListResponse.Items);     // Process  the video responses 
                    nextpagetoken = searchListResponse.NextPageToken;
                }

                for (int i = 0; i < res.Count; i++)
                {
                    var temp = new YoutubePlaylist(res[i].Id.PlaylistId, res[i].Snippet.Title, "", "", extractThumbnail(res[i].Snippet));
                    playlists.Add(temp);
                }
                newChannel.Playlist_list = playlists;
                return newChannel;
            }
            catch (Exception)
            {

                return null;
            }
        }

        // TODO:: EDGE CASE check when there is no playlists in channel. 
        public List<YoutubePlaylist> getPlayList_ListFromChannel(string ytChannelid)
        {
            string nextpagetoken = " ";
            List<SearchResult> res = new List<SearchResult>();
            List<YoutubePlaylist> playlists = new List<YoutubePlaylist>();

            while (nextpagetoken != null)
            {
                var searchListRequest = myService.Search.List("snippet");
                searchListRequest.MaxResults = 30;
                searchListRequest.ChannelId = ytChannelid;
                searchListRequest.PageToken = nextpagetoken;
                searchListRequest.Order = Google.Apis.YouTube.v3.SearchResource.ListRequest.OrderEnum.Date;// otions are date rating relevance  title videoCount viewCount 
                searchListRequest.Type = "playlist";

                var searchListResponse = searchListRequest.Execute();   // Call the search.list method to retrieve results matching the specified query term.
                res.AddRange(searchListResponse.Items);     // Process  the video responses 
                nextpagetoken = searchListResponse.NextPageToken;
            }

            for (int i = 0; i < res.Count; i++)
            {
                var temp = new YoutubePlaylist(res[i].Id.PlaylistId, res[i].Snippet.Title, "", "", extractThumbnail(res[i].Snippet));
                playlists.Add(temp);
            }
            return playlists;
        }
        #endregion

        #region SYNC


        public async Task syncAllAsyncReportProgress(int sleepTime, IProgress<MyTaskProgressReport> progress)
        {
            int retries = Constants.YOUTUBE_PROGRESS_REPORT_RETRIES;
            int startCondition = 10;
            bool isStarted = false;
            var t = new Task(() =>{var res =  syncAll(); });
            t.Start();
            progress.Report(new MyTaskProgressReport { CurrentProgressAmount = playlistVideoCount, TotalProgressAmount = playlistAmount, CurrentProgressMessage = string.Format("collecting channel data") });

            while (channelsToSync > 0 || channelsToSync == 0 && startCondition-- > 0 && !isStarted)
            {
                if (channelsToSync > 0 && !isStarted )
                    isStarted = true;
                await Task.Delay(sleepTime);

                progress.Report(new MyTaskProgressReport { CurrentProgressAmount = playlistVideoCount, TotalProgressAmount = playlistAmount, CurrentProgressMessage = string.Format("Number of channels remain {0}", channelsToSync) });

                while (chanelvideoCount < channelAmount || channelAmount == 0 && retries-- > 0)
                {
                    await Task.Delay(sleepTime);
                    if (channelAmount == 0)
                    {
                        progress.Report(new MyTaskProgressReport { CurrentProgressAmount = chanelvideoCount, TotalProgressAmount = channelAmount, CurrentProgressMessage = string.Format("Collecting channel data...") });
                    }
                    else
                        progress.Report(new MyTaskProgressReport { CurrentProgressAmount = chanelvideoCount, TotalProgressAmount = channelAmount, CurrentProgressMessage = string.Format("Number of channels remain {0}\nLoaded {1} out of {2} videos", channelsToSync, chanelvideoCount, channelAmount) });
                }
                //progress.Report(new MyTaskProgressReport { CurrentProgressAmount = chanelvideoCount, TotalProgressAmount = channelAmount, CurrentProgressMessage = string.Format("Collecting playlists data") });

                retries = Constants.YOUTUBE_PROGRESS_REPORT_RETRIES;
                while (playlistVideoCount < playlistAmount || playlistAmount == 0 && retries-- > 0)
                {
                    await Task.Delay(sleepTime);
                    if (channelAmount != 0)
                    {
                        progress.Report(new MyTaskProgressReport { CurrentProgressAmount = playlistVideoCount, TotalProgressAmount = playlistAmount, CurrentProgressMessage = string.Format("Loaded {0} out of {1} playlists", chanelvideoCount, channelAmount) });
                    }
                }
                //progress.Report(new MyTaskProgressReport { CurrentProgressAmount = playlistVideoCount, TotalProgressAmount = playlistAmount, CurrentProgressMessage = string.Format("Number of channels to sync: {0}", channelsToSync) });
                if (channelsToSync == 0)
                {
                    await Task.Delay(1000);
                }
            }

            playlistVideoCount = 0;
            playlistAmount = 0;
            channelAmount = 0;
            chanelvideoCount = 0;
            progress.Report(new MyTaskProgressReport { CurrentProgressAmount = playlistVideoCount, TotalProgressAmount = playlistAmount, CurrentProgressMessage = string.Format("") });

        }








        // sync all regular channels - check if need to update and get videos if needed
        public async Task<bool> syncYoutubeChannels()
        {
            List<YouTubeChannel> ytbChannels = db.getYoutubeChannelList();
            foreach (YouTubeChannel channel in ytbChannels)
            {
                await syncOneChannel(channel);
            }
            return true;
        }

        public async Task<bool> syncOneChannel(YouTubeChannel ytbChannel)
        {
            if (DateTime.Now.Subtract(ytbChannel.LastUpdated).Days > 7 || ytbChannel.VideoList == null)     // check if one week has passed since last update
            {
                if (ytbChannel.VideoList != null)       // clear all videos and reload
                    ytbChannel.VideoList.Clear();
                ytbChannel.VideoList = await GetVideosFromChannelAsync(ytbChannel.Path);
                ytbChannel.LastUpdated = DateTime.Now;
                db.updateYoutubeChannel(ytbChannel);
            }
            return true;
        }

        public async Task<bool> syncYoutubePlaylistChannels()
        {
            List<YoutubePlaylistChannel> ytbPlsChannels = db.getPlaylistChannels();
            foreach (YoutubePlaylistChannel channel in ytbPlsChannels)
            {
                await syncOnePlaylistChannel(channel);
            }
            return true;
        }
        public async Task<bool> syncOnePlaylistChannel(YoutubePlaylistChannel plsChannel)
        {
            if (DateTime.Now.Subtract(plsChannel.LastUpdated).Days > 7 || plsChannel.Playlist_list == null)     // check if one week has passed since last update
            {
                foreach (var item in plsChannel.Playlist_list)
                {
                    item.Videos = await GetVideosFromPlaylistAsync(item.Path);
                    plsChannel.LastUpdated = DateTime.Now;
                    db.updateYoutubePlaylistChannel(plsChannel);
                }
            }
            return true;
        }

        public async Task<bool> syncAll()
        {
            List<YoutubePlaylistChannel> ytbPlsChannels = db.getPlaylistChannels();
            List<YouTubeChannel> ytbChannels = db.getYoutubeChannelList();

            channelsToSync = ytbPlsChannels.Count + ytbChannels.Count;
            foreach (YoutubePlaylistChannel channel in ytbPlsChannels)
            {
                await syncOnePlaylistChannel(channel);
                playlistVideoCount = 0;
                playlistAmount = 0;
                channelAmount = 0;
                chanelvideoCount = 0;
                channelsToSync--;
            }

            foreach (YouTubeChannel channel in ytbChannels)
            {
                await syncOneChannel(channel);
                playlistVideoCount = 0;
                playlistAmount = 0;
                channelAmount = 0;
                chanelvideoCount = 0;
                channelsToSync--;
            }
            return true;
        }


        #endregion

        #region HELPER METHODS
        public async Task<string> durationReq(string videoID)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            string req = "https://www.googleapis.com/youtube/v3/videos?id=" + videoID + "&key=AIzaSyCq2vcpfZaE-pyS5fSALAjNvqVw_rfkCio&part=contentDetails";
            var stringTask = await client.GetStringAsync(req);
            return stringTask;
        }
        private string extractDuration(string response)
        {
            try
            {
                dynamic data = JObject.Parse(response);
                string duration = data.items[0].contentDetails.duration;
                TimeSpan ts = System.Xml.XmlConvert.ToTimeSpan(duration);
                return ts.TotalSeconds.ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }

        private void start_stop_sync( bool isStarting)
        {
            playlistVideoCount = playlistAmount= channelAmount= chanelvideoCount = 0;
            if (isStarting)
            {

            }
        }





        private string extractThumbnail(SearchResultSnippet snippet)
        {
            string thumbnailUrl = "";
            var Thumbnails = snippet.Thumbnails;
            if (Thumbnails != null && Thumbnails.Default__ != null)
                thumbnailUrl = Thumbnails.Default__.Url;
            return thumbnailUrl;
        } 
        #endregion
    }
}
