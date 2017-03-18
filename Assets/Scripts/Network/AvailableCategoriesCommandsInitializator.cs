namespace Network
{

    using Commands.Server;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    public class AvailableCategoriesCommandsInitializator
    {
        private AvailableCategoriesCommandsInitializator()
        {
        }

        public static void Initialize(IServerNetworkManager networkManager, IGameDataExtractor extractor, ILeaderboardDataManipulator leaderboard)
        {
            networkManager.CommandsManager.AddCommand(new GetAvailableCategoriesCommand(networkManager));
            networkManager.CommandsManager.AddCommand(new SelectedCategoryCommand(extractor, leaderboard));
        }
    }
}
