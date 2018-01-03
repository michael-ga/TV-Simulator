using LiteDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace MediaClasses
{
    // very useful link
    // https://github.com/mbdavid/LiteDB/wiki/Collections
    public class Database
    {

        #region Fields And Constructor
        LiteDatabase db;
        LiteCollection<Media> mediaCollection;
        LiteCollection<TvSeries> TVCollection;
        LiteCollection<Movie> movieCollection;
        LiteCollection<Music> musicCollection;
        LiteCollection<YouTubeChannel> youtube_channelCollection;



        public Database()
        {
            if (!Directory.Exists(Constants.DB_FILE_PATH))
                Directory.CreateDirectory(Constants.DB_FILE_PATH);
            db = new LiteDatabase(@"C:\TVSimulatorDB\MyData.db");// get connction string from app.config
            mediaCollection = db.GetCollection<Media>(Constants.MEDIA_COLLECTION);
            TVCollection = db.GetCollection<TvSeries>(Constants.TV_SERIES_COLLECTION);
            movieCollection = db.GetCollection<Movie>(Constants.MOVIE_COLLECTION);
            musicCollection = db.GetCollection<Music>(Constants.MUSIC_COLLECTION);
            youtube_channelCollection = db.GetCollection<YouTubeChannel>(Constants.YOUTUBE_CHANNEL_COLLECTION);
        }
        #endregion
        //TODO: CHECK QUERIES 
        public void removeCollections()
        {
            mediaCollection.Delete(Query.All(Query.Descending));
            movieCollection.Delete(Query.All(Query.Descending));
            TVCollection.Delete(Query.All(Query.Descending)); 
            musicCollection.Delete(Query.All(Query.Descending));
        }

        
        #region Collection queries
        // remove whole collection
        public void removeMediaCollection()
        {
            mediaCollection.Delete(Query.All(Query.Descending));
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
                //var media = db.GetCollection<Media>(Constants.MEDIA_COLLECTION);
                Media temp = new Media(obj.Path, obj.Name, obj.Duration, obj.Gnere);
                mediaCollection.Insert(temp);
            }
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
                    mediaCollection.Insert((Media)obj);
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
        #endregion


        #region youtube
        public List<YouTubeChannel> getYoutubeChannelList()
        {
            return youtube_channelCollection.FindAll().ToList();
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

       


        #endregion
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
