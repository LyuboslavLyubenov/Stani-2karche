namespace Assets.Scripts.Utils.States.EveryBodyVsTheTeacher
{

    using System;

    using Interfaces.Commands.Jokers.Selected;
    using Interfaces.Network.NetworkManager;

    using Network;

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
