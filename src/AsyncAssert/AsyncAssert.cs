// ReSharper disable RedundantNameQualifier - for readability
// ReSharper disable RedundantCast - casts needed for mono 3.x.x
namespace AsyncAssert
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using NUnit.Framework;

    public static class AsyncAssert//this class is required because we are in publish and subscribe land (opposed to call and reply)
    {
        private static IAsyncAssertLogger _logger;

        public static IAsyncAssertLogger Logger
        {
            get { return _logger ?? new NullAsyncAssertLogger(); }
            set { _logger = value; }
        }

        public static void TrueWithin(Func<bool> function, TimeSpan within,  Func<string> getFailMsg)
        {
            TrueWithin(function,within,(TimeSpan?)null,getFailMsg);
        }

        public static void TrueWithin(Func<bool> func, TimeSpan timeSpan, Func<bool> inconclusive,Func<string> inconclusiveMsg,IList<Action> failureActions = null)
        {
            TrueWithin(func, timeSpan, (TimeSpan?)null,(Func<string>) null, inconclusive, inconclusiveMsg,failureActions);
        }

        public static void TrueWithin(Func<bool> function, TimeSpan within, TimeSpan? interval = null, 
            Func<string> getFailMsg = null, 
            Func<bool> inconclusive = null, 
            Func<string> getInconclusiveMsg = null,
            IList<Action> failureActions = null)
        {
            Logger.Trace(
                "Asserting that function should be true within {0} seconds at {1}", within.TotalSeconds, DateTime.UtcNow);

            var limit = DateTime.Now.Add(within);
            do
            {
                if (GetResult(function))
                {
                    return;
                }
                failureActions = failureActions ?? new Action[0];
                failureActions.ToList().ForEach(x=>x());
                Thread.Sleep((int) (interval == null ? 20 : interval.Value.TotalMilliseconds));
            } while (limit > DateTime.Now);

            string inconclusiveMsg = getInconclusiveMsg != null ? getInconclusiveMsg() : "No inconclusive msg";
            if (inconclusive != null && inconclusive())
            {
                Assert.Inconclusive(inconclusiveMsg);
            }
            string failMsg = getFailMsg != null ? getFailMsg() : function.ToString();
            Assert.Fail("Expected function to be true within {0} seconds but it wasn't at {1} [{2}]", within.TotalSeconds, DateTime.UtcNow, failMsg);
        }

        private static bool GetResult(Func<bool> function)
        {
            try
            {
                return function();
            }
            catch (Exception e)
            {
                Logger.Error("While testing for assert the exception was thrown {0}", e.Message);
                return false;
            }
        }

        public static void TrueWithin(Func<bool> function, Action callbackTest, TimeSpan within, TimeSpan? interval = null)
        {
            var callbackList = new List<Action> { callbackTest };
            TrueWithin(function, callbackList, within, interval);
        }

        public static void TrueWithin(Func<bool> function, List<Action> callbackTests, TimeSpan within, TimeSpan? interval = null)
        {
            var limit = DateTime.Now.Add(within);
            while (limit > DateTime.Now)
            {
                Thread.Sleep((int)(interval == null ? 25 : interval.Value.TotalMilliseconds));
                if (function())
                {
                    callbackTests.ForEach(t => t());
                    return;
                }
            }
        }

        public static void RemainsFalseFor(Func<bool> function, TimeSpan timespan)
        {
            var limit = DateTime.Now.Add(timespan);
            while (limit > DateTime.Now)
            {
                if (function())
                {
                    Assert.Fail("Expected function to remain false for {0} seconds but it wasn't", timespan.TotalSeconds);
                }
                Thread.Sleep(50);
            }
        }

        public static void RemainsTrueFor(Func<bool> function, TimeSpan timespan)
        {
            var limit = DateTime.Now.Add(timespan);
            while (limit > DateTime.Now)
            {
                if (!function())
                {
                    Assert.Fail("Expected function to remain false for {0} seconds but it wasn't", timespan.TotalSeconds);
                }
                Thread.Sleep(50);
            }
        }

        public static void TrueBetween(Func<bool> function, TimeSpan lower, TimeSpan upper)
        {
            var start = DateTime.Now;
            var upperLimit = DateTime.Now.Add(upper);
            var lowerLimit = DateTime.Now.Add(lower);
            while (upperLimit > DateTime.Now)
            {
                if (function())
                {
                    if (DateTime.Now < lowerLimit)
                    {
                        Assert.Fail("Expected function to remain false for {0} seconds, but was true after {1} milliseconds", lower.TotalSeconds, DateTime.Now.Subtract(start).TotalMilliseconds);
                    }
                    return;
                }
                Thread.Sleep(25);
            }
            Assert.Fail("Expected function to be true before {0} seconds but it wasn't", upper.TotalSeconds);
        }
    }
}