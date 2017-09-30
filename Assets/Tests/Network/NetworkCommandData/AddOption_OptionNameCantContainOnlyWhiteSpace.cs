using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;

namespace Tests.Network.NetworkCommandData
{

    public class AddOption_OptionNameCantContainOnlyWhiteSpace : MonoBehaviour
    {
        void Start()
        {
            new NetworkCommandDataClass("123").AddOption("         ", "123");    
        }
    }
    
}
   