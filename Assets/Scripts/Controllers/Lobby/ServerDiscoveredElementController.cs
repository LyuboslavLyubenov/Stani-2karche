namespace Assets.Scripts.Controllers.Lobby
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Utils.Unity;

    using UnityEngine.UI;

    public class ServerDiscoveredElementController : ExtendedMonoBehaviour
    {
        private Text gameType;
        private Text creatorName;
        private Text connectedClients;

        public string ServerIPAddress
        {
            get;
            private set;
        }

        public string GameType
        {
            get
            {
                return this.gameType.text;
            }
            private set
            {
                this.gameType.text = value;
            }
        }

        public string CreatorName
        {
            get
            {
                return this.creatorName.text;
            }
            private set
            {
                this.creatorName.text = value;
            }
        }

        public string ConnectedClients
        {
            get
            {
                return this.connectedClients.text;
            }
            private set
            {
                this.connectedClients.text = value;
            }
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.gameType = this.transform.Find("CategoryType").GetComponent<Text>();
            this.creatorName = this.transform.Find("CreatorName").GetComponent<Text>();
            this.connectedClients = this.transform.Find("ConnectedClients").GetComponent<Text>();
        }

        public void SetData(CreatedGameInfo_DTO gameInfo)
        {
            var translatedGameName = LanguagesManager.Instance.GetValue(gameInfo.GameType);

            this.GameType = translatedGameName;
            this.CreatorName = gameInfo.HostUsername;
            this.ConnectedClients = gameInfo.ServerInfo.ConnectedClientsCount + "/" + gameInfo.ServerInfo.MaxConnectionsAllowed;
            this.ServerIPAddress = gameInfo.ServerInfo.LocalIPAddress;
        }
    }
}