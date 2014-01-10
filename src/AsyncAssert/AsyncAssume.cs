namespace AsyncAssert
{
    using System;

    public static class AsyncAssume
    {
        public static void TrueWithin(Func<bool> function, TimeSpan within, TimeSpan? interval = null,
                                      Func<string> getFailMsg = null)
        {
            AsyncAssert.TrueWithin(function,within,interval,getFailMsg);
        }
    }
}