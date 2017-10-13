using System;
using System.Threading;

namespace AsyncAssert.Core
{
    public static class Wait
    {
        public static bool UntilTrueOrTimeout(Func<bool> function, TimeSpan timeout, TimeSpan? checkInterval = null)
        {
            var limit = DateTime.Now.Add(timeout);
            while (limit > DateTime.Now)
            {
                try
                {
                    if (function())
                    {
                        return true;
                    }
                }
                catch { }
                Thread.Sleep(checkInterval ?? TimeSpan.FromSeconds(1));
            }
            return false;
        }
    }
}