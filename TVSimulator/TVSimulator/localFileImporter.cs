using MediaClasses;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LiteDB;
using System.Windows;

namespace TVSimulator
{ 
    class localFileImporter : EventArgs
    {
        public delegate void videoLoaded(Object o,List<Media> arg);
        public event videoLoaded OnVideoLoaded;

        List<String> allPathes;
        List<Media> allMedia;

        public localFileImporter()
        {
            allPathes = new List<string>();
            allMedia = new List<Media>();
        }

        public async void LoadLocalFilesFromDirectory(String path, bool isIncludeSubfolders)
        {
            getAllMediaFromDirectory(path, isIncludeSubfolders);
            await  getAllMedia();
            saveMediaToDB();
        }

        // get files paths from folder List<string>(folder path) 
        public void getAllMediaFromDirectory(String path, bool isIncludeSubfolders)
        {
            allPathes.Clear();
            allMedia.Clear();
            String[] extensions = new String[] { "*.mkv", "*.avi", "*.wmv", "*.mp4", "*.mp3", "*.flac", "*.wav" };    // put here all file possible extensions
            String[] fileListArr;
            foreach (String extension in extensions)
            {
                if (isIncludeSubfolders) // include subfolders
                    fileListArr = Directory.GetFiles(path, extension, System.IO.SearchOption.AllDirectories);
                else
                    fileListArr = Directory.GetFiles(path, extension);

                foreach (String file in fileListArr)
                    allPathes.Add(file);
            }
        }

        private async Task<bool> getAllMedia()
        {
            foreach (var item in allPathes)
                await sortToTypes(item);        //await = dont move on until answer from OMDB server - ASYNC
            if (allMedia.Count > 0)
                OnVideoLoaded(this, allMedia);
            return true;
        }

