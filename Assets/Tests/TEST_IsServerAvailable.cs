using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Tests
{

    using Assets.Scripts.Utils.Unity;

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
