using MediaClasses;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TVSimulator
{
    interface IFileImporter
    {

        void getLocalFilesToDB(string path,bool includeSubfolders);//tested

        void removeFileFromDB(string name);

        Media getFileFromDB(string name);

        void removeCollectionFromDB(string collectionName);//tested

        List<Media> getMediaListFromDB();//tested

        List<Movie> getMovieListFromDB();//tested

        List<Music> getMusicListFromDB();//tested

        List<TvSeries> getTVseriesListFromDB();//tested

        //***************************************************************************//

        void getAllMediaFromDirectory(string path, bool isIncludeSubfolders);

        Task<bool> SortMediaToTypes(string filePath);

        Task<bool> videoHandler(FileInfo fileInfo, string type, string filePath);

        void musicHandler(string path, string fileName);

        string extractVideoName(string fullName, string compareArg);

        Task<Media> extendVideoInfo(string videoName, string path, string type);

    }
}
