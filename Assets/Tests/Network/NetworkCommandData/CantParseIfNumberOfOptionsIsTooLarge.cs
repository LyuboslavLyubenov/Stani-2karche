using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{

    public class CantParseIfNumberOfOptionsIsTooLarge : MonoBehaviour
    {
        void Start()
        {
            NetworkCommandDataClass.Parse("10 101 asdasd");
        }
    }
    
}