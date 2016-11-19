using UnityEngine;
using System;

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
