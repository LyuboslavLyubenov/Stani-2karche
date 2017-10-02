using System;
using UnityEngine;
using Network.NetworkManagers;
using Commands;
using Utils.Unity;
using NetworkCommandDataClass = Commands.NetworkCommandData;

namespace Tests.Network.NetworkCommandData
{

    public class ClientStressor : ExtendedMonoBehaviour
    {
        void Start()
        {
            var networkManager = ClientNetworkManager.Instance;
            networkManager.ConnectToHost("127.0.0.1");

            this.StartStressing();

            networkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
        }

        void OnDisconnectedFromServer(object sender, System.EventArgs args)
        {
            ClientNetworkManager.Instance.ConnectToHost("127.0.0.1");
        }

        private void StartStressing()
        {
            var dummyCommandWithoutOptions = NetworkCommandDataClass.From<DummyCommand>();
            var dummyCommandWithOptions = new NetworkCommandDataClass("DummyWithOptions");

            for (int i = 0; i < 10; i++)
            {
                dummyCommandWithOptions.AddOption("Option" + i, "Value" + i);    
            }

            this.CoroutineUtils.RepeatEveryNthFrame(10, () =>
                {
                    var networkManager = ClientNetworkManager.Instance;

                    if (!networkManager.IsConnected)
                    {
                        return;
                    }

                    ClientNetworkManager.Instance.SendServerCommand(dummyCommandWithoutOptions);
                    ClientNetworkManager.Instance.SendServerCommand(dummyCommandWithOptions);
                 
                });
        }
    }
}