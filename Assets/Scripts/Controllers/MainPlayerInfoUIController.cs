using UnityEngine;
using System.Collections;

public class MainPlayerInfoUIController : MonoBehaviour
{
    public BasicExamServer Server;

    FieldUIController connectionIdField;
    FieldUIController isConnectedField;

    void Start()
    {
        connectionIdField = transform.Find("ConnectionIdField").GetComponent<FieldUIController>();
        isConnectedField = transform.Find("IsConnectedField").GetComponent<FieldUIController>();

        Server.MainPlayerData.OnConnected += OnMainPlayerConnected;
        Server.MainPlayerData.OnDisconnected += OnMainPlayerConnected;
    }

    void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
    {
        connectionIdField.Value = Server.MainPlayerData.ConnectionId.ToString();
        isConnectedField.Value = LanguagesManager.Instance.GetValue("Connected");
    }

    void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
    {
        connectionIdField.Value = "";
        isConnectedField.Value = LanguagesManager.Instance.GetValue("Disconnected");
    }
}
