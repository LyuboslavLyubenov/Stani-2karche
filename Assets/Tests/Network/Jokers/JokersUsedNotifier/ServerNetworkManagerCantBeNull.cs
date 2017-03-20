using JokersData = Network.JokersData;
using Network_JokersUsedNotifier = Network.JokersUsedNotifier;

namespace Tests.Network.Jokers.JokersUsedNotifier
{

    using UnityEngine;

    public class ServerNetworkManagerCantBeNull : MonoBehaviour
    {
        void Start()
        {
            new Network_JokersUsedNotifier(null, new JokersData());
        }
    }

}