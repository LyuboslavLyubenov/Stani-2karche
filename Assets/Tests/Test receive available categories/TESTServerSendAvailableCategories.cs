using UnityEngine;
using System.Collections;

public class TESTServerSendAvailableCategories : MonoBehaviour
{
    public ServerNetworkManager NetworkManager;
    public LocalGameData GameData;
    public LeaderboardSerializer Leaderboard;

    void Start()
    {
        NetworkManager.CommandsManager.AddCommand("GetAvailableCategories", new ReceivedServerGetAllAvailableCategoriesCommand(NetworkManager));
        NetworkManager.CommandsManager.AddCommand("SelectedCategory", new ReceivedServerSelectedCategoryCommand(GameData, Leaderboard));
    }
}
