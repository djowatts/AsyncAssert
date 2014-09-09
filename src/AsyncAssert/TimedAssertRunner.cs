namespace AsyncAssert
{
    using System;
    using System.Collections.Generic;

    public class TimedAssertRunner
    {
        private readonly WaitTime _waitTime;

        public WaitTime WaitTime
        {
            get { return _waitTime; }
        }
        public TimedAssertRunner(TimeSpan ts)
        {
            _waitTime = new WaitTime(ts);
        }

        public TimedAssertRunner(WaitTime waitTime)
        {
            _waitTime = waitTime;
        }

        public IList<Action> FailureActions { get; set; }

        public void TrueBeforeTimeout(Func<bool> test, Func<string> failMsg = null, Func<bool> inconclusiveTest = null, Func<string> inconclusiveMessage = null)
        {
            var interval = TimeSpan.FromMilliseconds(100);
            AsyncAssert.TrueWithin(test, _waitTime.Remainder(),interval , failMsg, inconclusiveTest, inconclusiveMessage,
                FailureActions);
        }

        public void EqualBeforeTimeout(Func<object> actual,Func<object> expected)
        {
            TrueBeforeTimeout(() => actual() == expected(),
                () => string.Format("Expected equal but was {0} vs {1}", actual(), expected()));
        }
        public void TrueBeforeTimeout(Func<bool> test, Func<bool> inconclusiveTest, Func<string> inconclusiveMessage)
        {
            AsyncAssert.TrueWithin(test, _waitTime.Remainder(), inconclusiveTest, inconclusiveMessage);
        }
    }
}