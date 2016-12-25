using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    public class ConnectedClientDataElementUIController : MonoBehaviour
    {
        Text connectionIdText;
        Text usernameText;

        public int ConnectionId
        {
            get
            {
                return int.Parse(this.connectionIdText.text);
            }
        }

        public string Username
        {
            get
            {
                return this.usernameText.text;
            }
        }

        void Start()
        {
            var connectionIdTextObj = this.transform.Find("ConnectionId");
            var usernameTextObj = this.transform.Find("Username");

            this.connectionIdText = connectionIdTextObj.GetComponent<Text>();
            this.usernameText = usernameTextObj.GetComponent<Text>();
        }

        public void Fill(ConnectedClientData clientData)
        {
            if (clientData == null)
            {
                throw new ArgumentNullException("clientData");
            }

            this.connectionIdText.text = clientData.ConnectionId.ToString();
            this.usernameText.text = clientData.Username;
        }
    }

}