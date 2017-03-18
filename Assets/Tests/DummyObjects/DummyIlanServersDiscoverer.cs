namespace Tests.DummyObjects
{

    using System;

    using EventArgs;

    using Interfaces.Services;

    public class DummyIlanServersDiscoverer : ILANServersDiscoverer
    {
        public event EventHandler<IpEventArgs> OnFound = delegate
            { };

        public void FakeFoundServer(string ipAddress)
        {
            this.OnFound(this, new IpEventArgs(ipAddress));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}