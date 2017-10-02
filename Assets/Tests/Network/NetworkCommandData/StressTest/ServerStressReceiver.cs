using System;
using UnityEngine;
using Network.NetworkManagers;
using Commands;
using Utils.Unity;
using NetworkCommandDataClass = Commands.NetworkCommandData;

namespace Tests.Network.NetworkCommandData
{
    public class ServerStressReceiver : ExtendedMonoBehaviour
    {
        void Start()
        {
            this.AllocateMoreMemory();

            this.CoroutineUtils.RepeatEverySeconds(10, System.GC.Collect);

            var networkManager = ServerNetworkManager.Instance;
            networkManager.CommandsManager.AddCommand(new DummyCommand());
            networkManager.CommandsManager.AddCommand("DummyWithOptions", new DummyCommand());
        }

        private void AllocateMoreMemory()
        {
            var tmp = new System.Object[1024 * 512];

            // make allocations in smaller blocks to avoid them to be treated in a special way, which is designed for large blocks
            for (int i = 0; i < tmp.Length; i++)
                tmp[i] = new byte[1024];

            // release reference
            tmp = null;
        }
    }

}
   