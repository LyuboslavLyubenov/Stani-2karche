using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConnectionSettingsUIController : MonoBehaviour
{
    public GameObject IPTextObj;
    public AndroidUIController androidUIController;

    Text ipText = null;

    void Start()
    {
        ipText = IPTextObj.GetComponent<Text>();
    }

    public void ConnectToServer()
    {
        androidUIController.Connect(ipText.text);
    }
}
