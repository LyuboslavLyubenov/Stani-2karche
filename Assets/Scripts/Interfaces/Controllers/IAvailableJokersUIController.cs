namespace Assets.Scripts.Interfaces.Controllers
{

    using System;

    using EventArgs;

    public interface IAvailableJokersUIController
    {
        event EventHandler<JokerEventArgs> OnAddedJoker;
        event EventHandler<JokerEventArgs> OnUsedJoker;

        int JokersCount
        {
            get;            
        }

        void AddJoker(IJoker joker);

        void ClearAll();
    }
}
