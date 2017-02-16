namespace Assets.Scripts.Controllers
{
    using UnityEngine.UI;

    using DTOs;

    using Localization;
    using Utils.Unity;

    public class ServerDiscoveredElementController : ExtendedMonoBehaviour
    {
        private Text category;
        private Text creatorName;
        private Text connectedClients;

        public string ServerIPAddress
        {
            get;
            private set;
        }

        public string Category
        {
            get
            {
                return this.category.text;
            }
            private set
            {
                this.category.text = value;
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
            this.category = this.transform.Find("CategoryType").GetComponent<Text>();
            this.creatorName = this.transform.Find("CreatorName").GetComponent<Text>();
            this.connectedClients = this.transform.Find("ConnectedClients").GetComponent<Text>();
        }

        public void SetData(CreatedGameInfo_DTO gameInfo)
        {
            var translatedGameName = LanguagesManager.Instance.GetValue(gameInfo.GameType);

            this.Category = translatedGameName;
            this.CreatorName = gameInfo.HostUsername;
            this.ConnectedClients = gameInfo.ServerInfo.ConnectedClientsCount + "/" + gameInfo.ServerInfo.MaxConnectionsAllowed;
            this.ServerIPAddress = gameInfo.ServerInfo.LocalIPAddress;
        }
    }
}