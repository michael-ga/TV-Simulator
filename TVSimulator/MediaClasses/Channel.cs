using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class Channel
    {
        private int channelNumber;
        private string typeOfMedia;       //movies,tv serias,music or youtube stream
        private string genre;             //the name should be channel main genre 
        private MediaSchedule schedule;
        
        public Channel()
        { }

        public Channel(int channelNumber,string typeOfMedia,string genre)
        {
            this.channelNumber = channelNumber;
            this.genre = genre;
            this.typeOfMedia = typeOfMedia;
            //this.schedule = schedule;
            //TODO:playNow and playNext is done when the schedule will be ready
        }
    }
}
