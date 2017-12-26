using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YoutubeVideo : Media
    {
        private string photoURL;
        private string channelId;
        private int id;

        public YoutubeVideo(string path, string name, string duration = "", string genre = "", string channelId ="",string photoURL = "") : base(path, name, duration, genre)
        {
            this.channelId = channelId;
            this.photoURL = photoURL;
        }

        public string PhotoURL { get => photoURL; set => photoURL = value; }
        public string ChannelId { get => channelId; set => channelId = value; }
        public int Id { get => id; set => id = value; }
    }
}
