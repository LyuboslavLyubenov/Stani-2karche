using UnityEngine;

namespace Assets.Tests.Test_Kinvey_Wrapper.Delete
{

    using Assets.Scripts;
    using Assets.Scripts.Network;

    public class TEST_DeleteEntity : MonoBehaviour
    {
        private void Start()
        {
            var kinveyWrapper = new KinveyWrapper();

            kinveyWrapper.LoginAsync("ivan", "ivan", (data) =>
                {
                    kinveyWrapper.DeleteEntityAsync("Servers", "58356df6f08321f70dc31bd3", (data1) =>
                        {
                            Debug.Log("Deleted count " + data1.count);
                        }, Debug.LogException);
                }, Debug.LogException);
        }
	
    }

}
