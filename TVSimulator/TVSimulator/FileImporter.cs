#define testing
using MediaClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WMPLib;


namespace TVSimulator
{
    class FileImporter : EventArgs,IFileImporter
    {
        #region fields
        public delegate void videoLoaded(Object o, List<Media> arg);
        public event videoLoaded OnVideoLoaded;

        List<String> allPathes;
        List<Media> allMedia;
        List<Movie> allMovies;
        List<TvSeries> allTVseries;
        List<Music> allMusic;
        Database db;

        private string [] videoExt =  { ".mkv", ".avi", ".wmv", ".mp4", ".mpeg", ".mpg", ".3gp" };  //  need to change from list to array
        private string [] musicExt =  { ".mp3", ".flac", ".ogg", ".wav", ".wma" };
        private string [] mediaExt = { ".mkv", ".avi", ".wmv", ".mp4", ".mpeg", ".mpg", ".3gp", ".mp3", ".flac", ".ogg", ".wav", ".wma" };

        public List<Media> AllMedia { get => allMedia; set => allMedia = value; }
        
        #endregion

        #region Constructor
        public FileImporter()
        {
            allPathes = new List<string>();
            allMedia = new List<Media>();
            allMovies = new List<Movie>();
            allTVseries = new List<TvSeries>();
            allMusic = new List<Music>();
            db = new Database();
        }
        #endregion

        #region Get pathes and sort

        public async void getAllMediaFromDirectory(string path, bool isIncludeSubfolders)  // get files paths from folder List<string>(folder path) 
        {
            allPathes.Clear();
            allMedia.Clear();
            string[] mediaExtStarrd = { "*.mkv", "*.avi", "*.wmv", "*.mp4", "*.mpeg", "*.mpg", "*.3gp", "*.mp3", "*.flac", "*.ogg", "*.wav", "*.wma" };

        String[] fileListArr;
            foreach (String extension in mediaExtStarrd)
            {
                if (isIncludeSubfolders) // include subfolders
                    fileListArr = Directory.GetFiles(path, extension, System.IO.SearchOption.AllDirectories);
                else
                    fileListArr = Directory.GetFiles(path, extension);

                foreach (String file in fileListArr)
                    allPathes.Add(file);
            }
            await getAllMedia();
            saveListsToDB();
        }

        private async Task<bool> SortMediaToTypes(string filePath)        // check file type(movie/music/tv series)  :: (String filePath) 
        {
            FileInfo fileInfo = new FileInfo(System.IO.Path.GetFileName(filePath));     // holds fileName and extenstion
            bool contains = videoExt.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);   // query to check if file is movie type

