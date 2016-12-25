namespace Assets.Scripts.EventArgs
{

    using EventArgs = System.EventArgs;

    public class PlayerCalledEventArgs : EventArgs
    {
        public PlayerCalledEventArgs(int playerConnectionId)
        {
            this.PlayerConnectionId = playerConnectionId;
        }

        public int PlayerConnectionId
        {
            get;
            private set;
        }

    }

}
