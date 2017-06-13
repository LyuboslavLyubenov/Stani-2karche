namespace Interfaces.Network
{

    using System.Collections.Generic;

    public interface IEveryBodyVsTheTeacherServer : IGameServer
    {
        IEnumerable<int> ConnectedMainPlayersConnectionIds
        {
            get;
        }

        bool StartedGame
        {
            get;
        }

        int PresenterId { get; }
    }
}
