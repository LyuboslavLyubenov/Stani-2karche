namespace Controllers
{

    using Commands;

    using Network.NetworkManagers;

    using UnityEngine;

    public class SurrenderConfirmUIController : MonoBehaviour
    {
        public void Surrender()
        {
            var surrenderCommand = new NetworkCommandData("Surrender");
            ClientNetworkManager.Instance.SendServerCommand(surrenderCommand);
            this.gameObject.SetActive(false);
        }
    }
}