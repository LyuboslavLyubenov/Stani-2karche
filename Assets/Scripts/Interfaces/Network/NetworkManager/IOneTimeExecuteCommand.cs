using System;

namespace Assets.Scripts.Interfaces
{

    /// <summary>
    /// When executed is destroyed from commandsmanager class
    /// </summary>
    public interface IOneTimeExecuteCommand : INetworkManagerCommand
    {
        bool FinishedExecution
        {
            get;
        }

        EventHandler OnFinishedExecution
        {
            get;
            set;
        }
    }

}