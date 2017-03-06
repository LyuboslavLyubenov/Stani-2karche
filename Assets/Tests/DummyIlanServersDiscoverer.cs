namespace Assets.Tests
{

    using System;

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;

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