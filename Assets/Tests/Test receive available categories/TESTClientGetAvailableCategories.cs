using UnityEngine;

public class TESTClientGetAvailableCategories : MonoBehaviour
{
    public ClientNetworkManager NetworkManager;
    public NotificationsServiceController NotificationsService;

    IAvailableCategoriesReader categoriesReader;

    void Start()
    {
        var threadUtils = ThreadUtils.Instance;

        NetworkManager.OnConnectedEvent += (sender, args) =>
        {
            categoriesReader = new RemoteAvailableCategoriesReader(NetworkManager, OnGetCategoriesTimeout, 5);
            categoriesReader.GetAllCategoriesAsync(OnGetAllCategories);
        };        
    }

    void OnGetCategoriesTimeout()
    {
        var errorMsg = LanguagesManager.Instance.GetValue("Errors/LoadCategoriesTimeout");
        Debug.LogWarning(errorMsg);
        NotificationsService.AddNotification(new Color(255, 255, 140), errorMsg);
    }

    void OnGetAllCategories(string[] categories)
    {
        var allCategories = string.Join(", ", categories);
        Debug.Log(allCategories);
    }
	
}
