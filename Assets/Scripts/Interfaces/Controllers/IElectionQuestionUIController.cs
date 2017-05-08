namespace Interfaces.Controllers
{
    public interface IElectionQuestionUIController : IQuestionUIController
    {
        string HighestVotedAnswer { get; }

        void AddVoteFor(string answer);
    }

}
