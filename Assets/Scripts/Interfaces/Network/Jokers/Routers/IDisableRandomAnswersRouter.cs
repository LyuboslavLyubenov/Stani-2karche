namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    public interface IDisableRandomAnswersRouter : IJokerRouter
    {
        void Activate(int answersToDisableCount);
    }
}