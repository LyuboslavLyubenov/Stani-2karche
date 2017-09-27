using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Commands.Client
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public class SendDeviceIdToServerCommand : INetworkManagerCommand
    {
        private readonly IClientNetworkManager networkManager;

        public SendDeviceIdToServerCommand(IClientNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var deviceId = "";

#if DEVELOPMENT_BUILD || UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_LINUX
            deviceId = Guid.NewGuid().ToString();
#else
            deviceId = SystemInfo.deviceUniqueIdentifier;
#endif

            var command = NetworkCommandData.From<SetClientIdCommand>();
            command.AddOption("DeviceId", deviceId);
            this.networkManager.SendServerCommand(command);
        }
    }
}
