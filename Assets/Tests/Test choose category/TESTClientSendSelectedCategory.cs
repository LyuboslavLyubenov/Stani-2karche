using UnityEngine;

public class TESTClientSendSelectedCategory : ExtendedMonoBehaviour
{
    public ClientNetworkManager NetworkManager;
    public ClientChooseCategoryUIController ChooseCategoryUIController;

    void Start()
    {
        CoroutineUtils.WaitForFrames(0, Initialize);
    }

    void Initialize()
    {
        NetworkManager.OnConnectedEvent += (sender, args) =>
        {
            ChooseCategoryUIController.gameObject.SetActive(true);
            ChooseCategoryUIController.OnChoosedCategory += (s, a) =>
            {
                var selectedCategoryCommand = new NetworkCommandData("SelectedCategory");
                selectedCategoryCommand.AddOption("Category", a.Name);
                NetworkManager.SendServerCommand(selectedCategoryCommand);
            };
        };
    }

    void OnTimeout()
    {
        
    }
}
