using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using TMDbLib;
namespace TVSimulator
{
    class FileImporter
    {
        private const string OMDB_API_KEY = "77f17a4d";
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
                    movieHandler(fileInfo);
                }
                //..............................................
                Regex TVSeriesRegex = new Regex(@"([\.\w']+?)([sS]([0-9]{2})[eE]([0-9]{2})\..*)");
                if (TVSeriesRegex.IsMatch(fileInfo.Name))
                {
                    tvSeriesHandler(fileInfo);
                }
                //TODO : edge cases if name are not recognized 
                // 1. movie name is number. - match a regex to this scenario and handler

            }
        }

        private async void extentMovieInfo(string movieName)
        {
            OMDbSharp.OMDbClient client = new OMDbSharp.OMDbClient(OMDB_API_KEY,false);
            var x = await client.GetItemByTitle(movieName);
        }
        // create file object by type and call save to DB 

        // 

        #region Helper Methods
        // extract name, extends info and call save to db
        private void movieHandler(FileInfo fileInfo)
        {
            string[] potentialYearVals = { "201", "200", "199", "198", "197", "196", "195" };
            string movieName="";
            foreach (string val in potentialYearVals)
            {
                movieName = extractVideoName(fileInfo.Name, val, "Movie"); 
                if ( !(movieName.Equals("")) )
                    break;
            }
            if(movieName.Equals(""))
            {
                // movie name not found -- should be impossible if regex accepted

            }
            extentMovieInfo(movieName);
        }

        // extract name, extends info and call save to db
        private void tvSeriesHandler(FileInfo fileInfo)
        {
            string[] potentialVals = { "S0", "S1", "S2" };
            string seriesName = "";
             foreach (string val in potentialVals)
            {
                seriesName = extractVideoName(fileInfo.Name, val, "Movie");
                if (!(seriesName.Equals("")))
                    break;
            }
            if (seriesName.Equals(""))
            {
                // series name not found -- should be impossible if regex accepted

            }
            extentMovieInfo(seriesName);// need to check if contains data
        }

        // generic function to extract video specific name
        private string extractVideoName(string fullName,string compareArg,string type)
        {
            if (fullName.Contains(compareArg))
            {
                int x = fullName.IndexOf(compareArg);
                string movieName = fullName.Substring(0, x);
                movieName = movieName.Replace(".", " ");
                movieName = movieName.Replace("-", " ");
                //TODO: write movieName.Replace(x, " "); with all potential chars - find LINQ solution
                extentMovieInfo(movieName);
                return movieName;
            }
            return "";

        }
        #endregion Helper Methods
    }
}
