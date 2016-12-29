using UnityEngine;

namespace Assets.Tests.Test_receive_available_categories
{

    using Assets.Scripts.IO;
    using Assets.Scripts.Network.NetworkManagers;

    using Scripts;
    using Scripts.Commands.Server;
    using Scripts.Network;

    public class TESTServerSendAvailableCategories : MonoBehaviour
    {
        public ServerNetworkManager NetworkManager;
        public GameDataExtractor GameData;
        public LeaderboardSerializer Leaderboard;

        private void Start()
        {
            this.NetworkManager.CommandsManager.AddCommand("GetAvailableCategories", new GetAllAvailableCategoriesCommand(this.NetworkManager));
            this.NetworkManager.CommandsManager.AddCommand("SelectedCategory", new SelectedCategoryCommand(this.GameData, this.Leaderboard));
        }
    }

}
