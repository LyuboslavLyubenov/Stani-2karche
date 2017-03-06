namespace Assets.Scripts.Interfaces.GameData
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface IGameDataQuestionsSender
    {
        event EventHandler<ServerSentQuestionEventArgs> OnBeforeSend;

        event EventHandler<ServerSentQuestionEventArgs> OnSentQuestion;
    }
}
