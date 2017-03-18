namespace Interfaces.Network.Jokers.Routers
{

    using System;

    using EventArgs;

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
