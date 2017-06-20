namespace Commands.Server
{

    using System.Collections.Generic;

    using Assets.Scripts.Extensions;

    public class MainPlayerConnectingCommand : PlayerConnectingCommand
    {
        public MainPlayerConnectingCommand(PlayerConnectingDelegate onPlayerConnecting)
            : base(onPlayerConnecting)
        {
        }
    }

}