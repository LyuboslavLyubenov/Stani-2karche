using UnityEngine;

namespace Assets.Tests.Test_receive_available_categories
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils;

    public class TESTClientGetAvailableCategories : MonoBehaviour
    {
        public ClientNetworkManager NetworkManager;
        public NotificationsServiceController NotificationsService;

        IAvailableCategoriesReader categoriesReader;

        void Start()
        {
            var threadUtils = ThreadUtils.Instance;

            this.NetworkManager.OnConnectedEvent += (sender, args) =>
                {
                    this.categoriesReader = new RemoteAvailableCategoriesReader(this.NetworkManager, this.OnGetCategoriesTimeout, 5);
                    this.categoriesReader.GetAllCategories(this.OnGetAllCategories);
                };        
        }

        void OnGetCategoriesTimeout()
        {
            var errorMsg = LanguagesManager.Instance.GetValue("Errors/LoadCategoriesTimeout");
            Debug.LogWarning(errorMsg);
            this.NotificationsService.AddNotification(new Color(255, 255, 140), errorMsg);
        }

        void OnGetAllCategories(string[] categories)
        {
            var allCategories = string.Join(", ", categories);
            Debug.Log(allCategories);
        }
	
    }

}
