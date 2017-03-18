namespace Interfaces.Services
{

    using System;

    using EventArgs;

    public interface ILANServersDiscoverer : IDisposable
    {
        event EventHandler<IpEventArgs> OnFound;
    }

}