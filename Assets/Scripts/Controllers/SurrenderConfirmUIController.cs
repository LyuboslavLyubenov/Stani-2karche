namespace Assets.Scripts.Controllers
{
    using UnityEngine;

    using Commands;
    using Network.NetworkManagers;

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