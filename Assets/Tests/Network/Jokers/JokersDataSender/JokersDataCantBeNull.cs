using Network_JokersDataSender = Network.JokersDataSender;

namespace Tests.Network.Jokers.JokersDataSender
{

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject;

    public class JokersDataCantBeNull : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager serverNetworkManager;
        
        void Start()
        {
            new Network_JokersDataSender(null, 1, this.serverNetworkManager);
        }
    }
}