using System;

using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Enums;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    public class ServerDiscoveredElementController : ExtendedMonoBehaviour
    {
        Text category;
        Text creatorName;
        Text connectedClients;

        public string ServerIPAddress
        {
            get;
            private set;
        }

        void Start()
        {
            this.category = this.transform.Find("CategoryType").GetComponent<Text>();
            this.creatorName = this.transform.Find("CreatorName").GetComponent<Text>();
            this.connectedClients = this.transform.Find("ConnectedClients").GetComponent<Text>();
        }

        public void SetData(CreatedGameInfo_Serializable gameInfo)
        {
            this.category.text = this.TranslateGameType(gameInfo.GameType);
            this.creatorName.text = gameInfo.HostUsername;
            this.connectedClients.text = gameInfo.ServerInfo.ConnectedClientsCount + "/" + gameInfo.ServerInfo.MaxConnectionsAllowed;
            this.ServerIPAddress = gameInfo.ServerInfo.LocalIPAddress;
        }

        string TranslateGameType(GameType gameType)
        {
            var enumName = Enum.GetName(typeof(GameType), gameType);
            return LanguagesManager.Instance.GetValue(enumName);
        }
    }

}
