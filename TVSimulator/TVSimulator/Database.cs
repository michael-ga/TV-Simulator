using LiteDB;
using MediaClasses;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace TVSimulator
{
    // very useful link
    // https://github.com/mbdavid/LiteDB/wiki/Collections
    class Database
    {

        #region Fields And Constructor
        LiteDatabase db;
        public Database()
        {
            if (!Directory.Exists(Constants.DB_FILE_PATH))
                Directory.CreateDirectory(Constants.DB_FILE_PATH);
            db = new LiteDatabase(ConfigurationManager.ConnectionStrings["LiteDB"].ConnectionString);// get connction string from app.config
        }
        #endregion
        //TODO: CHECK QUERIES 

        #region Collection queries
        // remove whole collection
        public void removeMediaCollection(string collName)
        {
            var media = db.GetCollection<Media>(collName);
            media.Delete(Query.All(Query.Descending));
        }
        // insert to collection
        public void addMediaList(List<Media> mediaList)
        {
            if (mediaList.Count <= 0)
                return;
            foreach (Media obj in mediaList)
            {
                var media = db.GetCollection<Media>(Constants.ALL_MEDIA_COLLECTION);
                Media temp = new Media(obj.Path, obj.Name, obj.Duration, obj.Gnere);
                media.Insert(temp);
                //if (obj is Movie)
                //    media.Insert(((Movie)obj));
                //else if (obj is TvSeries)
                //    media.Insert((TvSeries)obj);
                //else if (obj is Music)
                //    media.Insert((Music)obj);
                //else
                //media.Insert((Media)obj);
            }
        }
        public List<Media> getAllMediaList()
        {
            var media = db.GetCollection<Media>(Constants.ALL_MEDIA_COLLECTION);
            return (List<Media>)media.FindAll();
        }
        #endregion


        #region single file queries
        // return media object of a given name
        public Media getMediaFileByName(string name)
        {
            var media = db.GetCollection<Media>(Constants.ALL_MEDIA_COLLECTION);
            media.EnsureIndex(x => x.Name);
            return media.Find(x => x.Name.StartsWith(name)).First();
        } 
        public void removeFileByName(string name)
        {
            var collection = db.GetCollection<Media>(Constants.ALL_MEDIA_COLLECTION);
            collection.EnsureIndex(x => x.Name);
            collection.Delete(x => x.Name.StartsWith(name));
        }
        #endregion
        
    }
}
