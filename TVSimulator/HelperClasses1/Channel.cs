using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelperClasses
{
    public class Channel
    {
        private int channelNumber;
        private string typeOfMedia;       //movies,tv serias,music or youtube stream
        private string genre;             //the name should be channel main genre
        private List<Media> media;
        private List<double> durationList;
        private Database db;
        private DateTime StartCycleTime;
        private string youtubeChannelID;
        private int youtubeVideoIndex = 0;
        private List<YoutubeVideo> youtubeVideoList;
        private int id;

        public List<Media> Media { get => media; set => media = value; }
        public List<double> DurationList { get => durationList; set => durationList = value; }
        public int ChannelNumber { get => channelNumber; set => channelNumber = value; }
        public string Genre { get => genre; set => genre = value; }
        public string TypeOfMedia { get => typeOfMedia; set => typeOfMedia = value; }
        public string YoutubeChannelID { get => youtubeChannelID; set => youtubeChannelID = value; }
        public int YoutubeVideoIndex { get => youtubeVideoIndex; set => youtubeVideoIndex = value; }
        public List<YoutubeVideo> YoutubeVideoList { get => youtubeVideoList; set => youtubeVideoList = value; }
        public int Id { get => id; set => id = value; }


        //private /*MediaSchedule*/ schedule;

        public Channel()
        { }

        public Channel(int channelNumber, string typeOfMedia, string genre)
        {
            this.channelNumber = channelNumber;
            this.genre = genre;
            this.typeOfMedia = typeOfMedia;

            this.media = new List<Media>();
            this.durationList = new List<double>();

            this.db = new Database();
            this.StartCycleTime = DateTime.Parse(Constants.START_CYCLE);

            this.YoutubeVideoList = new List<YoutubeVideo>();
            //this.schedule = schedule;
            //TODO:playNow and playNext is done when the schedule will be ready
        }

        public void buildSchedule()
        {
            if (typeOfMedia.Equals(Constants.MOVIE))
                buildMovieSchedule();
            if (typeOfMedia.Equals(Constants.TVSERIES))
                buildTVSchedule();
        }

        public void buildMovieSchedule()
        {
            List<Movie> movies = db.getMovieList();
            double sum = 0;
            var durTime = new TimeSpan();

            foreach (Movie m in movies)
                if (m.getFirstGenre().Equals(genre))
                {
                    media.Add((Media)m);
                    durTime = m.getDurationTimespan();
                    sum += durTime.TotalMilliseconds;
                    durationList.Add(sum);
                }
        }

        public void buildTVSchedule()
        {
            List<TvSeries> tvs = db.getTVList();
            double sum = 0;
            var durTime = new TimeSpan();

            foreach (TvSeries t in tvs)
                if (t.getFirstGenre().Equals(genre))
                {
                    media.Add((Media)t);
                    durTime = t.getDurationTimespan();    
                    sum += durTime.TotalMilliseconds;
                    durationList.Add(sum);
                }
        }
        

    }
}
