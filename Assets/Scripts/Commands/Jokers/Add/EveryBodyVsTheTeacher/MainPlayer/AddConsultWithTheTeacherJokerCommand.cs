using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer
{
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter;

    public class AddConsultWithTheTeacherJokerCommand : AddJokerToMainPlayerAbstract<ConsultWithTeacherJoker>
    {
        public AddConsultWithTheTeacherJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager)
            : base(jokersUIController, networkManager)
        {
        }
    }
}