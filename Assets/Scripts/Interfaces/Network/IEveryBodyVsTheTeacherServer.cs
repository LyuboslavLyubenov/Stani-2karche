namespace Interfaces.Network
{

    using System.Collections.Generic;

    public interface IEveryBodyVsTheTeacherServer : IGameServer
    {
        IEnumerable<int> MainPlayersConnectionIds
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
