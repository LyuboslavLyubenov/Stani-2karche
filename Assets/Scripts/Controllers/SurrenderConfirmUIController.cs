using UnityEngine;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

    public class SurrenderConfirmUIController : MonoBehaviour
    {
        public ClientNetworkManager NetworkManager;

        public void Surrender()
        {
            var surrenderCommand = new NetworkCommandData("Surrender");
            this.NetworkManager.SendServerCommand(surrenderCommand);
            this.gameObject.SetActive(false);
        }
    }

}
