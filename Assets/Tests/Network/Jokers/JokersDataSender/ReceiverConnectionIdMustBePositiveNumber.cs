using JokersData = Network.JokersData;
using Network_JokersDataSender = Network.JokersDataSender;

namespace Tests.Network.Jokers.JokersDataSender
{

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject;

    public class ReceiverConnectionIdMustBePositiveNumber : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager serverNetworkManager;

        [Inject]
        private JokersData jokersData;

        void Start()
        {
            new Network_JokersDataSender(this.jokersData, -1, this.serverNetworkManager);
        }
    }

}