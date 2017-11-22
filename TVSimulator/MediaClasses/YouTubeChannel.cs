using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    class YouTubeChannel : Media
    {
        private string description;

        public YouTubeChannel(string path, string name, string duration = "", string genre = "") : base(path, name, duration, genre)
        {
            this.Description = Description;
        }

        public string Description { get => description; set => description = value; }
    }
}
