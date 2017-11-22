using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class Movie : Media
    {
        private string year;
        protected string descryption;
        protected string director;
        protected string imdbRating;


        public Movie(string path, string name, string duration = "", string genre = "",string director="",string des="",string imdbRaiting="",string year= "") : base(path, name, duration, genre)
        {
            this.descryption = des;
            this.director = director;
            this.imdbRating = imdbRaiting;
            this.year = year;
            //TODO: parse the runtime string  to duration timespan.
        }

        public string Year { get => year; set => year = value; }
        public string Descryption { get => descryption; set => descryption = value; }
        public string Director { get => director; set => director = value; }
        public string ImdbRating { get => imdbRating; set => imdbRating = value; }
    }
}


