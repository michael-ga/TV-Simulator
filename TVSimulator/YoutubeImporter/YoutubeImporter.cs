using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TVSimulator;

namespace YoutubeImporter
{
    /// <summary>
    ///  this class handles api calls and data extracting from Youtube Data Api
    /// </summary>
    public class Search
    {
        private string request = @"https://www.googleapis.com/youtube/v3/videos?id=XXX&key=AIzaSyCq2vcpfZaE-pyS5fSALAjNvqVw_rfkCio&part=snippet,contentDetails";
        
        #region Fields and Ctor
        YouTubeService myService;
        public Search()
        {
            myService = getService();
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

        //TODO:: have various queries to get different videos from channel using search types from API.

        #region Queries 
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
        //  get list of all 50 videos from channel
        public async Task<List<YoutubeVideo>> GetVideosFromChannelAsync(string ytChannelId,int maxResults = 30)
        {
            List<SearchResult> res = new List<SearchResult>();
            List<YoutubeVideo> videoList = new List<YoutubeVideo>();
            string dur;
            string nextpagetoken = " ";

            while (nextpagetoken != null)
            {
                var searchListRequest = myService.Search.List("snippet");
                searchListRequest.MaxResults = maxResults;
                searchListRequest.ChannelId = ytChannelId;
                searchListRequest.PageToken = nextpagetoken;
                searchListRequest.Type = "video";

                var searchListResponse = searchListRequest.Execute();   // Call the search.list method to retrieve results matching the specified query term.
                res.AddRange(searchListResponse.Items);     // Process  the video responses 
                nextpagetoken = searchListResponse.NextPageToken;
            }
            foreach (var item in res)
            {
                // problem getting video duration
                dur = await durationReq(item.Id.VideoId.ToString());
                dur = extractDuration(dur);
                YoutubeVideo temp = new YoutubeVideo(item.Id.VideoId.ToString(), item.Snippet.Title, dur, "", ytChannelId,item.Snippet.Thumbnails.Default__.Url);
                videoList.Add(temp);
            }
            return videoList;
        }
        #endregion


        public async Task<string> durationReq(string videoID)
        {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                string req = "https://www.googleapis.com/youtube/v3/videos?id=" + videoID + "&key=AIzaSyCq2vcpfZaE-pyS5fSALAjNvqVw_rfkCio&part=snippet,contentDetails";
                var stringTask = await client.GetStringAsync(req);
                 return stringTask;
        }
        private string extractDuration(string response)
        {
            int x = response.IndexOf("duration")+8;
            x = response.IndexOf("P", x);
            int y = response.IndexOf("\"",x);
            return response.Substring(x,y - x);
        }
    }


    ///// <summary>
    ///// this class get handles database related function fro Importer
    ///// </summary>
    //public class DBManager
    //{
    //    Database db;

    //    public DBManager()
    //    {
    //        db = new Database();
    //    }

    //    public void getChannels()
    //    {
            
    //    }

    //}

}
