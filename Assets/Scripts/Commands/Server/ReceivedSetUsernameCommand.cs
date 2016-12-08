using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

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
            networkManager.SetClientUsername(connectionId, "Играч " + connectionId);
            return;
        }
        else
        {
            var username = commandsParamsValues["Username"];

            if (DoesUsernameContaisForbiddenWords(username))
            {
                var message = LanguagesManager.Instance.GetValue("KickMessages/UsernameContainsBannedWords");
                networkManager.KickPlayer(connectionId, message);
                return;
            }

            networkManager.SetClientUsername(connectionId, username);
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
