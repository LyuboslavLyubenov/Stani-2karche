namespace States.EveryBodyVsTheTeacher.Server
{

    using System;

    using Assets.Scripts.Utils.States.EveryBodyVsTheTeacher;

    using Commands;
    using Commands.Jokers.Selected;

    using EventArgs;

    using Interfaces;
    using Interfaces.Commands.Jokers.Selected;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Jokers;
    using Jokers.Kalitko;

    using Localization;

    using Network;

    using Notifications;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class FirstRoundState : IState
    {
        private readonly Type[] jokersForThisRound = new []
                                                            {
                                                                typeof(MainPlayerKalitkoJoker),
                                                                typeof(TrustRandomPersonJoker),
                                                                typeof(ConsultWithTheTeacherJoker)
                                                            };

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IGameDataExtractor gameDataExtractor;
        
        [Inject]
        private IAvailableCategoriesReader availableCategoriesReader;

        [Inject]
        private JokersData jokersData;

        [Inject]
        private JokersDataSender jokersDataSender;
        
        [Inject]
        private IKalitkoJokerRouter kalitkoJokerRouter;
        
        [Inject]
        private ITrustRandomPersonJokerRouter trustRandomPersonJokerRouter;

        [Inject]
        private IDisableRandomAnswersRouter disableRandomAnswersRouter;

        private IElectionJokerCommand[] selectJokerCommands;
        
        private void InitializeSelectJokerCommands()
        {
            var commandsManager = this.networkManager.CommandsManager;
            this.selectJokerCommands = new IElectionJokerCommand[]
                                      {
                                          new SelectedKalitkoJokerCommand(this.server, this.kalitkoJokerRouter), 
                                          new SelectedTrustRandomPersonJokerCommand(this.server, this.trustRandomPersonJokerRouter), 
                                          new SelectedConsultWithTeacherJokerCommand(this.server, this.disableRandomAnswersRouter)
                                      };
            commandsManager.AddCommands(this.selectJokerCommands);
        }


        public void OnStateEnter(StateMachine stateMachine)
        {
            this.gameDataExtractor.ExtractDataSync();

            for (int i = 0; i < this.jokersForThisRound.Length; i++)
            {
                var jokerToAdd = this.jokersForThisRound[i];
                this.jokersData.AddJoker(jokerToAdd);
            }
            
            this.InitializeSelectJokerCommands();
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            JokersUtils.RemoveRemainingJokers(jokersForThisRound, this.jokersData);
            JokersUtils.RemoveSelectJokerCommands(this.networkManager, this.selectJokerCommands);
        }
    }
}