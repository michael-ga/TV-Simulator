using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class Video
    {

        protected string name;
        protected string year;
        protected string gnere;
        protected string descryption;
        protected string director;
        protected string imdbRating;
        protected string type;
        private string runtime;
        TimeSpan duaration;

        public Video()
        {

        }
        public Video(string name, string type,string year = "", string gnere = "", string descryption = "", string director = "", string duaration = "", string imdbRating = "")
        {
            this.name = name;
            this.type = type;
            this.year = year;
            this.gnere = gnere;
            this.descryption = descryption;
            this.director = director;
            this.imdbRating = imdbRating;
            this.runtime = duaration;
            //TODO: parse the runtime string  to duration timespan.
        }


        public string Name { get => name; set => name = value; }
        public string Year { get => year; set => year = value; }
        public string Gnere { get => gnere; set => gnere = value; }
        public string Descryption { get => descryption; set => descryption = value; }
        public TimeSpan Duaration { get => duaration; set => duaration = value; }
        public string Director { get => director; set => director = value; }
        public string ImdbRating { get => imdbRating; set => imdbRating = value; }
        protected string Type { get => type; set => type = value; }
        protected string Runtime { get => runtime; set => runtime = value; }
    }
}


