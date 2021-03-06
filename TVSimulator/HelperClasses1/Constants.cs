﻿
namespace HelperClasses
{
    public class Constants
    {
        // arrays and collections
        //public const string [] videoExt = new co{ ".mkv", ".avi", ".wmv", ".mp4" };
        // file importer constants
        public const int YOUTUBE_PROGRESS_REPORT_RETRIES = 6;
        public const string OMDB_APIKEY = "77f17a4d";
        public const string MOVIE = "movie";
        public const string TVSERIES = "TVseries";
        public const string MUSIC = "music";
        public const string YOUTUBE_CHANNEL = "YouTubeChannel";
        public const string YOUTUBE_PLAYLIST_CHANNEL = "YouTubePlaylistChannel";


        public const string DB_DIR_PATH = @"C:\\TVSimulatorDB";
        public const string DB_FILE_PATH = DB_DIR_PATH + "\\MyData.db";

        // DB constants
        public const string MEDIA_COLLECTION = "media";
        public const string MOVIE_COLLECTION = "movie";
        public const string TV_SERIES_COLLECTION = "tvseries";
        public const string MUSIC_COLLECTION = "music";
        public const string YOUTUBE_CHANNEL_COLLECTION = "youtube_channel";
        public const string YOUTUBE_VIDEO_COLLECTION = "youtube_video";
        public const string YOUTUBE_PLAYLIST_CHANNEL_COLLECTION = "youtube_playlist";
        public const string CHANNEL_COLLECTION = "channel";
        public const string TIME_COLLECTION = "times";
        //times constants 
        public const string START_CYCLE = "26/12/2017 13:30:00";
        public string CONNECTION_STRING = @"C:\TVSimulatorDB\MyData.db";
        public enum channelSwitch
        {
            decreament, increament
        }

        public enum playerSwitch
        {
            Local,Youtube
        }


        // algorithm constants
        public const int MAX_DICT_KEYS = 1000;

    }
}
