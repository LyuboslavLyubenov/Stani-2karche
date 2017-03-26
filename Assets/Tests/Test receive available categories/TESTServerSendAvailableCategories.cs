using GetAvailableCategoriesCommand = Commands.Server.GetAvailableCategoriesCommand;
using SelectedCategoryCommand = Commands.Server.SelectedCategoryCommand;
using ServerNetworkManager = Network.NetworkManagers.ServerNetworkManager;

namespace Tests.Test_receive_available_categories
{

    using IO;

    using UnityEngine;

    public class TESTServerSendAvailableCategories : MonoBehaviour
    {
        public ServerNetworkManager NetworkManager;
        public GameDataExtractor GameData;
        public LeaderboardDataManipulator Leaderboard;

        void Start()
        {        
            this.NetworkManager.CommandsManager.AddCommand("GetAvailableCategories", new GetAvailableCategoriesCommand(this.NetworkManager, new LocalCategoriesReader()));
            this.NetworkManager.CommandsManager.AddCommand("SelectedCategory", new SelectedCategoryCommand(this.GameData, this.Leaderboard));
        }
    }

}
