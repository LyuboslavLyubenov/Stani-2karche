namespace Assets.Scripts.Interfaces.Network
{
    using System;

    public interface IRemoteStateActivator : IDisposable
    {
        void Bind(string id, IState stateToActivate);

        void UnBind(string id);

        void UnBind(IState state); 
    }
}