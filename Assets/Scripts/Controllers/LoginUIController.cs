namespace Assets.Scripts.Controllers
{
    using Network;

    using UnityEngine;
    using UnityEngine.UI;
    using DTOs.KinveySerializableObj;

    using Debug = UnityEngine.Debug;

    public class LoginUIController : MonoBehaviour
    {
        public InputField UsernameInputField;
        public InputField PasswordInputField;

        private void OnLoggedIn(_UserReceivedData userData)
        {
            //TODO:
            Debug.Log("Logged in successfuly ");
        }

        public void Login()
        {
            var username = this.UsernameInputField.text;
            var password = this.PasswordInputField.text;
            var kinveyWrapper = new KinveyWrapper();
            kinveyWrapper.LoginAsync(username, password, this.OnLoggedIn, Debug.LogException);
        }
    }

}