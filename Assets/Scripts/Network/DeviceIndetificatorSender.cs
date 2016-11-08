using UnityEngine;
using System.Collections;

public class DeviceIndetificatorSender : ExtendedMonoBehaviour
{
    public ClientNetworkManager NetworkManager;

    void Start()
    {
        CoroutineUtils.WaitForFrames(0, () => Initialize());
    }

    void Initialize()
    {
        var getDeviceIndetificator = new DummyCommand();
        getDeviceIndetificator.OnExecuted += (sender, args) => SendDeviceIndetificator();

        NetworkManager.CommandsManager.AddCommand("GetDeviceIndetificator", getDeviceIndetificator);
    }

    void SendDeviceIndetificator()
    {
        var deviceINdetificatorCommand = new NetworkCommandData("DeviceIndetificator");
        deviceINdetificatorCommand.AddOption("DeviceIndetificator", SystemInfo.deviceUniqueIdentifier);
        NetworkManager.SendServerCommand(deviceINdetificatorCommand);
    }
}