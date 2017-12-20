using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YoutubeVideo : Media
    {
        private string url;
        private string channelId;
        public YoutubeVideo(string path, string name, string duration = "", string genre = "", string channelId ="") : base(path, name, duration, genre)
        {
            this.channelId = channelId;
        }

        public string Url { get => url; set => url = value; }
        public string ChannelId { get => channelId; set => channelId = value; }
    }
}
