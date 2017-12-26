using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    class MediaSchedule
    {
        private List<Media> mediaList;
        private List<TimeSpan> durationList;

        private int numOfMedia;
        private TimeSpan totalDuration;

        private Media playNow;   //which media played right now
        private Media playNext;  // optional

        public MediaSchedule() {   }
    
        //TODO: constructor and functions
    }

}
