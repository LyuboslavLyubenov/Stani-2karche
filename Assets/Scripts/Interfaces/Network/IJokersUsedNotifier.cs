using JokerTypeEventArgs = EventArgs.JokerTypeEventArgs;

namespace Assets.Scripts.Interfaces.Network
{

    using System;

    using EventArgs;

    public interface IJokersUsedNotifier : IDisposable
    {
        event EventHandler<JokerTypeEventArgs> OnUsedJoker;
    }
}
