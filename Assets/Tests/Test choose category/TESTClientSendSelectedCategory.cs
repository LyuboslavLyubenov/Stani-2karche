namespace Assets.Tests.Test_choose_category
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Controllers;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.Utils.Unity;

    public class TESTClientSendSelectedCategory : ExtendedMonoBehaviour
    {
        public ClientNetworkManager NetworkManager;
        public ClientChooseCategoryUIController ChooseCategoryUIController;

        private void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, this.Initialize);
        }

        private void Initialize()
        {
            this.NetworkManager.OnConnectedEvent += (sender, args) =>
                {
                    this.ChooseCategoryUIController.gameObject.SetActive(true);
                    this.ChooseCategoryUIController.OnChoosedCategory += (s, a) =>
                        {
                            var selectedCategoryCommand = new NetworkCommandData("SelectedCategory");
                            selectedCategoryCommand.AddOption("Category", a.Name);
                            this.NetworkManager.SendServerCommand(selectedCategoryCommand);
                        };
                };
        }

        private void OnTimeout()
        {
        
        }
    }

}
