using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElbrusSkiResort
{
    public static class Timer
    {
        public static TimeSpan allTime { get; set; }

        public static bool limiter = true;
        public static void TimerStart()
        {
            allTime = new TimeSpan(0, 1, 0);
        }
    }
}
