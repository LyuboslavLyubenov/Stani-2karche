namespace Assets.Tests.DummyObjects.UIControllers
{

    using System.Linq;

    using Interfaces.Controllers;

    public class DummyElectionQuestionUIController : DummyQuestionUIController, IElectionQuestionUIController
    {
        public string HighestVotedAnswer
        {
            get
            {
                return base.answersVotes.OrderByDescending(av => av.Value)
                    .First()
                    .Key;
            }
        }

        public void AddVoteFor(string answer)
        {
            base.answersVotes[answer]++;
        }
    }
}