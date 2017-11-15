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

        public object Hasseware { get; private set; }

        // get files paths from folder List<string>(folder path) 
        public void getAllMediaFromDirectory(String path)
        {
            String[] extensions = new String[] { "*.mkv", "*.avi", "*.wmv" ,".mp4",".mp3",".flac",".wav"};    // put here all file possible extensions
            String[] fileListArr;
            foreach (String extension in extensions)
            {
                fileListArr = Directory.GetFiles(path, extension);

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
                // movie regex : ([\.\w']+?)(\.[0-9]{4}\..*)
                Regex movieRegex = new Regex(@"([\.\w']+?)(\.[0-9]{4}\..*)");
                //..........................................................

                //TODO : edge case if name are not recognized 
                if (movieRegex.IsMatch(fileInfo.Name))
                {
                    if (fileInfo.Name.Contains("201") )
                    {
                        int x = fileInfo.Name.IndexOf("201");
                        string movieName = fileInfo.Name.Substring(0, x);
                        movieName = movieName.Replace(".", " ");
                        movieName = movieName.Replace("-", " ");
                        //TODO: write movieName.Replace(x, " "); with all potential chars - find LINQ solution
                        MessageBox.Show("its a movie!\n" + movieName);
                        extentMovieInfo(movieName);
                    }
                }
                //..............................................
                Regex TVSeriesRegex = new Regex(@"([\.\w']+?)([sS]([0-9]{2})[eE]([0-9]{2})\..*)");
                if (TVSeriesRegex.IsMatch(fileInfo.Name))
                {
                    if (fileInfo.Name.Contains("S0"))
                    {
                        int x = fileInfo.Name.IndexOf("S0");
                        string TvSeriesName = fileInfo.Name.Substring(0, x);
                        TvSeriesName = TvSeriesName.Replace(".", " ");
                        TvSeriesName = TvSeriesName.Replace("-", " ");
                        MessageBox.Show("its a TV Series!\n" + TvSeriesName);
                    }
                }

            }
        }

        private async void extentMovieInfo(string movieName)
        {
            //string apiKEY = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIzMGM1NWU0ODgzMDkwMTg4NDYyMTJlYTRlMzk2MjNjZiIsInN1YiI6IjVhMGI5ZDhhOTI1MTQxNGRmMzAwMWVmZSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.EDkzdAnMmvCEhx4RhrkSRdBzi3hXmNrtSv8YIqwIU8w";
            //TMDbLib.Client.TMDbClient client = new TMDbLib.Client.TMDbClient(apiKEY);
            //var x = await client.SearchKeywordAsync(movieName);
            //TMDbLib.Objects.Authentication.


        }
        // create file object by type and call save to DB 

        // 

        #region Helper Methods

        #endregion Helper Methods
    }
}
