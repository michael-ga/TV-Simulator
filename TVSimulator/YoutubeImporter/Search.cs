

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeImporter
{
    public class Search
    {
        YouTubeService myService;

        public async Task< List<YouTubeChannel>> Import(string searchValue, int maxResults = 50)
        {
           return await customChannelSearch(searchValue, maxResults);
                //showListOnWindow(list);
                
        }
            public Search()
            {
                myService = getService();
            }
           // this function returns list of channels result from query.
            public async Task<List<YouTubeChannel>> customChannelSearch(string searchValue,int maxResults = 50)
            {
                var searchListRequest = myService.Search.List("snippet");
                searchListRequest.Q = searchValue; // searchListRequest.Q -> search query takes every word with OR operator a|b 
                searchListRequest.Type = "channel"; // optional values are "channel","video","playlist"
                searchListRequest.MaxResults = maxResults;

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();

                List<YouTubeChannel> channels = new List<YouTubeChannel>();

                // Add each result to the appropriate list, and then display the lists of
                // matching videos, channels, and playlists.
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#channel":
                            YouTubeChannel temp = new YouTubeChannel(searchResult.Snippet.ChannelId, searchResult.Snippet.Title, "", "", searchResult.Snippet.Thumbnails.Default__.Url);
                            channels.Add(temp);
                            break;
                    }
                }
                return channels;
            }

            private void showListOnWindow(List<YouTubeChannel> list)
            {
                


            }
            private YouTubeService getService()
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyCq2vcpfZaE-pyS5fSALAjNvqVw_rfkCio",
                    ApplicationName = this.GetType().ToString()
                });
                return youtubeService;
            }
        }
    


















    public class UploadVideo
        {
            public void UploadVideoa()
            {
             
                try
                {
                    new UploadVideo().Run().Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
           

            private async Task Run()
            {
                UserCredential credential;
                string path = System.IO.Directory.GetCurrentDirectory();
                path = path.Substring(0, path.IndexOf("TVSimulator"))+ @"TVSimulator\resources\client_secrets.json";
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        // This OAuth 2.0 access scope allows an application to upload files to the
                        // authenticated user's YouTube channel, but doesn't allow other types of access.
                        new[] { YouTubeService.Scope.YoutubeUpload },
                        "user",
                        CancellationToken.None
                    );
                }


                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
                });

                var video = new Video();
                video.Snippet = new VideoSnippet();
                video.Snippet.Title = "Default Video Title";
                video.Snippet.Description = "Default Video Description";
                video.Snippet.Tags = new string[] { "tag1", "tag2" };
                video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                video.Status = new VideoStatus();
                video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"
                var filePath = @"REPLACE_ME.mp4"; // Replace with path to actual movie file.

                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                    videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                    videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                    await videosInsertRequest.UploadAsync();
                }
            }

            void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
            {
                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                        break;

                    case UploadStatus.Failed:
                        Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                        break;
                }
            }

            void videosInsertRequest_ResponseReceived(Video video)
            {
                Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
            }
        }
    }

}

