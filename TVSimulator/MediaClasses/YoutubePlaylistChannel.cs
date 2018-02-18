using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YoutubePlaylistChannel : Media
    {
        private string photoURL;
        private string channelId;
        private string channelTitle;
        private List<YoutubePlaylist> playlist_list;
        private int id;

        public YoutubePlaylistChannel() { }

        public YoutubePlaylistChannel(string path, string name, string duration = "", string genre = "", string channelId = "", string photoURL = "", string channelTitle = "") : base(path, name, duration, genre)
        {
            this.channelId = channelId;
            this.channelTitle = channelTitle;
            this.photoURL = photoURL;
            playlist_list = null;
        }


        public string PhotoURL { get => photoURL; set => photoURL = value; }
        public string ChannelId { get => channelId; set => channelId = value; }
        public int Id { get => id; set => id = value; }
        public string ChannelTitle { get => channelTitle; set => channelTitle = value; }
        public List<YoutubePlaylist> Playlist_list { get => playlist_list; set => playlist_list = value; }
    }
}
