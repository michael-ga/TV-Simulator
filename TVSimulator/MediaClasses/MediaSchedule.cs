using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class MediaSchedule
    {
        private Dictionary<DateTime, string> boardSchedule;
    
        private Media playNow;   //which media played right now
        private Media playNext;  // optional

        public MediaSchedule() { }

        public MediaSchedule(Dictionary<DateTime, string> boardSchedule)
        {
            this.boardSchedule = boardSchedule;
        }

        public Dictionary<DateTime, string> BoardSchedule { get => boardSchedule; set => boardSchedule = value; }
        public Media PlayNow { get => playNow; set => playNow = value; }
        public Media PlayNext { get => playNext; set => playNext = value; }

        //TODO: constructor and functions
    }

}
