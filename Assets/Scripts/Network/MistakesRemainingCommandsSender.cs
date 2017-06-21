using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Network
{
    using System;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;

    using StateMachine;

    public class MistakesRemainingCommandsSender : IMistakesRemainingCommandsSender
    {
        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;
        private readonly IRoundsSwitcher roundsSwitcher;
        private readonly StateMachine stateMachine;
        private readonly PresenterConnectingCommand presenterConnectingCommand;

        public MistakesRemainingCommandsSender(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server, 
            IRoundsSwitcher roundsSwitcher,
            StateMachine stateMachine)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (roundsSwitcher == null)
            {
                throw new ArgumentNullException("roundsSwitcher");
            }

            if (stateMachine == null)
            {
                throw new ArgumentNullException("stateMachine");
            }

            this.networkManager = networkManager;
            this.server = server;
            this.roundsSwitcher = roundsSwitcher;
            this.stateMachine = stateMachine;
            this.presenterConnectingCommand = new PresenterConnectingCommand(this.OnPresenterConnecting);

            this.roundsSwitcher.OnSelectedInCorrectAnswer += OnSelectedInCorrectAnswer;
            this.roundsSwitcher.OnSwitchedToNextRound += OnSwitchedToNextRound;
            
            this.networkManager.CommandsManager.AddCommand(this.presenterConnectingCommand);
        }

        private void OnSwitchedToNextRound(object sender, EventArgs args)
        {
            this.SendLoadMistakesRemainingCommandToPresenterIfPossible();
        }


        private void OnSelectedInCorrectAnswer(object sender, EventArgs args)
        {
            this.SendLoadMistakesRemainingCommandToPresenterIfPossible();
        }

        private void OnPresenterConnecting(int connectionId)
        {
            this.SendLoadMistakesRemainingCommandToPresenterIfPossible();
        }

        private void SendLoadMistakesRemainingCommandToPresenterIfPossible()
        {
            if (this.IsAllowedToSendToPresenter())
            {
                this.SendLoadMistakesRemainingCommandToPresenter();
            }
        }

        private bool IsAllowedToSendToPresenter()
        {
            return
                this.server.StartedGame &&
                !this.server.IsGameOver &&
                this.stateMachine.CurrentState.IsImplemetingInterface(typeof(IRoundState));
        }

        private void SendLoadMistakesRemainingCommandToPresenter()
        {
            var roundState = (IRoundState)this.stateMachine.CurrentState;
            var loadMistakesRemainingCommand = NetworkCommandData.From<LoadMistakesRemainingCommand>();
            loadMistakesRemainingCommand.AddOption("Count", roundState.MistakesRemaining.ToString());
            this.networkManager.SendClientCommand(this.server.PresenterId, loadMistakesRemainingCommand);
        }

        public void Dispose()
        {
            this.roundsSwitcher.OnSelectedInCorrectAnswer -= OnSelectedInCorrectAnswer;
            this.roundsSwitcher.OnSwitchedToNextRound -= OnSwitchedToNextRound;

            if (this.networkManager.CommandsManager.Exists(this.presenterConnectingCommand))
            {
                this.networkManager.CommandsManager.RemoveCommand(this.presenterConnectingCommand);
            }
        }
    }
}
