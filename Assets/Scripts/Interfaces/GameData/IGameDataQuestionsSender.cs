namespace Interfaces.GameData
{

    using System;

    using EventArgs;

    public interface IGameDataQuestionsSender
    {
        event EventHandler<ServerSentQuestionEventArgs> OnBeforeSend;

        event EventHandler<ServerSentQuestionEventArgs> OnSentQuestion;
    }
}
