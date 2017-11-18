using System;

namespace UI
{
    public class TvSeries : Video
    {
        string episode;
        string season;
        public TvSeries(Video video,string episode = "",string season = "")
        {
            this.episode = episode;
            this.season = season;
        }

        public string Episode { get => episode; set => episode = value; }
        public string Season { get => season; set => season = value; }
    }
}
