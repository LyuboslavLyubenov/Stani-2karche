namespace Commands.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Interfaces.Network.NetworkManager;

    using Localization;

    public class SetUsernameCommand : INetworkManagerCommand
    {
        private IServerNetworkManager networkManager;

        private const string BannedWordsInUsernameFileName = "bannedWordsInUsernames.txt";

        public SetUsernameCommand(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
        }

        public void Execute(Dictionary<string, string> commandsParamsValues)
        {
            var connectionId = int.Parse(commandsParamsValues["ConnectionId"]);
            var username = this.networkManager.GetClientUsername(connectionId);

            //username already set
            if (!string.IsNullOrEmpty(username))
            {
                return;
            }

            if (commandsParamsValues.ContainsKey("Username"))
            {
                username = commandsParamsValues["Username"];

                if (this.DoesUsernameContaisForbiddenWords(username))
                {
                    var message = LanguagesManager.Instance.GetValue("KickMessages/UsernameContainsBannedWords");
                    this.networkManager.KickPlayer(connectionId, message);
                    return;
                }
            }
            else
            {
                //empty username :(
                username = "Играч " + username;
            }

            this.networkManager.SetClientUsername(connectionId, username);
        }

        private bool DoesUsernameContaisForbiddenWords(string username)
        {
            var gameDirectoryPath = Assets.Scripts.Utils.PathUtils.GetGameDirectoryPath();
            var forbiddenUsernamesFilePath = gameDirectoryPath + "/LevelData/" + BannedWordsInUsernameFileName;
            var forbiddenUsernames = File.ReadAllLines(forbiddenUsernamesFilePath);
            var usernameLower = username.ToLowerInvariant();
            return forbiddenUsernames.Any(u => u.ToLowerInvariant().Contains(usernameLower));
        }
    }
}