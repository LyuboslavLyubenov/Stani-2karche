namespace Assets.Tests.Network.RemoteStateActivator
{

    using Assets.Scripts.Interfaces.Network;

    using UnityEngine;

    using Zenject.Source.Usage;

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