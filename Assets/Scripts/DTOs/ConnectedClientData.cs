namespace DTOs
{

    using System;

    public class ConnectedClientData
    {
        private const int MinUsernameLength = 1;

        private int connectionId;

        private string username;

        public int ConnectionId
        {
            get
            {
                return this.connectionId;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }    

                this.connectionId = value;
            }
        }

        public string Username
        {
            get
            {
                return this.username;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < MinUsernameLength)
                {
                    throw new ArgumentException("Username must be at least " + MinUsernameLength + " symbols long.");
                }

                this.username = value;
            }
        }

        public ConnectedClientData(int connectionId, string username)
        {
            this.ConnectionId = connectionId;
            this.Username = username;
        }

        public bool Equals(ConnectedClientData data)
        {
            return (this.ConnectionId == data.ConnectionId && this.Username == data.Username);
        }
    }

}
