
namespace HelperClasses
{
    public class Constants
    {
        // arrays and collections
        //public const string [] videoExt = new co{ ".mkv", ".avi", ".wmv", ".mp4" };
        // file importer constants
        public const string OMDB_APIKEY = "77f17a4d";
        public const string MOVIE = "movie";
        public const string TVSERIES = "TVseries";
        public const string MUSIC = "music";
        public const string YOUTUBE_CHANNEL = "YouTubeChannel";

        public const string DB_FILE_PATH = @"C:\\TVSimulatorDB";
        // DB constants
        public const string MEDIA_COLLECTION = "media";
        public const string MOVIE_COLLECTION = "movie";
        public const string TV_SERIES_COLLECTION = "tvseries";
        public const string MUSIC_COLLECTION = "music";
        public const string YOUTUBE_CHANNEL_COLLECTION = "youtube_channel";
        public const string YOUTUBE_VIDEO_COLLECTION = "youtube_video";
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

    }
}
