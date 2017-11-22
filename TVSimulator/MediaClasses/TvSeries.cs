using System;

namespace MediaClasses
{
    public class TvSeries : Media
    {
        private string episode;
        private string season;


        public TvSeries(string path, string name, string duration = "", string genre = "", string episode = "", string season = "") : base(path, name, duration, genre)
        {
            this.episode = episode;
            this.season = season;
        }

        public string Episode { get => episode; set => episode = value; }
        public string Season { get => season; set => season = value; }
    }
}
