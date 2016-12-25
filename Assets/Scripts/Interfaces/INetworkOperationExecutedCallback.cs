using System;

namespace Assets.Scripts.Interfaces
{

    public interface INetworkOperationExecutedCallback
    {
        EventHandler OnExecuted
        {
            get;
            set;
        }
    }

}