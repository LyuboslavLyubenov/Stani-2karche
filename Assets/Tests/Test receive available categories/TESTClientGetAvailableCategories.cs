using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using RemoteAvailableCategoriesReader = Network.RemoteAvailableCategoriesReader;

namespace Tests.Test_receive_available_categories
{

    using Interfaces;

    using Localization;

    using Notifications;

    using UnityEngine;

    using Utils;

    public class TESTClientGetAvailableCategories : MonoBehaviour
    {
        public ClientNetworkManager NetworkManager;
        public NotificationsController Notifications;

        private IAvailableCategoriesReader categoriesReader;

        private void Start()
        {
            var threadUtils = ThreadUtils.Instance;

            this.NetworkManager.OnConnectedEvent += (sender, args) =>
                {
                    this.categoriesReader = new RemoteAvailableCategoriesReader(this.NetworkManager, this.OnGetCategoriesTimeout, 5);
                    this.categoriesReader.GetAllCategories(this.OnGetAllCategories);
                };        
        }

        private void OnGetCategoriesTimeout()
        {
            var errorMsg = LanguagesManager.Instance.GetValue("Errors/LoadCategoriesTimeout");
            Debug.LogWarning(errorMsg);
            this.Notifications.AddNotification(new Color(255, 255, 140), errorMsg);
        }

        private void OnGetAllCategories(string[] categories)
        {
            var allCategories = string.Join(", ", categories);
            Debug.Log(allCategories);
        }
	
    }

}
