﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class BasicExamServerSelectPlayerTypeUIController : MonoBehaviour
{
    public Button HostButton;
    public Button GuestButton;

    public void Initialize(BasicExamGameInfo_Serializable gameInfo)
    {
        if (gameInfo.ServerInfo.IsFull)
        {
            throw new Exception("Server is full");
        }

        HostButton.gameObject.SetActive(gameInfo.CanConnectAsMainPlayer);
        GuestButton.gameObject.SetActive(gameInfo.CanConnectAsAudience);

        var serverInfo = gameInfo.ServerInfo;

        if (!string.IsNullOrEmpty(serverInfo.ExternalIpAddress))
        {
            PlayerPrefsEncryptionUtils.SetString("ServerExternalIP", serverInfo.ExternalIpAddress);    
        }

        PlayerPrefsEncryptionUtils.SetString("ServerLocalIP", serverInfo.LocalIPAddress);
    }

    public void OpenHostScene()
    {
        SceneManager.LoadScene("BasicExamMainPlayer");
    }

    public void OpenGuestScene()
    {
        SceneManager.LoadScene("BasicExamAudience");
    }
}
