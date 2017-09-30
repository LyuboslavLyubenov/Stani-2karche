using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{

    public class CantParseIfNumberForTitleIsLessThan3CharactersLong : MonoBehaviour
    {
        void Start()
        {
            NetworkCommandDataClass.Parse("1 asdasdsd");
        }
    }
    
}