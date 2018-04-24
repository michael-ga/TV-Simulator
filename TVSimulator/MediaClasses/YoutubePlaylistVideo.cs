using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YoutubePlaylistVideo : Media
    {
        private string photoURL;
        private string playlistId;
        private int id;
        public YoutubePlaylistVideo() { }
        private string season = "01";
        private string episode;
        public YoutubePlaylistVideo(string path, string name, string duration = "", string genre = "", string playlistId = "", string photoURL = "",string episode = "") : base(path, name, duration, genre)
        {
            this.playlistId = playlistId;
            this.photoURL = photoURL;
            this.episode = episode;
        }
        public TimeSpan getTimeSpan()
        {
            return System.Xml.XmlConvert.ToTimeSpan(duration);
        }

        public string PhotoURL { get => photoURL; set => photoURL = value; }
        public string ChannelId { get => playlistId; set => playlistId = value; }
        public int Id { get => id; set => id = value; }
        public string Episode { get => episode; set => episode = value; }
        public string Season { get => season; set => season = value; }
    }
}
