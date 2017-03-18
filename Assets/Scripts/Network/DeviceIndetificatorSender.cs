namespace Network
{

    using System;

    using Commands;

    using Network.NetworkManagers;

    using UnityEngine;

    public class DeviceIndetificatorSender
    {
        private ClientNetworkManager networkManager;

        public DeviceIndetificatorSender(ClientNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
            this.Initialize();
        }

        private void Initialize()
        {
            var getDeviceIndetificator = new DummyCommand();
            getDeviceIndetificator.OnExecuted += (sender, args) => this.SendDeviceIndetificator();

            this.networkManager.CommandsManager.AddCommand("GetDeviceIndetificator", getDeviceIndetificator);
        }

        private void SendDeviceIndetificator()
        {
            var deviceINdetificatorCommand = new NetworkCommandData("DeviceIndetificator");
            deviceINdetificatorCommand.AddOption("DeviceIndetificator", SystemInfo.deviceUniqueIdentifier);
            this.networkManager.SendServerCommand(deviceINdetificatorCommand);
        }
    }

}