using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class Video
    {
        protected string _path;
        protected string _name;
        protected string _year;
        protected string _gnere;
        protected string _descryption;
        protected string _director;
        protected string _imdbRating;
        protected string _type;
        private string _runtime;

        public Video()
        {

        }
        public Video(string path, string name, string type, string year = "", string gnere = "", string descryption = "", string director = "", string duaration = "", string imdbRating = "")
        {
            this._path = path;
            this._name = name;
            this._type = type;
            this._year = year;
            this._gnere = gnere;
            this._descryption = descryption;
            this._director = director;
            this._imdbRating = imdbRating;
            this._runtime = duaration;
            //TODO: parse the runtime string  to duration timespan.
        }


        public string Name { get => _name; set => _name = value; }
        public string Year { get => _year; set => _year = value; }
        public string Gnere { get => _gnere; set => _gnere = value; }
        public string Descryption { get => _descryption; set => _descryption = value; }
        public string Director { get => _director; set => _director = value; }
        public string ImdbRating { get => _imdbRating; set => _imdbRating = value; }
        protected string Type { get => _type; set => _type = value; }
        protected string Runtime { get => _runtime; set => _runtime = value; }
        public string Path { get => _path; set => _path = value; }
    }
}


