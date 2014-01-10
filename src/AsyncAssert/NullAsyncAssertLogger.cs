namespace AsyncAssert
{
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