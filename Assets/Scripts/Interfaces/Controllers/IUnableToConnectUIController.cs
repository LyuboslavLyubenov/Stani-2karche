using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Interfaces.Controllers
{
    public interface IUnableToConnectUIController
    {
        string ServerIP { get; set; }

        void TryAgainToConnectToServer(IClientNetworkManager networkManager);

        void ReturnToLobby();
    }
}