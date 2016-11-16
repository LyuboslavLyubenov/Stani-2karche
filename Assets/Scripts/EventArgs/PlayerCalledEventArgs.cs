using System;

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
