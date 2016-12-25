namespace Assets.Scripts.EventArgs
{

    using EventArgs = System.EventArgs;

    public class DataSentEventArgs : EventArgs
    {

        public DataSentEventArgs(int connectionId, string username, string message)
        {
            this.ConnectionId = connectionId;
            this.Username = username;
            this.Message = message;
        }

        public int ConnectionId
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }
    }

}
