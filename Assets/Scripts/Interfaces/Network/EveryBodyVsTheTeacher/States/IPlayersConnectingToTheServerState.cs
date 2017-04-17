namespace Interfaces.Network.EveryBodyVsTheTeacher.States
{

    using System;
    using System.Collections.ObjectModel;

    using Assets.Scripts.Interfaces;

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

        event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerConnected;
        event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerDisconnected;

        event EventHandler OnEveryBodyRequestedGameStart;

        event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerRequestedGameStart;

        event EventHandler<ClientConnectionIdEventArgs> OnAudiencePlayerConnected;
        event EventHandler<ClientConnectionIdEventArgs> OnAudiencePlayerDisconnected;
    }
}
