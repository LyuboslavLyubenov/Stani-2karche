using JokersData = Network.JokersData;
using Network_JokersDataSender = Network.JokersDataSender;

namespace Tests.Network.Jokers.JokersDataSender
{

    using UnityEngine;

    using Zenject.Source.Usage;

    public class ServerNetworkManagerCantBeNull : MonoBehaviour
    {
        [Inject]
        private JokersData jokersData;

        void Start()
        {
            new Network_JokersDataSender(this.jokersData, 1, null);
        }
    }

}