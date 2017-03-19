using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Interfaces.Controllers
{
    public interface IUnableToConnectUIController
    {
        string ServerIP { get; set; }

        void TryAgainToConnectToServer(IClientNetworkManager networkManager);

        void ReturnToLobby();
    }
}