using KinveyWrapper = Network.KinveyWrapper;

namespace Tests.Test_Kinvey_Wrapper.Does_username_exists
{

    using UnityEngine;

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
