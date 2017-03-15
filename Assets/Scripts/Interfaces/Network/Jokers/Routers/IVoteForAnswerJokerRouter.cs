namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    using System.Collections.Generic;

    public interface IVoteForAnswerJokerRouter : IJokerRouter
    {
        void Activate(int timeToAnswerInSeconds, IEnumerable<int> clientsThatMustVote);
    }
}
