using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MediaClasses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TVSimulator;

namespace YoutubeImporter
{
    /// <summary>
    ///  this class handles api calls and data extracting from Youtube Data Api
    /// </summary>
    public class Search
    {
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
        public List<YoutubeVideo> GetVideosFromChannelAsync(string ytChannelId,int maxResults = 30)
        {
            List<SearchResult> res = new List<SearchResult>();
            List<YoutubeVideo> videoList = new List<YoutubeVideo>();
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
                YoutubeVideo temp = new YoutubeVideo(item.Id.VideoId.ToString(), item.Snippet.Title, item.ETag.Length.ToString(), "", ytChannelId,item.Snippet.Thumbnails.Default__.Url);
                videoList.Add(temp);
            }
            return videoList;
        }
        #endregion
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
