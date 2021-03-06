﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class YouTubeChannel : Media
    {
        private string description;
        private string photoURL;
        private int id;

        public YouTubeChannel() { }

        public YouTubeChannel(string path, string name, string duration = "", string genre = "",string photoURL="") : base(path, name, duration, genre)
        {
            this.Description = Description;
            this.photoURL = photoURL;
        }

        public string Description { get => description; set => description = value; }
        public string PhotoURL { get => photoURL; set => photoURL = value; }
        public int Id { get => id; set => id = value; }
    }
}
