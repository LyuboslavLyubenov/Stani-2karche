using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{

    public class CantAddOptionsWithSameOptionKey : MonoBehaviour
    {
        void Start()
        {
            var commandData = new NetworkCommandDataClass("123");
            commandData.AddOption("Option1", "Value1");
            commandData.AddOption("Option1", "Value2");
        }
    }
    
}
   