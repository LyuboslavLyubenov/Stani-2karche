using DisableRandomAnswersJoker = Jokers.DisableRandomAnswersJoker;
using HelpFromFriendJoker = Jokers.HelpFromFriendJoker;
using SelectedLittleIsBetterThanNothingJokerCommand = Commands.Jokers.Selected.SelectedLittleIsBetterThanNothingJokerCommand;
using SelectedTrustRandomPersonJokerCommand = Commands.Jokers.Selected.SelectedTrustRandomPersonJokerCommand;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server
{

    using System;
    
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Utils.States.EveryBodyVsTheTeacher;

    using Interfaces;
    using Interfaces.Commands.Jokers.Selected;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Network;

    using StateMachine;

    using Zenject.Source.Usage;

    public class ThirdRoundState : IState
    {
        private readonly Type[] jokersForThisRound = new Type[]
                                                     {
                                                         typeof(HelpFromFriendJoker)
                                                     };

        [Inject]
        private JokersData jokersData;

        [Inject]
        private JokersDataSender jokersDataSender;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private ITrustRandomPersonJokerRouter trustRandomPersonJokerRouter;

        private IElectionJokerCommand[] selectJokerCommands;

        private void InitializeSelectJokerCommands()
        {
            var commandsManager = this.networkManager.CommandsManager;
            this.selectJokerCommands = new IElectionJokerCommand[]
                                       {
                                           new SelectedTrustRandomPersonJokerCommand(this.server, this.trustRandomPersonJokerRouter), 
                                       };
            commandsManager.AddCommands(this.selectJokerCommands);
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            this.jokersData.AddJoker<DisableRandomAnswersJoker>();
            this.jokersData.AddJoker<TrustAudienceJoker>();
            this.InitializeSelectJokerCommands();
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            JokersUtils.RemoveRemainingJokers(jokersForThisRound, this.jokersData);
            JokersUtils.RemoveSelectJokerCommands(this.networkManager, this.selectJokerCommands);
        }
    }
}
