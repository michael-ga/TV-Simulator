using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class Media
    {
        protected string path;
        protected string name;
        protected string genre;
        protected string duration;
        private int id;
        public Media()
        {

        }
        public Media(string path,string name, string duration = "",string genre = "")
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
    }
}
