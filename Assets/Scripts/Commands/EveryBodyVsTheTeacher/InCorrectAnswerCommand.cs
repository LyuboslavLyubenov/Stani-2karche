using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.EveryBodyVsTheTeacher
{

    using System.Collections.Generic;

    public class InCorrectAnswerCommand : INetworkManagerCommand
    {
        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            throw new System.NotImplementedException();
        }
    }
}
