namespace EventArgs
{

    using System;

    public class MessageEventArgs : IpEventArgs
    {
        public MessageEventArgs(string ip, string message)
            : base(ip)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            this.Message = message;
        }

        public string Message
        {
            get;
            private set;
        }
    }

}
