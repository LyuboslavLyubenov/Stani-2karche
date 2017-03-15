namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    using System;

    using Assets.Scripts.EventArgs;

    /// <summary>
    /// Used to send question to client and receive response
    /// </summary>
    public interface IAskClientQuestionRouter : IJokerRouter, IDisposable
    {
        event EventHandler<AnswerEventArgs> OnReceivedAnswer;

        bool Active { get; }
        
        void Activate(int clientConnectionId, int timeToAnswerInSeconds, ISimpleQuestion question);

        void Deactivate();
    }
}
