namespace Assets.Scripts.Network
{
    using System;

    using NetworkManagers;

    using UnityEngine;

    using Commands;

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