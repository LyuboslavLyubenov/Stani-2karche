using UnityEngine;
using System.Collections;

public class SurrenderConfirmUIController : MonoBehaviour
{
    public ClientNetworkManager NetworkManager;

    public void Surrender()
    {
        var surrenderCommand = new NetworkCommandData("Surrender");
        NetworkManager.SendServerCommand(surrenderCommand);
        gameObject.SetActive(false);
    }
}
