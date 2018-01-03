using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace TVSimulator
{
    public class Channel
    {
        private int channelNumber;
        private string typeOfMedia;       //movies,tv serias,music or youtube stream
        private string genre;             //the name should be channel main genre
        private List<Media> media;
        private List<int> durationList;
        private Database db;
        private DateTime StartCycleTime;

        public List<Media> Media { get => media; set => media = value; }
        public List<int> DurationList { get => durationList; set => durationList = value; }
        public int ChannelNumber { get => channelNumber; set => channelNumber = value; }
        public string Genre { get => genre; set => genre = value; }
        public string TypeOfMedia { get => typeOfMedia; set => typeOfMedia = value; }

        //private /*MediaSchedule*/ schedule;

        public Channel()
        { }

        public Channel(int channelNumber, string typeOfMedia, string genre)
        {
            this.channelNumber = channelNumber;
            this.genre = genre;
            this.typeOfMedia = typeOfMedia;

            this.media = new List<Media>();
            this.durationList = new List<int>();

            this.db = new Database();
            this.StartCycleTime = DateTime.Parse(Constants.START_CYCLE); 


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
            var sum = 0;
            foreach (Movie m in movies)
                if (m.getFirstGenre().Equals(genre))
                {
                    if (m.getDurationInMin() != -1)
                    {
                        media.Add((Media)m);
                        sum += m.getDurationInMin();
                        durationList.Add(sum);
                    }
                }
        }

        public void buildTVSchedule()
        {
            List<TvSeries> tvs = db.getTVList();
            var sum = 0;

            foreach (TvSeries t in tvs)
                if (t.getFirstGenre().Equals(genre))
                {
                    if (t.getDurationInMin() != -1)
                    {
                        media.Add((Media)t);
                        sum += t.getDurationInMin();
                        durationList.Add(sum);
                    }
                }
        }

    }
}
