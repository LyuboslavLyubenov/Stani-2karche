namespace Network
{

    using Commands.Server;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    public class AvailableCategoriesCommandsInitializer
    {
        private AvailableCategoriesCommandsInitializer()
        {
        }

        public static void Initialize(
            IServerNetworkManager networkManager, 
            IGameDataExtractor extractor, 
            ILeaderboardDataManipulator leaderboard,
            IAvailableCategoriesReader availableCategoriesReader)
        {
            networkManager.CommandsManager.AddCommand(new GetAvailableCategoriesCommand(networkManager, availableCategoriesReader));
            networkManager.CommandsManager.AddCommand(new SelectedCategoryCommand(extractor, leaderboard));
        }
    }
}
