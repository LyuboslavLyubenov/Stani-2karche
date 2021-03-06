﻿namespace Controllers
{

    using System;

    using DTOs;

    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    using Utils.Unity;

    public class SelectPlayerTypeUIController : MonoBehaviour
    {
        public Button JoinAsMainPlayerButton;
        public Button JoinAsAudiencePlayerButton;
        
        public void Initialize(EveryBodyVsTheTeacherGameInfo_DTO gameInfo_DTO)
        {
            if (gameInfo_DTO.ServerInfo.IsFull)
            {
                throw new Exception("Server is full");
            }

            if (gameInfo_DTO.IsGameStarted)
            {
                throw new Exception("Game already started");
            }
            
            this.JoinAsMainPlayerButton.gameObject.SetActive(gameInfo_DTO.CanConnectAsMainPlayer);
            this.JoinAsAudiencePlayerButton.gameObject.SetActive(gameInfo_DTO.CanConnectAsAudience);

            var serverInfo = gameInfo_DTO.ServerInfo;
            this.SaveConnectionDataForNextScene(serverInfo);
        }
        
        private void SaveConnectionDataForNextScene(ServerInfo_DTO serverInfo)
        {
            if (!string.IsNullOrEmpty(serverInfo.ExternalIpAddress))
            {
                PlayerPrefsEncryptionUtils.SetString("ServerExternalIP", serverInfo.ExternalIpAddress);
            }

            PlayerPrefsEncryptionUtils.SetString("ServerLocalIP", serverInfo.LocalIPAddress);
        }

        public void OpenHostScene()
        {
            SceneManager.LoadScene("EverybodyVsTheTeacherMainPlayer");
        }

        public void OpenGuestScene()
        {
            SceneManager.LoadScene("EverybodyVsTheTeacherAudience");
        }
    }
}