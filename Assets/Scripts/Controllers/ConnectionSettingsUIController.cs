using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.EventArgs;

    public class ConnectionSettingsUIController : MonoBehaviour
    {
        public Text IPText;

        public EventHandler<IpEventArgs> OnConnectToServer = delegate
            {
            };

        void Start()
        {
            if (this.IPText == null)
            {
                throw new NullReferenceException("IPText is null on ConnectionSettingsUIController obj");
            }
        }

        public void ConnectToServer()
        {
            var ipEventArgs = new IpEventArgs(this.IPText.text);
            this.OnConnectToServer(this, ipEventArgs);
        }
    }

}
