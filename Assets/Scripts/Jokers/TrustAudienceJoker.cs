using IAnswerPollRouter = Interfaces.Network.Jokers.Routers.IAnswerPollRouter;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;

namespace Assets.Scripts.Jokers
{
    using System;
    using Interfaces;
    using UnityEngine;

    public class TrustAudienceJoker : IJoker
    {
        public event EventHandler OnActivated = delegate
            {
            };
        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };
        public event EventHandler OnFinishedExecution = delegate
            {
            };

        private readonly IAnswerPollRouter answerPollRouter;

        private readonly IGameDataIterator gameDataIterator;

        public Sprite Image
        {
            get; private set;
        }

        public bool Activated
        {
            get; private set;
        }

        public TrustAudienceJoker(IAnswerPollRouter answerPollRouter, IGameDataIterator gameDataIterator)
        {
            this.answerPollRouter = answerPollRouter;
            this.gameDataIterator = gameDataIterator;
            throw new NotImplementedException();
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }
    }
}