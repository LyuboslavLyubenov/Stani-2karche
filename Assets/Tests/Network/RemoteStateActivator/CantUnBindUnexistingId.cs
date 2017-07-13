namespace Assets.Tests.Network.RemoteStateActivator
{

    using Assets.Scripts.Interfaces.Network;

    using UnityEngine;

    using Zenject;

    public class CantUnBindUnexistingId : MonoBehaviour
    {
        [Inject]
        private IRemoteStateActivator stateActivator;

        void Start()
        {
            this.stateActivator.UnBind("UnexistingId");
        }
    }
}