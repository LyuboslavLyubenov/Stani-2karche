namespace Interfaces.Controllers
{

    public interface IElectionBubbleUIController
    {
        int VoteCount { get; }

        void AddVote();

        void ResetVotesToZero();
    }

}