namespace Tests
{

    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;

    using Utils.Unity;

    public class TEST_IsServerAvailable : MonoBehaviour
    {

        public InputField InputField;

        public void Test()
        {
            var ip = this.InputField.text;

            NetworkTransport.Init();
            NetworkManagerUtils.Instance.IsServerUp(ip, 7788, (isRunning) => Debug.Log(isRunning));
        }

    }

}
