namespace Assets.Scripts.Network
{

    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.GameData;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    public class AvailableCategoriesCommandsInitializator : ExtendedMonoBehaviour
    {
        public ServerNetworkManager NetworkManager;
        public GameDataExtractor GameData;
        public LeaderboardSerializer Leaderboard;

        void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, () =>
                {
                    this.NetworkManager.CommandsManager.AddCommand(new GetAllAvailableCategoriesCommand(this.NetworkManager));
                    this.NetworkManager.CommandsManager.AddCommand(new SelectedCategoryCommand(this.GameData, this.Leaderboard));
                });
        }

    }

}
