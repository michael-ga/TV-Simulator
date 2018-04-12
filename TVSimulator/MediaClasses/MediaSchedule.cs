using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    public class MediaSchedule
    {
        private Dictionary<DateTime, Media> boardSchedule;
        private Media playNow;   //which media played right 
        private Media playNext;  // optional

        public MediaSchedule() { }

        public MediaSchedule(Dictionary<DateTime, Media> boardSchedule)
        {
            this.boardSchedule = boardSchedule;
        }

        public Dictionary<DateTime, Media> BoardSchedule { get => boardSchedule; set => boardSchedule = value; }
        public Media PlayNow { get => playNow; set => playNow = value; }
        public Media PlayNext { get => playNext; set => playNext = value; }

        //TODO: constructor and functions
    }

}
