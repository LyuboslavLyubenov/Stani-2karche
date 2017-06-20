namespace Interfaces.Network
{
    using System.Collections.Generic;

    public interface IEveryBodyVsTheTeacherServer : IGameServer
    {
        IEnumerable<int> ConnectedMainPlayersConnectionIds
        {
            get;
        }

        IEnumerable<int> MainPlayersConnectionIds
        {
            get;
        }

        IEnumerable<int> SurrenderedMainPlayersConnectionIds
        {
            get;
        }

        bool StartedGame
        {
            get;
        }

        int PresenterId
        {
            get;
        }

        void AddMainPlayerToSurrenderList(int connectionId);
    }
}