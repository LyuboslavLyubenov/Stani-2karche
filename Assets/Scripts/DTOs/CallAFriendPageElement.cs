using System;

namespace Assets.Scripts
{

    /// <summary>
    /// Used for individual cell of the phone (Call A friend joker) 
    /// </summary>
    public class CallFriendPageElement
    {
        public CallFriendPageElement(int connectionId, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.ConnectionId = connectionId;
            this.Name = name;
        }

        public int ConnectionId
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }
    }

}