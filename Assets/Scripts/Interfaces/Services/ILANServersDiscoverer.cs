namespace Assets.Scripts.Interfaces.Services
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface ILANServersDiscoverer : IDisposable
    {
        event EventHandler<IpEventArgs> OnFound;
    }

}