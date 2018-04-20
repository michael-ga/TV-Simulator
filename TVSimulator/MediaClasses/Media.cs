using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class Media
    {
        public const string GENERAL_GENRE = "general";

        protected string path;
        protected string name;
        protected string genre;
        protected string duration;
        private int id;
        public Media()
        {

        }
        public Media(string path,string name, string duration = "",string genre = "general")
        {
            this.path = path;
            this.name = name;
            this.duration = duration;
            this.genre = genre;
        }

        public override string ToString()
        {
            return " name: " + name + "\n path: " + path + " \n duration: " + duration + " \n genre: " + genre;
        
        }
        public string Duration { get => duration; set => duration = value; }
        public string Gnere { get => genre; set => genre = value; }
        public string Path { get => path; set => path = value; }
        public string Name { get => name; set => name = value; }
        public int Id { get => id; set => id = value; }

        public string getFirstGenre()
        {
            if(genre == null)
                return GENERAL_GENRE;
            int x = genre.IndexOf(",");

            if (genre.Equals(GENERAL_GENRE) || x < 0)
                return genre;
            return genre.Substring(0,x);
        }

        public TimeSpan getDurationTimespan()
        {
            if (this is YoutubeVideo)
            {
                Double durVal = Double.Parse(duration);
                return TimeSpan.FromSeconds(durVal);
            }
            return TimeSpan.Parse(duration);
        }
    }
}
