namespace Interfaces.Network.Jokers
{

    using System;

    using EventArgs;

    public interface IAskClientQuestionResultRetriever : IDisposable
    {
        event EventHandler<AskClientQuestionResponseEventArgs> OnReceivedAnswer;

        event EventHandler<JokerSettingsEventArgs> OnReceivedSettings;

        event EventHandler OnReceiveAnswerTimeout;

        event EventHandler OnReceiveSettingsTimeout;

        bool Active { get; }

        void Activate(int playerConnectionId);

        void Deactivate();
    }

}
