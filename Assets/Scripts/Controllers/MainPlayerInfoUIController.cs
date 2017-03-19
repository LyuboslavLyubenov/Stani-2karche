namespace Controllers
{

    using EventArgs;

    using Localization;

    using Network.Servers;

    using UnityEngine;

    public class MainPlayerInfoUIController : MonoBehaviour
    {
        public BasicExamServer Server;

        private FieldUIController connectionIdField;
        private FieldUIController isConnectedField;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.connectionIdField = this.transform.Find("ConnectionIdField").GetComponent<FieldUIController>();
            this.isConnectedField = this.transform.Find("IsConnectedField").GetComponent<FieldUIController>();

            this.Server.MainPlayerData.OnConnected += this.OnMainPlayerConnected;
            this.Server.MainPlayerData.OnDisconnected += this.OnMainPlayerDisconnected;
        }

        private void OnMainPlayerConnected(object sender, ClientConnectionIdEventArgs args)
        {
            this.connectionIdField.Value = this.Server.MainPlayerData.ConnectionId.ToString();
            this.isConnectedField.Value = LanguagesManager.Instance.GetValue("MainPlayerInfo/Yes");
        }

        private void OnMainPlayerDisconnected(object sender, ClientConnectionIdEventArgs args)
        {
            this.connectionIdField.Value = "-1";
            this.isConnectedField.Value = LanguagesManager.Instance.GetValue("MainPlayerInfo/No");
        }
    }

}
