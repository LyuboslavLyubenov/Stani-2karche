using UnityEngine;
using UnityEngine.UI;

public class EnterNameUIController : MonoBehaviour
{
    public GameObject UsernameTextField;
    public GameObject ConnectSettingsUI;

    void Start()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            this.gameObject.SetActive(false);
            ConnectSettingsUI.SetActive(true);
        }
    }

    public void SaveUsername()
    {
        var usernameText = UsernameTextField.GetComponent<Text>();
        PlayerPrefs.SetString("Username", usernameText.text);
    }
}
