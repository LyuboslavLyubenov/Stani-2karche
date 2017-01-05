﻿using UnityEngine;

namespace Assets.Tests.Test_Kinvey_Wrapper.Does_username_exists
{

    using Assets.Scripts.Network;

    public class TEST_DoesUsernameExists : MonoBehaviour
    {
        private void Start()
        {
            var kinveyWrapper = new KinveyWrapper();
            kinveyWrapper.DoesUsernameExistsAsync("ivan", (data) =>
                {
                    Debug.Log("Exists = " + data.usernameExists);
                }, Debug.LogException);
        }
	
    }

}
