using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Utils;

    public class GeneralSettingsUI : ExtendedMonoBehaviour
    {
        InputField usernameInputField;

        void Awake()
        {
            this.usernameInputField = this.transform.Find("ChangeUsername")
                .Find("InputField")
                .GetComponent<InputField>();    
        }

        void OnEnable()
        {
            if (!PlayerPrefsEncryptionUtils.HasKey("Username"))
            {
                return;   
            }
        
            var username = PlayerPrefsEncryptionUtils.GetString("Username");
            this.usernameInputField.text = username;
        }

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
