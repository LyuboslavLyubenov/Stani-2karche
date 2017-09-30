using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;

namespace Tests.Network.NetworkCommandData
{

    public class CorrectNamePropertyValue : MonoBehaviour
    {
        void Start()
        {
            var commandName = "123";
            var command = new NetworkCommandDataClass(commandName);

            if (command.Name == commandName)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }
    }

}
   