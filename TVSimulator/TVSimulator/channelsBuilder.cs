﻿using System;
using MediaClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ChannelsBuilder()
        {
            this.db = new Database();
            this.movies = db.getMovieList();
            this.tvSeries = db.getTVList();
            this.musics = db.getMusicList();
        }

        public List<Channel> LocalChannels { get => localChannels; set => localChannels = value; }
        public List<Movie> Movies { get => movies; set => movies = value; }
        public List<TvSeries> TvSeries { get => tvSeries; set => tvSeries = value; }
        public List<Music> Music { get => musics; set => musics = value; }

        #endregion

        #region functions channels builder

        public void buildLocalChannels()
        {
            List<Channel> c = new List<Channel>();
            string genreStr = "";

            var channelNumber = 1;
            //create movie channels

            foreach (Movie m in movies)
            {
                if (m.Gnere != null)
                {
                    genreStr += m.Gnere;
                    genreStr += ",";
                }
            }

            string[] genre = getGenreArray(genreStr);
        
            for (var i=0;i < genre.Count();i++)
            {
                c.Add(new Channel(channelNumber,Constants.MOVIE, genre[i]));
                channelNumber++;
            }

            //create tv series channels
            genreStr = "";
            foreach (TvSeries t in tvSeries)
            {
                if (t.Gnere != null)
                {
                    genreStr += t.Gnere;
                    genreStr += ",";
                }
            }
            genre = getGenreArray(genreStr);
            for (var i = 0; i < genre.Count(); i++)
            {
                c.Add(new Channel(channelNumber, Constants.TVSERIES, genre[i]));
                channelNumber++;
            }
        }

        public void buildYouTubeChannels() {}
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

    }
}
