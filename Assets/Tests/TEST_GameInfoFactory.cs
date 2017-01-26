using Assets.Scripts;
using Assets.Scripts.Network.NetworkManagers;

using UnityEngine;

namespace Assets.Tests
{

    using Assets.Scripts.Network.Servers;

    public class TEST_GameInfoFactory : MonoBehaviour
    {

        public ServerNetworkManager NetworkManager;

        public BasicExamServer Server;

        // Use this for initialization
        void Start ()
        {
            var gameInfo = GameInfoFactory.Instance.Get(this.NetworkManager, this.Server);
            Debug.LogFormat("{0} {1} {2}", gameInfo.HostUsername, gameInfo.ServerInfo.LocalIPAddress, gameInfo.ServerInfo.ExternalIpAddress);
        }
    }

}
