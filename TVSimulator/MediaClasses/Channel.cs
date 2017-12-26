using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    class Channel
    {
        private int channelNumber;
        private string typeOfMedia;       //movies,tv serias,music or youtube stream
        private string genre;             //the name should be channel main genre 
        private MediaSchedule schedule;
        private Media playNow;   //which media played right now
        private Media playNext;  // optional
        
        public Channel()
        { }

        public Channel(int channelNumber,string typeOfMedia,string genre,MediaSchedule schedule)
        {
            this.channelNumber = channelNumber;
            this.genre = genre;
            this.typeOfMedia = typeOfMedia;
            this.schedule = schedule;
            //TODO:playNow and playNext is done when the schedule will be ready
        }

        //TODO:play function - play the current media from the schedule here in the constructor
    }
}
