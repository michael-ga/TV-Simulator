using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    class Channel
    {
        private string name;     //the name should be channel main genre 
        private MediaSchedule schedule;
        private Boolean type;    //local or youtube streaming 
        private Media playNow;   //which media played right now
        private Media playNext;  // optional
        private Boolean isWatchingNow;    // if user watching the channel- 1 else- 0
        
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
