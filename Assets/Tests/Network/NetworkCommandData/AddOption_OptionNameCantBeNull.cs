using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;

namespace Tests.Network.NetworkCommandData
{

    public class AddOption_OptionNameCantBeNull : MonoBehaviour
    {
        void Start()
        {
            new NetworkCommandDataClass("123").AddOption(null, "123");    
        }
    }
    
}
   