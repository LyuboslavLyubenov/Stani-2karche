using UnityEngine;
using UnityEngine.UI;
using System;

public class ConnectedClientDataElementUIController : MonoBehaviour
{
    Text connectionIdText;
    Text usernameText;

    public int ConnectionId
    {
        get
        {
            return int.Parse(connectionIdText.text);
        }
    }

    public string Username
    {
        get
        {
            return usernameText.text;
        }
    }

    void Start()
    {
        var connectionIdTextObj = transform.FindChild("ConnectionId");
        var usernameTextObj = transform.FindChild("Username");

        connectionIdText = connectionIdTextObj.GetComponent<Text>();
        usernameText = usernameTextObj.GetComponent<Text>();
    }

    public void Fill(ConnectedClientData clientData)
    {
        if (clientData == null)
        {
            throw new ArgumentNullException("clientData");
        }

        connectionIdText.text = clientData.ConnectionId.ToString();
        usernameText.text = clientData.Username;
    }

    void GetRealUsername()
    {
        
    }
}