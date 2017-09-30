using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;

namespace Tests.Network.NetworkCommandData
{

    public class CommandNameLengthCantBeLessThan3Characters : MonoBehaviour
    {
        void Start()
        {
            new NetworkCommandDataClass("12");
        }
    }

}
   