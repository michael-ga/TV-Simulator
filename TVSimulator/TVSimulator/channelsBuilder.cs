using System;
using MediaClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperClasses;

namespace TVSimulator
{
    public class ChannelsBuilder
    {
        #region fields

        private List<Channel> localChannels;
        private List<Channel> youTubeChannels;
        private Channel UserChannel;
        private Database db;

        private List<Movie> movies;
        private List<TvSeries> tvSeries;
        private List<Music> musics;
        private List<YouTubeChannel> youtube;

        public ChannelsBuilder()
        {
            this.db = new Database();
            this.movies = db.getMovieList();
            this.tvSeries = db.getTVList();
            this.musics = db.getMusicList();
            this.Youtube = db.getYoutubeChannelList();
        }

        public List<Channel> LocalChannels { get => localChannels; set => localChannels = value; }
        public List<Movie> Movies { get => movies; set => movies = value; }
        public List<TvSeries> TvSeries { get => tvSeries; set => tvSeries = value; }
        public List<Music> Music { get => musics; set => musics = value; }
        public List<YouTubeChannel> Youtube { get => youtube; set => youtube = value; }

        #endregion

        #region functions channels builder

        public void buildLocalChannels()
        {
            localChannels = new List<Channel>();

            List<string> gMovie = new List<string>();
            List<string> gTV = new List<string>();
            //List<string> gMusic = new List<string>();

            var channelNumber = 1;

            //create movie channels

            foreach (Movie m in movies)
                gMovie.Add(m.getFirstGenre());

            gMovie = gMovie.Distinct().ToList();

            for (var i = 0; i < gMovie.Count(); i++)
            {
                var chan = new Channel(channelNumber, Constants.MOVIE, gMovie.ElementAt(i));
                localChannels.Add(chan);
                localChannels.ElementAt(channelNumber - 1).addMedia();
                localChannels.ElementAt(channelNumber - 1).bs(getDateCycle());
                db.insertChannel(chan);
                channelNumber++;
            }

            //create tv series channels
            foreach (TvSeries t in tvSeries)
                gTV.Add(t.getFirstGenre());

            gTV = gTV.Distinct().ToList();
            for (var i = 0; i < gTV.Count(); i++)
            {
                var chan = new Channel(channelNumber, Constants.TVSERIES, gTV.ElementAt(i));
                localChannels.Add(chan);
                localChannels.ElementAt(channelNumber - 1).addMedia();
                localChannels.ElementAt(channelNumber - 1).bTVs(getDateCycle());
                db.insertChannel(chan);
                channelNumber++;
            }

            //buildYouTubeChannels();
            buildYouTubePlaylistChannels();
        }

        public void buildYouTubeChannels()
        {
            var channels = db.getYoutubeChannelList();
            var channelNumber = localChannels.Count() + 1;
            for (var i = 0; i < channels.Count(); i++)
            {
                var chan = new Channel(channelNumber, Constants.YOUTUBE_CHANNEL, "");
                localChannels.Add(chan);
                localChannels.ElementAt(channelNumber - 1).YoutubeChannelID = channels[i].Path;
                db.insertChannel(chan);
                channelNumber++;
            }
               
        }

        public void buildYouTubePlaylistChannels()
        {
            var ytbPlaylistChannels = db.getYoutubePlaylistChannelList();
            var channelNumber = db.getLastChannelIndex() + 1;
            localChannels = db.getChannelList();
            for (var i = 0; i < ytbPlaylistChannels.Count(); i++)
            {
                var chan = new Channel(channelNumber, Constants.YOUTUBE_PLAYLIST_CHANNEL, "");
                localChannels.Add(chan);
                localChannels.ElementAt(channelNumber - 1).YoutubeChannelID = ytbPlaylistChannels[i].Path;
                //localChannels.ElementAt(channelNumber - 1).addMedia();
                localChannels.ElementAt(channelNumber - 1).buildSchedule(); // add media here
                localChannels.ElementAt(channelNumber - 1).bTVs_YT(getDateCycle());  // build Dictionary schedule - Main algorithm

                db.insertChannel(chan);
                channelNumber++;
            }

        }
        public void buildUserChannel() {}

        #endregion

        #region subMethods

        private string[] getGenreArray(string genreStr)
        {
            string[] genre = genreStr.Split(',');
            genre = genre.Distinct().ToArray();
            genre = genre.Take(genre.Count() - 1).ToArray();
            return genre;
        }

        #endregion

        private DateTime getDateCycle()
        {
            int numOfDays = 7;
            BroadcastTime bt = db.getTimes();
            var timeNow = bt.StartCycleTime;
            var i = (int)timeNow.DayOfWeek;     //get number of day 0-sunday,1-monday,2-tuesday... 6-saturday
            var hourNow = timeNow.TimeOfDay.Hours;

            DateTime temp = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, 0, 0, 0);
            if (hourNow < bt.StartTime[i])
            {
                temp = temp.AddHours(bt.StartTime[i]);
                return temp;
            }
            else if (hourNow > bt.StartTime[i] && hourNow < bt.EndTime[i])
            {
                return timeNow;
            }
            else if (hourNow > bt.EndTime[i])
            {
                temp = temp.AddDays(1);
                temp = temp.AddHours(bt.StartTime[(i + 1) % numOfDays]);
                return temp;
            }
            return timeNow;

        }
    }
}
