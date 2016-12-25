using UnityEngine;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Utils;

    public class DeviceIndetificatorSender : ExtendedMonoBehaviour
    {
        public ClientNetworkManager NetworkManager;

        void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, () => this.Initialize());
        }

        void Initialize()
        {
            var getDeviceIndetificator = new DummyCommand();
            getDeviceIndetificator.OnExecuted += (sender, args) => this.SendDeviceIndetificator();

            this.NetworkManager.CommandsManager.AddCommand("GetDeviceIndetificator", getDeviceIndetificator);
        }

        void SendDeviceIndetificator()
        {
            var deviceINdetificatorCommand = new NetworkCommandData("DeviceIndetificator");
            deviceINdetificatorCommand.AddOption("DeviceIndetificator", SystemInfo.deviceUniqueIdentifier);
            this.NetworkManager.SendServerCommand(deviceINdetificatorCommand);
        }
    }

}