namespace Assets.Scripts.Interfaces.Network.Jokers
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface IAskPlayerQuestionResultRetriever : IDisposable
    {
        event EventHandler<AskPlayerResponseEventArgs> OnReceivedAnswer;

        event EventHandler<JokerSettingsEventArgs> OnReceivedSettings;

        event EventHandler OnReceiveAnswerTimeout;

        event EventHandler OnReceiveSettingsTimeout;

        bool Active { get; }

        void Activate(int playerConnectionId);
    }

}
