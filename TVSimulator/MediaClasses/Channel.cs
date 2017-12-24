using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    class Channel
    {
        private string name;     //the name need to be channel main genre 
        private MediaSchedule schedule;
        private Boolean type;    //local or youtube streaming TODO:local variables to LOCAL_CHANNEL , YOUTUBE_CHANNEL
        private Media playNow;
        private Media playNext;  // optional

        public Channel()
        { }

        public Channel(string name,MediaSchedule schedule,Boolean type)
        {
            this.name = name;
            this.schedule = schedule;
            this.type = type;
            //TODO:playNow and playNext is done when the schedule will be ready
        }

        //TODO:play function - play the current media from the schedule here in the constructor

    }
}