            if (contains) // if the extension is of A video file we go here
            {
                Regex movieRegex = new Regex(@"([\.\w']+?)(\.[0-9]{4}\..*)");
                Regex TVSeriesRegex = new Regex(@"([\.\w']+?)([sS]([0-9]{2})[eE]([0-9]{2})\..*)");

                if (movieRegex.IsMatch(fileInfo.Name))
                    await videoHandler(fileInfo, Constants.MOVIE, filePath);
                else if (TVSeriesRegex.IsMatch(fileInfo.Name))
                {
                    await videoHandler(fileInfo, Constants.TVSERIES, filePath);
                }
                else
                {
                    if(fileInfo.Name.ToLower().Contains("season"))
                    {
                        fileInfo.Name.Replace("Season", "S"); fileInfo.Name.Replace("Episode", "E");
                        fileInfo.Name.Replace("season", "S"); fileInfo.Name.Replace("episode", "E");
                        fileInfo.Name.Replace("Season","S"); fileInfo.Name.Replace("episode", "E");
                        await videoHandler(fileInfo, Constants.TVSERIES, filePath);
                    }
                    else
                        allMedia.Add(new Media(filePath, fileInfo.Name));
                }
                return true;
            }
            else
            {
                contains = musicExt.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);
                if (contains)
                    musicHandler(filePath, fileInfo.Name);
            }
            return true;
        }
        
        #endregion

        #region media handlers

        public async Task<bool> videoHandler(FileInfo fileInfo, string type, string filePath)         // extract name, extends info and call save to db
        {
            string[] potentialMovieVals = { "201", "200", "199", "198", "197", "196", "195" };
            string[] potentialTvVals = { "S0", "S1", "S2" };
            string videoName = "";

            if (type.Equals(Constants.MOVIE))
            {
                foreach (string val in potentialMovieVals)
                {
                    videoName = extractVideoName(fileInfo.Name, val);
                    if (!(videoName.Equals("")))
                        break;
                }
            }

            if(type.Equals(Constants.TVSERIES))
            {
                foreach (string val in potentialTvVals)
                {
                    videoName = extractVideoName(fileInfo.Name, val);
                    if (!(videoName.Equals("")))
                        break;
                }
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
            catch (Exception) { return false; }
        }

        public void musicHandler(string path,string fileName)
        {
            try
            {
                TagLib.File data = TagLib.File.Create(path);
                var tag = data.Tag;
                var songName = "";
                if (tag.Title == null)
                    songName = fileName;
                else
                    songName = tag.Title;
                
                Music music = new Music(path, songName, data.Properties.Duration.ToString(), tag.FirstGenre, tag.FirstPerformer, tag.Album, tag.Year.ToString(), tag.Lyrics);
                allMedia.Add((Media)music);
                allMusic.Add(music);
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public string extractVideoName(string fullName, string compareArg)
        {
            if (fullName.Contains(compareArg))
            {
                int x = fullName.IndexOf(compareArg);
                string videoName = fullName.Substring(0, x);
                videoName = videoName.Replace(".", " ").Replace("-"," ").Replace("_"," ");
                return videoName;
            }
            return "";
        }

        public async Task<Media> extendVideoInfo(string videoName, string path, string type)
        {
            OMDbSharp.OMDbClient client = new OMDbSharp.OMDbClient(Constants.OMDB_APIKEY, false);
            var x = await client.GetItemByTitle(videoName);     // return object with properties

            if (type.Equals(Constants.MOVIE))
            {
                string duration = getDuration(path).Minutes.ToString();duration += ".";
                var movie = new Movie(path, x.Title, duration, x.Genre, x.Director, x.Plot, x.imdbRating, x.Year);
                allMovies.Add(movie);
                return movie;
            }
            else if (type.Equals(Constants.TVSERIES))
            {
                string duration = getDuration(path).Minutes.ToString() + ".";
                string[] data = getSeasonAndEpisode(path);      //  data[0] = season , data[1] = episode
                var TvSeries = new TvSeries(path, x.Title, duration , x.Genre, data[0], data[1], x.Plot, x.imdbRating, x.Year);
                allTVseries.Add(TvSeries);
                return TvSeries;
            }
            else
                return null;
        }

        #endregion media handlers

        #region Helper Methods
        // generic function to extract video specific name
        private async Task<bool> getAllMedia()
        {
            foreach (var item in allPathes)
                await SortMediaToTypes(item);        //await = dont move on until answer from OMDB server - ASYNC
            if (allMedia.Count > 0)                   // commented in UI
                OnVideoLoaded(this, allMedia);
            return true;
        }

        private string[] getSeasonAndEpisode(string fullName)
        {
            if (fullName.Contains("S0"))    //need to extend this 
            {
                int x = fullName.IndexOf("S0");
                string season = fullName.Substring(x + 1, x + 3 - (x+1));
                string episode = fullName.Substring(x + 4, x + 6 - (x+4));
                string[] res = { season, episode };
                return res;
            }
            return null;
        }

        public TimeSpan getDuration(string path)
        {
            TagLib.File data = TagLib.File.Create(path);
            return data.Properties.Duration;
        }

        public void saveListsToDB()
        {
            db.removeCollections();
            db.insertByType(allMedia);
        }
        
        #endregion Helper Methods
    }

   
}