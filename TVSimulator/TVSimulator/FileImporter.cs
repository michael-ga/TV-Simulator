using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        }

        // check file type(movie/music/tv series)  :: (String filePath) 
        public void sortToTypes(string filePath)
        {
            FileInfo fileInfo = new FileInfo(System.IO.Path.GetFileName(filePath)); // holds fileName and extenstion
            
            // query to check if file is movie type
            var strings = new List<string> { "mkv", "avi", "wmv", "mp4" };
            bool contains = strings.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);
            if (contains) // if the extension is of A video file we go here
            {
                filePath.Replace(".", " ");filePath.Replace("-", " ");
                // TODO : identify if it A movie or other video.
            }
            //FileInfo f = new FileInfo(fileName);
        }


        // create file object by type and call save to DB 

        // 

        #region Helper Methods
        // return file name from path
        private string getFileName(string path)
        {
            string[] pathArr = path.Split('\\');
            string[] fileArr = pathArr.Last().Split('.');
            return fileArr.Last().ToString();
        }
        #endregion Helper Methods
    }
}
