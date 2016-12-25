using UnityEngine;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Network;

    public class MainPlayerInfoUIController : MonoBehaviour
    {
        public BasicExamServer Server;

        FieldUIController connectionIdField;
        FieldUIController isConnectedField;

        void Start()
        {
            this.connectionIdField = this.transform.Find("ConnectionIdField").GetComponent<FieldUIController>();
            this.isConnectedField = this.transform.Find("IsConnectedField").GetComponent<FieldUIController>();

            this.Server.MainPlayerData.OnConnected += this.OnMainPlayerConnected;
            this.Server.MainPlayerData.OnDisconnected += this.OnMainPlayerDisconnected;
        }

        void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.connectionIdField.Value = this.Server.MainPlayerData.ConnectionId.ToString();
            this.isConnectedField.Value = LanguagesManager.Instance.GetValue("MainPlayerInfo/Yes");
        }

        void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.connectionIdField.Value = "-1";
            this.isConnectedField.Value = LanguagesManager.Instance.GetValue("MainPlayerInfo/No");
        }
    }

}
