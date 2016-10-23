using System;

public class JokerTypeEventArgs : EventArgs
{
    public Type JokerType
    {
        get;
        private set;
    }

    public JokerTypeEventArgs(Type jokerType)
    {
        JokerUtils.ValidateJokerType(jokerType);
        this.JokerType = jokerType;
    }
}