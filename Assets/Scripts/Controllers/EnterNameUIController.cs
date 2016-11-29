﻿using UnityEngine;
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

        CoroutineUtils.WaitForFrames(0, Initialize);
    }

    void Initialize()
    {
        if (!PlayerPrefsEncryptionUtils.HasKey("Username"))
        {
            var username = PlayerPrefsEncryptionUtils.GetString("Username");
            OnUsernameSet(this, new UserNameEventArgs(username));
            gameObject.SetActive(false);
        }
    }

    void Deactivate()
    {
        GetComponent<Animator>().SetTrigger("disable");
    }

    public void SaveUsername()
    {
        var usernameText = UsernameTextField.GetComponent<Text>();
        PlayerPrefsEncryptionUtils.SetString("Username", usernameText.text);
        Deactivate();
        OnUsernameSet(this, new UserNameEventArgs(usernameText.text));
    }
}