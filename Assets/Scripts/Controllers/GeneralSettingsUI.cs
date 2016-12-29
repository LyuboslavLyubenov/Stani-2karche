namespace Assets.Scripts.Controllers
{
    using UnityEngine.UI;

    using Utils.Unity;

    public class GeneralSettingsUI : ExtendedMonoBehaviour
    {
        private InputField usernameInputField;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Awake()
        {
            this.usernameInputField = this.transform.Find("ChangeUsername")
                .Find("InputField")
                .GetComponent<InputField>();    
        }

        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            if (!PlayerPrefsEncryptionUtils.HasKey("Username"))
            {
                return;   
            }
        
            var username = PlayerPrefsEncryptionUtils.GetString("Username");
            this.usernameInputField.text = username;
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnDisable()
        {
            var username = this.usernameInputField.text;

            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            PlayerPrefsEncryptionUtils.SetString("Username", username);
        }
    }

}
