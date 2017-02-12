namespace Assets.Scripts.Interfaces
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface IEverybodyVsTheTeacherServer
    {
        event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerConnected;
        event EventHandler<ClientConnectionDataEventArgs> OnAudiencePlayerConnected;
        event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerDisconnected;
        event EventHandler<ClientConnectionDataEventArgs> OnAudiencePlayerDisconnected;
    }

}