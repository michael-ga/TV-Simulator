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
        private string lyrics;
        private int id;

        public Music() { }
        public Music(string path, string name, string duration = "", string genre = "", string artist="", string album="", string year="",string lyrics = "") : base(path, name, duration, genre)
        {
            this.artist = artist;
            this.album = album;
            this.year = year;
            this.lyrics = lyrics;
        }

        public string Artist { get => artist; set => artist = value; }
        public string Album { get => album; set => album = value; }
        public string Year { get => year; set => year = value; }
        public string Lyrics { get => lyrics; set => lyrics = value; }
        public int Id { get => id; set => id = value; }
    }
}
