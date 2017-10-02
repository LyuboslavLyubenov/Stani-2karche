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
        public int Stress = 1;

        void Start()
        {
            var networkManager = ClientNetworkManager.Instance;
            networkManager.ConnectToHost("127.0.0.1");

            var dummyCommandWithoutOptions = NetworkCommandDataClass.From<DummyCommand>();
            var dummyCommandWithOptions = new NetworkCommandDataClass("DummyWithOptions");

            for (int i = 0; i < this.Stress; i++)
            {
                dummyCommandWithOptions.AddOption("Option" + i, "Value" + i);    
            }

            this.CoroutineUtils.RepeatEveryNthFrame(1, () =>
                {
                    for (int i = 0; i < this.Stress; i++)
                    {
                        networkManager.SendServerCommand(dummyCommandWithoutOptions);
                        networkManager.SendServerCommand(dummyCommandWithOptions);
                    }
                });
        }
    }
}
