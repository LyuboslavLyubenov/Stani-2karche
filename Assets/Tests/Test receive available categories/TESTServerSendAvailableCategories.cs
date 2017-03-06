using UnityEngine;

namespace Assets.Tests.Test_receive_available_categories
{

    using Assets.Scripts.IO;
    using Assets.Scripts.Network.NetworkManagers;

    using Scripts.Commands.Server;

    public class TESTServerSendAvailableCategories : MonoBehaviour
    {
        public ServerNetworkManager NetworkManager;
        public GameDataExtractor GameData;
        public LeaderboardDataManipulator Leaderboard;

        private void Start()
        {
            this.NetworkManager.CommandsManager.AddCommand("GetAvailableCategories", new GetAvailableCategoriesCommand(this.NetworkManager));
            this.NetworkManager.CommandsManager.AddCommand("SelectedCategory", new SelectedCategoryCommand(this.GameData, this.Leaderboard));
        }
    }

}
