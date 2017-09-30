using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using Jokers.EveryBodyVsTheTeacher.Kalitko;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer
{

    using Assets.Scripts.Interfaces.Controllers;

    public class AddKalitkoJokerCommand : AddJokerToMainPlayerAbstract<KalitkoJoker>
    {
        public AddKalitkoJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager)
            : base(jokersUIController, networkManager)
        {
        }
    }
}
