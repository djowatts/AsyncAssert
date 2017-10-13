using System;
using System.Threading;

namespace AsyncAssert.Core
{
    public static class Sleep
    {
        /// <summary>
        /// Syntatic sugar so you can write Sleep.For(5.Seconds());
        /// </summary>
        /// <param name="timeSpan"></param>
        public static void For(TimeSpan timeSpan)
        {
            Thread.Sleep((int)timeSpan.TotalMilliseconds);
        }
    }
}