using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Assets.Scripts.Commands.Server
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Network;

    public class ReceivedSetUsernameCommand : INetworkManagerCommand
    {
        ServerNetworkManager networkManager;

        const string BannedWordsInUsernameFileName = "bannedWordsInUsernames.txt";

        public ReceivedSetUsernameCommand(ServerNetworkManager networkManager)
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

            if (!commandsParamsValues.ContainsKey("Username"))
            {
                //empty username :(
                this.networkManager.SetClientUsername(connectionId, "Играч " + connectionId);
                return;
            }
            else
            {
                var username = commandsParamsValues["Username"];

                if (this.DoesUsernameContaisForbiddenWords(username))
                {
                    var message = LanguagesManager.Instance.GetValue("KickMessages/UsernameContainsBannedWords");
                    this.networkManager.KickPlayer(connectionId, message);
                    return;
                }

                this.networkManager.SetClientUsername(connectionId, username);
                return;
            }
        }

        bool DoesUsernameContaisForbiddenWords(string username)
        {
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..\\LevelData\\" + BannedWordsInUsernameFileName;
            var forbiddenUsernames = File.ReadAllLines(execPath);
            var usernameLower = username.ToLowerInvariant();
            return forbiddenUsernames.Any(u => u.ToLowerInvariant().Contains(usernameLower));
        }
    }

}
