namespace AsyncAssert
{
    public interface IAsyncAssertLogger
    {
        void Trace(string msg, params object[] @params);

        void Error(string msg, params object[] @params);
    }
}