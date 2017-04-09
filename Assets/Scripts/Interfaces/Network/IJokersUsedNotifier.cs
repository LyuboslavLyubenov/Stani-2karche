using JokerTypeEventArgs = EventArgs.JokerTypeEventArgs;

namespace Interfaces.Network
{

    using System;

    public interface IJokersUsedNotifier : IDisposable
    {
        event EventHandler<JokerTypeEventArgs> OnUsedJoker;
    }
}
