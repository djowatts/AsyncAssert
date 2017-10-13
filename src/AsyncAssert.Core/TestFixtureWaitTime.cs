using System;

namespace AsyncAssert.Core
{
    public static class TestFixtureWaitTime
    {
        private static DateTime? _dateTime;
        public static TimeSpan SetUp(TimeSpan timeSpan)
        {
            _dateTime = DateTime.UtcNow.Add(timeSpan);
            return timeSpan;
        }

        public static TimeSpan Remainder()
        {
            if (!_dateTime.HasValue)
            {
                _dateTime = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));
            }
            return TimeSpan.FromMilliseconds(Math.Max(10, (int)(_dateTime.Value - DateTime.UtcNow).TotalMilliseconds));
        }

        public static TimeSpan MaxWait(TimeSpan timeSpan)
        {
            int millisecondsRemaining = Math.Max(10, (int)(_dateTime.Value - DateTime.UtcNow).TotalMilliseconds);
            double maxWaitTime = timeSpan.TotalMilliseconds;
            int waitTime = Math.Min(millisecondsRemaining, Convert.ToInt32(maxWaitTime));
            return TimeSpan.FromMilliseconds(waitTime);
        }

        private static void SetTime(DateTime dateTime)
        {
            _dateTime = dateTime;
        }
    }
}