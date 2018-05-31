using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YoutubeVideo : Media
    {
        private string photoURL;
        private string channelId;
        private string description;
        private int id;
        public YoutubeVideo() { }
        public YoutubeVideo(string path, string name, string duration = "", string genre = "", string channelId ="",string photoURL = "",string description ="") : base(path, name, duration, genre)
        {
            this.channelId = channelId;
            this.photoURL = photoURL;
            this.description = description;
        }
        public TimeSpan getTimeSpan()
        {
            return System.Xml.XmlConvert.ToTimeSpan(duration);
        }
        
        public string PhotoURL { get => photoURL; set => photoURL = value; }
        public string ChannelId { get => channelId; set => channelId = value; }
        public int Id { get => id; set => id = value; }
        public string Description { get => description; set => description = value; }
    }
}
