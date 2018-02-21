using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YoutubePlaylistChannel : Media
    {
        private string photoURL;
        private List<YoutubePlaylist> playlist_list;
        private int id;
        private DateTime lastUpdated;

        public YoutubePlaylistChannel() { }

        public YoutubePlaylistChannel(string path, string name, string duration = "", string genre = "", string photoURL = "") : base(path, name, duration, genre)
        {
            this.photoURL = photoURL;
            playlist_list = null;
            lastUpdated = DateTime.MinValue;
        }


        public string PhotoURL { get => photoURL; set => photoURL = value; }
        public int Id { get => id; set => id = value; }
        public List<YoutubePlaylist> Playlist_list { get => playlist_list; set => playlist_list = value; }
        public DateTime LastUpdated { get => lastUpdated; set => lastUpdated = value; }
    }
}
