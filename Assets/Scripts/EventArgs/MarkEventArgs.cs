using System;


[Serializable]
public sealed class MarkEventArgs : EventArgs
{
    public MarkEventArgs(int mark, int nextMark)
    {
        this.Mark = mark;
        this.NextMark = nextMark;
    }

    public int Mark
    {
        get;
        private set;
    }

    public int NextMark
    {
        get;
        private set;
    }
}