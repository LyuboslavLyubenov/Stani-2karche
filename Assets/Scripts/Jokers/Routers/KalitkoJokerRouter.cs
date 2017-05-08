using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IKalitkoJokerRouter = Interfaces.Network.Jokers.Routers.IKalitkoJokerRouter;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using ISimpleQuestion = Interfaces.ISimpleQuestion;
using NetworkCommandData = Commands.NetworkCommandData;
using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Jokers.Routers
{

    using System;
    using System.Linq;

    using Assets.Scripts.Commands.Jokers.Result;
    using Assets.Scripts.Utils;

    using Commands.Jokers;

    using Utils;

    public class KalitkoJokerRouter : IKalitkoJokerRouter
    {
        private const float DefaultChanceForAnswer = 0.5f; //50%
        private const float DefaultChanceForCorrectAnswer = 0.5f; //50%

        public event EventHandler OnActivated = delegate { };
        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate { };

        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;

        private readonly IGameDataIterator gameDataIterator;

        private readonly float chanceForAnswer;
        private readonly float chanceForCorrectAnswer;
        
        public KalitkoJokerRouter(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server,
            IGameDataIterator gameDataIterator) 
            : 
            this (
                networkManager, 
                server, 
                gameDataIterator, 
                DefaultChanceForAnswer, 
                DefaultChanceForCorrectAnswer)
        {
            
        }

        public KalitkoJokerRouter(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server,
            IGameDataIterator gameDataIterator,
            float chanceForAnswer = DefaultChanceForAnswer,
            float chanceForCorrectAnswer = DefaultChanceForCorrectAnswer)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            if (chanceForAnswer < 0 || chanceForAnswer > 1)
            {
                throw new ArgumentOutOfRangeException("chanceForAnswer");
            }

            if (chanceForCorrectAnswer < 0 || chanceForCorrectAnswer > 1)
            {
                throw new ArgumentOutOfRangeException("chanceForCorrectAnswer");
            }

            this.networkManager = networkManager;
            this.server = server;
            this.gameDataIterator = gameDataIterator;
            this.chanceForAnswer = chanceForAnswer;
            this.chanceForCorrectAnswer = chanceForCorrectAnswer;
        }

        private void SendJokerResult(ISimpleQuestion question)
        {
            var jokerResultCommand = NetworkCommandData.From<KalitkoJokerResultCommand>();
            var random = new Random();
            var shouldSendAnswer = random.NextDouble() <= this.chanceForAnswer;
            var shouldSendCorrectAnswer = random.NextDouble() <= this.chanceForCorrectAnswer;

            if (shouldSendAnswer)
            {
                string answer;

                if (shouldSendCorrectAnswer)
                {
                    answer = question.CorrectAnswer;
                }
                else
                {
                    answer = question.Answers.Where(a => a != question.CorrectAnswer)
                        .ToList()
                        .GetRandomElement();
                }

                jokerResultCommand.AddOption("Answer", answer);
            }

            this.networkManager.SendClientCommand(this.server.PresenterId, jokerResultCommand);
        }

        public void Activate()
        {
            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                    {
                        this.SendJokerResult(question);
                        this.OnActivated(this, EventArgs.Empty);
                    },
                (exception) =>
                    {
                        this.networkManager.CommandsManager.RemoveCommand<SelectedKalitkoJokerCommand>();
                        this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    });
        }
    }
}