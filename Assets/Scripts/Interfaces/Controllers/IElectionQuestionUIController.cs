using IQuestionUIController = Interfaces.IQuestionUIController;

namespace Assets.Scripts.Interfaces.Controllers
{
    public interface IElectionQuestionUIController : IQuestionUIController
    {
        string HighestVotedAnswer { get; }

        void AddVoteFor(string answer);
    }

    public interface IElectionBubbleUIController
    {
        int VoteCount { get; }

        void AddVote();

        void ResetVotesToZero();
    }
}
