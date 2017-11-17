using MediaClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TVSimulator
{

    class FileImporter
    {
        private const string OMDB_APIKEY = "77f17a4d";
        private const string MOVIE = "movie";
        private const string TVSERIES = "TVseries";


        List<String> allPathes;
        
        public FileImporter()
        {
            allPathes = new List<string>();
        }


        // get files paths from folder List<string>(folder path) 
        public void getAllMediaFromDirectory(String path,bool isIncludeSubfolders)
        {
            String[] extensions = new String[] { "*.mkv", "*.avi", "*.wmv" ,".mp4",".mp3",".flac",".wav"};    // put here all file possible extensions
            String[] fileListArr;
            foreach (String extension in extensions)
            {
                if(isIncludeSubfolders)
                {
                     fileListArr = Directory.GetFiles(path, extension, System.IO.SearchOption.AllDirectories);
                }
                else
                {
                    fileListArr = Directory.GetFiles(path, extension);
                }
    
                foreach (String file in fileListArr)
                    allPathes.Add(file);
            }
            for (int i = 0; i < 5; i++) //TEST : check function sortToType
            {
                sortToTypes(allPathes[i]);
            }
        }

        // check file type(movie/music/tv series)  :: (String filePath) 
        public void sortToTypes(string filePath)
        {
            FileInfo fileInfo = new FileInfo(System.IO.Path.GetFileName(filePath)); // holds fileName and extenstion
            
            // query to check if file is movie type
            var movieExt = new List<string> { ".mkv", ".avi", ".wmv", ".mp4" };  //  need to change from list to array
            bool contains = movieExt.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);
            if (contains) // if the extension is of A video file we go here
            {
                Regex movieRegex = new Regex(@"([\.\w']+?)(\.[0-9]{4}\..*)");
                //..........................................................
                if (movieRegex.IsMatch(fileInfo.Name))
                {
                    videoHandler(fileInfo,MOVIE);
                }
                //..............................................
                Regex TVSeriesRegex = new Regex(@"([\.\w']+?)([sS]([0-9]{2})[eE]([0-9]{2})\..*)");
                if (TVSeriesRegex.IsMatch(fileInfo.Name))
                {
                    videoHandler(fileInfo,TVSERIES);
                }
                //TODO : edge cases if name are not recognized 
                // 1. movie name is number. - match a regex to this scenario and handler

            }
        }
        
        #region sorted media handlers
        // extract name, extends info and call save to db
        private async void videoHandler(FileInfo fileInfo,string type)
        {
            string[] potentialMovieVals = { "201", "200", "199", "198", "197", "196", "195" };
            string[] potentialTvVals = { "S0", "S1", "S2" };
            string videoName ="";
            //...................
            if(type.Equals(MOVIE))
            {
                foreach (string val in potentialMovieVals)
                {
                    videoName = extractVideoName(fileInfo.Name, val); 
                    if ( !(videoName.Equals("")) )
                        break;
                }
            }
            //........................
            if(type.Equals(TVSERIES))
            {
                foreach (string val in potentialTvVals)
                {
                    videoName = extractVideoName(fileInfo.Name, val);
                    if (!(videoName.Equals("")))
                        break;
                }
            }
            //..........................................................
            
            if (videoName.Equals(""))
            {
                // movie or series name not found -- should be impossible if regex accepted

            }
            Video video = await extendVideoInfo(videoName,type);
            if(video.GetType().Equals(TVSERIES))
            {
                string[] data = getSeasonAndEpisode(fileInfo.Name);
                TvSeries tv;
                if (data != null)
                tv = new TvSeries(video, data[0],data[1]);
            }
        }

       

        #endregion sorted media handlers


        #region database functions
        // create file object by type and call save to DB 
        #endregion database functions



        #region Helper Methods
        // generic function to extract video specific name
        private string extractVideoName(string fullName,string compareArg)
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

        private async Task<Video> extendVideoInfo(string videoName,string type)
        {
            OMDbSharp.OMDbClient client = new OMDbSharp.OMDbClient(OMDB_APIKEY, false);
            var x = await client.GetItemByTitle(videoName);
            return new Video(x.Title, type,x.Year, x.Genre, x.Plot, x.Director, x.Runtime,x.imdbRating);
        }

        private string[] getSeasonAndEpisode(string fullName)
        {
            if (fullName.Contains("S0"))
            {
                int x = fullName.IndexOf("S0");
                string season = fullName.Substring(x + 1, x + 3);
                string episode = fullName.Substring(x + 4, x + 6);
                string[] res = { season, episode };
                return res;
            }
            return null;
        }
        #endregion Helper Methods
    }
}
