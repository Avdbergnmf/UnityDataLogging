using UnityEngine;

namespace UnityDataLogging.Utility
{
    public class TimeManager : MonoBehaviour // By: Nicolas Wenk - https://github.com/Artorg-MCLab/RobotDeletion/commits?author=nicolaswenk
    {
        protected static System.Diagnostics.Stopwatch timer;
        private static long nanosecPerTick;
        //private static float? initTimeShift = null;
    
        void Awake()
        {
            if (timer == null)
            {
                long frequency = System.Diagnostics.Stopwatch.Frequency;
                Debug.Log("Timer frequency in ticks per second = " + frequency);
                nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
                Debug.Log("Timer is accurate within " + nanosecPerTick + " nanoseconds");

                timer = System.Diagnostics.Stopwatch.StartNew();
            }
        }

        /// <summary>
        /// Return the system time in seconds based on the number of elapsed ticks.
        /// </summary>
        public float SystemTime
        {
            get { return timer.ElapsedTicks * nanosecPerTick / 1000000000.0f; }
        }
    }
}
