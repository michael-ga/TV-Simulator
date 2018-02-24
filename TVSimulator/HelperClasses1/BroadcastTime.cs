using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperClasses
{
    public class BroadcastTime
    {
        private int id;
        DateTime startCycleTime; // start date to broadcast media 
        int[] startTime;
        int[] endTime;

        public const int DAY_IN_WEEK = 7;

        public BroadcastTime(){}

        public BroadcastTime(DateTime startCycleTime, int[] startTime, int[] endTime)
        {
            this.startCycleTime = startCycleTime;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public DateTime StartCycleTime { get => startCycleTime; set => startCycleTime = value; }
        public int[] StartTime { get => startTime; set => startTime = value; }
        public int[] EndTime { get => endTime; set => endTime = value; }
        public int Id { get => id; set => id = value; }

    }
}
