namespace Network.TcpSockets
{

    using System;

    public class AsyncOperationStatusCallbacks
    {
        public Action OnSuccess;
        public Action<Exception> OnError;
    }

}