namespace Controllers
{

    using System;

    using DTOs;

    using Localization;

    using Network.NetworkManagers;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class BasicExamClientOptionsUIController : MonoBehaviour
    {
        private FieldUIController connectionIdField;
        private FieldUIController usernameField;
        private FieldUIController roleField;

        private InputField reasonInputField;

        void Start()
        {
            this.connectionIdField = this.transform.Find("ConnectionIdField").GetComponent<FieldUIController>();
            this.usernameField = this.transform.Find("UsernameField").GetComponent<FieldUIController>();
            this.roleField = this.transform.Find("RoleField").GetComponent<FieldUIController>();
            this.reasonInputField = this.transform.Find("KickBanGroup/ReasonInputField").GetComponent<InputField>();

            var kickButton = this.transform.Find("KickBanGroup/KickButton").GetComponent<Button>();
            var banButton = this.transform.Find("KickBanGroup/BanButton").GetComponent<Button>();

            kickButton.onClick.AddListener(new UnityAction(this.OnKick));
            banButton.onClick.AddListener(new UnityAction(this.OnBan));
        }

        private void OnKick()
        {
            var connectionId = int.Parse(this.connectionIdField.Value);
            var reason = this.reasonInputField.text;

            if (string.IsNullOrEmpty(reason))
            {
                ServerNetworkManager.Instance.KickPlayer(connectionId);    
            }
            else
            {
                ServerNetworkManager.Instance.KickPlayer(connectionId, reason);    
            }

            this.gameObject.SetActive(false);
        }

        private void OnBan()
        {
            var connectionId = int.Parse(this.connectionIdField.Value);
            ServerNetworkManager.Instance.BanPlayer(connectionId);

            this.gameObject.SetActive(false);
        }

        public void Set(ConnectedClientData clientData, BasicExamClientRole role)
        {
            var enumName = Enum.GetName(typeof(BasicExamClientRole), role);

            this.connectionIdField.Value = clientData.ConnectionId.ToString();
            this.usernameField.Value = clientData.Username;
            this.roleField.Value = LanguagesManager.Instance.GetValue(enumName);
        }
    }

}