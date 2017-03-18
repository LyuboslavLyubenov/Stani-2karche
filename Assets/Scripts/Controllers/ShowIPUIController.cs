namespace Controllers
{

    using System;

    using Localization;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    public class ShowIPUIController : MonoBehaviour
    {
        public Text IPText;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.IPText.text = NetworkUtils.GetLocalIP(); 
            NetworkUtils.GetExternalIP(this.OnFoundIP, this.OnNetworkError);
        }

        private void OnFoundIP(string ip)
        {
            this.IPText.text += Environment.NewLine;
            this.IPText.text += ip.Trim();
        }

        private void OnNetworkError(string errorMsg)
        {
            this.IPText.text = LanguagesManager.Instance.GetValue("Errors/NoInternetConnection");
        }
    }

}
