namespace Assets.Scripts.Controllers.Lobby
{

    using System;

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Utils.Unity;

    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class BasicExamServerSelectPlayerTypeUIController : MonoBehaviour
    {
        public Button HostButton;
        public Button GuestButton;

        public void Initialize(BasicExamGameInfo_DTO gameInfo_DTO)
        {
            if (gameInfo_DTO.ServerInfo.IsFull)
            {
                throw new Exception("Server is full");
            }

            this.HostButton.gameObject.SetActive(gameInfo_DTO.CanConnectAsMainPlayer);
            this.GuestButton.gameObject.SetActive(gameInfo_DTO.CanConnectAsAudience);

            var serverInfo = gameInfo_DTO.ServerInfo;
            
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

}