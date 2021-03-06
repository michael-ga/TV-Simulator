﻿using System;
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
        private List<Media> mediaVideos;
        private List<YouTubeChannel> youtube;

        public ChannelsBuilder()
        {
            this.db = new Database();
            this.movies = db.getMovieList();
            this.tvSeries = db.getTVList();
            this.mediaVideos = db.getAllMediaList();
            this.Youtube = db.getYoutubeChannelList();
        }

        public List<Channel> LocalChannels { get => localChannels; set => localChannels = value; }
        public List<Movie> Movies { get => movies; set => movies = value; }
        public List<TvSeries> TvSeries { get => tvSeries; set => tvSeries = value; }
        public List<Media> Media { get => mediaVideos; set => mediaVideos = value; }
        public List<YouTubeChannel> Youtube { get => youtube; set => youtube = value; }

        #endregion

        #region functions channels builder


        public void rebuildAllChannels()
        {
            db.removeChannelCollection();
            buildLocalChannels();
            buildYouTubeChannels();
            buildYouTubePlaylistChannels();
        }
        
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

            // add general channels
            if (mediaVideos == null || mediaVideos.Count()==0)
                return;
            var gChan = new Channel(channelNumber, Constants.MOVIE, "General");
            localChannels.Add(gChan);
            localChannels.ElementAt(channelNumber - 1).addMedia();
            localChannels.ElementAt(channelNumber - 1).bs(getDateCycle());
            db.insertChannel(gChan);
            channelNumber++;
        }

        public void buildYouTubeChannels()
        {
            var channels = db.getYoutubeChannelList();
            var channelNumber = db.getLastChannelIndex() + 1;
            for (var i = 0; i < channels.Count(); i++)
            {
                var chan = new Channel(channelNumber, Constants.YOUTUBE_CHANNEL, "");
                chan.YoutubeChannelID = channels[i].Path; 
                if (chan.buildYoutubeSchedule() == -1)    // if no videos not adding this channel
                    continue;

                BroadcastTime bt = db.getTimes();
                int[] startTime = bt.StartTime;
                int[] endTime = bt.EndTime;
                var tmp = new BroadcastTime(DateTime.Now, startTime, endTime);


                if (chan.bs(getDateCycle()) == -1)
                    continue;


                db.insertBroadcastTime(tmp);
                chan.YoutubeChannelName = channels[i].Name;
                localChannels.Add(chan);
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
                chan.YoutubeChannelID = ytbPlaylistChannels[i].Path;
                if (chan.buildYoutubePlaylistSchedule() == -1)    // if no videos not adding this channel
                    continue;

                BroadcastTime bt = db.getTimes();
                int[] startTime = bt.StartTime;
                int[] endTime = bt.EndTime;
                var tmp = new BroadcastTime(DateTime.Now, startTime, endTime);

                if (chan.bTVs_YT(getDateCycle()) == -1)
                    continue;

                chan.YoutubeChannelName = ytbPlaylistChannels[i].Name;
                localChannels.Add(chan);
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

        public DateTime getDateCycle()
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