        // check file type(movie/music/tv series)  :: (String filePath) 
        public async Task<bool> sortToTypes(string filePath)
        {
            FileInfo fileInfo = new FileInfo(System.IO.Path.GetFileName(filePath));     // holds fileName and extenstion

            // query to check if file is movie type
            var movieExt = new List<string> { ".mkv", ".avi", ".wmv", ".mp4" };  //  need to change from list to array
            var musicExt = new List<string> { ".mp3", ".flac", ".ogg", ".wav" };
            bool contains = movieExt.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);
            if (contains) // if the extension is of A video file we go here
            {
                Regex movieRegex = new Regex(@"([\.\w']+?)(\.[0-9]{4}\..*)");
                Regex TVSeriesRegex = new Regex(@"([\.\w']+?)([sS]([0-9]{2})[eE]([0-9]{2})\..*)");
                //..........................................................
                if (movieRegex.IsMatch(fileInfo.Name))
                {
                    await videoHandler(fileInfo, Constants.MOVIE, filePath);
                }
                //..............................................
                else if (TVSeriesRegex.IsMatch(fileInfo.Name))
                {
                    await videoHandler(fileInfo, Constants.TVSERIES, filePath);
                }

                //TODO : edge cases if name are not recognized 
                // 1. movie name is number. - match a regex to this scenario and handler
                //else
                //{
                //    musicHandler(filePath);
                //}
                else
                {
                    allMedia.Add(new Media(filePath, fileInfo.Name));
                }
                return true;
            }
            else
            {
                contains = musicExt.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);
                if(contains)
                {
                    musicHandler(filePath);
                }
            }
            return true;
        }

        #region sorted media handlers
        // extract name, extends info and call save to db
        private async Task<bool> videoHandler(FileInfo fileInfo, string type, string filePath)
        {
            string[] potentialMovieVals = { "201", "200", "199", "198", "197", "196", "195" };
            string[] potentialTvVals = { "S0", "S1", "S2" };
            string videoName = "";
            //...................
            if (type.Equals(Constants.MOVIE))
            {
                foreach (string val in potentialMovieVals)
                {
                    videoName = extractVideoName(fileInfo.Name, val);
                    if (!(videoName.Equals("")))
                        break;
                }
            }
            //........................

            if(type.Equals(Constants.TVSERIES))
            {
                foreach (string val in potentialTvVals)
                {
                    videoName = extractVideoName(fileInfo.Name, val);
                    if (!(videoName.Equals("")))
                        break;
                }
            }
            //..........................................................
                
            if (videoName.Equals(""))       //incase media name not found 
            {
                // movie or series name not found -- should be impossible if regex accepted

            }
            try
            {
                Media media = await extendVideoInfo(videoName,filePath,type);
                if (media == null)
                {
                    media = new Media(filePath, videoName);
                    return false;
                }
                if(media is Movie)
                    allMedia.Add((Movie)media);
                if (media is TvSeries)
                    allMedia.Add((TvSeries)media);
                
                return true;
            }
            catch (Exception)
            {
                return false;
                // TODO: implement what to do in case of faliure
            }
        }

        //............................................
        private void musicHandler(string path)
        {
            try
            {
                TagLib.File data = TagLib.File.Create(path);
                var tag = data.Tag;
                Music music = new Music(path, tag.Title, data.Properties.Duration.ToString(), tag.FirstGenre, tag.FirstPerformer, tag.Album, tag.Year.ToString(), tag.Lyrics);
                allMedia.Add((Media)music);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion sorted media handlers


        #region database functions
        // create file object by type and call save to DB 
        public void createDB()
        {
            var db = new LiteDatabase(@"C:\\TVSimulatorDB\MyData.db");
            var media = db.GetCollection<Media>("media");
            Media video = new Media("", "");
            video.Name = "split";
            video.Path = "my path";
            video.Gnere = "horror";
           
            media.Insert(video);
        }

        public void getDbVAls()
        {
            if(!Directory.Exists(Constants.DB_FILE_PATH))
                Directory.CreateDirectory(Constants.DB_FILE_PATH);
            var db = new LiteDatabase(@"C:\\TVSimulatorDB\MyData.db");
            var media = db.GetCollection<Media>("media");
            media.EnsureIndex(x => x.Name);
            var results = media.Find(x => x.Name.StartsWith("s"));
            MessageBox.Show(results.ElementAt(1).ToString());
        }

        private void saveMediaToDB()
        {
            
            var db = new LiteDatabase(@"C:\\TVSimulatorDB\MyData.db");
            var media = db.GetCollection<Media>("media");
            foreach (Media obj in allMedia)
            {
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
        public void readMediaCollectionFromDB()
        {
            var db = new LiteDatabase(@"C:\\TVSimulatorDB\MyData.db");
            var media = db.GetCollection<Media>("media");
            var res = media.FindAll();
            foreach (var item in res)
            {
                item.ToString();
            }
        }
        #endregion database functions



        #region Helper Methods
        // generic function to extract video specific name
        private string extractVideoName(string fullName, string compareArg)
        {
            if (fullName.Contains(compareArg))
            {
                int x = fullName.IndexOf(compareArg);
                string videoName = fullName.Substring(0, x);
                videoName = videoName.Replace(".", " ");
                videoName = videoName.Replace("-", " ");
                //TODO: write movieName.Replace(x, " "); with all potential chars - find LINQ solution
                return videoName;
            }
            return "";
        }

        private async Task<Media> extendVideoInfo(string videoName, string path,string type)
        {
            OMDbSharp.OMDbClient client = new OMDbSharp.OMDbClient(Constants.OMDB_APIKEY, false);
            var x = await client.GetItemByTitle(videoName);     // return object with properties

            if (type.Equals(Constants.MOVIE))
            {
                var movie = new Movie(path, x.Title, x.Runtime, x.Genre, x.Director, x.Plot, x.imdbRating, x.Year);
                return movie;
            }
            else if (type.Equals(Constants.TVSERIES))
            {
                string[] data = getSeasonAndEpisode(path);      //  data[0] = season , data[1] = episode
                var TvSeries = new TvSeries(path, x.Title, x.Runtime, x.Genre, data[0], data[1], x.Plot, x.imdbRating, x.Year);
                return TvSeries;
            }
            else
                return null;
        }

        private string[] getSeasonAndEpisode(string fullName)
        {
            if (fullName.Contains("S0"))
            {
                int x = fullName.IndexOf("S0");
                string season = fullName.Substring(x + 1, x + 3 - (x+1));
                string episode = fullName.Substring(x + 4, x + 6 - (x+4));
                string[] res = { season, episode };
                return res;
            }
            return null;
        }
        #endregion Helper Methods
    }

   
}
