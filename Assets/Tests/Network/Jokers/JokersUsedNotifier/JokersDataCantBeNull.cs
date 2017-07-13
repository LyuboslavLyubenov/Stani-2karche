using Network_JokersUsedNotifier = Assets.Scripts.Network.JokersUsedNotifier;

namespace Tests.Network.Jokers.JokersUsedNotifier
{

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject;

    public class JokersDataCantBeNull : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            new Network_JokersUsedNotifier(this.networkManager, null);
        }
    }

}