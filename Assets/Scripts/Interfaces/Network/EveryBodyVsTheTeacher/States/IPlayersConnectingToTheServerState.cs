namespace Interfaces.Network.EveryBodyVsTheTeacher.States
{

    using System;
    using System.Collections.ObjectModel;

    using EventArgs;

    public interface IPlayersConnectingToTheServerState : IState
    {
        ReadOnlyCollection<int> MainPlayersConnectionIds
        {
            get;
        }

        ReadOnlyCollection<int> AudiencePlayersConnectionIds
        {
            get;
        }

        event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerConnected;
        event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerDisconnected;

        event EventHandler OnEveryBodyRequestedGameStart;

        event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerRequestedGameStart;

        event EventHandler<ClientConnectionDataEventArgs> OnAudiencePlayerConnected;
        event EventHandler<ClientConnectionDataEventArgs> OnAudiencePlayerDisconnected;
    }
}
