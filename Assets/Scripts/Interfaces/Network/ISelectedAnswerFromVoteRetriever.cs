namespace Assets.Scripts.Interfaces.Network
{

    using System;

    using EventArgs;

    /// <summary>
    /// Used to collect answers from main players in EveryBodyVsTheTeacher
    /// </summary>
    public interface ICollectVoteResultForAnswerForCurrentQuestion : IDisposable
    {
        event EventHandler<AnswerEventArgs> OnCollectedVote;

        event EventHandler<UnhandledExceptionEventArgs> OnLoadingCurrentQuestionError;

        bool Collecting { get; }

        void StartCollecting();

        void StopCollecting();
    }
}
