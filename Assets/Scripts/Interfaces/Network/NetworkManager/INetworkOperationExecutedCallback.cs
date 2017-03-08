namespace Assets.Scripts.Interfaces.Network.NetworkManager
{

    using System;

    public interface INetworkOperationExecutedCallback
    {
        EventHandler OnExecuted
        {
            get;
            set;
        }
    }

}