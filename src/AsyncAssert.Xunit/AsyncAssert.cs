using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AsyncAssert.Xunit
{
    public static class AsyncAssert
    {
        private static IAsyncAssertLogger _logger;

        public static IAsyncAssertLogger Logger
        {
            get => _logger ?? new NullAsyncAssertLogger();
            set => _logger = value;
        }

        public static void TrueWithin(Func<bool> function, TimeSpan within, Func<string> getFailMsg)
        {
            TrueWithin(function, within, (TimeSpan?)null, getFailMsg);
        }

        public static void TrueWithin(Func<bool> func, TimeSpan timeSpan, Func<bool> inconclusive, Func<string> inconclusiveMsg, IList<Action> failureActions = null)
        {
            TrueWithin(func, timeSpan, (TimeSpan?)null, (Func<string>)null, failureActions);
        }

        public static async void TrueWithinAsync(Func<Task<bool>> function, TimeSpan within, TimeSpan? interval = null, Func<Task<string>> getFailMsg = null, IList<Func<Task>> failureActions = null)
        {
            Logger.Trace(
                "Asserting that function should be true within {0} seconds at {1}", within.TotalSeconds, DateTime.UtcNow);

            var limit = DateTime.Now.Add(within);
            do
            {
                if (await function())
                {
                    return;
                }
                failureActions = failureActions ?? new Func<Task>[0];
                failureActions.ToList().ForEach(async x => await x());
                Thread.Sleep((int)(interval == null ? 20 : interval.Value.TotalMilliseconds));
            } while (limit > DateTime.Now);

            string failMsg = getFailMsg != null ? await getFailMsg() : function.ToString();
            Assert.True(false, $"Expected function to be true within {within.TotalSeconds} seconds but it wasn't at {DateTime.UtcNow} [{failMsg}]");
        }

        public static void TrueWithin(Func<bool> function, TimeSpan within, TimeSpan? interval = null,
            Func<string> getFailMsg = null,
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
                failureActions.ToList().ForEach(x => x());
                Thread.Sleep((int)(interval == null ? 20 : interval.Value.TotalMilliseconds));
            } while (limit > DateTime.Now);

            string failMsg = getFailMsg != null ? TryGet(getFailMsg) : function.ToString();
            Assert.True(false,$"Expected function to be true within {within.TotalSeconds} seconds but it wasn't at {DateTime.UtcNow} [{failMsg}]");
        }

        private static string TryGet(Func<string> getFailMsg)
        {
            try
            {
                return getFailMsg();
            }
            catch
            {
                return "error getting error message";
            }
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
                    Assert.True(false,$"Expected function to remain false for {timespan.TotalSeconds} seconds but it wasn't");
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
                    Assert.True(false,$"Expected function to remain false for {timespan.TotalSeconds} seconds but it wasn't");
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
                        Assert.True(false,$"Expected function to remain false for {lower.TotalSeconds} seconds, but was true after {DateTime.Now.Subtract(start).TotalMilliseconds} milliseconds");
                    }
                    return;
                }
                Thread.Sleep(25);
            }
            Assert.True(false,$"Expected function to be true before {upper.TotalSeconds} seconds but it wasn't");
        }

    }

    public class NullAsyncAssertLogger : IAsyncAssertLogger
    {
        public void Trace(string msg, params object[] @params)
        {
        }

        public void Error(string msg, params object[] @params)
        {
        }
    }
}
