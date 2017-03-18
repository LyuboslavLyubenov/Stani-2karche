namespace Zenject.Source.Util
{

    using System;

    [System.Diagnostics.DebuggerStepThrough]
    public class ZenjectException : Exception
    {
        public ZenjectException(string message)
            : base(message)
        {
        }

        public ZenjectException(
            string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
