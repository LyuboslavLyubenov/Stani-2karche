using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConnectionSettingsUIController : MonoBehaviour
{
    public GameObject IPTextObj;

    Text ipText = null;

    AndroidUIController androidUIController = null;

    // Use this for initialization
    void Start()
    {
        ipText = IPTextObj.GetComponent<Text>();
        androidUIController = GameObject.FindWithTag("MainCamera").GetComponent<AndroidUIController>();
    }

    public void ConnectToServer()
    {
        androidUIController.Connect(ipText.text);
    }
}
