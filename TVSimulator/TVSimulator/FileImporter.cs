#define testing
using HelperClasses;
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
    class FileImporter : EventArgs, IFileImporter
    {
        #region fields
        public delegate void videoLoaded(Object o, List<Media> arg);
        public event videoLoaded OnVideoLoaded;

        List<String> allDirectories;
        List<String> allPathes;
        List<Media> allMedia;
        List<Movie> allMovies;
        List<TvSeries> allTVseries;
        List<Music> allMusic;
        Database db;
        MyTaskProgressReport reporter;
        private bool listsSaved = false;

        private string[] videoExt = { ".mkv", ".avi", ".wmv", ".mp4", ".mpeg", ".mpg", ".3gp" };  //  need to change from list to array
        private string[] musicExt = { ".mp3", ".flac", ".ogg", ".wav", ".wma" };
        private string[] mediaExt = { ".mkv", ".avi", ".wmv", ".mp4", ".mpeg", ".mpg", ".3gp", ".mp3", ".flac", ".ogg", ".wav", ".wma" };

        public List<Media> AllMedia { get => allMedia; set => allMedia = value; }

        #endregion

        #region Constructor
        public FileImporter()
        {
            allDirectories = new List<string>();
            allPathes = new List<string>();
            allMedia = new List<Media>();
            allMovies = new List<Movie>();
            allTVseries = new List<TvSeries>();
            allMusic = new List<Music>();
            reporter = new MyTaskProgressReport();
            db = new Database();
        }
        #endregion

        #region Get pathes and sort
        public async Task<bool> addDirectoryList(List<costumPath> dirs)
        {
            foreach (costumPath val in dirs)
            {
                await getAllMediaFromDirectory(val.Path, val.IsIncludeSubFolders);
            }

            saveListsToDB();
            return true;
        }
        public async Task<int> getAllMediaFromDirectory(string path, bool isIncludeSubfolders)  // get files paths from folder List<string>(folder path) 
        {
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
            if (allPathes.Count() == 0)
                return -1;

            // all pathes added - update total progress ammount here
            reporter.TotalProgressAmount = allPathes.Count();

            // get all media files from pathes
            await getAllMedia();
            saveListsToDB();
            return 0;
        }

        private async Task<bool> SortMediaToTypes(string filePath)        // check file type(movie/music/tv series)  :: (String filePath) 
        {
            FileInfo fileInfo = new FileInfo(System.IO.Path.GetFileName(filePath));     // holds fileName and extenstion
            bool contains = videoExt.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);   // query to check if file is movie type

            if (contains) // if the extension is of A video file we go here
            {
                Regex movieRegex = new Regex(@"([\.\w']+?)(\.[0-9]{4}\..*)");
                Regex TVSeriesRegex = new Regex(@"([\. \w']+?)([sS]([0-9]{2})[eE]([0-9]{2})( \..)*)");

                if (movieRegex.IsMatch(fileInfo.Name))
                    await videoHandler(fileInfo.Name, Constants.MOVIE, filePath);
                else if (TVSeriesRegex.IsMatch(fileInfo.Name) || fileInfo.Name.ToLower().Contains("season"))
                {
                    if (TVSeriesRegex.IsMatch(fileInfo.Name))
                        await videoHandler(fileInfo.Name, Constants.TVSERIES, filePath);
                    else
                    {
                        string temp = adjustTVString(fileInfo.Name);
                        await videoHandler(temp, Constants.TVSERIES, filePath);
                    }
                }
                else
                {
                    //var dur = getfilePath);
                    var dur = getDuration(filePath).ToString();
                    allMedia.Add(new Media(filePath, fileInfo.Name, dur));
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

        public async Task<bool> videoHandler(string name, string type, string filePath)         // extract name, extends info and call save to db
        {
            string[] potentialMovieVals = { "201", "200", "199", "198", "197", "196", "195" };
            string[] potentialTvVals = { "S0", "S1", "S2" };
            string videoName = "";

            if (type.Equals(Constants.MOVIE))
            {
                foreach (string val in potentialMovieVals)
                {
                    videoName = extractVideoName(name, val);
                    if (!(videoName.Equals("")))
                        break;
                }
            }

            if (type.Equals(Constants.TVSERIES))
            {
                foreach (string val in potentialTvVals)
                {
                    videoName = extractVideoName(name, val);
                    if (!(videoName.Equals("")))
                        break;
                }
            }

            try
            {
                Media media = await extendVideoInfo(name, videoName, filePath, type);
                if (media == null)
                {
                    media = new Media(filePath, videoName);
                    return false;
                }
                if (media is Movie)
                    allMedia.Add((Movie)media);
                if (media is TvSeries)
                    allMedia.Add((TvSeries)media);

                return true;
            }
            catch (Exception) { return false; }
        }

        public void musicHandler(string path, string fileName)
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

                var dur = getDuration(path).ToString();

                Music music = new Music(path, songName, dur, tag.FirstGenre, tag.FirstPerformer, tag.Album, tag.Year.ToString(), tag.Lyrics);
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
                videoName = videoName.Replace(".", " ").Replace("-", " ").Replace("_", " ");
                return videoName;
            }
            return "";
        }

        public async Task<Media> extendVideoInfo(string originalName, string videoName, string path, string type)
        {
            OMDbSharp.OMDbClient client = new OMDbSharp.OMDbClient(Constants.OMDB_APIKEY, false);
            var x = await client.GetItemByTitle(videoName);     // return object with properties
            var id = x.imdbID;
            string duration = getDuration(path).ToString();

            if (type.Equals(Constants.MOVIE))
            {
                Movie movie;
                if (id == null)
                    movie = new Movie(path, videoName, duration);
                else
                    movie = new Movie(path, x.Title, duration, x.Genre, x.Director, x.Plot, x.imdbRating, x.Year);
                allMovies.Add(movie);
                return movie;
            }
            else if (type.Equals(Constants.TVSERIES))
            {
                string[] data = getSeasonAndEpisode(originalName);      //  data[0] = season , data[1] = episode
                TvSeries TvSeries;

                try
                {
                    if (id == null)
                        TvSeries = new TvSeries(path, videoName, duration, "general", data[0], data[1]);
                    else
                    {
                        var y = await client.GetSeriesEpisode(id, int.Parse(data[0]), int.Parse(data[1]));
                        if (!y.Plot.Equals("N/A"))
                            TvSeries = new TvSeries(path, x.Title, duration, x.Genre, data[0], data[1], y.Plot, x.imdbRating, x.Year);
                        else
                            TvSeries = new TvSeries(path, x.Title, duration, x.Genre, data[0], data[1], x.Plot, x.imdbRating, x.Year);
                    }
                }
                catch (Exception)
                {
                    TvSeries = new TvSeries(path, x.Title, duration, x.Genre, data[0], data[1], x.Plot, x.imdbRating, x.Year);
                }
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
            {
                await SortMediaToTypes(item);        //await = dont move on until answer from OMDB server - ASYNC
                reporter.CurrentProgressAmount++;
                reporter.CurrentProgressMessage = "loading " + reporter.CurrentProgressAmount.ToString()+ " out of "+reporter.TotalProgressAmount.ToString() +" media files";
            }


            // if (allMedia.Count > 0)                   // send event to UI
            // OnVideoLoaded(this, allMedia);

            return true;
        }

        private string[] getSeasonAndEpisode(string name)
        {
            if (name.Contains("S0") || name.Contains("S1"))    //need to extend this 
            {
                int x;
                if (name.Contains("S0"))
                    x = name.IndexOf("S0") + 1;
                else
                    x = name.IndexOf("S1") + 1;

                int y = name.IndexOf("E") + 1;
                string season = name.Substring(x, 2);
                string episode = name.Substring(y, 2);
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
            listsSaved = true;
        }

        private string adjustTVString(string origin)
        {
            string temp = origin.Replace("Season ", "S");
            temp = temp.Replace(" Episode ", "E");
            temp = temp.Replace("Episode ", "E");
            temp = temp.Replace("season ", "S");
            temp = temp.Replace("Episode", "E");
            temp = temp.Replace("season", "S");
            temp = temp.Replace("episode", "E");
            temp = temp.Replace("Season", "S");
            temp = temp.Replace("episode", "E");
            return temp;
        }

        #endregion Helper Methods

        public async Task<bool> syncAllAsyncReportProgress(int sleepTime, IProgress<MyTaskProgressReport> progress, List<costumPath> dirs)
        {
            var t = new Task(() => { var res = addDirectoryList(dirs); });
            t.Start();
            
            while(!listsSaved)
            {
                await Task.Delay(sleepTime);

                progress.Report(new MyTaskProgressReport { CurrentProgressAmount = reporter.CurrentProgressAmount, TotalProgressAmount = reporter.TotalProgressAmount, CurrentProgressMessage = reporter.CurrentProgressMessage });

            }
            return listsSaved;
        }


    }

    
}



