using LiteDB;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;


namespace HelperClasses
{
    // very useful link
    // https://github.com/mbdavid/LiteDB/wiki/Collections
    public class Database
    {
        public delegate void channelAdded(Object o);

        public event channelAdded OnchannelAdded;

        #region Fields And Constructor
        LiteDatabase db;
        LiteCollection<Media> mediaCollection;
        LiteCollection<TvSeries> TVCollection;
        LiteCollection<Movie> movieCollection;
        LiteCollection<Music> musicCollection;
        LiteCollection<YouTubeChannel> youtube_channelCollection;
        LiteCollection<YoutubePlaylistChannel> youtube_Playlist_channelCollection;

        LiteCollection<YoutubeVideo> youtube_videoCollection;
        LiteCollection<Channel> channelCollection;
        LiteCollection<BroadcastTime> timeCollection;


        public LiteCollection<Media> MediaCollection { get => mediaCollection; set => mediaCollection = value; }

        public Database()
        {
            if (!Directory.Exists(Constants.DB_DIR_PATH))
                Directory.CreateDirectory(Constants.DB_DIR_PATH);
            db = new LiteDatabase(@"C:\TVSimulatorDB\MyData.db");// get connction string from app.config
            MediaCollection = db.GetCollection<Media>(Constants.MEDIA_COLLECTION);
            TVCollection = db.GetCollection<TvSeries>(Constants.TV_SERIES_COLLECTION);
            movieCollection = db.GetCollection<Movie>(Constants.MOVIE_COLLECTION);
            musicCollection = db.GetCollection<Music>(Constants.MUSIC_COLLECTION);
            youtube_videoCollection = db.GetCollection<YoutubeVideo>(Constants.YOUTUBE_VIDEO_COLLECTION);
            youtube_channelCollection = db.GetCollection<YouTubeChannel>(Constants.YOUTUBE_CHANNEL_COLLECTION);
            youtube_Playlist_channelCollection = db.GetCollection<YoutubePlaylistChannel>(Constants.YOUTUBE_PLAYLIST_CHANNEL_COLLECTION);
            channelCollection = db.GetCollection<Channel>(Constants.CHANNEL_COLLECTION);
            timeCollection = db.GetCollection<BroadcastTime>(Constants.TIME_COLLECTION);

        }
        #endregion
        //TODO: CHECK QUERIES 
        public void removeCollections()
        {
            MediaCollection.Delete(Query.All(Query.Descending));
            movieCollection.Delete(Query.All(Query.Descending));
            TVCollection.Delete(Query.All(Query.Descending)); 
            musicCollection.Delete(Query.All(Query.Descending));
        }

        public void removeChannelCollection()
        {
            channelCollection.Delete(Query.All(Query.Descending));
        }


            #region Collection queries
            // remove whole collection
            public void removeMediaCollection()
        {
            MediaCollection.Delete(Query.All(Query.Descending));
        }

        public void removeCollectionByName(string collectionName)
        {
            switch (collectionName)
            {
                case Constants.YOUTUBE_CHANNEL_COLLECTION:
                    youtube_channelCollection.Delete(Query.All());
                    break;

                default:
                    break;

            }
        }

        // insert to Media objects to collection
        public void insertMediaList(List<Media> mediaList)
        {
            if (mediaList.Count <= 0)
                return;
            foreach (Media obj in mediaList)
            {
                Media temp = new Media(obj.Path, obj.Name, obj.Duration, obj.Gnere);
                MediaCollection.Insert(temp);
            }
        }

        public void insertChannel(Channel c)
        {
            channelCollection.Insert(c);
        }

        public void insertBroadcastTime(BroadcastTime bt)
        {
            timeCollection.Insert(bt);
        }

        // insert to 3 collections by type
        public void insertByType(List<Media> mediaList)
        {
            foreach (Media obj in mediaList)
            {
                if (obj is Movie)
                    movieCollection.Insert(((Movie)obj));
                else if (obj is TvSeries)
                    TVCollection.Insert((TvSeries)obj);
                else if (obj is Music)
                    musicCollection.Insert((Music)obj);
                else
                    MediaCollection.Insert((Media)obj);
            }
        }

        public List<Movie> getMovieList()
        {
            var movie = db.GetCollection<Movie>(Constants.MOVIE_COLLECTION);
            return movie.FindAll().ToList();
        }

        public List<TvSeries> getTVList()
        {
            var tv = db.GetCollection<TvSeries>(Constants.TV_SERIES_COLLECTION);
            return tv.FindAll().ToList();
        }

        public List<Music> getMusicList()
        {
            var music = db.GetCollection<Music>(Constants.MUSIC_COLLECTION);
            return music.FindAll().ToList();
        }

        public List<Media> getAllMediaList()
        {
            var media = db.GetCollection<Media>(Constants.MEDIA_COLLECTION);
            return media.FindAll().ToList();
        }

        public List<Channel> getChannelList()
        {
            var channel = db.GetCollection<Channel>(Constants.CHANNEL_COLLECTION);
            return channel.FindAll().ToList();
        }

        public BroadcastTime getTimes()
        {
            var bt = db.GetCollection<BroadcastTime>(Constants.TIME_COLLECTION);
            return bt.FindById(1);
        }
        #endregion

        #region youtube
        public List<YouTubeChannel> getYoutubeChannelList()
        {
            return youtube_channelCollection.FindAll().ToList();
        }

