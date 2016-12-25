using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Localization;
    using Assets.Scripts.Network;

    public class BasicExamClientOptionsUIController : MonoBehaviour
    {
        public ServerNetworkManager NetworkManager;

        FieldUIController connectionIdField;
        FieldUIController usernameField;
        FieldUIController roleField;
        InputField reasonInputField;

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

        void OnKick()
        {
            var connectionId = int.Parse(this.connectionIdField.Value);
            var reason = this.reasonInputField.text;

            if (string.IsNullOrEmpty(reason))
            {
                this.NetworkManager.KickPlayer(connectionId);    
            }
            else
            {
                this.NetworkManager.KickPlayer(connectionId, reason);    
            }

            this.gameObject.SetActive(false);
        }

        void OnBan()
        {
            var connectionId = int.Parse(this.connectionIdField.Value);
            this.NetworkManager.BanPlayer(connectionId);

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