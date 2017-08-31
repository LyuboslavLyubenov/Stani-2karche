namespace Commands.Server
{

    using System.Collections.Generic;

    public class MainPlayerConnectingCommand : PlayerConnectingCommand
    {
        public MainPlayerConnectingCommand(PlayerConnectingDelegate onPlayerConnecting)
            : base(onPlayerConnecting)
        {
        }
    }

}