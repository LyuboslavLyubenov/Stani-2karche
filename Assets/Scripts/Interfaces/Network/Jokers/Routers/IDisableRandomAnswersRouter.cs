namespace Interfaces.Network.Jokers.Routers
{

    public interface IDisableRandomAnswersRouter : IJokerRouter
    {
        void Activate(int answersToDisableCount);
    }
}