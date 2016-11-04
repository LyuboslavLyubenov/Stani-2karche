﻿using UnityEngine;
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
        Server.MainPlayerData.OnDisconnected += OnMainPlayerDisconnected;
    }

    void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
    {
        connectionIdField.Value = Server.MainPlayerData.ConnectionId.ToString();
        isConnectedField.Value = LanguagesManager.Instance.GetValue("MainPlayerInfo/Yes");
    }

    void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
    {
        connectionIdField.Value = "-1";
        isConnectedField.Value = LanguagesManager.Instance.GetValue("MainPlayerInfo/No");
    }
}
