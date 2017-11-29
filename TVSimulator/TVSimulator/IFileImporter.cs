﻿using MediaClasses;
using System.Collections.Generic;
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
    
    }
}
