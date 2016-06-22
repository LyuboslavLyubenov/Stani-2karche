using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class EnterNameUIController : MonoBehaviour
{
    public GameObject UsernameTextField;
    public EventHandler OnUsernameSet = delegate
    {

    };

    void Start()
    {
        StartCoroutine(InitializationCoroutine());
    }

    IEnumerator InitializationCoroutine()
    {
        yield return null;

        if (PlayerPrefs.HasKey("Username"))
        {
            OnUsernameSet(this, EventArgs.Empty);
            this.gameObject.SetActive(false);
        }
    }

    public void SaveUsername()
    {
        var usernameText = UsernameTextField.GetComponent<Text>();
        PlayerPrefs.SetString("Username", usernameText.text);
        OnUsernameSet(this, EventArgs.Empty);
        this.gameObject.SetActive(false);
    }
}
