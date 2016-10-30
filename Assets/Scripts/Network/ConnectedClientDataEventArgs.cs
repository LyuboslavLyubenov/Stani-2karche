using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class ConnectedClientDataEventArgs : EventArgs
{
    public ConnectedClientData ClientData 
    {
        get; 
        set; 
    }

    public ConnectedClientDataEventArgs(ConnectedClientData clientData)
    {
        if (clientData == null)
        {
            throw new ArgumentNullException("clientData");
        }
            
        this.ClientData = clientData;
    }
}