using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Network.EveryBodyVsTheTeacher.Presenter
{

    using Assets.Scripts.Commands.Jokers.Election.Presenter;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    public class JokerElectionUICommandsBinder : JokerJokerElectionUiCommandsBinder
    {
        public JokerElectionUICommandsBinder(
            IClientNetworkManager networkManager, 
            IJokerElectionUIController jokerElectionUIController, 
            GameObject jokerElectionUI, 
            GameObject successfullyActivatedJokerUI, 
            GameObject unsuccessfullyActivatedJokerUI)
            : 
            base(networkManager, 
                 jokerElectionUIController, 
                 jokerElectionUI, 
                 successfullyActivatedJokerUI, 
                 unsuccessfullyActivatedJokerUI)
        {
        }

        public new void Bind(IJoker joker)
        {
            //HACK
            //TODO: REFACTOR
            base.Bind(joker);
            var jokerName = joker.GetName();
            base.networkManager.CommandsManager.RemoveCommand("ElectionJokerResultFor" + jokerName + "Joker");

            var electionJokerResult = new ElectionJokerResultCommand(
                base.jokerElectionUI,
                base.successfullyActivatedJokerUI,
                base.unsuccessfullyActivatedJokerUI,
                joker);

            base.networkManager.CommandsManager.AddCommand(
                "ElectionJokerResultFor" + jokerName + "Joker",
                electionJokerResult);
        }
    }
}