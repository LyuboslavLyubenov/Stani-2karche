namespace Assets.Scripts.Interfaces
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface ILANServersDiscoveryService : IDisposable
    {
        event EventHandler<IpEventArgs> OnFound;
    }

}