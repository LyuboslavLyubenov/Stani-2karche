using Interfaces.GameData;
using System.Linq;
using Assets.Scripts.Utils;

namespace Jokers.Routers
{
    using System;

    using Commands;
    
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJokerRouter : IDisableRandomAnswersRouter
    {
        private readonly IServerNetworkManager networkManager;

        private readonly IGameDataIterator gameDataIterator;

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public DisableRandomAnswersJokerRouter(
            IServerNetworkManager networkManager, 
            IGameDataIterator gameDataIterator)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }
                
            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;
        }

        public void Activate(int answersToDisableCount, int connectionId)
        {
            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                {
                    if (answersToDisableCount < 0 || answersToDisableCount >= question.Answers.Length)
                    {
                        var exception = new ArgumentOutOfRangeException("answersToDisableCount");
                        this.OnError(this, new UnhandledExceptionEventArgs(exception, false));
                        return;
                    }

                    if (connectionId <= 0)
                    {
                        var exception = new ArgumentOutOfRangeException("connectionId");
                        this.OnError(this, new UnhandledExceptionEventArgs(exception, false));
                        return;
                    }

                    var settingsCommand = new NetworkCommandData("DisableRandomAnswersJokerSettings");
                    var answersToDisable = question.Answers.Where(a => question.CorrectAnswer != a)
                        .ToList()
                        .GetRandomElements(answersToDisableCount)
                        .ToArray();

                    for (int i = 0; i < answersToDisable.Length; i++)
                    {
                        var answer = answersToDisable[i];
                        settingsCommand.AddOption(i.ToString(), answer);
                    }

                    this.networkManager.SendClientCommand(connectionId, settingsCommand);

                    this.OnActivated(this, EventArgs.Empty);
                }, 
                (exception) =>
                {
                    this.OnError(this, new UnhandledExceptionEventArgs(exception, false));
                });
        }
    }
}