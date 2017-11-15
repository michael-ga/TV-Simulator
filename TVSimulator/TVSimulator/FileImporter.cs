using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace TVSimulator
{
    class FileImporter
    {

        List<String> allPathes;
        
        public FileImporter()
        {
            allPathes = new List<string>();
        }

        // get files paths from folder List<string>(folder path) 
        public void getAllMediaFromDirectory(String path)
        {
            String[] extensions = new String[] { "*.mkv", "*.avi", "*.wmv" ,".mp4",".mp3",".flac",".wav"};    // put here all file possible extensions

            foreach (String extension in extensions)
            {
                String[] fileListArr = Directory.GetFiles(path, extension);

                foreach (String file in fileListArr)
                    allPathes.Add(file);
            }
            for (int i = 0; i < 5; i++)
            {
                sortToTypes(allPathes[i]);
            }
        }

        // check file type(movie/music/tv series)  :: (String filePath) 
        public void sortToTypes(string filePath)
        {
            FileInfo fileInfo = new FileInfo(System.IO.Path.GetFileName(filePath)); // holds fileName and extenstion
            
            // query to check if file is movie type
            var strings = new List<string> { ".mkv", ".avi", ".wmv", ".mp4" };
            bool contains = strings.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);
            if (contains) // if the extension is of A video file we go here
            {
                filePath.Replace(".", " ");filePath.Replace("-", " ");
                // TODO : identify if it A movie or other video.
                // movie regex : ([\.\w']+?)(\.[0-9]{4}\..*)
                Regex movieRegex = new Regex(@"([\.\w']+?)(\.[0-9]{4}\..*)");

                if (movieRegex.IsMatch(fileInfo.Name))
                {
                    // TO DO: EXTRCT SPECIFIC MOVIE NAME
                    if (fileInfo.Name.Contains("201") )
                    {
                        int x = fileInfo.Name.IndexOf("201");
                        string movieName = fileInfo.Name.Substring(0, x);
                        movieName = movieName.Replace(".", " ");
                        movieName = movieName.Replace("-", " ");
                        MessageBox.Show("its a movie!\n" + movieName);

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


        // create file object by type and call save to DB 

        // 

        #region Helper Methods

        #endregion Helper Methods
    }
}
