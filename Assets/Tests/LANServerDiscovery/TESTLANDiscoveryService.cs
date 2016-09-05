using UnityEngine;
using System.Collections;

public class TESTLANDiscoveryService : MonoBehaviour
{
    public LANServersDiscoveryBroadcastService LanDiscovery;

    void Start()
    {
        LanDiscovery.OnFound += OnFound;
    }

    void OnFound(object sender, IpEventArgs args)
    {
        Debug.Log("Server found " + args.IPAddress);
    }
}
