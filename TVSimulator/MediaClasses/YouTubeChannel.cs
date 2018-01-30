using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YouTubeChannel : Media
    {
        private string description;
        private string photoURL;
        private int id;
        private int currentVideoIndex;
        private List<YoutubeVideo> videoList;
        public YouTubeChannel() { }

        public YouTubeChannel(string path, string name, string duration = "", string genre = "",string photoURL="") : base(path, name, duration, genre)
        {
            this.Description = Description;
            this.photoURL = photoURL;
            currentVideoIndex = 0;
        }

        public string Description { get => description; set => description = value; }
        public string PhotoURL { get => photoURL; set => photoURL = value; }
        public int Id { get => id; set => id = value; }
        public int CurrentVideoIndex { get => currentVideoIndex; set => currentVideoIndex = value; }
        public List<YoutubeVideo> VideoList { get => videoList; set => videoList = value; }
    }
}
