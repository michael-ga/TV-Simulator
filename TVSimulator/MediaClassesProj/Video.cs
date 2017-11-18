using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaClassesProj
{
    [BsonIgnoreExtraElements]
    public class Video
    {
        [BsonElement("name")]
        protected string name;
        [BsonElement("year")]
        protected string year;
        [BsonElement("gnere")]
        protected string gnere;
        [BsonElement("description")]
        protected string descryption;
        [BsonElement("director")]
        protected string director;
        [BsonElement("raiting")]
        protected string imdbRating;
        [BsonElement("type")]
        protected string type;
        [BsonElement("runtime")]
        private string runtime;
        TimeSpan duaration;

        public Video()
        {

        }
        public Video(string name, string type,string year = "", string gnere = "", string descryption = "", string director = "", string duaration = "", string imdbRating = "")
        {
            this.name = name;
            this.type = type;
            this.year = year;
            this.gnere = gnere;
            this.descryption = descryption;
            this.director = director;
            this.imdbRating = imdbRating;
            this.runtime = duaration;
            //TODO: parse the runtime string  to duration timespan.
        }


        public string Name { get => name; set => name = value; }
        public string Year { get => year; set => year = value; }
        public string Gnere { get => gnere; set => gnere = value; }
        public string Descryption { get => descryption; set => descryption = value; }
        public TimeSpan Duaration { get => duaration; set => duaration = value; }
        public string Director { get => director; set => director = value; }
        public string ImdbRating { get => imdbRating; set => imdbRating = value; }
        protected string Type { get => type; set => type = value; }
        protected string Runtime { get => runtime; set => runtime = value; }
    }
}


