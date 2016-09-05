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

        HostButton.gameObject.SetActive(false);
        GuestButton.gameObject.SetActive(false);

        if (gameInfo.CanConnectAsAudience)
        {
            HostButton.gameObject.SetActive(true);
        }

        if (gameInfo.CanConnectAsMainPlayer)
        {
            GuestButton.gameObject.SetActive(true);
        }

        var serverInfo = gameInfo.ServerInfo;
        var serverIP = (serverInfo.ExternalIpAddress == string.Empty) ? serverInfo.LocalIPAddress : serverInfo.ExternalIpAddress;
        PlayerPrefs.SetString("ServerIP", serverIP);
    }

    public void OpenHostScene()
    {
        if (!PlayerPrefs.HasKey("ServerIP"))
        {
            throw new Exception("Not set serverIP");
        }

        PlayerPrefs.SetString("BasicExamConnectionType", "Player");
        StartGame();
    }

    public void OpenGuestScene()
    {
        if (!PlayerPrefs.HasKey("ServerIP"))
        {
            throw new Exception("Not set serverIP");
        }

        PlayerPrefs.SetString("BasicExamConnectionType", "Audience");
        StartGame();
    }

    void StartGame()
    {
        SceneManager.LoadScene("BasicExam");
    }
}