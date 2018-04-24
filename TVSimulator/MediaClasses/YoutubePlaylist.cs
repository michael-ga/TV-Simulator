using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YoutubePlaylist : Media
    {

        private string photoURL;
        private int id;
        private List<YoutubePlaylistVideo> _videos;
       
        public YoutubePlaylist() { }

        public YoutubePlaylist(string path, string name, string duration = "", string genre = "", string photoURL = "") : base(path, name, duration, genre)
        {
            this.photoURL = photoURL;
        }

        public string PhotoURL { get => photoURL; set => photoURL = value; }
        public int Id { get => id; set => id = value; }
        public List<YoutubePlaylistVideo> Videos { get => _videos; set => _videos = value; }
    }
}
