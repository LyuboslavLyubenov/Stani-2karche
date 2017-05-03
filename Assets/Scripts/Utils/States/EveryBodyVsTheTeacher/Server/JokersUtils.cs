using IElectionJokerCommand = Interfaces.Commands.Jokers.Selected.IElectionJokerCommand;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokersData = Network.JokersData;

namespace Assets.Scripts.Utils.States.EveryBodyVsTheTeacher.Server
{

    using System;

    public class JokersUtils
    {
        JokersUtils()
        {   
        }

        public static void RemoveRemainingJokers(Type[] jokersForThisRound, JokersData jokersData)
        {
            for (int i = 0; i < jokersForThisRound.Length; i++)
            {
                var jokerType = jokersForThisRound[i];

                if (jokersData.AvailableJokers.Contains(jokerType))
                {
                    jokersData.RemoveJoker(jokerType);
                }
            }
        }
        
        public static void RemoveSelectJokerCommands(IServerNetworkManager networkManager, IElectionJokerCommand[] selectJokerCommands)
        {
            var commandsManager = networkManager.CommandsManager;

            for (int i = 0; i < selectJokerCommands.Length; i++)
            {
                var jokerElectionCommand = selectJokerCommands[i];
                commandsManager.RemoveCommand(jokerElectionCommand);
            }
        }
    }
}
