namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher
{
    using System;

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Utils.Unity;

    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class ServerSelectInfoUIController : MonoBehaviour
    {
        public Button JoinAsMainPlayerButton;
        public Button JoinAsAudiencePlayerButton;
        
        public void Initialize(EverybodyVsTheTeacherGameInfo_DTO gameInfo_DTO)
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
            SaveConnectionDataForNextScene(serverInfo);
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