using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using CielaSpike;
using System.Net;
using System;
using System.Text;

public class GameInfoReceivedData
{
    public GameInfoReceivedData(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentNullException("json");
        }

        this.GameInfo = JsonUtility.FromJson<CreatedGameInfo_Serializable>(json);
    }

    public CreatedGameInfo_Serializable GameInfo
    {
        get;
        private set;
    }
}
