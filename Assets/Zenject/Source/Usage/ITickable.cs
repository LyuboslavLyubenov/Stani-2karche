namespace Zenject.Source.Usage
{
    public interface ITickable
    {
        void Tick();
    }

    public interface IFixedTickable
    {
        void FixedTick();
    }

    public interface ILateTickable
    {
        void LateTick();
    }
}

