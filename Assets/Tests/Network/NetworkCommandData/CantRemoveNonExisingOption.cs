using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{

    public class CantRemoveNonExisingOption : MonoBehaviour
    {
        void Start()
        {
            var commandData = new NetworkCommandDataClass("123");
            commandData.RemoveOption("NonExistingOption");
        }
    }
}
   