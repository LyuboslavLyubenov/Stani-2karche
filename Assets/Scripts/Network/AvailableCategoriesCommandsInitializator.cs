namespace Assets.Scripts.Network
{

    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.Utils;

    public class AvailableCategoriesCommandsInitializator : ExtendedMonoBehaviour
    {
        public ServerNetworkManager NetworkManager;
        public LocalGameData GameData;
        public LeaderboardSerializer Leaderboard;

        void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, () =>
                {
                    this.NetworkManager.CommandsManager.AddCommand("GetAvailableCategories", new ReceivedServerGetAllAvailableCategoriesCommand(this.NetworkManager));
                    this.NetworkManager.CommandsManager.AddCommand("SelectedCategory", new ReceivedServerSelectedCategoryCommand(this.GameData, this.Leaderboard));
                });
        }

    }

}
