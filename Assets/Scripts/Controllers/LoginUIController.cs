using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.KinveySerializableObj;

    using Debug = UnityEngine.Debug;

    public class LoginUIController : MonoBehaviour
    {
        public InputField UsernameInputField;
        public InputField PasswordInputField;

        void OnLoggedIn(_UserReceivedData userData)
        {
            Debug.Log("Logged in successfuly ");
        }

        public void Login()
        {
            var username = this.UsernameInputField.text;
            var password = this.PasswordInputField.text;
            KinveyWrapper.Instance.LoginAsync(username, password, this.OnLoggedIn, Debug.LogException);
        }
    }

}

