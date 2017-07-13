namespace Commands.Server
{

    public class MainPlayerConnectingCommand : PlayerConnectingCommand
    {
        public MainPlayerConnectingCommand(PlayerConnectingDelegate onPlayerConnecting)
            : base(onPlayerConnecting)
        {
        }
    }

}