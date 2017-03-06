namespace Assets.Scripts.Interfaces
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface ILANServersDiscoverer : IDisposable
    {
        event EventHandler<IpEventArgs> OnFound;
    }

}