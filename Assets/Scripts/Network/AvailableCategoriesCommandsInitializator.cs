public class AvailableCategoriesCommandsInitializator : ExtendedMonoBehaviour
{
    public ServerNetworkManager NetworkManager;
    public LocalGameData GameData;
    public LeaderboardSerializer Leaderboard;

    void Start()
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                NetworkManager.CommandsManager.AddCommand("GetAvailableCategories", new ReceivedServerGetAllAvailableCategoriesCommand(NetworkManager));
                NetworkManager.CommandsManager.AddCommand("SelectedCategory", new ReceivedServerSelectedCategoryCommand(GameData, Leaderboard));
            });
    }

}
