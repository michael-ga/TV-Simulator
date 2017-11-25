using System;

namespace MediaClasses
{
    public class TvSeries : Media
    {
        private string episode;
        private string season;
        private string description;
        private string imdbRating;
        private string year;
        private int id;

        public TvSeries()
        { }

        public TvSeries(string path, string name, string duration = "", string genre = "", string season = "" , string episode = "", string description = "",string imdbRating = "",string year = "") : base(path, name, duration, genre)
        {
            this.episode = episode;
            this.season = season;
            this.description = description;
            this.Year = year;
            this.ImdbRating = imdbRating;
        }

        public string Episode { get => episode; set => episode = value; }
        public string Season { get => season; set => season = value; }
        public string Description { get => description; set => description = value; }
        public string Year { get => year; set => year = value; }
        public string ImdbRating { get => imdbRating; set => imdbRating = value; }
        public int Id { get => id; set => id = value; }

    }
}
