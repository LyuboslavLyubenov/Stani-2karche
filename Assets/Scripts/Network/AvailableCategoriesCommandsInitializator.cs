namespace Assets.Scripts.Network
{
    using Commands.Server;

    using IO;
    using NetworkManagers;

    public class AvailableCategoriesCommandsInitializator
    {
        private AvailableCategoriesCommandsInitializator()
        {
        }

        public static void Initialize(ServerNetworkManager networkManager, GameDataExtractor extractor, LeaderboardSerializer leaderboard)
        {
            networkManager.CommandsManager.AddCommand(new GetAllAvailableCategoriesCommand(networkManager));
            networkManager.CommandsManager.AddCommand(new SelectedCategoryCommand(extractor, leaderboard));
        }
    }
}
