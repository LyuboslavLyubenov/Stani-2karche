using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{

    public class From_CreatingWithValidName : MonoBehaviour
    {
        void Start()
        {
            var commandData = NetworkCommandDataClass.From<TestCommand>();

            if (commandData.Name == "Test")
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }

        public class TestCommand
        {
        }
    }
}