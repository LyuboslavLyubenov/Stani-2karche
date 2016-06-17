using System;

public class PlayerCalledEventArgs : EventArgs
{
    public PlayerCalledEventArgs(int playerConnectionId)
    {
        if (playerConnectionId < 0)
        {
            throw new ArgumentOutOfRangeException("playerConnectionId");
        }

        this.PlayerConnectionId = playerConnectionId;
    }

    public int PlayerConnectionId
    {
        get;
        private set;
    }

}
