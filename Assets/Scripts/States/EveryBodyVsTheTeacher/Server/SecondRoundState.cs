using AskAudienceJoker = Jokers.AskAudienceJoker;
using DisableRandomAnswersJoker = Jokers.DisableRandomAnswersJoker;
using SelectedLittleIsBetterThanNothingJokerCommand = Commands.Jokers.Selected.SelectedLittleIsBetterThanNothingJokerCommand;


namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server
{
    using System;

    using Assets.Scripts.Commands.Jokers.Selected;
    using Assets.Scripts.Utils.States.EveryBodyVsTheTeacher;

    using Interfaces;
    using Interfaces.Commands.Jokers.Selected;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Jokers;

    using Network;

    using StateMachine;

    using Zenject.Source.Usage;

    public class SecondRoundState : IState
    {
        private readonly Type[] jokersForThisRound = new Type[]
                                                     {
                                                         typeof(LittleIsBetterThanNothingJoker),
                                                         typeof(AskAudienceJoker)
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
        private IDisableRandomAnswersRouter disableRandomAnswersRouter;

        [Inject]
        private IHelpFromAudienceJokerRouter audienceJokerRouter;

        [Inject]
        private IGameDataIterator gameDataIterator;

        private IElectionJokerCommand[] selectJokerCommands;
        
        private void InitializeSelectJokerCommands()
        {
            var commandsManager = this.networkManager.CommandsManager;
            this.selectJokerCommands = new IElectionJokerCommand[]
                                       {
                                           new SelectedLittleIsBetterThanNothingJokerCommand(this.server, this.disableRandomAnswersRouter),
                                           new SelectedHelpFromAudienceElectionJokerCommand(this.server, this.audienceJokerRouter, this.gameDataIterator), 
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