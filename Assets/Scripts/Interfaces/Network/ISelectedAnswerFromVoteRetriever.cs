using AnswerEventArgs = EventArgs.AnswerEventArgs;

namespace Interfaces.Network
{

    using System;

    /// <summary>
    /// Used to collect answers from main players in EveryBodyVsTheTeacher
    /// </summary>
    public interface ICollectVoteResultForAnswerForCurrentQuestion : IDisposable
    {
        event EventHandler<AnswerEventArgs> OnCollectedVote;
        event EventHandler OnNoVotesCollected;
        event EventHandler<UnhandledExceptionEventArgs> OnLoadingCurrentQuestionError;

        bool Collecting { get; }

        void StartCollecting();

        void StopCollecting();
    }
}
