using System;

namespace MediaClasses
{
    public class Movie
    {

        private string name;
        private string year;
        private string gnere;
        private string descryption;
        private string director;
        private string imdbRating;
        TimeSpan duaration;

        public Movie()
        {

        }
        public Movie(string name, string year = "", string gnere = "", string descryption = "", string director = "", string duaration ="",string imdbRating ="")
        {
            this.name = name;
            this.year = year;
            this.gnere = gnere;
            this.descryption = descryption;
            this.director = director;
            //TODO: parse the duration to A timespan.
        }


        public string Name { get => name; set => name = value; }
        public string Year { get => year; set => year = value; }
        public string Gnere { get => gnere; set => gnere = value; }
        public string Descryption { get => descryption; set => descryption = value; }
        public TimeSpan Duaration { get => duaration; set => duaration = value; }
        public string Director { get => director; set => director = value; }
        public string ImdbRating { get => imdbRating; set => imdbRating = value; }
    }
}
