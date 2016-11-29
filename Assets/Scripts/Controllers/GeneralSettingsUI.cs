using UnityEngine;
using UnityEngine.UI;

public class GeneralSettingsUI : ExtendedMonoBehaviour
{
    InputField usernameInputField;

    void Awake()
    {
        usernameInputField = transform.Find("ChangeUsername")
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
        usernameInputField.text = username;
    }

    void OnDisable()
    {
        var username = usernameInputField.text;

        if (string.IsNullOrEmpty(username))
        {
            return;
        }

        PlayerPrefsEncryptionUtils.SetString("Username", username);
    }
}
