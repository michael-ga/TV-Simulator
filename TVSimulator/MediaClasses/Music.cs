using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class Music : Media
    {
        private string artist;
        private string album;
        private string year;

        public Music(string path, string name, string duration = "", string genre = "", string artist, string album, string year) : base(path, name, duration, genre)
        {
            this.artist = artist;
            this.album = album;
            this.year = year;
        }

        public string Artist { get => artist; set => artist = value; }
        public string Album { get => album; set => album = value; }
        public string Year { get => year; set => year = value; }
    }
}
