namespace AsyncAssert
{
    using System;

    /// <summary>
    /// This class allows a total assert time to be configured for an entire test fixture.
    /// This is to prevent an integration test failure taking an inexorable amount of time due many long waiting asserts in seperate tests within a fixture 
    /// </summary>
    public class WaitTime
    {
        private DateTime? _dateTime;
        public WaitTime(TimeSpan timeSpan)
        {
            _dateTime = DateTime.UtcNow.Add(timeSpan);
        }

        public TimeSpan Remainder()
        {
            if (!_dateTime.HasValue)
            {
                _dateTime = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));
            }
            return TimeSpan.FromMilliseconds(Math.Max(10, (int)(_dateTime.Value - DateTime.UtcNow).TotalMilliseconds));
        }

        /// <summary>
        /// Returns min timespan between remainder and param
        /// </summary>
        /// <param name="timeSpan">Maximum time to wait, will be ignored if remainder is lower</param>
        /// <returns></returns>
        public TimeSpan MaxWait(TimeSpan timeSpan)
        {
            int millisecondsRemaining = Math.Max(10, (int)(_dateTime.Value - DateTime.UtcNow).TotalMilliseconds);
            double maxWaitTime = timeSpan.TotalMilliseconds;
            int waitTime = Math.Min(millisecondsRemaining, Convert.ToInt32(maxWaitTime));
            return TimeSpan.FromMilliseconds(waitTime);
        }

    }
}