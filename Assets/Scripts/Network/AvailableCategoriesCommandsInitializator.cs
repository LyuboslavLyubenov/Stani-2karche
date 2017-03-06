namespace Assets.Scripts.Network
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.GameData;

    using Commands.Server;

    using IO;
    using NetworkManagers;

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
