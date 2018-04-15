using MediaClasses;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    sum += durTime.TotalSeconds;
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
                    sum += durTime.TotalSeconds;
                    durationList.Add(sum);
                }
        }

        public void addMedia()
        {
            if (typeOfMedia.Equals(Constants.MOVIE))
            {
                List<Movie> movies = db.getMovieList();

                foreach (Movie m in movies)
                    if (m.getFirstGenre().Equals(genre))
                        media.Add((Media)m);
            }
            if (typeOfMedia.Equals(Constants.TVSERIES))
            {
                List<TvSeries> tvs = db.getTVList();

                foreach (TvSeries t in tvs)
                    if (t.getFirstGenre().Equals(genre))
                        media.Add((Media)t);
            }
        }

        public void bs(DateTime startDate)
        {
            Dictionary<DateTime, Media> board = new Dictionary<DateTime, Media>();
            int numOfDays = 7;
            var startTime = db.getTimes().StartTime;        // init lists from DB
            var endTime = db.getTimes().EndTime;            // init lists from DB
            DateTime date = startDate;
            DateTime temp;

            TimeSpan mediaDuration;
            var i = 0;
            var j = 0;  // run all over the movies
            var k = 0;
            var mediaLength = media.Count;

            //create live broadcast media

            while (k < numOfDays || j < mediaLength)
            {
                i = (int)date.DayOfWeek;

                DateTime finalHourDate = new DateTime(date.Year, date.Month, date.Day);
                finalHourDate = finalHourDate.AddHours(endTime[i]);     //end time in one day

                mediaDuration = media[j % mediaLength].getDurationTimespan();
                temp = date.Add(mediaDuration);                     // current date + media duration

                if (DateTime.Compare(temp, finalHourDate) < 0)     //if temp is earlier than finalhourDate
                {
                    board.Add(date, media[j % mediaLength]);
                    date = temp;
                    j++;
                }
                else
                {
                    temp = date.AddDays(1);
                    i = (int)temp.DayOfWeek;
                    date = new DateTime(temp.Year, temp.Month, temp.Day, startTime[i], 0, 0);
                    k++;
                }
            }
            //create repeat broadcast media

            i = j = k = 0;
            date = startDate;
            // set the last date for adding media until this date 
            var fd = board.Keys.Last().AddDays(1);          //day after final day 
            var stfs = startTime[(int)fd.DayOfWeek];        //start time of final day
            DateTime finalDate = new DateTime(fd.Year, fd.Month, fd.Day, stfs, 0, 0);

            Dictionary<DateTime, Media> tempList = new Dictionary<DateTime, Media>();

            while (DateTime.Compare(date, finalDate) < 0)
            {
                for (i = 0; i < board.Count; i++)
                {
                    var day = board.Keys.ElementAt(i).Day;
                    var month = board.Keys.ElementAt(i).Month;
                    var year = board.Keys.ElementAt(i).Year;
                    if (date.Year == year && date.Month == month && date.Day == day)
                        tempList.Add(board.Keys.ElementAt(i), board.Values.ElementAt(i));
                }
                temp = tempList.Keys.Last().Add(tempList.Values.Last().getDurationTimespan());      // start time to add repeat media
                
                DateTime startHourDate = new DateTime(date.Year, date.Month, date.Day);
                startHourDate = startHourDate.AddDays(1);
                startHourDate = startHourDate.AddHours(startTime[(int)startHourDate.DayOfWeek]);     //start time in one day

                while(DateTime.Compare(temp, startHourDate) < 0)     //if temp is earlier than startHourDate
                {
                    board.Add(temp, tempList.ElementAt(j % tempList.Count).Value);
                    var mediaDur = tempList.ElementAt(j % tempList.Count).Value.getDurationTimespan();
                    temp = temp.Add(mediaDur);
                    j++;
                }
                tempList.Clear();
                date = startHourDate;
                j = 0;
            }

            var l = board.OrderBy(key => key.Key);
            board = l.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

        }
    }
}

