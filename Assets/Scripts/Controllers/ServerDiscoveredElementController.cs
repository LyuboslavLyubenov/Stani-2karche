namespace Assets.Scripts.Controllers
{

    using System.Linq;

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

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.category = this.transform.Find("CategoryType").GetComponent<Text>();
            this.creatorName = this.transform.Find("CreatorName").GetComponent<Text>();
            this.connectedClients = this.transform.Find("ConnectedClients").GetComponent<Text>();
        }

        public void SetData(CreatedGameInfo_DTO gameInfo)
        {
            var serverGameName = gameInfo.GameTypeFullName.Split('.').Last();
            var translatedGameName = LanguagesManager.Instance.GetValue(serverGameName);

            this.category.text = translatedGameName;
            this.creatorName.text = gameInfo.HostUsername;
            this.connectedClients.text = gameInfo.ServerInfo.ConnectedClientsCount + "/" + gameInfo.ServerInfo.MaxConnectionsAllowed;
            this.ServerIPAddress = gameInfo.ServerInfo.LocalIPAddress;
        }
    }

}