        public List<YoutubePlaylistChannel> getYoutubePlaylistChannelList()
        {
            return youtube_Playlist_channelCollection.FindAll().ToList();
        }
        // this function inserts if channel not exist
        public bool insertYoutubechannel(YouTubeChannel channel)
        {
            var exist = checkIfIDExsis(channel.Path, Constants.YOUTUBE_CHANNEL_COLLECTION, "Path");
            if (!exist)
            {
                youtube_channelCollection.Insert(channel);
                return true;
            }
            return false;
        }
        // for debug only
        public bool insertYoutubeVideoList(List<YoutubeVideo> videos)
        {
            foreach (YoutubeVideo item in videos)
            {
                //var exist = checkIfIDExsis(item.Id.ToString(), Constants.YOUTUBE_VIDEO_COLLECTION, "Id");
                //if (!exist)
                //{
                youtube_videoCollection.Insert(item);
                //    }
                //    return false;
                //}
                //return true;
            }
            return true;
        }

        public bool updateYoutubeChannel(YouTubeChannel ytbChannel)
        {
            return youtube_channelCollection.Update(ytbChannel);
        }

        public bool updateYoutubePlaylistChannel(YoutubePlaylistChannel ytbChannel)
        {
            return youtube_Playlist_channelCollection.Update(ytbChannel);
        }

        public bool insertPlaylistChannel(YoutubePlaylistChannel channel)
        {
            var exist = checkIfIDExsis(channel.Path, Constants.YOUTUBE_PLAYLIST_CHANNEL_COLLECTION, "Path");
            if (!exist)
            {
                youtube_Playlist_channelCollection.Insert(channel);
                return true;
            }
            return false;
        }

        
        // for debug only
        public List<YoutubeVideo> getYotubeVIdeos()
        {
            return youtube_videoCollection.FindAll().ToList();
        }


        #endregion




        public List<Media> getYoutubePlaylistVideosFromPlaylist(string channelID)
        {
            List<Media> listToReturn = new List<Media>();
            var playlistChannel = youtube_Playlist_channelCollection.Find(Query.EQ("Path", channelID)).First(); // find the playlist channel by the channel ID
            var playlist = playlistChannel.Playlist_list;
            foreach (var item in playlist)
            {
                if (item.Videos != null)
                    listToReturn.AddRange(item.Videos);
            }
            return listToReturn;
        }

    public YoutubePlaylistChannel getPlayListChannelByChannelID(string id)
        {
            return youtube_Playlist_channelCollection.Find(Query.EQ("Path",id)).First();
        }

        public YoutubePlaylist getPlayListByPlaylistID(string PlaylistChannelID,string playlistID)
        {
            var temp = getPlayListChannelByChannelID(PlaylistChannelID);
            var playlists = temp.Playlist_list;
            foreach (var item in playlists)
            {
                if (item.Path.Equals(playlistID))
                    return item;
            }
            return null;
        }

        public List<YoutubeVideo> getYoutubeVideosfromChannel(string chnlID)
        {
            var channel = youtube_channelCollection.Find(Query.EQ("Path", chnlID)).First();
            return channel.VideoList;
        }

    public List<YoutubePlaylistChannel> getPlaylistChannels()
        {
            return youtube_Playlist_channelCollection.FindAll().ToList();
        }

        public int getLastChannelIndex()
        {
            return channelCollection.Count();
        }


        #region Helper Functions
        // helper currenty adjusted to youtube channel only
        private bool checkIfIDExsis(string uniqueField, string collectionName, string colName)
        {
            bool res = false;
            if (collectionName.Equals(Constants.YOUTUBE_CHANNEL_COLLECTION))
            {
                if (youtube_channelCollection.Exists(Query.EQ(colName, uniqueField)))
                    res = true;
            }
            if (collectionName.Equals(Constants.YOUTUBE_PLAYLIST_CHANNEL_COLLECTION))
            {
                if (youtube_Playlist_channelCollection.Exists(Query.EQ(colName, uniqueField)))
                    res = true;
            }
            return res;
        } 

        // this function is general and it used to remove one element by it's unique field from some collection.
        public bool removeElementByIDFromCollection(string collectionName, string compareArg)
        {
            switch (collectionName)
            {
                case Constants.YOUTUBE_CHANNEL_COLLECTION:
                    var exist = youtube_channelCollection.Exists(Query.EQ("Path", compareArg));
                    if (exist)
                    {
                        youtube_channelCollection.Delete(Query.EQ("Path", compareArg));
                        return true;
                    }
                    return false;
                //......continue for each media type
                //
                //
                //....
                case Constants.YOUTUBE_PLAYLIST_CHANNEL_COLLECTION:
                    var isExist = youtube_Playlist_channelCollection.Exists(Query.EQ("Path", compareArg));
                    if (isExist)
                    {
                        youtube_Playlist_channelCollection.Delete(Query.EQ("Path", compareArg));
                        return true;
                    }
                    return false;
                default:
                    break;
            }
            return false;

        }
        #endregion

        #region single file queries
        // return media object of a given name
        public Media getMediaFileByName(string name)
        {
            var media = db.GetCollection<Media>(Constants.MEDIA_COLLECTION);
            media.EnsureIndex(x => x.Name);
            return media.Find(x => x.Name.StartsWith(name)).First();
        } 

        public void removeFileByName(string name)
        {
            var collection = db.GetCollection<Media>(Constants.MEDIA_COLLECTION);
            collection.EnsureIndex(x => x.Name.ToLower());
            collection.Delete(x => x.Name.StartsWith(name.ToLower()));
        }
        #endregion
        
    }
}
