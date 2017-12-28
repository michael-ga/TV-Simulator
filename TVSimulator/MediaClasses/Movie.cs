using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class Movie : Media
    {
        protected string description;
        protected string director;
        protected string imdbRating;
        private string year;
        private int id;
        public Movie() { }

        public Movie(string path, string name, string duration = "", string genre = "",string director="",string description = "",string imdbRaiting="",string year= "") : base(path, name, duration, genre)
        {
            this.description = description;
            this.director = director;
            this.imdbRating = imdbRaiting;
            this.year = year;
            //TODO: parse the runtime string  to duration timespan.
        }

        public string Year { get => year; set => year = value; }
        public string Description { get => description; set => description = value; }
        public string Director { get => director; set => director = value; }
        public string ImdbRating { get => imdbRating; set => imdbRating = value; }
        public int Id { get => id; set => id = value; }

        public int getDurationInMin()
        {
            if (Duration == null || Duration == "")
                return -1;
            int x = Duration.IndexOf(".");
            return int.Parse(Duration.Substring(0, x));
             
        }
        
    }
}


