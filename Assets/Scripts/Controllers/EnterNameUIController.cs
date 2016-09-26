using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class EnterNameUIController : ExtendedMonoBehaviour
{
    public GameObject UsernameTextField;
    public EventHandler<UserNameEventArgs> OnUsernameSet = delegate
    {

    };

    void Start()
    {
        if (UsernameTextField == null)
        {
            throw new NullReferenceException("UsernameTextField is null on EnterNameUIController obj");
        }

        transform.localScale = new Vector3(0, 0, 0);

        CoroutineUtils.WaitForFrames(1, Initialize);
    }

    void Initialize()
    {
        if (PlayerPrefsEncryptionUtils.HasKey("Username"))
        {
            var username = PlayerPrefsEncryptionUtils.GetString("Username");
            OnUsernameSet(this, new UserNameEventArgs(username));
            Deactivate();
        }
    }

    void Deactivate()
    {
        GetComponent<Animator>().SetTrigger("disabled");
    }

    public void SaveUsername()
    {
        var usernameText = UsernameTextField.GetComponent<Text>();
        PlayerPrefsEncryptionUtils.SetString("Username", usernameText.text);
        OnUsernameSet(this, new UserNameEventArgs(usernameText.text));
        Deactivate();
    }
}