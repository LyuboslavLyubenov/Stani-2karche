using System.Collections.Generic;

namespace Assets.Scripts.Interfaces
{

    public interface INetworkManagerCommand
    {
        void Execute(Dictionary<string, string> commandsOptionsValues);
    }

}