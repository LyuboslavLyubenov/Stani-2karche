using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class EnterNameUIController : ExtendedMonoBehaviour
{
    public GameObject UsernameTextField;
    public EventHandler OnUsernameSet = delegate
    {

    };

    void Start()
    {
        if (UsernameTextField == null)
        {
            throw new NullReferenceException("UsernameTextField is null on EnterNameUIController obj");
        }

        CoroutineUtils.WaitForFrames(1, Initialize);
    }

    void Initialize()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            OnUsernameSet(this, EventArgs.Empty);
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
        PlayerPrefs.SetString("Username", usernameText.text);
        OnUsernameSet(this, EventArgs.Empty);
        Deactivate();
    }
}
