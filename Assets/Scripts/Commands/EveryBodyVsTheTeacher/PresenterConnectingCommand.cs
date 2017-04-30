using PlayerConnectingCommand = Commands.Server.PlayerConnectingCommand;

namespace Assets.Scripts.Commands.EveryBodyVsTheTeacher
{
    public class PresenterConnectingCommand : PlayerConnectingCommand
    {
        public PresenterConnectingCommand(PlayerConnectingDelegate onPlayerConnecting)
            : base(onPlayerConnecting)
        {
        }
    }
}